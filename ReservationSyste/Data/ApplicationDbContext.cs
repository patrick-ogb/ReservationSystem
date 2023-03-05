using Microsoft.EntityFrameworkCore;
using ReservationSyste.Controllers;
using ReservationSyste.Models;

namespace ReservationSyste.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
            public DbSet<Reservation>   Reservations { get; set; }
            public DbSet<CheckInOut> CheckInOuts { get; set; }
            public DbSet<PersonalProfile> PersonalProfiles { get; set; }
            public DbSet<PersonalProfileRoom> PersonalProfileRooms { get; set; }
            public DbSet<TransactionModel> Transactions { get; set; }
    }
}
