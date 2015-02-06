using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHubServices.Models.TocElements
{
    public abstract class TocElement
    {
        public int Number;

        public string Title;

        public abstract string ProduceMarkdown();
    }

    public class Chapter : TocElement
    {
        public override string ProduceMarkdown()
        {
            throw new NotImplementedException();
        }
    }

    public class Section : TocElement
    {
        public override string ProduceMarkdown()
        {
            throw new NotImplementedException();
        }
    }

    public class Sub1Section : TocElement
    {
        public override string ProduceMarkdown()
        {
            throw new NotImplementedException();
        }
    }

    public class Sub2Section : TocElement
    {
        public override string ProduceMarkdown()
        {
            throw new NotImplementedException();
        }
    }

    public class Sub3Section : TocElement
    {
        public override string ProduceMarkdown()
        {
            throw new NotImplementedException();
        }
    }

    public class Sub4Section : TocElement
    {
        public override string ProduceMarkdown()
        {
            throw new NotImplementedException();
        }
    }
}