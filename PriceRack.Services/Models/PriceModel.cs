namespace PriceRack.Services
{
    public class PriceModel
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public string Instrument { get; set; }
    }
}