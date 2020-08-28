using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDB
{
    class Utilities
    {
        private string strmsg = "";
        public void Logger(String msg)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            strmsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg;
            System.IO.StreamWriter file = new System.IO.StreamWriter("Synclog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", true);
            file.WriteLine(strmsg);

            file.Close();

        }
    }
}
