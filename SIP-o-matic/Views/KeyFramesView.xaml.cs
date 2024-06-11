using Microsoft.Office.Interop.PowerPoint;
using SIP_o_matic.ViewModels;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
using Office = Microsoft.Office.Core;
using PowerPoint=Microsoft.Office.Interop.PowerPoint;

namespace SIP_o_matic.Views
{
	/// <summary>
	/// Logique d'interaction pour KeyFramesView.xaml
	/// </summary>
	public partial class KeyFramesView : UserControl
	{
		public KeyFramesView()
		{
			InitializeComponent();
		}


		private static RenderTargetBitmap? ControlToImage(UIElement? target, double dpiX, double dpiY)
		{
			if (target == null) return null;
			// render control content
			Rect bounds = new Rect(0, 0, target.DesiredSize.Width, target.DesiredSize.Height);// VisualTreeHelper.GetDescendantBounds(target);
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
															(int)(bounds.Height * dpiY / 96.0),
														   dpiX, dpiY,
														   PixelFormats.Pbgra32);
			DrawingVisual dv = new DrawingVisual();
			using (DrawingContext ctx = dv.RenderOpen())
			{
				VisualBrush vb = new VisualBrush(target);
				ctx.DrawRectangle(vb, null, VisualTreeHelper.GetDescendantBounds(target));
			}
			rtb.Render(dv);
			return rtb;
		}

		private void ExportPPTCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = DataContext is KeyFrameViewModelCollection;
			e.Handled = true;
			e.ContinueRouting = true;
		}


		private async void ExportPPTCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			KeyFrameViewModelCollection? keyFrames;

			keyFrames = DataContext as KeyFrameViewModelCollection;
			if (keyFrames == null) return;

			await CreatePowerPointPresentationAsync(keyFrames);

		}

		private void ExportPPTCommandBinding_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = DataContext is KeyFrameViewModelCollection;
			e.Handled = true;
			e.ContinueRouting = true;
		}

		private async Task CreatePowerPointPresentationAsync(KeyFrameViewModelCollection KeyFrames)
		{
			PowerPoint.Application pptApplication;
			Presentations pptPresentations;
			Presentation pptPresentation;
			Slides slides;
			Slide slide;
			PowerPoint.Shape shape;
			KeyFrameViewModel keyFrame;
			RenderTargetBitmap? bmp;
			PngBitmapEncoder encoder;
			BitmapFrame frame;
			string tmpFile;
			string tmpFolder;

			

			try
			{
				tmpFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIP-o-matic");
				System.IO.Directory.CreateDirectory(tmpFolder);
				

				pptApplication = new PowerPoint.Application();
				// Create the Presentation File
				pptPresentations = pptApplication.Presentations;
				pptPresentation = pptPresentations.Add(Office.MsoTriState.msoTrue);
				PowerPoint.CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[PowerPoint.PpSlideLayout.ppLayoutText];
				slides = pptPresentation.Slides;

				for(int index=0;index<KeyFrames.Count;index++)
				{
					keyFrame = KeyFrames[index];
					KeyFrames.SelectedItem = keyFrame;
					await Task.Delay(100);

					bmp = ControlToImage(callsView, 96, 96);
					if (bmp == null) return;
					frame = BitmapFrame.Create(bmp);
					encoder = new PngBitmapEncoder();
					encoder.Frames.Add(frame);

					tmpFile = System.IO.Path.Combine(tmpFolder, $"tmp{index}.png");

					using (var stream = System.IO.File.Create(tmpFile))
					{
						encoder.Save(stream);
					}

					slide = slides.AddSlide(index+1, customLayout);
					slide.Shapes[1].TextFrame.TextRange.Text = $"Call flow message [{keyFrame.MessageIndex}]";
					
					shape = slide.Shapes.AddPicture(tmpFile, Office.MsoTriState.msoFalse,Office.MsoTriState.msoTrue,10,10);
				}
							

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			
		}



	}

}


