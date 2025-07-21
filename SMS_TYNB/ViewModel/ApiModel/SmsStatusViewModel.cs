using System.Xml.Serialization;

namespace SMS_TYNB.ViewModel.ApiModel
{
    public class SmsStatusViewModel
    {
        public class GetSmsStatusRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "get_sms_status_api";

            [XmlElement(ElementName = "REQID")]
            public string ReqId { get; set; }

            [XmlElement(ElementName = "AGENTID")]
            public string AgentId { get; set; }

            [XmlElement(ElementName = "CONTRACTID")]
            public string ContractId { get; set; }

            [XmlElement(ElementName = "SENDTIME")]
            public string SendTime { get; set; }

            [XmlElement(ElementName = "APIUSER")]
            public string ApiUser { get; set; }

            [XmlElement(ElementName = "APIPASS")]
            public string ApiPass { get; set; }
        }

        // Model cho response
        [XmlRoot(ElementName = "RPLY")]
        public class GetSmsStatusResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }
        }

    }
}
