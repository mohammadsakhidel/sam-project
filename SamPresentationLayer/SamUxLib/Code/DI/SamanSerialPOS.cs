using SAMAN_PcToPos;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamUxLib.Code.DI
{
    public class SamanSerialPOS : IPOS
    {
        #region Fields:
        string _portName;
        #endregion

        #region Ctors:
        public SamanSerialPOS(string portName)
        {
            _portName = portName;
        }
        #endregion

        #region IPOS Implementation:
        public event EventHandler<PosResponseEventArgs> PosResponse;

        public void PayRequest(int amount, bool print, bool verifyLater)
        {
            var terminal = new TTerminal(_portName);

            var data = new Dictionary<string, object>();
            data["Amount1"] = amount;
            data["TotalFee"] = amount;

            var xml = ToXml(data);
            terminal.SendToCOM(xml);

            terminal.XMLReceived += (sender, e) => {
                PosResponse?.Invoke(sender, new PosResponseEventArgs {
                    Succeeded = e.IsSuccessful,
                    Data = e.XmlRecieve,
                    DataFormat = DataFormat.xml
                });
            };
        }

        public void VerifyPayment(string id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods:
        string ToXml(Dictionary<string, object> dic)
        {
            var xml = "";
            foreach (var item in dic)
            {
                xml += $"<{item.Key}>{item.Value}</{item.Key}>";
            }
            return xml;
        }
        #endregion
    }
}