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
using System.Text.RegularExpressions;

namespace SFUAndroid.Activities
{
    [Activity(Label = "")]
    public class ProtectedServicesBrowserActivity : Activity
    {
        private string mURL;
        private const string WEBCT_URL = "https://webct.sfu.ca/webct/cobaltMainFrame.dowebct?appforward=/webct/viewMyWebCT.dowebct";
        private const string COURSYS_URL = "https://courses.cs.sfu.ca/";
        private const string CONNECT_URL = "https://connect.sfu.ca/zimbra/m/zmain#1";
        private const string GOSFU_URL = "http://sakai.sfu.ca/portal/login";
        private const string SAKAI_URL = "http://sakai.sfu.ca/portal/login";


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ProtectedServicesBrowser);


            mURL = this.Intent.GetStringExtra("url");
            WebView view = FindViewById<WebView>(Resource.Id.ps_webView);
            ProtectedServiceWebViewClient client = new ProtectedServiceWebViewClient(base.ApplicationContext);

            view.SetWebViewClient(client);
            view.Settings.JavaScriptEnabled = true;
            view.ClearSslPreferences();
            view.Visibility = ViewStates.Invisible;
            
            
            CookieSyncManager.CreateInstance(this);
            CookieManager cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
            CookieSyncManager.Instance.Sync();
            cookieManager.RemoveSessionCookie();
            cookieManager.SetAcceptCookie(true);
            Cookie cookie = CookieService.GetCookieWithName("CASTGC");
            cookieManager.SetCookie("cas.sfu.ca", cookie.Name + "=" + cookie.Value + "; domain=" + cookie.Domain);
            CookieSyncManager.Instance.Sync();
            view.LoadUrl(mURL);
            

        }



        class ProtectedServiceWebViewClient : WebViewClient
        {
            private Context mContext;
            private bool mHasLoggedIn;

            public ProtectedServiceWebViewClient(Context context) : base()
            {
                this.mContext = context;
            }


            /// <summary>
            /// The SFU cookies are buggy and sometimes wont login
            /// if the webview is directed to the login page (CAS). inject javascript in order
            /// to login the person in. 
            /// </summary>
            /// <param name="view"></param>
            /// <param name="url"></param>
            public override void OnPageFinished(WebView view, string url)
            {
                if(Regex.IsMatch(url, "https:\\/\\/cas\\.sfu\\.ca.*"))
                {

                    view.LoadUrl("javascript:(function() { document.getElementById('computingId').value='swa53'; document.getElementById('password').value='5jun38';document.forms[0].submit(); })()");
                    mHasLoggedIn = true;
                }
                else if(url == "https://sims-prd.sfu.ca/psc/csprd_1/EMPLOYEE/HRMS/c/SA_LEARNER_SERVICES.SSS_STUDENT_CENTER.GBL?&" && mHasLoggedIn == false)
                {
                    view.LoadUrl("javascript:(function() { document.getElementById('user').value='swa53'; document.getElementById('pwd').value='5jun38';document.getElementById('userid').value='SWA53'; document.forms[0].submit(); })()");
                    mHasLoggedIn = true;
                }
                else if ((url == WEBCT_URL || url == CONNECT_URL || url == COURSYS_URL || url == SAKAI_URL) && mHasLoggedIn == true)
                {
                    view.Visibility = ViewStates.Visible;
                    mHasLoggedIn = false;
                }
                else
                {
                    view.Visibility = ViewStates.Visible;
                    mHasLoggedIn = false;
                }

                base.OnPageFinished(view, url);
            }

        }


    }
}