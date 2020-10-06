using HumanResource.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common
{
    public class HandleTaskSchedule
    {
        private static DateTime GetDateEndScheduleTask(int WorkTime)
        {
            DateTime futureDate = new DateTime();

            if (WorkTime <= 4)
            {
                return DateTime.Now.Date.AddHours(8 + WorkTime);
            }
            else if (WorkTime <= 8)
            {
                return DateTime.Now.Date.AddHours(8 + WorkTime + 1);
            }
            else
            {
                int day = WorkTime / 8;
                int hour = WorkTime % 8;
                if (hour == 0)
                {
                    futureDate = DateTime.Now.Date.AddDays(day - 1).AddHours(17);
                }
                else
                {
                    futureDate = DateTime.Now.Date.AddDays(day);
                    if (hour <= 4)
                    {
                        futureDate = futureDate.AddHours(8 + hour);
                    }
                    else if (hour <= 8)
                    {
                        futureDate = futureDate.AddHours(8 + hour + 1);
                    }
                }
                int sumCN = SumSunDay(futureDate.Date, DateTime.Now.Date);
                futureDate = futureDate.AddDays(sumCN);
            }

            return futureDate;
        }
        private static int SumSunDay(DateTime fromDate, DateTime toDate)
        {
            DateTime sunDayFirst = fromDate.AddDays((DayOfWeek.Sunday + 7 - fromDate.DayOfWeek) % 7);
            int totalDays = (toDate - sunDayFirst).Days;
            int totalSunDays = totalDays / 7;
            return totalSunDays;
        }
    }
}
