using System.Xml.Serialization;

namespace SMS_TYNB.ViewModel.ApiModel
{
    public class SmsApiViewModel
    {
        public class SendSmsRequest
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; } = "send_sms_list";

            [XmlElement(ElementName = "REQID")]
            public string ReqId { get; set; }

            [XmlElement(ElementName = "LABELID")]
            public string LabelId { get; set; }

            [XmlElement(ElementName = "CONTRACTID")]
            public string ContractId { get; set; }

            [XmlElement(ElementName = "CONTRACTTYPEID")]
            public int ContractTypeId { get; set; }

            [XmlElement(ElementName = "TEMPLATEID")]
            public string TemplateId { get; set; }

            [XmlElement(ElementName = "PARAMS")]
            public List<SmsParam> Params { get; set; } = new List<SmsParam>();

            [XmlElement(ElementName = "SCHEDULETIME")]
            public string ScheduleTime { get; set; }

            [XmlElement(ElementName = "MOBILELIST")]
            public string MobileList { get; set; }

            [XmlElement(ElementName = "ISTELCOSUB")]
            public string IsTelcoSub { get; set; } = "0";

            [XmlElement(ElementName = "AGENTID")]
            public string AgentId { get; set; }

            [XmlElement(ElementName = "APIUSER")]
            public string ApiUser { get; set; }

            [XmlElement(ElementName = "APIPASS")]
            public string ApiPass { get; set; }

            [XmlElement(ElementName = "USERNAME")]
            public string UserName { get; set; }

            [XmlElement(ElementName = "DATACODING")]
            public string DataCoding { get; set; } = "0";

            [XmlElement(ElementName = "SALEORDERID")]
            public string SaleOrderId { get; set; }

            [XmlElement(ElementName = "PACKAGEID")]
            public string PackageId { get; set; }
        }

        public class SmsParam
        {
            [XmlElement(ElementName = "NUM")]
            public int Number { get; set; }

            [XmlElement(ElementName = "CONTENT")]
            public string Content { get; set; }
        }

        // Model cho response
        [XmlRoot(ElementName = "RPLY")]
        public class SendSmsResponse
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "ERROR")]
            public string Error { get; set; }

            [XmlElement(ElementName = "ERROR_DESC")]
            public string ErrorDesc { get; set; }
        }
        public enum ContractTypeEnum
        {
            QC = 2,
            CSKH = 1
        }
    }
}
