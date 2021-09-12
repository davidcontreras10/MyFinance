using System;
using System.ServiceProcess;
using CurrencyUpdateService;

namespace CurrencyUpdateWinService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            try
            {
                InternalLog.SimpleLog("Constructor enter");
                _currencyExchangeUpdateService = new CurrencyExchangeUpdateService();
            }
            catch (Exception ex)
            {
                InternalLog.SimpleLog("Constructor catch " + ex);
            }
            
        }
            
        protected override void OnStart(string[] args)
        {
            InternalLog.SimpleLog("OnStart Enter");
            try
            {
                _currencyExchangeUpdateService.Start();
            }
            catch (Exception ex)
            {
                InternalLog.SimpleLog("OnStart catch " + ex);
            }
            
        }

        protected override void OnStop()
        {
            try
            {
                _currencyExchangeUpdateService.Stop();
            }
            catch (Exception ex)
            {
                InternalLog.SimpleLog("OnStop catch " + ex);
            }

        }

        private readonly CurrencyExchangeUpdateService _currencyExchangeUpdateService;
    }
}
