using System;
using System.IO;
using System.Timers;

namespace ProgrammingSession
{
    internal class Program
    {
        public static Timer aTimer;
        static void Main(string[] args)
        {
            try
            {
                GetUserInput();

                aTimer = new Timer(Data.interval);
                aTimer.Elapsed += new ElapsedEventHandler(RunThis);
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }

        }
        private static void RunThis(object source, ElapsedEventArgs e)
        {
            SyncFolderContents();
        }
        public static void SyncFolderContents()
        {
            try
            {
                if (!Directory.Exists(Data.source))
                {
                    Directory.CreateDirectory(Data.source);
                    Log.Debug($"Created {Data.source} directory");
                }
                if (!Directory.Exists(Data.replica))
                {
                    Directory.CreateDirectory(Data.replica);
                    Log.Debug($"Created {Data.replica} directory");
                }

                var dir = new DirectoryInfo(Data.source);
                var destDir = new DirectoryInfo(Data.replica);

                foreach (FileInfo fi in destDir.GetFiles())
                {
                    var sourceFile = Path.Combine(dir.FullName, fi.Name);
                    if (!File.Exists(sourceFile))
                    {
                        fi.Delete();
                        Log.Debug($"{fi.Name} deleted from {fi.FullName}");
                    }
                }
                foreach (string sourceFile in Directory.GetFiles(dir.ToString()))
                {
                    FileInfo srcFile = new FileInfo(sourceFile);
                    string srcFileName = srcFile.Name;
                    FileInfo destFile = new FileInfo(Data.replica + srcFile.FullName.Replace(Data.source, ""));

                    if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists)
                    {
                        File.Copy(srcFile.FullName, destFile.FullName, true);
                        Log.Debug($"{srcFileName} copied from {srcFile.FullName} to {destFile.FullName} folder");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                aTimer.Stop();
                throw new Exception(ex.ToString());
            }
        }
        private static void GetUserInput()
        {
            try
            {
                Console.WriteLine("Enter the Log file path");
                Data.logPath = Console.ReadLine();
                if (Data.logPath == string.Empty)
                {
                    string msg = "Log file path cannot be empty";
                    Console.WriteLine(msg);
                    throw new Exception(msg);
                }
                Log.LogCleanup();

                Console.WriteLine("Enter the source folder path");
                Data.source = Console.ReadLine();
                Console.WriteLine("Enter the replica folder path");
                Data.replica = Console.ReadLine();
                Console.WriteLine("Enter the Synchronization interal in miliseconds");
                Data.interval = Convert.ToDouble(Console.ReadLine());
                if (Data.source == string.Empty || Data.replica == string.Empty)
                {
                    throw new Exception("source folder path & replica folder path cannot be empty");
                }
            }
            catch (FormatException)
            {
                throw;
            }
        }

        public static class Data
        {
            public static string source;
            public static string replica;
            public static double interval;
            public static string logPath;
        }
    }
}
