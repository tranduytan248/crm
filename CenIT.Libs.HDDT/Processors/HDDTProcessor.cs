using CenIT.Libs.HDDT.Providers;


namespace CenIT.Libs.HDDT.Processors
{
    public class HDDTProcessor
    {       
        private readonly HDDTProvider _eInvProvider;

        public HDDTProcessor(string sUrlPortalService, string sUrlBusinessService, string sUrlPublishService)
        {
            _eInvProvider = new HDDTProvider(sUrlPortalService, sUrlBusinessService, sUrlPublishService);
        }
        public string GetInView(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            string result =  _eInvProvider.GetInvView(username, password, InvToken, dataDemo);
            return result;
        }

        public string GetInViewNoPay(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            string result = _eInvProvider.GetInvViewNoPay(username, password, InvToken, dataDemo);
            return result;
        }

        public string DownloadInvPDF(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            string result = _eInvProvider.DownloadInvPDF(username, password, InvToken, dataDemo);
            return result;
        }
        public string DownloadInvPDFNoPay(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            string result = _eInvProvider.DownloadInvPDFNoPay(username, password, InvToken, dataDemo);
            return result;
        }
    }
}