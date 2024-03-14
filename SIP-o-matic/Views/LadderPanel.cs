using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SIP_o_matic.Views
{
	public class LadderPanel : Panel
	{


		public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register("Devices", typeof(IEnumerable<object>), typeof(LadderPanel), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,DevicesPropertyChanged));
		public IEnumerable<object> Devices
		{
			get { return (IEnumerable<object>)GetValue(DevicesProperty); }
			set { SetValue(DevicesProperty, value); }
		}

		public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth", typeof(int), typeof(LadderPanel), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public int ColumnWidth
		{
			get { return (int)GetValue(ColumnWidthProperty); }
			set { SetValue(ColumnWidthProperty, value); }
		}


		public static readonly DependencyProperty SourceDeviceProperty = DependencyProperty.RegisterAttached("SourceDevice", typeof(object), typeof(LadderPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		public static readonly DependencyProperty DestinationDeviceProperty = DependencyProperty.RegisterAttached("DestinationDevice", typeof(object), typeof(LadderPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

		public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.RegisterAttached("IsFlipped", typeof(bool), typeof(LadderPanel), new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



		private int width
		{
			get
			{
				if (Devices == null) return 0;
				return Devices.Count() * ColumnWidth;
			}
		}



		public LadderPanel()
		{
			
		}


		private static void DevicesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LadderPanel? panel;
			panel = d as LadderPanel;
			if (panel == null) return;
			panel.OnDevicesPropertyChanged((IEnumerable<object>)e.OldValue, (IEnumerable<object>)e.NewValue);
		}
		private void OnDevicesPropertyChanged(IEnumerable<object> OldValue, IEnumerable<object> NewValue)
		{
			INotifyCollectionChanged? collectionChanged;

			collectionChanged = OldValue as INotifyCollectionChanged;
			if (collectionChanged != null) collectionChanged.CollectionChanged -= CollectionChanged_CollectionChanged;
			collectionChanged = NewValue as INotifyCollectionChanged;
			if (collectionChanged != null) collectionChanged.CollectionChanged += CollectionChanged_CollectionChanged;
		}

		private void CollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			this.InvalidateArrange();
		}

		public static object GetSourceDevice(DependencyObject obj)
		{
			return obj.GetValue(SourceDeviceProperty);
		}

		public static void SetSourceDevice(DependencyObject obj, object value)
		{
			obj.SetValue(SourceDeviceProperty, value);
		}
		public static object GetDestinationDevice(DependencyObject obj)
		{
			return obj.GetValue(DestinationDeviceProperty);
		}
		public static void SetDestinationDevice(DependencyObject obj, object value)
		{
			obj.SetValue(DestinationDeviceProperty, value);
		}

		public static bool GetIsFlipped(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsFlippedProperty);
		}
		public static void SetIsFlipped(DependencyObject obj, bool value)
		{
			obj.SetValue(IsFlippedProperty, value);
		}









		protected override Size MeasureOverride(Size availableSize)
		{
			double height=0;

			foreach(UIElement element in Children)
			{
				element.Measure(new Size((double)ColumnWidth, availableSize.Height));
				height += element.DesiredSize.Height;
			}

			return new Size(width, height);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			object sourceDevice;
			object destinationDevice;
			int sourceColumn,destinationColumn;
			Rect rect;
			double x1,x2,y = 0;
			List<object> devices;

			if (Devices == null) devices=new List<object>();
			else devices = new List<object>(Devices);

			foreach (UIElement element in Children)
			{
				sourceDevice=GetSourceDevice(element);
				destinationDevice=GetDestinationDevice(element);

				sourceColumn = devices.IndexOf(sourceDevice);
				destinationColumn = devices.IndexOf(destinationDevice);

				if (sourceColumn>destinationColumn)
				{
					x1 = destinationColumn * ColumnWidth + ColumnWidth/2.0f;
					x2 = sourceColumn * ColumnWidth + +ColumnWidth / 2.0f;
					SetIsFlipped(element, true);
				}
				else
				{
					x1 = sourceColumn * ColumnWidth + ColumnWidth / 2.0f;
					x2 = destinationColumn * ColumnWidth + +ColumnWidth / 2.0f;
					SetIsFlipped(element, false);
				}

				rect = new Rect(x1, y, x2 - x1, element.DesiredSize.Height);
				element.Arrange(rect);

				y += element.RenderSize.Height;

			}
			return new Size(width, y);
		}

	}
}
