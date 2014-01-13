using Android.Content;
using Android.Views;
using Android.Widget;
using SFUAndroid.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFUAndroid.Adapters
{
    public class BusRouteAdapter : ArrayAdapter<BusRoute>
    {
        private List<BusRoute> mBusRoutes;

        public BusRouteAdapter(Context context, int textViewResourceId, List<BusRoute> busRoutes) : base(context, textViewResourceId)
        {
            this.mBusRoutes = busRoutes;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                //If the box is too small, make it bigger
                LayoutInflater layoutInflator = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = layoutInflator.Inflate(Resource.Layout.BusRoute, null);
            }
            BusRoute busRoute = mBusRoutes.ElementAt(position);
            if (busRoute != null)
            {
                
                TextView tx = view.FindViewById<TextView>(Resource.Id.RouteName);
                tx.Text = busRoute.RouteNumber + " - " + busRoute.RouteName;
                tx = view.FindViewById<TextView>(Resource.Id.RouteTimes);
                tx.Text = busRoute.BusRouteTimes.ToString();

            }
            return view;
        }
    }
}
