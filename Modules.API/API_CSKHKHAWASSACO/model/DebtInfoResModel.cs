using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.API.API_CSKHKHAWASSACO.model
{
    public class ApiResponseModel
    {
        public bool Success { get; set; }
        public string Data { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ARTransactionModel
    {
        public string ARTransactionID { get; set; }
        public string Address { get; set; }
        public string ContractCode { get; set; }
        public int CrAmt { get; set; }
        public string CustomerName { get; set; }
        public int DrAmt { get; set; }
        public string InvoiceNbr { get; set; }
        public string Period { get; set; }
        public int RemaintAmt { get; set; }
    }
    public class DebtDataModel
    {
        public List<ARTransactionModel> GetARByCustomerResult { get; set; }
    }

    public class PayInvoiceModel
    {

    }

}
