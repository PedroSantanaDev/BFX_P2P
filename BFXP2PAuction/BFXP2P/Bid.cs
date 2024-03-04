namespace BFXP2PAuction.BFXP2P
{
    public static class Bid
    {
        /// <summary>
        /// Notify a big has been placed.
        /// </summary>
        /// <param name="item">Auctioned item</param>
        /// <param name="client">User bidding on the item</param>
        /// <param name="bidAmount">The amount bid on the item</param>
        public static void NotifyBid(string item, string client, double bidAmount)
        {
            Console.WriteLine($"Bid of {bidAmount} USDt placed on {item} by {client}.");
        }
        /// <summary>
        /// Notify of a winning bid
        /// </summary>
        /// <param name="item">Item for auction</param>
        /// <param name="winner">Winner bidder</param>
        /// <param name="winningBid">Winning bid amount</param>
        public static void NotifyAuctionWinningBid(string item, string winner, double winningBid)
        {
            Console.WriteLine($"{winner} won the auction for item {item} with a bid of {winningBid} USDt.");
        }
    }
}
