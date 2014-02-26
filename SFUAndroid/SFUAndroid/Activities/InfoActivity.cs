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

            InfoItem reviewItem = new InfoItem("Write a Review", (LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService));
            InfoItem twitterItem = new InfoItem("Twitter", (LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService));
            Header header = new Header((LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), "ChangeLog");
            InfoItem changes = new InfoItem("ChangeLog", (LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService));
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

        public InfoItem(string text, LayoutInflater layoutInflater)
        {
            mText = text;
            mInflater = layoutInflater;

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



            return view;
        }
    }

}