using System.Xml.Serialization;
using static SMS_TYNB.ViewModel.ApiModel.ContractApiViewModel;

namespace SMS_TYNB.ViewModel.ApiModel
{
    public class SmsTemplate
    {
        [XmlRoot(ElementName = "RQST")]
        public class GetSmsTemplateApiRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "get_template";

            [XmlElement(ElementName = "REQID")]
            public string ReqId { get; set; }

            [XmlElement(ElementName = "AGENTID")]
            public string AgentId { get; set; }

            [XmlElement(ElementName = "LABELID")]
            public string LabelId { get; set; }

            [XmlElement(ElementName = "APIUSER")]
            public string ApiUser { get; set; }

            [XmlElement(ElementName = "APIPASS")]
            public string ApiPass { get; set; }
        }
        [XmlRoot(ElementName = "RPLY")]
        public class GetSmsTemplateApiResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }

            [XmlElement(ElementName = "TEMPLATEDETAIL")]
            public List<TEMPLATEDETAIL> ContractDetails { get; set; } = new List<TEMPLATEDETAIL>();
        }
        public class TEMPLATEDETAIL
        {
            [XmlElement(ElementName = "TEMPLATEID")]
            public long TemplateId { get; set; }
            [XmlElement(ElementName = "TEMPLATETYPE")]
            public string TemplateType { get; set; }
            [XmlElement(ElementName = "TEMPLATECONTENT")]
            public string TemplateContent { get; set; }
            [XmlElement(ElementName = "TOTALPARAM")]
            public int TotalParam { get; set; }
        }
    }
}
