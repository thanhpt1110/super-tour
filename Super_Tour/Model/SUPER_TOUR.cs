using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    internal class SUPER_TOUR:DbContext
    {
       // string conString = ConfigurationManager.ConnectionStrings["SuperTour"].ConnectionString;
        public SUPER_TOUR() : base(ConfigurationManager.ConnectionStrings["SuperTour"].ConnectionString)
        {
        }
        public DbSet<ACCOUNT> ACCOUNTs {get; set;}
        public DbSet<BOOKING> BOOKINGs { get; set; }
        public DbSet<BOOKING_DETAILS> BOOKING_DETAILSs { get; set; }
        public DbSet<CUSTOMER> CUSTOMERs { get; set; }
        public DbSet<PACKAGE> PACKAGEs { get; set; }
        public DbSet<TICKET> TICKETs { get; set; }
        public DbSet<TOUR> TOURs { get; set; }
        public DbSet<TOUR_DETAILS> TOUR_DETAILs { get; set; }
        public DbSet<TRAVEL> TRAVELs { get; set; }
        public DbSet<TYPE_PACKAGE> TYPE_PACKAGEs { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACCOUNT>().ToTable("account");
            modelBuilder.Entity<BOOKING>().ToTable("booking");
            modelBuilder.Entity<BOOKING_DETAILS>().ToTable("booking_details");
            modelBuilder.Entity<CUSTOMER>().ToTable("customer");
            modelBuilder.Entity<PACKAGE>().ToTable("package");
            modelBuilder.Entity<TICKET>().ToTable("ticket");
            modelBuilder.Entity<TOUR>().ToTable("tour");
            modelBuilder.Entity<TOUR_DETAILS>().ToTable("tour_details");
            modelBuilder.Entity<TRAVEL>().ToTable("travel");
            modelBuilder.Entity<TYPE_PACKAGE>().ToTable("type_package");
            base.OnModelCreating(modelBuilder);
        }
    }
}
