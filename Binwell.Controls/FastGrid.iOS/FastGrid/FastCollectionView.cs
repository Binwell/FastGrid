//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using CoreGraphics;
using UIKit;

namespace Binwell.Controls.FastGrid.iOS.FastGrid
{
    public sealed class FastCollectionView : UICollectionView
    {
        public bool SelectionEnable { get; set; }

        public FastCollectionView() : this(default(CGRect))
        {
        }

        public FastCollectionView(CGRect rect) : base(rect, new LeftFlowCollectionViewLayout())
        {
			AutoresizingMask = UIViewAutoresizing.None;
            ContentMode = UIViewContentMode.ScaleAspectFill;
        }
		
        public double RowSpacing
        {
            get => (CollectionViewLayout as UICollectionViewFlowLayout)?.MinimumLineSpacing ?? 0;
	        set
            {
	            if (CollectionViewLayout is UICollectionViewFlowLayout layout) layout.MinimumLineSpacing = (float) value;
            }
        }

        public double ColumnSpacing
        {
            get => (CollectionViewLayout as UICollectionViewFlowLayout)?.MinimumInteritemSpacing ?? 0;
	        set
            {
	            if (CollectionViewLayout is UICollectionViewFlowLayout layout) layout.MinimumInteritemSpacing = (float) value;
            }
        }
		
    }
}