using SamClientDataAccess.ClientModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Contexts
{
    public class SamClientDbContext : DbContext
    {
        #region Ctors:
        public SamClientDbContext() : base("name=SamClientConnection")
        {

        }
        #endregion

        #region DbSets:
        public DbSet<ClientSetting> ClientSettings { get; set; }
        public DbSet<LocalConsolation> Consolations { get; set; }
        public DbSet<LocalBanner> Banners { get; set; }
        public DbSet<LocalDisplay> Displays { get; set; }
        public DbSet<LocalObit> Obits { get; set; }
        public DbSet<LocalObitHolding> ObitHoldings { get; set; }
        public DbSet<DownloadImageTask> DownloadImageTasks { get; set; }
        #endregion

        #region Fluent API:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientSetting>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LocalConsolation>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LocalBanner>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LocalObit>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<LocalObitHolding>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}