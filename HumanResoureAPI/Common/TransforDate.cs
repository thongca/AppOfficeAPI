using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common
{
    public class TransforDate
    {
        public static double FromDateToDouble(DateTime date)
        {
            if (date == null)
            {
                return 0;
            } else
            {
                var dateDouble = (date - new DateTime(1970, 01, 01)).TotalSeconds;
                return dateDouble;
            }

        }
        public static DateTime FromDoubleToDate(double date)
        {
            if (date == 0)
            {
                return DateTime.Now;
            }
            else
            {
                var dateTime = new DateTime(1970, 01, 01).AddSeconds(date);
                return dateTime;
            }

        }
    }
}
