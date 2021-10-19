using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSprospectos.Models
{
    public class LoginRequest
    {
        public string User    { get; set; }
        public string Password { get; set; }
    }
}