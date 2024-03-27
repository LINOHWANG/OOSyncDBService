using System;
using System.Collections.Generic;
using System.Configuration;
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
            string logPath = ConfigurationManager.AppSettings["ApplicationPath"] + "\\" + ConfigurationManager.AppSettings["LogPath"] + "\\";
            // Determine whether the directory exists.
            if (!Directory.Exists(logPath))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(logPath);
            }

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            strmsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + msg;
            System.IO.StreamWriter file = new System.IO.StreamWriter(logPath + "OOSyncLog2_" + DateTime.Now.ToString("yyyyMMdd") + ".log", true);
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
        public void SaveOrderAsFile(String collection)
        {

            // Set a variable to the Documents path.
            //string logPath = Directory.GetCurrentDirectory() + "\\Logs\\";
            string orderPath = ConfigurationManager.AppSettings["ApplicationPath"] + "\\" + ConfigurationManager.AppSettings["OrderPath"] + "\\";
            // Determine whether the directory exists.
            if (!Directory.Exists(orderPath))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(orderPath);
            }

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            strmsg = collection; //DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + collection;
            System.IO.StreamWriter file = new System.IO.StreamWriter(orderPath + "OrderCollection_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".ord", true);
            file.WriteLine(strmsg);

            file.Close();
        }
        internal string ReadOrderFromFile(string path)
        {
            //string path = ConfigurationManager.AppSettings["OrderPath"] + "\\OrderCollection_test.txt";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                Logger("Order File Not Found = " + path);
                return string.Empty;
            }

            // Open the file to read from.
            string readText = File.ReadAllText(path);
            return readText;
        }
        internal string ReadTestOrderFromFile()
        {
            //string path = ConfigurationManager.AppSettings["OrderPath"] + "\\OrderCollection_test.txt";
            string path = ConfigurationManager.AppSettings["ApplicationPath"] + "\\" + ConfigurationManager.AppSettings["OrderPath"] + "\\OrderCollection_test.txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                Logger("Test Order File Not Found = " + path);
                return string.Empty;
            }

            // Open the file to read from.
            string readText = File.ReadAllText(path);
            return readText;
        }
    }
}
