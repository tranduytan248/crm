using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Modules.API.Models
{
    public class ResponseContentModel
    {
        public int Status {  get; set; }

        public string Message { get; set; }
    }
}