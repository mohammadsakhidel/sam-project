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
    public class CustomerRepo : Repo<SamDbContext, Customer>, ICustomerRepo
    {
        public Customer Find(string cellPhoneNumber)
        {
            var cellphone = cellPhoneNumber.Substring(cellPhoneNumber.Length - 10, 10);
            return set
                .Where(c => c.CellPhoneNumber.Contains(cellphone))
                .FirstOrDefault();
        }
    }
}
