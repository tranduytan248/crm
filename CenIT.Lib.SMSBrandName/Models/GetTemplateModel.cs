using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetLabelModel
    {
        public DataRequestGetLabelModel RQST { get; set; } = new DataRequestGetLabelModel();
    }

    public class ResponseGetLabel
    {
        public DataResponseGetLabel RPLY { get; set; }
    }

    public class DataRequestGetLabelModel
    {
        public string name { get; set; } = "get_label";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //ID của khách hàng quảng cáo
        public string ADSERID { get; set; } = "[adser_id]";
        //ID của hợp đồng
        public string CONTRACTID { get; set; } = "[adser_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API(Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }

    public class DataResponseGetLabel
    {
        public string name { get; set; } = "get_label";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
        public GetLabelDetail LABELDETAIL { get; set; } = new GetLabelDetail();
    }

    public class GetLabelDetail
    {
        //ID của nhãn
        public int LABELID { get; set; } = 0;
        //Tên nhãn
        public string LABEL { get; set; } = "";
        //Số hiển thị (dành cho trường hợp gửi ngoại mạng)
        public int DISPLAYNUMBER { get; set; } = 0;
    }
}
