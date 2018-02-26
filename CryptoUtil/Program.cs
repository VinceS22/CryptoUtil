using System;
using System.Collections.Generic;

namespace CryptoUtil
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.Read();
        }

        static void FetchAllCoins()
        {

        }
    }

    // Represents a purchase made
    public class Purchase
    {
        decimal price;
        DateTime purchaseDateTime;

        public Purchase(decimal price, DateTime? purchaseDateTime)
        {
            this.price = price;
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
        public decimal CurrentPrice { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }


    }


    // Objects that inherit this interface are responsible for fetching data from the respective data store 
    public interface IDataService
    {
        IList<Purchase> GetPurchaseHistory();

        IList<Coin> GetAllCurrentCoins();

        /// <summary>
        /// Maybe we don't want to update the coin list all the time since we have >4500 coins to pick from.
        /// </summary>
        /// <param name="newCoinList">The list of coins that we want to add to the DB if they aren't there already</param>
        /// <param name="UpdateExisting">If true, we will just update current coins along with adding new coins.</param>
        /// <returns>True: New coin was added to CoinList. False: No new coins added to CoinList.</returns>
        bool UpdateCoinList(IList<Coin> newCoinList, bool UpdateExisting);
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

        public bool UpdateCoinList(IList<Coin> newCoinList, bool UpdateExisting)
        {
            return false;
        }
    }


    public interface IWebService
    {
        IList<Coin> GetAllCoins();
        IList<Coin> HydrateCoinsWithPrices(TimeFrame timeFrame);
        
    }

    public class CryptoCompareWebService : IWebService
    {
        public IList<Coin> GetAllCoins()
        {
            return new List<Coin>();
        }

        public IList<Coin> HydrateCoinsWithPrices(TimeFrame timeFrame)
        {
            return new List<Coin>();
        }
    }


    public enum TimeFrame
    {
        AllTime,
        Hourly,
        Minute
    }
}
