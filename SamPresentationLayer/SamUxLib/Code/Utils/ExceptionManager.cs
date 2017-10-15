using Newtonsoft.Json;
using SamUtils.Objects.Exceptions;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SamUxLib.Code.Utils
{
    public class ExceptionManager
    {
        public static void Handle(Exception ex)
        {
            var message = "";

            if (ex is HttpRequestException || ex is HttpException)
            {
                message = Messages.ApiErrorMessage;
                //var httpex = (HttpException)ex;
                //if (httpex.Response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                //{
                //    var jsonString = await httpex.Response.Content.ReadAsStringAsync();
                //    dynamic serverException = JsonConvert.DeserializeObject<dynamic>(jsonString);
                //    message = serverException != null && serverException.ExceptionMessage != null ? serverException.ExceptionMessage : "Error Occurred!";
                //}
                //else
                //{
                //    message = ex.Message;
                //}
            }
            else
            {
                message = ex.Message; //Messages.ErrorOccurredTryAgain;
            }

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                UxUtil.ShowError($"{ex.GetType().FullName}:{Environment.NewLine}{message}");
            });
        }

        public static void Log(Exception ex)
        {
            // log exception here
        }

        public static string ConverterException<T>(Exception ex)
        {
            if (typeof(T) == typeof(string))
                return ex.Message;
            else
                return null;
        }
    }
}
