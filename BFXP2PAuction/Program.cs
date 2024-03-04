using BFXP2PAuction.BFXP2P;
using BFXP2PAuction.Database;
using BFXP2PAuction.Models;
using Newtonsoft.Json.Linq;
using System.Net;


namespace BFXP2PAuction
{
    class Program
    {
        private static BFXAuction? _auction = null;
        static void Main(string[] args)
        {
            using (var context = new BFXAuctionContext())
            {
                context.Database.EnsureCreated();
            }

            // Listen for RPC requests
            Task.Run(BFXP2PAuctionStartServer);

            // Simulate auction
            Auction.AuctionInitialization("Pic#1", 75, new AuctionParticipant("Client#1"));
            Auction.AuctionInitialization("Pic#2", 60, new AuctionParticipant("Client#2"));

            // Simulate bidding
            BidOnAuction(new AuctionParticipant("Client#2"), "Pic#1", 75);
            BidOnAuction(new AuctionParticipant("Client#3"), "Pic#1", 75.5);
            BidOnAuction(new AuctionParticipant("Client#2"), "Pic#1", 80);

            Bid.NotifyAuctionWinningBid(_auction.Item, _auction.HighestBidder, _auction.CurrentPrice);
        }
        /// <summary>
        /// Start the server to start auctions
        /// </summary>
        static void BFXP2PAuctionStartServer()
        {
            // Listening for RPC requests
            HttpListener listener = new();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("Server listening on http://localhost:8080/");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                BFXP2PAuctionProcessRequest(context);
            }
        }

        /// <summary>
        /// Processes auction requests
        /// </summary>
        /// <param name="context">RPC context</param>
        static void BFXP2PAuctionProcessRequest(HttpListenerContext context)
        {
            // Process RPC
            StreamReader reader = new(context.Request.InputStream);
            string requestBody = reader.ReadToEnd();

            dynamic jsonRequest = JObject.Parse(requestBody);
            string method = jsonRequest.method;
            dynamic parameters = jsonRequest.parameters;

            // Handle RPC methods
            switch (method)
            {
                case "bid":
                    BidOnAuction(parameters.client, parameters.item, parameters.bidAmount);
                    break;
            }

            // Send response
            context.Response.ContentType = "application/json";
            JObject jsonResponse = new(new JProperty("status", "success"));
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(jsonResponse.ToString());
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();
        }

        /// <summary>
        /// Places a bid on an auction item
        /// </summary>
        /// <param name="client">Client that places the big</param>
        /// <param name="item">The item the client wants to bid on</param>
        /// <param name="bidAmount">Amount to bid</param>
        static void BidOnAuction(AuctionParticipant client, string item, double bidAmount)
        {
            using var context = new BFXAuctionContext();
            var auction = context.Auctions.FirstOrDefault(a => a.Item == item);
            if (auction == null)
            {
                Console.WriteLine($"Auction for item {item} not found.");
                return;
            }

            lock (auction)
            {
                if (!auction.Closed && bidAmount > auction.CurrentPrice)
                {
                    auction.CurrentPrice = bidAmount;
                    auction.HighestBidder = client.Name;
                    context.SaveChanges();
                    Console.WriteLine($"{client} placed a bid of {bidAmount} USDt on {item}.");

                    // Notify all participants about the bid
                    Bid.NotifyBid(auction.Item, client.Name, bidAmount);

                    // Check if the auction should be closed
                    if (bidAmount >= auction.InitialPrice && !auction.Closed)
                    {
                        Auction.CloseAuction(auction);
                    }
                }
                else
                {
                    if (bidAmount <= auction.InitialPrice && !auction.Closed)
                    {
                        Console.WriteLine($"{client.Name} placed a bid of {bidAmount} USDt on {item}. Bid amount must be higher than current price.");
                    }
                    // Check if the auction should be closed
                    if (bidAmount > auction.InitialPrice && !auction.Closed)
                    {
                        Console.WriteLine($"{client.Name} placed a bid of {bidAmount} USDt on {item}.");
                        Auction.CloseAuction(auction);
                    }
                }
            }

            if (auction.Closed)
            {
                _auction = auction;
            }
        }
    }
}

