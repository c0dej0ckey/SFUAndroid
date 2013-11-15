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

namespace SFUAndroid.Activities
{
    [Activity(Label = "Protected Services")]
    public class ProtectedServicesActivity : Activity
    {
        private string[] items = {"webct", "go sfu", "sfu connect", "my sfu", "coursys"};

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProtectedServices);

            ListView lv = FindViewById<ListView>(Resource.Id.listview);
            ArrayAdapter lvA = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);
            lv.SetAdapter(lvA);
            lv.ItemClick += item_Selected;

        }

        private void item_Selected(object sender, AdapterView.ItemClickEventArgs e)
        {
                ListView lv = FindViewById<ListView>(Resource.Id.listview);
                string t = (string)lv.GetItemAtPosition(e.Position);

            var browserActivity = new Intent(this, typeof (ProtectedServicesBrowserActivity));
            browserActivity.PutExtra("site", t.ToString());
            StartActivity(browserActivity);
            // Android.Widget.Toast.MakeText(this, t.ToString(), Android.Widget.ToastLength.Short).Show();

        }



        
    }
}