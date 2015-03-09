using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeroHedge
{
    public class Story
    {
        public int ID { get; set; }
        public string Title {get; set;}
        public string Body { get; set; }
        public string Reference { get; set; }
        public string Published { get; set; }
        public DateTime Updated { get; set; }
    }
}