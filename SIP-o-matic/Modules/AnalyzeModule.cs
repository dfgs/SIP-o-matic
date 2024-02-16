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
using SIP_o_matic.DataSources;
using SIP_o_matic.Models;
using SIP_o_matic.Models.Transactions;
using SIP_o_matic.ViewModels;
using SIPParserLib;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		private List<string> colors;
		private ColorManager colorManager;


		public AnalyzeModule(ILogger Logger, ProjectViewModel Project) : base(Logger)
		{
			ProgressStep step;

			if (Project == null) throw new ArgumentNullException(nameof(Project));
			this._project= Project;

			legs = new List<string>();
			colors = new List<string>();
			colorManager = new ColorManager(10);


			Transactions = new List<Transaction>();

			progressSteps = new List<ProgressStep>();

			step = new ProgressStep() { Label = "Clean project", TaskFactory = CleanProjectAsync };
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

			index = colors.IndexOf(key);
			if (index == -1)
			{
				index = colors.Count;
				colors.Add(key);
			}

			return colorManager.GetColorString(index);


		}


		private Transaction CreateNewTransaction(Request Request,string SourceDevice, string DestinationDevice)
		{
			string viaBranch;
			string cseq;
			string callID;
			string fromTag;

			LogEnter();

			callID = Request.GetCallID();
			viaBranch = Request.GetViaBranch();
			cseq = Request.GetCSeq();
			fromTag=Request.GetFromTag();
			

			switch (Request.RequestLine.Method)
			{
				case "INVITE": return new InviteTransaction(callID, viaBranch, cseq);
				case "ACK": return new AckTransaction(callID,  viaBranch, cseq);
				case "REFER": return new ReferTransaction(callID,  viaBranch, cseq);
				case "NOTIFY": return new NotifyTransaction(callID, viaBranch, cseq);
				case "BYE": return new ByeTransaction(callID,  viaBranch, cseq);
				default:
					string error = $"Failed to create new transaction: Invalid request method {Request.RequestLine.Method}";
					Log(LogLevels.Error, error);
					throw new NotImplementedException(error);
			}
		}
		
		private Call CreateNewCall(Request Request, string SourceDevice, string DestinationDevice)
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

		

		private async Task<bool> UpdateKeyFrameAsync(KeyFrame KeyFrame, Request Request, string SourceDevice, string DestinationDevice, uint MessageIndex)
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
		private async Task<bool> UpdateKeyFrameAsync(KeyFrame KeyFrame, Response Response, string SourceDevice, string DestinationDevice, uint MessageIndex)
		{
			Call? call;
			Transaction? transaction;
			string? toTag;

			LogEnter();

			await Task.Delay(1);


			transaction = Transactions.FirstOrDefault(item => item.Match(Response));
			if (transaction == null)
			{
				string error = "Cannot find matching transaction for response";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);

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

		private async Task<bool> UpdateKeyFrameAsync( KeyFrame KeyFrame, ProjectViewModel Project, SIPMessage SIPMessage, MessageViewModel Message)
		{
			string sourceDevice,destinationDevice;

			LogEnter();
			
			
			sourceDevice = Project.Devices.FindDeviceByAddress(Message.SourceAddress)?.Name ?? Message.SourceAddress;
			destinationDevice = Project.Devices.FindDeviceByAddress(Message.DestinationAddress)?.Name ?? Message.DestinationAddress;

			switch (SIPMessage)
			{
				case Request request:return await UpdateKeyFrameAsync(KeyFrame, request, sourceDevice, destinationDevice,Message.Index);
				case Response response:return await UpdateKeyFrameAsync(KeyFrame, response, sourceDevice, destinationDevice, Message.Index);
				default:
					string error = "Invalid SIP message type";
					Log(LogLevels.Error, error);
					throw new InvalidOperationException(error);
			}

		}

		private async Task CreateKeyFramesAsync(CancellationToken CancellationToken, int Index)
		{
			StringReader reader;
			SIPMessage sipMessage;

			KeyFrame newKeyFrame;
			MessageViewModel message;


			LogEnter();

			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}


			message = _project.Messages[Index];

			reader = new StringReader(message.Content, ' ');
			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(reader);
			}
			catch (Exception ex)
			{
				string error = $"Failed to decode SIP message [{message.Index}]:\r\n" + ex.Message + "\r\n" + message.Content;
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

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

				if (await UpdateKeyFrameAsync(newKeyFrame,_project, sipMessage, message))
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




	}
}
