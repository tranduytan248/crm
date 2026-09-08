using System;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// Lớp VNPTApiService: Cung cấp các hàm nghiệp vụ để giao tiếp với hệ thống VNPT Water (ARService)
/// Mỗi hàm gọi đến API tương ứng thông qua lớp BaseApiClient.
/// </summary>
public class KWC_ApiService
{
    // Đối tượng client dùng chung cho các lời gọi HTTP
    private readonly BaseApiClient _client = new BaseApiClient();

    // Base URL cho các API nghiệp vụ
    private readonly string _baseApiUrl = ConfigurationManager.AppSettings["API_KHAWASSACO_APIURL"];
    private readonly string _secureKey = ConfigurationManager.AppSettings["API_KHAWASSACO_SECUREKEY"];

    /// <summary>
    /// Lấy thông tin nợ của khách hàng dựa theo mã hợp đồng.
    /// </summary>
    /// <param name="contractCode">Mã hợp đồng khách hàng (ContractCode)</param>
    /// <returns>ApiResult chứa dữ liệu JSON trả về từ API</returns>
    public async Task<ApiResult> GetDebtInfoAsync(string contractCode)
    {
        // Tạo URL gọi API
        var url = $"{_baseApiUrl}ARByCustomer/{contractCode}/{_secureKey}";

        // Gọi API GET thông qua BaseApiClient
        var result = await _client.GetAsync(url);

        // Trả về kết quả chung
        return new ApiResult { Success = true, Data = result };
    }

    /// <summary>
    /// Gửi yêu cầu xác nhận thanh toán (gạch nợ) cho hợp đồng nước.
    /// </summary>
    /// <param name="req">Đối tượng DebtRequest chứa thông tin giao dịch</param>
    /// <returns>ApiResult chứa phản hồi từ API VNPT</returns>
    public async Task<ApiResult> PayInvoiceAsync(DebtRequest req)
    {
        // API gạch nợ (ARCheckDebt)
        var url = $"{_baseApiUrl}ARCheckDebt/VNPTMONEY/{_secureKey}/";

        // Chuyển đối tượng sang JSON
        string json = JsonConvert.SerializeObject(req);

        // Gửi POST request
        var result = await _client.PostAsync(url, json);

        // Trả kết quả
        return new ApiResult { Success = true, Data = result };
    }

    /// <summary>
    /// Lấy thông tin chi tiết khách hàng theo mã hợp đồng.
    /// </summary>
    /// <param name="contractCode">Mã hợp đồng khách hàng</param>
    /// <returns>ApiResult chứa dữ liệu JSON khách hàng</returns>
    public async Task<ApiResult> GetCustomerInfoAsync(string contractCode)
    {
        var url = $"{_baseApiUrl}GetCustomerInfo/{contractCode}/{_secureKey}";
        var result = await _client.GetAsync(url);
        return new ApiResult { Success = true, Data = result };
    }

    /// <summary>
    /// Lấy danh sách công nợ trong khoảng thời gian (từ ngày - đến ngày).
    /// </summary>
    /// <param name="from">Ngày bắt đầu</param>
    /// <param name="to">Ngày kết thúc</param>
    /// <returns>ApiResult chứa danh sách nợ trong khoảng thời gian</returns>
    public async Task<ApiResult> GetDebtByDateAsync(DateTime from, DateTime to)
    {
        var url = $"{_baseApiUrl}ARByDate/VNPTMONEY/{from:yyyy-MM-dd}/{to:yyyy-MM-dd}/{_secureKey}";
        var result = await _client.GetAsync(url);
        return new ApiResult { Success = true, Data = result };
    }
}
