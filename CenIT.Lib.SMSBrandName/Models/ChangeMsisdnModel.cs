using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestChangeMsisdnModel
    {
        public DataRequestChangeMsisdnModel RQST { get; set; } = new DataRequestChangeMsisdnModel();
    }

    public class ResponseChangeMsisdn
    {
        public DataResponseChangeMsisdn RPLY { get; set; }
    }

    public class DataRequestChangeMsisdnModel
    {
        public string name { get; set; } = "change_msisdn";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //Số thuê bao gốc
        public string MSISNDA { get; set; } = "[MSISNDA]";
        //Số thuê bao đích
        public string MSISNDB { get; set; } = "[MSISNDB]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseChangeMsisdn
    {
        public string name { get; set; } = "change_msisdn";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
      
    }
    
}
