using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

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
                    .Child(reservation.ReservationId)  // Az ID stringgé konvertálva
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

        public async Task<List<Reservation>> GetReservationsByRestaurantId(string restaurantId)
        {
            try
            {
                // Az összes foglalás lekérése a "Reservation" gyűjteményből
                var allReservations = await FirebaseService
                    .Client
                    .Child("Reservation") // "Reservation" gyűjtemény
                    .OnceAsync<Reservation>();

                // Csak azokat a foglalásokat szűrjük, amelyek a megadott RestaurantId-hez tartoznak
                var filteredReservations = allReservations
                    .Where(r => r.Object.RestaurantId == restaurantId)
                    .Select(r => r.Object)
                    .ToList();

                return filteredReservations; // Szűrt foglalások listájának visszaadása
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a foglalások lekérése során: {ex.Message}");
                return new List<Reservation>(); // Hiba esetén üres listát adunk vissza
            }
        }

        public async Task<List<Reservation>> GetReservationsByUserId(string userId)
        {
            try
            {
                // Az összes foglalás lekérése a "Reservation" gyűjteményből
                var allReservations = await FirebaseService
                    .Client
                    .Child("Reservation") // "Reservation" gyűjtemény
                    .OnceAsync<Reservation>();

                // Csak azokat a foglalásokat szűrjük, amelyek a megadott RestaurantId-hez tartoznak
                var filteredReservations = allReservations
                    .Where(r => r.Object.UserId == userId)
                    .Select(r => r.Object)
                    .ToList();

                return filteredReservations; // Szűrt foglalások listájának visszaadása
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a foglalások lekérése során: {ex.Message}");
                return new List<Reservation>(); // Hiba esetén üres listát adunk vissza
            }
        }


        public async Task<bool> UpdateReservationStatus(string reservationId, string newStatus)
        {
            try
            {
                // Lekérjük a foglalás adatokat a "Reservation" gyűjteményből a megadott ReservationId alapján
                var reservationRef = FirebaseService
                    .Client
                    .Child("Reservation")  // "Reservation" gyűjtemény
                    .Child(reservationId);  // A foglalás egyedi azonosítója

                // Az aktuális foglalás adatainak lekérése
                var reservationData = await reservationRef.OnceSingleAsync<Reservation>();

                // Ha a foglalás nem található, akkor visszatérünk false-szal
                if (reservationData == null)
                {
                    Console.WriteLine("A foglalás nem található.");
                    return false;
                }

                // A státusz frissítése
                reservationData.Status = newStatus;

                // Az új adatokat visszaírjuk a Firebase-be
                await reservationRef.PutAsync(reservationData);  // Az egész foglalást újraírjuk

                return true;  // Ha sikerült a frissítés
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a foglalás státuszának frissítésekor: {ex.Message}");
                return false;  // Hiba esetén false-t adunk vissza
            }
        }

    }
}
