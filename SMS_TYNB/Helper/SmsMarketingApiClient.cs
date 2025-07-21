using RestSharp;
using SMS_TYNB.ViewModel.ApiModel;
using static SMS_TYNB.ViewModel.ApiModel.ContractApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.CreateSmsTemplateViewModel;
using static SMS_TYNB.ViewModel.ApiModel.LabelApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.PrepaidOrdersApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsStatusViewModel;

namespace SMS_TYNB.Helper
{
    public class SmsMarketingApiClient
    {
        private readonly RestClient _client;
        private readonly bool _useJson;
        private readonly string _baseUrl;

        public SmsMarketingApiClient(string baseUrl, bool useJson = false)
        {
            _baseUrl = baseUrl;
            _useJson = useJson;
            _client = new RestClient(baseUrl);
        }
        public async Task<TResponse> CallApiAsync<TRequest, TResponse>(TRequest request, string endpoint)
        where TRequest : class
        where TResponse : class, new()
        {
            var restRequest = new RestRequest(endpoint, Method.Post);

            if (_useJson)
            {
                restRequest.AddHeader("Content-Type", "application/json;charset=UTF-8");
                restRequest.AddJsonBody(request);
            }
            else
            {
                restRequest.AddHeader("Content-Type", "text/xml;charset=UTF-8");
                restRequest.AddXmlBody(request);
            }

            var response = await _client.ExecuteAsync<TResponse>(restRequest);

            if (!response.IsSuccessful)
            {
                throw new Exception($"API call failed: {response.StatusCode} - {response.ErrorMessage}");
            }

            return response.Data;
        }

        /// <summary>
        /// lấy danh sách các khách hàng
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="agentId"></param>
        /// <param name="apiUser"></param>
        /// <param name="apiPass"></param>
        /// <returns></returns>
        public async Task<GetAdserResponse> GetAdserAsync(string requestId, int agentId, string apiUser, string apiPass)
        {
            var request = new AdserApiViewModel
            {
                Name = "get_adser",
                ReqId = requestId,
                AgentId = agentId,
                ApiUser = apiUser,
                ApiPass = apiPass
            };

            return await CallApiAsync<AdserApiViewModel, GetAdserResponse>(request, "smsmarketing/api");
        }
        /// <summary>
        /// Lấy danh sách hợp đồng
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="agentId"></param>
        /// <param name="adserId"></param>
        /// <param name="apiUser"></param>
        /// <param name="apiPass"></param>
        /// <returns></returns>
        public async Task<GetContractResponse> GetContractsAsync(string requestId, string agentId, string adserId,
            string apiUser, string apiPass)
        {
            var request = new GetContractRequest
            {
                Name = "get_contract",
                ReqId = requestId,
                AgentId = agentId,
                AdserId = adserId,
                ApiUser = apiUser,
                ApiPass = apiPass
            };

            return await CallApiAsync<GetContractRequest, GetContractResponse>(request, "smsmarketing/api");
        }
        /// <summary>
        /// hàm gửi tin nhắn theo danh sách
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendSmsResponse> SendSmsAsync(SendSmsRequest request)
        {
            return await CallApiAsync<SendSmsRequest, SendSmsResponse>(request, "smsmarketing/api");
        }
        /// <summary>
        /// hàm lấy trạng thái tin nhắn đã gửi
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="agentId"></param>
        /// <param name="contractId"></param>
        /// <param name="sendTime"></param>
        /// <param name="apiUser"></param>
        /// <param name="apiPass"></param>
        /// <returns></returns>
        public async Task<GetSmsStatusResponse> GetSmsStatusAsync(string requestId, string agentId, string contractId, string sendTime, string apiUser, string apiPass)
        {
            var request = new GetSmsStatusRequest
            {
                Name = "get_sms_status",
                ReqId = requestId,
                AgentId = agentId,
                ContractId = contractId,
                SendTime = sendTime,
                ApiUser = apiUser,
                ApiPass = apiPass
            };

            return await CallApiAsync<GetSmsStatusRequest, GetSmsStatusResponse>(request, "smsmarketing/api");
        }
        /// <summary>
        /// lấy danh sách label
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="agentId"></param>
        /// <param name="contractId"></param>
        /// <param name="sendTime"></param>
        /// <param name="apiUser"></param>
        /// <param name="apiPass"></param>
        /// <returns></returns>
        public async Task<GetLabelApiResponse> GetLabelAsync(string requestId, string agentId, string contractId, string sendTime, string apiUser, string apiPass)
        {
            var request = new GetLabelApiRequest
            {
                Name = "get_label",
                ReqId = requestId,
                AgentId = agentId,
                ContractId = contractId,
                SendTime = sendTime,
                ApiUser = apiUser,
                ApiPass = apiPass
            };

            return await CallApiAsync<GetLabelApiRequest, GetLabelApiResponse>(request, "smsmarketing/api");
        }
        /// <summary>
        /// hàm tạo template qua API
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="agentId"></param>
        /// <param name="apiUser"></param>
        /// <param name="apiPass"></param>
        /// <param name="content"></param>
        /// <param name="totalParam"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<CreateSmsTemplateApiResponse> CreateSmsTemplateAsync(string requestId, string agentId,
            string apiUser, string apiPass, string content, string totalParam, string userName)
        {
            var request = new CreateSmsTemplateApiRequest
            {
                Name = "create_sms_template",
                ReqId = requestId,
                AgentId = agentId,
                ApiUser = apiUser,
                ApiPass = apiPass,
                Content = content,
                TotalParams = totalParam,
                UserName = userName
            };

            return await CallApiAsync<CreateSmsTemplateApiRequest, CreateSmsTemplateApiResponse>(request, "smsmarketing/api");
        }
        public async Task<GetPrepaidOrdersApiResponse> GetPrepaidOrdersAsync(string agentId,
            string apiUser, string apiPass, string contractId, string pakageId)
        {
            var request = new GetPrepaidOrdersApiRequest
            {
                Name = "get_prepaid_orders",
                AgentId = agentId,
                ApiUser = apiUser,
                ApiPass = apiPass,
                ContractId = contractId,
                PackageId = pakageId,
            };

            return await CallApiAsync<GetPrepaidOrdersApiRequest, GetPrepaidOrdersApiResponse>(request, "smsmarketing/api");
        }
    }
}
