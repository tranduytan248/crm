using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.API.Models
{
    public class BaseApiModel
    {
        public string Signature { get; set; }

        public string ApplyFor { get; set; }

        public string Token { get; set; }

        public string UserName { get; set; }
    }
}