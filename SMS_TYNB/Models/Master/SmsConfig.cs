namespace SMS_TYNB.Models.Master
{
    public class SmsConfig
    {
        public long Id { get; set; }
        public string LabelId { get; set; }
        public string ContractId { get; set; }
        public string TemplateId { get; set; }
        public int IsTelCoSub { get; set; }
        public string AgentId { get; set; }
        public string ApiUser { get; set; }
        public string ApiPass { get; set; }
        public string UserName { get; set; }
        public int DataCoding { get; set; }
        public string SaleOrderId { get; set; }
        public string PakageId { get; set; }
        public string UrlSms { get; set; }
        public bool IssEnSms { get; set; }
    }
}
