using System;
using System.Diagnostics;

namespace AsCrone.Module
{
    public static class DayCounter
    {
        public static int GetLocalDay(string localDateTime)
        {
            DateTime localizedDate = DateTime.Parse(localDateTime);
            TimeSpan diffResult = DateTime.Now.Date - localizedDate.Date;
            int dayCount = (int)Math.Round(diffResult.TotalDays);
            return dayCount;
        }
    }

}



