using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Binwell.Controls.FastGrid.FastGrid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Size = Xamarin.Forms.Size;
using View = Android.Views.View;

namespace Binwell.Controls.FastGrid.Android.FastGrid {
	public class FastGridAdapter : RecyclerView.Adapter {
		readonly RecyclerView _recyclerView;
		IEnumerable _items;

		readonly DisplayMetrics _displayMetrics;
		readonly FastGridViewRenderer _fastGridViewRenderer;

		readonly FastGridView _fastGridView;
		readonly PropertyInfo _realParentProperty;

        FastGridView Element { get; }

		public IEnumerable Items {
			get => _items;
			set {
				if (_items is INotifyCollectionChanged oldCollection) {
					oldCollection.CollectionChanged -= NewCollection_CollectionChanged;
				}

				_items = value;
				if (_items is INotifyCollectionChanged newCollection) {
					newCollection.CollectionChanged += NewCollection_CollectionChanged;
				}

				NotifyDataSetChanged();
			}
		}

		void NewCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			_fastGridViewRenderer.CalculateLayoutRects();
			try {
				((DefaultItemAnimator) _recyclerView.GetItemAnimator()).SupportsChangeAnimations = !Element.CollectionChangedWithoutAnimation;
			}
			catch {
				//ignored
			}

			RecyclerView.ItemAnimator animator = null;
			if (Element.CollectionChangedWithoutAnimation) {
				animator = _recyclerView.GetItemAnimator();
				_recyclerView.SetItemAnimator(null);
			}

			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems == null) return;
					var oneAdd = e.NewItems.Count == 1;
					if (oneAdd) NotifyItemInserted(e.NewStartingIndex);
					else NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems == null) return;
					var oneRemove = e.OldItems.Count == 1;
					if (oneRemove) NotifyItemRemoved(e.OldStartingIndex);
					else NotifyItemRangeRemoved(e.OldStartingIndex, e.OldItems.Count);
					break;
				case NotifyCollectionChangedAction.Replace:
					NotifyItemChanged(e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Move:
					NotifyItemMoved(e.OldStartingIndex, e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Reset:
					NotifyDataSetChanged();
					break;
			}

		    if (Element.CollectionChangedWithoutAnimation)
		        _recyclerView.SetItemAnimator(animator);
		}

		public FastGridAdapter(IEnumerable items, RecyclerView recyclerView, FastGridView fastGridView, DisplayMetrics displayMetrics, FastGridViewRenderer fastGridViewRenderer) {
			Items = items;
			_recyclerView = recyclerView;
			Element = fastGridView;
			_displayMetrics = displayMetrics;
			_fastGridViewRenderer = fastGridViewRenderer;
			_fastGridView = fastGridView;
			_realParentProperty = typeof(Element).GetProperty("RealParent");
		}

		public override int GetItemViewType(int position) {
			var item = Items.Cast<object>().ElementAt(position);
			if (Element.ItemTemplateSelector is FastGridTemplateSelector selector)
				return selector.GetViewType(item, _fastGridView);
			return -1;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			var templateSelector = Element.ItemTemplateSelector;

			if (templateSelector == null) return new GridViewHolder(GetViewCell(parent.Context, null, parent, new Size(100, 100), null, viewType, Element), null);

			FastGridCell gridViewCell = null;
			var cellSize = Size.Zero;

			if (templateSelector.OnSelectTemplateByViewType(viewType, null) is FastGridDataTemplate template) {
				gridViewCell = template.CreateContent() as FastGridCell;
				cellSize = template.CellSize;
			}

			if (gridViewCell == null) return new GridViewHolder(GetViewCell(parent.Context, null, parent, new Size(100, 100), null, viewType, Element), null);


			cellSize.Width = cellSize.Width > 0 ? cellSize.Width : -1;
			cellSize.Height = cellSize.Height > 0 ? cellSize.Height : -1;

			//Without this line crashed in GetRenderer method for ListView
			_realParentProperty?.SetValue(gridViewCell, _fastGridView);
			var view = GetViewCell(parent.Context, gridViewCell, parent, cellSize, mMainView_Click, viewType, Element);

			_realParentProperty?.SetValue(gridViewCell, null);
			gridViewCell.Parent = _fastGridView;

			switch (viewType) {
				case 0: return new GridViewHolder0(view, gridViewCell.GetType());
				case 1: return new GridViewHolder1(view, gridViewCell.GetType());
				case 2: return new GridViewHolder2(view, gridViewCell.GetType());
				case 3: return new GridViewHolder3(view, gridViewCell.GetType());
				case 4: return new GridViewHolder4(view, gridViewCell.GetType());
				case 5: return new GridViewHolder5(view, gridViewCell.GetType());
				case 6: return new GridViewHolder6(view, gridViewCell.GetType());
				case 7: return new GridViewHolder7(view, gridViewCell.GetType());
				case 8: return new GridViewHolder8(view, gridViewCell.GetType());
				case 9: return new GridViewHolder9(view, gridViewCell.GetType());
				case 10: return new GridViewHolder10(view, gridViewCell.GetType());
				case 11: return new GridViewHolder11(view, gridViewCell.GetType());
				case 12: return new GridViewHolder12(view, gridViewCell.GetType());
				case 13: return new GridViewHolder13(view, gridViewCell.GetType());
				case 14: return new GridViewHolder14(view, gridViewCell.GetType());
				case 15: return new GridViewHolder15(view, gridViewCell.GetType());
			}

			return new GridViewHolder(view, gridViewCell.GetType());
		}

		public static View GetViewCell(Context context, FastGridCell fastGridCell, ViewGroup parent, Size initialCellSize, EventHandler click, int viewType, FastGridView element) {
			if (fastGridCell == null) return new Space(context);
			fastGridCell.PrepareCell(initialCellSize);
			var view = FormsToNativeDroid.ConvertFormsToNative(context, fastGridCell.View, initialCellSize, context.Resources.DisplayMetrics.Density);
			if (view == null) return null;

			view.Tag = new ViewCellProperties {
				Cell = fastGridCell,
				ViewType = viewType,
				Size = new Rectangle(0, 0, initialCellSize.Width, initialCellSize.Height)
			};

			fastGridCell.View.GestureRecognizers.Clear();

			var tap = new TapGestureRecognizer {
				Command = new Command(() => {
					fastGridCell.ItemTapped();
					click?.Invoke(view, EventArgs.Empty);
				})
			};
			fastGridCell.View.GestureRecognizers.Add(tap);

			return view;
		}


		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			if (!(holder is GridViewHolder myHolder)) return;
			var item = Items.Cast<object>().ElementAt(position);
			var properties = myHolder.View.Tag as ViewCellProperties;
			var cell = properties?.Cell;

			if (cell == null) return;
			try {
				cell.BindingContext = item;
				var pSize = properties.Size;

				if (pSize.Width > 0 && pSize.Height > 0) {
					cell.Layout(pSize);
				}
				else {
					var size = cell.View.Measure(properties.Size.Width, properties.Size.Height, MeasureFlags.IncludeMargins).Request;
					pSize.Width = pSize.Width > 0 ? pSize.Width : size.Width;
					pSize.Height = pSize.Height > 0 ? pSize.Height : size.Height;
					var d = _displayMetrics.Density;
					cell.Layout(pSize);
					myHolder.View.LayoutParameters = new ViewGroup.LayoutParams(ConvertDpToPixels(pSize.Width, d), ConvertDpToPixels(pSize.Height, d));
					(myHolder.View as IVisualElementRenderer)?.Tracker?.UpdateLayout();
				}

			}
			catch {
				//ignored
			}
		}

		static int ConvertDpToPixels(double dpValue, float density) {
			var pixels = (int) (dpValue * density);
			return pixels;
		}

		void mMainView_Click(object sender, EventArgs e) {
			if (sender == null) return;
			try {
				var position = _recyclerView.GetChildAdapterPosition((View) sender);
				var item = Items.Cast<object>().ElementAt(position);
				Element.InvokeItemSelectedEvent(this, item);
			}
			catch {
				// ignored
			}
		}

		public override int ItemCount {
			get {
				var count = (Items as ICollection)?.Count ?? 0;
				return count;
			}
		}
	}

	internal sealed class ViewCellProperties : Java.Lang.Object {
		public FastGridCell Cell { get; set; }
		public int ViewType { get; set; }
		public EventHandler Click { get; set; }
		public Rectangle Size { get; set; }
	}
}