using System.Xml.Serialization;

namespace SMS_TYNB.ViewModel.ApiModel
{
    [XmlRoot(ElementName = "RQST")]
    public class AdserApiViewModel
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "REQID")]
        public string ReqId { get; set; }

        [XmlElement(ElementName = "AGENTID")]
        public int AgentId { get; set; }

        [XmlElement(ElementName = "APIUSER")]
        public string ApiUser { get; set; }

        [XmlElement(ElementName = "APIPASS")]
        public string ApiPass { get; set; }
    }
    [XmlRoot(ElementName = "RPLY")]
    public class GetAdserResponse
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "ERROR")]
        public string Error { get; set; }

        [XmlElement(ElementName = "ERROR_DESC")]
        public string ErrorDesc { get; set; }

        [XmlElement(ElementName = "ADSERDETAIL")]
        public List<AdserDetail> AdserDetails { get; set; } = new List<AdserDetail>();
    }
    public class AdserDetail
    {
        [XmlElement(ElementName = "ADSERID")]
        public int AdserId { get; set; }

        [XmlElement(ElementName = "ADSERNAME")]
        public string AdserName { get; set; }

        [XmlElement(ElementName = "ADSERADDR")]
        public string AdserAddr { get; set; }

        [XmlElement(ElementName = "ADSERPAPER")]
        public string AdserPaper { get; set; }

        [XmlElement(ElementName = "ADSERMOBILE")]
        public string AdserMobile { get; set; }

        [XmlElement(ElementName = "ADSEREMAIL")]
        public string AdserEmail { get; set; }
    }
}
