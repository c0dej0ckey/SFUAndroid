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
using Newtonsoft.Json;
using Com.Fima.Cardsui.Objects;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Transit",  ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class TransitActivity : Activity
    {
        private static List<string> sStops = new List<string>(); //{ "53096", "51861", "52998", "52807", "55836", "55738", "61035", "55070", "61787", "55210", "55713", "54993", "55714", "56406", "55441", "55612" };
        private List<Stop> mStops;
        private static string apiKey = "AWkVpR4XnN8gmsf31mku";
        private IMenuItem mAddStopMenu;
        private IMenu mActionBarMenu;
        private CardUI mCardView;
        private bool mRemoving = false;
        /// <summary>
        /// parse data issue
        /// </summary>
        /// <param name="bundle"></param>
        

        protected override void OnCreate(Bundle bundle)
        {
            //load stop ids and times
            //make card view swipeable
            //if swipeable remove the stop

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Transit);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            
            mStops = LoadBuses();
           
            mCardView = this.FindViewById<CardUI>(Resource.Id.BusRouteCardUI);
            mCardView.SetSwipeable(false);

            if (mStops != null)
            {
                foreach (Stop stop in mStops)
                {
                    CardStack cs = new CardStack();
                    mCardView.AddStack(cs);

                    string routeString = string.Empty;
                    foreach (BusRoute route in stop.Routes)
                    {
                        routeString += route.RouteNumber + "\t" + route.RouteName + "\n" + route.BusRouteTimes + "\n";
                    }

                    MyCard card = new MyCard(stop.StopId, routeString);
                    mCardView.AddCard(card);
                    
                }
            }
            else
            {
                mStops = new List<Stop>();
            }

            mCardView.Refresh();


        }




        private void AddStop()
        {

            AddStopDialogFragment fragment = new AddStopDialogFragment();
            fragment.Show(FragmentManager, "Add Stop");
            
        }

        private void Refresh()
        {
            
            mCardView.ClearCards();

            List<string> routes = mStops.Select(b => b.StopId).ToList();
            mStops.Clear();

            foreach(string route in routes)
            {
                GetStop(route);
            }

        }

        private void RemoveStop()
        {
            
            mCardView.ClearCards();
                    

            foreach (Stop stop in mStops)
            {
                CardStack cs = new CardStack();
                mCardView.AddStack(cs);


                string routeString = string.Empty;
                foreach (BusRoute route in stop.Routes)
                {
                    routeString += route.RouteNumber + "\t" + route.RouteName + "\n" + route.BusRouteTimes + "\n";
                }

                ImageCard card = new ImageCard(stop.StopId, routeString, this);
                mCardView.AddCard(card);
            }
            RunOnUiThread(() =>
            {

                mCardView.Refresh();
             });
        }

        public void RemoveRoute(ImageCard card)
        {
            string stopId = card.Title;
            Stop st = mStops.Where(b => b.StopId == stopId).FirstOrDefault();
            mStops.Remove(st);

            
            mCardView.ClearCards();

            foreach (Stop stop in mStops)
            {
                CardStack cs = new CardStack();
                mCardView.AddStack(cs);


                string routeString = string.Empty;
                foreach (BusRoute route in stop.Routes)
                {
                    routeString += route.RouteNumber + "\t" + route.RouteName + "\n" + route.BusRouteTimes + "\n";
                }
                ImageCard c = new ImageCard(stop.StopId, routeString, this);
                mCardView.AddCard(c);
            }

            RunOnUiThread(() =>
            {

            mCardView.Refresh();

            });

        }

        private void CancelRemove()
        {
          
                mCardView.ClearCards();

                foreach (Stop stop in mStops)
                {
                    CardStack cs = new CardStack();
                    mCardView.AddStack(cs);


                    string routeString = string.Empty;
                    foreach (BusRoute route in stop.Routes)
                    {
                        routeString += route.RouteNumber + "\t" + route.RouteName + "\n" + route.BusRouteTimes + "\n";
                    }
                    MyCard card = new MyCard(stop.StopId, routeString);
                    mCardView.AddCard(card);
                }

            RunOnUiThread(() =>
            {
                mCardView.Refresh();

            });
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
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)result.AsyncState;
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

            string stopId = request.RequestUri.OriginalString.Split('/')[6];
            Stop stop = new Stop(stopId);

            foreach(JObject s in stopObject)
            {
                string routeNo = s["RouteNo"].ToString();
                string routeName = s["RouteName"].ToString();

                
                BusRoute route = new BusRoute(routeNo, routeName);
                JArray times = s["Schedules"] as JArray;
                foreach (JObject obj in times)
                {
                    string time = obj["ExpectedLeaveTime"].ToString();
                    route.AddRouteTime(time);
                }

                stop.Routes.Add(route);
            }

            mStops.Add(stop);
            SaveBuses(mStops);

            
            CardStack cs = new CardStack();
            mCardView.AddStack(cs);

            string routeString = string.Empty;
            foreach(BusRoute route in stop.Routes)
            {
                routeString += route.RouteNumber + "\t" + route.RouteName + "\n" + route.BusRouteTimes + "\n";
            }

            MyCard card = new MyCard(stop.StopId, routeString);
            mCardView.AddCard(card);

            RunOnUiThread(() =>
            {

                mCardView.Refresh();
            });


            
        }



        private List<Stop> LoadBuses()
        {
            List<Stop> buses = new List<Stop>();
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = preferences.GetString("buses", string.Empty);
            buses = JsonConvert.DeserializeObject<List<Stop>>(json);
            return buses;
        }

        private void SaveBuses(List<Stop> busRoutes)
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
            mActionBarMenu = menu;
            mAddStopMenu = menu.FindItem(Resource.Id.action_add_stop);
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_add_stop:
                    if(mRemoving)
                    {
                        CancelRemove();
                        mRemoving = false;
                        IMenuItem stopItem = mActionBarMenu.FindItem(Resource.Id.action_remove_stop);
                        stopItem.SetTitle("Edit");
                    }
                    AddStop();
                    return true;
                case Resource.Id.action_refresh_transit:
                    if(mRemoving)
                    {
                        CancelRemove();
                        mRemoving = false;
                        IMenuItem stopItem = mActionBarMenu.FindItem(Resource.Id.action_remove_stop);
                        stopItem.SetTitle("Edit");
                    }
                    Refresh();
                    return true;
                case Resource.Id.action_remove_stop:
                    if (mRemoving)
                    {
                        IMenuItem stopItem = mActionBarMenu.FindItem(Resource.Id.action_remove_stop);
                        stopItem.SetTitle("Edit");
                        CancelRemove();
                        mRemoving = false;
                    }
                    else
                    {
                        item.SetTitle("Done");
                        RemoveStop();
                        mRemoving = true;
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }


        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            this.Finish();

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
