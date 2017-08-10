using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSyncAgent.Code.Utils
{
    public class ExceptionManager
    {
        public static void Handle(Exception ex, EventLog logger)
        {
            logger.WriteEntry(ex.Message);
        }

        public static void Handle(Exception ex, EventLog logger, string source)
        {
            logger.WriteEntry($"{source}: {ex.Message}");
        }
    }
}
