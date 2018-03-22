using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
            Handle(ex, logger, "");
        }

        public static void Handle(Exception ex, EventLog logger, string source)
        {
            var message = $"{source}: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}";

            if (ex is DbEntityValidationException vex)
            {
                foreach (var entityValidationErrors in vex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        message += Environment.NewLine + $"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}";
                    }
                }
            }

            logger.WriteEntry(message);
        }
    }
}
