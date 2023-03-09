using Microsoft.EntityFrameworkCore;
using ReservationSyste.Controllers;
using ReservationSyste.Data;
using ReservationSyste.Enums;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ReservationSyste.Services.Repository
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ReservationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<CheckInOut> GetCheckInOutAsync()
        {
            return  await _context.CheckInOuts.FirstOrDefaultAsync();
        }
        public async Task<int> CreateReservationAsync(Reservation reservation)
        {
                 await _context.Reservations.AddAsync(reservation);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<Reservation>> GetAllReservationAsync()
        {
            return await (from rFind in _context.Reservations 
                         where rFind.ReservationStatus.Equals(1) || rFind.ReservationStatus.Equals(2)
                         select rFind).ToListAsync();
        }

        //public async Task<List<Reservation>> GetAllReservationAsync()
        //{
        //    List<Reservation> reservations = new List<Reservation>();
        //    var reservation = _context.Reservations;
        //    if (reservation.Count() > 0)
        //    {
        //        reservations = await (from rFind in reservation
        //                              where rFind.ReservationStatus.Equals(1) || rFind.ReservationStatus.Equals(2)
        //                              select rFind).ToListAsync();
        //    }
        //    return reservations;
        //}

        public async  Task<List<Reservation>> SearchReservationAsync(string searchTerm)
        {
            var reservatn = (await (from reservation in _context.Reservations
                                    where reservation.Name.Contains(searchTerm.Trim())
                                    || reservation.RoomText.Contains(searchTerm.Trim())
                                    || reservation.Content.Contains(searchTerm.Trim())
                                    select reservation).ToListAsync())
                 .Where(x => x.ReservationStatus != (int)ReservationStatusEnum.Requested
            && x.ReservationStatus != (int)ReservationStatusEnum.Booked).ToList();

            return reservatn;
        }
        public async Task<Reservation> UpdateReservationAsync(Reservation model)
        {
            var reservation = _context.Reservations.Attach(model);
            reservation.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return model;
        }
        public async Task<Reservation> FindReservationAsync(int Id) => await _context.Reservations.FirstOrDefaultAsync(r => r.Id == Id);
        public async Task<int> CreatePersonalProfileAsync(PersonalProfile personalProfile , ApplicationDbContext contextDb)
        {
            await contextDb.PersonalProfiles.AddAsync(personalProfile);
            await contextDb.SaveChangesAsync();
            return personalProfile.Id; 
        }
        public async Task<int> CreatePersonalProfileRoomAsync(PersonalProfileRoom personalProfileRoom, ApplicationDbContext contextDb)
        {
            await contextDb.AddAsync(personalProfileRoom);
            await contextDb.SaveChangesAsync();
            return personalProfileRoom.Id;
        }
        public async Task<int> UpdateReservationStatusAsync(int roomId, int status, ApplicationDbContext contextDb)
        {
           var entity = await _context.Reservations.FirstOrDefaultAsync(x => x.Id== roomId);
            if (entity != null)
            {
                entity.ReservationStatus= status;
                contextDb.Reservations.Update(entity);
                await contextDb.SaveChangesAsync();
                return entity.Id;
            }
            return 0;
        }
        public bool VerifyEmail(string email)
        {
            var result = from pfFind in _context.PersonalProfiles
                         where pfFind.Email == email
                         select true;
            return result.Any();
        }
        public async Task<string> GetRoomId(string reference)
        {
            var result = await (from trans in _context.Transactions
                         where trans.TrxRef == reference
                         select trans.RoomId).FirstOrDefaultAsync();
            return result;
        }
        public async Task<PersonalProfile> GetUserName(string email)
        {
            var result = from persn in _context.PersonalProfiles where (persn.Email == email) select persn;

            var newResult = await _context.PersonalProfiles.FirstOrDefaultAsync(x => x.Email.Equals(email));
            return newResult;
        }
        private void Test(string email)
        {
            var rpFindNew = from pf in _context.PersonalProfiles
                            where pf.Email.Equals(email)
                            join rf in _context.Reservations
                           on pf.Id equals rf.Id
                            select new PersonalProfileRoom{};
        }


    }

    public class AuthResponse
    {

    }


}
