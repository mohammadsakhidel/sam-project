using SmsLib.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Practices.Unity;

namespace SamAPI.Code.Utils
{
    public class SmsUtil
    {
        public static void Send(string message, string number)
        {
            Task.Run(() =>
            {
                try
                {
                    ISmsManager _smsManager = UnityConfig.Container.Resolve<ISmsManager>();
                    _smsManager.Send(message, new string[] { number });
                }
                catch { }
            });
        }
    }
}