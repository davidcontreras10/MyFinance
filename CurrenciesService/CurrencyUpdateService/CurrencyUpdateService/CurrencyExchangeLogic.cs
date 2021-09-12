using System.Configuration;

namespace CurrencyUpdateService
{
    public class CurrencyExchangeLogic
    {
        #region Attributes

        private readonly CurrencyExchangeDbService _currencyExchangeDbService;
        private readonly DolarBancoCentralInterface _dolarBancoCentralInterface;

        #endregion

        #region Public Methods

        public void UpdateCurrencyExchangeData()
        {
            var list = _dolarBancoCentralInterface.GetTodaysBccrVentanillaModel();
            _currencyExchangeDbService.InsertBccrVentanillaModel(list);
        }

        public void UpdateCurrencyExchangeDataByHtmlDocument(string htmlStringDoc)
        {
            var list = _dolarBancoCentralInterface.GetTodaysBccrVentanillaModelFromStringDoc(htmlStringDoc);
            _currencyExchangeDbService.InsertBccrVentanillaModel(list);
        }

        public void UpdateCurrencyExchangeDataByUrl(string url)
        {
            var list = _dolarBancoCentralInterface.GetTodaysBccrVentanillaModel(url);
            _currencyExchangeDbService.InsertBccrVentanillaModel(list);
        }

        #endregion

        #region Private Methods



        #endregion

        #region Constructor

        public CurrencyExchangeLogic()
        {
            _currencyExchangeDbService = new CurrencyExchangeDbService(new CurrencyUpdateServiceConnectionConfig());
            _dolarBancoCentralInterface = new DolarBancoCentralInterface();
        }

        #endregion
    }
}
