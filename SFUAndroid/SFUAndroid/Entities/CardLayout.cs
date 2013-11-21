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
using Android.Util;
using Android.Views.Animations;

namespace SFUAndroid.Entities
{
    public class CardLayout : LinearLayout, Android.Views.ViewTreeObserver.IOnGlobalLayoutListener
    {
        public CardLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeLayoutObserver();
        }

        public CardLayout(Context context) : base(context)
        {
            InitializeLayoutObserver();
        }

        private void InitializeLayoutObserver()
        {
            base.Orientation = Android.Widget.Orientation.Vertical;
            base.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }


        public void OnGlobalLayout()
        {
            base.ViewTreeObserver.RemoveGlobalOnLayoutListener(this);
            int heightPx = Context.Resources.DisplayMetrics.HeightPixels;
            bool inversed = false;
            int childCount = base.ChildCount;
            for(int  i = 0; i < childCount; i++)
            {
                View child = base.GetChildAt(i);
                int[] location = new int[2];
                child.GetLocationOnScreen(location);
                if (location[1] > heightPx)
                    break;
                if (!inversed)
                    child.StartAnimation(AnimationUtils.LoadAnimation(Context, Resource.Layout.slide_up_left));
                else
                    child.StartAnimation(AnimationUtils.LoadAnimation(Context, Resource.Layout.slide_up_right));
            }

        }
    }
}