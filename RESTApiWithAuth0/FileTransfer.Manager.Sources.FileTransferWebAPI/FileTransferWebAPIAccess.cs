using FileTransfer.Definitions.Dto;
using FileTransfer.WebAPI.Definitions;
using FileTransfer.WebAPI.Definitions.Dto;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public class FileTransferWebAPIAccess :
        IFileTransferWebAPIAccess
    {
        const string _audience = "https://file-transfer";
        readonly API _api;
        protected readonly HttpClient _client;

        public bool IsConnected { get; protected set; }
        public string ID { get; }

        public FileTransferWebAPIAccess(TransferSourceDto aAccessData)
        {
            this.IsConnected = false;
            _client = new HttpClient();

            this.ID = aAccessData.ID.ToString();
            _api = JsonConvert.DeserializeObject<API>(aAccessData.ConnectionDescription);

            Task task = GetAccessToken();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
     

        public async Task<Guid> TransferCreate(FileTransferCreateDto<string> aRequest)
        {
            var request = new FileTransferCreateDto<string>() { FileID = aRequest.FileID };

            var json = JsonConvert.SerializeObject(request);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", this.AccessToken);

            var response = await _client.PostAsync(_api.url, data);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot create a transfer. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");

                throw ex;
            }

            string resultContent = await response.Content?.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SingleResponse<Guid>>(resultContent).Model;
        }
        
        public async Task<FileTransferDto> TransferGet(Guid aID)
        {
            var response = await _client.GetAsync($"{_api.url}/{aID}");

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot get a transfer. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");
                throw ex;
            }

            string resultContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SingleResponse<FileTransferDto>>(resultContent).Model;
        }

        public async Task<bool> TransferRemove(Guid aID)
        {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", this.AccessToken);

            var response = await _client.DeleteAsync($"{_api.url}/{aID}");

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot delete a transfer. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");
                throw ex;
            }
            string resultContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Response>(resultContent).HasError;
        }


        private object _lockAccessToken = new object();
        string _accessToken;
        private string AccessToken
        {
            get { lock (_lockAccessToken) { return _accessToken; } }
            set { lock (_lockAccessToken) { _accessToken = value; } }
        }

        private async Task GetAccessToken()
        {
            AccessTokenRequest request = new AccessTokenRequest()
            {
                client_id = _api.client_id,
                client_secret = _api.client_secret,
                audience = _audience,
                grant_type = "client_credentials"
            };

            var requestJson = JsonConvert.SerializeObject(request);
            var client = new HttpClient();
            var data = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://{_api.domanin}/oauth/token", data);


            if (response.IsSuccessStatusCode)
            {
                string ret  = await response.Content.ReadAsStringAsync();
                AccessTokenResponse responseToken = JsonConvert.DeserializeObject<AccessTokenResponse>(ret);
                this.AccessToken = responseToken.access_token;
                this.IsConnected = true;
            }         
        }
     
        struct AccessTokenRequest
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string audience { get; set; }
            public string grant_type { get; set; }
        }

        class AccessTokenResponse
        { 
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }

        class API
        {
            public string url { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string domanin { get; set; }
        }
    }
}
