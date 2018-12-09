//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Binwell.Controls.FastGrid.FastGrid;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

namespace Binwell.Controls.FastGrid.iOS.FastGrid {
	public sealed class FastCollectionViewCell : UICollectionViewCell {
		UIView _view;
		object _originalBindingContext;
		UIGestureRecognizer _tapGestureRecognizer;
		CGSize _lastSize;
		internal Action<object> CellTapped;

		FastGridCell ViewCell { get; set; }

		public static UIView ConvertFormsToNative(View view, Rectangle size) {
			if (view == null) return null;
			try {
				if (Platform.GetRenderer(view) == null)
					Platform.SetRenderer(view, Platform.CreateRenderer(view));
				var vRenderer = Platform.GetRenderer(view);
				var viewGroup = vRenderer.NativeView;
				view.Layout(size);
				return viewGroup;
			}
			catch {
				return new UIView(new CGRect(0, 0, 1, 1));
			}
		}

		public void RecycleCell(object data, FastGridTemplateSelector dataTemplate, VisualElement parent) {
			if (ViewCell == null) {
				var cellSize = new Size(Bounds.Width, Bounds.Height);

				if (!(dataTemplate is FastGridTemplateSelector templateSelector)) throw new NotSupportedException(@"DataTemplate should be FastGridTemplateSelector");

				var template = templateSelector.SelectTemplate(data) as FastGridDataTemplate;
				ViewCell = template?.CreateContent() as FastGridCell;
				cellSize = template?.CellSize ?? cellSize;

				if (ViewCell != null) {
					ViewCell.BindingContext = data;
					ViewCell.PrepareCell(cellSize);
					ViewCell.Parent = parent;

					_originalBindingContext = data;
					_view = ConvertFormsToNative(ViewCell.View, new Rectangle(new Point(0, 0), cellSize));
				}

				if (_view == null) {
					return;
				}

				_view.AutoresizingMask = UIViewAutoresizing.All;
				_view.ContentMode = UIViewContentMode.ScaleAspectFit;
				_view.ClipsToBounds = true;

				ContentView.AddSubview(_view);
			}
			else if (data == _originalBindingContext) {
				ViewCell.BindingContext = _originalBindingContext;
			}
			else {
				ViewCell.BindingContext = data;
			}

			var gr = GestureRecognizers;
			if (gr != null && gr.Length > 0) {
				gr.ForEach(RemoveGestureRecognizer);
			}

			_tapGestureRecognizer = new UITapGestureRecognizer(Tapped);
			AddGestureRecognizer(_tapGestureRecognizer);
		}

		void Tapped() {
			ViewCell.ItemTapped();
			if (ViewCell?.BindingContext != null)
				CellTapped?.Invoke(ViewCell.BindingContext);
		}

		[Export("initWithFrame:")]
		public FastCollectionViewCell(CGRect frame) : base(frame) {
		}

		protected override void Dispose(bool disposing) {
			if (_tapGestureRecognizer != null)
				Device.BeginInvokeOnMainThread(() => {
					RemoveGestureRecognizer(_tapGestureRecognizer);
					_tapGestureRecognizer = null;
				});
			;
			_view = null;
			_originalBindingContext = null;
			if (ViewCell != null) {
				ViewCell.BindingContext = null;
				ViewCell.Parent = null;
				ViewCell = null;
			}

			base.Dispose(disposing);
		}

		public override void LayoutSubviews() {
			base.LayoutSubviews();

			if (_lastSize.Equals(CGSize.Empty) || !_lastSize.Equals(Frame.Size) && ViewCell != null) {
				ViewCell?.View?.Layout(Frame.ToRectangle());
				_lastSize = Frame.Size;
			}

			if (_view != null && !_view.Frame.Equals(Bounds))
				_view.Frame = Bounds;
		}
	}
}