using SamClientDataAccess.Contexts;
using SamClientDataAccess.Repos.BaseClasses;
using SamModels.Entities.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos
{
    public class BlobRepo : Repo<SamClientDbContext, Blob>
    {
        #region Ctors:
        public BlobRepo() : base()
        {

        }
        public BlobRepo(SamClientDbContext context) : base(context)
        {

        }
        #endregion

        #region Extension:
        public void AddOrUpdateImage(ImageBlob imageBlob)
        {
            var exists = set.Where(b => b.ID == imageBlob.ID).Any();
            if (!exists)
            {
                Add(imageBlob);
            }
            else
            {
                Update(imageBlob);
            }
        }

        public void Update(ImageBlob newImageBlob)
        {
            var blob = set.OfType<ImageBlob>().SingleOrDefault(b => b.ID == newImageBlob.ID);
            if (blob != null)
            {
                blob.Bytes = newImageBlob.Bytes;
                blob.ThumbImageBytes = newImageBlob.ThumbImageBytes;
                blob.ImageFormat = newImageBlob.ImageFormat;
                blob.ImageWidth = newImageBlob.ImageWidth;
                blob.ImageHeight = newImageBlob.ImageHeight;
            }
        }
        #endregion
    }
}
