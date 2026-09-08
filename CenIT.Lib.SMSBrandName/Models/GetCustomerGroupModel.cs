using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CenIT.Lib.SMSBrandName.Models
{
    public class RequestGetCustomerGroupModel
    {
        public DataRequestGetCustomerGroupModel RQST { get; set; } = new DataRequestGetCustomerGroupModel();
    }

    public class ResponseGetCustomerGroup
    {
        public DataResponseGetCustomerGroup RPLY { get; set; }
    }

    public class DataRequestGetCustomerGroupModel
    {
        public string name { get; set; } = "get_customer_group";
        //Request ID
        public string REQID { get; set; } = "[request_id]";
        //ID của nhà đại lý (Vinaphone cấp)
        public string AGENTID { get; set; } = "[agent_id]";
        //ID của khách hàng quảng cáo
        public string ADSERID { get; set; } = "[adser_id]";
        //Username của API (Vinaphone cấp)
        public string APIUSER { get; set; } = "[api_user]";
        //Password của API (Vinaphone cấp)
        public string APIPASS { get; set; } = "[api_pass]";
    }

    public class DataResponseGetCustomerGroup
    {
        public string name { get; set; } = "get_customer_group";
        /// <summary>
        /// -1 Exception
        /// 0 Success
        /// 1 Username, password, IP, status các API không hợp lệ
        /// </summary>
        public string ERROR { get; set; } = "0";
        public string ERROR_DESC { get; set; } = "Success";
        public GetCustomerGroupDetail CUSTOMERGROUPDETAIL { get; set; } = new GetCustomerGroupDetail();
    }

    public class GetCustomerGroupDetail
    {
        //ID của nhóm thuê bao
        public int TELCOGROUPID { get; set; } = 0;
        //Tên nhóm thuê bao
        public string TELCOGROUPNAME { get; set; } = "";
        //Mô tả về nhóm thuê bao
        public string TELCOGROUPDESC { get; set; } = "";
    }
}
