using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Framework.Localization;

namespace Microsoft.AspNet.Mvc.Localization
{
    public class ValidationAttributeLocalizer<TAttribute, TResourceSource> : IValidationAttributeLocalizer<TAttribute, TResourceSource>
        where TAttribute : ValidationAttribute
    {
        private readonly IStringLocalizer _localizer;
        private readonly IValidationLocalizer<TAttribute> _attributeLocalizer;

        public ValidationAttributeLocalizer(IStringLocalizer<TResourceSource> localizer, IValidationLocalizer<TAttribute> attributeLocalizer)
        {
            _localizer = localizer;
            _attributeLocalizer = attributeLocalizer;
        }

        public LocalizedString FormatMessage(ValidationAttributeLocalizationContext<TAttribute> context)
        {
            return _attributeLocalizer.FormatMessage(context);
        }
    }

    public class ValidationAttributeLocalizer<TAttribute> : IValidationLocalizer<TAttribute>
        where TAttribute : ValidationAttribute
    {
        public virtual LocalizedString FormatMessage(ValidationAttributeLocalizationContext<TAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName);
        }
    }

    public class RequiredAttributeLocalizer : ValidationAttributeLocalizer<RequiredAttribute>
    {

    }

    public class CreditCardAttributeLocalizer : ValidationAttributeLocalizer<CreditCardAttribute>
    {

    }

    public class EmailAddressAttributeLocalizer : ValidationAttributeLocalizer<EmailAddressAttribute>
    {

    }

    public class PhoneAttributeLocalizer : ValidationAttributeLocalizer<PhoneAttribute>
    {

    }

    public class UrlAttributeLocalizer : ValidationAttributeLocalizer<UrlAttribute>
    {

    }

    public class EnumDataTypeAttributeLocalizer : ValidationAttributeLocalizer<EnumDataTypeAttribute>
    {

    }

    public class FileExtensionsAttributeLocalizer : ValidationAttributeLocalizer<FileExtensionsAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<FileExtensionsAttribute> context)
        {
            var extensionsNormalized = context.Attribute.Extensions.Replace(" ", "").Replace(".", "").ToLowerInvariant();
            var extensionsParsed = extensionsNormalized.Split(',').Select(e => "." + e);
            var extensionsFormatted = extensionsParsed.Aggregate((left, right) => left + ", " + right);

            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                extensionsFormatted);
        }
    }

    public class StringLengthAttributeLocalizer : ValidationAttributeLocalizer<StringLengthAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<StringLengthAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                context.Attribute.MinimumLength,
                context.Attribute.MaximumLength);
        }
    }

    public class RangeAttributeLocalizer : ValidationAttributeLocalizer<RangeAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<RangeAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                context.Attribute.Minimum,
                context.Attribute.Maximum);
        }
    }

    public class CompareAttributeLocalizer : ValidationAttributeLocalizer<CompareAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<CompareAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                context.Attribute.OtherPropertyDisplayName ?? context.Attribute.OtherProperty);
        }
    }

    public class MaxLengthAttributeLocalizer : ValidationAttributeLocalizer<MaxLengthAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<MaxLengthAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                context.Attribute.Length);
        }
    }

    public class MinLengthAttributeLocalizer : ValidationAttributeLocalizer<MaxLengthAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<MaxLengthAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                context.Attribute.Length);
        }
    }

    public class RegularExpressionAttributeLocalizer : ValidationAttributeLocalizer<RegularExpressionAttribute>
    {
        public override LocalizedString FormatMessage(ValidationAttributeLocalizationContext<RegularExpressionAttribute> context)
        {
            return context.Localizer.GetString(
                context.Attribute.ErrorMessage,
                context.ValidationContext.MemberName,
                context.Attribute.Pattern);
        }
    }
}