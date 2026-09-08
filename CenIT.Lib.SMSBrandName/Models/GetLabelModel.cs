using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetTemplateModel
    {
        public DataRequestGetTemplateModel RQST { get; set; } = new DataRequestGetTemplateModel();
    }

    public class ResponseGetTemplate
    {
        public DataResponseGetTemplate RPLY { get; set; }
    }

    public class DataRequestGetTemplateModel
    {
        public string name { get; set; } = "get_template";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //ID của nhãn
        public string LABELID { get; set; } = "[label_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API(Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }

    public class DataResponseGetTemplate
    {
        public string name { get; set; } = "get_template";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
        public GetTemplateDetail TEMPLATEDETAIL { get; set; } = new GetTemplateDetail();
    }

    public class GetTemplateDetail
    {
        //ID của mẫu tin nhắn
        public int TEMPLATEID { get; set; } = 0;
        //Loại mẫu tin nhắn
        public string TEMPLATETYPE { get; set; } = "";
        //Chi tiết mẫu tin nhắn
        //Ví dụ: "Cong ty {P1} hen gap mat dau xuan tai {P2} vao hoi { P3 }. Demo {P4}, quan tri mang {P5}"
        public string TEMPLATECONTENT { get; set; } = "";
        //Tổng số tham số truyền vào mẫu tin nhắn
        public int TOTALPARAM { get; set; } = 0;
    }
}
