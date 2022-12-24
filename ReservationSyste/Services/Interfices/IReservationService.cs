using ReservationSyste.Controllers;
using ReservationSyste.Models;

namespace ReservationSyste.Services.Interfices
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationAsync();
        Task<int> CreateReservationAsync(Reservation reservation);
        Task<Reservation> UpdateReservationAsync(Reservation reservation);
        Task<Reservation> FindReservationAsync(int Id);
        Task<CheckInOut > GetCheckInOutAsync();
        Task<List<Reservation>> SearchReservationAsync(string searchTerm);
    }
}
