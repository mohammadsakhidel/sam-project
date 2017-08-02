using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Objects.Exceptions
{
    public class HttpException : Exception
    {
        #region Fields:
        HttpResponseMessage _response = null;
        #endregion

        #region Ctors:
        public HttpException(HttpResponseMessage response)
        {
            _response = response;
        }
        #endregion

        #region Props:
        public HttpResponseMessage Response
        {
            get
            {
                return _response;
            }
        }
        #endregion
    }
}
