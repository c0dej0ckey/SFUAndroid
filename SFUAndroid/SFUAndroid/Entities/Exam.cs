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
using Newtonsoft.Json;

namespace SFUAndroid.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Exam
    {
        private string mStartTime;
        private string mEndTime;
        private DateTime mDate;

        public Exam(string startTime, string  endTime, DateTime date)
        {
            this.mStartTime = startTime;
            this.mEndTime = endTime;
            this.mDate = date;
        }

        [JsonProperty]
        public string StartTime
        {
            get { return mStartTime; }
            set { mStartTime = value; }
        }

        [JsonProperty]
        public string EndTime
        {
            get { return mEndTime; }
            set { mEndTime = value; }
        }

        [JsonProperty]
        public DateTime Date
        {
            get { return mDate; }
            set { mDate = value; }
        }

    }
}