using SMS_TYNB.Models.Master;
using SMS_TYNB.ViewModel.ApiModel;
using static SMS_TYNB.ViewModel.ApiModel.ContractApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsStatusViewModel;

namespace SMS_TYNB.Helper
{
    public static class SmsHelper
    {
        private readonly static SmsMarketingApiClient apiClient = new SmsMarketingApiClient("http://123.31.36.151:8888", useJson: false);
        private static string GetRequestId()
        {
            var requestId = (DateTime.Now.Ticks / 10000).ToString();
            var reqId = $"REQ_{DateTime.Now.Ticks.ToString().Substring(10)}";
            return reqId;
        }
        public static async Task<GetAdserResponse> GetAdserAsync(int agentId, string apiUser, string apiPass)
        {
            var result = new GetAdserResponse();
            try
            {
                var reqId = GetRequestId();
                var response = await apiClient.GetAdserAsync(
                    requestId: reqId,
                    agentId: agentId,
                    apiUser: apiUser,
                    apiPass: apiPass);

                if (response.Error == "0")
                {
                    result = response;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static async Task<GetContractResponse> GetContractAsync(string agentId, string adserId,
            string apiUser, string apiPass)
        {
            var result = new GetContractResponse();
            try
            {
                // 1. Lấy danh sách hợp đồng
                var reqId = GetRequestId();
                var contractRes = await apiClient.GetContractsAsync(
                    requestId: reqId,
                    agentId: agentId,
                    adserId: adserId,
                    apiUser: apiUser,
                    apiPass: apiPass);
                if (contractRes.Error == "0")
                {
                    result = contractRes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// lấy trạng thái tin nhắn
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="contractId"></param>
        /// <param name="apiUser"></param>
        /// <param name="apiPass"></param>
        /// <returns></returns>
        public static async Task<GetSmsStatusResponse> GetSmsStatusAsync(string agentId, string contractId, string apiUser, string apiPass)
        {
            var result = new GetSmsStatusResponse();
            try
            {
                // 1. Lấy danh sách hợp đồng
                var reqId = GetRequestId();
                // 3. Kiểm tra trạng thái
                var status = await apiClient.GetSmsStatusAsync(
                    requestId: reqId,
                    agentId: agentId,
                    contractId: contractId,
                    sendTime: DateTime.Now.ToString("dd/MM/yyyy"),
                    apiUser: apiUser,
                    apiPass: apiPass);
                if (status.Error == "0")
                {
                    result = status;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            return result;
        }
        public static async Task<SendSmsResponse> SendSmsAsync(SmsConfig config, ContractTypeEnum contractType,
            string mobileList, string agentId, string apiUser, string apiPass, string paramContent)
        {
            var result = new SendSmsResponse();
            try
            {
                var reqId = GetRequestId();
                var smsRequest = new SendSmsRequest
                {
                    ReqId = reqId,
                    LabelId = config.LabelId,
                    ContractId = config.ContractId,
                    ContractTypeId = (int)contractType, // 1: CSKH, 2: QC
                    TemplateId = config.TemplateId,
                    MobileList = mobileList,
                    AgentId = agentId,
                    ApiUser = apiUser,
                    ApiPass = apiPass,
                    UserName = config.UserName,
                    Params = new List<SmsParam>
                    {
                        new SmsParam { Number = 1, Content = paramContent }
                    },
                    ScheduleTime = string.Empty,
                    IsTelcoSub = config.IsTelCoSub,
                    DataCoding = config.DataCoding,
                    SaleOrderId = config.SaleOrderId,
                    PackageId = string.Empty,
                };
                var sendResult = await apiClient.SendSmsAsync(smsRequest);
                if (sendResult.Error == "0")
                {
                    result = sendResult;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            return result;
        }
    }
}
