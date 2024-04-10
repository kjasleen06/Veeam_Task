using System;
using System.IO;
using static ProgrammingSession.Program;

namespace ProgrammingSession
{
    internal static class Log
    {
        private static string logPath = logPath ?? Data.logPath;
        enum MessageTags
        {
            Debug,
            Error
        }
        public static void Debug(string message)
        {
            PrintMsg(MessageTags.Debug.ToString(), message);

        }
        public static void Error(string message)
        {
            PrintMsg(MessageTags.Error.ToString(), message);

        }
        private static void PrintMsg(string tag, string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    var syntax = $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}" + " [" + tag + "] " + message;
                    sw.WriteLine(syntax);
                    Console.WriteLine(syntax);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }
        public static void LogCleanup()
        {
            var dir = Path.GetDirectoryName(logPath);
            FileInfo fi = new FileInfo(logPath);

            if (fi.Exists)
            {
                var dateTimeFormat = $"{DateTime.Now.ToString("MM/dd/yyyy hh:mm tt").Replace(" ", "_").Trim().Replace("-", "_").Replace(":", "_")}";
                var fileName = fi.Name.Split('.')[0];
                var newPath = Path.Combine(dir, fileName + dateTimeFormat + ".txt");
                File.Move(logPath, newPath);
            }
        }
    }
}
