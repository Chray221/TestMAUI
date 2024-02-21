using System;
using System.Reflection;

namespace TestMAUI.Helpers.Extensions
{
	public static class SQLiteExteionsions
	{
		public static void CopyDatabaseToAppData(string databaseName)
        {
            string databasePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, databaseName);

            if (IsFileExisting(databasePath)) return;

            Task<Stream> task = FileSystem.OpenAppPackageFileAsync($"Resources/Raw/{databaseName}");
            var stream = task.Result;
            using MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            File.WriteAllBytes(databasePath, memoryStream.ToArray());
        }

        public static void CopyEmbeddedDatabaseToAppData(string resourcePath, string databaseName)
        {
            ////Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            //Assembly assembly = typeof(App).Assembly;
            //string databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            //using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            //using MemoryStream memoryStream = new MemoryStream();
            //stream.CopyTo(memoryStream);
            //File.WriteAllBytes(databasePath, memoryStream.ToArray());
            CopyEmbeddedDatabaseToAppData(
                resourcePath,
                Path.Combine(FileSystem.AppDataDirectory, databaseName));
        }

        public static void CopyEmbeddedDatabase(string resourcePath, string databasePath)
        {
            if (IsFileExisting(databasePath)) return;

            //Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            Assembly assembly = typeof(App).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            File.WriteAllBytes(databasePath, memoryStream.ToArray());
        }

        private static bool IsFileExisting(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}

