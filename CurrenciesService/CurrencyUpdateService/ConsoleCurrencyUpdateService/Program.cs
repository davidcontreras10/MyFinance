using System;
using System.Runtime.InteropServices;
using CurrencyUpdateService;

namespace ConsoleCurrencyUpdateService
{
    class Program
    {

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main()
        {
            StartService();
        }

        private static void StartService()
        {
            try
            {
                HideConsole();
                var service = new CurrencyExchangeUpdateService();
                service.Start();

            }
            catch (Exception ex)
            {
                ShowConsole();
                Console.WriteLine(ex);
                Console.ReadLine();
            }

        }

        private static void ShowConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOW);
        }

        private static void HideConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
