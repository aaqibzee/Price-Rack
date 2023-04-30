namespace PriceRack.Common.Extensions
{
    public static  class TimeExtension
    {
        public static DateTime RoundHour(this DateTime time)
        {
            return time.Date.AddHours(time.Hour);
        }

        public static  long ConvertToUnixTime(this DateTime dateTime)
        {
            var unixEpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixEpochTime).Ticks;
            var unixTimeStampInMilliseconds = unixTimeStampInTicks / TimeSpan.TicksPerMillisecond;
            return unixTimeStampInMilliseconds / 1000;
        }
    }
}
