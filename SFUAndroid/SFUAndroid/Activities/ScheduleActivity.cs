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
using Newtonsoft.Json;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using SFUAndroid.Services;
using SFUAndroid.Adapters;
using Com.Fima.Cardsui.Views;
using Com.Fima.Cardsui.Objects;
using System.Threading;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Schedule", ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class ScheduleActivity : Activity
    {
        private List<Course> mCourses;
        private CardUI mCardView;
        private ProgressDialog mDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Schedule);

            CardUI mCardView = this.FindViewById<CardUI>(Resource.Id.cardsview);
            mCardView.SetSwipeable(false);
            



            ActionBar actionBar = this.ActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);

            mCourses = new List<Course>();

            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string computingId = preferences.GetString("ComputingId", string.Empty);
            string password = preferences.GetString("Password", string.Empty);

            if (string.IsNullOrEmpty(computingId) && string.IsNullOrEmpty(password))
            {
                Android.Widget.Toast.MakeText(this, "Please Login First", ToastLength.Long).Show();
            }
            else
            {

                ////load courses - if not found request them from GOSFU
                mCourses = GetCourses();
                if (mCourses == null)
                {
                    mDialog = new ProgressDialog(this);
                    mDialog.Indeterminate = true;
                    mDialog.SetMessage("Loading...");
                    mDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    mDialog.Show();
                    TryParseCourses();


                }
                else
                {
                    foreach (Course course in mCourses)
                    {
                        CardStack cs = new CardStack();
                        mCardView.AddStack(cs);
                        string str = string.Empty;
                        foreach (CourseOffering offering in course.CourseOfferings)
                        {
                            str = str + offering.Days + "\t" + offering.StartTime + " - " + offering.EndTime + "\n" + offering.Location + "\n";
                        }

                        mCardView.AddCard(new MyCard(course.ClassName, course.Instructor + "\n" + str));
                    }
                }

                mCardView.Refresh();
            }
            
            
            
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.schedule_activity_actions, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Resource.Id.action_refresh:
                    RefreshSchedule();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }

            
        }

        private void RefreshSchedule()
        {
            mDialog = new ProgressDialog(this);
            mDialog.Indeterminate = true;
            mDialog.SetMessage("Loading...");
            mDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mDialog.Show();
            TryParseCourses();
        }

        #region Course Parsing

        /// <summary>
        /// Create the login request to GOSFU and attach any cookies needed.
        /// </summary>
        private void TryParseCourses()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://go.sfu.ca/psp/paprd/?cmd=login&languageCd=ENG");
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;
            request.CookieContainer = new CookieContainer();
            foreach (Cookie cookie in CookieService.GetCookies().Where(c => !c.Name.Equals("CASTGC")))
            {
                if (cookie.Domain == ".sfu.ca")
                    request.CookieContainer.Add(new Uri("http://www" + cookie.Domain + cookie.Path), cookie);
                else
                    request.CookieContainer.Add(new Uri("http://" + cookie.Domain + cookie.Path), cookie);
            }
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 920)";
            request.BeginGetRequestStream(new AsyncCallback(GetSIMSRequestStream), request);

        }

        /// <summary>
        /// Write the login data to request and send it
        /// </summary>
        /// <param name="asyncResult"></param>
        private void GetSIMSRequestStream(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            Stream stream = request.EndGetRequestStream(asyncResult);
            string loginData = string.Format("user={0}&pwd={1}&userid={2}&Submit=Login", "swa53", "5jun38", "SWA53");
            byte[] bytes = Encoding.UTF8.GetBytes(loginData);
            stream.Write(bytes, 0, loginData.Length);
            stream.Close();
            request.BeginGetResponse(new AsyncCallback(GetSIMSResponse), request);

        }

        /// <summary>
        /// Get the login response from SIMS and save any cookies to the app
        /// </summary>
        /// <param name="result"></param>
        private void GetSIMSResponse(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

            Cookie casCookie = CookieService.GetCookieWithName("CASTGC");

            CookieService.DeleteCookies();
            if (casCookie != null)
            {
                CookieService.AddCookie(casCookie);
            }
            CookieCollection cookies = request.CookieContainer.GetCookies(new Uri("https://go.sfu.ca"));
            foreach (Cookie cookie in cookies)
                CookieService.AddCookie(cookie);
            //Settings.GetStudentId();
            string request2String = string.Format("https://sims-prd.sfu.ca/psc/csprd_1/EMPLOYEE/HRMS/c/SA_LEARNER_SERVICES.SS_ES_STUDY_LIST.GBL?Page=SS_ES_STUDY_LIST&Action=U&ACAD_CAREER=UGRD&EMPLID=&INSTITUTION=SFUNV&STRM={0}", "1137"); //SemesterHelper.GetSemesterId());
            HttpWebRequest request2 = (HttpWebRequest)HttpWebRequest.Create(request2String);
            request2.Method = "GET";
            request2.CookieContainer = new CookieContainer();

            foreach (Cookie cookie in cookies)
            {
                if (cookie.Domain == ".sfu.ca")
                    request2.CookieContainer.Add(new Uri("http://www" + cookie.Domain), cookie);
                else
                    request2.CookieContainer.Add(new Uri("https://" + cookie.Domain), cookie);
            }


            request2.BeginGetResponse(new AsyncCallback(GetClassesResponse), request2);
        }

        /// <summary>
        /// If already made a request from the server, attach the appropriate cookies and send request 
        /// for class data
        /// </summary>
        private void GetSIMSResponseWithCookies()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://sims-prd.sfu.ca/psc/csprd_1/EMPLOYEE/HRMS/c/SA_LEARNER_SERVICES.SS_ES_STUDY_LIST.GBL?Page=SS_ES_STUDY_LIST&Action=U&ACAD_CAREER=UGRD&EMPLID=&INSTITUTION=SFUNV&STRM=", "1137")); //SemesterHelper.GetSemesterId()));

            request.CookieContainer = new CookieContainer();
            foreach (Cookie cookie in CookieService.GetCookies().Where(c => c.Domain != "cas.sfu.ca"))
            {
                if (cookie.Domain == ".sfu.ca")
                    request.CookieContainer.Add(new Uri("http://www" + cookie.Domain + cookie.Path), cookie);
                else
                    request.CookieContainer.Add(new Uri("http://" + cookie.Domain + cookie.Path), cookie);
            }
            request.BeginGetResponse(new AsyncCallback(GetClassesResponse), request);
        }

        /// <summary>
        /// Get the user's classes and parse the response
        /// </summary>
        /// <param name="result"></param>
        private void GetClassesResponse(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string classString = reader.ReadToEnd();
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(classString);
            ParseClasses(document);

        }

        /// <summary>
        /// Parse the classes and save them to file
        /// </summary>
        /// <param name="document"></param>
        private void ParseClasses(HtmlDocument document)
        {
            int classIndex = 0;
            int detailsIndex = 0;
            int profCount = 0;
            List<Course> courses = new List<Course>();
            while (document.GetElementbyId("CLASS_TBL_VW_CLASS_SECTION$" + classIndex) != null)
            {
                HtmlNode classDescription = document.GetElementbyId("win1divDERIVED_SSE_DSP_CLASS_DESCR$" + classIndex);

                string className = classDescription.FirstChild.InnerText;

                string section = document.GetElementbyId("CLASS_TBL_VW_CLASS_SECTION$" + classIndex).InnerText;
                string type = document.GetElementbyId("PSXLATITEM_XLATSHORTNAME$95$$" + classIndex).InnerText;
                string credits = document.GetElementbyId("STDNT_ENRL_SSVW_UNT_TAKEN$" + classIndex).InnerText;
                string classStatus = document.GetElementbyId("PSXLATITEM_XLATSHORTNAME$" + classIndex).InnerText;
                string profName = string.Empty;



                try
                {
                    profName = document.GetElementbyId("PERSONAL_VW_NAME$135$$" + profCount).InnerText;
                }
                catch (NullReferenceException) { }

                Course course = new Course(className, section, credits, classStatus, profName, type);

                if (!string.IsNullOrEmpty(profName))
                {
                    profCount++;
                }
                HtmlNode node = document.GetElementbyId("CLASS_TBL_VW_CLASS_SECTION$" + (classIndex + 1));
                if (node != null)
                {
                    while (document.GetElementbyId("CLASS_MTG_VW_MEETING_TIME_START$" + detailsIndex) != null && document.GetElementbyId("win1divCLASS_MTG_VW_MEETING_TIME_START$" + detailsIndex).Line < document.GetElementbyId("CLASS_TBL_VW_CLASS_SECTION$" + (classIndex + 1)).Line)
                    {
                        string startTime = string.Empty;
                        string endTime = string.Empty;
                        string location = string.Empty;
                        string days = string.Empty;
                        string date = string.Empty;
                        try
                        {
                            startTime = document.GetElementbyId("CLASS_MTG_VW_MEETING_TIME_START$" + detailsIndex).InnerText;
                            endTime = document.GetElementbyId("CLASS_MTG_VW_MEETING_TIME_END$" + detailsIndex).InnerText;
                            location = document.GetElementbyId("DERIVED_SSE_DSP_DESCR40$" + detailsIndex).InnerText;
                            days = document.GetElementbyId("DERIVED_SSE_DSP_CLASS_MTG_DAYS$" + detailsIndex).InnerText;
                            date = document.GetElementbyId("DERIVED_SSE_DSP_START_DT$" + detailsIndex).InnerText;
                        }
                        catch (NullReferenceException) { }

                        CourseOffering courseOffering = new CourseOffering(startTime, endTime, location, days, date);
                        course.AddCourseOffering(courseOffering);

                        detailsIndex++;
                    }

                }
                classIndex++;
                courses.Add(course);

            }
            foreach (Course course in courses)
            {
                if (course.Type == "Lecture")
                {
                    List<CourseOffering> offerings = course.CourseOfferings;
                    offerings.RemoveAt(offerings.Count - 1);
                }

            }
            mCourses = new List<Course>(courses);
           


            SaveCourses(courses);
            mCardView = this.FindViewById<CardUI>(Resource.Id.cardsview);
            foreach(Course course in mCourses)
            {
                
                CardStack cs = new CardStack();
                mCardView.AddStack(cs);
                string str = string.Empty;
                foreach(CourseOffering offering in course.CourseOfferings)
                {
                    str = str + offering.Days + "\t" + offering.StartTime + " - " + offering.EndTime + "n"  + offering.Location + "\n";
                }

                mCardView.AddCard(new MyCard(course.ClassName, course.Instructor + "\n" + str));

                RunOnUiThread(() =>
                    {
                        mDialog.Cancel();
                        mCardView.RefreshDrawableState();
                        mCardView.Refresh();
                        
                    });

                //mCardView.Refresh();
            }


            //RunOnUiThread(() =>
            //    {
            //        mCourseAdapter = new CourseAdapter(this, Resource.Layout.Course, mCourses);
            //        ListView CourseListView = FindViewById<ListView>(Resource.Id.CourseListView);
            //        CourseListView.Adapter = mCourseAdapter;
            //        mCourseAdapter.AddAll(mCourses);
            //        mCourseAdapter.NotifyDataSetChanged();
            //    });


        }
        #endregion

        /// <summary>
        /// Load courses from disk
        /// </summary>
        /// <returns></returns>
        private List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = preferences.GetString("courses", string.Empty);
             courses = JsonConvert.DeserializeObject < List<Course>>(json);
            return courses;
        }

        /// <summary>
        /// Save courses to disk
        /// </summary>
        /// <param name="courses"></param>
        private void SaveCourses(List<Course> courses)
        {
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = JsonConvert.SerializeObject(courses);
            var editor = preferences.Edit();
            editor.PutString("courses", json);

            editor.Commit();
        }



    }
}