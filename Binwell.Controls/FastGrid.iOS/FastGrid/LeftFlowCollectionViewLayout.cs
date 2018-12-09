using System;
using CoreGraphics;
using UIKit;

namespace Binwell.Controls.FastGrid.iOS.FastGrid
{
	public class LeftFlowCollectionViewLayout : UICollectionViewFlowLayout
	{
	    public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
	    {
	        var attributes = base.LayoutAttributesForElementsInRect(rect);

	        if (ScrollDirection == UICollectionViewScrollDirection.Horizontal) 
	            return attributes;

	        var maxY = -1.0;
	        var leftMargin = SectionInset.Left;
	        var w = CollectionView.Frame.Width;

	        foreach (var layoutAttribute in attributes)
	        {
	            var frame = layoutAttribute.Frame;
	            var x = leftMargin;

	            if (x > SectionInset.Left && leftMargin + frame.Width > w)
	            {
	                x = SectionInset.Left;
	                leftMargin = SectionInset.Left + frame.Width;
	            }
	            else
	            {
	                x = leftMargin;
	                leftMargin += frame.Width;
	            }

	            frame.X = x;
	            layoutAttribute.Frame = frame;
	            maxY = Math.Max(layoutAttribute.Frame.GetMaxY(), maxY);
	        }

	        return attributes;
	    }
    }
}