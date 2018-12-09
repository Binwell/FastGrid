//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Binwell.Controls.FastGrid.FastGrid {
	[ContentProperty("View")]
	public abstract class FastGridCell : ContentView, IDisposable {
		public View View {
			get => Content;
			set => Content = value;
		}

		public bool IsInitialized { get; private set; }
		public Size CellSize { get; set; }

		object _currentBindingContext;

		public void PrepareCell(Size cellSize) {
			CellSize = cellSize;
			InitializeCell();
			if (BindingContext != null)
				SetupCell(false);
			IsInitialized = true;
		}

		protected override void OnBindingContextChanged() {
			base.OnBindingContextChanged();

			if (_currentBindingContext is INotifyPropertyChanged notifyPropertyChanged)
				notifyPropertyChanged.PropertyChanged -= OnBindingContextPropertyChanged;

			_currentBindingContext = BindingContext;

			if (_currentBindingContext is INotifyPropertyChanged newNotifyPropertyChanged)
				newNotifyPropertyChanged.PropertyChanged += OnBindingContextPropertyChanged;

			if (IsInitialized)
				SetupCell(true);
		}

		protected abstract void InitializeCell();

		public virtual void ItemTapped() {}

		protected abstract void SetupCell(bool isRecycled);

		public virtual void Dispose() {
			if (_currentBindingContext is INotifyPropertyChanged notifyPropertyChanged)
				notifyPropertyChanged.PropertyChanged -= OnBindingContextPropertyChanged;

			_currentBindingContext = null;
		}

		protected virtual void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e) {
		}
	}
}