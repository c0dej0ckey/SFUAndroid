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
using Com.Fima.Cardsui.Views;
using Com.Fima.Cardsui.Objects;
using SFUAndroid.Entities;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Change Log", ParentActivity = typeof(InfoActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class ChangeLogActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.ChangeLog);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            CardUI changeView = this.FindViewById<CardUI>(Resource.Id.ChangeLogView);

            CardStack cs = new CardStack();
            changeView.AddStack(cs);

            MyCard change1000 = new MyCard("1.0.0.0", "first deployment");
            MyCard change1010 = new MyCard("1.0.1.0", "updated transit section to allow addition/removal of stops");
            MyCard change1011 = new MyCard("1.0.1.1", "fixed schedule bugs where courses were appearing more than once");
            MyCard change1012 = new MyCard("1.0.1.2", "fixed bug in transit where stops and their information weren't appearing");
            MyCard change1013 = new MyCard("1.0.1.3", "fixed bug where removed stops were still appearing on refresh of transit stops");
            MyCard change1100 = new MyCard("1.1.0.0", "added extra information menu when click on overflow on course in schedule view");
            MyCard change1200 = new MyCard("1.2.0.0", "added information page, including twitter, ratings and changelog");

            changeView.AddCard(change1200);

            changeView.AddCard(change1100);

            changeView.AddCard(change1013);
            
            
            changeView.AddCard(change1012);

            changeView.AddCard(change1011);

            changeView.AddCard(change1010);

            changeView.AddCard(change1000);

            changeView.Refresh();

            base.OnCreate(bundle);

            // Create your application here
        }
    }
}