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

namespace SFUAndroid
{
    public class CourseAdapter : ArrayAdapter<Course>
    {
        private List<Course> mCourses;

        public CourseAdapter(Context context, int textViewResourceId, List<Course> courses) : base(context, textViewResourceId)
        {
            
            this.mCourses = courses;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if(v == null)
            {
                LayoutInflater vi = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                v = vi.Inflate(Resource.Layout.Course, null);
            }
            Course course = mCourses.ElementAt(position);
            if(course != null)
            {
                TextView tx = v.FindViewById<TextView>(Resource.Id.CourseName);
                tx.Text = course.ClassName;
            }
            return v;
            //return base.GetView(position, convertView, parent);
        }


    }
}