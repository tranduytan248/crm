using System.ComponentModel.DataAnnotations;
using TSFramework.Libs.Processors;

namespace TSFramework.Libs.Attributes
{
    public class CustomCompareAttribute : CompareAttribute
    {
        public CustomCompareAttribute(string otherProperty) : base(otherProperty)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(AppProcessor.Messagor.GetMessage(ErrorMessage), name, OtherPropertyDisplayName);
        }
    }
}