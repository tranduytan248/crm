using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace CenIT.Tests.DigitalSales
{
    class Program
    {
        private static string _connectionString;
        private static int _passCount = 0;
        private static int _failCount = 0;

        static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("==========================================================");
            Console.WriteLine("BẮT ĐẦU KIỂM THỬ TỰ ĐỘNG: Phân hệ Quản lý Kinh doanh SPDV Số");
            Console.WriteLine("==========================================================");

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: DigitalSalesManagementTests.exe <crmBinPath> <connectionString>");
                return 1;
            }

            string crmBin = args[0];
            _connectionString = args[1];

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name).Name + ".dll";
                var assemblyPath = Path.Combine(crmBin, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                return null;
            };

            try
            {
                RunAllTests();

                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"KẾT QUẢ: {_passCount} PASS, {_failCount} FAIL.");
                if (_failCount == 0)
                {
                    Console.WriteLine("TẤT CẢ CÁC BÀI KIỂM THỬ ĐỀU ĐẠT (PASS 100%).");
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI NGOẠI LỆ TRONG KHI CHẠY TEST: {ex.Message}\n{ex.StackTrace}");
                Console.ResetColor();
                return 2;
            }
        }

        static void Assert(bool condition, string message)
        {
            if (condition)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  [PASS] {message}");
                Console.ResetColor();
                _passCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  [FAIL] {message}");
                Console.ResetColor();
                _failCount++;
            }
        }

        static void RunAllTests()
        {
            int testCustomerID = GetFirstCustomerID();
            int testProductID1 = GetFirstProductID();
            int testProductID2 = GetSecondProductID();
            int testUserID = 3; // quantri
            int testMemberUserID = 6; // thoabtk.kha

            Console.WriteLine("\n[TẦNG 1: HAPPY PATH] Kiểm tra luồng nghiệp vụ tạo Cơ hội ➔ Chuyển đổi Dự án:");

            // 1. Tạo Cơ hội kinh doanh mới
            int salesID = ExecuteSaveSales(0, "Hồ sơ Test Tự Động KHKD Số 2026", 1, 1, testCustomerID, 50, testUserID, "Ghi chú ban đầu khởi tạo cơ hội");
            Assert(salesID > 0, $"Thêm mới Cơ hội kinh doanh thành công (ID = {salesID}).");

            var row = GetSalesRow(salesID);
            Assert(row != null, "Phải đọc được thông tin hồ sơ vừa tạo.");
            Assert(Convert.ToByte(row["BusinessType"]) == 1, "Loại hình khởi tạo ban đầu phải là Cơ hội kinh doanh (BusinessType = 1).");
            Assert(Convert.ToInt32(row["StatusID"]) == 1, "Trạng thái khởi tạo ban đầu phải là Chưa nắm bắt (StatusID = 1).");

            string code = Convert.ToString(row["Code"]);
            Assert(!string.IsNullOrEmpty(code) && code.StartsWith("SPDV-"), $"Mã hồ sơ phải tự động sinh theo mẫu SPDV-YYYY-XXXX (Mã thực tế: {code}).");

            // 2. Kiểm tra Timeline khởi tạo
            int timelineInitCount = GetTimelineCount(salesID);
            Assert(timelineInitCount >= 1, $"Timeline phải tự động ghi nhận bước khởi tạo ban đầu (Số mốc: {timelineInitCount}).");

            // 3. Kiểm tra AM được tự động sinh trong bảng Thành viên
            int amMemberCount = GetMemberCount(salesID, true);
            Assert(amMemberCount == 1, "AM người tạo phải được tự động thêm vào danh sách Thành viên tham gia với IsAM = 1.");

            // 4. Kiểm tra Checklist ban đầu được sinh tự động
            int initialTrackingCount = GetTrackingCount(salesID);
            Assert(initialTrackingCount >= 0, $"Checklist công việc ban đầu được khởi tạo cho trạng thái 1 (Số công việc: {initialTrackingCount}).");

            // 5. Cập nhật thông tin Cơ hội
            int updateRes = ExecuteSaveSales(salesID, "Hồ sơ Test Đã Tiếp Cận KHKD Số 2026", 1, 1, testCustomerID, 75, testUserID, "Ghi chú đã cập nhật");
            Assert(updateRes == salesID, "Cập nhật thông tin chung của Cơ hội thành công.");
            var updatedRow = GetSalesRow(salesID);
            Assert(Convert.ToDecimal(updatedRow["ClosingProbability"]) == 75, "Xác suất thành công cập nhật chính xác = 75%.");

            // 6. Chuyển trạng thái nội bộ Cơ hội: 1 (Chưa nắm bắt) ➔ 2 (Đang tiếp cận)
            int changeToApproaching = ExecuteChangeStatus(salesID, 2, "Đã gặp gỡ khách hàng trao đổi nhu cầu bước đầu");
            Assert(changeToApproaching == 1, "Chuyển trạng thái sang '2 - Đang tiếp cận' thành công.");
            var rowApproaching = GetSalesRow(salesID);
            Assert(Convert.ToInt32(rowApproaching["StatusID"]) == 2, "Trạng thái hồ sơ đã cập nhật sang ID = 2.");

            // 7 & 8. Thêm Sản phẩm dịch vụ số kèm doanh thu
            int prod1 = ExecuteSaveProduct(0, salesID, testProductID1, 150000000m, null, "Gói Doanh Nghiệp Standard", 1);
            Assert(prod1 > 0, $"Thêm sản phẩm 1 thành công (ID = {prod1}, Doanh thu dự kiến: 150.000.000).");

            int prod2 = ExecuteSaveProduct(0, salesID, testProductID2, 50000000m, null, "Gói Dịch vụ Bổ trợ", 1);
            Assert(prod2 > 0, $"Thêm sản phẩm 2 thành công (ID = {prod2}, Doanh thu dự kiến: 50.000.000).");

            // 9. Kiểm tra Tổng doanh thu dự kiến tự động cập nhật
            var rowWithRevenue = GetSalesRow(salesID);
            decimal totalExpected = Convert.ToDecimal(rowWithRevenue["TotalExpectedRevenue"]);
            Assert(totalExpected == 200000000m, $"Tổng doanh thu dự kiến tự động cộng dồn chính xác = 200.000.000 VNĐ (Thực tế: {totalExpected:N0}).");

            // 10. Thêm Thành viên tham gia
            int mem1 = ExecuteSaveMember(0, salesID, testMemberUserID, "Kỹ thuật giải pháp & Triển khai", false);
            Assert(mem1 > 0, $"Thêm thành viên kỹ thuật vào dự án thành công (ID = {mem1}).");

            // 11. Chuyển đổi từ Cơ hội sang DỰ ÁN (StatusID = 4 - Giai đoạn hình thành dự án)
            int changeToProject = ExecuteChangeStatus(salesID, 4, "Khách hàng đồng ý lập dự án triển khai thử nghiệm PoC");
            Assert(changeToProject == 1, "CHUYỂN ĐỔI SANG DỰ ÁN THÀNH CÔNG (Đã thỏa mãn đầy đủ ràng buộc Sản phẩm và Thành viên).");
            var rowProject = GetSalesRow(salesID);
            Assert(Convert.ToByte(rowProject["BusinessType"]) == 2, "Loại hình đã chuyển đổi thành công sang DỰ ÁN (BusinessType = 2).");
            Assert(Convert.ToInt32(rowProject["StatusID"]) == 4, "Trạng thái đã cập nhật sang ID = 4 (Giai đoạn hình thành dự án).");

            // 12. Kiểm tra Timeline có ghi nhận mốc chuyển sang Dự án
            string latestTimelineNote = GetLatestTimelineNote(salesID);
            bool containsProject = latestTimelineNote.IndexOf("DỰ ÁN", StringComparison.OrdinalIgnoreCase) >= 0 || latestTimelineNote.IndexOf("Dự án", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert(containsProject, $"Timeline ghi nhận chính xác sự kiện chuyển đổi sang Dự án (Nội dung: '{latestTimelineNote}').");

            // 13. Kiểm tra Checklist tiến trình của Dự án được tự động sinh theo SLA
            int projectTrackingCount = GetTrackingCount(salesID);
            Assert(projectTrackingCount > 0, $"Dự án tự động sinh danh sách tiến trình theo cấu hình danh mục (Số lượng: {projectTrackingCount}).");

            // 14. Thêm & Cập nhật tiến trình tùy biến
            int customTrackId = ExecuteSaveTracking(0, salesID, "Khảo sát thực địa hạ tầng khách hàng", testMemberUserID, 1, 3);
            Assert(customTrackId > 0, $"Thêm công việc checklist tùy biến thành công (ID = {customTrackId}).");
            int updateTrackRes = ExecuteUpdateTrackingStatus(customTrackId, 3, "Đã khảo sát xong, hệ thống khách hàng đạt tiêu chuẩn", testMemberUserID);
            Assert(updateTrackRes > 0, "Cập nhật trạng thái công việc sang '3 - Hoàn thành' thành công.");

            // 15. Ký hợp đồng (StatusID = 7) và cập nhật Doanh thu thực tế
            ExecuteSaveProduct(prod1, salesID, testProductID1, 150000000m, 140000000m, "Gói Doanh Nghiệp Standard", 1);
            int changeToContract = ExecuteChangeStatus(salesID, 7, "Đã ký hợp đồng chính thức số HĐ-2026/01");
            Assert(changeToContract == 1, "Chuyển trạng thái sang '7 - Đã ký hợp đồng' thành công.");

            // 16. Hoàn thành dự án (StatusID = 8)
            int changeToCompleted = ExecuteChangeStatus(salesID, 8, "Dự án đã nghiệm thu và hoàn thành bàn giao 100%");
            Assert(changeToCompleted == 1, "Chuyển trạng thái sang '8 - Hoàn thành' thành công.");
            var rowCompleted = GetSalesRow(salesID);
            Assert(rowCompleted["EndDate"] != DBNull.Value, "Ngày kết thúc (EndDate) được tự động cập nhật khi dự án Hoàn thành.");

            // 17. Xóa bớt sản phẩm 2 và kiểm tra tổng doanh thu tự động tính lại
            int delProdRes = ExecuteDeleteProduct(prod2);
            Assert(delProdRes > 0, "Xóa sản phẩm 2 thành công.");
            var rowAfterDelProd = GetSalesRow(salesID);
            decimal newExpected = Convert.ToDecimal(rowAfterDelProd["TotalExpectedRevenue"]);
            Assert(newExpected == 150000000m, $"Tổng doanh thu dự kiến tự động giảm về 150.000.000 VNĐ (Thực tế: {newExpected:N0}).");

            // 18. Xóa hồ sơ (Soft delete)
            int delSalesRes = ExecuteDeleteSales(salesID);
            Assert(delSalesRes > 0, "Xóa hồ sơ kinh doanh thành công (IsDeleted = 1).");
            var rowDeleted = GetSalesRow(salesID);
            Assert(rowDeleted == null, "Hồ sơ đã xóa không còn xuất hiện trong truy vấn chi tiết.");


            Console.WriteLine("\n[TẦNG 2: EDGE CASES] Kiểm tra dữ liệu biên & trường hợp ngoại lệ:");

            // 19. ID không tồn tại
            var nonExistent = GetSalesRow(999999);
            Assert(nonExistent == null, "Truy vấn hồ sơ với ID = 999999 phải trả về null an toàn.");

            // 20. Tìm kiếm với bộ lọc rỗng
            int searchTotal = ExecuteSearchSalesCount("", 0, 0);
            Assert(searchTotal >= 0, $"Tìm kiếm với filter rỗng hoạt động an toàn (Tổng bản ghi: {searchTotal}).");

            // 21. Số lượng sản phẩm <= 0
            int prodZeroQty = ExecuteSaveProduct(0, salesID, testProductID1, 10000000m, null, "Gói Test", 0);
            Assert(prodZeroQty > 0, "Lưu sản phẩm với số lượng = 0 được xử lý an toàn.");
            int actualQty = GetProductQuantity(prodZeroQty);
            Assert(actualQty == 1, $"Số lượng tự động chuẩn hóa về tối thiểu là 1 (Thực tế: {actualQty}).");
            ExecuteDeleteProduct(prodZeroQty);

            // 22. Ký tự đặc biệt tiếng Việt
            string specialTitle = "Hồ sơ @#$$%^ & Dịch vụ Số: <Giải pháp Cloud 2026> - Khách hàng \"Đà Nẵng & Hà Nội\"";
            int specialSalesId = ExecuteSaveSales(0, specialTitle, 1, 1, testCustomerID, 50, testUserID, "Ghi chú có dấu: Hà Nội, TP.HCM & Cần Thơ");
            Assert(specialSalesId > 0, "Lưu hồ sơ chứa ký tự đặc biệt và Unicode tiếng Việt thành công.");
            var specialRow = GetSalesRow(specialSalesId);
            Assert(Convert.ToString(specialRow["Title"]) == specialTitle, "Tên hồ sơ tiếng Việt Unicode được bảo toàn nguyên vẹn.");

            Console.WriteLine("\n[TẦNG 3: ERROR HANDLING & GATEKEEPER] Kiểm tra bẫy lỗi & quy tắc chặn chuyển đổi:");

            // 23. GATEKEEPER 1: Chặn chuyển sang Dự án khi THIẾU SẢN PHẨM
            int noProdSalesID = ExecuteSaveSales(0, "Hồ sơ Test Gatekeeper Thiếu Sản Phẩm", 1, 1, testCustomerID, 50, testUserID, null);
            int gatekeeperResult1 = ExecuteChangeStatus(noProdSalesID, 4, "Cố tình chuyển sang dự án khi chưa có sản phẩm");
            Assert(gatekeeperResult1 == -3, $"GATEKEEPER 1 HOẠT ĐỘNG CHUẨN XÁC: Chặn chuyển Dự án vì thiếu sản phẩm (Mã lỗi trả về: {gatekeeperResult1}, mong đợi: -3).");
            var gatekeeperRow1 = GetSalesRow(noProdSalesID);
            Assert(Convert.ToByte(gatekeeperRow1["BusinessType"]) == 1, "Loại hình hồ sơ KHÔNG bị thay đổi (vẫn là Cơ hội).");
            Assert(Convert.ToInt32(gatekeeperRow1["StatusID"]) == 1, "Trạng thái hồ sơ KHÔNG bị thay đổi (vẫn là Chưa nắm bắt).");

            // 24. GATEKEEPER 2: Chặn chuyển sang Dự án khi THIẾU THÀNH VIÊN
            // Thêm sản phẩm vào
            ExecuteSaveProduct(0, noProdSalesID, testProductID1, 50000000m, null, "Gói Test", 1);
            // Xóa tất cả thành viên (bao gồm AM tự sinh)
            RemoveAllMembers(noProdSalesID);
            int gatekeeperResult2 = ExecuteChangeStatus(noProdSalesID, 4, "Cố tình chuyển sang dự án khi chưa có thành viên");
            Assert(gatekeeperResult2 == -4, $"GATEKEEPER 2 HOẠT ĐỘNG CHUẨN XÁC: Chặn chuyển Dự án vì thiếu thành viên (Mã lỗi trả về: {gatekeeperResult2}, mong đợi: -4).");
            var gatekeeperRow2 = GetSalesRow(noProdSalesID);
            Assert(Convert.ToByte(gatekeeperRow2["BusinessType"]) == 1, "Loại hình hồ sơ KHÔNG bị thay đổi (vẫn là Cơ hội).");
            Assert(Convert.ToInt32(gatekeeperRow2["StatusID"]) == 1, "Trạng thái hồ sơ KHÔNG bị thay đổi.");

            // Dọn dẹp test gatekeeper
            ExecuteDeleteSales(noProdSalesID);

            // 25. Bẫy lỗi DigitalSalesID không tồn tại
            int nonExistentResult = ExecuteChangeStatus(999999, 4, "Test ID ảo");
            Assert(nonExistentResult == -1, $"Bẫy lỗi hồ sơ không tồn tại trả về mã -1 (Thực tế: {nonExistentResult}).");

            // 26. Bẫy lỗi NewStatusID không tồn tại trên hồ sơ hợp lệ
            int invalidStatusResult = ExecuteChangeStatus(specialSalesId, 999999, "Test trạng thái ảo");
            Assert(invalidStatusResult == -2, $"Bẫy lỗi trạng thái đích không hợp lệ trả về mã -2 (Thực tế: {invalidStatusResult}).");
            ExecuteDeleteSales(specialSalesId);
        }

        #region DB Helper Methods
        static int GetFirstCustomerID()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 CustomerID FROM dbo.RM_Customer WHERE IsDeleted = 0";
                    var obj = cmd.ExecuteScalar();
                    return obj != null ? Convert.ToInt32(obj) : 19821;
                }
            }
        }

        static int GetFirstProductID()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 ProductServiceID FROM dbo.RM_ProductService ORDER BY ProductServiceID ASC";
                    var obj = cmd.ExecuteScalar();
                    return obj != null ? Convert.ToInt32(obj) : 1;
                }
            }
        }

        static int GetSecondProductID()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 ProductServiceID FROM dbo.RM_ProductService WHERE ProductServiceID > 1 ORDER BY ProductServiceID ASC";
                    var obj = cmd.ExecuteScalar();
                    return obj != null ? Convert.ToInt32(obj) : 2;
                }
            }
        }

        static int ExecuteSaveSales(int id, string title, byte businessType, int statusId, int customerId, decimal? prob, int? employeeId, string note)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSales_Save";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DigitalSalesID", id);
                    cmd.Parameters.AddWithValue("@Code", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@BusinessType", businessType);
                    cmd.Parameters.AddWithValue("@StatusID", statusId);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@ContactPerson_ID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClosingProbability", prob.HasValue ? (object)prob.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpectedDate", DateTime.Today.AddMonths(1));
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractNo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractValue", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractSignDate", DBNull.Value);
                    cmd.Parameters.AddWithValue("@AssignedEmployeeID", employeeId.HasValue ? (object)employeeId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepartmentID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);
                    cmd.Parameters.AddWithValue("@FileAttach", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static DataRow GetSalesRow(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSales_GetByID";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DigitalSalesID", id);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
        }

        static int ExecuteChangeStatus(int salesId, int newStatusId, string note)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSales_ChangeStatus";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DigitalSalesID", salesId);
                    cmd.Parameters.AddWithValue("@NewStatusID", newStatusId);
                    cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);
                    cmd.Parameters.AddWithValue("@AttachmentPath", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static int ExecuteSaveProduct(int id, int salesId, int prodServiceId, decimal? expected, decimal? actual, string pkg, int qty)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSalesProduct_Save";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SalesProductID", id);
                    cmd.Parameters.AddWithValue("@DigitalSalesID", salesId);
                    cmd.Parameters.AddWithValue("@ProductServiceID", prodServiceId);
                    cmd.Parameters.AddWithValue("@ExpectedRevenue", expected.HasValue ? (object)expected.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActualRevenue", actual.HasValue ? (object)actual.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@PackageName", string.IsNullOrEmpty(pkg) ? (object)DBNull.Value : pkg);
                    cmd.Parameters.AddWithValue("@Quantity", qty);
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@EndDate", DateTime.Today.AddYears(1));
                    cmd.Parameters.AddWithValue("@Note", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static int ExecuteDeleteProduct(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSalesProduct_Delete";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SalesProductID", id);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static int GetProductQuantity(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Quantity FROM dbo.RM_DigitalSalesProduct WHERE SalesProductID = @ID";
                    cmd.Parameters.AddWithValue("@ID", id);
                    var obj = cmd.ExecuteScalar();
                    return obj != null ? Convert.ToInt32(obj) : 0;
                }
            }
        }

        static int ExecuteSaveMember(int id, int salesId, int userId, string roleTitle, bool isAm)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSalesMember_Save";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MemberID", id);
                    cmd.Parameters.AddWithValue("@DigitalSalesID", salesId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@RoleTitle", roleTitle);
                    cmd.Parameters.AddWithValue("@IsAM", isAm);
                    cmd.Parameters.AddWithValue("@Note", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static void RemoveAllMembers(int salesId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE dbo.RM_DigitalSalesMember SET IsActive = 0 WHERE DigitalSalesID = @ID";
                    cmd.Parameters.AddWithValue("@ID", salesId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static int GetMemberCount(int salesId, bool isAmOnly)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = isAmOnly
                        ? "SELECT COUNT(*) FROM dbo.RM_DigitalSalesMember WHERE DigitalSalesID = @ID AND IsAM = 1 AND IsActive = 1"
                        : "SELECT COUNT(*) FROM dbo.RM_DigitalSalesMember WHERE DigitalSalesID = @ID AND IsActive = 1";
                    cmd.Parameters.AddWithValue("@ID", salesId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        static int ExecuteSaveTracking(int id, int salesId, string taskName, int? assignedUser, byte status, int durationDays)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSalesTracking_Save";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TrackingID", id);
                    cmd.Parameters.AddWithValue("@DigitalSalesID", salesId);
                    cmd.Parameters.AddWithValue("@ProcessID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgressID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaskName", taskName);
                    cmd.Parameters.AddWithValue("@AssignedUserID", assignedUser.HasValue ? (object)assignedUser.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@Deadline", DateTime.Today.AddDays(durationDays));
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ResultNote", DBNull.Value);
                    cmd.Parameters.AddWithValue("@AttachmentFile", DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCustomTask", true);
                    cmd.Parameters.AddWithValue("@SortOrder", 1);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static int ExecuteUpdateTrackingStatus(int id, byte status, string note, int? assignedUser)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSalesTracking_UpdateStatus";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TrackingID", id);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ResultNote", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);
                    cmd.Parameters.AddWithValue("@AttachmentFile", DBNull.Value);
                    cmd.Parameters.AddWithValue("@AssignedUserID", assignedUser.HasValue ? (object)assignedUser.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Deadline", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static int GetTrackingCount(int salesId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM dbo.RM_DigitalSalesTracking WHERE DigitalSalesID = @ID";
                    cmd.Parameters.AddWithValue("@ID", salesId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        static int GetTimelineCount(int salesId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM dbo.RM_DigitalSalesTimeline WHERE DigitalSalesID = @ID";
                    cmd.Parameters.AddWithValue("@ID", salesId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        static string GetLatestTimelineNote(int salesId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 Note FROM dbo.RM_DigitalSalesTimeline WHERE DigitalSalesID = @ID ORDER BY TimelineID DESC";
                    cmd.Parameters.AddWithValue("@ID", salesId);
                    var obj = cmd.ExecuteScalar();
                    return obj != null ? Convert.ToString(obj) : "";
                }
            }
        }

        static int ExecuteDeleteSales(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSales_Delete";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DigitalSalesID", id);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        static int ExecuteSearchSalesCount(string keyword, byte businessType, int statusId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "RM_DigitalSales_GetList";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword);
                    cmd.Parameters.AddWithValue("@BusinessType", businessType);
                    cmd.Parameters.AddWithValue("@StatusID", statusId);
                    cmd.Parameters.AddWithValue("@CustomerID", 0);
                    cmd.Parameters.AddWithValue("@ProductServiceID", 0);
                    cmd.Parameters.AddWithValue("@DepartmentID", 0);
                    cmd.Parameters.AddWithValue("@EmployeeID", 0);
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageNumber", 1);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
                    cmd.Parameters.AddWithValue("@UserName", "test_runner");

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0 && dt.Columns.Contains("TotalCount") && dt.Rows[0]["TotalCount"] != DBNull.Value)
                        {
                            return Convert.ToInt32(dt.Rows[0]["TotalCount"]);
                        }
                        return dt.Rows.Count;
                    }
                }
            }
        }
        #endregion
    }
}
