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
using SFUAndroid.Adapters;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.IO;
using Android.Content.Res;

namespace SFUAndroid.Activities
{
    [Activity(Label = "BurnabyRoomSelect", ParentActivity = typeof(MapsActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class BurnabyRoomSelectActivity : Activity
    {
       


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
             SetContentView(Resource.Layout.BurnabyCampusView);


             Intent intent = this.Intent;

            //string floor = intent.GetStringExtra("FloorName");
            //int x = int.Parse(intent.GetStringExtra("X"));
            //int y = int.Parse(intent.GetStringExtra("Y"));
            

             ImageView img = this.FindViewById<ImageView>(Resource.Id.BurnabyCampusImageView);
             img.SetImageBitmap(DecodeSampledBitmapFromResource(Resources, Resource.Drawable.sfu_campus_mapj, 892, 700));

             AssetManager am = this.Resources.Assets;

             //FileStream buffer = new FileStream(Assets.Open("sfu_campus_map.png"), FileAccess.Read);
            
            // Bitmap bmp = BitmapFactory.DecodeStream(Assets.Open("sfu_campus_map.png"));
           //  BitmapDrawable d = new BitmapDrawable(bmp);
           //  img.SetBackgroundResource(Resource.Drawable.sfu_campus_map);


            //using(var dispose = BitmapFactory.DecodeResource(Resources, Resource.Drawable.sfu_campus_map, o))
            //{
                
            //}


            //var imageHeight = o.OutHeight;
            //var imageWidth = o.OutWidth;
            //var imageType = o.OutMimeType;


        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = (float)options.OutHeight;
            var width = (float)options.OutWidth;
            var inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                inSampleSize = width > height
                                    ? height / reqHeight
                                    : width / reqWidth;
            }

            return (int)inSampleSize;
        }

        public static Bitmap DecodeSampledBitmapFromResource(Android.Content.Res.Resources res, int resId, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true,
            };
            using (var dispose = BitmapFactory.DecodeResource(res, resId, options))
            {
            }

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeResource(res, resId, options);
        }

    }
}