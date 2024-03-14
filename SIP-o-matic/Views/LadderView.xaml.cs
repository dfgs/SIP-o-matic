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
	/// Logique d'interaction pour LadderView.xaml
	/// </summary>
	public partial class LadderView : UserControl
	{


		public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register("Devices", typeof(IEnumerable<object>), typeof(LadderView), new PropertyMetadata(null));
		public IEnumerable<object> Devices
		{
			get { return (IEnumerable<string>)GetValue(DevicesProperty); }
			set { SetValue(DevicesProperty, value); }
		}



		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(LadderView), new PropertyMetadata(null));
		public object ItemsSource
		{
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}



		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LadderView), new PropertyMetadata(null));
		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty ItemToolTipProperty = DependencyProperty.Register("ItemToolTip", typeof(object), typeof(LadderView), new PropertyMetadata(null));
		public object ItemToolTip
		{
			get { return (object)GetValue(ItemToolTipProperty); }
			set { SetValue(ItemToolTipProperty, value); }
		}

		public LadderView()
		{
			InitializeComponent();
		}

		private void contentScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			headerScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

		private void contentScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			contentScrollViewer.ScrollToVerticalOffset(contentScrollViewer.VerticalOffset - e.Delta);
			e.Handled = true;
		}


	}
}
