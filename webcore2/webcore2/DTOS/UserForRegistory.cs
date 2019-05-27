using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webcore2.DTOS
{
    public class UserForLogin
    {
        [Required]
        public string username { get; set; }
        [StringLength(10, ErrorMessage = "length is bigger !!", MinimumLength = 6)]
        public string password { get; set; }
    }
}
