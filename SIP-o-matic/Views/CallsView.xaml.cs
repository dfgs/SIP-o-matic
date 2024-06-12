﻿using System;
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

		public async Task<RenderTargetBitmap?> CopyToImageAsync()
		{
			return await LadderView.CopyToImageAsync();
		}



	}
}
