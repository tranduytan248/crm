using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.API.Models
{
    public class UserAuthenCationHeaderModels
    {

        public string SessionID { get; set; }

        public string Userkey { get; set; }

        public string Ticket { get; set; }

        public string AppCode { get; set; }
        public string Email { get; set; }
    }
}