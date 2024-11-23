using Microsoft.EntityFrameworkCore;
using SupportSystem.Shared;
using System.Collections.Generic;

namespace SupportSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
    }
}
