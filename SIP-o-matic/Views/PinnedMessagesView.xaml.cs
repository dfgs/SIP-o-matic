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
		public PinnedMessagesView()
		{
			InitializeComponent();
		}

		private void PinMessageCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;e.Handled = false;
        }

		private void PinMessaggeCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MessagesFrameViewModel? frame;
			MessageViewModel? message;

			frame = DataContext as MessagesFrameViewModel;
			if (frame == null) return;

			message = e.Parameter as MessageViewModel;
			if (message == null) return;

			frame.PinMessage(message);
		}
	}
}
