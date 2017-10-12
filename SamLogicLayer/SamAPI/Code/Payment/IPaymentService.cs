using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamAPI.Code.Payment
{
    public interface IPaymentService
    {
        #region Props:
        string UniqueID { get; set; }
        string BankPageUrl { get; }
        string CallbackUrl { get; }
        string ProviderName { get; }
        #endregion

        #region Methods:
        string GetToken(int amount);
        bool Verify(string paymentId, string referenceCode);
        bool Reverse(string referenceCode);
        #endregion
    }
}