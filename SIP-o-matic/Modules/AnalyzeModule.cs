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

namespace SIP_o_matic.Modules
{
	public class AnalyzeModule : Module
	{
		public AnalyzeModule(ILogger Logger) : base(Logger)
		{
		}
		private Transaction CreateNewTransaction(Request Request)
		{
			string? viaBranch;
			string? cseq;
			string? callID;

			callID = Request.GetHeader<CallIDHeader>()?.Value;
			if (callID == null)
			{
				string error = $"CallID header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			viaBranch = Request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value;
			if (viaBranch == null)
			{
				string error = $"Via branch missing in SIP message";
				throw new InvalidOperationException(error);
			}

			cseq = Request.GetHeader<CSeqHeader>()?.Value;
			if (cseq == null)
			{
				string error = $"CSeq missing in SIP message";
				throw new InvalidOperationException(error);
			}

			switch (Request.RequestLine.Method)
			{
				case "INVITE": return new InviteTransaction(callID, viaBranch, cseq, Transaction.States.Undefined);
				case "ACK": return new AckTransaction(callID, viaBranch, cseq, Transaction.States.Undefined);
				case "REFER": return new ReferTransaction(callID, viaBranch, cseq, Transaction.States.Undefined);
				case "NOTIFY": return new NotifyTransaction(callID, viaBranch, cseq, Transaction.States.Undefined);
				case "BYE": return new ByeTransaction(callID, viaBranch, cseq, Transaction.States.Undefined);
				default:
					throw new NotImplementedException($"Failed to create new transaction: Invalid request method {Request.RequestLine.Method}");
			}
		}

		private Call CreateNewCall(Request Request, string SourceAddress, string DestinationAddress)
		{
			string? callID;
			Address? from, to;

			callID = Request.GetHeader<CallIDHeader>()?.Value;
			if (callID == null)
			{
				string error = $"CallID header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			from = Request.GetHeader<FromHeader>()?.Value;
			if (from == null)
			{
				string error = $"From header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			to = Request.GetHeader<ToHeader>()?.Value;
			if (to == null)
			{
				string error = $"To header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return new Call(callID, SourceAddress, DestinationAddress, from.Value, to.Value, Call.States.OnHook, false);
		}

		private void UpdateKeyFrame(KeyFrame KeyFrame, Request Request, string SourceAddress, string DestinationAddress)
		{
			Call? call;
			Transaction? transaction;

			transaction = KeyFrame.Transactions.FirstOrDefault(item => item.Match(Request));
			if (transaction == null)
			{
				// check if all other transactions are terminated
				transaction = CreateNewTransaction(Request);
				KeyFrame.Transactions.Add(transaction);
			}

			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Request));
			if (call == null)
			{
				call = CreateNewCall(Request, SourceAddress, DestinationAddress);
				KeyFrame.Calls.Add(call);
			}

			// update transaction
			transaction.Update(Request);
			// update call
			call.Update(transaction);

		}
		private void UpdateKeyFrame(KeyFrame KeyFrame, Response Response)
		{
			Call? call;
			Transaction? transaction;

			transaction = KeyFrame.Transactions.FirstOrDefault(item => item.Match(Response));
			if (transaction == null) throw new InvalidOperationException("Cannot find matching transaction for response");


			call = KeyFrame.Calls.FirstOrDefault(item => item.Match(Response));
			if (call == null) throw new InvalidOperationException("Cannot find matching call for response");


			// update transaction
			transaction.Update(Response);
			// update call
			call.Update(transaction);

		}

		private void UpdateKeyFrame(KeyFrame KeyFrame, SIPMessage SIPMessage, MessageViewModel Message)
		{

			switch (SIPMessage)
			{
				case Request request:
					UpdateKeyFrame(KeyFrame, request, Message.SourceAddress, Message.DestinationAddress);
					break;
				case Response response:
					UpdateKeyFrame(KeyFrame, response);
					break;
				default: throw new InvalidOperationException("Invalid SIP message type");
			}

		}
		public async Task CreateKeyFramesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{
			StringReader reader;
			SIPMessage sipMessage;

			KeyFrame newKeyFrame;
			KeyFrame? previousKeyFrame = null;

			await foreach (MessageViewModel message in Project.Messages.ToAsyncEnumerable())
			{
				reader = new StringReader(message.Content, ' ');
				try
				{
					sipMessage = SIPGrammar.SIPMessage.Parse(reader);
				}
				catch (Exception ex)
				{
					string error = $"Failed to decode SIP message [{message.Index}]:\r\n" + ex.Message + "\r\n" + message.Content;
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

					UpdateKeyFrame(newKeyFrame, sipMessage, message);
				}
				catch (Exception ex)
				{
					string error = $"Failed to create key frame:\r\n" + ex.Message + $"\r\n[{message.Index}] " + message.Content;
					throw new InvalidOperationException(error);
				}
				Project.KeyFrames.Add(newKeyFrame);
				previousKeyFrame = newKeyFrame;
			}
		}



	}
}
