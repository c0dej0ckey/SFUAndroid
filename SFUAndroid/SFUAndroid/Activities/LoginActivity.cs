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
using System.Net;
using System.IO;
using HtmlAgilityPack;
using SFUAndroid.Services;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Login")]
    public class LoginActivity : Activity
    {
        private string mKey;
        private string ComputingId;
        private string Password;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Login);
            Button loginButton = FindViewById<Button>(Resource.Id.LoginUserButton);
            loginButton.Click += TryLoginUser;

            // Create your application here
        }

        private void TryLoginUser(object sender, EventArgs e)
        {
            EditText computingIdBox = FindViewById<EditText>(Resource.Id.ComputingIdText);
            ComputingId = computingIdBox.Text;
            EditText passwordBox = FindViewById<EditText>(Resource.Id.PasswordText);
            Password = passwordBox.Text;

            WebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://cas.sfu.ca/cgi-bin/WebObjects/cas.woa/wa/login");
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;
            IAsyncResult response = request.BeginGetResponse(new AsyncCallback(GetLoginResponseCallback), request);
            //login sh1t
        }

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

        private void LoginUser()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://cas.sfu.ca/cgi-bin/WebObjects/cas.woa/wa/login");
            //ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;
            request.CookieContainer = new CookieContainer();
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.BeginGetRequestStream(new AsyncCallback(GetLoginRequestStreamCallback), request);
        }

        private void GetLoginRequestStreamCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;

            Stream stream = request.EndGetRequestStream(asyncResult);
            string loginData = "username=" + ComputingId + "&password=" + Password + "&lt=" + mKey;
            byte[] bytes = Encoding.UTF8.GetBytes(loginData);
            stream.Write(bytes, 0, loginData.Length);
            stream.Close();

            request.BeginGetResponse(new AsyncCallback(GetLoggedInCallback), request);
        }

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
                    //ServiceLocator.AddService<CookieCollection>(cookies);
                    //FlurryWP8SDK.Api.SetUserId(Settings.ComputingId);
                    //sLogInStatus = false;


                }
            }
            //getting cookie failed
            /*
            if (sLogInStatus == true)
            {
                ErrorVisibility = Visibility.Visible;
                Loading = Visibility.Collapsed;
                sLogInStatus = false;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    OnPropertyChanged("Loading");
                    OnPropertyChanged("ErrorVisibility");
                });

            }
             */
        }


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