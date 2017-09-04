using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsLib.Objects
{
    public interface ISmsManager
    {
        Task<bool> SendAsync(string message, IEnumerable<string> numbers);
    }
}
