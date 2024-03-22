using SIP_o_matic.ViewModels;
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
	/// Logique d'interaction pour PinnedMessagesView.xaml
	/// </summary>
	public partial class PinnedMessagesView : UserControl
	{
		private Dictionary<string, string> highlights;



		public static readonly DependencyProperty HighLightsProperty = DependencyProperty.Register("HighLights", typeof(IEnumerable<KeyValuePair<string, string>>), typeof(PinnedMessagesView), new PropertyMetadata(null));
		public IEnumerable<KeyValuePair<string,string>> HighLights
		{
			get { return (IEnumerable<KeyValuePair<string,string>>)GetValue(HighLightsProperty); }
			set { SetValue(HighLightsProperty, value); }
		}




		public PinnedMessagesView()
		{
			highlights = new Dictionary<string, string>();
			InitializeComponent();
		}

		private void PinMessageCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;e.Handled = false;
        }

		private void PinMessageCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			EventsFrameViewModel? frame;
			MessageViewModel? message;

			frame = DataContext as EventsFrameViewModel;
			if (frame == null) return;

			message = e.Parameter as MessageViewModel;
			if (message == null) return;

			frame.PinMessage(message);
		}

		private void HighlightSelectionCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;e.CanExecute = true;
		}

		private void HighlightSelectionCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string? color;
			string? selectedText;

			color = e.Parameter as string;
			if (color == null) return;

			if (color=="Transparent")
			{
				highlights.Clear();
				this.HighLights = highlights.ToArray();
				return;
			}

			selectedText = sipMessageViewSelection.SelectedText;
			if (string.IsNullOrEmpty(selectedText))
			{
				highlights.Remove(color);
			}
			else
			{

				if (highlights.ContainsKey(color))
				{
					highlights[color] = selectedText;
				}
				else
				{
					highlights.Add(color, selectedText);
				}
			}
			this.HighLights = highlights.ToArray();
		}


	}
}
