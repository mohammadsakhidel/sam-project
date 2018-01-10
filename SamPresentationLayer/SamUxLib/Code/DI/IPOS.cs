using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUxLib.Code.DI
{
    public interface IPOS
    {
        #region Methods:
        void Open();
        void Close();
        bool PayRequest(int amount, bool print, bool verifyLater);
        void VerifyPayment(params string[] args);
        #endregion

        #region Events:
        event EventHandler<PosResponseEventArgs> PosResponse;
        #endregion
    }

    public class PosResponseEventArgs : EventArgs
    {
        public bool Succeeded { get; set; }
        public string Data { get; set; }
        public DataFormat DataFormat { get; set; }
    }

    public enum DataFormat
    {
        xml = 1,
        json = 2,
        comma_separated = 3
    }
}
