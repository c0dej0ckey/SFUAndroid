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
    [Activity(Label = "Transit", ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class TransitActivity : Activity
    {
        private static List<string> sStops = new List<string>(); //{ "53096", "51861", "52998", "52807", "55836", "55738", "61035", "55070", "61787", "55210", "55713", "54993", "55714", "56406", "55441", "55612" };
        private List<BusRoute> mBurnabyBusRoutes;
        private List<BusRoute> mSurreyBusRoutes;
        private static string apiKey = "AWkVpR4XnN8gmsf31mku";
        private List<BusRoute> mBusRoutes;
        private BusRouteAdapter mBusRouteAdapter;
        private ListView mBusRouteListView;
        private IMenuItem mAddStopMenu;
        

        protected override void OnCreate(Bundle bundle)
        {
            //load stop ids and times
            //make card view swipeable
            //if swipeable remove the stop

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Transit);

            mBurnabyBusRoutes = new List<BusRoute>();
            mSurreyBusRoutes = new List<BusRoute>();
            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            mBusRoutes = new List<BusRoute>();
            mBusRouteAdapter = new BusRouteAdapter(this, Resource.Layout.BusRoute, mBurnabyBusRoutes);
            mBusRouteListView = FindViewById<ListView>(Resource.Id.BusRoutesListView);
            mBusRouteListView.Adapter = mBusRouteAdapter;
            if(mBusRouteListView.Count == 0)
            {
                mBusRouteListView.Visibility = ViewStates.Invisible;
            }
            Button button = this.FindViewById<Button>(Resource.Id.AddStopButton);
            button.Click += AddStop_ButtonClick;

            //this.ActionBar.AddTab(actionBar.NewTab().SetText("BURNABY").SetTabListener(this));
            //this.ActionBar.AddTab(actionBar.NewTab().SetText("SURREY").SetTabListener(this));
            

            //GetBusTimes();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.transit_activity_actions, menu);
            mAddStopMenu = menu.FindItem(Resource.Id.action_add_stop);
            return base.OnCreateOptionsMenu(menu);
        }

        public void AddStop_ButtonClick(object sender, EventArgs e)
        {
            EditText editText = this.FindViewById<EditText>(Resource.Id.BusStopIdEditText);
            if(editText.Text.Length != 5)
            {
                RunOnUiThread(() =>
                    {
                        Android.Widget.Toast.MakeText(this, "Stop Id not recognized", Android.Widget.ToastLength.Long).Show();
                    });
            }
            else
            {
                //get request
                //save stop id

            }
            editText.Visibility = ViewStates.Invisible;
            Button button = this.FindViewById<Button>(Resource.Id.AddStopButton);
            button.Visibility = ViewStates.Invisible;
            TextView textView = this.FindViewById<TextView>(Resource.Id.BusStopIdTextView);
            textView.Visibility = ViewStates.Invisible;


        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_add_stop:
                    AddStop();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }


        }


        private void AddStop()
        {
            EditText editText = this.FindViewById<EditText>(Resource.Id.BusStopIdEditText);

            if (editText.Visibility == ViewStates.Visible)
            { //cancel adding
                editText.Visibility = ViewStates.Invisible;
                Button button = this.FindViewById<Button>(Resource.Id.AddStopButton);
                button.Visibility = ViewStates.Invisible;
                TextView textView = this.FindViewById<TextView>(Resource.Id.BusStopIdTextView);
                textView.Visibility = ViewStates.Invisible;
                RunOnUiThread(() => mAddStopMenu.SetIcon(Resource.Drawable.ic_action_new));
            }
            else
            {
                editText.Visibility = ViewStates.Visible;
                Button button = this.FindViewById<Button>(Resource.Id.AddStopButton);
                button.Visibility = ViewStates.Visible;
                TextView textView = this.FindViewById<TextView>(Resource.Id.BusStopIdTextView);
                textView.Visibility = ViewStates.Visible;
                RunOnUiThread(() => mAddStopMenu.SetIcon(Resource.Drawable.ic_action_cancel));
            }
        }

        public override void OnBackPressed()
        {
            mBusRouteListView = null;
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            this.FinishAffinity();

            //base.OnBackPressed();
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
                    try
                    {
                        if (int.Parse(route.RouteNumber) < 300)
                        {
                            mBurnabyBusRoutes.Add(route);
                            mBusRouteAdapter.Add(route);
                            mBusRouteAdapter.NotifyDataSetChanged();
                        }
                        else
                        {
                            mSurreyBusRoutes.Add(route);

                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                });

        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
           // throw new NotImplementedException();
        }

        //public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        //{
        //    switch(tab.Position)
        //    {
        //        case 0:
        //            if (mBurnabyBusRoutes != null)
        //            {
        //                mBusRouteAdapter = new BusRouteAdapter(this, Resource.Layout.BusRoute, mBurnabyBusRoutes);
        //                mBusRouteAdapter.AddAll(mBurnabyBusRoutes);
                        
        //                mBusRouteListView.Adapter = mBusRouteAdapter;
        //                ((BaseAdapter)mBusRouteListView.Adapter).NotifyDataSetChanged();
        //                mBusRouteAdapter.NotifyDataSetChanged();
                        
        //            }
        //            break;
        //        case 1:
        //            if (mSurreyBusRoutes != null)
        //            {
        //                mBusRouteAdapter = new BusRouteAdapter(this, Resource.Layout.BusRoute, mSurreyBusRoutes);
        //                mBusRouteAdapter.AddAll(mSurreyBusRoutes);
        //                mBusRouteListView.Adapter = mBusRouteAdapter;
        //                ((BaseAdapter)mBusRouteListView.Adapter).NotifyDataSetChanged();
        //                mBusRouteAdapter.NotifyDataSetChanged();
        //            }
        //            break;
        //    }
        //}

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
           // throw new NotImplementedException();
        }

        
    }

  
    
}
