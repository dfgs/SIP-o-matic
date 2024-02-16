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
	public class AnalyzeModule : Module
	{

		private List<Transaction> Transactions;
		

		public AnalyzeModule(ILogger Logger) : base(Logger)
		{
			Transactions = new List<Transaction>();
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

		

		private bool UpdateKeyFrame(KeyFrame KeyFrame, Request Request, string SourceDevice, string DestinationDevice, uint MessageIndex)
		{
			Call? call;
			Transaction? transaction;


			LogEnter();

			

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
		private bool UpdateKeyFrame(KeyFrame KeyFrame, Response Response, string SourceDevice, string DestinationDevice, uint MessageIndex)
		{
			Call? call;
			Transaction? transaction;
			string? toTag;

			LogEnter();

			

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

		private bool UpdateKeyFrame( KeyFrame KeyFrame, ProjectViewModel Project, SIPMessage SIPMessage, MessageViewModel Message)
		{
			string sourceDevice,destinationDevice;

			LogEnter();
			
			
			sourceDevice = Project.Devices.FindDeviceByAddress(Message.SourceAddress)?.Name ?? Message.SourceAddress;
			destinationDevice = Project.Devices.FindDeviceByAddress(Message.DestinationAddress)?.Name ?? Message.DestinationAddress;

			switch (SIPMessage)
			{
				case Request request:return UpdateKeyFrame(KeyFrame, request, sourceDevice, destinationDevice,Message.Index);
				case Response response:return UpdateKeyFrame(KeyFrame, response, sourceDevice, destinationDevice, Message.Index);
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
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}

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

					newKeyFrame.MessageIndex = message.Index;

					if (UpdateKeyFrame(newKeyFrame,Project, sipMessage, message))
					{
						Project.KeyFrames.Add(newKeyFrame);
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
		}



	}
}
