using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilag_Medic.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder); 
        //    //builder.Entity<IdentityUser>().ToTable("User"); //change default entity name to User
        //    #region "Seed Data"
        //    builder.Entity<IdentityRole>()(
        //        //new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
        //        //new { Id = "2", Name = "User", NormalizedName = "USER" }
        //        );
        //    #endregion
        //    builder.Entity<IdentityUser>(entity =>
        //    {
        //        //trying to add new tables to the database
        //        //entity.Property(e => e.Staff_id)
        //    });

        //}
        //public DbSet<Jwt_TokenTest.Data.Model.TestData> TestData { get; set; }
    }
}
