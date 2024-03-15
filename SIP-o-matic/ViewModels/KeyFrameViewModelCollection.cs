using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class KeyFrameViewModelCollection : GenericViewModelList<KeyFrame, KeyFrameViewModel>
	{



		public static readonly DependencyProperty MoveToStartCommandProperty = DependencyProperty.Register("MoveToStartCommand", typeof(ViewModelCommand), typeof(KeyFrameViewModelCollection), new PropertyMetadata(null));
		public ViewModelCommand MoveToStartCommand
		{
			get { return (ViewModelCommand)GetValue(MoveToStartCommandProperty); }
			set { SetValue(MoveToStartCommandProperty, value); }
		}


		public static readonly DependencyProperty MoveToEndCommandProperty = DependencyProperty.Register("MoveToEndCommand", typeof(ViewModelCommand), typeof(KeyFrameViewModelCollection), new PropertyMetadata(null));
		public ViewModelCommand MoveToEndCommand
		{
			get { return (ViewModelCommand)GetValue(MoveToEndCommandProperty); }
			set { SetValue(MoveToEndCommandProperty, value); }
		}


		public static readonly DependencyProperty MoveToPreviousCommandProperty = DependencyProperty.Register("MoveToPreviousCommand", typeof(ViewModelCommand), typeof(KeyFrameViewModelCollection), new PropertyMetadata(null));
		public ViewModelCommand MoveToPreviousCommand
		{
			get { return (ViewModelCommand)GetValue(MoveToPreviousCommandProperty); }
			set { SetValue(MoveToPreviousCommandProperty, value); }
		}

		public static readonly DependencyProperty MoveToNextCommandProperty = DependencyProperty.Register("MoveToNextCommand", typeof(ViewModelCommand), typeof(KeyFrameViewModelCollection), new PropertyMetadata(null));
		public ViewModelCommand MoveToNextCommand
		{
			get { return (ViewModelCommand)GetValue(MoveToNextCommandProperty); }
			set { SetValue(MoveToNextCommandProperty, value); }
		}

		private IDeviceNameProvider deviceNameProvider;

		public KeyFrameViewModelCollection(IList<KeyFrame> Source, IDeviceNameProvider DeviceNameProvider) : base(Source, (SourceItem) => new KeyFrameViewModel(SourceItem, DeviceNameProvider))
		{
			this.deviceNameProvider = DeviceNameProvider;
			MoveToStartCommand = new ViewModelCommand(MoveToStartCanExecute, MoveToStartExecuted);
			MoveToEndCommand = new ViewModelCommand(MoveToEndCanExecute, MoveToEndExecuted);
			MoveToNextCommand = new ViewModelCommand(MoveToNextCanExecute, MoveToNextExecuted);
			MoveToPreviousCommand = new ViewModelCommand(MoveToPreviousCanExecute, MoveToPreviousExecuted);
		}

		private bool MoveToStartCanExecute(object? arg)
		{
			return Count>0;
		}

		private void MoveToStartExecuted(object? obj)
		{
			SelectedItem = this[0];
		}

		private bool MoveToEndCanExecute(object? arg)
		{
			return Count > 0;
		}

		private void MoveToEndExecuted(object? obj)
		{
			SelectedItem = this[Count-1];
		}

		private bool MoveToPreviousCanExecute(object? arg)
		{
			if (SelectedItem==null) return false;
			return (Count > 0) && (IndexOf(SelectedItem)>0);
		}

		private void MoveToPreviousExecuted(object? obj)
		{
			int index;
			if (SelectedItem == null) return;
			index=IndexOf(SelectedItem);
			SelectedItem = this[index-1];
		}
		private bool MoveToNextCanExecute(object? arg)
		{
			if (SelectedItem == null) return false;
			return (Count > 0) && (IndexOf(SelectedItem) < Count-1);
		}

		private void MoveToNextExecuted(object? obj)
		{
			int index;
			if (SelectedItem == null) return;
			index = IndexOf(SelectedItem);
			SelectedItem = this[index + 1];
		}






		

		
		

		internal static KeyFrameViewModelCollection CreateTestData()
		{
			KeyFrameViewModel keyFrame;
			KeyFrameViewModelCollection keyFrames;

			keyFrames = new KeyFrameViewModelCollection(new List<KeyFrame>(),null);
			keyFrame = KeyFrameViewModel.CreateTestData();
			keyFrames.AddInternal(keyFrame);

			return keyFrames;
		}

	}
}
