namespace GitHubServices.BusinessLogic.TagPageCreator.Domain
{
    public abstract class StringDomainObject
    {
        protected readonly string value;

        protected StringDomainObject(string value)
        {
            this.value = value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return string.Equals(value, ((StringDomainObject)obj).value);
        }

        public override string ToString()
        {
            return value;
        }
    }
}