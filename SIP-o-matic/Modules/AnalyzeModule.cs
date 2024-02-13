using System;
using System.Collections.Generic;
using System.Linq;
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
	public class AnalyzeModule : Module
	{

		private List<Transaction> Transactions;
		

		public AnalyzeModule(ILogger Logger) : base(Logger)
		{
			Transactions = new List<Transaction>();
		}
		private Transaction CreateNewTransaction(Request Request,string SourceDevice, string DestinationDevice)
		{
			string? viaBranch;
			string? cseq;
			string? callID;

			LogEnter();

			callID = Request.GetHeader<CallIDHeader>()?.Value;
			if (callID == null)
			{
				string error = $"CallID header missing in SIP message";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			viaBranch = Request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value;
			if (viaBranch == null)
			{
				string error = $"Via branch missing in SIP message";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			cseq = Request.GetHeader<CSeqHeader>()?.Value;
			if (cseq == null)
			{
				string error = $"CSeq missing in SIP message";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			switch (Request.RequestLine.Method)
			{
				case "INVITE": return new InviteTransaction(callID,SourceDevice,DestinationDevice, viaBranch, cseq);
				case "ACK": return new AckTransaction(callID, SourceDevice,DestinationDevice, viaBranch, cseq);
				case "REFER": return new ReferTransaction(callID, SourceDevice,DestinationDevice, viaBranch, cseq);
				case "NOTIFY": return new NotifyTransaction(callID, SourceDevice,DestinationDevice, viaBranch, cseq);
				case "BYE": return new ByeTransaction(callID, SourceDevice,DestinationDevice, viaBranch, cseq);
				default:
					string error = $"Failed to create new transaction: Invalid request method {Request.RequestLine.Method}";
					Log(LogLevels.Error, error);
					throw new NotImplementedException(error);
			}
		}
		
		private Call CreateNewCall(Request Request, string SourceDevice, string DestinationDevice)
		{
			string? callID;
			Address? from, to;

			LogEnter();

			callID = Request.GetHeader<CallIDHeader>()?.Value;
			if (callID == null)
			{
				string error = $"CallID header missing in SIP message";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			from = Request.GetHeader<FromHeader>()?.Value;
			if (from == null)
			{
				string error = $"From header missing in SIP message";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			to = Request.GetHeader<ToHeader>()?.Value;
			if (to == null)
			{
				string error = $"To header missing in SIP message";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}

			return new Call(callID, SourceDevice, DestinationDevice, from.Value, to.Value, Call.States.OnHook, false);
		}

		private void UpdateKeyFrame(KeyFrame KeyFrame, Request Request, string SourceDevice, string DestinationDevice)
		{
			Call? call;
			Transaction? transaction;

			LogEnter();

			transaction = Transactions.FirstOrDefault(item => item.Match(Request,SourceDevice,DestinationDevice));
			if (transaction == null)
			{
				// check if all other transactions are terminated
				transaction = CreateNewTransaction(Request,SourceDevice,DestinationDevice);
				Transactions.Add(transaction);
			}

			

			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Request, SourceDevice, DestinationDevice));
			if (call == null)
			{
				call = CreateNewCall(Request, SourceDevice, DestinationDevice);
				KeyFrame.Calls.Add(call);
			}


			// update transaction
			if (transaction.Update(Request,SourceDevice,DestinationDevice))
			{
				// update call
				call.Update(transaction);
			}

		}
		private void UpdateKeyFrame(KeyFrame KeyFrame, Response Response, string SourceDevice, string DestinationDevice)
		{
			Call? call;
			Transaction? transaction;

			LogEnter();

			transaction = Transactions.FirstOrDefault(item => item.Match(Response, SourceDevice, DestinationDevice));
			if (transaction == null)
			{
				string error = "Cannot find matching transaction for response";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);

			}


			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Response, SourceDevice, DestinationDevice));
			if (call == null)
			{
				string error = "Cannot find matching call for response";
				Log(LogLevels.Error, error);
				throw new InvalidOperationException(error);
			}


			// update transaction
			if (transaction.Update(Response, SourceDevice, DestinationDevice))
			{
				// update call
				call.Update(transaction);
			}

		}

		private void UpdateKeyFrame( KeyFrame KeyFrame, ProjectViewModel Project, SIPMessage SIPMessage, MessageViewModel Message)
		{
			string sourceDevice,destinationDevice;

			LogEnter();

			/*if (Message.Index==100)
			{
				int t = 0;
			}
			//*/

			sourceDevice = Project.Devices.FindDeviceByAddress(Message.SourceAddress)?.Name ?? Message.SourceAddress;
			destinationDevice = Project.Devices.FindDeviceByAddress(Message.DestinationAddress)?.Name ?? Message.DestinationAddress;

			switch (SIPMessage)
			{
				case Request request:
					UpdateKeyFrame(KeyFrame, request, sourceDevice, destinationDevice);
					break;
				case Response response:
					UpdateKeyFrame(KeyFrame, response, sourceDevice, destinationDevice);
					break;
				default:
					string error = "Invalid SIP message type";
					Log(LogLevels.Error, error);
					throw new InvalidOperationException(error);
			}

		}
		public async Task CreateKeyFramesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{
			StringReader reader;
			SIPMessage sipMessage;

			KeyFrame newKeyFrame;
			KeyFrame? previousKeyFrame = null;

			LogEnter();

			await foreach (MessageViewModel message in Project.Messages.ToAsyncEnumerable())
			{
				/*if (message.Index==124)
				{
					int t = 0;
				}//*/

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
					if (previousKeyFrame == null) newKeyFrame = new KeyFrame(message.Timestamp);
					else
					{
						newKeyFrame = previousKeyFrame.Clone();
						newKeyFrame.Timestamp = message.Timestamp;
					}

					UpdateKeyFrame(newKeyFrame,Project, sipMessage, message);
				}
				catch (Exception ex)
				{
					string error = $"Failed to create key frame:\r\n" + ex.Message + $"\r\n[{message.Index}] " + message.Content;
					Log(LogLevels.Error, error);
					throw new InvalidOperationException(error);
				}
				Project.KeyFrames.Add(newKeyFrame);
				previousKeyFrame = newKeyFrame;
			}
		}



	}
}
