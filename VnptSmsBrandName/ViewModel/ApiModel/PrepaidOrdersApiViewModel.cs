using System.Xml.Serialization;
using static SMS_TYNB.ViewModel.ApiModel.LabelApiViewModel;

namespace SMS_TYNB.ViewModel.ApiModel
{
    public class PrepaidOrdersApiViewModel
    {
        [XmlRoot(ElementName = "RQST")]
        public class GetPrepaidOrdersApiRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "get_prepaid_orders";

            [XmlElement(ElementName = "AGENTID")]
            public string AgentId { get; set; }

            [XmlElement(ElementName = "CONTRACTID")]
            public string ContractId { get; set; }

            [XmlElement(ElementName = "PACKAGEID")]
            public string PackageId { get; set; }

            [XmlElement(ElementName = "APIUSER")]
            public string ApiUser { get; set; }

            [XmlElement(ElementName = "APIPASS")]
            public string ApiPass { get; set; }
        }
        [XmlRoot(ElementName = "RPLY")]
        public class GetPrepaidOrdersApiResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }

            [XmlElement(ElementName = "DATA")]
            public List<PrepaidOrderData> Datas { get; set; } = new List<PrepaidOrderData>();
        }
        public class PrepaidOrderData
        {
            [XmlElement(ElementName = "PACKAGE_ID")]
            public long PakageId { get; set; }
            [XmlElement(ElementName = "BALANCE")]
            public string Balance { get; set; }
        }
    }
}
