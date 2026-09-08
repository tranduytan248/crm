using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetContractModel
    {
        public DataRequestGetContractModel RQST { get; set; } = new DataRequestGetContractModel();
    }

    public class ResponseGetContract
    {
        public DataResponseGetContract RPLY { get; set; }
    }

    public class DataRequestGetContractModel
    {
        public string name { get; set; } = "get_contract";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //ID của khách hàng quảng cáo
        public string ADSERID { get; set; } = "[adser_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API(Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseGetContract
    {
        public string name { get; set; } = "get_contract";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
        public GetContractDetail CONTRACTDETAIL { get; set; } = new GetContractDetail();
    }

    public class GetContractDetail
    {
        //ID của hợp đồng
        public int CONTRACTID { get; set; } = 0;
        //Số hợp đồng
        public string CONTRACTNUMBER { get; set; } = "";
        //Ngày hợp đồng
        public string CONTRACTDATE { get; set; } = "";
        //Ngày bắt đầu
        public string STARTVALIDDATE { get; set; } = "";
        //Ngày kết thúc
        public string ENDVALIDDATED { get; set; } = "";
        //Tên hợp đồng
        public string CONTRACTNAME { get; set; } = "";
        //ID loại hợp đồng
        public int CONTRACTTYPEID { get; set; } = 0;
        //Loại hợp đồng
        public string CONTRACTTYPENAME { get; set; } = "";
    }
}
