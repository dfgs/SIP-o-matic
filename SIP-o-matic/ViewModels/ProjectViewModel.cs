using EthernetFrameReaderLib;
using LogLib;
using PcapngFile;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModel: ViewModel
	{


		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ProjectViewModel), new PropertyMetadata("new project"));
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectViewModel), new PropertyMetadata(null));
		public string Path
		{
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}

		public ObservableCollection<FileViewModel> Files
		{
			get;
			private set;
		}

		public static readonly DependencyProperty SelectedFileProperty = DependencyProperty.Register("SelectedFile", typeof(FileViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public FileViewModel? SelectedFile
		{
			get { return (FileViewModel)GetValue(SelectedFileProperty); }
			set { SetValue(SelectedFileProperty, value); }
		}

		public ObservableCollection<CallViewModel> Calls
		{
			get;
			private set;
		}

		public static readonly DependencyProperty SelectedCallProperty = DependencyProperty.Register("SelectedCall", typeof(CallViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public CallViewModel? SelectedCall
		{
			get { return (CallViewModel)GetValue(SelectedCallProperty); }
			set { SetValue(SelectedCallProperty, value); }
		}



		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			Files = new ObservableCollection<FileViewModel>();
			Calls = new ObservableCollection<CallViewModel>();
		}




		/*private SIPMessageViewModel CreateSIPMessageViewModel(Event Event)
		{
			SIPMessage sipMessage;

			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Event.Message, ' ');
			}
			catch (Exception ex)
			{
				return new InvalidMessageViewModel(Event, ex.Message);
			}
			switch (sipMessage)
			{
				case Request request: return new RequestViewModel(Event, request);
				case Response response:return new ResponseViewModel(Event, response);
				default: return new InvalidMessageViewModel(Event, $"Invalid message type {sipMessage.GetType()}");
			}
		}*/
		public CallViewModel? FindCallByUID(int UID)
		{
			return Calls.FirstOrDefault(item => item.UID == UID);
		}


		public void AddEvent(FileViewModel FileViewModel,Event Event)
		{
			CallViewModel? callViewModel;
			int callUID;
			SIPMessage sipMessage;

			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Event.Message, ' ');
			}
			catch (Exception ex)
			{
				Log(LogLevels.Error, ex.Message);
				return;
			}

			callUID = SIPUtils.GetCallUID(sipMessage);

			callViewModel=FindCallByUID(callUID);

			if (callViewModel == null)
			{
				callViewModel=new CallViewModel(Logger,callUID);
				Calls.Add(callViewModel);
			}
			callViewModel.AddSIPMessage(FileViewModel,Event, sipMessage);
		}

		
		public async Task AddFileAsync(string Path)
		{
			IDataSource dataSource;
			FileViewModel fileViewModel;

			fileViewModel = new FileViewModel(Logger);
			fileViewModel.Path = Path;
			Files.Add(fileViewModel);

			dataSource = new WiresharkDataSource(Path);
			await foreach(Event _event in dataSource.EnumerateEventsAsync())
			{
				fileViewModel.Events.Add(_event);
				AddEvent(fileViewModel, _event);
			}
		}
		public async Task RemoveFileAsync(FileViewModel File)
		{
			/*SIPMessageViewModel? sipMessageViewModel;
			int UID;

			Files.Remove(File);

			await foreach (Event _event in File.Events.ToAsyncEnumerable())
			{
				UID = _event.Message.GetHashCode();

				sipMessageViewModel = FindMessageByUID(UID);

				if (sipMessageViewModel != null)
				{
					sipMessageViewModel.RemoveSourceFile(File);
					if (sipMessageViewModel.Count == 0) SIPMessages.Remove(sipMessageViewModel);
					continue;
				}

				SelectedFile = null;
				Files.Remove(File);
				
			}*/
			await Task.Yield();
		}






	}
}
