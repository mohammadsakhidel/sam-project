using SamDataAccess.Contexts;
using SamDataAccess.Repos.BaseClasses;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SamDataAccess.Repos
{
    public class ConsolationRepo : Repo<SamDbContext, Consolation>, IConsolationRepo
    {
        public void Create(Consolation consolation)
        {
            using (var ts = new TransactionScope())
            {
                var newCustomer = new Customer {
                    FullName = consolation.Customer.FullName,
                    CellPhoneNumber = consolation.Customer.CellPhoneNumber,
                    IsMember = false,
                };
                //context.Customers.Add(newCustomer);
                //context.SaveChanges();

                consolation.Customer = newCustomer;
                context.Consolations.Add(consolation);
                context.SaveChanges();

                ts.Complete();
            }
        }
    }
}
