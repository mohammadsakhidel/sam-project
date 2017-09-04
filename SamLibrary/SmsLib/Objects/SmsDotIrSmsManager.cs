using SmsLib.Models;
using SmsLib.Models.SmsDotIrModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmsLib.Objects
{
    public class SmsDotIrSmsManager : ISmsManager
    {
        #region Configs:
        internal const int TOKEN_LIFETIME_SECONDS = 30;
        internal const string SECRET_KEY = "eaa2aa019a13b5ab1aa6ae02";
        internal const string API_KEY = "vRL3ouOwMeXHEXrYKH1VZnXXXWLYJbxv";
        internal const string API_BASE_ADDRESS = "http://restfulsms.com/api/";
        internal const string SECURITY_TOKEN_HEADER = "x-sms-ir-secure-token";
        internal const string LINE = "50002015117700";
        #endregion

        #region API ENDPOINTS:
        public const string ENDPOINT_TOKEN = "Token";
        public const string ENDPOINT_SEND = "MessageSend";
        #endregion

        #region Fields:
        private static string token;
        private static DateTime? lastTokenTime = null;
        #endregion

        #region Private Methods:
        private async Task UpdateToken()
        {
            if (!string.IsNullOrEmpty(token) &&
                lastTokenTime > DateTime.Now.AddSeconds(-30))
                return;

            #region Call Service:
            using (var hc = CreateClient())
            {
                var model = new TokenRequestModel
                {
                    UserApiKey = API_KEY,
                    SecretKey = SECRET_KEY
                };
                var response = await hc.PostAsJsonAsync(ENDPOINT_TOKEN, model);
                response.EnsureSuccessStatusCode();

                var responseModel = await response.Content.ReadAsAsync<TokenResponseModel>();
                token = responseModel.TokenKey;
            }
            #endregion
        }
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(API_BASE_ADDRESS);
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add(SECURITY_TOKEN_HEADER, token);
            return client;
        }
        #endregion

        #region Public Methods:
        public async Task<bool> SendAsync(string message, IEnumerable<string> numbers)
        {
            await UpdateToken();

            using (var hc = CreateClient())
            {
                var model = new SendModel {
                    LineNumber = LINE,
                    CanContinueInCaseOfError = false,
                    Messages = new string[] { message },
                    MobileNumbers = numbers.ToArray(),
                    SendDateTime = ""
                };
                var response = await hc.PostAsJsonAsync<SendModel>(ENDPOINT_SEND, model);
                response.EnsureSuccessStatusCode();
                var responseModel = await response.Content.ReadAsAsync<ApiResponseBase>();
                return responseModel.IsSuccessful;
            }
        }
        #endregion
    }
}
