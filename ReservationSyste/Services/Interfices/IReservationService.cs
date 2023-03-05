using ReservationSyste.Controllers;
using ReservationSyste.Data;
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
        Task<int> CreatePersonalProfileAsync(PersonalProfile personalProfile, ApplicationDbContext context);
        Task<int> CreatePersonalProfileRoomAsync(PersonalProfileRoom personalProfileRoom, ApplicationDbContext context);
        Task<int> UpdateReservationStatusAsync(int roomId,  int status, ApplicationDbContext context);
        Task<PersonalProfile> GetUserName(string email);
         bool VerifyEmail(string email);

    }
}
