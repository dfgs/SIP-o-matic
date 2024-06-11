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
using System.Windows.Threading;
using System.Xml.Linq;

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

		private static RenderTargetBitmap? ControlToImage(UIElement? Header,UIElement? Content, double dpiX, double dpiY)
		{
			if ((Header == null) || (Content==null)) return null;
			
			Rect headerBounds = new Rect(0, 0, Header.DesiredSize.Width , Header.DesiredSize.Height);
			Rect contentBounds = new Rect(0, 0, Content.DesiredSize.Width , Content.DesiredSize.Height );

			Size headerSize = headerBounds.Size;
			Size contentSize = contentBounds.Size;

			Size targetSize = new Size(Math.Max(headerSize.Width,contentSize.Width),(headerSize.Height+contentSize.Height));
			Rect targetRect = new Rect(targetSize);

			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)targetRect.Width,(int)targetRect.Height,dpiX, dpiY,PixelFormats.Pbgra32);

			RenderTargetBitmap renderTargetBitmapHeader = new RenderTargetBitmap((int)headerBounds.Width, (int)headerBounds.Height, dpiX, dpiY, PixelFormats.Pbgra32);
			RenderTargetBitmap renderTargetBitmapContent = new RenderTargetBitmap((int)contentBounds.Width, (int)contentBounds.Height, dpiX, dpiY, PixelFormats.Pbgra32);

			renderTargetBitmapHeader.Render(Header);
			renderTargetBitmapContent.Render(Content);

			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext context = drawingVisual.RenderOpen())
			{
				//context.PushTransform(new ScaleTransform(ScaleX, ScaleY));
				context.DrawRectangle(Brushes.White, null, targetRect);

				context.DrawImage(renderTargetBitmapHeader, new Rect(0, 0, headerSize.Width, headerSize.Height));
				context.DrawImage(renderTargetBitmapContent, new Rect(0 , headerSize.Height, contentSize.Width , contentSize.Height));
			}

			renderTargetBitmap.Render(drawingVisual);
			return renderTargetBitmap;
		}

		

		private void CopyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
			e.Handled = true;
			e.ContinueRouting = true;
		}


		private async void CopyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			contentScrollViewer.ScrollToHome();
			headerScrollViewer.ScrollToHome();
			await Task.Delay(100); // async method needed in order to scroll content to home before clipboard capture
			
			RenderTargetBitmap? bmp = ControlToImage(header,content, 96,96);
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
