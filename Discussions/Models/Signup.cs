using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Discussions.Models
{
    public class Signup
    {
        public string Firstname{ get; set; }
        public string Lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}