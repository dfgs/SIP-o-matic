using SIP_o_matic.ViewModels;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
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


		public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register("SelectedText", typeof(string), typeof(SIPMessageView), new PropertyMetadata(null));
		public string SelectedText
		{
			get { return (string)GetValue(SelectedTextProperty); }
			private set { SetValue(SelectedTextProperty, value); }
		}

		public static readonly DependencyProperty HighLightsProperty = DependencyProperty.Register("HighLights", typeof(IEnumerable<KeyValuePair<string, string>>), typeof(SIPMessageView), new FrameworkPropertyMetadata(null, HighLightsChanged));
		public IEnumerable<KeyValuePair<string, string>> HighLights
		{
			get { return (IEnumerable<KeyValuePair<string, string>>)GetValue(HighLightsProperty); }
			set { SetValue(HighLightsProperty, value); }
		}




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
		private void rtb_SelectionChanged(object sender, RoutedEventArgs e)
		{
			SelectedText = rtb.Selection.Text;
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
			HighLightText();
		}

		private static void HighLightsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SIPMessageView? view;
			view = d as SIPMessageView;
			if (view == null) return;
			view.OnHighLightsChanged();
		}

		protected void OnHighLightsChanged()
		{
			HighLightText();
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
			if (Header is InvalidHeader) Write(Paragraph, Header.Name + ": ", "Red");
			else Write(Paragraph, Header.Name + ": ", "Blue");
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

		
		private void HighLightText()
		{

			if (HighLights == null) return;

			TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

			// clear backgrounds
			textRange.ApplyPropertyValue(TextElement.BackgroundProperty, null);

			foreach (KeyValuePair<string,string> keyValuePair in HighLights)
			{

				//get the rtbtextbox text
				string textBoxText = textRange.Text;


				
				//using regex to get the search count
				//this will include search word even it is part of another word
				//say we are searching "hi" in "hi, how are you Mahi?" --> match count will be 2 (hi in 'Mahi' also)

				Regex regex = new Regex(keyValuePair.Value);
				int count_MatchFound = Regex.Matches(textBoxText, regex.ToString()).Count;

				for (TextPointer startPointer = rtb.Document.ContentStart;
							startPointer.CompareTo(rtb.Document.ContentEnd) <= 0;
								startPointer = startPointer!.GetNextContextPosition(LogicalDirection.Forward))
				{
					//check if end of text
					if (startPointer.CompareTo(rtb.Document.ContentEnd) == 0)
					{
						break;
					}

					//get the adjacent string
					string parsedString = startPointer.GetTextInRun(LogicalDirection.Forward);

					//check if the search string present here
					int indexOfParseString = parsedString.IndexOf(keyValuePair.Value);

					if (indexOfParseString >= 0) //present
					{
						//setting up the pointer here at this matched index
						startPointer = startPointer.GetPositionAtOffset(indexOfParseString);

						if (startPointer != null)
						{
							//next pointer will be the length of the search string
							TextPointer nextPointer = startPointer.GetPositionAtOffset(keyValuePair.Value.Length);

							//create the text range
							TextRange searchedTextRange = new TextRange(startPointer, nextPointer);

							//color up 
							searchedTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString(keyValuePair.Key)));

						}
					}
				}
							
				
			}


		}


	}
}
