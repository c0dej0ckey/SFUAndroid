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
using Android.Util;
using Android.GoogleMaps;
using UK.CO.Senab.Photoview;

namespace SFUAndroid.Activities
{
    [Activity(Label = "BurnabyRoomSelect", ParentActivity = typeof(MapsActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class BurnabyRoomSelectActivity : Activity
    {

        //protected override bool IsRouteDisplayed
        //{
        //    get { return false; }
        //}

        private PhotoViewAttacher mAttacher;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
             SetContentView(Resource.Layout.BurnabyCampusView);
             this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

             Intent intent = this.Intent;

            string floor = intent.GetStringExtra("RoomName");
            int x = intent.GetIntExtra("X", 0) ;
            int y = intent.GetIntExtra("Y", 0);

            //map is scaled by 1/2.
            x = x / 2;
            y = y / 2;


             
             ImageView photoView = this.FindViewById<ImageView>(Resource.Id.BurnabyCampusImageView);


             
             var bmp = DecodeSampledBitmapFromResource(Resources, Resource.Drawable.sfu_campus_mapscaled, 1792, 955);
             Bitmap mutablebmp = bmp.Copy(bmp.GetConfig(), true);
             int[,] pixels = new int[bmp.Width, bmp.Height];
             Bitmap pin = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.pin);
             //pin.GetPixels(pixels, 0, pin.Width, 0, 0, pin.Width, pin.Height);
             for (int i = 0; i < 40; i++)
             {
                 for (int j = 0; j < 68; j++)
                 {
                     int pixelColor = pin.GetPixel(i, j);
                     //Color c = this.Resources.GetColor(pixelColor);
                     Color c = new Color(pixelColor);
                     if (c.A == 0 && c.B == 0 && c.R == 0 && c.G == 0)
                     {
                         mutablebmp.SetPixel(x + i - 20, y + j - 68, new Color(bmp.GetPixel(x + i - 20, y + j - 68)));
                     }
                     else
                     {
                         mutablebmp.SetPixel(x + i - 20, y + j - 68, c);
                     }
                 }

             }


             photoView.SetImageBitmap(mutablebmp);
             mAttacher = new PhotoViewAttacher(photoView);



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


    public class MapOverlay : Overlay
    {

        GeoPoint _gp;
        private Bitmap _bitmap;
        private Resources resource;

        public MapOverlay(GeoPoint gp, Bitmap bitmap, Resources resources)
        {
            _gp = gp;
            this._bitmap = bitmap;
            this.resource = resources;
        }



        public override void Draw(Android.Graphics.Canvas canvas,
               MapView mapView, bool shadow)
        {
            base.Draw(canvas, mapView, shadow);

            //var paint = new Paint();
            //paint.AntiAlias = true;
            //paint.Color = Color.Transparent;
            //paint.Alpha = 50;





            var pt = mapView.Projection.ToPixels(_gp, null);
            float distance =
                    mapView.Projection.MetersToEquatorPixels(200);

           // canvas.DrawBitmap(_bitmap, new Rect(0,0, _bitmap.Width, _bitmap.Height);


          //  canvas.DrawRect(pt.X, pt.Y, pt.X + distance, pt.Y +
               //     distance, paint);
        }
    }

    public class MyView : View
    {
        public MyView(Context context) : base(context) { }

        public MyView(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public MyView(Context context, IAttributeSet attrs, int defstyle) : base(context, attrs, defstyle) { }

        public MyView(IntPtr ptr, JniHandleOwnership own) : base(ptr, own) { }

        protected override void OnDraw(Canvas canvas)
        {
            //canvas.Set
            base.OnDraw(canvas);
        }
    }
}