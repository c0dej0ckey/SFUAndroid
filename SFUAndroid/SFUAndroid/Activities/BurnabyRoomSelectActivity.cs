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

namespace SFUAndroid.Activities
{
    [Activity(Label = "BurnabyRoomSelect")]
    public class BurnabyRoomSelectActivity : Activity
    {
        private List<Room> mRooms;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // SetContentView(Resource.Layout.SurreyFloorSelect);
            mRooms = new List<Room>();

            Intent intent = base.Intent;

            String floor = intent.GetStringExtra("FloorName");

            // var path = @"Assets/Maps/surrey-campus-list.csv";
            //StreamResourceInfo res = System.Windows.Application.GetResourceStream(new Uri(path, UriKind.Relative));

            StreamReader reader = new StreamReader(Assets.Open("burnaby-campus-list.csv"));

            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = line.Split(',');
                Room room = new Room(data[0], data[1], int.Parse(data[2]), int.Parse(data[3]));
                mRooms.Add(room);
            }
        }
    }
}