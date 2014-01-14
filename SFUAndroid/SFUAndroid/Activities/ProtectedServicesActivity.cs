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
    [Activity(Label = "Protected Services", ParentActivity=typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class ProtectedServicesActivity : Activity
    {
       
      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProtectedServices);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            ListView lv = FindViewById<ListView>(Resource.Id.listview);
           // ArrayAdapter lvA = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);
            PSListViewAdapter lvA = new PSListViewAdapter(this);
            lv.Adapter = lvA;

        }



        private class PSListViewAdapter : BaseAdapter
        {
            private List<string> mData;
            private List<string> mHeaders = new List<string>() { "Sites", "webct", "go sfu", "sfu connect", "my sfu", "coursys" };
            private List<string> mSites = new List<string>() { null, "access to course related information", "get the lastest enrollment information", "stay updated with latest emails", "book and course information", "check grades" };
            private Context mContext;
            private static int HDR_POS1 = 0;
            private static int HDR_POS2 = 6;
            private static Java.Lang.Integer LIST_HEADER = new Java.Lang.Integer(0);
            private static Java.Lang.Integer LIST_ITEM = new Java.Lang.Integer(1);
            private List<string> mURLs = new List<string>() {"https://webct.sfu.ca/webct/urw/ssinboundCAS.siURN:X-WEBCT-VISTA-V1:ae0c1f73-8e3a-65d6-001c-5fd50753fb4e.snWebCT/cobaltMainFrame.dowebct?&allow=sfu,apache&app=WebCT",
"https://sims-prd.sfu.ca/psc/csprd_1/EMPLOYEE/HRMS/c/SA_LEARNER_SERVICES.SSS_STUDENT_CENTER.GBL", "https://connect.sfu.ca/zimbra/mail#1", "http://sakai.sfu.ca/portal/login", "https://courses.cs.sfu.ca/" };


            public PSListViewAdapter(Context context) : base() { this.mContext = context; }



            public override int Count
            {
                get { return mHeaders.Count; }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return position;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            private String getHeader(int position)
            {

                if (position == HDR_POS1 || position == HDR_POS2)
                {
                    return mHeaders[position];
                }

                return null;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                string headerText = getHeader(position);
                if (headerText != null)
                {
                    if (convertView == null || convertView.Tag == LIST_ITEM)
                    {
                        view = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListViewHeader, parent, false);
                        view.Tag = LIST_HEADER;
                    }

                    TextView headerTextView = (TextView)view.FindViewById<TextView>(Resource.Id.lv_list_hdr);
                    headerTextView.Text = headerText;
                    return view;
                }
                view = convertView;
                if (convertView == null || convertView.Tag == LIST_HEADER)
                {
                    view = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListView, parent, false);
                    view.Tag = LIST_ITEM;
                    view.Click += ProtectedService_Selected;
                }



                TextView header = (TextView)view.FindViewById<TextView>(Resource.Id.lv_item_header);
                header.Text = mHeaders[position % mHeaders.Count];

                TextView subText = view.FindViewById<TextView>(Resource.Id.lv_item_subtext);
                subText.Text = mSites[position % mSites.Count];

                View divider = view.FindViewById(Resource.Id.item_separator);
                if (position == HDR_POS2 - 1)
                {
                    divider.Visibility = ViewStates.Invisible;
                }

                return view;
            }

            public void ProtectedService_Selected(object sender, EventArgs e)
            {
                View v = sender as View;
                TextView textView = v.FindViewById<TextView>(Resource.Id.lv_item_header);
                int index = mHeaders.IndexOf(textView.Text);
                index = index - 1;
                string url = mURLs.ElementAt(index);

                var browserActivity = new Intent(mContext, typeof(ProtectedServicesBrowserActivity));
                browserActivity.PutExtra("url", url);
                mContext.StartActivity(browserActivity);

            }

        }
        
    }

  

}