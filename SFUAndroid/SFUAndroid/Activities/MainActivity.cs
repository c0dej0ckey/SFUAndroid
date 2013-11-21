﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SFUAndroid.Activities
{
    [Activity(Label = "SFUAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.ProtectedServicesButton);
            button.Click += NavigateToProtectedServices;

            Button loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += NavigateToLoginView;

            Button coursesButton = FindViewById<Button>(Resource.Id.ScheduleButton);
            coursesButton.Click += NavigateToCoursesView;

            Button booksButton = FindViewById<Button>(Resource.Id.BooksButton);
            booksButton.Click += NavigateToBooksView;

            Button transitButton = FindViewById<Button>(Resource.Id.TransitButton);
            transitButton.Click += NavigateToTransitView;


        }

        void NavigateToProtectedServices(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ProtectedServicesActivity));
            StartActivity(intent);
        }

        void NavigateToLoginView(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }

        void NavigateToCoursesView(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ScheduleActivity));
            StartActivity(intent);
        }

        void NavigateToBooksView(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(BooksActivity));
            StartActivity(intent);
        }

        void NavigateToTransitView(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TransitActivity));
            StartActivity(intent);
        }

    }
}

