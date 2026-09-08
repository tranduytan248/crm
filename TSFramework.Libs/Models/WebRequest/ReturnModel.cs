using System.Collections.Generic;

namespace TSFramework.Libs.Models.WebRequest
{
    public class ReturnModel
    {
        public int Status { get; set; }
        public string CodeError { get; set; }
        public string Message { get; set; }
        public List<object> Data { get; set; }
    }
}