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
using Android.Preferences;

namespace SFUAndroid.Services
{
    public static class Settings
    {

        public static string GetComputingId(Context context)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            return preferences.GetString("ComputingId", string.Empty);
        }

        public static string GetPassword(Context context)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            return preferences.GetString("Password", string.Empty);
        }


    }
}