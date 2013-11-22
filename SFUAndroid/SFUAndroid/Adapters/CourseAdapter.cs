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

namespace SFUAndroid.Adapters
{

    /// <summary>
    /// This class creates the custom view for the schedule activity
    /// </summary>
    public class CourseAdapter : ArrayAdapter<Course>
    {
        private List<Course> mCourses;

        public CourseAdapter(Context context, int textViewResourceId, List<Course> courses) : base(context, textViewResourceId)
        {
            
            this.mCourses = courses;
        }

        /// <summary>
        /// Override what to display in the ListView Item
        /// </summary>
        /// <param name="position">position of item</param>
        /// <param name="convertView">view associated with it</param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                //If the box is too small, make it bigger
                LayoutInflater layoutInflator = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = layoutInflator.Inflate(Resource.Layout.Course, null);
            }
            Course course = mCourses.ElementAt(position);
            if(course != null)
            {
                TextView tx = view.FindViewById<TextView>(Resource.Id.CourseName);
                tx.Text = course.ClassName;
                tx = view.FindViewById<TextView>(Resource.Id.CourseSection);
                tx.Text = course.Section;
                tx = view.FindViewById<TextView>(Resource.Id.CourseInstructor);
                tx.Text = course.Instructor;

                ListView classTimeListView = view.FindViewById<ListView>(Resource.Id.ClassTimeListView);

                List<string> classInformation = new List<string>();
                foreach(CourseOffering offering in course.CourseOfferings)
                {
                    classInformation.Add(offering.Location);
                    string cl = offering.StartTime + " - " + offering.EndTime + "  " + offering.Days;
                    classInformation.Add(cl);
                }

                ArrayAdapter<string> courseTimes = new ArrayAdapter<string>(this.Context, Resource.Layout.ListViewAlignRight, classInformation);
                //courseTimes.AddAll(classTimes);
                classTimeListView.Adapter = courseTimes;
                courseTimes.NotifyDataSetChanged();

            }
            return view;
        }


    }
}