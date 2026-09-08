using System;

namespace Modules.API.Helpers
{
    public class FKeyHelper
    {
        public static string GenFKey()
        {
            long i = 1;
            foreach (var b in Guid.NewGuid().ToByteArray()) i *= b + 1;
            return $"{i - DateTime.Now.Ticks:x}".ToUpper();
        }
    }
}