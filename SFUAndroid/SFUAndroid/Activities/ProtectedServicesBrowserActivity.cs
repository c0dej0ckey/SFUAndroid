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
using Android.Webkit;

namespace SFUAndroid.Activities
{
    [Activity(Label = "")]
    public class ProtectedServicesBrowserActivity : Activity
    {
        private string mSite;

        protected override void OnCreate(Bundle bundle)
        {

            mSite = this.Intent.GetStringExtra("site");
            WebView view = FindViewById<WebView>(Resource.Id.ps_webView);
            //view.Url = "https://courses.cs.sfu.ca";
            //view.

            base.OnCreate(bundle);

            // Create your application here
        }
    }
}