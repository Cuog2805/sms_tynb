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
            public int AgentId { get; set; }

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
        public class SmsResponse
        {
            public string Name { get; set; }
            public string Error { get; set; }
            public string ErrorDesc { get; set; }
        }
        public enum ContractTypeEnum
        {
            QC = 2,
            CSKH = 1
        }
        public class SmsRequestData
        {
            public string name { get; set; }
            public string REQID { get; set; }
            public string LABELID { get; set; }
            public string CONTRACTID { get; set; }
            public string CONTRACTTYPEID { get; set; }
            public string TEMPLATEID { get; set; }
            public List<SmsPar> PARAMS { get; set; }
            public string SCHEDULETIME { get; set; }
            public string MOBILELIST { get; set; }
            public string ISTELCOSUB { get; set; }
            public string AGENTID { get; set; }
            public string APIUSER { get; set; }
            public string APIPASS { get; set; }
            public string USERNAME { get; set; }
            public string DATACODING { get; set; }
        }
        public class SmsPar
        {
            public string NUM { get; set; }
            public string CONTENT { get; set; }
        }
        public class SmsRequestWrapper
        {
            public SmsRequestData RQST { get; set; }
        }
        public class SmsRes
        {
            public SmsResponseObj RPLY { get; set; }
        }
        public class SmsResponseObj
        {
            public string ERROR_DESC { get; set; }
            public string name { get; set; }
            public string ERROR { get; set; }
        }
    }
}
