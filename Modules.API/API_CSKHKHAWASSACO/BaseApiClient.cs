using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Processors;

public class BaseApiClient
{
    //private readonly string _authUrl = "https://vnpt.ctnkh.com.vn/Services/AuthenticationTokenService.svc/Authenticate";
    private readonly string _authUrl = ConfigurationManager.AppSettings["API_KHAWASSACO_AUTHURL"];
    private string _token;
    private readonly string _username =  ConfigurationManager.AppSettings["API_KHAWASSACO_USERNAME"];
    private readonly string _password =  ConfigurationManager.AppSettings["API_KHAWASSACO_PASSWORD"];

    private async Task<string> GetTokenAsync()
    {
        using (var client = new HttpClient())
        {
            var auth = Encoding.ASCII.GetBytes($"{_username}:{_password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(auth));

            var response = await client.PostAsync(_authUrl, null);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                //Loại bỏ dấu nháy ngoài cùng
                _token = result.Trim('"');

                //Nếu còn escape \/, thay bằng /
               _token = _token.Replace(@"\/", "/");

                return _token;
            }


            throw new Exception("Lỗi lấy token: " + result);
        }
    }

    private async Task EnsureTokenAsync()
    {
        if (string.IsNullOrEmpty(_token))
            await GetTokenAsync();
    }

    public async Task<string> GetAsync(string url)
    {
        await EnsureTokenAsync();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Token", _token);
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }

    public async Task<string> PostAsync(string url, string json)
    {
        await EnsureTokenAsync();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Token", _token);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
