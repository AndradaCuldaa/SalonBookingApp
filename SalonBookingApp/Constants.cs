using System;
using System.Collections.Generic;
using System.Text;

namespace SalonBookingApp
{
    public static class Constants
    {
        public const string DatabaseFilename = "SalonBooking.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            
            SQLite.SQLiteOpenFlags.ReadWrite |
            
            SQLite.SQLiteOpenFlags.Create |
            
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
