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
    public class Selection
    {
        private string mTitle;
        private Bitmap mBitmap;

        public Selection(string title, Bitmap bitmap)
        {
            this.mTitle = title;
            this.mBitmap = bitmap;
        }

        public Bitmap Bitmap
        {
            get { return this.mBitmap; }
        }

        public string Title
        {
            get { return this.mTitle; }
        }
    }
}