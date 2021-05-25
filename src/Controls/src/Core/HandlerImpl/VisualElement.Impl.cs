﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	public partial class VisualElement : IView
	{
		Semantics _semantics;
		IFrameworkElementHandler _handler;

		public Rectangle Frame => Bounds;

		public IFrameworkElementHandler Handler
		{
			get => _handler;
			set
			{
				_handler = value;
				IsPlatformEnabled = _handler != null;
			}
		}

		Paint Maui.IVisual.Background
		{
			get
			{
				if (!Brush.IsNullOrEmpty(Background))
					return Background;
				if (BackgroundColor.IsNotDefault())
					return new SolidColorBrush(BackgroundColor);
				return null;
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
			(Handler)?.UpdateValue(propertyName);
		}

		IFrameworkElement IFrameworkElement.Parent => Parent as IView;

		public Size DesiredSize { get; protected set; }

		public void Arrange(Rectangle bounds)
		{
			Layout(bounds);
		}

		Size IArrangeable.Arrange(Rectangle bounds)
		{
			return ArrangeOverride(bounds);
		}

		// The CloseEnough methods will likely go away once we get rid of the cause (see comments below); if not, we'll
		// add a proper extension method or add a compare method to MG with a tolerance parameter
		bool CloseEnough(double a, double b)
		{
			const double tolerance = 0.0001;

			return (Math.Abs(a - b) < tolerance);
		}

		bool CloseEnough(Rectangle currentBounds, Rectangle newBounds)
		{
			if (!CloseEnough(currentBounds.X, newBounds.X))
			{
				return false;
			}

			if (!CloseEnough(currentBounds.Y, newBounds.Y))
			{
				return false;
			}

			if (!CloseEnough(currentBounds.Width, newBounds.Width))
			{
				return false;
			}

			if (!CloseEnough(currentBounds.Height, newBounds.Height))
			{
				return false;
			}

			return true;
		}

		// ArrangeOverride provides a way to allow subclasses (e.g., Layout) to override Arrange even though
		// the interface has to be explicitly implemented to avoid conflict with the old Arrange method
		protected virtual Size ArrangeOverride(Rectangle bounds)
		{
			// We check the previous bounds here to avoid getting into a loop caused by the OnSizeAllocated override
			// in View.cs; the arrange it forces ends up back here and if we have a margin, ComputeFrame will 
			// keep applying it in a loop until the element disappears. Hopefully we can remove the OnSizeAllocated 
			// hack at some point and avoid this extra check.
			if (!CloseEnough(Bounds, bounds))
			{
				// Setting Bounds here is equivalent to setting the Frame
				Bounds = this.ComputeFrame(bounds);
			}

			return Frame.Size;
		}

		public void Layout(Rectangle bounds)
		{
			Bounds = bounds;
		}

		void IArrangeable.InvalidateMeasure()
		{
			InvalidateMeasureOverride();
		}

		// InvalidateMeasureOverride provides a way to allow subclasses (e.g., Layout) to override InvalidateMeasure even though
		// the interface has to be explicitly implemented to avoid conflict with the VisualElement.InvalidateMeasure method
		protected virtual void InvalidateMeasureOverride() => Handler?.UpdateValue(nameof(IView.InvalidateMeasure));

		void IArrangeable.InvalidateArrange()
		{
		}

		Size IArrangeable.Measure(double widthConstraint, double heightConstraint)
		{
			return MeasureOverride(widthConstraint, heightConstraint);
		}

		// MeasureOverride provides a way to allow subclasses (e.g., Layout) to override Measure even though
		// the interface has to be explicitly implemented to avoid conflict with the old Measure method
		protected virtual Size MeasureOverride(double widthConstraint, double heightConstraint)
		{
			DesiredSize = this.ComputeDesiredSize(widthConstraint, heightConstraint);
			return DesiredSize;
		}

		Maui.FlowDirection IFlowLayout.FlowDirection => FlowDirection.ToPlatformFlowDirection();

		Semantics IFrameworkElement.Semantics
		{
			get => _semantics;
		}

		// We don't want to initialize Semantics until someone explicitly 
		// wants to modify some aspect of the semantics class
		internal Semantics SetupSemantics() =>
			_semantics ??= new Semantics();

		// VisualElement/View together implement parts of IView; the mapping doesn't go 1:1 
		// So we have a couple of members of IView implemented here with default values which are 
		// overridden in View
		Thickness IView.Margin => default;
		Visibility IView.Visibility => default;
		Primitives.LayoutAlignment IView.HorizontalLayoutAlignment => default;
		Primitives.LayoutAlignment IView.VerticalLayoutAlignment => default;
	}
}
