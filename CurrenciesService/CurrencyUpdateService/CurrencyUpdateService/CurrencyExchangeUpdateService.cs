using System;
using System.Configuration;
using System.Threading;

namespace CurrencyUpdateService
{
    public class CurrencyExchangeUpdateService
    {

        #region Constructor

        public CurrencyExchangeUpdateService()
        {
            _currencyExchangeLogic = new CurrencyExchangeLogic();
            _runningServiceFlag = true;
        }

        #endregion

        #region Service Methods
        public void Start()
        {
            _runningServiceFlag = true;
            StartRunningService();
        }

        public void Stop()
        {
            _runningServiceFlag = false;
        }

        public void Pause()
        {

        }

        #endregion

        #region Private Methods

        private static int GetNextUpdateTime()
        {
            const int defaultValue = 5 * 60 * 1000;
            try
            {
                var updateTimeValue = ConfigurationManager.AppSettings["UpdateTime"];
                int updateTime;
                updateTime = int.TryParse(updateTimeValue, out updateTime) ? updateTime : defaultValue;
                return updateTime;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private void StartRunningService()
        {
            new Thread(StartRunningServiceThread).Start(this);
        }

        private void StartRunningServiceThread(object serviceObject)
        {
            var service = (CurrencyExchangeUpdateService) serviceObject;
            service.RunningService();
        }

        private void RunningService()
        {
            while (_runningServiceFlag)
            {
                try
                {
                    var updateTime = GetNextUpdateTime();
                    Thread.Sleep(updateTime);
                    CallUpdateExecute();
                }
                catch (Exception)
                {
                }
                
            }
        }

        private bool GetUseThreadConfigValue()
        {
            try
            {
                var useThreadConfigValue = ConfigurationManager.AppSettings["UserThreadConfig"];
                bool value;
                value = bool.TryParse(useThreadConfigValue, out value) && value;
                return value;
            }
            catch (Exception)
            {
                return false;
            }
            
            
        }

        private void CallUpdateExecute()
        {
            try
            {
                if (GetUseThreadConfigValue())
                {
                    var thread = new Thread(CallUpdateExecuteThread);
                    thread.Start(this);
                }
                else
                {
                    CallUpdateExecuteAction();
                }

            }
            catch (Exception)
            {

            }
        }

        private void CallUpdateExecuteAction()
        {
            _currencyExchangeLogic.UpdateCurrencyExchangeData();
        }

        private static void CallUpdateExecuteThread(object serviceObject)
        {
            try
            {
                var service = (CurrencyExchangeUpdateService)serviceObject;
                service._currencyExchangeLogic.UpdateCurrencyExchangeData();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Attributes

        private readonly CurrencyExchangeLogic _currencyExchangeLogic;
        private bool _runningServiceFlag;

        #endregion
    }
}
