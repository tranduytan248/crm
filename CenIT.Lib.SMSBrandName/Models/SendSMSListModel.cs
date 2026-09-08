using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestSendSMSListModel
    {
        public DataRequestSendSMSListModel RQST { get; set; } = new DataRequestSendSMSListModel();
    }

    public class ResponseSendSMSList
    {
        public DataResponseSendSMSList RPLY { get; set; }
    }

    public class DataRequestSendSMSListModel
    {
        public string name { get; set; } = "send_sms_list";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhãn -> xem trên portal
        public string LABELID { get; set; } = "[label_id]";
        //ID của nhãn -> xem trên portal
        public string CONTRACTID { get; set; } = "[contract_id]";
        //ID Hợp đồng -> xem trên portal
        public string TEMPLATEID { get; set; } = "[template_id]";
        //Số thứ tự của tham số truyền vào mẫu bản tin, nếu template
        //không có tham số, chỉ cần truyền cặp<PARAMS></PARAMS>
        public List<DataRequestRaramsSendSMSListModel> PARAMS { get; set; } = new List<DataRequestRaramsSendSMSListModel>();
        //Tin nhắn QC = 2, tin nhắn CSKH = 1
        public string CONTRACTTYPEID { get; set; } = "[schedule_time]";
        //Đặt lịch gửi tin. Cấu trúc là : dd/MM/yyyy hh24:mi, 
        //ví dụ : 08/05/2012 16:30 Trong trường hợp muốn tin gửi đi luôn, chỉ cần truyền cặp thẻ <SCHEDULETIME></SCHEDULETIME>
        public string SCHEDULETIME { get; set; } = "[schedule_time]";
        //Danh sách các số thuê bao cần gửi, các thuê bao phân cách bởi dấu phẩy, và không có khoảng trắng, ví dụ 84912000111
        public string MOBILELIST { get; set; } = "[mobile_list]";
        //Sử dụng nhóm thuê bao của nhà mạng. luôn = 0
        public string ISTELCOSUB { get; set; } = "[is_telco_sub]";
        //ID của nhà đại lý (Vinaphone cấp) -> xem trên portal
        public string AGENTID { get; set; } = "[agent_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
        //User đăng nhập của Agent -> Username vào portal
        public string USERNAME { get; set; } = "[user_name]";
        //Định dạng ký tự gửi tin: 0 gửi không dấu, 8 gửi tiếng Việt có dấu.
        //Mặc định: 0
        //Lưu ý: Gửi tin có dấu hiện tại chỉ hỗ trợ đối với mạng VinaPhone
        public string DATACODING { get; set; } = "[data_coding]";
        //Mã đơn hàng. Các tin nhắn thuộc cùng 1 đơn hàng thì có
        //SaleOrderId giống nhau dành cho k/h gửi tin TMĐT
        public string SALEORDERID { get; set; } = "[sale_order id]";
        //Mã gói tin dành cho k/h gửi tin TMĐT
        public string PACKAGEID { get; set; } = "[package id]";
    }

    public class DataRequestRaramsSendSMSListModel
    {
        //Số thứ tự của tham số truyền vào mẫu bản tin, nếu template
        //không có tham số, chỉ cần truyền cặp<PARAMS></PARAMS>
        public string NUM { get; set; } = "1";
        //Nội dung của tham số tương ứng
        public string CONTENT { get; set; } = "[param_1]";
    }
    
    public class DataResponseSendSMSList
    {
        public string name { get; set; } = "send_sms_list";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
    }
    
}
