using SamDataAccess.Contexts;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IPaymentRepo : IRepo<SamDbContext, Payment>
    {
        void UpdateWithSave(Payment newPayment);
    }
}
