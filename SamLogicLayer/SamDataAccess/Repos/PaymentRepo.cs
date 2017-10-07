using RamancoLibrary.Utilities;
using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos
{
    public class PaymentRepo : Repo<SamDbContext, Payment>, IPaymentRepo
    {
        public void UpdateWithSave(Payment newPayment)
        {
            var payment = Get(newPayment.ID);
            if (payment != null)
            {
                payment.Status = newPayment.Status;
                payment.ReferenceCode = newPayment.ReferenceCode;
                payment.LastUpdateTime = DateTimeUtils.Now;
                Save();
            }
        }
    }
}
