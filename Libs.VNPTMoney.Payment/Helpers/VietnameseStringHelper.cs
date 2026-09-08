using System.Text;

namespace CenIT.Libs.VNPTMoney.Payment.Helpers
{
    public class VietnameseStringHelper
    {
        public static string RemoveVietnameseAccents(string str)
        {
            if (str == null)
            {
                return null;
            }

            // Khai báo bảng chữ cái có dấu và không dấu
            string withAccentLower = "àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ";
            string withAccentUpper = withAccentLower.ToUpper();
            string withoutAccent = "aaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd";

            // Loại bỏ dấu từng ký tự
            StringBuilder sb = new StringBuilder();
            foreach (char ch in str)
            {
                int indexLower = withAccentLower.IndexOf(ch);
                int indexUpper = withAccentUpper.IndexOf(ch);

                if (indexLower != -1)
                {
                    sb.Append(withoutAccent[indexLower]);
                }
                else if (indexUpper != -1)
                {
                    sb.Append(char.ToUpper(withoutAccent[indexUpper]));
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }
    }
}