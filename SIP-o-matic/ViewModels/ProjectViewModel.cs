using EthernetFrameReaderLib;
using PcapngFile;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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



		public ObservableCollection<SIPMessageViewModel> SIPMessages
		{
			get;
			private set;
		}


		public ProjectViewModel()
		{
			Files = new ObservableCollection<FileViewModel>();
			SIPMessages = new ObservableCollection<SIPMessageViewModel>();
		}


		public SIPMessageViewModel? FindMessageByUID(int UID)
		{
			return SIPMessages.FirstOrDefault(item => item.UID == UID);
		}

		private SIPMessageViewModel CreateSIPMessageViewModel(int UID,Event Event)
		{
			SIPMessage sipMessage;

			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Event.Message, ' ');
			}
			catch (Exception ex)
			{
				return new InvalidMessageViewModel(UID,Event.Timestamp, Event.Message, ex.Message);
			}
			switch (sipMessage)
			{
				case Request request: return new RequestViewModel(UID,Event.Timestamp, request);
				case Response response:return new ResponseViewModel(UID,Event.Timestamp, response);
				default: return new InvalidMessageViewModel(UID,Event.Timestamp, Event.Message, $"Invalid message type {sipMessage.GetType()}");
			}
		}

		public async Task AddFileAsync(string Path)
		{
			IDataSource dataSource;
			FileViewModel fileViewModel;
			SIPMessageViewModel? sipMessageViewModel;
			int UID;

			fileViewModel = new FileViewModel();
			fileViewModel.Path = Path;
			Files.Add(fileViewModel);

			dataSource = new WiresharkDataSource(Path);
			await foreach(Event _event in dataSource.EnumerateEventsAsync())
			{
				fileViewModel.Events.Add(_event);
				UID = _event.Message.GetHashCode();

				sipMessageViewModel = FindMessageByUID(UID);

				if (sipMessageViewModel != null)
				{
					sipMessageViewModel.AddSourceFile(fileViewModel);
					continue;
				}
				else
				{
					sipMessageViewModel = CreateSIPMessageViewModel(UID,_event);
					sipMessageViewModel.AddSourceFile(fileViewModel);
					SIPMessages.Add(sipMessageViewModel);
				}
			}
		}
		public async Task RemoveFileAsync(FileViewModel File)
		{
			SIPMessageViewModel? sipMessageViewModel;
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
				
			}
		}






	}
}
