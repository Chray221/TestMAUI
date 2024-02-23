using System;
namespace TestMAUI.Helpers
{
	public static class MyConstants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public const string DictionaryResourceName = "TestMAUI.Resources.Files.EnglishDictionary.db";
        public const string DictionaryDBName = "EnglishDictionary.db";
        public static string DictionaryDBPath =>
            Path.Combine(FileSystem.AppDataDirectory, DictionaryDBName);

    }
}

