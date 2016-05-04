using System;
using System.IO;
using System.Text;

namespace Q2C.Core
{
    public static class Log
    {
        private static string FilePath
        {
            get
            {
                return Path.Combine(AppEnvironment.Directory, DateTime.Today.ToString("yyyyMMdd") + ".log");
            }
        }

        public static void Write(Exception exception)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppEnvironment.Directory) && !Directory.Exists(AppEnvironment.Directory))
                    Directory.CreateDirectory(AppEnvironment.Directory);

                if (!File.Exists(FilePath))
                    using (File.Create(FilePath)) { }

                using (var sw = new StreamWriter(FilePath, true, Encoding.GetEncoding("windows-1251")))
                {
                    sw.Write("Дата/Время: ");
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("--------------------------------------");
                    sw.WriteLine(exception.ToString());
                    sw.WriteLine("--------------------------------------");
                    sw.WriteLine("");
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }
    }
}
