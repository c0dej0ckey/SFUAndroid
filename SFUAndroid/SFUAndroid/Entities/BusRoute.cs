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

namespace SFUAndroid.Entities
{
    public class Stop
    {
        private string mStopId;
        private List<BusRoute> mRoutes;

        public Stop(string stopId)
        {
            this.mStopId = stopId;
            mRoutes = new List<BusRoute>();
        }

        public string StopId
        {
            get { return mStopId; }
            set { mStopId = value; }
        }

        public List<BusRoute> Routes
        {
            get { return mRoutes; }
            set { mRoutes = value; }
        }
    }

    public class BusRoute
    {
        private string mRouteNumber;
        private string mRouteName;
        private List<string> mBusRouteTimes;

        public BusRoute(string routeNumber, string routeName)
        {
            this.mBusRouteTimes = new List<string>();
            this.mRouteName = routeName;
            this.mRouteNumber = routeNumber;
        }

        public string RouteNumber
        {
            get { return this.mRouteNumber; }
            set { this.mRouteNumber = value; }
        }

        public string RouteName
        {
            get { return this.mRouteName; }
            set { this.mRouteName = value; }
        }


        public string BusRouteTimes
        {
            get 
            {
                string busRouteTimes = string.Empty;
                foreach(string time in mBusRouteTimes)
                {
                    string[] timeDate = time.Split(' ');
                    busRouteTimes += timeDate[0] + " ";
                }
                return busRouteTimes;
            }
        }

        public List<string> BusRouteTime2
        {
            get { return this.mBusRouteTimes; }
            set { this.mBusRouteTimes = value; }
        }

        public void AddRouteTime(string time)
        {
            mBusRouteTimes.Add(time);
        }

        public void RemoveRouteTimes()
        {
            mBusRouteTimes.Clear();
        }
    }
}