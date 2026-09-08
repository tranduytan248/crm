using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TSFramework.Libs.Utils
{
    public static class UtilString
    {
        private static readonly Random _random = new Random();

        /// <summary>
        ///     Remove sign for vietnamese
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSign4VietnameseString(string str)
        {
            //return str;
            if (string.IsNullOrEmpty(str)) return str;
            string[] arr1 =
            {
                "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ",
                "í", "ì", "ỉ", "ĩ", "ị",
                "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ",
                "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự",
                "ý", "ỳ", "ỷ", "ỹ", "ỵ"
            };
            string[] arr2 =
            {
                "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
                "i", "i", "i", "i", "i",
                "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
                "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
                "y", "y", "y", "y", "y"
            };
            for (var i = 0; i < arr1.Length; i++)
            {
                str = str.Replace(arr1[i], arr2[i]);
                str = str.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }

            return str;
        }

        /// <summary>
        ///     Lấy một chuỗi con với độ dài cố định
        /// </summary>
        /// <param name="target">Chuỗi ban đầu</param>
        /// <param name="maxlength">Độ dài</param>
        /// <returns></returns>
        public static string TruncString(string target, int maxlength)
        {
            var output = target;

            if (target.Length > maxlength)
                output = output.Substring(0, maxlength) + "...";

            return output;
        }

        /// <summary>
        ///     Kiểm tra 1 chuỗi có phải là một số không
        /// </summary>
        /// <param name="theInt">Tên chuỗi</param>
        /// <returns>
        ///     <c>true</c> nếu là số; ngược lại, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(string theInt)
        {
            try
            {
                Convert.ToInt32(theInt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Chuyển đổi từ chuỗi sang số
        /// </summary>
        /// <param name="theInt">Chuỗi định dạng số</param>
        /// <returns></returns>
        public static int GetInt(string theInt)
        {
            var output = 0;

            if (IsNumeric(theInt))
                output = Convert.ToInt32(theInt);

            return output;
        }

        /// <summary>
        ///     Lấy các từ trong một chuỗi trừ khoảng trắng và đưa vào mảng danh sách
        /// </summary>
        /// <param name="data">Tên chuỗi</param>
        /// <param name="delimiter">char cần để split</param>
        /// <param name="sort">Nếu thiết lập <c>true</c> [sort].</param>
        /// <returns></returns>
        public static ArrayList LoadArrayList(string data, string delimiter, bool sort = true)
        {
            var output = new ArrayList();
            Array arr = data.Split(char.Parse(delimiter));

            for (var i = 0; i < arr.Length; i++) output.Add(arr.GetValue(i));

            if (sort)
                output.Sort();
            output.TrimToSize();

            return output;
        }

        /// <summary>
        ///     Lấy phần mở rộng của file
        /// </summary>
        /// <param name="filename">Tên file bao gồm phân mở rộng</param>
        /// <returns></returns>
        public static string GetExtention(string filename)
        {
            var ext = filename.LastIndexOf(".", StringComparison.Ordinal);
            return filename.Substring(ext + 1);
        }

        /// <summary>
        ///     Kiểm tra file có phải là định dạng cho phép không dựa vào phần mở rộng của file
        /// </summary>
        /// <param name="filename">Tên file</param>
        /// <param name="allowkey">Từ khóa cho ph</param>
        /// <returns>
        ///     <c>true</c> nếu định dạng là hình ảnh; ngược lại, <c>false</c>.
        /// </returns>
        public static bool IsImage(string filename, string allowkey)
        {
            var ext = GetExtention(filename);

            var arr = LoadArrayList(ConfigurationManager.AppSettings[allowkey], "|");
            if (arr.IndexOf(ext) > -1)
                return true;

            return false;
        }

        public static string GetUrlFromString(string title)
        {
            return Regex.Replace(title, "[^\\w_-]+", "-").ToLower();
        }

        /// <summary>
        ///     Lấy n ký đầu tiên từ một chuỗi nhưng không làm mất ý nghĩa của chữ
        /// </summary>
        /// <param name="str">Tên chuỗi</param>
        /// <param name="length">Chiều dài</param>
        /// <returns></returns>
        public static string SubStringToLength(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            var words = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words[0].Length > length)
                return "";

            var sb = new StringBuilder();
            foreach (var word in words)
            {
                if ((sb + word).Length > length) return $"{sb.ToString().TrimEnd(' ')}";
                sb.Append(word + " ");
            }

            return $"{sb.ToString().TrimEnd(' ')}";
        }

        /// <summary>
        ///     Removes the word.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string RemoveWord(string str)
        {
            if (str.Contains("'")) str = str.Replace("'", "");

            if (str.Contains("\\")) str = str.Replace("\\", string.Empty);

            if (str.Contains("=")) str = str.Replace("=", "");
            return str;
        }

        /// <summary>
        ///     Inputs the text.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <returns></returns>
        public static string InputText(string input, int maxlength)
        {
            var retVal = new StringBuilder();

            if (string.IsNullOrEmpty(input)) return retVal.ToString();
            input = input.Trim();

            if (input.Length > maxlength) input = input.Substring(0, maxlength);

            foreach (var t in input)
                switch (t)
                {
                    case '"':
                        retVal.Append("&quot;");
                        break;

                    case '<':
                        retVal.Append("&lt;");
                        break;

                    case '>':
                        retVal.Append("&gt;");
                        break;

                    default:
                        retVal.Append(t);
                        break;
                }

            retVal.Replace("'", " ");

            return retVal.ToString();
        }

        /// <summary>
        ///     Generates the string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string GenerateString(string input)
        {
            if (input.Contains(" ")) input = input.Replace(" ", "-");

            return input;
        }

        public static string Left(string str, int length)
        {
            var result = str.Substring(0, length);
            return result;
        }

        public static string Right(string str, int length)
        {
            var result = str.Substring(str.Length - length, length);
            return result;
        }

        /// <summary>
        ///     Hàm chuyển đổi chuỗi có dấu thành không dấu
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertToUnSign(string text)
        {
            text = text.Replace(" -", "");
            text = text.Replace("- ", "");
            text = text.Replace("-", " ");
            text = text.Replace("  ", " ");
            text = text.Replace("\"", "");

            for (var i = 33; i < 48; i++) text = text.Replace(((char)i).ToString(), "");

            for (var i = 58; i < 65; i++) text = text.Replace(((char)i).ToString(), "");

            for (var i = 91; i < 97; i++) text = text.Replace(((char)i).ToString(), "");

            for (var i = 123; i < 127; i++) text = text.Replace(((char)i).ToString(), "");


            text = text.Replace(" ", "-");

            if (text.EndsWith("-")) text = text.Remove(text.Length - 1);

            var regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");

            var strFormD = text.Normalize(NormalizationForm.FormD);
            return regex.Replace(strFormD, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToLower();
        }

        public static string TruncateText(string fullText, int numberOfCharacters)
        {
            string text;
            if (fullText.Length > numberOfCharacters)
            {
                var spacePos = fullText.IndexOf(" ", numberOfCharacters, StringComparison.Ordinal);
                if (spacePos > -1)
                    text = fullText.Substring(0, spacePos) + "...";
                else
                    text = fullText;
            }
            else
            {
                text = fullText;
            }

            var regexStripHtml = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = regexStripHtml.Replace(text, " ");
            return text;
        }

        public static string EnsureTrailingSlash(string stringThatNeedsTrailingSlash)
        {
            if (!stringThatNeedsTrailingSlash.EndsWith("/")) return stringThatNeedsTrailingSlash + "/";
            return stringThatNeedsTrailingSlash;
        }

        public static string StripHtmlTags(string source)
        {
            return Regex.Replace(source, "<.*?>|&.*?;", string.Empty);
        }

        public static string EncodePassword(string password)
        {
            var encoding = new UnicodeEncoding();
            var hashBytes = encoding.GetBytes(password);

            var sha1 = new SHA1CryptoServiceProvider();
            var cryptPassword = sha1.ComputeHash(hashBytes);
            return BitConverter.ToString(cryptPassword);
        }

        public static string FStrMd5(string aStrSource)
        {
            var fBytPassword = Encoding.ASCII.GetBytes(aStrSource);
            var fMd5 = MD5.Create();
            var fBytHash = fMd5.ComputeHash(fBytPassword);
            var sBuilder = new StringBuilder();
            for (var i = 0; i < fBytHash.Length; i++) sBuilder.Append(fBytHash[i].ToString("x2"));
            return sBuilder.ToString();
        }

        //public static string MD5EncodeString(string str)
        //{
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        //}

        //public static string SHA1EncodeString(string str)
        //{
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");
        //}

        //public static string SHA512EncodeString(string str)
        //{
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA512");
        //}

        public static string GenerateStrongPassword(int length)
        {
            var alphaCaps = "QWERTYUIOPASDFGHJKLZXCVBNM";
            var alphaLow = "qwertyuiopasdfghjklzxcvbnm";
            var numerics = "1234567890";
            var special = "@#$";

            var allChars = alphaCaps + alphaLow + numerics + special;
            var r = new Random();
            var generatedPassword = "";

            if (length < 4)
                throw new Exception("Number of characters should be greater than 4.");

            int pLower, pUpper, pNumber, pSpecial;
            var posArray = "0123456789";
            if (length < posArray.Length)
                posArray = posArray.Substring(0, length);
            pLower = GetRandomPosition(ref posArray, r);
            pUpper = GetRandomPosition(ref posArray, r);
            pNumber = GetRandomPosition(ref posArray, r);
            pSpecial = GetRandomPosition(ref posArray, r);


            for (var i = 0; i < length; i++)
                if (i == pLower)
                    generatedPassword += GetRandomChar(alphaCaps, r);
                else if (i == pUpper)
                    generatedPassword += GetRandomChar(alphaLow, r);
                else if (i == pNumber)
                    generatedPassword += GetRandomChar(numerics, r);
                else if (i == pSpecial)
                    generatedPassword += GetRandomChar(special, r);
                else
                    generatedPassword += GetRandomChar(allChars, r);
            return generatedPassword;
        }

        private static string GetRandomChar(string fullString, Random r)
        {
            return fullString.ToCharArray()[(int)Math.Floor(r.NextDouble() * fullString.Length)].ToString();
        }

        private static int GetRandomPosition(ref string posArray, Random r)
        {
            int pos;
            var randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble()
                                                                    * posArray.Length)].ToString();
            pos = int.Parse(randomChar);
            posArray = posArray.Replace(randomChar, "");
            return pos;
        }

        public static string RandomStringNumber(int codeCount)
        {
            var allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            var allCharArray = allChar.Split(',');
            var randomCode = "";
            var temp = -1;

            var rand = new Random();
            for (var i = 0; i < codeCount; i++)
            {
                if (temp != -1) rand = new Random(i * temp * (int)DateTime.Now.Ticks);
                var t = rand.Next(36);
                if (temp != -1 && temp == t) return RandomStringNumber(codeCount);
                temp = t;
                randomCode += allCharArray[t];
            }

            return randomCode;
        }

        //public static string RandomNumber(int codeCount)
        //{
        //    var allChar = "0,1,2,3,4,5,6,7,8,9";
        //    var allCharArray = allChar.Split(',');
        //    var randomCode = "";
        //    var temp = -1;

        //    var rand = new Random();
        //    for (var i = 0; i < codeCount; i++)
        //    {
        //        if (temp != -1) rand = new Random(i * temp * (int)DateTime.Now.Ticks);
        //        var t = rand.Next(10);
        //        if (temp != -1 && temp == t) return RandomNumber(codeCount);
        //        temp = t;
        //        randomCode += allCharArray[t];
        //    }

        //    return randomCode;
        //}

        public static string RandomNumber(int length)
        {
            const string digits = "0123456789";
            var result = new char[length];
            int lastDigit = -1;

            for (int i = 0; i < length; i++)
            {
                int digit;
                do
                {
                    digit = _random.Next(10);
                } while (i > 0 && digit == lastDigit); // chỉ tránh trùng liên tiếp

                result[i] = digits[digit];
                lastDigit = digit;
            }

            return new string(result);
        }
        public static string Trim(object obj)
        {
            var result = "";
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    return obj.ToString().Trim();
            return result;
        }

        public static string FormatCurrencyFromString(object num)
        {
            if (num == null || num.ToString() == "0") return num?.ToString();
            try
            {
                var rs = $"{Convert.ToDecimal(num):0,0}";
                return rs.Replace(".", ",");
            }
            catch (Exception)
            {
                return num.ToString();
            }
        }

        public static DataTable SplitToTable(string sData, char[] separators = null)
        {
            var tableData = new DataTable();
            tableData.Columns.Add("Val1");
            tableData.Columns.Add("Val2");
            tableData.Columns.Add("Val3");

            if (string.IsNullOrEmpty(sData)) return tableData;
            if (separators == null)
            {
                separators = new char[] { };
                separators[0] = ',';
            }

            var separator = separators[0];
            var dataVals = sData.Split(separator);

            if (separators.Length == 1)
            {
                foreach (var val in dataVals) tableData.Rows.Add(val, null, null);
            }
            else if (separators.Length == 2)
            {
                var secondSeparator = separators[1];
                foreach (var val in dataVals)
                {
                    var splitVal = val.Split(secondSeparator);
                    tableData.Rows.Add(splitVal[0], splitVal[1], null);
                }
            }

            return tableData;
        }

        public static bool IsValidEmail(string email)
        {
            var pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" +
                          @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" +
                          @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        /// <summary>
        ///     Lấy giá trị từ chuỗi dựa vào mẫu
        /// </summary>
        /// <param name="sTemplate"></param>
        /// <param name="sContents"></param>
        /// <returns></returns>
        public static List<string> GetValueFromStringBaseOnTemplate(string sTemplate, string sContents)
        {
            var pattern = "^" + Regex.Replace(sTemplate, @"\{[0-9]+\}", "(.*?)") + "$";

            var r = new Regex(pattern);
            var m = r.Match(sContents);

            var ret = new List<string>();

            for (var i = 1; i < m.Groups.Count; i++) ret.Add(m.Groups[i].Value);

            return ret;
        }

        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) return false;
            strInput = strInput.Trim();
            if (strInput.StartsWith("{") && strInput.EndsWith("}") || //For object
                strInput.StartsWith("[") && strInput.EndsWith("]")) //For array
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            return false;
        }

        public static string ToLowerFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }

        /// <summary>
        /// Kiểm tra định dạng số điện thoại Việt Nam
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        //public static bool IsVietnamesePhoneNumber(string phone)
        //{
        //    if (string.IsNullOrWhiteSpace(phone))
        //        return false;

        //    // Loại bỏ khoảng trắng, dấu chấm, hoặc gạch
        //    phone = phone.Replace(" ", "").Replace(".", "").Replace("-", "");

        //    // Regex kiểm tra số di động Việt Nam (03,05,07,08,09 hoặc +84 tương ứng)
        //    var regex = new Regex(@"^(?:\+84|84|0)(3|5|7|8|9)\d{8}$");

        //    return regex.IsMatch(phone);
        //}
    

        public static bool IsVietnamesePhoneNumber(string phone, string[] prefixes)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // 1. Chuẩn hóa: bỏ khoảng trắng, dấu chấm, gạch, dấu +
            phone = phone.Trim();
            phone = phone.Replace(" ", "")
                         .Replace(".", "")
                         .Replace("-", "")
                         .Replace("(", "")
                         .Replace(")", "");

            // 2. Nếu có dạng +84xxxxxxxxx → chuyển về 0xxxxxxxxx
            if (phone.StartsWith("+84"))
            {
                phone = "0" + phone.Substring(3);
            }

            // 3. Nếu có dạng 84xxxxxxxxx → chuyển về 0xxxxxxxxx
            if (phone.StartsWith("84") && phone.Length == 11)
            {
                phone = "0" + phone.Substring(2);
            }

            // 4. Phải đúng 10 số
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{10}$"))
                return false;

            // 5. Lấy 3 số đầu để kiểm tra
            string prefix = phone.Substring(0, 3);

            return prefixes.Contains(prefix);
        }

    }
}