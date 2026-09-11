using System.ComponentModel;
using TSFramework.Libs.Processors;

namespace TSFramework.Libs.Attributes
{
    public class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _resourceName;

        public CustomDisplayNameAttribute(string resourceName) : base(resourceName ?? string.Empty)
        {
            _resourceName = resourceName;
        }

        public override string DisplayName
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_resourceName) && AppProcessor.Messagor != null)
                    {
                        var msg = AppProcessor.Messagor.GetMessage(_resourceName);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            return msg;
                        }
                    }
                }
                catch
                {
                    // Fallback an toan neu co ngoai le doc resource
                }

                if (!string.IsNullOrEmpty(_resourceName))
                {
                    return _resourceName;
                }

                return !string.IsNullOrEmpty(DisplayNameValue) ? DisplayNameValue : string.Empty;
            }
        }
    }
}