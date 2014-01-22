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
using Android.Graphics;

namespace SFUAndroid.Entities
{
    public class Book : Item
    {
        private string mClassNumber;
        private string mClassName;
        private string mTitle;
        private string mAuthor;
        private string mStatus;
        private string mIsbn;
        private float mNewPrice;
        private float mUsedPrice;
        private Bitmap mImage;
        private LayoutInflater mInflater;
        

        public Book(LayoutInflater inflater, string className, string classNumber, string title, string author, string status, string isbn, float newPrice, float usedPrice)
        {
            this.ClassName = className;
            this.ClassNumber = classNumber;
            this.Title = title;
            this.Author = author;
            this.Status = status;
            this.Isbn = isbn;
            this.NewPrice = newPrice;
            this.UsedPrice = usedPrice;
            this.mInflater = inflater;
        }

        public string ClassName
        {
            get { return this.mClassName; }
            set { this.mClassName = value; }
        }

        public string ClassNumber
        {
            get { return this.mClassNumber; }
            set { this.mClassNumber = value; }
        }

        public string Title
        {
            get { return this.mTitle; }
            set { this.mTitle = value; }
        }

        public string Author
        {
            get { return this.mAuthor; }
            set { this.mAuthor = value; }
        }

        public string Status
        {
            get { return this.mStatus; }
            set { this.mStatus = value; }
        }

        public string Isbn
        {
            get { return this.mIsbn; }
            set { this.mIsbn = value; }
        }

        public float NewPrice
        {
            get { return this.mNewPrice; }
            set { this.mNewPrice = value; }

        }

        public float UsedPrice
        {
            get { return this.mUsedPrice; }
            set { this.mUsedPrice = value; }
        }

        public Bitmap Image
        {
            get { return this.mImage; }
            set { this.mImage = value; }
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
                view = inflater.Inflate(Resource.Layout.Book, parent, false);
            }
            else
            {
                view = convertView;
            }
            if (Image != null)
            {
                ImageView iv = view.FindViewById<ImageView>(Resource.Id.button);
                //Bitmap b = BitmapFactory.DecodeResource(
                iv.SetImageBitmap(Image);
            }
            TextView tx = view.FindViewById<TextView>(Resource.Id.lv_item_header);
            tx.Text = Title;
            TextView tx2 = view.FindViewById<TextView>(Resource.Id.lv_item_subtext);
            tx2.Text = Author + " New Price: " + NewPrice + " Used Price: " + UsedPrice + " - " + Isbn;
            return view;
        }
    }

    public interface Item
    {
         int GetViewType();
         View GetView(LayoutInflater inflater, View convertView, ViewGroup parent);
    }

    public enum RowType
    {
        LIST_ITEM = 0,
        HEADER_ITEM = 1
    }

    public class BookArrayAdapter : ArrayAdapter<Item>
    {
        private LayoutInflater mInflater;

        

        public BookArrayAdapter(Context context, List<Item> items) : base(context, 0, items) { mInflater = LayoutInflater.From(context); }

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

    public class Header : Item
    {
        private string mName;
        private LayoutInflater mInflater;

        public Header(LayoutInflater inflater, string name)
        {
            this.mName = name;
            this.mInflater = inflater;
        }




        public int GetViewType()
        {
            return (int)RowType.HEADER_ITEM;
        }

        public View GetView(LayoutInflater inflater, View convertView, ViewGroup parent)
        {
            View view = null;
            if (convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.ListViewHeader, parent, false);
            }
            else
            {
                view = convertView;
            }
            TextView text = (TextView)view.FindViewById(Resource.Id.lv_list_hdr);
            text.Text = mName;
            return view;
        }
    }


}