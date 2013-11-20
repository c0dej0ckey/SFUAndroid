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

        public BookAdapter(List<Book> books, Context context, int resourceId) : base(context, resourceId)
        {
            mBooks = books;
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
                view = layoutInflator.Inflate(Resource.Layout.Book, null);
            }
            Book book = mBooks.ElementAt(position);
            if (book != null)
            {
                ImageView iv = view.FindViewById<ImageView>(Resource.Id.BookImage);
                iv.SetImageBitmap(book.Image);
                TextView tx = view.FindViewById<TextView>(Resource.Id.BookName);
                tx.Text = book.Title;
            }
            return view;
        }

    }
}