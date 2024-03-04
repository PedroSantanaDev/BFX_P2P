using BFXP2PAuction.Database;
using BFXP2PAuction.Models;

namespace BFXP2PAuction.BFXP2P
{
    public static class Auction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">Item to bid on</param>
        /// <param name="initialPrice">Initial price of the item</param>
        /// <param name="seller">Seller of the item</param>
        public static async void AuctionInitialization(string item, double initialPrice, AuctionParticipant seller)
        {
            using var context = new BFXAuctionContext();
            context.Auctions.Add(new BFXAuction
            {
                Item = item,
                InitialPrice = initialPrice,
                CurrentPrice = initialPrice,
                HighestBidder = seller.Name,
                Closed = false
            });
            await context.SaveChangesAsync();
            Console.WriteLine($"Auction for item {item} started by {seller.Name} with initial price {initialPrice} USDt.");
        }
        /// <summary>
        /// Closes an auction after a buyer wont.
        /// </summary>
        /// <param name="auction">An auction</param>
        public static async void CloseAuction(BFXAuction auction)
        {
            using var context = new BFXAuctionContext();
            auction.Closed = true;
            await context.SaveChangesAsync();
        }
    }
}
