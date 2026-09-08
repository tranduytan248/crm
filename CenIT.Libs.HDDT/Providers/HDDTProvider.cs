
namespace CenIT.Libs.HDDT.Providers
{
    public class HDDTProvider
    {      
        private readonly ServiceHDDTProvider _serviceProvider;

  
        public HDDTProvider(string urlPortalService, string urlBusinessService, string urlPublishService)
        {
            _serviceProvider = new ServiceHDDTProvider(urlPortalService, urlBusinessService, urlPublishService);
        } 

        public string GetInvView(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            return _serviceProvider.GetInvView(username, password,InvToken, dataDemo);
        }
        public string GetInvViewNoPay(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            return _serviceProvider.GetInvViewNoPay(username, password, InvToken, dataDemo);
        }
        public string DownloadInvPDF(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            return _serviceProvider.DownloadInvPDF(username, password, InvToken, dataDemo);
        }

        public string DownloadInvPDFNoPay(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            return _serviceProvider.DownloadInvPDFNoPay(username, password, InvToken, dataDemo);
        }

    }
}