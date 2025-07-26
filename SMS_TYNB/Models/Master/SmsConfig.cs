using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master
{
    public class SmsConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public string LabelId { get; set; }
        public string ContractId { get; set; }
        public string TemplateId { get; set; }
        public string IsTelCoSub { get; set; }
        public int AgentId { get; set; }
        public string ApiUser { get; set; }
        public string ApiPass { get; set; }
        public string UserName { get; set; }
        public string DataCoding { get; set; }
        public string SaleOrderId { get; set; }
        public string PackageId { get; set; }
        public string UrlSms { get; set; }
        public bool IssEnSms { get; set; }
        public bool IsActive { get; set; }
    }
}
