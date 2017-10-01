using RamancoLibrary.Utilities;
using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class ConsolationImageRepo : Repo<SamClientDbContext, ConsolationImage>
    {
        #region Extension:
        public void AddOrUpdate(ConsolationImage cImage)
        {
            var exists = set.Where(i => i.ConsolationID == cImage.ConsolationID).Any();
            if (!exists)
            {
                Add(cImage);
            }
            else
            {
                Update(cImage);
            }
        }
        public void Update(ConsolationImage newImage)
        {
            var consolationImage = set.SingleOrDefault(i => i.ConsolationID == newImage.ConsolationID);
            if (consolationImage != null)
            {
                consolationImage.Bytes = newImage.Bytes;
                consolationImage.LastUpdateTime = DateTimeUtils.Now;
            }
        }
        #endregion
    }
}
