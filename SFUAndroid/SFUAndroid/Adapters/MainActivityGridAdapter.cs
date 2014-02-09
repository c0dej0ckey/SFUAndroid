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
using Android.Graphics.Drawables;
using SFUAndroid.Activities;

namespace SFUAndroid.Adapters
{
    public class MainActivityGridAdapter : ArrayAdapter<Selection>
    {
        private List<Selection> mData = new List<Selection>();

        public MainActivityGridAdapter(Context context, int layoutResourceId, List<Selection> data) : base(context, layoutResourceId)
        {
            this.mData = data;   
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                LayoutInflater layoutInflator = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = layoutInflator.Inflate(Resource.Layout.MainActivityOption, null);
            }
            Selection selection = mData.ElementAt(position);
            if (selection != null)
            {
                Button b = view.FindViewById<Button>(Resource.Id.MainSelectionButton);
                b.Click += Selection_ItemClick;
               // b.Text = selection.Title;
                b.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, new BitmapDrawable(this.Context.Resources,selection.Bitmap), null, null);

            }
            
            return view;
            //return base.GetView(position, convertView, parent);
        }
        
       
        public void Selection_ItemClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button.Text.Equals("Schedule"))
            {
                Intent intent = new Intent(this.Context, typeof(ScheduleActivity));
                this.Context.StartActivity(intent);
            }
            else if (button.Text.Equals("Maps"))
            {
                Intent intent = new Intent(this.Context, typeof(MapsActivity));
                this.Context.StartActivity(intent);
            }
            else if (button.Text.Equals("Protected \n Services"))
            {
                Intent intent = new Intent(this.Context, typeof(ProtectedServicesActivity));
                this.Context.StartActivity(intent);
            }
            else if (button.Text.Equals("Books"))
            {
                Intent intent = new Intent(this.Context, typeof(BooksActivity));
                this.Context.StartActivity(intent);
            }
            else //Transit
            {
                Intent intent = new Intent(this.Context, typeof(TransitActivity));
                this.Context.StartActivity(intent);
            }
        }

    }

    
}