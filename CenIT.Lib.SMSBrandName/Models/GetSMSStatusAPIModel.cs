using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetSMSStatusAPIModel
    {
        public DataRequestGetSMSStatusAPIModel RQST { get; set; } = new DataRequestGetSMSStatusAPIModel();
    }

    public class ResponseGetSMSStatusAPI
    {
        public DataResponseGetSMSStatusAPI RPLY { get; set; }
    }

    public class DataRequestGetSMSStatusAPIModel
    {
        public string name { get; set; } = "get_sms_status_api";
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
    
    public class DataResponseGetSMSStatusAPI
    {
        public string name { get; set; } = "get_sms_status_api";
        /// <summary>
        /// -1 Exception
        /// 0 Tin đã gửi thành công (CSKH: SENT, QC: DELIVRD)
        /// 1 Tin đã gửi lỗi (SENT_FAIL)
        /// 2 Tin đang chờ gửi (PENDING)
        /// 3 Tin đã đẩy, đang chờ cập nhật trạng thái gửi tin (QC: SENT)
        /// 4 Tin đã đẩy, đang chờ gửi lại (RETRYING)
        /// 5 Đã nhận qua API, chưa gen tin
        /// 10 User, Pass, IP không hợp lệ
        /// </summary>
        public string STATUS { get; set; } = "0";
        //Số lượng MT tính phí trong trường hợp tin nhắn thành công
        public string MT_COUNT { get; set; } = "Success";
      
    }
    
}
