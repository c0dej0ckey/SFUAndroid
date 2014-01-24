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
     [Activity(Label = "Maps", ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class MapsActivity : Activity
    {
         private List<Room> mRooms;
         

         protected override void OnCreate(Bundle bundle)
         {
             base.OnCreate(bundle);
             SetContentView(Resource.Layout.Maps);

             ActionBar actionBar = this.ActionBar;
             actionBar.SetDisplayHomeAsUpEnabled(true);

             mRooms = new List<Room>();

             Spinner surreyMapSpinner = FindViewById<Spinner>(Resource.Id.SurreyFloorSpinner);
             surreyMapSpinner.ItemSelected += NavigateToSurreyRoomView;

             Spinner burnabyMapSpinner = FindViewById<Spinner>(Resource.Id.BurnabyBuildingSpinner);
             burnabyMapSpinner.ItemSelected += NavigateToBurnabyRoomView;

             StreamReader reader = new StreamReader(Assets.Open("burnaby-campus-list.csv"));
             string line = null;
             while ((line = reader.ReadLine()) != null)
             {
                 string[] data = line.Split(',');
                 Room room = new Room(data[0], data[1], int.Parse(data[2]), int.Parse(data[3]));
                 mRooms.Add(room);
             }

             List<string> roomNames = new List<string>();
             roomNames.Add("Burnaby");
             foreach (Room room in mRooms)
             {
                 roomNames.Add(room.Name);
             }

             ArrayAdapter<string> burnabySpinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, roomNames);
             burnabyMapSpinner.Adapter = burnabySpinnerAdapter;
             burnabySpinnerAdapter.NotifyDataSetChanged();

         }
         private void NavigateToBurnabyRoomView(object sender, EventArgs e)
         {
             Spinner spinner = sender as Spinner;
             if (spinner.SelectedItem.ToString() != "Burnaby")
             {
                 string building = spinner.SelectedItem.ToString();
                 Room room = mRooms.Where(r => r.Name == building).FirstOrDefault();

                 Intent intent = new Intent(this, typeof(BurnabyRoomSelectActivity));
                 intent.PutExtra("RoomName", room.Name);
                 intent.PutExtra("X", room.X);
                 intent.PutExtra("Y", room.Y);
                 
                 StartActivity(intent);
             }
         }

         private void  NavigateToSurreyRoomView(object sender, EventArgs e)
         {
             Spinner spinner = sender as Spinner;
             if (spinner.SelectedItem.ToString() != "Surrey")
             {
                 string floor = spinner.SelectedItem.ToString();

                 Intent intent = new Intent(this, typeof(SurreyRoomSelectActivity));
                 intent.PutExtra("FloorName", floor);
                 StartActivity(intent);
             }
         }


         public override void OnBackPressed()
         {
             Intent intent = new Intent(this, typeof(MainActivity));
             StartActivity(intent);
             Finish();

             //base.OnBackPressed();
         }

    }
}