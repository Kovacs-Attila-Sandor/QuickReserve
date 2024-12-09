using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Threading.Tasks;

namespace QuickReserve.Services
{
    public class ReservationService
    {
        // Foglalás hozzáadása a Firebase adatbázishoz
        public async Task<bool> AddReservation(Reservation reservation)
        {
            try
            {
                // Az új foglalás ID-jának automatikus generálása
                reservation.ReservationId = Guid.NewGuid().ToString();

                // Foglalás adatainak mentése a "Reservation" gyűjteménybe a ReservationId alapján
                await FirebaseService
                    .Client
                    .Child("Reservation")  // "Reservation" gyűjtemény
                    .Child(reservation.ReservationId.ToString())  // Az ID stringgé konvertálva
                    .PutAsync(JsonConvert.SerializeObject(reservation));  // Az adatokat JSON formátumban mentjük

                return true;  // Ha sikeres volt a mentés
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a foglalás mentése során: {ex.Message}");
                return false;  // Hiba esetén false
            }
        }

        // Foglalás lekérése a Firebase-ből a ReservationId alapján
        public async Task<Reservation> GetReservationById(string reservationId)
        {
            try
            {
                // Lekérjük a foglalás adatokat a "Reservation" gyűjteményből a megadott ReservationId alapján
                var reservationData = await FirebaseService
                    .Client
                    .Child("Reservation")  // "Reservation" gyűjtemény
                    .Child(reservationId)  // A ReservationId már string
                    .OnceSingleAsync<Reservation>();  // Az adatokat Reservation típusra deszerializáljuk

                return reservationData;  // A lekért foglalás adatokat visszaadjuk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a foglalás lekérése során: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }        
    }
}
