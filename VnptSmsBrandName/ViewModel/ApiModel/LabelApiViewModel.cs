using System.Xml.Serialization;
using static VnptSmsBrandName.ViewModel.ApiModel.ContractApiViewModel;

namespace VnptSmsBrandName.ViewModel.ApiModel
{
    public class LabelApiViewModel
    {
        [XmlRoot(ElementName = "RQST")]
        public class GetLabelApiRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "get_label";

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
        [XmlRoot(ElementName = "RPLY")]
        public class GetLabelApiResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }

            [XmlElement(ElementName = "LABELDETAIL")]
            public List<LabelDetail> LabelDetails { get; set; } = new List<LabelDetail>();
        }
        public class LabelDetail
        {
            [XmlElement(ElementName = "LABELID")]
            public long LabelId { get; set; }
            [XmlElement(ElementName = "LABEL")]
            public string Label { get; set; }
            [XmlElement(ElementName = "DISPLAYNUMBER")]
            public string DisplayNumber { get; set; }
        }
    }
}
