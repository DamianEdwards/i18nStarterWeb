using System;

namespace Microsoft.Framework.Localization
{
    public struct LocalizedString
    {
        public LocalizedString(string key, string value)
            : this(key, value, resourceNotFound: false)
        {

        }

        public LocalizedString(string key, string value, bool resourceNotFound)
        {
            Key = key;
            Value = value;
            ResourceNotFound = resourceNotFound;
        }

        public static implicit operator string (LocalizedString localizedString)
        {
            return localizedString.Value;
        }

        public string Key { get; }

        public string Value { get; }

        public bool ResourceNotFound { get; }

        public override string ToString() => Value;
    }
}