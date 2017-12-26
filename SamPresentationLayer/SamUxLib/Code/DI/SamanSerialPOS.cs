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
        TTerminal _terminal = null;
        string _portName;
        bool _opened = false;
        #endregion

        #region Ctors:
        public SamanSerialPOS(string portName)
        {
            _portName = portName;
            _terminal = new TTerminal(_portName);
            _terminal.XMLReceived += _terminal_XMLReceived;
        }
        #endregion

        #region IPOS Implementation:
        public event EventHandler<PosResponseEventArgs> PosResponse;

        public void Open()
        {
            if (!_opened)
                _terminal.OpenPort();

            _opened = true;
        }

        public void Close()
        {
            if (_opened)
                _terminal.ClosePort();

            _opened = false;
        }

        public void PayRequest(int amount, bool print, bool verifyLater)
        {
            if (!_opened)
                Open();

            _terminal.ShowMessages = false;
            _terminal.ConfirmFlag = verifyLater;
            _terminal.PrintFlag = print ? (byte)1 : (byte)0;

            var data = new Dictionary<string, object>();
            data["Amount1"] = amount;
            data["TotalFee"] = amount;
            data["PrgVer"] = "v2.0.50727";
            var xml = ToXml(data);

            var res = _terminal.SendToCOM(xml);

        }

        public void VerifyPayment(params string[] args)
        {
            _terminal.SetConfirmFlag(true);
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

        #region Event Handlers:
        private void _terminal_XMLReceived(object sender, XmlReceivedEventArgs e)
        {
            var args = new PosResponseEventArgs
            {
                Succeeded = e.IsSuccessful,
                Data = e.XmlRecieve,
                DataFormat = DataFormat.xml
            };
            PosResponse?.Invoke(sender, args);
        }
        #endregion
    }
}