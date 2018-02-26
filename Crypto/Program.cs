using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CryptoUtil
{
    public class Program
    {
        private static bool debug = true;
        static void Main(string[] args)
        {
            Console.WriteLine("Crypto Tool");
            Console.WriteLine("Pick a number");
            Console.WriteLine("1. Pick a crypto coin");
            Console.WriteLine("2. Log a purchase");
            var availableChoices = new[] { 1, 2 };
            var choice = GetInput(availableChoices);
            switch (choice)
            {
                case 1:
                    ReturnCoin();
                    break;
                case 2:
                    LogPurchase();
                    break;
                default:
                    break;
            }
        }

        private static void ReturnCoin()
        {
            var availableChoices = new[] {1, 2};
            Console.WriteLine("How would you like to determine your coin?");
            Console.WriteLine("1. Random weighed towards low Market Cap");
            Console.WriteLine("2. Completely random");

            var choice = GetInput(availableChoices);

            var webService = WebServiceFactory.BuildWebService(WebServiceFactory.WebServiceType.CryptoCompare);
            var coinList = webService.GetAllCoins();
            webService.HydrateCoinsWithPrices(coinList, TimeFrame.Minute, 1);


            switch (choice)
            {
                case 1:
                    AssignWeights(coinList);
                    ProcessWeightedRandomChoice(coinList);
                    break;
                case 2:

                    break;
                default:
                    break;
            }

            Console.ReadLine();
        }

        private static void ProcessWeightedRandomChoice(IList<Coin> coinList)
        {
            if (coinList.Count == 0)
            {
                throw new ArgumentException("Empty list given");
            }

            int totalweight = coinList.Sum(c => c.Weight);
            Random rand = new Random();
            int randNumber = rand.Next(totalweight);
            var selectedCoin = new Coin();
            if (debug)
            {
                Console.WriteLine("totalweight: " + totalweight);
                Console.WriteLine("Random Number: " + randNumber);)
            }

            foreach (var coin in coinList)
            {
                if (randNumber < coin.Weight)
                {
                    selectedCoin = coin;
                    break;
                }

                randNumber = randNumber - coin.Weight;
            }


            Console.WriteLine("And your coin is: ");
            Console.WriteLine(selectedCoin.ToString());
        }
        private static int GetInput(int[] possibleChoices)
        {
            var correctInput = false;
            var result = -1;
            while (!correctInput)
            {
                var input = Console.ReadLine();
                int.TryParse(input, out result);

                if (possibleChoices.Length > 0 && possibleChoices.Any(val => val == result))
                    correctInput = true;
                else if (possibleChoices.Length == 0 && result != 0)
                    correctInput = true;
            }
            return result;
        }

        /// <summary>
        /// Assuming that the service is reuturning in order by market cap dominance from beginning of the list being highest market cap
        /// and will be less market cap as we go down the lst.
        /// </summary>
        /// <param name="coinList">List of coins that we are going to assign weights to for ramdom number purposes.</param>
        private static void AssignWeights(IList<Coin> coinList)
        {
            for (int i = 0; i < coinList.Count; i++)
            {
                coinList[i].Weight = i + 1;
            }
        }

        private static void LogPurchase()
        {
            throw new NotImplementedException();
        }

        #region Model Objects
        // Represents a purchase made
        public class Purchase
        {
            public Coin Coin { get; set; }
            public DateTime PurchaseDateTime { get; set; }

            public Purchase(Coin coin, DateTime? purchaseDateTime)
            {
                this.Coin = coin;
                purchaseDateTime = purchaseDateTime.HasValue ? purchaseDateTime : DateTime.Now;
            }
        }

        public class Coin
        {
            // Name of the coin. EX: Bitcoin
            public string Name { get; set; }
            // Symbol of coin. EX: BTC
            public string Symbol { get; set; }
            // Price of coin with in currency format. EX: 
            public decimal PriceCurrent { get; set; }

            public decimal PriceHigh { get; set; }

            public decimal PriceLow { get; set; }

            public decimal PriceOpen { get; set; }

            public decimal PriceClose { get; set; }

            public int MarketCap { get; set; }

            public int Weight { get; set; }
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("Name: ");
                sb.Append(Name);
                sb.Append(" (");
                sb.Append(Symbol);
                sb.AppendLine(")");
                sb.Append("Market Cap: ");
                sb.AppendLine(MarketCap.ToString());
                sb.AppendLine("Weight: ");
                sb.Append(Weight.ToString());

                return sb.ToString();
            }
        }
        #endregion
        #region DataService
        public class DataServiceFactory
        {
            public static IDataService BuildDataService(DataServiceType type)
            {
                if (type == DataServiceType.SqlServer)
                {
                    return new SqlServerCoinDataService();
                }

                return null;
            }

            public enum DataServiceType
            {
                SqlServer
            }
        }
        // Objects that inherit this interface are responsible for fetching data from the respective data store 
        public interface IDataService
        {
            
            IList<Coin> GetAllCurrentCoins();

            /// <summary>
            /// Maybe we don't want to update the coin list all the time since we have >4500 coins to pick from.
            /// </summary>
            /// <param name="newCoinList">The list of coins that we want to add to the DB if they aren't there already</param>
            /// <param name="updateExisting">If true, we will just update current coins along with adding new coins.</param>
            /// <returns>True: New coin was added to CoinList. False: No new coins added to CoinList.</returns>
            bool UpdateCoinList(IList<Coin> newCoinList, bool updateExisting);

            IList<Purchase> GetPurchaseHistory();

            bool ProcessPurchase(Purchase purchase);

        }

        public class SqlServerCoinDataService : IDataService
        {
            public IList<Coin> GetAllCurrentCoins()
            {
                return new List<Coin>();
            }

            public IList<Purchase> GetPurchaseHistory()
            {
                return new List<Purchase>();
            }

            public bool ProcessPurchase(Purchase purchase)
            {

                return true;
            }

            public bool UpdateCoinList(IList<Coin> newCoinList, bool updateExisting)
            {
                return false;
            }
        }

        #endregion
        #region WebService
        public class WebServiceFactory
        {
            public static IWebService BuildWebService(WebServiceType type)
            {
                if (type == WebServiceType.CryptoCompare)
                {
                    return new CryptoWebService();
                }
                if (type == WebServiceType.Test)
                {
                    return new TestWebService();
                }
                return null;
            }

            public enum WebServiceType
            {
                CryptoCompare,
                Test
            }

        }

        public interface IWebService
        {
            IList<Coin> GetAllCoins();
            IList<Coin> HydrateCoinsWithPrices(IList<Coin> coinsToHydrate, TimeFrame timeFrame, int numberOfPoints);

        }

        public class CryptoWebService : IWebService
        {
            public IList<Coin> GetAllCoins()
            {
                return new List<Coin>();
            }

            public IList<Coin> HydrateCoinsWithPrices(IList<Coin> coinsToHydrate, TimeFrame timeFrame, int numberOfPoints)
            {
                return new List<Coin>();
            }
        }

        public class TestWebService : IWebService
        {
            public IList<Coin> GetAllCoins()
            {
                var list = new List<Coin>
                {
                    new Coin { Name = "1", Symbol = "1c"},
                    new Coin { Name = "2" , Symbol = "2"},
                    new Coin { Name = "3", Symbol = "3c" },
                    new Coin { Name = "4", Symbol = "4c" },
                    new Coin { Name = "5", Symbol = "5c" },
                    new Coin { Name = "6", Symbol = "6c" },
                    new Coin { Name = "7", Symbol = "7c" },
                    new Coin { Name = "8", Symbol = "8c" }
                };

                return list;
            }

            public IList<Coin> HydrateCoinsWithPrices(IList<Coin> coinsToHydrate, TimeFrame timeFrame, int numberOfPoints)
            {
                return new List<Coin>();
            }
        }
        #endregion
        #region enums

        public enum TimeFrame
        {
            AllTime,
            Daily,
            Hourly,
            Minute
        }
        #endregion enums

    }
}