using System;
using System.IO;

namespace DHCSDateUpdate
{
    public abstract class LogWriter
    {
        public static string GetTempPath()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!path.EndsWith("\\")) path += "\\";
            return path + "Log\\";
        }

        public static void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetTempPath() + string.Format("DHCSDateUpdate{0:MMddyyyy}.txt", DateTime.Today));
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
