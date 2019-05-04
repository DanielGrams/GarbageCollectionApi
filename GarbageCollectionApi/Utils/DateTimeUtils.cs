namespace GarbageCollectionApi.Utils
{
    using System;

    public static class DateTimeUtils
    {
        public static DateTime Utc(int year, int month, int day, int hour, int minute = 0, int second = 0)
        {
            return DateTime.SpecifyKind(new DateTime(year, month, day, hour, minute, second), DateTimeKind.Utc);
        }

        public static DateTime Utc(int year, int month, int day)
        {
            return DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        }
    }
}