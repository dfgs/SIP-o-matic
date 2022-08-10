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

		private SIPMessageViewModel ParseSIPMessage(string Message)
		{
			SIPMessage sipMessage;
			int UID;

			UID = Message.GetHashCode();
			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Message, ' ');
			}
			catch (Exception ex)
			{
				return new InvalidMessageViewModel(UID,Message, ex.Message);
			}
			switch (sipMessage)
			{
				case Request request: return new RequestViewModel(UID, request);
				case Response response:return new ResponseViewModel(UID, response);
				default: return new InvalidMessageViewModel(UID,Message, $"Invalid message type {sipMessage.GetType()}");
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
			await foreach(string message in dataSource.EnumerateMessagesAsync())
			{
				fileViewModel.Messages.Add(message);
				UID = message.GetHashCode();

				sipMessageViewModel = FindMessageByUID(UID);

				if (sipMessageViewModel != null)
				{
					sipMessageViewModel.AddSourceFile(fileViewModel);
					continue;
				}
				else
				{
					sipMessageViewModel = ParseSIPMessage(message);
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

			await foreach (string message in File.Messages.ToAsyncEnumerable())
			{
				UID = message.GetHashCode();

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
