using System;

namespace GitHubServices.BusinessLogic
{
    public class ExceptionHelper
    {
        public void Print(AggregateException e)
        {
            foreach (var inner in e.InnerExceptions)
            {
                Print(inner, "");
            }
        }


        public void Print(Exception e, string indent)
        {
            if (e == null)
                return;
            Console.WriteLine(indent + e.GetType() + "::" + e.Message);
            Print(e.InnerException, "  " + indent);
        }
    }
}