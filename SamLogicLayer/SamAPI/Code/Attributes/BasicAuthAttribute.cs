using SamUtils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Filters;

namespace SamAPI.Code.Attributes
{
    public class BasicAuthAttribute : ActionFilterAttribute
    {
        #region CONST FIELDS:
        private const string BASIC_AUTH_SCHEME = "Basic";
        #endregion

        #region OVERRIDE:
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            #region ANONYMOUS ACCESS AVAILABILITY CHECK:
            bool hasAnonymousAttribute = actionContext.ActionDescriptor
                .GetCustomAttributes<AllowAnonymousAttribute>().Any();
            #endregion

            if (!hasAnonymousAttribute)
            {
                #region CHECK AUTHORIZATION HEADER:
                var validSchemes = new List<string>() { BASIC_AUTH_SCHEME };
                HttpRequestMessage request = actionContext.Request;
                AuthenticationHeaderValue authorization = request.Headers.Authorization;
                var isAuthHeaderInvalid = (authorization == null) ||
                    (!validSchemes.Contains(authorization.Scheme)) ||
                    (String.IsNullOrEmpty(authorization.Parameter));
                if (isAuthHeaderInvalid)
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                #endregion
                #region BASIC AUTHORIZATION:
                try
                {
                    Tuple<string, string> userpass = ExtractUserNameAndPassword(authorization.Parameter);
                    if (!Collections.ApiClients.Where(c => c.Key == userpass.Item1 && c.Value == userpass.Item2).Any())
                        throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
                catch
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
                #endregion
            }
        }
        #endregion

        #region Private Methods:
        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            var credBytes = Convert.FromBase64String(authorizationParameter);
            Encoding encoding = Encoding.ASCII;
            encoding = (Encoding)encoding.Clone();
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            var decodedCredentials = encoding.GetString(credBytes);
            return new Tuple<string, string>(decodedCredentials.Split(new char[] { ':' })[0], decodedCredentials.Split(new char[] { ':' })[1]);
        }
        #endregion
    }
}