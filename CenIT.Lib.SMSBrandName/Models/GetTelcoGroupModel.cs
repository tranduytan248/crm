using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetTelcoGroupModel
    {
        public DataRequestGetTelcoGroupModel RQST { get; set; } = new DataRequestGetTelcoGroupModel();
    }

    public class ResponseGetTelcoGroup
    {
        public DataResponseGetTelcoGroup RPLY { get; set; }
    }

    public class DataRequestGetTelcoGroupModel
    {
        public string name { get; set; } = "get_telco_group";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseGetTelcoGroup
    {
        public string name { get; set; } = "get_telco_group";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
        public GetTelcoGroupDetail TELCOGROUPDETAIL { get; set; } = new GetTelcoGroupDetail();
    }

    public class GetTelcoGroupDetail
    {
        //ID của khách hàng
        public int TELCOGROUPID { get; set; } = 0;
        //Tên khách hàng
        public string TELCOGROUPNAME { get; set; } = "";
        //Địa chỉ khách hàng
        public string TELCOGROUPDESC { get; set; } = "";
    }
}
