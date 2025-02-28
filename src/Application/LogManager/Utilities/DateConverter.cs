namespace MicroservicesLogger.Utilities
{
    public static class DateConverter
    {
        public static DateTime ToBrazilianDateTime(this DateTime dateTime) => ConvertToBrazilianDateTimeZone(dateTime).DateTime;

        private static DateTimeOffset ConvertToBrazilianDateTimeZone(DateTime dateTime) =>
            TimeZoneInfo.ConvertTime(dateTime, Environment.OSVersion.Platform == PlatformID.Win32NT ?
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time") :
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"));
    }
}