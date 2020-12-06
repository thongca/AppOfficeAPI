using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common.WorksCommon
{
    public class SpaceTimeOnDay
    {
        /// <summary>
        /// Tinh thoi gian lam viec tu thoi gian den thoi gian
        /// </summary>
        /// <param name="dates"></param>
        /// <param name="datee"></param>
        /// <returns></returns>
        public static double CalSpaceTimeOnDay(DateTime dates, DateTime datee)
        {
            double time = 0;
            double days = Convert.ToInt32((datee.Date - dates.Date).TotalDays) + 1;
            if (days == 1)
            {
                if (InDay(dates, datee) > 1)
                {
                    time = InDay(dates, datee);
                }
            }
            else if (days == 2)
            {
                if (TwoDay(dates, datee) > 1)
                {
                    time = TwoDay(dates, datee);
                }
            }
            else
            {
                if (ThreeDayOrMore(dates, datee) > 1)
                {
                    time = ThreeDayOrMore(dates, datee);
                }
            }
            return time;
        }
        private static double ThreeDayOrMore(DateTime dates, DateTime datee)
        {
            TimeSpan tssa = new TimeSpan(12, 00, 0);
            TimeSpan ssa = new TimeSpan(8, 00, 0);
            TimeSpan tsch = new TimeSpan(17, 00, 0);
            TimeSpan ts13 = new TimeSpan(13, 00, 0);
            double time = 0;
            double days = Convert.ToInt32((datee.Date - dates.Date).TotalDays) + 1;
            if (days == 3)
            {
                DateTime endDay1 = dates.Date + tsch;
                DateTime startDay2 = datee.Date + ssa;
                double timeday1 = InDay(dates, endDay1);
                double timeday2 = InDay(startDay2, datee);
                if (timeday1 + timeday2 > 1)
                {
                    time = timeday1 + timeday2 + 8 * 60;
                }
            }
            else if (days > 3)
            {
                DateTime endDay1 = dates.Date + tsch;
                DateTime startDay2 = datee.Date + ssa;
                double timeday1 = InDay(dates, endDay1);
                double timeday2 = InDay(startDay2, datee);
                if (timeday1 + timeday2 > 1)
                {
                    time = timeday1 + timeday2 + (days - 3) * 8 * 60;
                }
            }


            return time;
        }
        private static double TwoDay(DateTime dates, DateTime datee)
        {
            TimeSpan tssa = new TimeSpan(12, 00, 0);
            TimeSpan ssa = new TimeSpan(8, 00, 0);
            TimeSpan tsch = new TimeSpan(17, 00, 0);
            TimeSpan ts13 = new TimeSpan(13, 00, 0);
            double time = 0;
            DateTime endDay1 = dates.Date + tsch;
            DateTime startDay2 = datee.Date + ssa;
            double timeday1 = InDay(dates, endDay1);
            double timeday2 = InDay(startDay2, datee);
            if (timeday1 + timeday2 > 1)
            {
                time = timeday1 + timeday2;
            }
            return time;
        }
        private static double InDay(DateTime dates, DateTime datee)
        {
            TimeSpan tssa = new TimeSpan(12, 00, 0);
            TimeSpan ts8 = new TimeSpan(8, 00, 0);
            TimeSpan tsch = new TimeSpan(17, 00, 0);
            TimeSpan ts13 = new TimeSpan(13, 00, 0);
            double time = 0;
            if (dates.Hour < 8)
            {
                if ((datee - dates).TotalHours >= 0 && datee.Hour < 12) // nếu chỉ nằm trong buổi sáng
                {
                    time = (datee - (dates.Date + ts8)).TotalMinutes;
                }
                else if ((datee - dates).TotalHours >= 0 && datee.Hour == 12) // nếu giờ kết thúc nằm trong khoảng 12 - 13
                {
                    time = ((datee.Date + tssa) - (dates.Date + ts8)).TotalMinutes;
                }
                else if ((datee - dates).TotalHours >= 0 && datee.Hour >= 13 && datee.Hour < 17) // nếu giờ kết thúc nằm trong khoảng 12 - 13
                {
                    time = (datee - (dates.Date + ts8)).TotalMinutes - 60;
                }
                else if ((datee - dates).TotalHours >= 0 && datee.Hour >= 17) // nếu giờ kết thúc nằm trong khoảng 12 - 13
                {
                    time = ((datee.Date + tsch) - (dates.Date + ts8)).TotalMinutes - 60;
                }
            }
            else if (dates.Hour >= 8 && dates.Hour < 12)
            {
                if ((datee - dates).TotalHours >= 0 && datee.Hour < 12) // nếu chỉ nằm trong buổi sáng
                {
                    time = (datee - dates).TotalMinutes;
                }
                else if ((datee - dates).TotalHours >= 0 && datee.Hour == 12) // nếu giờ kết thúc nằm trong khoảng 12 - 13
                {
                    time = ((datee.Date + tssa) - dates).TotalMinutes;
                }
                else if ((datee - dates).TotalHours >= 1 && datee.Hour >= 13 && datee.Hour < 17) // nếu giờ kết thúc nằm lớn hơn 13
                {
                    time = (datee - dates).TotalMinutes - 60;
                }
                else if ((datee - dates).TotalHours >= 1 && datee.Hour >= 17) // nếu giờ kết thúc nằm lớn hơn 17
                {
                    time = ((datee.Date + tsch) - dates).TotalMinutes - 60;
                }
            }
            else if (dates.Hour == 12)
            {
                if (datee.Hour >= 14 && datee.Hour < 17) // giờ sau chỉ trong buổi chiều
                {
                    time = (datee - (dates.Date + ts13)).TotalMinutes;
                }
                else if (datee.Hour >= 14 && datee.Hour >= 17)
                {
                    time = ((datee.Date + tsch) - (dates.Date + ts13)).TotalMinutes;
                }
            }
            else if (dates.Hour > 12 && dates.Hour < 17)
            {
                if ((datee - dates).TotalHours >= 0 && datee.Hour < 17)
                {
                    time = (datee - dates).TotalMinutes;
                }
                else if ((datee - dates).TotalHours >= 0 && datee.Hour >= 17)
                {
                    time = ((datee.Date + tsch) - dates).TotalMinutes;
                }
            }
            else if (dates.Hour >= 17 && dates.Hour <= 23 && datee > dates)
            {
                  time = (datee - dates).TotalMinutes;
            }
            return time;
        }
    }
}
