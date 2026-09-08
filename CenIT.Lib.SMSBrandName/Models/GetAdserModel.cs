using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetAdserModel
    {
        public DataRequestGetAdserModel RQST { get; set; } = new DataRequestGetAdserModel();
    }

    public class ResponseGetAdser
    {
        public DataResponseGetAdser RPLY { get; set; }
    }

    public class DataRequestGetAdserModel
    {
        public string name { get; set; } = "get_adser";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseGetAdser
    {
        public string name { get; set; } = "get_adser";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
        public GetAdserDetail ADSERDETAIL { get; set; } = new GetAdserDetail();
    }

    public class GetAdserDetail
    {
        //ID của khách hàng
        public int ADSERID { get; set; } = 0;
        //Tên khách hàng
        public string ADSERNAME { get; set; } = "";
        //Địa chỉ khách hàng
        public string ADSERADDR { get; set; } = "";
        //Số giấy tờ của khách hàng
        public string ADSERPAPER { get; set; } = "";
        //Điện thoại liên hệ của khách hàng
        public string ADSERMOBILE { get; set; } = "";
        //Email của khách hàng
        public string ADSEREMAIL { get; set; } = "";
    }
}
