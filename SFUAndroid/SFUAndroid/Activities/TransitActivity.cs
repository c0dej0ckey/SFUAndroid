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
using Com.Fima.Cardsui.Views;
using Com.Fima.Cardsui.Objects;
using Newtonsoft.Json;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Transit", ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class TransitActivity : Activity
    {
        private static List<string> sStops = new List<string>(); //{ "53096", "51861", "52998", "52807", "55836", "55738", "61035", "55070", "61787", "55210", "55713", "54993", "55714", "56406", "55441", "55612" };
        private List<BusRoute> mBusRoutes;
        private static string apiKey = "AWkVpR4XnN8gmsf31mku";
        private BusRouteAdapter mBusRouteAdapter;
        private ListView mBusRouteListView;
        private IMenuItem mAddStopMenu;
        private CardUI mCardView;
        

        protected override void OnCreate(Bundle bundle)
        {
            //load stop ids and times
            //make card view swipeable
            //if swipeable remove the stop

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Transit);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            mBusRoutes = new List<BusRoute>();
            mBusRoutes = LoadBuses();
           
            mCardView = this.FindViewById<CardUI>(Resource.Id.BusRouteCardUI);
            mCardView.SetSwipeable(true);

            if (mBusRoutes != null)
            {
                foreach (BusRoute route in mBusRoutes)
                {
                    CardStack cs = new CardStack();
                    mCardView.AddStack(cs);
                    MyCard card = new MyCard(route.RouteNumber + "\t" + route.RouteName + "\t" + route.StopId, route.BusRouteTimes);
                    mCardView.AddCard(card);
                }
            }

            


        }



        private void AddStop()
        {

            AddStopDialogFragment fragment = new AddStopDialogFragment();
            fragment.Show(FragmentManager, "Add Stop");
            
        }

        private void Refresh()
        {
            RunOnUiThread(() =>
                {
                    mCardView.ClearCards();
                });

            List<string> routes = mBusRoutes.Select(b => b.StopId).ToList();
            mBusRoutes.Clear();

            foreach(string route in routes)
            {
                GetStop(route);
            }

        }

       

        public void GetStop(string stopId)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://api.translink.ca/RTTIAPI/V1/stops/{0}/estimates?apiKey={1}", stopId, apiKey));
            request.Method = "GET";
            request.Accept = "application/json";
            request.BeginGetResponse(new AsyncCallback(GetStopResponse), request);
        }


        private void GetStopResponse(IAsyncResult result)
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
            catch (Exception e)
            {
                return;
            }
            JArray stopObject = JArray.Parse(json);
            string routeNo = stopObject[0]["RouteNo"].ToString();
            string routeName = stopObject[0]["RouteName"].ToString();

            BusRoute route = new BusRoute(routeNo, routeName, "");
            JArray times = stopObject[0]["Schedules"] as JArray;
            foreach (JObject obj in times)
            {
                string time = obj["ExpectedLeaveTime"].ToString();
                route.AddRouteTime(time);
            }

            RunOnUiThread(() =>
                {
                    CardStack cs = new CardStack();
                    mCardView.AddStack(cs);
                    MyCard card = new MyCard(route.RouteNumber + "\t" + route.RouteName + "\t" + route.StopId, route.BusRouteTimes);
                    mCardView.AddCard(card);
                });


            mBusRoutes.Add(route);
            SaveBuses(mBusRoutes);
        }

      

        private List<BusRoute> LoadBuses()
        {
            List<BusRoute> buses = new List<BusRoute>();
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = preferences.GetString("buses", string.Empty);
            buses = JsonConvert.DeserializeObject<List<BusRoute>>(json);
            return buses;
        }

        private void SaveBuses(List<BusRoute> busRoutes)
        {
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = JsonConvert.SerializeObject(busRoutes);
            var editor = preferences.Edit();
            editor.PutString("buses", json);

            editor.Commit();
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.transit_activity_actions, menu);
            mAddStopMenu = menu.FindItem(Resource.Id.action_add_stop);
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_add_stop:
                    AddStop();
                    return true;
                case Resource.Id.action_refresh_transit:
                    Refresh();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }


        }

        public override void OnBackPressed()
        {
            mBusRouteListView = null;
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            this.FinishAffinity();

        }

        
    }

    public class AddStopDialogFragment : DialogFragment
    {


        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            LayoutInflater inflater = Activity.LayoutInflater;
            builder.SetView(inflater.Inflate(Resource.Layout.AddStopDialog, null));
            builder.SetPositiveButton("Add", AddStop_Click);
            builder.SetNegativeButton("Cancel", Cancel_Click);

            return builder.Create();
        }


        public void AddStop_Click(object sender, EventArgs e)
        {
            AlertDialog dialog = sender as AlertDialog;
            EditText stopText = dialog.FindViewById<EditText>(Resource.Id.AddStopEditText);
            string stopId = stopText.Text;
            TransitActivity activity = (TransitActivity)Activity;
            activity.GetStop(stopId);
        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
        
    }
  
    
}
