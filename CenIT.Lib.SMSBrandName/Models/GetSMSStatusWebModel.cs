using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetSMSStatusWebModel
    {
        public DataRequestGetSMSStatusWebModel RQST { get; set; } = new DataRequestGetSMSStatusWebModel();
    }

    public class ResponseGetSMSStatusWeb
    {
        public DataResponseGetSMSStatusWeb RPLY { get; set; }
    }

    public class DataRequestGetSMSStatusWebModel
    {
        public string name { get; set; } = "get_sms_status_web";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //Số thuê bao gốc
        public string MSISND { get; set; } = "[MSISNDA]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //ID của hợp đồng
        public string CONTRACTID { get; set; } = "[CONTRACT_id]";
        //ID của nhãn
        public string LABELID { get; set; } = "[LABEL_ID]";
        //ID của template
        public string TEMPLATEID { get; set; } = "[TEMPLATE_ID]";
        //Ngày gửi tin
        //Nếu khi gửi tin, SCHEDULETIME không thiết lập, lúc verify
        //truyền vào ngày tạo
        public string SCHEDULETIME { get; set; } = "[SCHEDULE_TIME]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }
    
    public class DataResponseGetSMSStatusWeb
    {
        public string name { get; set; } = "get_sms_status_web";
        /// <summary>
        /// -1 Exception
        /// 0 Tin đã gửi thành công (CSKH: SENT, QC: DELIVRD)
        /// 1 Tin đã gửi lỗi (SENT_FAIL)
        /// 2 Tin đang chờ gửi (PENDING)
        /// 3 Tin đã đẩy, đang chờ cập nhật trạng thái gửi tin (QC: SENT)
        /// 4 Tin đã đẩy, đang chờ gửi lại (RETRYING)
        /// 5 Đã nhận qua Web, chưa gen tin
        /// 10 User, Pass, IP không hợp lệ
        /// </summary>
        public string STATUS { get; set; } = "0";
        //Số lượng MT tính phí trong trường hợp tin nhắn thành công
        public string MT_COUNT { get; set; } = "Success";
      
    }
    
}
