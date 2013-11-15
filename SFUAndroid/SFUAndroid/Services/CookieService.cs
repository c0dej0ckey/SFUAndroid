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
using System.Net;

namespace SFUAndroid.Services
{
    public static class CookieService
    {
        private static List<Cookie> mCookies;

        static CookieService()
        {
            mCookies = new List<Cookie>();
        }

        public static void AddCookie(Cookie cookie)
        {
            mCookies.Add(cookie);
        }

        public static void RemoveCookie(Cookie cookie)
        {
            mCookies.Remove(cookie);
        }

        public static List<Cookie> GetCookies()
        {
            return mCookies;
        }

        public static void DeleteCookies()
        {
            mCookies.Clear();
        }

        public static Cookie GetCookieWithName(string name)
        {
            return mCookies.Where(c => c.Name == name).FirstOrDefault();
        }

        public static void RemoveCookieWithName(string name)
        {
            Cookie cookie = mCookies.Where(c => c.Name == name).FirstOrDefault();
            mCookies.Remove(cookie);
        }

        public static bool CookieExists(string name)
        {
            return mCookies.Where(c => c.Name == name).FirstOrDefault() != null;
        }


    }
}