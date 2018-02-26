using System;
using System.Collections.Generic;
using System.Linq;
using CryptoUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class WebServiceUnitTests
    {

        /// <summary>
        /// Assumes that Bitcoin hasn't been wiped off the face of the planet.
        /// </summary>
        [TestMethod]
        public void TestAllCoinsWebCall()
        {
            foreach (var type in GetAllWebServiceTestTypes())
            {
                Program.IWebService webService = Program.WebServiceFactory.BuildWebService(type);
                IList<Program.Coin> coinList = webService.GetAllCoins();

                Assert.AreEqual(true, coinList.Any(coin => coin.Name == "Bitcoin"));
            }
        }

        [TestMethod]
        public void TestHydrateCoinWithValidListing()
        {
            foreach (var type in GetAllWebServiceTestTypes())
            {
                Program.IWebService webService = Program.WebServiceFactory.BuildWebService(type);
                IList<Program.Coin> coinList = new List<Program.Coin>();
                coinList.Add(new Program.Coin() {Symbol = "BTC"});
                webService.HydrateCoinsWithPrices(coinList, Program.TimeFrame.Daily, 60);

                Assert.AreEqual(true, coinList[0].PriceCurrent > 0);
                Assert.AreEqual(true, coinList[0].PriceClose > 0);
                Assert.AreEqual(true, coinList[0].PriceHigh > 0);
                Assert.AreEqual(true, coinList[0].PriceLow > 0);
                Assert.AreEqual(true, coinList[0].PriceOpen > 0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHydrateCoinWithInvalidListing()
        {
            foreach (var type in GetAllWebServiceTestTypes())
            {
                Program.IWebService webService = Program.WebServiceFactory.BuildWebService(type);
                IList<Program.Coin> coinList = new List<Program.Coin>();
                coinList.Add(new Program.Coin() {Symbol = Guid.NewGuid().ToString()});

                webService.HydrateCoinsWithPrices(coinList, Program.TimeFrame.Daily, 60);
            }
        }

        private List<Program.WebServiceFactory.WebServiceType> GetAllWebServiceTestTypes()
        {
            return Enum.GetValues(typeof(Program.WebServiceFactory.WebServiceType)).Cast<Program.WebServiceFactory.WebServiceType>().ToList();
        }
    }
}
