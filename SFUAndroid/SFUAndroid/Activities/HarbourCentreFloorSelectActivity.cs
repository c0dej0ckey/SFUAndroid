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
    [Activity(Label = "HarbourCentreFloorSelect")]
    public class HarbourCentreFloorSelectActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //SetContentView(Resource.Layout.HarbourCentreFloorSelect);
        }
    }
}