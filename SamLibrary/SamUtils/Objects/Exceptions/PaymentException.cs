using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Objects.Exceptions
{
    public class PaymentException : Exception
    {
        public PaymentException()
        {

        }

        public PaymentException(string message) : base(message)
        {

        }
    }
}
