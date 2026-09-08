using System;
using System.Net;
using CenIT.Libs.HDDT.InvoiceBusinessService;
using CenIT.Libs.HDDT.InvoicePortalService;
using CenIT.Libs.HDDT.InvoicePublishService;

namespace CenIT.Libs.HDDT.Providers
{
    public class ServiceHDDTProvider
    {
        private readonly BusinessService _businessClient = new BusinessService();
        private readonly PortalService _portalClient = new PortalService();
        private readonly PublishService _publishClient = new PublishService();

        public ServiceHDDTProvider(string urlPortalService, string urlBusinessService, string urlPublishService)
        {
            _portalClient.Url = urlPortalService;
            _publishClient.Url = urlPublishService;
            _businessClient.Url = urlBusinessService;
        }

        public bool IsOnline()
        {
            try
            {
                var clientTest = new WebClient();
                clientTest.OpenRead(_portalClient.Url);
                clientTest.OpenRead(_publishClient.Url);
                clientTest.OpenRead(_businessClient.Url);

                var portalRequest = (HttpWebRequest)WebRequest.Create(_portalClient.Url);
                var portalResponse = (HttpWebResponse)portalRequest.GetResponse();

                var publishRequest = (HttpWebRequest)WebRequest.Create(_portalClient.Url);
                var publishResponse = (HttpWebResponse)publishRequest.GetResponse();

                var businessRequest = (HttpWebRequest)WebRequest.Create(_portalClient.Url);
                var businessResponse = (HttpWebResponse)businessRequest.GetResponse();

                return portalResponse.StatusCode == HttpStatusCode.OK &&
                       publishResponse.StatusCode == HttpStatusCode.OK &&
                       businessResponse.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public bool IsCorrectUser(string sEInvServiceAccount, string sEInvServiceAcPass)
        {
            try
            {
                var errCode = _businessClient.reportMonth(DateTime.Now.Year, DateTime.Now.Month, sEInvServiceAccount, sEInvServiceAcPass);
                return !errCode.Contains("ERR:");
                //return errCode != "ERR:1";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Thêm mới và phát hành các hóa đơn (V5 22/08/2022)
        /// </summary>
        /// <param name="sXmlInvData">
        ///     Danh sách các hóa đơn cần thêm mới dưới dạng chuỗi xml
        ///     ======================================================
        ///     <Invoices>
        ///     	<Inv>
        ///     		<key>Giá trị khóa để phân biệt hóa đơn xuất cho khách hàng nào</key>
        ///     		<Invoice>
        ///     			<CusCode>Mã khách hàng*</CusCode>
        ///     			<CusName>Tên khách hàng*</CusName>
        ///     			<CusAddress>Địa chỉ khách hàng*</CusAddress>
        ///     			<CusPhone>Điện thoại khách hàng</CusPhone>
        ///     			<CusTaxCode>Mã số thuế KH (Bắt buộc với KH là Doanh nghiệp)</CusTaxCode>
        ///     			<PaymentMethod>Phương thức thanh toán</PaymentMethod>
        ///     			<KindOfService>Tháng hóa đơn</KindOfService>
        ///     			<Products>
        ///     				<Product>
        ///     					<ProdName>Tên sản phẩm*</ProdName>
        ///     					<ProdUnit>Đơn vị tính</ProdUnit>
        ///     					<ProdQuantity>Số lượng</ProdQuantity>
        ///     					<ProdPrice>Đơn giá</ProdPrice>
        ///     					<Amount>Tổng tiền sau thuế*</Amount>
        ///     					<Remark>Remark</Remark>
        ///     					<Total>Tổng tiền trước thuế*</Total>
        ///     					<VATRate>Thuế GTGT*</VATRate>
        ///     					<VATAmount>Tổng tiền thuế*</VATAmount>
        ///     					<Extra1>Mở rộng 1</Extra1>
        ///     					<Extra2>Mở rộng 2</Extra2>
        ///     					<Discount>Chiết khấu</Discount>
        ///     					<DiscountAmount>Tổng tiền chiết khấu</DiscountAmount>
        ///     					<IsSum> Tính chất * (0-Hàng hóa, dịch vụ; 1-Khuyến mại; 2-Chiết khấu thương mại(trong trường hợp muốn thể hiện thông tin chiết khấu theo dòng); 4-Ghi chú/diễn giải)</IsSum>
        ///     				</Product>
        ///     			</Products>
        ///     			<Total>Tổng tiền trước thuế*</Total>
        ///     			<DiscountAmount>Tiền giảm trừ</DiscountAmount>
        ///     			<VATRate>Thuế GTGT*</VATRate>
        ///     			<VATAmount>Tiền thuế GTGT*</VATAmount>
        ///     			<Amount>Tổng tiền*</Amount>
        ///     			<AmountInWords>Số tiền viết bằng chữ*</AmountInWords>
        ///     			<Extra>Trường mở rộng</Extra>
        ///     			<ArisingDate>Ngày dịch vụ</ArisingDate>
        ///     			<PaymentStatus>Trạng thái thanh toán</PaymentStatus>
        ///     			<EmailDeliver>EmailDeliver</EmailDeliver>
        ///     			<ComName>Tên công ty</ComName>
        ///     			<ComAddress>Địa chỉ công ty</ComAddress>
        ///     			<ComTaxCode>Mã số thuế</ComTaxCode>
        ///     			<ComFax>Company Fax</ComFax>
        ///     			<ResourceCode>ResourceCode</ResourceCode>
        ///     			<GrossValue> Thành tiền trước thuế KCT</GrossValue>
        ///     			<GrossValue0> Thành tiền trước thuế 0%</GrossValue0>
        ///     			<VatAmount0> Tiền thuế 0% </VatAmount0>
        ///     			<GrossValue5>Thành tiền trước thuế 5%</GrossValue5>
        ///     			<VatAmount5> Tiền thuế 5% </VatAmount5>
        ///     			<GrossValue8>Thành tiền trước thuế 8% </GrossValue8>
        ///     			<VatAmount8> Tiền thuế 8% </VatAmount8>
        ///     			<GrossValue10> Thành tiền trước thuế 10% </GrossValue10>
        ///     			<VatAmount10> Tiền thuế 10 %</VatAmount10>
        ///     			<Buyer>Tên đơn vị mua hàng</Buyer>
        ///     			<Name>Tên hóa đơn</Name>
        ///     			<ComPhone>Điện thoại công ty</ComPhone>
        ///     			<ComBankName>Tên ngân hàng</ComBankName>
        ///     			<ComBankNo>Số tài khoản ngân hàng</ComBankNo>
        ///     			<CreateDate>CreateDate</CreateDate>
        ///     			<DiscountRate>Chiết khấu</DiscountRate>
        ///     			<CusSignStatus>1.1</CusSignStatus>
        ///     			<CreateBy>CreateBy</CreateBy>
        ///     			<PublishBy>PublishBy</PublishBy>
        ///     			<Note>Note</Note>
        ///     			<ProcessInvNote>ProcessInvNote</ProcessInvNote>
        ///     			<Fkey>Fkey</Fkey>
        ///     			<GrossValue_NonTax/>
        ///     			<CurrencyUnit>Đơn vị tiền tệ</CurrencyUnit>
        ///     			<ExchangeRate>Tỷ giá</ExchangeRate>
        ///     			<ConvertedAmount>Tổng tiền quy đổi</ConvertedAmount>
        ///     			<Extra1>Extra1</Extra1>
        ///     			<Extra2>Extra2</Extra2>
        ///     			<SMSDeliver>SMSDeliver</SMSDeliver>
        ///     			<LDDNBo>LDDNBO</LDDNBo>
        ///     			<HDSo>HDSO</HDSo>
        ///     			<HVTNXHang>HVTNXHANG</HVTNXHang>
        ///     			<TNVChuyen>TNVCHUYEN</TNVChuyen>
        ///     			<PTVChuyen>PTVCHUYEN</PTVChuyen>
        ///     			<HDKTSo>HDKTSO</HDKTSo>
        ///     			<HDKTNgay>HDKTNgay</HDKTNgay>
        ///     		</Invoice>
        ///     	</Inv>
        ///     </Invoices>
        /// </param>
        /// <param name="sEmpAccount">Tài khoản nhân viên được cấp quyền tạo hóa đơn</param>
        /// <param name="sEmpAcPass">Mật khẩu tài khoản nhân viên được cấp quyền tạo hóa đơn</param>
        /// <param name="sEInvServiceAccount">Tài khoản dùng để gọi service</param>
        /// <param name="sEInvServiceAcPass">Mật khẩu tài khoản dùng để gọi service</param>
        /// <param name="sEInvPattern">Mẫu hóa đơn đăng ký trên hệ thống hóa đơn điện tử</param>
        /// <param name="sEInvSerial">Số serial đăng ký trên hệ thống hóa đơn điện tử</param>
        /// <returns>
        ///     + OK:pattern;serial1-key1_num1,key2_num12,... (tạo mới và phát hành hóa đơn thành công + danh sách FKey và số hoá đơn đã phát hành)
        ///      [OK:2/001;C24TAA-BD58DE940D4C6EB0_68,XD58DE940D4CZZZ_69,...] = [pattern;serial;key_số hóa đơn]
        ///     + ERR:1     : Tài khoản đăng nhập sai hoặc không có quyền
        ///     bỏ+ ERR:2     : Chuỗi token không chính xác
        ///     + ERR:3     : Dữ liệu xml đầu vào không đúng quy định (Hệ thống sẽ trả về lỗi nếu 1 hóa đơn trong chuỗi XML đầu vào không hợp lệ, cả lô hóa đơn sẽ không được phát hành)
        ///     bỏ+ ERR:4     : Công ty chưa được đăng kí mẫu hóa đơn nào
        ///     + ERR:5     : Không phát hành được hóa đơn (Lỗi không xác định, kiểm tra exception trả về (DB roll back))
        ///     + ERR:6     : Dải hóa đơn không đủ số hóa đơn cho lô phát hành
        ///     + ERR:7     : Thông tin về Username/pass không hợp lệ
        ///     bỏ+ ERR:8     : hóa đơn cần điều chỉnh đã bị thay thế. Không thể điều chỉnh được nữa.
        ///     bỏ+ ERR:9     : Trạng thái hóa đơn không được điều chỉnh
        ///     + ERR:10    : Lô có số hóa đơn vượt quá số lượng cho phép
        ///     bỏ+ ERR:11    : hóa đơn chưa cho deliver, ko xem được
        ///     + ERR:13    : Lỗi trùng fkey (1 hoặc nhiều hóa đơn trong lô hóa đơn có Fkey trùng với Fkey của hóa đơn đã phát hành)
        ///     bỏ+ ERR:14    : Lỗi NoFactory
        ///     + ERR:20    : Pattern và Serial không phù hợp, hoặc không tồn tại hóa đơn đã đăng kí có sử dụng Pattern và Serial truyền vào.
        ///     + ERR:21    : Lỗi trùng số hóa đơn
        ///     + ERR:29    : Lỗi chứng thư hết hạn
        ///     + ERR:30    : Danh sách hóa đơn tồn tại ngày hóa đơn nhỏ hơn ngày hóa đơn đã phát hành
        /// </returns>
        public string ImportAndPublishInv(string sXmlInvData, string sEmpAccount, string sEmpAcPass,
            string sEInvServiceAccount, string sEInvServiceAcPass, string sEInvPattern, string sEInvSerial)
        {
            try
            {
                var resultOfService = _publishClient.ImportAndPublishInv(
                    sEmpAccount, // Account
                    sEmpAcPass, // ACPass
                    sXmlInvData, // XMLInvData
                    sEInvServiceAccount, // UserName
                    sEInvServiceAcPass, // Password
                    sEInvPattern, // Pattern
                    sEInvSerial, // Serial
                    0); // convert 
                return resultOfService;
            }
            catch (Exception ex)
            {
                return "ERR:0";
            }
        }

        /// <summary>
        ///     Mô tả lỗi từ service ImportAndPublishInv
        /// </summary>
        public string ErrorServiceProcess_ImportAndPublishInv(string errCode)
        {
            switch (errCode)
            {
                case "ERR:0": // bổ sung lỗi gọi service (không có trong tài liệu dev !)
                    return "[ERR:0]: Phát sinh lỗi khi gọi service";
                case "ERR:1":
                    return "[ERR:1]: Tài khoản đăng nhập sai hoặc không có quyền thêm khách hàng";
                case "ERR:3":
                    return "[ERR:3]: Dữ liệu xml đầu vào không đúng quy định";
                case "ERR:5":
                    return "[ERR:5]: Không phát hành được hóa đơn";
                case "ERR:6":
                    return "[ERR:6]: Dải hóa đơn không đủ số hóa đơn cho lô phát hành";
                case "ERR:7":
                    return "[ERR:7]: Thông tin về Username/pass không hợp lệ";
                case "ERR:10":
                    return "[ERR:10]: Lô có số hóa đơn vượt quá số lượng cho phép";
                case "ERR:13":
                    return "[ERR:13]: Lỗi trùng fkey";
                case "ERR:20":
                    return "[ERR:20]: Pattern và Serial không phù hợp, hoặc không tồn tại hóa đơn đã đăng kí có sử dụng Pattern và Serial truyền vào";
                case "ERR:21":
                    return "[ERR:21]: Lỗi trùng số hóa đơn";
                case "ERR:29":
                    return "[ERR:29]: Lỗi chứng thư hết hạn";
                case "ERR:30":
                    return "[ERR:30]: Danh sách hóa đơn tồn tại ngày hóa đơn nhỏ hơn ngày hóa đơn đã phát hành";
                default:
                    return $"[{errCode}]";
            }
        }

        /// <summary>
        ///     Gạch nợ hóa đơn theo danh sách fkey truyền vào (V5_22082022)
        ///     (mô tả cũ: Xác nhận thanh toán cho hóa đơn bằng FKey. Có thể xác nhận cho 1 hoặc nhiều hóa đơn)
        /// </summary>
        /// <param name="lstFKey">Chuỗi Fkey xác định hóa đơn cần lấy(các Fkey phân biệt nhau bằng “_”) </param>
        /// <param name="sEInvServiceAccount">Tài khoản dùng để gọi service (tài khoản có quyền ServiceRole trong hệ thống)</param>
        /// <param name="sEInvServiceAcPass">Mật khẩu tài khoản dùng để gọi service</param>
        /// <returns>
        ///     + OK        : Đánh dấu hóa đơn trong list đã được gạch nợ
        ///     + ERR:1     : Tài khoản đăng nhập sai hoặc không có quyền
        ///     bỏ+ ERR:2     : Chuỗi token không chính xác
        ///     bỏ+ ERR:3     : Dữ liệu xml đầu vào không đúng quy định
        ///     bỏ+ ERR:4     : Công ty chưa được đăng kí mẫu hóa đơn nào
        ///     bỏ+ ERR:5     : Không phát hành
        ///     + ERR:6     : Không tìm thấy hóa đơn tương ứng chuỗi đưa vào
        ///     + ERR:7     : Không tìm thấy thông tin công ty tương ứng, hoặc lỗi không xác định
        ///     bỏ+ ERR:8     : hóa đơn cần điều chỉnh đã bị thay thế. Không thể điều chỉnh được nữa.
        ///     bỏ+ ERR:9     : Trạng thái hóa đơn không được điều chỉnh
        ///     bỏ+ ERR:10    : Lô có số hóa đơn vượt quá max cho phép
        ///     bỏ+ ERR:11    : hóa đơn chưa cho deliver, ko xem được
        ///     + ERR:13    : Hóa đơn đã được gạch nợ trước đó
        ///     bỏ+ ERR:14    : Lỗi NoFactory
        ///     bỏ+ ERR:20    : Pattern và serial không phù hợp
        /// </returns>
        public string ConfirmPaymentFkey(string lstFKey, string sEInvServiceAccount, string sEInvServiceAcPass)
        {
            try
            {
                var resultOfService = _businessClient.confirmPaymentFkey(lstFKey, sEInvServiceAccount, sEInvServiceAcPass);
                return resultOfService;
            }
            catch (Exception ex)
            {
                return "ERR:0";
            }
        }

        /// <summary>
        ///     Mô tả lỗi từ service confirmPaymentFkey
        /// </summary>
        public string ErrorServiceProcess_ConfirmPaymentFkey(string errCode)
        {
            switch (errCode)
            {
                case "ERR:0": // bổ sung lỗi gọi service (không có trong tài liệu dev !)
                    return "[ERR:0]: Phát sinh lỗi khi gọi service";
                case "ERR:1":
                    return "[ERR:1]: Tài khoản đăng nhập sai hoặc không có quyền";
                case "ERR:6":
                    return "[ERR:6]: Không tìm thấy hóa đơn tương ứng chuỗi đưa vào";
                case "ERR:7":
                    return "[ERR:7]: Không tìm thấy thông tin công ty tương ứng, hoặc lỗi không xác định";
                case "ERR:13":
                    return "[ERR:13]: Hóa đơn đã được gạch nợ trước đó";
                default:
                    return $"[{errCode}]";
            }
        }

        /// <summary>
        ///     Thêm mới hoặc cập nhật thông tin danh sách khách hàng
        /// </summary>
        /// <param name="sXmlCusData">
        ///     <Customers>
        ///         <Customer>
        ///             <Name>Tên khách hàng*</Name>
        ///             <Code>Mã khách hàng*</Code>
        ///             <Account>Tài khoản đăng nhập</Account>
        ///             <TaxCode>Mã số thuế (bắt buộc với khách hàng là doanh nghiệp)</TaxCode>
        ///             <Address>Địa chỉthanh toán*</Address>
        ///             <BankAccountName>Tên tài khoản ngân hàng</BankAccountName>
        ///             <BankName>Tên ngân hàng</BankName>
        ///             <BankNumber>Số tài khoản</BankNumber>
        ///             <Email>Email</Email>
        ///             <Fax>Số fax</Fax>
        ///             <Phone>Điện thoại</Phone>
        ///             <ContactPerson>Liên hệ</ContactPerson>
        ///             <RepresentPerson>Người đại diện</RepresentPerson>
        ///             <CusType>Loại khách hàng (1: Doanh nghiệp/0: Cá nhân)*</CusType>
        ///             <IsEmail>Khách hàng nhận gửi mail khi phát hành hay không, 1: Có nhận/0: không nhận(Nếu không có thẻ này mặc định có nhận mail)</IsEmail >
        ///         </Customer>
        ///         <Customer>...</Customer>
        ///     </Customers>
        /// </param>
        /// <param name="sEInvServiceAccount">Tài khoản được cấp phát cho khách hàng để gọi đến webservice (tài khoản có quyền ServiceRole trong hệ thống)</param>
        /// <param name="sEInvServiceAcPass">Mật khẩu tài khoản dùng để gọi service</param>
        /// <returns>
        ///     + -1: Tài khoản đăng nhập sai hoặc không có quyền thêm khách hàng
        ///     + -2: Không import được khách hàng vào db (Có rollback db)
        ///     + -3: Dữ liệu xml đầu vào không đúng quy định (Chỉ cần 1 customer trong chuỗi xml không hợp lệ, không thực hiện update trên tất cả dữ liệu đưa vào)
        ///     bỏ//+ -5: User đã tồn tại rồi
        ///     + N: Số lượng khách hàng đã import và update (N>0, N là kiểu integer)
        /// </returns>
        public int UpdateCus(string sXmlCusData, string sEInvServiceAccount, string sEInvServiceAcPass)
        {
            try
            {
                var resultOfService = _publishClient.UpdateCus(sXmlCusData, sEInvServiceAccount, sEInvServiceAcPass, 0);
                return resultOfService;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public string GetInvView(string username, string password,string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            try
            {
                var resultOfService = "";
                if (string.IsNullOrEmpty(dataDemo))
                {
                    resultOfService = _portalClient.getInvView(InvToken, username, password);
                }
                else
                {
                    resultOfService = _portalClient.getInvView(dataDemo, username, password);
                }
               
                return resultOfService;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public string GetInvViewNoPay(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            try
            {
                var resultOfService = "";
                if (string.IsNullOrEmpty(dataDemo))
                {
                    resultOfService = _portalClient.getInvViewNoPay(InvToken, username, password);
                }
                else
                {
                    resultOfService = _portalClient.getInvViewNoPay(dataDemo, username, password);
                }

                return resultOfService;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public string DownloadInvPDF(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            try
            {
                var resultOfService = "";
                if (string.IsNullOrEmpty(dataDemo))
                {
                    resultOfService = _portalClient.downloadInvPDF(InvToken, username, password);
                }
                else
                {
                    resultOfService = _portalClient.downloadInvPDF(dataDemo, username, password);
                }

                return resultOfService;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        public string DownloadInvPDFNoPay(string username, string password, string InvToken = "2/001;C25MAA;00000003", string dataDemo = "2/001;C25MAA;00000003")
        {
            try
            {
                var resultOfService = "";
                if (string.IsNullOrEmpty(dataDemo))
                {
                    resultOfService = _portalClient.downloadInvPDFNoPay(InvToken, username, password);
                }
                else
                {
                    resultOfService = _portalClient.downloadInvPDFNoPay(dataDemo, username, password);
                }

                return resultOfService;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /// <summary>
        ///     Mô tả lỗi từ service UpdateCus
        /// </summary>
        public string ErrorServiceProcess_UpdateCus(int errCode)
        {
            switch (errCode)
            {
                case 0: // bổ sung lỗi gọi service (không có trong tài liệu dev !)
                    return "[ERR:0]: Phát sinh lỗi khi gọi service";
                case -1:
                    return "[ERR:-1]: Tài khoản đăng nhập sai hoặc không có quyền thêm khách hàng";
                case -2:
                    return "[ERR:-2]: Không import được khách hàng vào db";
                case -3:
                    return "[ERR:-3]: Dữ liệu xml đầu vào không đúng quy định";
                default:
                    return $"[{errCode}]";
            }
        }      
    }
}