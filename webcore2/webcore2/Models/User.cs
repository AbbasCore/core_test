using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webcore2.Models
{
    public class User
    {
        public int id { get; set; }
        public string userName { get; set; }

        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }

    }
}
