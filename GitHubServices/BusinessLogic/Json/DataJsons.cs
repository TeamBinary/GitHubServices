﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHubServices.BusinessLogic.Json
{
    public class Links
    {
        public string self { get; set; }
        public string git { get; set; }
        public string html { get; set; }
    }

    public class ContentObject
    {
        public string name { get; set; }
        public string path { get; set; }
        public string sha { get; set; }
        public int size { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string git_url { get; set; }
        public string download_url { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public string encoding { get; set; }
        public Links _links { get; set; }
    }




    public class PutContent
    {
        public string message { get; set; }
        public string content{ get; set; }
        public string sha { get; set; }
    }
}