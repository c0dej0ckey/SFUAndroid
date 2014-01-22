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
}