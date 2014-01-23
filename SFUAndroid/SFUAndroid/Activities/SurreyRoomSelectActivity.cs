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
    [Activity(Label = "Select Room", ParentActivity = typeof(MapsActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class SurreyRoomSelectActivity : Activity
    {
        private List<Room> mRooms;
        private string mFloorNumber;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SurreyFloorSelect);
            mRooms = new List<Room>();

            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            Intent intent = base.Intent;

            String floor = intent.GetStringExtra("FloorName");
            string floorNumber = floor.Split(' ')[1];
            this.mFloorNumber = floorNumber;
            

            StreamReader reader = new StreamReader(Assets.Open("surrey-campus-list.csv"));

            ListView roomListView = this.FindViewById<ListView>(Resource.Id.SurreyRoomListView);
            
            roomListView.ItemClick += Room_Selected;

            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = line.Split(',');
                Room room = new Room(data[0], data[1], int.Parse(data[2]), int.Parse(data[3]));
                mRooms.Add(room);
            }

            //filter here
            List<Room> roomsOnFloor = mRooms.Where(r => r.Number.First() == floorNumber[0]).ToList<Room>();

            List<string> roomNames = new List<string>();
            foreach (Room room in roomsOnFloor)
            {
                roomNames.Add(room.Name);
            }

            ArrayAdapter<string> roomAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, roomNames);
            roomListView.Adapter = roomAdapter;
            roomAdapter.NotifyDataSetChanged();


        }

        public void Room_Selected(object sender, EventArgs e)
        {
            //string roomName = e.View.Text;
            Android.Widget.AdapterView.ItemClickEventArgs args = e as Android.Widget.AdapterView.ItemClickEventArgs;
            TextView textView = args.View as TextView;
            string roomName = textView.Text;
            Room room = mRooms.Where(r => r.Name == roomName).FirstOrDefault();
            Intent intent = new Intent(this, typeof(SurreyMapActivity));
            intent.PutExtra("RoomName", room.Name);
            intent.PutExtra("RoomNumber", room.Number);
            intent.PutExtra("FloorNumber", mFloorNumber);
            intent.PutExtra("X", room.X);
            intent.PutExtra("Y", room.Y);
            StartActivity(intent);



        }

    }
}