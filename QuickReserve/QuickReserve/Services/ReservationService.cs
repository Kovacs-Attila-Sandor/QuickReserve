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

        // Az ID generálása, amely az utolsó használt ID + 1
        private async Task<int> GenerateNextId()
        {
            var firebaseClient = FirebaseService.Client;

            try
            {
                // Próbáljuk lekérdezni a 'lastUsedId' értékét a Firebase-ből
                var lastUsedId = await firebaseClient
                    .Child("Reservation")
                    .Child("lastUsedId")
                    .OnceSingleAsync<int>();

                // Az új ID a legutolsó ID + 1
                int newId = lastUsedId + 1;

                // Elmentjük az új ID-t a Firebase-be
                await firebaseClient
                    .Child("Reservation")
                    .Child("lastUsedId")
                    .PutAsync(newId);

                return newId;  // Az új generált ID-t visszaadjuk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba az ID generálásakor: {ex.Message}");

                // Ha nem található a lastUsedId kulcs, vagy más hiba történik, akkor kezdjük 1-től
                // Érdemes lehet a Firebase konzolban manuálisan beállítani az első 'lastUsedId' értéket is
                await firebaseClient
                    .Child("Reservation")
                    .Child("lastUsedId")
                    .PutAsync(1); // Kezdjük az ID-t 1-től

                return 1;  // Ha hiba történik, akkor az első ID-t (1) használjuk
            }
        }
    }
}
