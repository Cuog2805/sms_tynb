using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SMS_TYNB.Models.Master;
using SMS_TYNB.ViewModel.ApiModel;
using Sprache;
using System.Text.Json.Nodes;
using System.Text;
using static SMS_TYNB.ViewModel.ApiModel.ContractApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsStatusViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsTemplate;

namespace SMS_TYNB.Helper
{
    public static class SmsHelper
    {
        private static string GetRequestId()
        {
            var reqId = $"REQ_{DateTime.Now.Ticks.ToString().Substring(10)}";
            return reqId;
        }
        public static SmsRes SendSms(SmsConfig config, string paramContent, string phoneList)
        {
            var res = new SmsRes();

            try
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Post, "http://123.31.36.151:8888/smsbn/api");

                var reqId = GetRequestId();

                var requestObj = new SmsRequestWrapper
                {
                    RQST = new SmsRequestData
                    {
                        name = "send_sms_list",
                        REQID = reqId,
                        LABELID = config.LabelId,
                        CONTRACTID = config.ContractId,
                        CONTRACTTYPEID = ((int)ContractTypeEnum.CSKH).ToString(),
                        TEMPLATEID = config.TemplateId,
                        PARAMS = new List<SmsPar>
                        {
                            new SmsPar { NUM = "1", CONTENT = paramContent }
                        },
                        SCHEDULETIME = "",
                        MOBILELIST = phoneList,
                        ISTELCOSUB = config.IsTelCoSub,
                        AGENTID = config.AgentId.ToString(),
                        APIUSER = config.ApiUser,
                        APIPASS = config.ApiPass,
                        USERNAME = config.UserName,
                        DATACODING = config.DataCoding
                    }
                };

                var jsonBody = JsonConvert.SerializeObject(requestObj);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using var response = client.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    res = new SmsRes
                    {
						REQID = GetRequestId(),
						RPLY = new SmsResponseObj
                        {
                            name = "send_sms_list",
                            ERROR = $"HTTP {response.StatusCode}",
                            ERROR_DESC = "Gửi SMS thất bại"
						}
                    };
                    return res;
                }

                var reContent = response.Content.ReadAsStringAsync().Result ?? string.Empty;
                res = JsonConvert.DeserializeObject<SmsRes>(reContent);
            }
            catch (Exception ex)
            {
                res = new SmsRes
                {
                    REQID = GetRequestId(),
                    RPLY = new SmsResponseObj
                    {
                        name = "send_sms_list",
                        ERROR = "exception",
                        ERROR_DESC = ex.Message
                    }
                };
            }

            return new SmsRes
            {
				REQID = GetRequestId(),
				RPLY = res.RPLY
			};
        }
    }
}
