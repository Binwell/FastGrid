using Binwell.Controls.FastGrid.FastGrid;
using Xamarin.Forms.Xaml;

namespace FastGridSample.Cells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryCell : FastGridCell
    {
        protected override void InitializeCell()
        {
            InitializeComponent();
        }

        protected override void SetupCell(bool isRecycled)
        {
        }
    }
}