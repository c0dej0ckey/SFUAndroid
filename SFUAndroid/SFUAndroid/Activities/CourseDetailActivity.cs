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
    [Activity(Label = "Extra Information", Theme = "@android:style/Theme.Holo.Light", ParentActivity=typeof(ScheduleActivity))]
    public class CourseDetailActivity : Activity
    {
        private List<Item> mInformation;
        private Course mCourse;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            

            

            SetContentView(Resource.Layout.CourseDetail);

            
            


            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);



            string courseName = Intent.GetStringExtra("CourseName");
            this.Window.SetTitle(courseName);
            string section = Intent.GetStringExtra("Section");
            string credits = Intent.GetStringExtra("Credits");
            string status = Intent.GetStringExtra("Status");
            string instructor = Intent.GetStringExtra("Instructor");
            string type = Intent.GetStringExtra("Type");
            int offeringCount = Intent.GetIntExtra("OfferingCount", 0);


            mCourse = new Course(courseName, section, credits, status, instructor, type);

            for (int i = 0; i < offeringCount; i++)
            {

                string startTime = Intent.GetStringExtra("StartTime" + i);
                string endTime = Intent.GetStringExtra("EndTime" + i);
                string location = Intent.GetStringExtra("Location" + i);
                string days = Intent.GetStringExtra("Days" + i);
                string date = Intent.GetStringExtra("Date" + i);
                CourseOffering offering = new CourseOffering(startTime, endTime, location, days, date);
                mCourse.AddCourseOffering(offering);
            }

            string examStartTime = Intent.GetStringExtra("ExamStartTime");
            string examEndTime = Intent.GetStringExtra("ExamEndTime");
            string examDate = Intent.GetStringExtra("ExamDate");

            Exam exam = new Exam(examStartTime, examEndTime, DateTime.Now);
            mCourse.Exam = exam;
            

            mInformation = new List<Item>();

            CourseDetail detail = new CourseDetail("CMPT 354", "D200", "3.00", "Enrolled", "John Edgar", "Lecture");
            mInformation.Add(detail);

            Header time = new Header(this.LayoutInflater, "Time & Location");
            mInformation.Add(time);

            CourseOfferingDetail offeringDetail = new CourseOfferingDetail("1230", "120", "SUR3340 \n 12:30 - 1:20", "Mon,Wed", "10-1-2014", (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService), this.BaseContext);
            mInformation.Add(offeringDetail);

            CourseOfferingDetail offeringDetail2 = new CourseOfferingDetail("530", "820", "SUR5560 \n 5:30 - 8:20", "Mon,Wed,Fri", "10-1-2014", (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService), this.BaseContext);
            mInformation.Add(offeringDetail2);

            Header header = new Header(this.LayoutInflater, "Exam");
            ExamDetail ex = new ExamDetail("3:30", "6:30", "6/1/2014", (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService));
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

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Window.SetTitle(mCourse.ClassName);
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