using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TSFramework.Libs.Processors;

namespace TSFramework.Libs.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        private string _displayName;

        public CustomRequiredAttribute()
        {
            try
            {
                ErrorMessage = AppProcessor.Messagor?.GetMessage("Common_RequiredMessage");
            }
            catch
            {
                // Safe fallback neu chay ngoai HttpContext hoac AppProcessor chua khoi tao
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = string.Format(FormatErrorMessage(metadata.GetDisplayName()), metadata.GetDisplayName()),
                ValidationType = "required"
            };
            return new[] { rule };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var attributes = validationContext.ObjectType.GetProperty(validationContext.MemberName)
                ?.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            _displayName = attributes != null && attributes.Length > 0
                ? ((DisplayNameAttribute)attributes[0]).DisplayName
                : validationContext.DisplayName;

            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            _displayName = name;
            return string.Format(ErrorMessageString, name);
        }
    }

    public class RequiredIf : RequiredAttribute, IClientValidatable
    {
        private string _displayName;

        public RequiredIf(string otherProperty, object otherPropertyValue)
        {
            ErrorMessage = AppProcessor.Messagor.GetMessage("Common_RequiredMessage");
            OtherProperty = otherProperty;
            OtherPropertyValue = otherPropertyValue;
        }

        public string OtherProperty { get; }
        public object OtherPropertyValue { get; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()), ValidationType = "requiredif"
            };
            // data-val-requiredif
            rule.ValidationParameters.Add("other", OtherProperty); // data-val-requiredif-other
            rule.ValidationParameters.Add("otherval", OtherPropertyValue); // data-val-requiredif-otherval

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null) return new ValidationResult($"Unknown property {OtherProperty}");
            var attributes = validationContext.ObjectType.GetProperty(validationContext.MemberName)
                ?.GetCustomAttributes(typeof(DisplayNameAttribute), true);

            _displayName = attributes != null && attributes.Length > 0
                ? ((DisplayNameAttribute)attributes[0]).DisplayName
                : validationContext.DisplayName;

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (Equals(OtherPropertyValue, otherValue)) // if other property has the configured value
                return base.IsValid(value, validationContext);

            return null;
        }

        public override string FormatErrorMessage(string name)
        {
            _displayName = name;
            return string.Format(ErrorMessageString, name);
        }
    }

    public class RequiredIfNot : RequiredAttribute, IClientValidatable
    {
        private string _displayName;

        public RequiredIfNot(string otherProperty, object otherPropertyValue)
        {
            OtherProperty = otherProperty;
            OtherPropertyValue = otherPropertyValue;
            ErrorMessage = AppProcessor.Messagor.GetMessage("Common_RequiredMessage");
        }

        public string OtherProperty { get; }
        public object OtherPropertyValue { get; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredifnot"
            };
            // data-val-requiredif
            rule.ValidationParameters.Add("other", OtherProperty); // data-val-requiredif-other
            rule.ValidationParameters.Add("otherval", OtherPropertyValue); // data-val-requiredif-otherval

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null) return new ValidationResult($"Unknown property {OtherProperty}");
            var attributes = validationContext.ObjectType.GetProperty(validationContext.MemberName)
                ?.GetCustomAttributes(typeof(DisplayNameAttribute), true);

            _displayName = attributes != null && attributes.Length > 0
                ? ((DisplayNameAttribute)attributes[0]).DisplayName
                : validationContext.DisplayName;
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (!Equals(OtherPropertyValue, otherValue)) // if other property has the configured value
                return base.IsValid(value, validationContext);

            return null;
        }

        public override string FormatErrorMessage(string name)
        {
            _displayName = name;
            return string.Format(ErrorMessageString, name);
        }
    }
}