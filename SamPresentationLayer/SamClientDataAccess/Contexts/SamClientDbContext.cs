using ClientModels.Models;
using SamClientDataAccess.ClientModels;
using SamModels.Entities;
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
        public DbSet<Mosque> Mosques { get; set; }
        public DbSet<Saloon> Saloons { get; set; }
        public DbSet<ClientSetting> ClientSettings { get; set; }
        public DbSet<Consolation> Consolations { get; set; }
        public DbSet<ConsolationImage> ConsolationImages { get; set; }
        public DbSet<Display> Displays { get; set; }
        public DbSet<Obit> Obits { get; set; }
        public DbSet<ObitHolding> ObitHoldings { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateCategory> TemplateCategories { get; set; }
        public DbSet<TemplateField> TemplateFields { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<DownloadImageTask> DownloadImageTasks { get; set; }
        #endregion

        #region Fluent API:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientSetting>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Mosque>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Saloon>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Consolation>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Obit>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ObitHolding>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Template>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<TemplateCategory>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<TemplateField>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Customer>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Blob>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ConsolationImage>().Property(e => e.ConsolationID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
