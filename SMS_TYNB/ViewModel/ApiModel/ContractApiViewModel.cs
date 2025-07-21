using System.Xml.Serialization;

namespace SMS_TYNB.ViewModel.ApiModel
{
    public class ContractApiViewModel
    {
        public class GetContractRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "get_contract";

            [XmlElement(ElementName = "REQID")]
            public string ReqId { get; set; }

            [XmlElement(ElementName = "AGENTID")]
            public string AgentId { get; set; }

            [XmlElement(ElementName = "ADSERID")]
            public string AdserId { get; set; }

            [XmlElement(ElementName = "APIUSER")]
            public string ApiUser { get; set; }

            [XmlElement(ElementName = "APIPASS")]
            public string ApiPass { get; set; }
        }

        // Model cho response
        [XmlRoot(ElementName = "RPLY")]
        public class GetContractResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }

            [XmlElement(ElementName = "CONTRACTDETAIL")]
            public List<ContractDetail> ContractDetails { get; set; } = new List<ContractDetail>();
        }

        public class ContractDetail
        {
            [XmlElement(ElementName = "CONTRACTID")]
            public int ContractId { get; set; }

            [XmlElement(ElementName = "CONTRACTNUMBER")]
            public string ContractNumber { get; set; }

            [XmlElement(ElementName = "CONTRACTDATE")]
            public string ContractDate { get; set; }

            [XmlElement(ElementName = "STARTVALIDDATE")]
            public string StartValidDate { get; set; }

            [XmlElement(ElementName = "ENDVALIDDATED")]
            public string EndValidDated { get; set; }

            [XmlElement(ElementName = "CONTRACTNAME")]
            public string ContractName { get; set; }

            [XmlElement(ElementName = "CONTRACTTYPEID")]
            public int ContractTypeId { get; set; }

            [XmlElement(ElementName = "CONTRACTTYPENAME")]
            public string ContractTypeName { get; set; }
        }
    }
}
