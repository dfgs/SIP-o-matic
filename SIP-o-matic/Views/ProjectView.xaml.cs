﻿using System;
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
	/// Logique d'interaction pour ProjectView.xaml
	/// </summary>
	public partial class ProjectView : UserControl
	{
		public ProjectView()
		{
			InitializeComponent();
		}

		

		private void UserControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox? listBox;
			
			listBox = e.OriginalSource as ListBox;
			if (listBox == null) return;
			//detailView.DataContext = listBox.SelectedItem;
		}
	}
}
