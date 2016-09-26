using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.SqlClient;
using CarFinderAPI.Areas;
using System;

namespace CarFinderAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("MyDBConnection", throwIfV1Schema: false)
        {
        }
        public async Task<List<string>> GetYears()
        {
            return await this.Database.SqlQuery<string>("GetYears_mc").ToListAsync();
        }
        public async Task<List<string>> GetMakes(int year)
        {
            return await this.Database.SqlQuery<string>("GetMakes_mc @year",
                new SqlParameter("year", year)).ToListAsync();
        }

        public async Task<List<string>> GetModels(int year, string make)
        {
            return await this.Database.SqlQuery<string>("GetModel_mc @year, @make",
                new SqlParameter("year", year),
                new SqlParameter("make", make)).ToListAsync();
        }
        public async Task<List<string>> GetTrims(int year, string make, string model)
        {
            return await this.Database.SqlQuery<string>("GetTrim_mc @year, @make, @model",
                new SqlParameter("year", year),
                new SqlParameter("make", make),
                new SqlParameter("model", model)).ToListAsync();
        }
        public async Task<List<Car>> GetCars(int year, string make, string model, string trim)
        {
            return await this.Database.SqlQuery<Car>("GetCarsSingle @year, @make, @model, @trim",
                new SqlParameter("year", year),
                new SqlParameter("make", make ?? ""),
                new SqlParameter("model", model ?? ""),
                new SqlParameter("trim", trim ?? "")).ToListAsync();
        }

        public async Task<Car> GetCar(int year, string make, string model, string trim)
        {
            return await this.Database.SqlQuery<Car>("GetCarsSingle @year, @make, @model, @trim",
                new SqlParameter("year", (object) year ?? DBNull.Value),
                new SqlParameter("make", (object) make ?? DBNull.Value),
                new SqlParameter("model", (object) model ?? DBNull.Value),
                new SqlParameter("trim", (object) trim ?? DBNull.Value)).FirstOrDefaultAsync();
        }
        public async Task<Car> GetCarById(int id)
        {
            return await this.Database.SqlQuery<Car>("GetCarById @id",
                new SqlParameter("id", id)).FirstOrDefaultAsync();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}