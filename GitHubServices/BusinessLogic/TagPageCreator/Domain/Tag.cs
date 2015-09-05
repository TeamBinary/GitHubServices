using System;

namespace GitHubServices.BusinessLogic.TagPageCreator.Domain
{
    /// <summary>
    /// attempts at using a base class to implement the general functionality
    /// </summary>
    public class Tag2 : StringDomainObject<Tag2>
    {
        public Tag2(string value)
            : base(value)
        {
        }

        public string Value
        {
            get
            {
                return value;
            }
        }

        protected override Tag2 New(string a)
        {
            return new Tag2(a);
        }
    }

    public class Tag
    {
        const int hex_ffffff = 16777215;

        public readonly string Value;
        public readonly string DisplayText;

        public readonly string HexCodeForValue;

        public Tag(string value)
        {
            Value = value;
            DisplayText = Value.Replace('_', ' ');
            var hashCode = Math.Abs(value.GetHashCode());
            var color = hashCode % hex_ffffff;
            HexCodeForValue = color.ToString("x").PadRight(6, '0');
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return string.Equals(Value, ((Tag)obj).Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}