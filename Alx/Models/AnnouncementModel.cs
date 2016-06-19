using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alx.Models
{
    public class AnnouncementModel
    {
        public int Id { get; set; }
        public string[] ImageString { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }

    }
}