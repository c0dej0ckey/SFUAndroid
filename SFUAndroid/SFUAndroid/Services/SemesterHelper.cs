using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SFUAndroid.Services
{
    public static class SemesterHelper
    {
        public static int GetSemesterId()
        {
            DateTime currentTime = DateTime.Now;
            int year = currentTime.Year;
            int month = currentTime.Month;
            int day = currentTime.Day;
            if (month == 3 || month == 6 || month == 12)
            {
                if (day > 10)
                {
                    month += 1;
                    if (month == 13)
                    {
                        month = 1;
                        year += 1;
                    }
                    return 1000 + ((year - 2000) * 10) + month;
                }
            }
            if (month <= 3)
                month = 1;
            else if (month <= 8)
                month = 4;
            else if (month <= 12)
                month = 7;
            return 1000 + ((year - 2000) * 10) + month;

        }
    }
}