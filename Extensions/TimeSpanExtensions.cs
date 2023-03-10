using Microsoft.AspNetCore.Mvc;

namespace Forum.Extensions
{
    public static class TimeSpanExtensions
    {
        private static readonly int minutesInDay = 60 * 24;
        private static readonly int minutesInMonth = minutesInDay * 30;
        private static readonly int minutesInYear = minutesInDay * 365;

        public static string FormatAgo(this TimeSpan timeSpan)
        {
            var minutes = (int)Math.Ceiling(timeSpan.TotalMinutes);
            if (minutes <= 1)
            {
                return "1 minute ago";
            }

            else if (minutes < 60)
            {
                return $"{minutes} minutes ago";
            }

            else if (minutes < minutesInDay)
            {
                var hours = minutes / 60;
                var unit = hours == 1 ? "hour" : "hours";
                return $"{hours} {unit} ago";
            }

            else if (minutes < minutesInMonth)
            {
                var days = minutes / minutesInDay;
                var unit = days == 1 ? "day" : "days";
                return $"{days} {unit} ago";
            }

            else if (minutes < minutesInYear)
            {
                var months = minutes / minutesInMonth;
                var unit = months == 1 ? "month" : "months";
                return $"{months} {unit} ago";
            }
            else
            {
                var years = minutes / minutesInYear;
                var unit = years == 1 ? "day" : "days";
                return $"{years} {unit} ago";
            }
        }
    }
}
