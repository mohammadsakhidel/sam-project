using SamUtils.Enums;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamWeb.Code.Utils
{
    public class EnumUtil
    {
        public static string PaymentStatusToString(PaymentStatus paymentStatus)
        {
            switch (paymentStatus)
            {
                case PaymentStatus.pending:
                    return Strings.PaymentStatus_Pending;
                case PaymentStatus.failed:
                    return Strings.PaymentStatus_Failed;
                case PaymentStatus.succeeded:
                    return Strings.PaymentStatus_Succeeded;
                case PaymentStatus.verified:
                    return Strings.PaymentStatus_Verified;
                default:
                    return "";
            }
        }

        public static string ConsolationStatusToString(ConsolationStatus status)
        {
            switch (status)
            {
                case ConsolationStatus.pending:
                    return Strings.ConsolationStatus_Pending;
                case ConsolationStatus.confirmed:
                    return Strings.ConsolationStatus_Confirmed;
                case ConsolationStatus.canceled:
                    return Strings.ConsolationStatus_Canceled;
                case ConsolationStatus.displayed:
                    return Strings.ConsolationStatus_Displayed;
                default:
                    return "";
            }
        }
    }
}