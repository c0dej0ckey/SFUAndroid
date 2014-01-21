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
using UK.CO.Senab.Photoview;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Surrey", ParentActivity = typeof(SurreyRoomSelectActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class SurreyMapActivity : Activity
    {
        private PhotoViewAttacher mAttacher;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SurreyCampusView);

            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            string roomName = this.Intent.GetStringExtra("RoomName");
            string roomNumber = this.Intent.GetStringExtra("RoomNumber");
            string floorNumber = this.Intent.GetStringExtra("FloorNumber");
            int x = this.Intent.GetIntExtra("X", 0);
            int y = this.Intent.GetIntExtra("Y", 0);

            ImageView photoView = this.FindViewById<ImageView>(Resource.Id.SurreyCampusImageView);

            Bitmap bmp = null;
            switch(floorNumber)
            {
                case "3":
                    bmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Campus_Guide_Galleria_3);
                    break;
                case "4":
                    bmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Campus_Guide_Galleria_4);
                    break;
                case "5":
                    bmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Campus_Guide_Galleria_5);
                    break;
                case "2":
                    bmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Campus_Guide_Podium_2);
                    break;
            }


            photoView.SetImageBitmap(bmp);
            mAttacher = new PhotoViewAttacher(photoView);

        }
    }
}