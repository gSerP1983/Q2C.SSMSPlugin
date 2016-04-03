using System;
using System.IO;

namespace Q2C.Core
{
    public static class AppEnvironment
    {
        public static string Directory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Q2C.SSMSPlugin");
            }
        }
    }
}