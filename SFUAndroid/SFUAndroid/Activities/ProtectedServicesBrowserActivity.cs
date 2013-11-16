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
using Android.Webkit;
using SFUAndroid.Services;
using System.Net;

namespace SFUAndroid.Activities
{
    [Activity(Label = "")]
    public class ProtectedServicesBrowserActivity : Activity
    {
        private string mSite;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ProtectedServicesBrowser);


            mSite = this.Intent.GetStringExtra("url");
            WebView view = FindViewById<WebView>(Resource.Id.ps_webView);
            WebViewClient client = new WebViewClient();
            //client.OnReceivedSslError += (
            //client.OnReceivedSslError = RecieveSslError;

            view.SetWebViewClient(client);
            view.Settings.JavaScriptEnabled = true;
            view.ClearSslPreferences();

            
            CookieSyncManager.CreateInstance(this);
            CookieManager cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
            CookieSyncManager.Instance.Sync();
            cookieManager.RemoveSessionCookie();
            cookieManager.SetAcceptCookie(true);
            Cookie cookie = CookieService.GetCookieWithName("CASTGC");
            cookieManager.SetCookie("cas.sfu.ca", cookie.Name + "=" + cookie.Value + "; domain=" + cookie.Domain);
            CookieSyncManager.Instance.Sync();
            view.LoadUrl(mSite);
            //view.LoadUrl("http://www.gooogle.ca/");

            

            // Create your application here
        }

        public  void RecieveSslError(WebView view, SslErrorHandler handler, Android.Net.Http.SslError error)
        {
            handler.Proceed();
        }

    }
}