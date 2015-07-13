namespace GitHubServices.BusinessLogic.TagPageCreator.Domain
{
    public class Tag : StringDomainObject
    {
        public Tag(string value)
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
    }
}