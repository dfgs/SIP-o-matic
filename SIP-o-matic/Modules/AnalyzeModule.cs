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
using SIPParserLib;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SIP_o_matic.corelib;
using System.Windows.Media.Animation;

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
		private ProjectViewModel _project;
		private KeyFrame? previousKeyFrame = null;
		private DateTime firstEvent;

		private List<string> legs;
		private List<string> callColors;
		private ColorManager callColorManager;
		private List<string> messageColors;
		private ColorManager messageColorManager;


		public AnalyzeModule(ILogger Logger, ProjectViewModel Project) : base(Logger)
		{
			ProgressStep step;

			if (Project == null) throw new ArgumentNullException(nameof(Project));
			this._project= Project;

			legs = new List<string>();
			callColors = new List<string>();
			callColorManager = new ColorManager();

			messageColors = new List<string>();
			messageColorManager = new ColorManager();


			Transactions = new List<Transaction>();

			progressSteps = new List<ProgressStep>();

			step = new ProgressStep() { Label = "Clean project", TaskFactory = CleanProjectAsync };
			step.MaximumGetter = () => 1;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Format messages frame", TaskFactory = FormatMessagesFrameAsync };
			step.MaximumGetter = () => 1;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Create key frames", TaskFactory = CreateKeyFramesAsync };
			step.MaximumGetter = () => _project.Messages.Count;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Format key frames", TaskFactory = FormatKeyFramesAsync };
			step.MaximumGetter = () => _project.KeyFrames.Count;
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
			_project.ClearKeyFrames();
			await Task.Delay(100);
		}

		private string GetLegName(string CallID, string SourceDevice, string DestinationDevice)
		{
			int index;
			string key;

			key = $"{CallID}/{Utils.Hash(SourceDevice, DestinationDevice)}";

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
		private string GetColor(string CallID, string ViaBranch,string CSeq)
		{
			int index;
			string key;

			key = CallID + "/" + ViaBranch+"/"+CSeq;

			index = messageColors.IndexOf(key);
			if (index == -1)
			{
				index = messageColors.Count;
				messageColors.Add(key);
			}

			return messageColorManager.GetColorString(index);
		}


		private Transaction CreateNewTransaction(IRequest Request,string SourceDevice, string DestinationDevice)
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
			

			switch (Request.Method)
			{
				case "INVITE": return new InviteTransaction(callID, viaBranch, cseq);
				case "ACK": return new AckTransaction(callID,  viaBranch, cseq);
				case "REFER": return new ReferTransaction(callID,  viaBranch, cseq);
				case "NOTIFY": return new NotifyTransaction(callID, viaBranch, cseq);
				case "BYE": return new ByeTransaction(callID,  viaBranch, cseq);
				default:
					string error = $"Failed to create new transaction: Invalid request method {Request.Method}";
					Log(LogLevels.Error, error);
					throw new NotImplementedException(error);
			}
		}
		
		private Call CreateNewCall(IRequest Request, string SourceDevice, string DestinationDevice)
		{
			string callID;
			string fromTag;
			Address from, to;

			LogEnter();

			callID = Request.GetCallID();
			fromTag = Request.GetFromTag();

			from = Request.GetFrom();
			to = Request.GetTo();

			return new Call(callID, SourceDevice, DestinationDevice,fromTag, from.ToHumanString()??"Undefined", to.ToHumanString() ?? "Undefined", Call.States.OnHook, false);
		}

		

		private async Task<bool> UpdateKeyFrameFromRequestAsync(KeyFrame KeyFrame, IRequest Request, string SourceDevice, string DestinationDevice, uint MessageIndex)
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

			
			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Request) && (Utils.Hash(SourceDevice, DestinationDevice) == Utils.Hash(item.SourceDevice, item.DestinationDevice)));
			if (call == null)
			{
				call = CreateNewCall(Request, SourceDevice, DestinationDevice);
				KeyFrame.Calls.Add(call);
			}


			// update transaction
			if (transaction.Update(Request,  MessageIndex))
			{
				// update call
				return call.Update(transaction);
			}
			else return false;

		}
		private async Task<bool> UpdateKeyFrameFromResponseAsync(KeyFrame KeyFrame, IResponse Response, string SourceDevice, string DestinationDevice, uint MessageIndex)
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

			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Response) && (Utils.Hash(SourceDevice,DestinationDevice)==Utils.Hash(item.SourceDevice,item.DestinationDevice )));
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

		private async Task<bool> UpdateKeyFrameAsync( KeyFrame KeyFrame, ProjectViewModel Project,  MessageViewModel Message)
		{

			LogEnter();
			
			
			if (Message.SIPMessage==null)
			{
				string error = "No parsed SIP message found";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			switch (Message.SIPMessage.MessageType)
			{
				case SIPMessageViewModel.Types.Request:return await UpdateKeyFrameFromRequestAsync(KeyFrame, Message.SIPMessage, Message.SourceDevice, Message.DestinationDevice, Message.Index);
				case SIPMessageViewModel.Types.Response:return await UpdateKeyFrameFromResponseAsync(KeyFrame, Message.SIPMessage, Message.SourceDevice, Message.DestinationDevice, Message.Index);
				default:
					string error = "Invalid SIP message type";
					Log(LogLevels.Error, error);
					throw new InvalidOperationException(error);
			}

		}

		private async Task CreateKeyFramesAsync(CancellationToken CancellationToken, int Index)
		{
			KeyFrame newKeyFrame;
			MessageViewModel message;


			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			message = _project.Messages[Index];


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

				if (await UpdateKeyFrameAsync(newKeyFrame,_project,  message))
				{
					_project.KeyFrames.Add(newKeyFrame);
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



		private async Task FormatKeyFrameAsync(CancellationToken CancellationToken,KeyFrameViewModel KeyFrame)
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


			await foreach (CallViewModel call in KeyFrame.Calls.ToAsyncEnumerable())
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

				call.MessageIndicesDescription = string.Join(',', call.MessageIndices.Select(index => $"[{index}]"));


			}
		}



		public async Task FormatKeyFramesAsync(CancellationToken CancellationToken, int Index)
		{
			KeyFrameViewModel keyFrame;

			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			if (_project.KeyFrames.Count == 0) return;

			keyFrame= _project.KeyFrames[Index];
			await FormatKeyFrameAsync(CancellationToken,  keyFrame);

		}




		private async Task FormatMessagesFrameAsync(CancellationToken CancellationToken, MessagesFrameViewModel MessagesFrame)
		{
			string callID;
			string viaBranch;
			string cseq;



			await foreach (MessageViewModel message in _project.Messages.ToAsyncEnumerable())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}

				Log(LogLevels.Debug, $"Formatting message\r\n{message.Content}");

				message.SourceDevice = _project.Devices.FindDeviceByAddress(message.SourceAddress)?.Name ?? message.SourceAddress;
				message.DestinationDevice = _project.Devices.FindDeviceByAddress(message.DestinationAddress)?.Name ?? message.DestinationAddress;
									

				callID = message.SIPMessage.GetCallID();
				viaBranch = message.SIPMessage.GetViaBranch();
				cseq = message.SIPMessage.GetCSeq();

				message.Color = GetColor(callID, viaBranch, cseq);

				MessagesFrame.Messages.Add(message);
			}

		}
		public async Task FormatMessagesFrameAsync(CancellationToken CancellationToken, int Index)
		{
			MessagesFrameViewModel messageFrame;

			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			if (_project.Messages.Count == 0) return;

			messageFrame = new MessagesFrameViewModel(_project.Logger);
			messageFrame.Load("");
			await FormatMessagesFrameAsync(CancellationToken, messageFrame);
			_project.MessagesFrame = messageFrame;

		}


	}
}
