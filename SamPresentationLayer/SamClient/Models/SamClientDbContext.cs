using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClient.Models
{
    public class SamClientDbContext : DbContext
    {
        #region Ctors:
        public SamClientDbContext() : base("name=SamClientConnection")
        {

        }
        #endregion

        #region Props:
        public DbSet<Mosque> Mosques { get; set; }
        public DbSet<ClientSetting> ClientSettings { get; set; }
        public DbSet<Consolation> Consolations { get; set; }
        public DbSet<Obit> Obits { get; set; }
        public DbSet<ObitHolding> ObitHoldings { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateField> TemplateFields { get; set; }
        public DbSet<TemplateCategory> TemplateCategories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        #endregion

        #region Fluent API:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClientSetting>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Mosque>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Consolation>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Obit>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ObitHolding>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Template>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<TemplateField>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<TemplateCategory>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Customer>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Province>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<City>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
        #endregion
    }
}
