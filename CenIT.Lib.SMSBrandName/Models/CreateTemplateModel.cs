using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestCreateTemplateModel
    {
        public DataRequestCreateTemplateModel RQST { get; set; } = new DataRequestCreateTemplateModel();
    }

    public class ResponseCreateTemplate
    {
        public DataResponseCreateTemplate RPLY { get; set; }
    }

    public class DataRequestCreateTemplateModel
    {
        public string name { get; set; } = "create_template";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //Tên của nhãn
        public string LABEL { get; set; } = "LABEL";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //ID của hợp đồng
        public string CONTRACTID { get; set; } = "[CONTRACT_id]";
        //Nội dung của template
        public string CONTENT { get; set; } = "[LABEL_ID]";
        //Số tham biến trong template (chỉ áp dụng với template QC)
        public string TOTALPARAMS { get; set; } = "TOTALPARAMS";
        //Nội dung tin nhắn mẫu (chỉ áp dụng với template CSKH)
        public string SAMPLEMESSAGE { get; set; } = "SAMPLEMESSAGE";
        //User đăng nhập của Agent -> Username vào portal
        public string USERNAME { get; set; } = "[USERNAME]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseCreateTemplate
    {
        public string name { get; set; } = "create_template";
        /// <summary>
        ///-1    Exception
        ///0     Tạo template thành công
        ///1     Username, password, IP, status các API không hợp lệ: Liên hệ media(Hoàn DD) để kiểm tra username, password của API, đồng thời kiểm tra IP phía server nhận được nếu cần
        ///10    User_name không hợp lệ(user đăng nhập của Agent trên portal không đúng)
        ///13    Hợp đồng không đúng
        ///14    Label không hợp lệ
        ///15    Agent không hợp lệ
        ///50    Template chứa từ khóa chặn
        ///51    Độ dài template không hợp lệ
        ///55    Danh sách tham số không hợp lệ
        ///56    Chỉ cho phép tối đa tạo 5 tham biến mỗi loại với template kiểu mới
        ///57    Tin nhắn mẫu không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        //Số lượng MT tính phí trong trường hợp tin nhắn thành công
        public string ERROR_DESC { get; set; } = "Success";
      
    }
    
}
