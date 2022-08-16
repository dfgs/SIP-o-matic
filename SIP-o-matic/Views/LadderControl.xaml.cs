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
		private static readonly object[] defaultColumns = new object[] { "A", "B", "C" };
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(IEnumerable<object>), typeof(LadderControl), new FrameworkPropertyMetadata(defaultColumns, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)) ;
		public IEnumerable<object> Columns
		{
			get { return (IEnumerable<object>)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}
		
		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(IEnumerable<object>), typeof(LadderControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public IEnumerable<object> Rows
		{
			get { return (IEnumerable<object>)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}

		public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth", typeof(int), typeof(LadderControl), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public int ColumnWidth
		{
			get { return (int)GetValue(ColumnWidthProperty); }
			set { SetValue(ColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty ColumnPaddingProperty = DependencyProperty.Register("ColumnPadding", typeof(int), typeof(LadderControl), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public int ColumnPadding
		{
			get { return (int)GetValue(ColumnPaddingProperty); }
			set { SetValue(ColumnPaddingProperty, value); }
		}

		public LadderControl()
		{
			InitializeComponent();
		}
	}
}
