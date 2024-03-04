using BFXP2PAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace BFXP2PAuction.Database
{
    public class BFXAuctionContext : DbContext
    {
        public DbSet<BFXAuction> Auctions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=BFX_auctions.db");
        }
    }
}
