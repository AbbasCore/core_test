using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webcore2.Models
{
    public class addresses
    {
        public int id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
