using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using Core.Cate.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TSFramework.Libs.Models.Mail;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Providers;
using TSFramework.Libs.Utils;

namespace Core.Cate.Services
{
    /// <summary>
    /// Service dùng chung để quản lý và gửi email theo mẫu cấu hình trong database.
    /// </summary>
    public class MailTemplateService
    {
        #region Declaration

        private const string DataProviderName = "CenIT.Provider.Sys";
        private const string MailTemplateGetByIdProcedure = "Sys_MailTemplate_GetByID";
        private const string MailTemplateGetActiveByCodeProcedure = "Sys_MailTemplate_GetActiveByCode";
        private const string MailTemplateSaveProcedure = "Sys_MailTemplate_Save";
        private const string MailTemplateDeleteProcedure = "Sys_MailTemplate_Delete";
        private const string MailTemplateParamGetByTemplateIdProcedure = "Sys_MailTemplateParam_GetByTemplateId";
        private const string MailTemplateParamGetByTemplateCodeProcedure = "Sys_MailTemplateParam_GetByTemplateCode";
        private const string MailTemplateParamSaveProcedure = "Sys_MailTemplateParam_Save";
        private const string MailTemplateParamDeleteByTemplateIdProcedure = "Sys_MailTemplateParam_DeleteByTemplateId";
        private const string MailSendLogInsertProcedure = "Sys_MailSendLog_Insert";
        private const string MailSendLogUpdateStatusProcedure = "Sys_MailSendLog_UpdateStatus";
        private const string MailSendStatusSent = "Sent";
        private const string MailSendStatusFailed = "Failed";

        private static readonly Regex TemplateTokenRegex = new Regex(
            @"(?:{{\s*(?<ParamCode>[A-Za-z0-9_]+)\s*}})|(?:\$\$(?<ParamCode>[A-Za-z0-9_]+))",
            RegexOptions.Compiled);

        private static readonly Regex ModelTokenRegex = new Regex(
            @"(?:@)?Model\.(?:(?:GetString|GetBoolean|GetDateTime|GetValue)\(\s*[""'](?<ParamCode>[A-Za-z0-9_]+)[""'][^)]*\)|(?<ParamCode>[A-Za-z0-9_]+))",
            RegexOptions.Compiled);

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy danh sách tham số theo mã mẫu email.
        /// </summary>
        private List<SysMailTemplateParamModel> GetTemplateParams(string templateCode)
        {
            List<SysMailTemplateParamModel> templateParams =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMailTemplateParamModel>(
                    MailTemplateParamGetByTemplateCodeProcedure,
                    DataProviderName,
                    templateCode);

            return templateParams ?? new List<SysMailTemplateParamModel>();
        }


        /// <summary>
        /// Gửi email theo config key. Config key sẽ map sang mã template trong AppSettings.
        /// </summary>
        public bool SendByConfigKey(
            string templateConfigKey,
            string toEmail,
            string jsonData,
            string createdBy,
            List<string> ccEmails = null)
        {
            if (string.IsNullOrWhiteSpace(templateConfigKey))
            {
                AppProcessor.Logger.Message("Thiếu config key của mẫu email.");
                return false;
            }

            string templateCode = GetTemplateCodeFromConfig(templateConfigKey);
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                AppProcessor.Logger.Message(
                    string.Format("Không tìm thấy mã template trong AppSettings. Key: {0}", templateConfigKey));
                return false;
            }

            return SendByTemplateCode(
                templateCode,
                toEmail,
                jsonData,
                createdBy,
                ccEmails);
        }


        /// <summary>
        /// Gửi email trực tiếp theo mã template.
        /// </summary>
        public bool SendByTemplateCode(
            string templateCode,
            string toEmail,
            string jsonData,
            string createdBy,
            List<string> ccEmails = null)
        {
            int? mailSendLogId = null;
            string normalizedCreatedBy = NormalizeCreatedBy(createdBy);

            try
            {
                if (string.IsNullOrWhiteSpace(templateCode))
                {
                    AppProcessor.Logger.Message("Thiếu mã template email.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(toEmail) || !UtilString.IsValidEmail(toEmail))
                {
                    AppProcessor.Logger.Message("Email người nhận không hợp lệ.");
                    return false;
                }

                SysMailTemplateModel template = GetActiveTemplate(templateCode);
                if (template == null)
                {
                    AppProcessor.Logger.Message(
                        string.Format("Không tìm thấy mẫu email đang hoạt động. TemplateCode: {0}", templateCode));
                    return false;
                }

                List<SysMailTemplateParamModel> templateParams = GetTemplateParams(templateCode);
                Dictionary<string, object> normalizedParameters = NormalizeParameters(jsonData, templateParams);
                MailTemplateRenderModel renderModel = BuildRenderModel(templateCode, jsonData, normalizedParameters);

                string subject = RenderTemplateText(template.SubjectTemplate, normalizedParameters);
                string templateFilePath = ResolveTemplatePath(template.FilePath);
                if (string.IsNullOrWhiteSpace(templateFilePath) || !File.Exists(templateFilePath))
                {
                    AppProcessor.Logger.Message(
                        string.Format("Không tìm thấy file template email. FilePath: {0}", template.FilePath));
                    return false;
                }

                mailSendLogId = InsertMailSendLog(
                    template.TemplateCode,
                    subject,
                    toEmail,
                    normalizedParameters,
                    template.FilePath,
                    normalizedCreatedBy);

                string body = RenderTemplateHtmlProvider.RenderPartialToHtml(templateFilePath, renderModel);
                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new InvalidOperationException("Nội dung email render ra rỗng.");
                }

                List<MailModel> mailItems = new List<MailModel>
                {
                    new MailModel
                    {
                        Subject = subject,
                        To = new List<string> { toEmail },
                        Cc = ccEmails,
                        Body = body,
                        IsBodyHtml = true
                    }
                };

                AppProcessor.Mailer.PushEmail(mailItems);
                UpdateMailSendLogStatus(mailSendLogId, MailSendStatusSent, null, 0, normalizedCreatedBy);

                return true;
            }
            catch (Exception ex)
            {
                UpdateMailSendLogStatus(mailSendLogId, MailSendStatusFailed, ex.Message, 0, normalizedCreatedBy);
                AppProcessor.Logger.Error(ex);
                return false;
            }
        }


        /// <summary>
        /// Lấy dữ liệu mẫu email để hiển thị lên màn hình chỉnh sửa.
        /// </summary>
        public SysMailTemplateModel GetTemplateForEdit(int mailTemplateId)
        {
            SysMailTemplateModel template = GetTemplateById(mailTemplateId);
            if (template == null)
            {
                return null;
            }

            template.FilePath = NormalizeStoredFilePath(template.FilePath);
            template.TemplateContent = LoadTemplateContent(template.FilePath);
            template.TemplateParams = GetTemplateParamsByTemplateId(template.MailTemplateId);
            template.DetectedParamCodes = BuildDetectedParamCodes(template.TemplateParams);

            return template;
        }


        /// <summary>
        /// Lưu mẫu email và đồng bộ lại danh sách tham số theo subject và nội dung file.
        /// </summary>
        public int SaveTemplate(SysMailTemplateModel model, string updatedBy)
        {
            if (model == null)
            {
                return 0;
            }

            model.TemplateCode = NormalizeTemplateCode(model.TemplateCode);
            model.FilePath = BuildTemplatePath(model.TemplateCode, model.FilePath);

            string templateContent = GetTemplateContentForSave(model);
            SaveTemplateFile(model.FilePath, templateContent);

            int mailTemplateId = SaveTemplateRecord(model, updatedBy);
            if (mailTemplateId <= 0)
            {
                return mailTemplateId;
            }

            List<SysMailTemplateParamModel> templateParams = BuildTemplateParamsForSave(
                model.TemplateParams,
                mailTemplateId,
                model.SubjectTemplate,
                templateContent);

            DeleteTemplateParamsByTemplateId(mailTemplateId, updatedBy);

            for (int index = 0; index < templateParams.Count; index++)
            {
                SaveTemplateParam(templateParams[index], updatedBy);
            }

            return mailTemplateId;
        }


        /// <summary>
        /// Xóa mềm mẫu email.
        /// </summary>
        public int DeleteTemplate(int mailTemplateId, string updatedBy)
        {
            int? result = AppProcessor.ProcedureProvider.Execute(
                MailTemplateDeleteProcedure,
                DataProviderName,
                mailTemplateId,
                updatedBy);

            return result.GetValueOrDefault(0);
        }


        /// <summary>
        /// Nhận diện danh sách tham số từ subject và nội dung template.
        /// </summary>
        private List<SysMailTemplateParamModel> DetectTemplateParams(
            int mailTemplateId,
            string subjectTemplate,
            string templateContent)
        {
            HashSet<string> detectedParamCodes =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            List<SysMailTemplateParamModel> detectedParams =
                new List<SysMailTemplateParamModel>();

            Action<string> appendParam = delegate (string paramCode)
            {
                if (string.IsNullOrWhiteSpace(paramCode) || detectedParamCodes.Contains(paramCode))
                {
                    return;
                }

                detectedParamCodes.Add(paramCode);

                detectedParams.Add(new SysMailTemplateParamModel
                {
                    MailTemplateId = mailTemplateId,
                    ParamCode = paramCode,
                    ParamName = paramCode,
                    IsRequired = false,
                    DefaultValue = string.Empty
                });
            };

            CollectTemplateTokens(subjectTemplate, appendParam);
            CollectTemplateTokens(templateContent, appendParam);
            CollectModelTokens(templateContent, appendParam);

            return detectedParams;
        }

        #endregion

        #region Private Methods

        private string GetTemplateCodeFromConfig(string templateConfigKey)
        {
            string templateCode = ConfigurationManager.AppSettings[templateConfigKey];
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                return string.Empty;
            }

            return templateCode.Trim();
        }


        private SysMailTemplateModel GetActiveTemplate(string templateCode)
        {
            SysMailTemplateModel template = AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMailTemplateModel>(
                MailTemplateGetActiveByCodeProcedure,
                DataProviderName,
                templateCode);

            return template;
        }


        private SysMailTemplateModel GetTemplateById(int mailTemplateId)
        {
            SysMailTemplateModel template = AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMailTemplateModel>(
                MailTemplateGetByIdProcedure,
                DataProviderName,
                mailTemplateId);

            return template;
        }


        private List<SysMailTemplateParamModel> GetTemplateParamsByTemplateId(int mailTemplateId)
        {
            List<SysMailTemplateParamModel> templateParams =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMailTemplateParamModel>(
                    MailTemplateParamGetByTemplateIdProcedure,
                    DataProviderName,
                    mailTemplateId);

            return templateParams ?? new List<SysMailTemplateParamModel>();
        }


        private int SaveTemplateRecord(SysMailTemplateModel model, string updatedBy)
        {
            int? result = AppProcessor.ProcedureProvider.Execute(
                MailTemplateSaveProcedure,
                DataProviderName,
                model.MailTemplateId,
                model.TemplateCode,
                model.TemplateName,
                model.SubjectTemplate,
                model.FilePath,
                model.IsActive,
                model.Description,
                updatedBy);

            return result.GetValueOrDefault(0);
        }


        private int SaveTemplateParam(SysMailTemplateParamModel model, string updatedBy)
        {
            int? result = AppProcessor.ProcedureProvider.Execute(
                MailTemplateParamSaveProcedure,
                DataProviderName,
                model.MailTemplateParamId,
                model.MailTemplateId,
                model.ParamCode,
                model.ParamName,
                model.IsRequired,
                model.DefaultValue,
                updatedBy);

            return result.GetValueOrDefault(0);
        }


        private int DeleteTemplateParamsByTemplateId(int mailTemplateId, string updatedBy)
        {
            int? result = AppProcessor.ProcedureProvider.Execute(
                MailTemplateParamDeleteByTemplateIdProcedure,
                DataProviderName,
                mailTemplateId,
                updatedBy);

            return result.GetValueOrDefault(0);
        }


        private MailTemplateRenderModel BuildRenderModel(
            string templateCode,
            string jsonData,
            Dictionary<string, object> parameters)
        {
            MailTemplateRenderModel renderModel = new MailTemplateRenderModel
            {
                TemplateCode = templateCode
            };

            renderModel.SetJsonData(jsonData);
            renderModel.SetParameters(parameters);

            return renderModel;
        }


        private Dictionary<string, object> NormalizeParameters(
            string jsonData,
            IEnumerable<SysMailTemplateParamModel> templateParams)
        {
            Dictionary<string, object> normalizedParameters = ParseJsonDataToDictionary(jsonData);

            foreach (SysMailTemplateParamModel templateParam in templateParams)
            {
                if (HasParameterValue(normalizedParameters, templateParam.ParamCode))
                {
                    continue;
                }

                if (templateParam.IsRequired && string.IsNullOrWhiteSpace(templateParam.DefaultValue))
                {
                    throw new InvalidOperationException(
                        string.Format("Thiếu tham số bắt buộc của mẫu email: {0}", templateParam.ParamCode));
                }

                normalizedParameters[templateParam.ParamCode] = templateParam.DefaultValue ?? string.Empty;
            }

            return normalizedParameters;
        }


        private Dictionary<string, object> ParseJsonDataToDictionary(string jsonData)
        {
            Dictionary<string, object> parameters =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return parameters;
            }

            try
            {
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
                if (jsonObject == null)
                {
                    return parameters;
                }

                foreach (JProperty property in jsonObject.Properties())
                {
                    parameters[property.Name] = property.Value == null || property.Value.Type == JTokenType.Null
                        ? string.Empty
                        : property.Value.ToObject<object>();
                }

                return parameters;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                throw new InvalidOperationException("Dữ liệu JSON của mẫu email không hợp lệ.");
            }
        }


        private bool HasParameterValue(IDictionary<string, object> parameters, string paramCode)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(paramCode))
            {
                return false;
            }

            if (!parameters.ContainsKey(paramCode))
            {
                return false;
            }

            object value = parameters[paramCode];
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            return true;
        }


        private string RenderTemplateText(string templateText, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(templateText))
            {
                return string.Empty;
            }

            string renderedText = TemplateTokenRegex.Replace(
                templateText,
                delegate (Match match)
                {
                    string paramCode = match.Groups["ParamCode"].Value;
                    return GetParameterValueAsString(parameters, paramCode);
                });

            return renderedText.Trim();
        }


        private string GetParameterValueAsString(IDictionary<string, object> parameters, string paramCode)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(paramCode) || !parameters.ContainsKey(paramCode))
            {
                return string.Empty;
            }

            object value = parameters[paramCode];
            if (value == null)
            {
                return string.Empty;
            }

            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString("dd/MM/yyyy HH:mm");
            }

            DateTime parsedDateTimeValue;
            if (DateTime.TryParse(Convert.ToString(value), out parsedDateTimeValue))
            {
                return parsedDateTimeValue.ToString("dd/MM/yyyy HH:mm");
            }

            return Convert.ToString(value) ?? string.Empty;
        }


        private string ResolveTemplatePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }

            string virtualPath = filePath.Replace("\\", "/").Trim();
            if (!virtualPath.StartsWith("~/"))
            {
                virtualPath = string.Concat("~/", virtualPath.TrimStart('/'));
            }

            string templatePath = HostingEnvironment.MapPath(virtualPath);
            return templatePath ?? string.Empty;
        }


        private string NormalizeStoredFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            return filePath.Replace("\\", "/").TrimStart('~', '/');
        }


        private string BuildTemplatePath(string templateCode, string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                return NormalizeStoredFilePath(filePath);
            }

            return string.Format("Contents/Modules/Sys/EmailTemplates/{0}.cshtml", templateCode);
        }


        private string NormalizeTemplateCode(string templateCode)
        {
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                return string.Empty;
            }

            return templateCode.Trim().ToUpperInvariant();
        }


        private string GetTemplateContentForSave(SysMailTemplateModel model)
        {
            if (model.TemplateFile != null && model.TemplateFile.ContentLength > 0)
            {
                ValidateTemplateUpload(model.TemplateFile);

                using (StreamReader reader = new StreamReader(model.TemplateFile.InputStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            if (!string.IsNullOrWhiteSpace(model.TemplateContent))
            {
                return model.TemplateContent;
            }

            if (model.MailTemplateId > 0)
            {
                return LoadTemplateContent(model.FilePath);
            }

            return string.Empty;
        }


        private void ValidateTemplateUpload(HttpPostedFileBase templateFile)
        {
            string extension = Path.GetExtension(templateFile.FileName);
            string[] allowedExtensions = { ".cshtml" };

            if (!allowedExtensions.Contains((extension ?? string.Empty).ToLowerInvariant()))
            {
                throw new InvalidOperationException("Chỉ hỗ trợ import file mẫu email định dạng .cshtml.");
            }
        }


        private void SaveTemplateFile(string filePath, string templateContent)
        {
            string absolutePath = ResolveTemplatePath(filePath);
            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                throw new InvalidOperationException("Không xác định được đường dẫn file mẫu email.");
            }

            string folderPath = Path.GetDirectoryName(absolutePath);
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new InvalidOperationException("Không xác định được thư mục lưu file mẫu email.");
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllText(absolutePath, templateContent ?? string.Empty, Encoding.UTF8);
        }


        private string LoadTemplateContent(string filePath)
        {
            string absolutePath = ResolveTemplatePath(filePath);
            if (string.IsNullOrWhiteSpace(absolutePath) || !File.Exists(absolutePath))
            {
                return string.Empty;
            }

            return File.ReadAllText(absolutePath);
        }


        private string BuildDetectedParamCodes(IEnumerable<SysMailTemplateParamModel> templateParams)
        {
            return string.Join(
                ", ",
                (templateParams ?? new List<SysMailTemplateParamModel>())
                    .Select(item => item.ParamCode)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }


        private List<SysMailTemplateParamModel> BuildTemplateParamsForSave(
            IEnumerable<SysMailTemplateParamModel> templateParams,
            int mailTemplateId,
            string subjectTemplate,
            string templateContent)
        {
            List<SysMailTemplateParamModel> normalizedTemplateParams = NormalizeTemplateParamsForSave(
                templateParams,
                mailTemplateId);

            if (normalizedTemplateParams.Count > 0)
            {
                return normalizedTemplateParams;
            }

            return DetectTemplateParams(
                mailTemplateId,
                subjectTemplate,
                templateContent);
        }


        private List<SysMailTemplateParamModel> NormalizeTemplateParamsForSave(
            IEnumerable<SysMailTemplateParamModel> templateParams,
            int mailTemplateId)
        {
            List<SysMailTemplateParamModel> normalizedTemplateParams =
                new List<SysMailTemplateParamModel>();

            if (templateParams == null)
            {
                return normalizedTemplateParams;
            }

            foreach (SysMailTemplateParamModel templateParam in templateParams)
            {
                if (templateParam == null || string.IsNullOrWhiteSpace(templateParam.ParamCode))
                {
                    continue;
                }

                SysMailTemplateParamModel normalizedTemplateParam = new SysMailTemplateParamModel
                {
                    MailTemplateId = mailTemplateId,
                    ParamCode = templateParam.ParamCode.Trim(),
                    ParamName = string.IsNullOrWhiteSpace(templateParam.ParamName)
                        ? templateParam.ParamCode.Trim()
                        : templateParam.ParamName.Trim(),
                    IsRequired = templateParam.IsRequired,
                    DefaultValue = templateParam.DefaultValue ?? string.Empty
                };

                normalizedTemplateParams.Add(normalizedTemplateParam);
            }

            return normalizedTemplateParams;
        }


        private void CollectTemplateTokens(string templateText, Action<string> appendParam)
        {
            if (string.IsNullOrWhiteSpace(templateText))
            {
                return;
            }

            foreach (Match match in TemplateTokenRegex.Matches(templateText))
            {
                appendParam(match.Groups["ParamCode"].Value);
            }
        }


        private void CollectModelTokens(string templateText, Action<string> appendParam)
        {
            if (string.IsNullOrWhiteSpace(templateText))
            {
                return;
            }

            foreach (Match match in ModelTokenRegex.Matches(templateText))
            {
                appendParam(match.Groups["ParamCode"].Value);
            }
        }


        private int? InsertMailSendLog(
            string templateCode,
            string mailSubject,
            string toEmail,
            IDictionary<string, object> parameters,
            string templateFilePath,
            string createdBy)
        {
            string paramJson = JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>());

            object mailSendLogIdValue = AppProcessor.ProcedureProvider.ExecuteScalar(
                MailSendLogInsertProcedure,
                DataProviderName,
                templateCode,
                mailSubject,
                toEmail,
                paramJson,
                templateFilePath,
                createdBy);

            if (mailSendLogIdValue == null || mailSendLogIdValue == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(mailSendLogIdValue);
        }


        private void UpdateMailSendLogStatus(
            int? mailSendLogId,
            string sendStatus,
            string errorMessage,
            int retryCount,
            string updatedBy)
        {
            if (!mailSendLogId.HasValue || mailSendLogId.Value <= 0)
            {
                return;
            }

            AppProcessor.ProcedureProvider.Execute(
                MailSendLogUpdateStatusProcedure,
                DataProviderName,
                mailSendLogId.Value,
                sendStatus,
                errorMessage,
                retryCount,
                updatedBy);
        }


        private string NormalizeCreatedBy(string createdBy)
        {
            if (!string.IsNullOrWhiteSpace(createdBy))
            {
                return createdBy.Trim();
            }

            return "SYSTEM";
        }
        #endregion
    }
}
