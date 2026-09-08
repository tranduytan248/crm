using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CenIT.Solution.TOC.WebApp.Models
{
    [XmlRoot(ElementName = "Invoices")]
    public class Invoices
    {
        [XmlElement(ElementName = "InvItem")] public List<InvSiteModel> ListInvItems { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "InvItem")]
    public class InvSiteModel
    {
        [XmlElement(ElementName = "Host")] public string Host { get; set; }

        [XmlElement(ElementName = "EmpAcc")] public string EmpAcc { get; set; }

        [XmlElement(ElementName = "EmpAccPw")] public string EmpAccPw { get; set; }

        [XmlElement(ElementName = "ServiceAcc")]
        public string ServiceAcc { get; set; }

        [XmlElement(ElementName = "ServiceAccPw")]
        public string ServiceAccPw { get; set; }
    }
}