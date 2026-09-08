using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.PushNotification.Extractor
{
    public class MessageIdExtractor
    {
        public static string ExtractMessageId(string name)
        {
            // Chuỗi đầu vào ví dụ
            // "projects/cskh-khawassco-app/messages/0:1762835290639576%5cee29645cee2964"

            // Phương thức hiệu quả nhất: Tìm vị trí của "/messages/" và lấy chuỗi con sau đó.
            string searchString = "/messages/";
            int startIndex = name.IndexOf(searchString);

            if (startIndex != -1)
            {
                // Bắt đầu từ sau "/messages/"
                return name.Substring(startIndex + searchString.Length);
            }
            else
            {
                // Trả về chuỗi rỗng hoặc ném lỗi nếu chuỗi không đúng định dạng
                return string.Empty;
            }
        }

        public static string ExtractMessageIdUsingSplit(string name)
        {
            // Tách chuỗi bằng ký tự '/'
            string[] parts = name.Split('/');

            // Message ID luôn là phần tử cuối cùng (phần tử thứ 5 nếu tính từ 0)
            // Ví dụ: [projects, cskh-khawassco-app, messages, 0:...]
            if (parts.Length >= 4)
            {
                return parts[parts.Length - 1];
            }

            return string.Empty;
        }
    }
}