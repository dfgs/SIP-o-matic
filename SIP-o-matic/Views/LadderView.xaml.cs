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
	/// Logique d'interaction pour LadderView.xaml
	/// </summary>
	public partial class LadderView : UserControl
	{



		


		public LadderView()
		{
			InitializeComponent();
		}

		private void RefreshCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			ProjectViewModel? projectViewModel;

			e.Handled = true; 

			projectViewModel = DataContext as ProjectViewModel;
			e.CanExecute =  (projectViewModel != null);

		}

		private void RefreshCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ProjectViewModel? projectViewModel;

			e.Handled = true;

			projectViewModel = DataContext as ProjectViewModel;
			if (projectViewModel == null) return;

			projectViewModel.Ladder?.Refresh(projectViewModel.Devices, projectViewModel.Calls);


		}
		private void ZoomInCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			ProjectViewModel? projectViewModel;

			e.Handled = true;

			projectViewModel = DataContext as ProjectViewModel;
			e.CanExecute = (projectViewModel?.Ladder?.SelectedEvent != null);
		}

		private void ZoomInCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ProjectViewModel? projectViewModel;

			e.Handled = true;

			projectViewModel = DataContext as ProjectViewModel;
			if (projectViewModel == null) return;

			projectViewModel.Ladder?.ZoomIn(projectViewModel.Devices, projectViewModel.Calls);
		}
		private void ZoomOutCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			ProjectViewModel? projectViewModel;

			e.Handled = true;

			projectViewModel = DataContext as ProjectViewModel;
			e.CanExecute = (projectViewModel?.Ladder?.SelectedEvent != null);
		}

		private void ZoomOutCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ProjectViewModel? projectViewModel;

			e.Handled = true;

			projectViewModel = DataContext as ProjectViewModel;
			if (projectViewModel == null) return;
			
			projectViewModel.Ladder?.ZoomOut(projectViewModel.Devices, projectViewModel.Calls);
		}

		private void LadderControl_RowDoubleClicked(object sender, MouseEventArgs e)
		{
			ProjectViewModel? projectViewModel;


			projectViewModel = DataContext as ProjectViewModel;
			if (projectViewModel == null) return;

			if (e.LeftButton==MouseButtonState.Pressed)
			{
				projectViewModel.Ladder?.ZoomIn(projectViewModel.Devices, projectViewModel.Calls);
			}
			else
			{
				projectViewModel.Ladder?.ZoomOut(projectViewModel.Devices, projectViewModel.Calls);
			}

		}
	}
}
