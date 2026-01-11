using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SalonBookingApp.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLiteNetExtensionsAsync.Extensions;

namespace SalonBookingApp.Data
{
    public class SalonDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public SalonDatabase(string dbPath)
        {
            
            _database = new SQLiteAsyncConnection(dbPath);
        }

        async Task Init()
        {
            await _database.CreateTableAsync<Client>();
            await _database.CreateTableAsync<Stylist>();
            await _database.CreateTableAsync<Service>();
            await _database.CreateTableAsync<Appointment>();
            await _database.CreateTableAsync<AppointmentService>();

            await SeedDatabaseAsync();
        }

      

        public async Task<List<Client>> GetClientsAsync()
        {
            await Init(); 
            return await _database.Table<Client>().ToListAsync();
        }

        public async Task<int> SaveClientAsync(Client client)
        {
            await Init(); 
            if (client.ID != 0)
                return await _database.UpdateAsync(client);
            else
                return await _database.InsertAsync(client);
        }

        public async Task<int> DeleteClientAsync(Client client)
        {
            await Init();
            return await _database.DeleteAsync(client);
        }

        public async Task<List<Stylist>> GetStylistsAsync()
        {
            await Init();
            return await _database.Table<Stylist>().ToListAsync();
        }

        public async Task<int> SaveStylistAsync(Stylist stylist)
        {
            await Init();
            if (stylist.ID != 0)
                return await _database.UpdateAsync(stylist);
            else
                return await _database.InsertAsync(stylist);
        }

        public async Task<int> DeleteStylistAsync(Stylist stylist)
        {
            await Init();
            return await _database.DeleteAsync(stylist);
        }

        // --- METODE PENTRU SERVICII ---

        public async Task<List<Service>> GetServicesAsync()
        {
            await Init();
            return await _database.Table<Service>().ToListAsync();
        }

        public async Task<int> SaveServiceAsync(Service service)
        {
            await Init();
            if (service.ID != 0)
                return await _database.UpdateAsync(service);
            else
                return await _database.InsertAsync(service);
        }

        public async Task<int> DeleteServiceAsync(Service service)
        {
            await Init();
            return await _database.DeleteAsync(service);
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            await Init();

            // 1. Luăm lista simplă de programări (fără relații)
            var appointments = await _database.Table<Appointment>().ToListAsync();

            // 2. Pentru fiecare programare, căutăm manual Clientul și Stilistul
            foreach (var a in appointments)
            {
                if (a.ClientID != 0)
                {
                    a.Client = await _database.Table<Client>()
                                    .Where(c => c.ID == a.ClientID)
                                    .FirstOrDefaultAsync();
                }

                if (a.StylistID != 0)
                {
                    a.Stylist = await _database.Table<Stylist>()
                                     .Where(s => s.ID == a.StylistID)
                                     .FirstOrDefaultAsync();
                }
            }

            return appointments;
        }
        public async Task<int> SaveAppointmentAsync(Appointment appointment)
        {
            await Init();
            if (appointment.ID != 0)
            {
                return await _database.UpdateAsync(appointment);
            }
            else
            {
                return await _database.InsertAsync(appointment);
            }
        }

        public async Task<int> DeleteAppointmentAsync(Appointment appointment)
        {
            await Init();
            return await _database.DeleteAsync(appointment);
        }
        public Task<List<Appointment>> GetAppointmentsWithChildrenAsync()
        {
            // Această funcție "lipește" automat datele despre Client, Stilist și Service
            // bazat pe ID-urile salvate.
            return _database.GetAllWithChildrenAsync<Appointment>(recursive: true);
        }

        public async Task SeedDatabaseAsync()
        {
            var existingStylists = await _database.Table<Stylist>().ToListAsync();
            if (existingStylists.Count == 0)
            {
                var officialStylists = new List<Stylist>
        {
            new Stylist { FirstName = "Ana", LastName = "Popescu", Specialization = "Coafură" },
            new Stylist { FirstName = "Andrei", LastName = "Ionescu", Specialization = "Barber Shop" },
            new Stylist { FirstName = "Elena", LastName = "Vasilescu", Specialization = "Manichiură" },
            new Stylist { FirstName = "Simona", LastName = "Marin", Specialization = "Cosmetică" },
            new Stylist { FirstName = "Andrada", LastName = "Popa", Specialization = "Machiaj" }
        };
                foreach (var s in officialStylists) await _database.InsertAsync(s);
            }
            // ... restul codului pentru servicii ...
            var existingServices = await _database.Table<Service>().ToListAsync();
            if (existingServices.Count == 0)
            {
                var officialServices = new List<Service>
        {
            // 1. Coafură și Coloristică (Durate în minute)
            new Service { Name = "Tuns Damă", Category = "Coafură", Price = 80, Duration = 45 },
            new Service { Name = "Styling", Category = "Coafură", Price = 100, Duration = 60 },
            new Service { Name = "Vopsit clasic", Category = "Coafură", Price = 200, Duration = 120 },
            new Service { Name = "Balayage", Category = "Coafură", Price = 450, Duration = 240 },
            new Service { Name = "Ombre", Category = "Coafură", Price = 400, Duration = 210 },

            // 2. Barber Shop
            new Service { Name = "Tuns bărbați", Category = "Barber Shop", Price = 50, Duration = 30 },
            new Service { Name = "Îngrijirea bărbii", Category = "Barber Shop", Price = 30, Duration = 20 },

            // 3. Manichiură și Pedichiură
            new Service { Name = "Manichiură semipermanentă", Category = "Unghii", Price = 90, Duration = 60 },
            new Service { Name = "Construcție gel", Category = "Unghii", Price = 150, Duration = 120 },
            new Service { Name = "Pedichiură", Category = "Unghii", Price = 100, Duration = 60 },

            // 4. Cosmetică
            new Service { Name = "Tratamente faciale", Category = "Cosmetică", Price = 180, Duration = 90 },
            new Service { Name = "Pensat și vopsit sprâncene", Category = "Cosmetică", Price = 60, Duration = 30 },

            // 5. Machiaj
            new Service { Name = "Machiaj de seară", Category = "Machiaj", Price = 250, Duration = 60 },
            new Service { Name = "Machiaj de mireasă", Category = "Machiaj", Price = 400, Duration = 90 }
        };

                foreach (var s in officialServices)
                    await _database.InsertAsync(s);
            }
        }

    }
}
