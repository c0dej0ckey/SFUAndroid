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
    [Activity(Label = "Protected Services", ParentActivity=typeof(MainActivity))]
    public class ProtectedServicesActivity : Activity
    {
        private List<string> items = new List<string>() {"webct", "go sfu", "sfu connect", "my sfu", "coursys"};
        private List<string> mURLs = new List<string>() {"https://webct.sfu.ca/webct/urw/ssinboundCAS.siURN:X-WEBCT-VISTA-V1:ae0c1f73-8e3a-65d6-001c-5fd50753fb4e.snWebCT/cobaltMainFrame.dowebct?&allow=sfu,apache&app=WebCT",
"https://sims-prd.sfu.ca/psc/csprd_1/EMPLOYEE/HRMS/c/SA_LEARNER_SERVICES.SSS_STUDENT_CENTER.GBL", "https://connect.sfu.ca/zimbra/mail#1", "http://sakai.sfu.ca/portal/login", "https://courses.cs.sfu.ca/" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProtectedServices);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            ListView lv = FindViewById<ListView>(Resource.Id.listview);
            ArrayAdapter lvA = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);
            lv.Adapter = lvA;
            lv.ItemClick += item_Selected;

        }

        private void item_Selected(object sender, AdapterView.ItemClickEventArgs e)
        {
                ListView lv = FindViewById<ListView>(Resource.Id.listview);
                string t = (string)lv.GetItemAtPosition(e.Position);
                int index = items.IndexOf(t);
                string url = mURLs.ElementAt(index);

            var browserActivity = new Intent(this, typeof (ProtectedServicesBrowserActivity));
            browserActivity.PutExtra("url", url);
            StartActivity(browserActivity);

        }



        
    }
}