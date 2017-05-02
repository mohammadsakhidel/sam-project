using Microsoft.AspNet.Identity.EntityFramework;
using SamDataAccess.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SamModels.Entities.Core;
using SamUtils.Enums;
using SamModels.Entities.Blobs;

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

        #region Core DbSets:
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Mosque> Mosques { get; set; }
        public DbSet<Obit> Obits { get; set; }
        public DbSet<Consolation> Consolations { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateExtraField> TemplateExtraFields { get; set; }
        public DbSet<TemplateCategory> TemplateCategories { get; set; }
        public DbSet<ObitHolding> ObitHoldings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeceasedPerson> DeceasedPersons { get; set; }
        #endregion

        #region Blobs DbSets:
        public DbSet<Blob> Blobs { get; set; }
        #endregion

        #region Fluent API:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Core Entities:
            modelBuilder.Entity<Province>().ToTable("Provinces", DbSchemaName.core.ToString());
            modelBuilder.Entity<City>().ToTable("Cities", DbSchemaName.core.ToString());
            modelBuilder.Entity<Mosque>().ToTable("Mosques", DbSchemaName.core.ToString());
            modelBuilder.Entity<Obit>().ToTable("Obits", DbSchemaName.core.ToString());
            modelBuilder.Entity<Consolation>().ToTable("Consolations", DbSchemaName.core.ToString());
            modelBuilder.Entity<Template>().ToTable("Templates", DbSchemaName.core.ToString());
            modelBuilder.Entity<TemplateExtraField>().ToTable("TemplateExtraFields", DbSchemaName.core.ToString());
            modelBuilder.Entity<TemplateCategory>().ToTable("TemplateCategories", DbSchemaName.core.ToString());
            modelBuilder.Entity<ObitHolding>().ToTable("ObitHoldings", DbSchemaName.core.ToString());
            modelBuilder.Entity<Customer>().ToTable("Customers", DbSchemaName.core.ToString());
            modelBuilder.Entity<DeceasedPerson>().ToTable("DeceasedPersons", DbSchemaName.core.ToString());
            #endregion

            #region Blob Entities:
            modelBuilder.Entity<Blob>().ToTable("Blobs", DbSchemaName.blob.ToString());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
