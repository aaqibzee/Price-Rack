namespace PriceRack.DataAccess.Entities
{
    public class PriceDTO
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public string Instrument { get; set; }
    }
}
