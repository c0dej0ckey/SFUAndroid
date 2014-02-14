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
using Com.Fima.Cardsui.Objects;
using SFUAndroid.Activities;

namespace SFUAndroid.Entities
{
    public class MyCard : RecyclableCard
    {
        public MyCard(string title) : base(title) { }

        public MyCard(string title, string description) : base(title, description) { }

        protected override int CardLayoutId
        {
            get { return Resource.Layout.card_ex; }
        }

        protected override void ApplyTo(View p0)
        {
            ((TextView)p0.FindViewById<TextView>(Resource.Id.title)).Text = Title;
            ((TextView)p0.FindViewById<TextView>(Resource.Id.description)).Text = Desc;
        }
    }

    public class ImageCard : RecyclableCard
    {
        private TransitActivity transitActivity;

        public ImageCard(string title, string description, TransitActivity transitActivity) : base(title, description) { this.transitActivity = transitActivity; }

        protected override int CardLayoutId
        {
            get { return Resource.Layout.card_picture; }
        }

        protected override void ApplyTo(View p0)
        {
            ((TextView)p0.FindViewById<TextView>(Resource.Id.title)).Text = Title;
            ((TextView)p0.FindViewById<TextView>(Resource.Id.description)).Text = Desc;
            Button button = ((Button)p0.FindViewById<Button>(Resource.Id.removeStopButton));
            
            //img.SetImageResource(Resource.Drawable.ic_action_cancel);
            button.Click += img_Click;
        }

        void img_Click(object sender, EventArgs e)
        {
            transitActivity.RemoveRoute(this);
        }

    }

    public class ClickableCard : RecyclableCard
    {
        private ScheduleActivity scheduleActivity;

        public ClickableCard(string title, string description, string color, string titleColor, Java.Lang.Boolean hasOverflow, Java.Lang.Boolean isClickable, ScheduleActivity scheduleActivity) :
            base(title, description, color, titleColor, hasOverflow, isClickable) { this.scheduleActivity = scheduleActivity; }

        protected override int CardLayoutId
        {
            get { return Resource.Layout.card_play; }
        }

        protected override void ApplyTo(View p0)
        {
            ((TextView)p0.FindViewById(Resource.Id.title)).Text = TitlePlay;
            ((TextView)p0.FindViewById(Resource.Id.description)).Text = Description;

            ImageView overflow = p0.FindViewById<ImageView>(Resource.Id.overflow);

            if (HasOverflow.BooleanValue() == true)
                ((ImageView)p0.FindViewById(Resource.Id.overflow)).Visibility = ViewStates.Visible;
            else
                ((ImageView)p0.FindViewById(Resource.Id.overflow)).Visibility = ViewStates.Gone;

            overflow.Click += overflow_Click;

        }

        public void overflow_Click(object sender, EventArgs e)
        {
            scheduleActivity.OpenExtraInfo(this);
        }



    }

    


}