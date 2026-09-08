public class DebtRequest
{
    public string ContractCode { get; set; }
    public string Period { get; set; }
    public int? CrAmt { get; set; }
    public string TranDate { get; set; }
    public string InvoiceNbr { get; set; }
    public string ARTransactionID { get; set; }
}