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
     [Activity(Label = "Maps")]
    public class MapsActivity : Activity
    {
         //private List<string> mSurreyFloors = new List<string>() { "Galleria 3", "Galleria 4" };
         

         protected override void OnCreate(Bundle bundle)
         {
             base.OnCreate(bundle);
             SetContentView(Resource.Layout.Maps);

             Spinner surreyMapSpinner = FindViewById<Spinner>(Resource.Id.SurreyFloorSpinner);
             surreyMapSpinner.ItemSelected += NavigateToSurreyRoomView;

             Spinner burnabyMapSpinner = FindViewById<Spinner>(Resource.Id.BurnabyBuildingSpinner);
             burnabyMapSpinner.ItemSelected += NavigateToBurnabyRoomView;

         }
         private void NavigateToBurnabyRoomView(object sender, EventArgs e)
         {
             Spinner spinner = sender as Spinner;
             if (spinner.SelectedItem.ToString() != "Burnaby")
             {
                 string floor = spinner.SelectedItem.ToString();

                 Intent intent = new Intent(this, typeof(BurnabyRoomSelectActivity));
                 intent.PutExtra("FloorName", floor);
                 StartActivity(intent);
             }
         }

         private void  NavigateToSurreyRoomView(object sender, EventArgs e)
         {
             Spinner spinner = sender as Spinner;
             if (spinner.SelectedItem.ToString() != "Surrey")
             {
                 string floor = spinner.SelectedItem.ToString();

                 Intent intent = new Intent(this, typeof(SurreyRoomSelectActivity));
                 intent.PutExtra("FloorName", floor);
                 StartActivity(intent);
             }
         }
    }
}