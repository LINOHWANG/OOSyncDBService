using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc
{
    class Utilities
    {
        private string strmsg = "";
        public void Logger(String msg)
        {

            // Set a variable to the Documents path.
            //string logPath = Directory.GetCurrentDirectory() + "\\Logs\\";
            string logPath = "C:\\OOSyncDBSvc\\Logs\\";
            // Determine whether the directory exists.
            if (!Directory.Exists(logPath))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(logPath);
            }

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            strmsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + msg;
            System.IO.StreamWriter file = new System.IO.StreamWriter(logPath + "OOSyncLog_" + DateTime.Now.ToString("yyyyMMdd") + ".log", true);
            file.WriteLine(strmsg);

            file.Close();
            //---------------------------------------------
            // Remove old log files : 14 days old
            string[] files = Directory.GetFiles(logPath);

            foreach (string onefile in files)
            {
                FileInfo fi = new FileInfo(onefile);
                if (fi.LastAccessTime < DateTime.Now.AddDays(-14))
                    fi.Delete();
            }
            //---------------------------------------------

        }
    }
}
