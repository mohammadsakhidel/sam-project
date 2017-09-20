using RamancoLibrary.Utilities;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class ObitRepo : Repo<SamClientDbContext, Obit>
    {
        #region Ctors:
        public ObitRepo() : base()
        {

        }
        public ObitRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extensions:
        public void AddOrUpdate(Obit obit)
        {
            var exists = set.Where(o => o.ID == obit.ID).Any();
            if (!exists)
            {
                Add(obit);
            }
            else
            {
                Update(obit);
            }
        }
        public void Update(Obit newObit)
        {
            var obit = Get(newObit.ID);
            if (obit != null)
            {
                obit.Title = newObit.Title;
                obit.ObitType = newObit.ObitType;
                obit.DeceasedIdentifier = newObit.DeceasedIdentifier;
                obit.LastUpdateTime = newObit.LastUpdateTime;

                //update holdings:
                if (obit.ObitHoldings != null)
                    context.ObitHoldings.RemoveRange(obit.ObitHoldings);

                obit.ObitHoldings = newObit.ObitHoldings;
            }
        }
        #endregion
    }
}
