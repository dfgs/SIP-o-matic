using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	/// Logique d'interaction pour CallsView.xaml
	/// </summary>
	public partial class CallsView : UserControl
	{
		public CallsView()
		{
			InitializeComponent();
		}

		

		private static RenderTargetBitmap? ControlToImage(Visual target, double dpiX, double dpiY)
		{
			if (target == null) return null;
			// render control content
			Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
															(int)(bounds.Height * dpiY / 96.0),
														   dpiX, dpiY,
														   PixelFormats.Pbgra32);
			DrawingVisual dv = new DrawingVisual();
			using (DrawingContext ctx = dv.RenderOpen())
			{
				VisualBrush vb = new VisualBrush(target);
				ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
			}
			rtb.Render(dv);
			return rtb;
		}

		private void CopyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute= true;
			e.Handled= true;
			e.ContinueRouting= true;
        }


		private void CopyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			RenderTargetBitmap? bmp = ControlToImage(this, 96,96);
			if (bmp == null) return;
			Clipboard.SetImage(bmp);

		}

		private void CopyCommandBinding_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
			e.Handled = true;
			e.ContinueRouting = true;
		}

	}
}
