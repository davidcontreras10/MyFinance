using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyUpdateWinService
{
    internal static class InternalLog
    {
        private const string LOG_FILE = "C:\\temp\\CurrencyUpdateWinService\\Log.txt";

        public static void SimpleLog(string text)
        {
            try
            {
                if (File.Exists(LOG_FILE))
                {
                    text += "\n";
                    File.AppendAllText(LOG_FILE, text);
                }
            }
            catch
            {
                
            }
            
        }
    }
}
