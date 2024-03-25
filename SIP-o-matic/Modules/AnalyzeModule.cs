using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLib;
using ModuleLib;
using ParserLib;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
using SIP_o_matic.corelib.Models.Transactions;
using SIP_o_matic.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SIP_o_matic.corelib;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using SIPParserLib;

namespace SIP_o_matic.Modules
{
	public class AnalyzeModule : BaseProgressModule
	{
		private List<ProgressStep> progressSteps;
		public override IEnumerable<ProgressStep> ProgressSteps
		{
			get => progressSteps;
		}

		private List<Transaction> Transactions;
		private Project project;
		private KeyFrame? previousKeyFrame = null;
		private DateTime firstEvent;

		private List<string> legs;
		private List<string> callColors;
		private ColorManager callColorManager;
		private List<string> transactionColors;
		private List<string> dialogColors;
		private ColorManager messageColorManager;


		public AnalyzeModule(ILogger Logger, Project Project) : base(Logger)
		{
			ProgressStep step;

			if (Project == null) throw new ArgumentNullException(nameof(Project));
			this.project= Project;

			legs = new List<string>();
			callColors = new List<string>();
			callColorManager = new ColorManager();

			transactionColors = new List<string>();
			dialogColors = new List<string>();
			messageColorManager = new ColorManager();


			Transactions = new List<Transaction>();

			progressSteps = new List<ProgressStep>();

			step = new ProgressStep() { Label = "Clean project", TaskFactory = CleanProjectAsync };
			step.MaximumGetter = () => 1;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Format messages frame", TaskFactory = FormatMessagesFrameAsync };
			step.MaximumGetter = () => project.Messages.Count;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Create RTP events", TaskFactory = CreateRTPEventsAsync };
			step.MaximumGetter = () => project.UDPStreams.Count;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Create key frames", TaskFactory = CreateKeyFramesAsync };
			step.MaximumGetter = () => project.Messages.Count;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Format key frames", TaskFactory = FormatKeyFramesAsync };
			step.MaximumGetter = () => project.KeyFrames.Count;
			step.Init();
			progressSteps.Add(step);


		}


		private async Task DelayAsync(CancellationToken CancellationToken, int Index)
		{
			if (CancellationToken.IsCancellationRequested) throw new Exception("Analysis canceled");
			await Task.Delay(1000);
		}
		private async Task CleanProjectAsync(CancellationToken CancellationToken, int Index)
		{
			if (CancellationToken.IsCancellationRequested) throw new Exception("Analysis canceled");
			project.KeyFrames.Clear();
			project.MessagesFrame = new EventsFrame();
			await Task.Delay(100);
		}

		private string GetLegName(string CallID, Device SourceDevice, Device DestinationDevice)
		{
			int index;
			string key;

			key = $"{CallID}/{Utils.Hash(SourceDevice.Name, DestinationDevice.Name)}";

			index = legs.IndexOf(key);
			if (index == -1)
			{
				index = legs.Count;
				legs.Add(key);
			}

			return $"L{index + 1}";

		}

		private string GetColor(string Caller, string Callee)
		{
			int index;
			string key;

			key = Caller + "/" + Callee;

			index = callColors.IndexOf(key);
			if (index == -1)
			{
				index = callColors.Count;
				callColors.Add(key);
			}

			return callColorManager.GetColorString(index);
		}
		private string GetTransactionColor(string CallID, string ViaBranch,string CSeq)
		{
			int index;
			string key;

			key = CallID + "/" + ViaBranch+"/"+CSeq;

			index = transactionColors.IndexOf(key);
			if (index == -1)
			{
				index = transactionColors.Count;
				transactionColors.Add(key);
			}

			return messageColorManager.GetColorString(index);
		}
		private string GetDialogColor(string CallID, string FromTag)
		{
			int index;
			string key;

			key = CallID + "/" + FromTag ;

			index = dialogColors.IndexOf(key);
			if (index == -1)
			{
				index = dialogColors.Count;
				dialogColors.Add(key);
			}

			return messageColorManager.GetColorString(index);
		}

		private Transaction CreateNewTransaction(Request Request, Device SourceDevice, Device DestinationDevice)
		{
			string viaBranch;
			string cseq;
			string callID;
			//string fromTag;

			LogEnter();

			callID = Request.GetCallID();
			viaBranch = Request.GetViaBranch();
			cseq = Request.GetCSeq();
			//fromTag=Request.GetFromTag();
			

			switch (Request.RequestLine.Method)
			{
				case "INVITE": return new InviteTransaction(callID, viaBranch, cseq);
				case "ACK": return new AckTransaction(callID,  viaBranch, cseq);
				case "REFER": return new ReferTransaction(callID,  viaBranch, cseq);
				case "NOTIFY": return new NotifyTransaction(callID, viaBranch, cseq);
				case "BYE": return new ByeTransaction(callID, viaBranch, cseq);
				case "OPTIONS": return new OptionsTransaction(callID, viaBranch, cseq);
				case "REGISTER": return new RegisterTransaction(callID, viaBranch, cseq);
				case "MESSAGE": return new MessageTransaction(callID, viaBranch, cseq);
				case "CANCEL": return new CancelTransaction(callID, viaBranch, cseq);
				case "SUBSCRIBE": return new SubscribeTransaction(callID, viaBranch, cseq);
				case "UPDATE": return new UpdateTransaction(callID, viaBranch, cseq);
				default:
					string error = $"Failed to create new transaction: Invalid request method {Request.RequestLine.Method}";
					Log(LogLevels.Error, error);
					throw new NotImplementedException(error);
			}
		}
		
		private Call CreateNewCall(Request Request, Device SourceDevice, Device DestinationDevice)
		{
			string callID;
			string fromTag;
			SIPParserLib.Address from, to;

			LogEnter();

			callID = Request.GetCallID();
			fromTag = Request.GetFromTag();

			from = Request.GetFrom();
			to = Request.GetTo();

			return new Call(callID, SourceDevice, DestinationDevice,fromTag, from.ToHumanString()??"Undefined", to.ToHumanString() ?? "Undefined", Call.States.OnHook, false);
		}

		

		private async Task<bool> UpdateKeyFrameFromRequestAsync(KeyFrame KeyFrame, Request Request, Device SourceDevice, Device DestinationDevice, uint MessageIndex)
		{
			Call? call;
			Transaction? transaction;


			LogEnter();

			await Task.Delay(1);

			transaction = Transactions.FirstOrDefault(item => item.Match(Request));
			if (transaction == null)
			{
				// check if all other transactions are terminated
				transaction = CreateNewTransaction(Request,SourceDevice,DestinationDevice);
				Transactions.Add(transaction);
			}


			// update transaction
			if (transaction.Update(Request,  MessageIndex))
			{
				if ((transaction is OptionsTransaction)
					|| (transaction is RegisterTransaction)
					|| (transaction is MessageTransaction)
					|| (transaction is SubscribeTransaction)
				) return false;// ignore options transactions

				call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Request) && (Utils.Hash(SourceDevice.Name, DestinationDevice.Name) == Utils.Hash(item.SourceDevice.Name, item.DestinationDevice.Name)));
				if (call == null)
				{
					call = CreateNewCall(Request, SourceDevice, DestinationDevice);
					KeyFrame.Calls.Add(call);
				}
				// update call
				return call.Update(transaction);
			}
			else return false;

		}
		private async Task<bool> UpdateKeyFrameFromResponseAsync(KeyFrame KeyFrame, Response Response, Device SourceDevice, Device DestinationDevice, uint MessageIndex)
		{
			Call? call;
			Transaction? transaction;
			string? toTag;

			LogEnter();

			await Task.Delay(1);


			transaction = Transactions.FirstOrDefault(item => item.Match(Response));
			if (transaction == null)
			{
				string error = "Cannot find matching transaction for response, possibly incomplete log file";
				Log(LogLevels.Warning, error);
				return false;
			}

			if ((transaction is OptionsTransaction)
				|| (transaction is RegisterTransaction)
				|| (transaction is MessageTransaction)
				|| (transaction is SubscribeTransaction)
				) return false;// ignore options transactions

			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Response) && (Utils.Hash(SourceDevice.Name,DestinationDevice.Name) ==Utils.Hash(item.SourceDevice.Name,item.DestinationDevice.Name )));
			if (call == null)
			{
				string error = "Cannot find matching call for response";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			toTag = Response.GetToTag();
			if (toTag!=null) call.ToTag = toTag;

			// update transaction
			if (transaction.Update(Response,  MessageIndex))
			{
				// update call
				return call.Update(transaction);
			}
			else return false;

		}

		private async Task<bool> UpdateKeyFrameAsync( KeyFrame KeyFrame, Project Project,  Message Message,SIPMessage SIPMessage)
		{

			LogEnter();
			
			
			if (SIPMessage==null)
			{
				string error = "No parsed SIP message found";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}
			

			switch (SIPMessage)
			{
				case Request request:return await UpdateKeyFrameFromRequestAsync(KeyFrame, request, project.GetDevice(Message.SourceAddress), project.GetDevice(Message.DestinationAddress), Message.Index);
				case Response response:return await UpdateKeyFrameFromResponseAsync(KeyFrame, response, project.GetDevice(Message.SourceAddress), project.GetDevice(Message.DestinationAddress), Message.Index);
				default:
					string error = "Invalid SIP message type";
					Log(LogLevels.Error, error);
					throw new InvalidOperationException(error);
			}

		}

		private async Task CreateKeyFramesAsync(CancellationToken CancellationToken, int Index)
		{
			KeyFrame newKeyFrame;
			Message message;
			ISIPMessage SIPMessage;
			Dialog? dialog;

			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			message = project.Messages[Index];
			SIPMessage = project.SIPMessages[Index];
			
			if (!(SIPMessage is SIPMessage validSIPMessage)) return;



			dialog = project.Dialogs.FirstOrDefault(item=>item.IsChecked && item.Match(validSIPMessage));
			if (dialog==null) return;	// filer only selected dialogs


			try
			{
				if (previousKeyFrame == null)
				{
					newKeyFrame = new KeyFrame(message.Timestamp);
					firstEvent = message.Timestamp;
				}
				else
				{
					newKeyFrame = previousKeyFrame.Clone();
					newKeyFrame.Timestamp = message.Timestamp;
				}

				newKeyFrame.MessageIndex = message.Index;

				if (await UpdateKeyFrameAsync(newKeyFrame,project,  message, validSIPMessage))
				{
					project.KeyFrames.Add( newKeyFrame);
				}

				previousKeyFrame = newKeyFrame;

			}
			catch (Exception ex)
			{
				string error = $"Failed to create key frame:\r\n" + ex.Message + $"\r\n[{message.Index}] " + message.Content;
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

		}



		private async Task FormatKeyFrameAsync(CancellationToken CancellationToken,KeyFrame KeyFrame)
		{

			KeyFrame.TimeSpan = KeyFrame.Timestamp - firstEvent;
			if (KeyFrame.TimeSpan.TotalSeconds < 1)
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Milliseconds}ms";
			}
			else if (KeyFrame.TimeSpan.TotalMinutes < 1)
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Seconds}.{KeyFrame.TimeSpan.Milliseconds}s";
			}
			else if (KeyFrame.TimeSpan.TotalHours < 1)
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Minutes}m{KeyFrame.TimeSpan.Seconds}.{KeyFrame.TimeSpan.Milliseconds}s";
			}
			else
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Hours}h{KeyFrame.TimeSpan.Minutes}m{KeyFrame.TimeSpan.Seconds}.{KeyFrame.TimeSpan.Milliseconds}s";
			}


			await foreach (Call call in KeyFrame.Calls.ToAsyncEnumerable())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}
				call.LegName = GetLegName(call.CallID, call.SourceDevice, call.DestinationDevice);

				if (call.ReplacedCallID == null) call.LegDescription = call.LegName;
				else call.LegDescription = $"{call.LegName} (replaces {GetLegName(call.ReplacedCallID, call.SourceDevice, call.DestinationDevice)})";

				call.Color = GetColor(call.Caller, call.Callee);
			}
		}



		public async Task FormatKeyFramesAsync(CancellationToken CancellationToken, int Index)
		{
			KeyFrame keyFrame;

			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			if (project.KeyFrames.Count == 0) return;

			keyFrame= project.KeyFrames[Index];
			await FormatKeyFrameAsync(CancellationToken,  keyFrame);

		}





		public async Task FormatMessagesFrameAsync(CancellationToken CancellationToken, int Index)
		{
			string callID;
			string viaBranch;
			string cseq;
			string fromTag;
			Dialog? dialog;
			Message message;
			ISIPMessage SIPMessage;
			Device sourceDevice, destinationDevice;

			message = project.Messages[Index];
			SIPMessage = project.SIPMessages[Index];
			if (!(SIPMessage is SIPMessage validSIPMessage)) return;

			dialog = project.Dialogs.FirstOrDefault(item => item.IsChecked && item.Match(validSIPMessage));
			if (dialog == null) return;// filer only selected dialogs

			Log(LogLevels.Debug, $"Formatting message\r\n{message.Content}");

			callID = validSIPMessage.GetCallID();
			viaBranch = validSIPMessage.GetViaBranch();
			cseq = validSIPMessage.GetCSeq();
			fromTag = validSIPMessage.GetFromTag();

			message.TransactionColor = GetTransactionColor(callID, viaBranch, cseq);
			message.DialogColor = GetDialogColor(callID, fromTag);

			project.MessagesFrame.Events.Add(message);

			sourceDevice = project.GetDevice(message.SourceAddress);
			destinationDevice = project.GetDevice(message.DestinationAddress);

			if (!project.MessagesFrame.Devices.Contains(sourceDevice)) project.MessagesFrame.Devices.Add(sourceDevice);
			if (!project.MessagesFrame.Devices.Contains(destinationDevice)) project.MessagesFrame.Devices.Add(destinationDevice);


			await Task.Delay(1);
		}



		private async Task CreateRTPEventsAsync(CancellationToken CancellationToken, int Index)
		{
			UDPStream stream;
			ISIPMessage SIPMessage;
			Dialog? dialog;
			IBody body;
			ConnectionField? connectionField;
			MediaField? mediaField;
			RTPStart rtpStart;

			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			stream= project.UDPStreams[Index];

			
			for(int t=0;t<project.Messages.Count;t++)
			{
				body = project.SDPBodies[t];
				if (!(body is SDP validSDP)) return;

				SIPMessage = project.SIPMessages[t];
				if (!(SIPMessage is SIPMessage validSIPMessage)) return;

				dialog = project.Dialogs.FirstOrDefault(item => item.IsChecked && item.Match(validSIPMessage));
				if (dialog == null) continue; // filer only selected dialogs//*/

				connectionField = validSDP.GetField<ConnectionField>();
				if (connectionField == null) continue;

				mediaField = validSDP.GetField<MediaField>();
				if (mediaField == null) continue;

				if ( (stream.DestinationPort==mediaField.Port) && (stream.DestinationAddress.ToString()==connectionField.Address) )
				{
					rtpStart = new RTPStart(stream.Timestamp,stream.SourceAddress,stream.DestinationAddress );
					project.MessagesFrame.Events.Add(rtpStart);

				}

			}

			await Task.Delay(1);




		}



	}
}
