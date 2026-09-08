using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.Helpers
{
    public static class FileHelper
    {
        public static bool IsImageExtension(string extension)
        {
            // Danh sách các phần mở rộng cho các định dạng hình ảnh thông thường
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            // Kiểm tra phần mở rộng có trong danh sách hay không
            foreach (string imageExtension in imageExtensions)
            {
                if (extension.Equals(imageExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}