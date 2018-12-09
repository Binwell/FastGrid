using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

namespace Binwell.Controls.FastGrid.Android.FastGrid
{
	public static class FormsToNativeDroid
	{
		public static View ConvertFormsToNative(Context context, Xamarin.Forms.View view, Size size, float density)
		{
			if (view == null) return null;
			if (Platform.GetRenderer(view) == null)
					Platform.SetRenderer(view, Platform.CreateRendererWithContext(view, context));

			var vRenderer = Platform.GetRenderer(view);
			var nativeView = vRenderer.View;
			var dpW = size.Width > 0 ? ConvertDpToPixels(size.Width, density) : ViewGroup.LayoutParams.WrapContent;
			var dpH = size.Height > 0 ? ConvertDpToPixels(size.Height, density) : ViewGroup.LayoutParams.WrapContent;

			nativeView.LayoutParameters = new ViewGroup.LayoutParams(dpW, dpH);

			return nativeView;
		}


		static int ConvertDpToPixels(double dpValue, float density) {
			var pixels = (int)(dpValue * density);
			return pixels;
		}
	}
}