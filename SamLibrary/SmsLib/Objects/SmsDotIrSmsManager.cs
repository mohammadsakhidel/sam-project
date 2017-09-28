using SmsIrRestful;
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
        internal const string LINE = "50002015117700";
        #endregion

        #region Fields:
        private static string _token;
        private static DateTime? _lastTokenTime = null;
        #endregion

        #region Private Methods:
        private void UpdateToken()
        {
            if (!string.IsNullOrEmpty(_token) &&
                _lastTokenTime > DateTime.Now.AddSeconds(-TOKEN_LIFETIME_SECONDS))
                return;

            Token tk = new SmsIrRestful.Token();
            _token = tk.GetToken(API_KEY, SECRET_KEY);
        }
        #endregion

        #region Public Methods:
        public bool Send(string message, IEnumerable<string> numbers)
        {
            UpdateToken();

            var model = new MessageSendObject()
            {
                LineNumber = LINE,
                CanContinueInCaseOfError = false,
                Messages = new string[] { message },
                MobileNumbers = numbers.ToArray(),
                SendDateTime = null
            };
            MessageSendResponseObject response = new MessageSend().Send(_token, model);
            return response.IsSuccessful;
        }
        #endregion
    }
}
