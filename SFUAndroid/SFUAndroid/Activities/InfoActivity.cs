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
using SFUAndroid.Entities;
using Com.Fima.Cardsui.Objects;

namespace SFUAndroid.Activities
{
    [Activity(Label = "App Info", ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class InfoActivity : Activity
    {
        private List<Item> mItems;

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.Info);

            ListView infoListView = this.FindViewById<ListView>(Resource.Id.InfoListView);

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            //CardUI changesUI = this.FindViewById<CardUI>(Resource.Id.ChangesCardsView);
            //changesUI.SetSwipeable(false);

            mItems = new List<Item>();

            InfoItem reviewItem = new InfoItem("Write a Review", (LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), this);
            InfoItem twitterItem = new InfoItem("Twitter", (LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), this);
            Header header = new Header((LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), "ChangeLog");
            InfoItem changes = new InfoItem("ChangeLog", (LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), this);
            mItems.Add(reviewItem);
            mItems.Add(twitterItem);
            
            mItems.Add(changes);
            InfoAdapter adapter = new InfoAdapter(this, mItems);
            infoListView.Adapter = adapter;
            adapter.NotifyDataSetChanged();

            //MyCard card = new MyCard("Changes", "asdkjhaskdjha");
            //CardStack cs = new CardStack();
            //changesUI.AddStack(cs);
            //changesUI.AddCard(card);
            //changesUI.Refresh();


            base.OnCreate(bundle);




        }

        public void GetOption(string text)
        {
            if (text == "Write a Review")
            {
                try
                {
                    StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.xamarin.sfuandroid")));
                }
                catch
                {
                    StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=com.xamarin.sfuandroid")));
                }
            }
            else if (text == "Twitter")
            {
                StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://twitter.com/c0dej0ckey")));
            }
            else
            {
                Intent intent = new Intent(this, typeof(ChangeLogActivity));
                StartActivity(intent);
            }
        }

    }

    public class InfoAdapter : ArrayAdapter<Item>
    {
        private LayoutInflater mInflater;



        public InfoAdapter(Context context, List<Item> items) : base(context, 0, items) { mInflater = LayoutInflater.From(context); }

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


    public class InfoItem : Item
    {
        private string mText;
        private LayoutInflater mInflater;
        private InfoActivity mInfoActivity;

        public InfoItem(string text, LayoutInflater layoutInflater, InfoActivity infoActivity)
        {
            mText = text;
            mInflater = layoutInflater;
            mInfoActivity = infoActivity;
        }

        public int GetViewType()
        {
            return (int)RowType.LIST_ITEM;
        }

        public View GetView(LayoutInflater inflater, View convertView, ViewGroup parent)
        {
            View view = null;
            if (convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.ListView, parent, false);
            }
            else
            {
                view = convertView;
            }

            TextView tx = view.FindViewById<TextView>(Resource.Id.lv_item_header);
            tx.Text = mText;

            view.Click += view_Click;

            ViewGroup.LayoutParams par = (ViewGroup.LayoutParams)view.LayoutParameters;
            par.Height = 80;
            view.LayoutParameters = par;

            return view;
        }

        void view_Click(object sender, EventArgs e)
        {
            View view = sender as View;
            TextView txtView = view.FindViewById<TextView>(Resource.Id.lv_item_header);
            mInfoActivity.GetOption(txtView.Text);
        }
    }

}