using System;

namespace Binwell.Controls.FastGrid.FastGrid
{
	public interface IScrollAwareElement
	{
		event EventHandler<ControlScrollEventArgs> OnStartScrollEvent;
		event EventHandler<ControlScrollEventArgs> OnStopScrollEvent;
		event EventHandler<ControlScrollEventArgs> OnScrollEvent;

		void RaiseOnScroll(double delta, double currentX, double currentY, ScrollActionType type);
		void RaiseOnStartScroll(double currentX, double currentY, ScrollActionType type);
		void RaiseOnStopScroll(double currentX, double currentY, ScrollActionType type, bool fullStop);
	}
	
	public enum ScrollActionType {
		Finger,
		Fling,
		Auto,
		None
	}

	public class ControlScrollEventArgs : EventArgs
	{
		public double Delta { get; set; }
		public double CurrentY { get; set; }
		public double CurrentX { get; set; }
		public ScrollActionType Type { get; set; }
		public bool FullStop { get; }

		public ControlScrollEventArgs(double delta, double currentX, double currentY, ScrollActionType type, bool fullStop=false)
		{
			Delta = delta;
			CurrentY = currentY;
			Type = type;
			FullStop = fullStop;
			CurrentX = currentX;
		}

	}
}
