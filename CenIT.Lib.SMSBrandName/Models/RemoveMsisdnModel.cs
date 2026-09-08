using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestRemoveMsisdnModel
    {
        public DataRequestRemoveMsisdnModel RQST { get; set; } = new DataRequestRemoveMsisdnModel();
    }

    public class ResponseRemoveMsisdn
    {
        public DataResponseRemoveMsisdn RPLY { get; set; }
    }

    public class DataRequestRemoveMsisdnModel
    {
        public string name { get; set; } = "remove_msisdn";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //Số thuê bao
        public string MSISDN { get; set; } = "[MSISDN]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseRemoveMsisdn
    {
        public string name { get; set; } = "remove_msisdn";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
      
    }
    
}
