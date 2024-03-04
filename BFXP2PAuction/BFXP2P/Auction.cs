﻿using BFXP2PAuction.Database;
using BFXP2PAuction.Models;

namespace BFXP2PAuction.BFXP2P
{
    public static class Auction
    {
        public static void AuctionInitialization(string item, double initialPrice, AuctionParticipant seller)
        {
            using (var context = new BFXAuctionContext())
            {
                context.Auctions.Add(new BFXAuction
                {
                    Item = item,
                    InitialPrice = initialPrice,
                    CurrentPrice = initialPrice,
                    HighestBidder = seller.Name,
                    Closed = false
                });
                context.SaveChanges();
                Console.WriteLine($"Auction for item {item} started by {seller.Name} with initial price {initialPrice} USDt.");
            }
        }

        public static void CloseAuction(BFXAuction auction)
        {
            using (var context = new BFXAuctionContext())
            {
                auction.Closed = true;
                context.SaveChanges();
            }
        }
    }
}
