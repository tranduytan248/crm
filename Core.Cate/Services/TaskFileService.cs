using Core.Cate.Caches;
using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Core.Cate.Services
{
    public class TaskFileService
    {
        private readonly string _folderImage = ConfigurationManager.AppSettings["AppImageRoot_Path"] ?? "/Contents/imgs";

        /// <summary>Đuôi file ảnh được phép.</summary>
        private static readonly HashSet<string> AllowedImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"
        };

        /// <summary>Đuôi file tài liệu được phép.</summary>
        private static readonly HashSet<string> AllowedDocumentExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".zip", ".rar"
        };

        /// <summary>Giới hạn kích thước file: 20MB.</summary>
        private const long MaxFileSizeBytes = 20 * 1024 * 1024;

        // Regex parse src ảnh từ HTML — dùng một lần, compile sẵn để tối ưu
        private static readonly Regex ImgSrcRegex = new Regex(
            @"<img[^>]+src\s*=\s*[""']([^""']+)[""']",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);


        private readonly RM_LogTaskCache _logTaskCache;

        public TaskFileService()
        {
            _logTaskCache = new RM_LogTaskCache();
        }

        // ── Upload ────────────────────────────────────────────────────────────

        /// <summary>
        /// Upload một file lên server, trả về đường dẫn tương đối (URL path).
        /// </summary>
        /// <param name="file">File cần upload.</param>
        /// <param name="subFolder">Thư mục con phân loại: "logtask" | "comment" | "task".</param>
        /// <returns>Đường dẫn URL tương đối, ví dụ: /Contents/uploads/task-files/comment/abc123.jpg</returns>
        public TaskFileUploadResult UploadFile(HttpPostedFileBase file, string subFolder = "task")
        {
            if (file == null || file.ContentLength == 0)
                return TaskFileUploadResult.Fail("File không hợp lệ hoặc rỗng.");

            if (file.ContentLength > MaxFileSizeBytes)
                return TaskFileUploadResult.Fail($"File vượt quá giới hạn {MaxFileSizeBytes / 1024 / 1024}MB.");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedImageExtensions.Contains(ext) && !AllowedDocumentExtensions.Contains(ext))
                return TaskFileUploadResult.Fail($"Định dạng file '{ext}' không được hỗ trợ.");

            // Xác định loại file
            var fileType = AllowedImageExtensions.Contains(ext)
                ? TaskFileType.Image
                : TaskFileType.Document;

            // Tạo tên file unique
            var fileName = $"{Guid.NewGuid()}{ext}";
            var relativeFolderPath = $"{_folderImage}/{subFolder}";
            var absoluteFolderPath = HttpContext.Current.Server.MapPath(relativeFolderPath);

            Directory.CreateDirectory(absoluteFolderPath);

            var absoluteFilePath = Path.Combine(absoluteFolderPath, fileName);
            file.SaveAs(absoluteFilePath);

            // URL trả về (bỏ ~)
            var urlPath = relativeFolderPath.TrimStart('~') + "/" + fileName;

            return TaskFileUploadResult.Ok(urlPath, file.FileName, fileType);
        }

        // ── Lưu vào DB ────────────────────────────────────────────────────────

        /// <summary>
        /// Lưu danh sách đường dẫn file vào bảng RM_LogTaskFilePath.
        /// </summary>
        public void SaveFilePaths(SaveFilePathRequest request, string username)
        {
            if (request == null || request.FilePaths == null || !request.FilePaths.Any()) return;

            foreach (var path in request.FilePaths.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                _logTaskCache.SaveFilePath(new RM_LogTaskFilePathModel
                {
                    FilePathID = 0,
                    LogTaskID = request.LogTaskID,
                    TaskManagementID = request.TaskManagementID,
                    CommentID = request.CommentID,
                    FilePath = path,
                    CreatedBy = username,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                }, username);
            }
        }

        /// <summary>
        /// Upload nhiều file cùng lúc rồi lưu ngay vào DB.
        /// </summary>
        public List<TaskFileUploadResult> UploadAndSave(
            IEnumerable<HttpPostedFileBase> files,
            SaveFilePathRequest dbRequest,
            string username,
            string subFolder = "task")
        {
            var results = new List<TaskFileUploadResult>();
            var savedPaths = new List<string>();

            foreach (var file in files ?? Enumerable.Empty<HttpPostedFileBase>())
            {
                var result = UploadFile(file, subFolder);
                results.Add(result);
                if (result.Success)
                    savedPaths.Add(result.UrlPath);
            }

            if (savedPaths.Any())
            {
                dbRequest.FilePaths = savedPaths;
                SaveFilePaths(dbRequest, username);
            }

            return results;
        }

        // ── Parse ảnh từ CKEditor ─────────────────────────────────────────────

        /// <summary>
        /// Parse tất cả src ảnh từ nội dung HTML của CKEditor.
        /// Chỉ lấy ảnh đã upload lên server (bỏ qua base64 và external URL).
        /// </summary>
        public List<string> ExtractImagePathsFromHtml(string htmlContent)
        {
            var paths = new List<string>();
            if (string.IsNullOrWhiteSpace(htmlContent)) return paths;

            var matches = ImgSrcRegex.Matches(htmlContent);
            foreach (Match match in matches)
            {
                var src = match.Groups[1].Value;
                // Chỉ lấy ảnh upload nội bộ: bắt đầu bằng / nhưng không phải //
                if (!string.IsNullOrEmpty(src) && src.StartsWith("/") && !src.StartsWith("//"))
                    paths.Add(src);
            }

            return paths;
        }

        /// <summary>
        /// Parse ảnh từ HTML rồi lưu vào DB.
        /// </summary>
        public void SaveImagePathsFromHtml(string htmlContent, SaveFilePathRequest dbRequest, string username)
        {
            var paths = ExtractImagePathsFromHtml(htmlContent);
            if (!paths.Any()) return;

            dbRequest.FilePaths = paths;
            SaveFilePaths(dbRequest, username);
        }

        // ── Xóa file ─────────────────────────────────────────────────────────

        /// <summary>
        /// Xóa file vật lý khỏi server theo URL path.
        /// </summary>
        public bool DeletePhysicalFile(string urlPath)
        {
            if (string.IsNullOrWhiteSpace(urlPath)) return false;
            try
            {
                var absolutePath = HttpContext.Current.Server.MapPath("~" + urlPath);
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                    return true;
                }
            }
            catch { /* log nếu cần */ }
            return false;
        }

        /// <summary>
        /// Xóa record file trong DB.
        /// </summary>
        public int DeleteFilePath(int filePathId, string username)
        {
            return _logTaskCache.DeleteFilePath(
                new RM_LogTaskFilePathModel { FilePathID = filePathId }, username);
        }

        // ── Query ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Lấy danh sách file theo LogTaskID hoặc TaskManagementID.
        /// </summary>
        public List<RM_LogTaskFilePathModel> GetFilePaths(int? logTaskId = null, int? taskManagementId = null, int? commentId = null)
        {
            int total;
            return _logTaskCache.GetFilePaths(new RM_LogTaskFilePathSearchModel
            {
                LogTaskID = logTaskId ?? 0,
                TaskManagementID = taskManagementId ?? 0,
                CommentID = commentId ?? 0
            }, out total);
        }

        /// <summary>
        /// Lấy danh sách file theo FilePathID.
        /// </summary>
        public RM_LogTaskFilePathModel GetFilePathById(int id)
        {
            return _logTaskCache.GetFilePathById(id);
        }
    }

    // ── DTOs ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Request lưu file path vào DB — điền ID tương ứng với nguồn gọi.
    /// </summary>
    public class SaveFilePathRequest
    {
        /// <summary>Điền khi lưu từ LogTask.</summary>
        public int LogTaskID { get; set; }

        /// <summary>Điền khi lưu từ TaskManagement.</summary>
        public int TaskManagementID { get; set; }

        /// <summary>Điền khi lưu từ Comment.</summary>
        public int CommentID { get; set; }

        /// <summary>Danh sách URL path các file cần lưu.</summary>
        public List<string> FilePaths { get; set; }
    }

    public enum TaskFileType { Image, Document }

    /// <summary>Kết quả upload một file.</summary>
    public class TaskFileUploadResult
    {
        public bool Success { get; private set; }
        public string UrlPath { get; private set; }
        public string OriginalFileName { get; private set; }
        public TaskFileType FileType { get; private set; }
        public string ErrorMessage { get; private set; }

        public static TaskFileUploadResult Ok(string urlPath, string originalName, TaskFileType type)
            => new TaskFileUploadResult { Success = true, UrlPath = urlPath, OriginalFileName = originalName, FileType = type };

        public static TaskFileUploadResult Fail(string error)
            => new TaskFileUploadResult { Success = false, ErrorMessage = error };
    }
}