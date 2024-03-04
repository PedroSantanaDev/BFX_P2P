namespace BFXP2PAuction.BFXP2P
{
    public static class Bid
    {
        public static void NotifyBid(string item, string client, double bidAmount)
        {
            Console.WriteLine($"Bid of {bidAmount} USDt placed on {item} by {client}.");
        }

        public static void NotifyAuctionWinningBid(string item, string winner, double winningBid)
        {
            Console.WriteLine($"{winner} won the auction for item {item} with a bid of {winningBid} USDt.");
        }
    }
}
