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
            Task.Run(() => StartServer());

            // Simulate auction
            Auction.AuctionInitialization("Pic#1", 75, "Client#1");
            Auction.AuctionInitialization("Pic#2", 60, "Client#2");

            // Simulate bidding
            BidOnAuction("Client#2", "Pic#1", 75);
            BidOnAuction("Client#3", "Pic#1", 75.5);
            BidOnAuction("Client#2", "Pic#1", 80);

            Bid.NotifyAuctionWinningBid(_auction.Item, _auction.HighestBidder, _auction.CurrentPrice);
        }

        static void StartServer()
        {
            // Start listening for incoming RPC requests
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("Server listening on http://localhost:8080/");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ProcessRequest(context);
            }
        }

        static void ProcessRequest(HttpListenerContext context)
        {
            // Process RPC
            StreamReader reader = new StreamReader(context.Request.InputStream);
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
            JObject jsonResponse = new JObject(new JProperty("status", "success"));
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(jsonResponse.ToString());
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();
        }



        static void BidOnAuction(string client, string item, double bidAmount)
        {
            using (var context = new BFXAuctionContext())
            {
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
                        auction.HighestBidder = client;
                        context.SaveChanges();
                        Console.WriteLine($"{client} placed a bid of {bidAmount} USDt on {item}.");

                        // Notify all participants about the bid
                        Bid.NotifyBid(auction.Item, client, bidAmount);

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
                            Console.WriteLine($"{client} placed a bid of {bidAmount} USDt on {item}. Bid amount must be higher than current price.");
                        }
                        // Check if the auction should be closed
                        if (bidAmount > auction.InitialPrice && !auction.Closed)
                        {
                            Console.WriteLine($"{client} placed a bid of {bidAmount} USDt on {item}.");
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
}

