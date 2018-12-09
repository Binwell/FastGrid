using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;

namespace Binwell.Controls.FastGrid.Android.FastGrid
{
    public class SmoothGridLayoutManager : GridLayoutManager
    {
	    readonly Context _context;
	    public RecyclerView RecyclerView { get; set; }
		
        public SmoothGridLayoutManager(Context context, int spanCount, int orientation, bool reverseLayout) : base(context, spanCount, orientation, reverseLayout)
        {
            _context = context;
        }

        public SmoothGridLayoutManager(Context context, int spanCount) : base(context, spanCount)
        {
            _context = context;
        }

        public SmoothGridLayoutManager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            _context = context;
        }

        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
        {
            LinearSmoothScroller smoothScroller = new CustomLinearSmoothScroller(_context, this);

            smoothScroller.TargetPosition = (position);
            StartSmoothScroll(smoothScroller);
        }

	    public override int ComputeVerticalScrollOffset(RecyclerView.State state) {

			var firstItemView = RecyclerView.GetChildAt(0);
		    var lastItemView = RecyclerView.GetChildAt(RecyclerView.ChildCount - 1);
		    var firstItem = RecyclerView.GetChildLayoutPosition(firstItemView);
		    var lastItem = RecyclerView.GetChildLayoutPosition(lastItemView);
		    var itemsBefore = firstItem;
		    if (firstItemView == null) return 0;
		    var laidOutArea = GetDecoratedBottom(lastItemView) - GetDecoratedTop(firstItemView);
		    var itemRange = lastItem - firstItem + 1;
		    var avgSizePerRow = (float)laidOutArea / itemRange;

		    var offset = (int)(itemsBefore * avgSizePerRow + PaddingTop - GetDecoratedTop(firstItemView));
		    return offset;
	    }



		internal class CustomLinearSmoothScroller : LinearSmoothScroller
        {
	        static float MILLISECONDS_PER_INCH = 75f;
	        readonly SmoothGridLayoutManager _layoutManager;

            public CustomLinearSmoothScroller(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public CustomLinearSmoothScroller(Context context, SmoothGridLayoutManager layoutManager) : base(context)
            {
                _layoutManager = layoutManager;
            }

            public override PointF ComputeScrollVectorForPosition(int targetPosition)
            {
                return _layoutManager?.ComputeScrollVectorForPosition(targetPosition);
            }

            protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
            {
                return MILLISECONDS_PER_INCH / (float) displayMetrics.DensityDpi;
            }
        }
    }
}