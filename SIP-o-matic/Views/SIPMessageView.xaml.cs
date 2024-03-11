using SIP_o_matic.ViewModels;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
	/// Logique d'interaction pour SIPMessageView.xaml
	/// </summary>
	public partial class SIPMessageView : UserControl
	{


		public static readonly DependencyProperty SIPMessageProperty = DependencyProperty.Register("SIPMessage", typeof(SIPMessageViewModel), typeof(SIPMessageView), new FrameworkPropertyMetadata(null,SIPMessagePropertyChanged));

		public SIPMessageViewModel SIPMessage
		{
			get { return (SIPMessageViewModel)GetValue(SIPMessageProperty); }
			set { SetValue(SIPMessageProperty, value); }
		}

		public SIPMessageView()
		{
			InitializeComponent();
		}

		private static void SIPMessagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SIPMessageView? view;
			view = d as SIPMessageView;
			if (view == null) return;
			view.OnSIPMessagePropertyChanged();
		}

		protected void OnSIPMessagePropertyChanged()
		{
			rtb.Document = CreateDocument();
		}


		private void Write(Paragraph Paragraph, string Text, string Color)
		{
			Run run = new Run(Text);
			run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color));
			Paragraph.Inlines.Add(run);
		}

		private void WriteLine(Paragraph Paragraph,string Text, string Color)
		{
			Write(Paragraph, Text+"\r\n", Color);		
		}

		private void WriteHeader(Paragraph Paragraph, MessageHeader Header)
		{
			Write(Paragraph, Header.Name + ": ", "Blue");
			WriteLine(Paragraph, Header.GetStringValue(), "Black");
		}
		private void WriteField(Paragraph Paragraph, SDPField Field)
		{
			Write(Paragraph, Field.Name + ": ", "YellowGreen");
			WriteLine(Paragraph, Field.DisplayValue, "Black");
		}
		private FlowDocument CreateDocument()
		{
			FlowDocument document = new FlowDocument();
			Paragraph paragraph = new Paragraph();

			if (SIPMessage == null) return document;

			WriteLine(paragraph, SIPMessage.Description, "Orchid");
			foreach(MessageHeader header in SIPMessage.Headers)
			{
				WriteHeader(paragraph, header);
			}
			if (SIPMessage.SDP!=null)
			{
				WriteLine(paragraph, "", "Black");
				foreach(SDPField field in SIPMessage.SDP.Fields)
				{
					WriteField(paragraph, field);
				}
			}

			document.Blocks.Add(paragraph);
			return document;
		}


	}
}
