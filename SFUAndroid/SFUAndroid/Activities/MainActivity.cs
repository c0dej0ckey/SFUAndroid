using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using HtmlAgilityPack;
using SFUAndroid.Services;
using System.Net;
using System.IO;
using System.Text;
using Android.Graphics;
using SFUAndroid.Adapters;
using System.Collections.Generic;
using SFUAndroid.Entities;

namespace SFUAndroid.Activities
{
    [Activity(Label = "SFU", MainLauncher = true, Icon = "@drawable/sfulogo")]
    public class MainActivity : Activity
    {
        private string mKey;
        private IMenu mActionBarMenu;
        private MainActivityGridAdapter mGridAdapter;

        protected override void OnCreate(Bundle bundle)
        {
           
            base.OnCreate(bundle);

            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string computingId = preferences.GetString("ComputingId", string.Empty);
            string password = preferences.GetString("Password", string.Empty);

            

            if(!computingId.Equals(string.Empty) && !password.Equals(string.Empty))
            {
                
                TryLoginUser();
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            List<Selection> menuSelections = new List<Selection>();


            Bitmap coursesIcon = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.courses);
            Selection sel = new Selection("Schedule", coursesIcon);
            menuSelections.Add(sel);

            Bitmap protectedServicesIcon = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.proserv);
            sel = new Selection("Protected \n Services", protectedServicesIcon);
            menuSelections.Add(sel);

            Bitmap transitIcon = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.transit);
            sel = new Selection("Transit", transitIcon);
            menuSelections.Add(sel);

            Bitmap mapsIcon = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.maps);
            sel = new Selection("Maps", mapsIcon);
            menuSelections.Add(sel);

            Bitmap booksIcon = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.library);
            sel = new Selection("Books", booksIcon);
            menuSelections.Add(sel);

            
            GridView gridView = FindViewById<GridView>(Resource.Id.gridView1);

            mGridAdapter = new MainActivityGridAdapter(this, Resource.Layout.main_activity_option, menuSelections);
            gridView.Adapter = mGridAdapter;
            gridView.ItemClick += Selection_ItemClick;
            mGridAdapter.AddAll(menuSelections);
            mGridAdapter.NotifyDataSetChanged();
          
            //RunOnUiThread(() =>
            //    {
            //        adapter.Add(sel);
            //        adapter.NotifyDataSetChanged();

            //    });

//MainActivityGridAdapter adapter = new MainActivityGridAdapter(this, Resource.Menu.main_activity_option, menuSelections);
            //gridView.Adapter = adapter;
            //adapter.Add(sel);
           // adapter.NotifyDataSetChanged();


            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.ProtectedServicesButton);
            //button.Click += NavigateToProtectedServices;


            //Button coursesButton = FindViewById<Button>(Resource.Id.ScheduleButton);
            //coursesButton.Click += NavigateToCoursesView;

            //Button booksButton = FindViewById<Button>(Resource.Id.BooksButton);
            //booksButton.Click += NavigateToBooksView;

            //Button transitButton = FindViewById<Button>(Resource.Id.TransitButton);
            //transitButton.Click += NavigateToTransitView;

            //Button mapsButton = FindViewById<Button>(Resource.Id.MapsButton);
            //mapsButton.Click += NavigateToMapsView;
           
         

        }

        protected override void OnResume()
        {
            if(CookieService.CookieExists("CASTGC"))
            {
                
                //IMenu menu = this.FindViewById<IMenu>(Resource.Menu.main_activity_actions);
                IMenuItem item = mActionBarMenu.FindItem(Resource.Id.action_login);
                item.SetTitle("Logout");
                
               
            }
            base.OnResume();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.mActionBarMenu = menu;
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.main_activity_actions, menu);
            IMenuItem item = menu.FindItem(Resource.Id.action_login);
            if(CookieService.CookieExists("CASTGC"))
            {
                item.SetTitle("Logout");
            }
            
            return base.OnCreateOptionsMenu(menu);
        }

        

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_login:
                    if (CookieService.CookieExists("CASTGC"))
                    {
                        LogoutUser();
                    }
                    else
                    {
                        NavigateToLoginView();
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }


        }


        void NavigateToProtectedServices(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ProtectedServicesActivity));
            StartActivity(intent);
        }

        void NavigateToLoginView()
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

        void NavigateToMapsView(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MapsActivity));
            StartActivity(intent);
        }

        private void Selection_ItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            Selection item = sender as Selection;
            if(item.Title.Equals("Schedule"))
            {
                Intent intent = new Intent(this, typeof(ScheduleActivity));
                StartActivity(intent);
            }
            else if(item.Title.Equals("Maps"))
            {
                Intent intent = new Intent(this, typeof(MapsActivity));
             StartActivity(intent);
            }
            else if(item.Title.Equals("Protected \n Services"))
            {
                Intent intent = new Intent(this, typeof(ProtectedServicesActivity));
            StartActivity(intent);
            }
            else if(item.Title.Equals("Books"))
            {
                Intent intent = new Intent(this, typeof(BooksActivity));
            StartActivity(intent);
            }
            else //Transit
            {
                Intent intent = new Intent(this, typeof(TransitActivity));
            StartActivity(intent);
            }
        }

        /// <summary>
        /// Trys to Login the User
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TryLoginUser()
        {
            WebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://cas.sfu.ca/cgi-bin/WebObjects/cas.woa/wa/login");
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;
            IAsyncResult response = request.BeginGetResponse(new AsyncCallback(GetLoginResponseCallback), request);

        }

        /// <summary>
        /// Logouts out the user
        /// </summary>
        private void LogoutUser()
        {
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            var editor = preferences.Edit();
            editor.PutString("ComputingId", "");
            editor.PutString("Password", "");
            editor.PutString("courses", "");
            CookieService.DeleteCookies();
            editor.Commit();
            IMenuItem menuItem = this.mActionBarMenu.FindItem(Resource.Id.action_login);
            menuItem.SetTitle("Login");
            /*
            RunOnUiThread(() =>
                {
                    Button loginButton = this.FindViewById<Button>(Resource.Id.LoginButton);
                    loginButton.Text = "Login";
                });
             * */
        }
        

        /// <summary>
        /// Get the login page to strip the page of the Key
        /// </summary>
        /// <param name="result"></param>
        private void GetLoginResponseCallback(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            //post logout
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string responseString = reader.ReadToEnd();
            HtmlDocument document = new HtmlDocument();
            document.OptionFixNestedTags = true;
            document.LoadHtml(responseString);
            HtmlNode node = CheckLine(document.DocumentNode);
            HtmlAttribute attribute = node.Attributes[1];
            mKey = attribute.Value;
            LoginUser();

        }

        /// <summary>
        /// Login the user with their computing Id and password
        /// </summary>
        private void LoginUser()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://cas.sfu.ca/cgi-bin/WebObjects/cas.woa/wa/login");
            request.CookieContainer = new CookieContainer();
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.BeginGetRequestStream(new AsyncCallback(GetLoginRequestStreamCallback), request);
        }

        /// <summary>
        /// Send the post request with the user's data
        /// </summary>
        /// <param name="asyncResult"></param>
        private void GetLoginRequestStreamCallback(IAsyncResult asyncResult)
        {
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;

            Stream stream = request.EndGetRequestStream(asyncResult);
            string loginData = "username=" + preferences.GetString("ComputingId", string.Empty) + "&password=" + preferences.GetString("Password", string.Empty) + "&lt=" + mKey;
            byte[] bytes = Encoding.UTF8.GetBytes(loginData);
            stream.Write(bytes, 0, loginData.Length);
            stream.Close();

            request.BeginGetResponse(new AsyncCallback(GetLoggedInCallback), request);
        }

        /// <summary>
        /// Get the page for if the user has logged in.
        /// If yes, CookieService will have a CASTGC cookie
        /// </summary>
        /// <param name="asyncResult"></param>
        private void GetLoggedInCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            Stream stream = stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            CookieCollection cookies = request.CookieContainer.GetCookies(new Uri("https://cas.sfu.ca/cgi-bin/WebObjects/cas.woa/wa/login"));


            if (CookieService.CookieExists("CASTGC"))
            {
                CookieService.RemoveCookieWithName("CASTGC");
            }

            foreach (Cookie cookie in cookies)
            {
                CookieService.AddCookie(cookie);
                if (cookie.Name == "CASTGC")
                {
                    
                    /*
                    RunOnUiThread(() =>
                        {
                            Button loginButton = this.FindViewById<Button>(Resource.Id.LoginButton);
                            loginButton.Text = "Logout";
                        });
                    */
                }
            }
        }

        /// <summary>
        /// Parse the html rows until the row with the key is found
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static HtmlNode CheckLine(HtmlNode node)
        {
            if (node.Line == 55)
            {
                return node;
            }
            foreach (HtmlNode nd in node.ChildNodes)
            {
                var val = CheckLine(nd);
                if (val != null)
                    return val;
            }


            return null;
        }

    }
}

