namespace BFXP2PAuction.Models
{
    public class BFXAuction
    {
        public int Id { get; set; }
        public string? Item { get; set; }
        public double InitialPrice { get; set; }
        public double CurrentPrice { get; set; }
        public string? HighestBidder { get; set; }
        public bool Closed { get; set; }
    }
}
