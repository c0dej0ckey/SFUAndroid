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
using SFUAndroid.Entities;
using SFUAndroid.Adapters;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Transit", ParentActivity=typeof(MainActivity))]
    public class TransitActivity : Activity
    {
        private static List<string> sStops = new List<string>() { "53096", "51861", "52998", "52807", "55836", "55738", "61035", "55070", "61787", "55210", "55713", "54993", "55714", "56406", "55441", "55612" };
        private static string apiKey = "AWkVpR4XnN8gmsf31mku";
        private List<BusRoute> mBusRoutes;
        private BusRouteAdapter mBusRouteAdapter;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Transit);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            mBusRoutes = new List<BusRoute>();
            mBusRouteAdapter = new BusRouteAdapter(this, Resource.Layout.BusRoute, mBusRoutes);
            ListView busRouteListView = FindViewById<ListView>(Resource.Id.BusRoutesListView);
            busRouteListView.Adapter = mBusRouteAdapter;
            // Create your application here
            GetBusTimes();
        }

        private void GetBusTimes()
        {
            foreach (string stop in sStops)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://api.translink.ca/RTTIAPI/V1/stops/{0}/estimates?apiKey={1}", stop, apiKey));
                request.Method = "GET";
                request.Accept = "application/json";
                request.BeginGetResponse(new AsyncCallback(GetBurnabyStopResponse), request);
            }
        }

        private void GetBurnabyStopResponse(IAsyncResult result)
        {
            string json = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                json = reader.ReadToEnd();
            }
            catch (Exception)
            {
                return;
            }
            JArray stopObject = JArray.Parse(json);
            string routeNo = stopObject[0]["RouteNo"].ToString();
            string routeName = stopObject[0]["RouteName"].ToString();

            BusRoute route = new BusRoute(routeNo, routeName);
            JArray times = stopObject[0]["Schedules"] as JArray;
            foreach (JObject obj in times)
            {
                string time = obj["ExpectedLeaveTime"].ToString();
                route.AddRouteTime(time);
            }

            RunOnUiThread(() =>
                {
                    mBusRoutes.Add(route);
                    mBusRouteAdapter.Add(route);
                    mBusRouteAdapter.NotifyDataSetChanged();
                });

        }

    }
}
