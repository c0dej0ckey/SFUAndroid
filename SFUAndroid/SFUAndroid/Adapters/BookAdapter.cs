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
    public class BookAdapter : ArrayAdapter<Book>
    {
        private List<Book> mBooks;
        private List<object> mHeaders = new List<object>() { "Books" };
        private static int HDR_POS1 = 0;
        private static int HDR_POS2 = 6;
        private static Java.Lang.Integer LIST_HEADER = new Java.Lang.Integer(0);
        private static Java.Lang.Integer LIST_ITEM = new Java.Lang.Integer(1);

        public BookAdapter(List<Book> books, Context context, int resourceId) : base(context, resourceId)
        {
            mHeaders.Add(books);
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
            var obj = GetHeader(position);
            if(obj is string)
            {
                if (view == null)
                {
                    //If the box is too small, make it bigger
                    LayoutInflater layoutInflator = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                   view =  layoutInflator.Inflate(Resource.Layout.ListViewHeader, null);
                    view.Tag = LIST_HEADER;
                }
                TextView headerTextView = (TextView)view.FindViewById<TextView>(Resource.Id.lv_list_hdr);
                headerTextView.Text = obj.ToString();
                return view;
            }
            view = convertView;
            if (view == null || convertView.Tag == LIST_HEADER)
            {
                //If the box is too small, make it bigger
                LayoutInflater layoutInflator = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = layoutInflator.Inflate(Resource.Layout.Book, null);
                view.Tag = LIST_ITEM;
            }
            Book book = (Book)mHeaders.ElementAt(position);
            if (book != null)
            {
                ImageView iv = view.FindViewById<ImageView>(Resource.Id.button);
                iv.SetImageBitmap(book.Image);
                TextView tx = view.FindViewById<TextView>(Resource.Id.lv_item_header);
                tx.Text = book.Title;
            }
            return view;
        }

        private object GetHeader(int position)
        {
            return mHeaders[position];
        }

    }
}