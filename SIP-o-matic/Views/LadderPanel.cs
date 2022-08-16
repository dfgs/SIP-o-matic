using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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



		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(IEnumerable<object>), typeof(LadderPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,ColumnsPropertyChanged));
		public IEnumerable<object> Columns
		{
			get { return (IEnumerable<object>)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}


		public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth", typeof(int), typeof(LadderPanel), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public int ColumnWidth
		{
			get { return (int)GetValue(ColumnWidthProperty); }
			set { SetValue(ColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty ColumnPaddingProperty = DependencyProperty.Register("ColumnPadding", typeof(int), typeof(LadderPanel), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public int ColumnPadding
		{
			get { return (int)GetValue(ColumnPaddingProperty); }
			set { SetValue(ColumnPaddingProperty, value); }
		}

		public static readonly DependencyProperty LeftColumnProperty = DependencyProperty.RegisterAttached("LeftColumn", typeof(object), typeof(LadderPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		public static readonly DependencyProperty RightColumnProperty = DependencyProperty.RegisterAttached("RightColumn", typeof(object), typeof(LadderPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));






		public LadderPanel()
		{
		}




		public static object GetLeftColumn(DependencyObject obj)
		{
			return (object)obj.GetValue(LeftColumnProperty);
		}

		public static void SetLeftColumn(DependencyObject obj, object value)
		{
			obj.SetValue(LeftColumnProperty, value);
		}

		public static object GetRightColumn(DependencyObject obj)
		{
			return (object)obj.GetValue(RightColumnProperty);
		}

		public static void SetRightColumn(DependencyObject obj, object value)
		{
			obj.SetValue(RightColumnProperty, value);
		}

		private static void ColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LadderPanel? panel;

			panel = d as LadderPanel;
			if (panel != null) panel.OnColumnsPropertyChanged(e);
		}

		protected void OnColumnsPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			INotifyCollectionChanged? notifyCollection;

			notifyCollection = e.OldValue as INotifyCollectionChanged;
			if (notifyCollection != null) notifyCollection.CollectionChanged -= NotifyCollection_CollectionChanged;

			notifyCollection = e.NewValue as INotifyCollectionChanged;
			if (notifyCollection != null) notifyCollection.CollectionChanged += NotifyCollection_CollectionChanged;
		}

		private void NotifyCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidateVisual();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			List<object> columns;
			object? leftColumn;
			object? rightColumn;
			double totalHeight =0;
			int leftIndex, rightIndex;

			columns = new List<object>();
			if (Columns != null) columns.AddRange(Columns);

			foreach (UIElement element in Children)
			{
				leftColumn = GetLeftColumn(element);
				if (!columns.Contains(leftColumn)) columns.Add(leftColumn);
				leftIndex = columns.IndexOf(leftColumn);

				rightColumn = GetRightColumn(element);
				if (!columns.Contains(rightColumn)) columns.Add(rightColumn);
				rightIndex = columns.IndexOf(rightColumn);

				if (leftIndex > rightIndex)
				{
					int tmp = rightIndex;
					rightIndex = leftIndex;
					leftIndex = tmp;
				}

				element.Measure(new Size((rightIndex - leftIndex) * ColumnWidth, availableSize.Height));
				totalHeight += element.DesiredSize.Height;
			}

			return new Size((columns.Count-1) * ColumnWidth+2*ColumnPadding, totalHeight);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			List<object> columns;
			object? leftColumn;
			object? rightColumn;
			double totalHeight = 0;
			int leftIndex,rightIndex;

			columns = new List<object>();
			if (Columns != null) columns.AddRange(Columns);

			foreach (UIElement element in Children)
			{
				leftColumn = GetLeftColumn(element);
				if (!columns.Contains(leftColumn)) columns.Add(leftColumn);
				leftIndex = columns.IndexOf(leftColumn);
				
				rightColumn = GetRightColumn(element);
				if (!columns.Contains(rightColumn)) columns.Add(rightColumn);
				rightIndex = columns.IndexOf(rightColumn);

				if (leftIndex>rightIndex)
				{
					int tmp = rightIndex;
					rightIndex = leftIndex;
					leftIndex = tmp;
				}

				element.Arrange(new Rect(leftIndex*ColumnWidth+ColumnPadding, totalHeight, (rightIndex-leftIndex)*ColumnWidth, element.DesiredSize.Height));

				totalHeight += element.DesiredSize.Height;

			}

			return new Size((columns.Count - 1) * ColumnWidth + 2 * ColumnPadding, totalHeight);
			

		}


	}
}
