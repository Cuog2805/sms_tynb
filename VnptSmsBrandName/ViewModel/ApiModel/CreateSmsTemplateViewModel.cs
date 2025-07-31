using System.Xml.Serialization;

namespace VnptSmsBrandName.ViewModel.ApiModel
{
    public class CreateSmsTemplateViewModel
    {
        [XmlRoot(ElementName = "RQST")]
        public class CreateSmsTemplateApiRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "create_template";

            [XmlElement(ElementName = "REQID")]
            public string ReqId { get; set; }

            [XmlElement(ElementName = "AGENTID")]
            public string AgentId { get; set; }
            [XmlElement(ElementName = "APIUSER")]
            public string ApiUser { get; set; }

            [XmlElement(ElementName = "APIPASS")]
            public string ApiPass { get; set; }

            [XmlElement(ElementName = "CONTENT")]
            public string Content { get; set; }

            [XmlElement(ElementName = "TOTALPARAMS")]
            public string TotalParams { get; set; }

            [XmlElement(ElementName = "USERNAME")]
            public string UserName { get; set; }
        }
        [XmlRoot(ElementName = "RPLY")]
        public class CreateSmsTemplateApiResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }

            [XmlElement(ElementName = "TEMPLATEID")]
            public string TemplateId  { get; set; }
        }
    }
}
