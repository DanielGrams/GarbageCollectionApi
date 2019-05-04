namespace GarbageCollectionApi.Models
{
    public class DataRefreshSettings
    {
        public int IntervalInDays { get; set; }

        public int StatusCheckInHours { get; set; }

        public int RequestDelayInMs { get; set; }
    }
}