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
using SFUAndroid.Entities;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Android.Graphics;
using SFUAndroid.Adapters;
using SFUAndroid.Services;

namespace SFUAndroid.Activities
{
    [Activity(Label = "Books", ParentActivity = typeof(MainActivity), Theme = "@android:style/Theme.Holo.Light")]
    public class BooksActivity : Activity
    {
        private List<Item> mBooks;
        private BookArrayAdapter mBookAdapter;
        private ProgressDialog mDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Books);
            mBooks = new List<Item>();
            mBookAdapter = new BookArrayAdapter(this, mBooks);
            //mBooks.Add(new Header((LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), "Books"));
           // mBooks.Add(new Book((LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), "hats", "sats", "zomg", "aaa", "sddsd", "s224454", 22, 22));
            
            
            ListView bookListView = FindViewById<ListView>(Resource.Id.BooksListView);
            bookListView.Adapter = mBookAdapter;


           // mBookAdapter.AddAll(mBooks);
          

           // mBookAdapter.Add(new Book((LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), "hats", "sats", "zomg", "aaa", "sddsd", "s224454", 22, 22));
          //  mBookAdapter.NotifyDataSetChanged();

           // GetBooks();


            //mBooks = LoadBooks();
            //if(mBooks == null)
            //{

            List<Course> courses = GetCourses();
            if (courses == null)
            {
                Android.Widget.Toast.MakeText(this, "No Books Found. Please Refresh Schedule.", Android.Widget.ToastLength.Long).Show();
            }
            else
            {

                mBookAdapter.Add(new Header((LayoutInflater)this.BaseContext.GetSystemService(Context.LayoutInflaterService), "Books"));
                mBookAdapter.NotifyDataSetChanged();

                mDialog = new ProgressDialog(this);
                mDialog.Indeterminate = true;
                mDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                mDialog.SetMessage("Loading...");
                mDialog.Show();
                GetBooks();
            }
            //}
            //else
            //{
            //   // mBooks = LoadBooks();
            //    mBookAdapter.AddAll(mBooks);
            //    mBookAdapter.NotifyDataSetChanged();
            //}


            
        }

        private void GetBooks()
        {
            HttpWebRequest request = null;
            List<Course> courses = GetCourses();
            courses = courses.Where(c => c.Type == "Lecture").ToList();
            foreach (Course course in courses)
            {
                request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://sfu.collegestoreonline.com/ePOS?form=shared3/textbooks/json/json_books.html&term={0}&dept={1}&crs={2}&sec={3}&go=Go", SemesterHelper.GetSemesterId(), Regex.Split(course.ClassName, @"(\w+)(\d)")[0].Trim().ToLower(), Regex.Split(course.ClassName, @"(\d+)")[1], course.Section));
                request.Method = "GET";
                request.BeginGetResponse(new AsyncCallback(GetBookResponse), request);
            }
        }

        private void GetBookResponse(IAsyncResult result)
        {

            try
            {

                HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                Stream stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    JObject jsonClass = JObject.Parse(json);

                    JObject course = (JObject)jsonClass["course"];

                    JArray bookArray = (JArray)course["books"];
                    foreach (JObject book in bookArray)
                    {
                        string title = book["title"].ToString();
                        if (title == "No Books Found")
                        {
                            continue;
                        }
                        string className = course["courseAcdeptcode"].ToString();
                        string classNumber = course["courseClass"].ToString();

                        string author = book["author"].ToString();
                        string status = book["bookstatus"].ToString();
                        string isbn = book["isbn"].ToString();

                        GetBookCover(isbn);

                        JArray detailsArray = (JArray)book["details"];
                        string newPrice = string.Empty;
                        string usedPrice = string.Empty;
                        foreach (JObject detail in detailsArray)
                        {
                            if (detail["isNew"].ToString() == "1")
                            {
                                newPrice = detail["price"].ToString();
                            }
                            else if (detail["isUsed"].ToString() == "1")
                            {
                                usedPrice = detail["price"].ToString();
                            }
                        }
                        float newP;
                        float.TryParse(newPrice, out newP);
                        float usedP;
                        float.TryParse(usedPrice, out usedP);
                        Book bk = new Book((LayoutInflater)this.GetSystemService(Context.LayoutInflaterService), className, classNumber, title, author, status, isbn, newP, usedP);
                        mBooks.Add(bk);

                        // mBookAdapter.AddBook(bk);

                        RunOnUiThread(() =>
                            {
                                mDialog.Cancel();
                                mBookAdapter.Add(new Book((LayoutInflater)this.GetSystemService(Context.LayoutInflaterService), className, classNumber, title, author, status, isbn, newP, usedP));
                                mBookAdapter.NotifyDataSetChanged();
                            });


                    }
                }
            }
            catch (Exception e) { }
        }

        private void GetBookCover(string isbn)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://www.googleapis.com/books/v1/volumes?q=isbn:{0}", isbn));
            ServicePointManager.ServerCertificateValidationCallback = (p1, p2, p3, p4) => true;
            request.BeginGetResponse(new AsyncCallback(GetBookCoverResponse), request);
        }

        private void GetBookCoverResponse(IAsyncResult result)
        {
            try
            {


                HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                Stream stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    JObject bookCover = JObject.Parse(json);
                    JArray items = bookCover["items"] as JArray;
                    if (items != null)
                    {
                        JObject item = items[0] as JObject;
                        JObject volumeInfo = item["volumeInfo"] as JObject;
                        JObject imageLinks = volumeInfo["imageLinks"] as JObject;
                        string thumbNail = imageLinks["smallThumbnail"].ToString();
                        WebClient client = new WebClient();
                        // client.OpenReadCompleted += client_OpenReadCompleted;
                        client.DownloadDataCompleted += (s, e) =>
                        {
                            var bytes = e.Result;
                            Bitmap bmp = null;
                            BitmapFactory.Options options = new BitmapFactory.Options();
                            if (bytes != null)
                            {
                                bmp = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
                                string isbn = client.Headers["Isbn"];
                                List<Book> books = mBooks.Where(b => b.GetType() == typeof(Book)).Cast<Book>().ToList<Book>();
                                Book book = books.Where(b => b.Isbn == isbn).FirstOrDefault();
                                book.Image = bmp;

                                RunOnUiThread(() =>
                                {
                                    mBookAdapter.NotifyDataSetChanged();
                                });
                            }
                            //string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                            //string localFileName = item.ToString();
                            //string localPath = Path.Combine(documentsPath, localFileName);
                            //File.WriteAllBytes(localPath, bytes);
                        };
                        client.Headers["Isbn"] = request.RequestUri.OriginalString.Split(':')[2];
                        client.DownloadDataAsync(new Uri(thumbNail));
                        //client.OpenReadAsync(new Uri(thumbNail), HttpCompletionOption.ResponseContentRead);
                        // var file = client.DownloadFile(





                    }

                }
            }
            catch (Exception e) { }
        }


        /// <summary>
        /// Load courses from disk
        /// </summary>
        /// <returns></returns>
        private List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = preferences.GetString("courses", string.Empty);
            courses = JsonConvert.DeserializeObject<List<Course>>(json);
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

        private List<Item> LoadBooks()
        {
            try
            {
                List<Item> books = new List<Item>();
                var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
                string json = preferences.GetString("books", string.Empty);
                List<Book> b = JsonConvert.DeserializeObject<List<Book>>(json);
                books.AddRange(b);
                return books;
            }
            catch (Exception e) { return null; }
        }

        private void SaveBooks(List<Book> books)
        {
            var preferences = this.GetSharedPreferences("sfuandroid-settings", FileCreationMode.Private);
            string json = JsonConvert.SerializeObject(books);
            var editor = preferences.Edit();
            editor.PutString("books", json);

            editor.Commit();
        }

    }
}