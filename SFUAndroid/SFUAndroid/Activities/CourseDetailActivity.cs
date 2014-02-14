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

namespace SFUAndroid.Activities
{
    [Activity(Label = "", Theme = "@android:style/Theme.Holo.Light", ParentActivity=typeof(ScheduleActivity))]
    public class CourseDetailActivity : Activity
    {
        private List<Item> mInformation;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CourseDetail);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            string x = Intent.GetStringExtra("ExamStartTime");
            string y = Intent.GetStringExtra("ExamEndTime");
            string z = Intent.GetStringExtra("ExamDate");

            

            mInformation = new List<Item>();
            Header header = new Header(this.LayoutInflater, "Exam");
            ExamDetail ex = new ExamDetail("asdasda", "asdasd", "asdasd", (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService));
            mInformation.Add(header);
            mInformation.Add(ex);
            Header gradesHeader = new Header(this.LayoutInflater, "Grades");
            mInformation.Add(gradesHeader);
            ListView courseDetailListView = this.FindViewById<ListView>(Resource.Id.CourseDetailListView);
            CourseDetailAdapter adapter = new CourseDetailAdapter(this, mInformation);
            courseDetailListView.Adapter = adapter;
            adapter.NotifyDataSetChanged();



            // Create your application here
        }
    }

    public class CourseDetailAdapter : ArrayAdapter<Item>
    {
        private LayoutInflater mInflater;



        public CourseDetailAdapter(Context context, List<Item> items) : base(context, 0, items) { mInflater = LayoutInflater.From(context); }

        public override int ViewTypeCount
        {
            get
            {
                return Enum.GetNames(typeof(RowType)).Length;
               // return 1;
            }
        }

        public override int GetItemViewType(int position)
        {
            return GetItem(position).GetViewType();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return GetItem(position).GetView(mInflater, convertView, parent);
        }
    }

}