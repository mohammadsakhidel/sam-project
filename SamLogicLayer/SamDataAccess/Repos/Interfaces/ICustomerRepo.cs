using SamDataAccess.Contexts;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface ICustomerRepo : IRepo<SamDbContext, Customer>
    {
        Customer Find(string cellPhoneNumber);
    }
}
