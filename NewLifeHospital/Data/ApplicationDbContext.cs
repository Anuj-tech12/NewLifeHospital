using Microsoft.EntityFrameworkCore;
using NewLifeHospital.Models;

namespace NewLifeHospital.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }


    public DbSet<Appointment> Appointments { get; set; }
    }


}
