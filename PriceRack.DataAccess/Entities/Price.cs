namespace PriceRack.DataAccess.Entities
{
    public class Price
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public string Instrument { get; set; }
        public Price()
        {

        }
        public Price(DateTime Time, decimal Value, string Instrument)
        {
            this.Time = Time;
            this.Value = Value;
            this.Instrument = Instrument;
        }
    }
}
