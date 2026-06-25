using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using SalonBookingApp.Models;
using SupabaseClient = Supabase.Client;
using MyClientModel = SalonBookingApp.Models.Client;

namespace SalonBookingApp.Data
{
    public partial class SalonDatabase
    {
        private readonly SupabaseClient _client;

        public SalonDatabase(SupabaseClient client)
        {
            _client = client;
        }

        public async Task<MyClientModel> GetClientByLoginAsync(string username, string password)
        {
            var response = await _client
                .From<MyClientModel>()
                .Where(x => x.Username == username)
                .Where(x => x.Password == password)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<Stylist> GetStylistByLoginAsync(string username, string password)
        {
            var response = await _client
                .From<Stylist>()
                .Where(x => x.Username == username)
                .Where(x => x.Password == password)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<List<MyClientModel>> GetClientsAsync()
        {
            var response = await _client.From<MyClientModel>().Get();
            return response.Models;
        }

        public async Task<MyClientModel> SaveClientAsync(MyClientModel client)
        {
            if (client.ID != 0)
            {
                var response = await _client.From<MyClientModel>().Update(client);
                return response.Model;
            }
            else
            {
                var response = await _client.From<MyClientModel>().Insert(client);
                return response.Model;
            }
        }

        public async Task DeleteClientAsync(MyClientModel client)
        {
            await _client.From<MyClientModel>().Delete(client);
        }

        public async Task<List<Stylist>> GetStylistsAsync()
        {
            var response = await _client.From<Stylist>().Get();
            return response.Models;
        }

        public async Task<Stylist> SaveStylistAsync(Stylist stylist)
        {
            if (stylist.ID != 0)
            {
                var response = await _client.From<Stylist>().Update(stylist);
                return response.Model;
            }
            else
            {
                var response = await _client.From<Stylist>().Insert(stylist);
                return response.Model;
            }
        }

        public async Task UpdateStylistAsync(Stylist stylist)
        {
            await _client.From<Stylist>().Update(stylist);
        }

        public async Task DeleteStylistAsync(Stylist stylist)
        {
            await _client.From<Stylist>().Delete(stylist);
        }

        public async Task<List<Service>> GetServicesAsync()
        {
            var response = await _client.From<Service>().Get();
            return response.Models;
        }

        public async Task<Service> SaveServiceAsync(Service service)
        {
            if (service.ID != 0)
            {
                var response = await _client.From<Service>().Update(service);
                return response.Model;
            }
            else
            {
                var response = await _client.From<Service>().Insert(service);
                return response.Model;
            }
        }

        public async Task DeleteServiceAsync(Service service)
        {
            await _client.From<Service>().Delete(service);
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            var response = await _client.From<Appointment>().Get();
            var appointments = response.Models;

            foreach (var app in appointments)
            {
                try
                {
                    var sResponse = await _client.From<Stylist>().Where(x => x.ID == app.StylistID).Single();
                    app.Stylist = sResponse;

                    var serResponse = await _client.From<Service>().Where(x => x.ID == app.ServiceID).Single();
                    app.Service = serResponse;
                }
                catch { }
            }

            return appointments;
        }

        public async Task<Appointment> SaveAppointmentAsync(Appointment appointment)
        {
            if (appointment.ID != 0)
            {
                var response = await _client.From<Appointment>().Update(appointment);
                return response.Model;
            }
            else
            {
                var response = await _client.From<Appointment>().Insert(appointment);
                return response.Model;
            }
        }

        public async Task DeleteAppointmentAsync(Appointment appointment)
        {
            await _client.From<Appointment>().Delete(appointment);
        }

        public async Task<List<Review>> GetReviewsForStylistAsync(int stylistId)
        {
            var response = await _client.From<Review>().Where(x => x.StylistID == stylistId).Get();
            return response.Models;
        }

        public async Task<Review> SaveReviewAsync(Review review)
        {
            if (review.ID != 0)
            {
                var response = await _client.From<Review>().Update(review);
                return response.Model;
            }
            else
            {
                var response = await _client.From<Review>().Insert(review);
                return response.Model;
            }
        }

        public async Task<List<ClientPackage>> GetPackagesForClientAsync(int clientId)
        {
            var response = await _client.From<ClientPackage>().Where(x => x.ClientID == clientId).Get();
            return response.Models;
        }

        public async Task<ClientPackage> SaveClientPackageAsync(ClientPackage package)
        {
            if (package.ID != 0)
            {
                var response = await _client.From<ClientPackage>().Update(package);
                return response.Model;
            }
            else
            {
                var response = await _client.From<ClientPackage>().Insert(package);
                return response.Model;
            }
        }

        public async Task DeleteClientPackageAsync(ClientPackage package)
        {
            await _client.From<ClientPackage>().Delete(package);
        }

        public async Task<List<Stylist>> GetStylistsForServiceAsync(int serviceId)
        {
            var links = await _client.From<StylistService>().Where(x => x.ServiceID == serviceId).Get();
            var ids = links.Models.Select(x => x.StylistID).ToList();

            var stylists = await _client.From<Stylist>().Get();
            return stylists.Models.Where(x => ids.Contains(x.ID)).ToList();
        }

        public async Task<List<Service>> GetServicesForStylistAsync(int stylistId)
        {
            var links = await _client.From<StylistService>().Where(x => x.StylistID == stylistId).Get();
            var serviceIds = links.Models.Select(x => x.ServiceID).ToList();

            var allServices = await GetServicesAsync();
            return allServices.Where(s => serviceIds.Contains(s.ID)).ToList();
        }

        public async Task UpdateStylistServicesAsync(int stylistId, List<Service> selectedServices)
        {
            await _client.From<StylistService>().Where(x => x.StylistID == stylistId).Delete();

            var newLinks = selectedServices.Select(s => new StylistService
            {
                StylistID = stylistId,
                ServiceID = s.ID
            }).ToList();

            if (newLinks.Any())
            {
                await _client.From<StylistService>().Insert(newLinks);
            }
        }

        public async Task<Stylist> GetStylistWithServicesAsync(int stylistId)
        {
            var response = await _client.From<Stylist>().Where(x => x.ID == stylistId).Single();
            return response;
        }

        public async Task<WorkScheduleDb> GetWorkScheduleAsync(int stylistId, DateTime date)
        {
            try
            {
                var response = await _client.From<WorkScheduleDb>()
                    .Where(x => x.StylistId == stylistId && x.ScheduleDate == date)
                    .Single();
                return response;
            }
            catch
            {
                return null;
            }
        }

        public async Task<WorkScheduleDb> SaveWorkScheduleAsync(WorkScheduleDb schedule)
        {
            if (schedule.Id != 0)
            {
                var response = await _client.From<WorkScheduleDb>().Update(schedule);
                return response.Model;
            }
            else
            {
                var response = await _client.From<WorkScheduleDb>().Insert(schedule);
                return response.Model;
            }
        }

    }
}