using Microsoft.AspNet.Identity.EntityFramework;
using SamDataAccess.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SamModels.Entities;
using SamUtils.Enums;
using SamModels.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SamDataAccess.Contexts
{
    public class SamDbContext : IdentityDbContext<AspNetUser>
    {
        #region Ctors:
        public SamDbContext() : base("name=SamDbConnection")
        {
        }

        public SamDbContext(string connectionString) : base(connectionString)
        {

        }
        #endregion

        #region DbSets:
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Mosque> Mosques { get; set; }
        public DbSet<Saloon> Saloons { get; set; }
        public DbSet<Obit> Obits { get; set; }
        public DbSet<Consolation> Consolations { get; set; }
        public DbSet<Display> Displays { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateField> TemplateFields { get; set; }
        public DbSet<TemplateCategory> TemplateCategories { get; set; }
        public DbSet<ObitHolding> ObitHoldings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<RemovedEntity> RemovedEntities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SystemParameter> SystemParameters { get; set; }
        #endregion

        #region Fluent API:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region More Configurations:
            modelBuilder.Entity<Province>().Property(p => p.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<City>().Property(c => c.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<SystemParameter>().Property(p => p.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
