using LogLib;
using ModuleLib;
using ParserLib;
using PcapngFile;
using SIP_o_matic.corelib;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
using SIP_o_matic.DataSources;
using SIP_o_matic.ViewModels;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SIP_o_matic.Modules
{
	public class ModelAnalyzeModule : BaseProgressModule
	{
		private List<ProgressStep> progressSteps;
		public override IEnumerable<ProgressStep> ProgressSteps
		{
			get => progressSteps;
		}

		private Project project;

		public ModelAnalyzeModule(ILogger Logger, Project Project) : base(Logger)
		{
			ProgressStep step;


			if (Project == null) throw new ArgumentNullException(nameof(Project));

			this.project = Project;

			progressSteps = new List<ProgressStep>();

			step = new ProgressStep() { Label = "Clean model", TaskFactory = CleanModelAsync };
			step.MaximumGetter = () => 1;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Decode SIP messages", TaskFactory = DecodeSIPMessagesAsync };
			step.MaximumGetter = () => project.Messages.Count;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Decode SDP bodies", TaskFactory = DecodeSDPBodiesAsync };
			step.MaximumGetter = () => project.Messages.Count;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Create dialogs", TaskFactory = CreateDialogsAsync };
			step.MaximumGetter = () => project.Messages.Count;
			step.Init();
			progressSteps.Add(step);



		}

		private async Task DelayAsync(CancellationToken CancellationToken,  int Index)
		{
			if (CancellationToken.IsCancellationRequested) throw new Exception("Analysis canceled");
			await Task.Delay(1000);
		}
		private async Task CleanModelAsync(CancellationToken CancellationToken, int Index)
		{
			project.SIPMessages.Clear();
			await Task.Delay(1);
		}

		private async Task DecodeSIPMessagesAsync(CancellationToken CancellationToken, int Index)
		{
			SIPMessage SIPMessage;
			ParserLib.StringReader reader;
			string Content;

			Content = project.Messages[Index].Content;

			reader = new ParserLib.StringReader(Content, ' ');
			try
			{
				SIPMessage = SIPParserLib.SIPGrammar.SIPMessage.Parse(reader);
				project.SIPMessages.Add(SIPMessage);
			}
			catch (Exception ex)
			{
				string error = $"Failed to decode SIP message ({ex.Message})\r\r{Content}";
				Log(LogLevels.Error, error);
				project.SIPMessages.Add(new InvalidSIPMessage());
			}
			await Task.Delay(1);

		}
		private async Task CreateDialogsAsync(CancellationToken CancellationToken, int Index)
		{
			Message message;
			Dialog? dialog = null;
			ISIPMessage SIPMessage;


			message = project.Messages[Index];
			SIPMessage = project.SIPMessages[Index];

			if (!(SIPMessage is SIPMessage validSIPMessage)) return;

			if (!(validSIPMessage is Request request)) return;
			if (request.RequestLine.Method != "INVITE") return;

			dialog = project.Dialogs.FirstOrDefault(item => item.Match(validSIPMessage));
			if (dialog != null) return;

			dialog = new Dialog(message.Timestamp, validSIPMessage.GetCallID(), message.SourceAddress, message.DestinationAddress, validSIPMessage.GetFromTag(), validSIPMessage.GetToTag(), validSIPMessage.GetFrom().ToHumanString() ?? "Undefined", validSIPMessage.GetTo().ToHumanString() ?? "Undefined");
			project.Dialogs.Add(dialog!);

			await Task.Delay(1);

		}
		private async Task DecodeSDPBodiesAsync(CancellationToken CancellationToken, int Index)
		{
			ISIPMessage SIPMessage;
			StringReader reader;
			IBody SDP;

			SIPMessage = project.SIPMessages[Index];

			if (!(SIPMessage is SIPMessage validSIPMessage))
			{
				project.SDPBodies.Add(new EmptySDP());
				return;
			}

			if (string.IsNullOrEmpty(validSIPMessage.Body))
			{
				project.SDPBodies.Add(new EmptySDP());
				return;
			}


			reader = new StringReader(validSIPMessage.Body);
			try
			{
				SDP = SIPParserLib.SDPGrammar.SDP.Parse(reader);
				project.SDPBodies.Add(SDP);
			}
			catch (Exception ex)
			{
				string error = $"Failed to decode SDP body ({ex.Message})\r\r{validSIPMessage.Body}";
				Log(LogLevels.Error, error);
				project.SDPBodies.Add(new EmptySDP());
			}



			await Task.Delay(1);
			
		}

		


	}
}
