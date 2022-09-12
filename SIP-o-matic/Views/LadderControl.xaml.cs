using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SIP_o_matic.Views
{
	/// <summary>
	/// Logique d'interaction pour LadderControl.xaml
	/// </summary>
	public partial class LadderControl : UserControl
	{

		public event MouseEventHandler? RowDoubleClicked;

		private static readonly object[] defaultColumns = new object[] { "A", "B", "C" };
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(IEnumerable<object>), typeof(LadderControl)) ;
		public IEnumerable<object> Columns
		{
			get { return (IEnumerable<object>)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(IEnumerable<object>), typeof(LadderControl));
		public IEnumerable<object> Rows
		{
			get { return (IEnumerable<object>)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}
		public static readonly DependencyProperty SelectedRowProperty = DependencyProperty.Register("SelectedRow", typeof(object), typeof(LadderControl),new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public object SelectedRow
		{
			get { return GetValue(SelectedRowProperty); }
			set { SetValue(SelectedRowProperty, value); }
		}

		public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth", typeof(int), typeof(LadderControl));
		public int ColumnWidth
		{
			get { return (int)GetValue(ColumnWidthProperty); }
			set { SetValue(ColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(LadderControl));
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		
		public static readonly DependencyProperty LineTemplateProperty = DependencyProperty.Register("RowTemplate", typeof(DataTemplate), typeof(LadderControl));
		public DataTemplate RowTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty ColumnTemplateProperty = DependencyProperty.Register("ColumnTemplate", typeof(DataTemplate), typeof(LadderControl));
		public DataTemplate ColumnTemplate
		{
			get { return (DataTemplate)GetValue(ColumnTemplateProperty); }
			set { SetValue(ColumnTemplateProperty, value); }
		}



		public static readonly DependencyProperty RowContainerStyleProperty = DependencyProperty.Register("RowContainerStyle", typeof(Style), typeof(LadderControl));
		public Style RowContainerStyle
		{
			get { return (Style)GetValue(RowContainerStyleProperty); }
			set { SetValue(RowContainerStyleProperty, value); }
		}


		public static readonly DependencyProperty ColumnPaddingProperty = DependencyProperty.Register("ColumnPadding", typeof(int), typeof(LadderControl));
		public int ColumnPadding
		{
			get { return (int)GetValue(ColumnPaddingProperty); }
			set { SetValue(ColumnPaddingProperty, value); }
		}

		public LadderControl()
		{
			InitializeComponent();
		}

		private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (SelectedRow == null) return;
			RowDoubleClicked?.Invoke(this, e);

		}

		private void contentScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			headerScrollViewer.ScrollToHorizontalOffset(contentScrollViewer.HorizontalOffset);
		}

	}
}
