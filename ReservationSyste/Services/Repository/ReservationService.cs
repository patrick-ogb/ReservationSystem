using Microsoft.EntityFrameworkCore;
using ReservationSyste.Controllers;
using ReservationSyste.Data;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;

namespace ReservationSyste.Services.Repository
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckInOut> GetCheckInOutAsync()
        {
            return  await _context.CheckInOuts.FirstOrDefaultAsync();
        }

        public async Task<int> CreateReservationAsync(Reservation reservation)
        {
                 await _context.Reservations.AddAsync(reservation);
            return _context.SaveChanges();
        }

        public async Task<List<Reservation>> GetAllReservationAsync()
        {
            return await _context.Reservations.ToListAsync();
        }


        public async  Task<List<Reservation>> SearchReservationAsync(string searchTerm)
        {
            var reservatn = await (from reservation in _context.Reservations
                                where reservation.Name.Contains(searchTerm.Trim())
                                || reservation.RoomText.Contains(searchTerm.Trim())
                                || reservation.Content.Contains(searchTerm.Trim())
                                select reservation).ToListAsync();

            return reservatn;
        }


        public async Task<Reservation> UpdateReservationAsync(Reservation model)
        {
            var reservation = _context.Reservations.Attach(model);
            reservation.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<Reservation> FindReservationAsync(int Id)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == Id);
        }
    }
}
