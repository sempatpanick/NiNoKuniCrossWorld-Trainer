using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNKCW_External
{

    public class ExchangeRateModel
    {
        public List<ItemExchangeRate> result { get; set; }
    }

    public class ItemExchangeRate
    {
        public string exchangeRate { get; set; }
        public string dataKey { get; set; }
        public string increaseExchangeRate { get; set; }
    }

    public class PriceModel
    {
        public int resultCode { get; set; }
        public string message { get; set; }
        public string tokenCode { get; set; }
        public ItemCurrenciesPrice currencies { get; set; }
    }

    public class ItemCurrenciesPrice
    {
        public ItemMBXCurrenciesPrice MBX { get; set; }
        public ItemUSDCurrenciesPrice USD { get; set; }
    }

    public class ItemMBXCurrenciesPrice
    {
        public string previousClosePriceMinor { get; set; }
        public string percentMinor { get; set; }
        public string volumeMinor { get; set; }
        public string priceMajor { get; set; }
        public string lastVolumeUpdated { get; set; }
        public string lastPreviousClosePriceUpdated { get; set; }
        public string previousClosePriceMajor { get; set; }
        public string volumeMajor { get; set; }
        public string counter { get; set; }
        public string percentMajor { get; set; }
        public string priceMinor { get; set; }
        public string lastPriceUpdated { get; set; }
    }

    public class ItemUSDCurrenciesPrice
    {
        public string previousClosePriceMinor { get; set; }
        public string percentMinor { get; set; }
        public string priceMajor { get; set; }
        public string lastPreviousClosePriceUpdated { get; set; }
        public string previousClosePriceMajor { get; set; }
        public string counter { get; set; }
        public string percentMajor { get; set; }
        public string priceMinor { get; set; }
        public string lastPriceUpdated { get; set; }
    }
}
