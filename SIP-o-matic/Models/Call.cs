using SIPParserLib;
using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.Models
{
	public class Call:ICloneable<Call>
	{
		public enum States {OnHook, Calling,Proceeding, Ringing, OnHold, InProgress };
		public enum Triggers {INVITE,BYE,UPDATE,ACK };

		private StateMachine<States, Triggers> fsm;
		private StateMachine<States, Triggers>.TriggerWithParameters<Request> INVITETrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Request> ACKTrigger;


		public required string CallID
		{
			get;
			set;
		}

		public required string SourceAddress
		{
			get;
			set;
		}

		public required string DestinationAddress
		{
			get;
			set;
		}

		public required Address FromURI
		{
			get;
			set;
		}
		public required Address ToURI
		{
			get;
			set;
		}

		public  States State
		{
			get => fsm.State;
		}

		
		[SetsRequiredMembers]
		public Call(string callID, string SourceAddress,string DestinationAddress, Address FromURI, Address ToURI, States InitialState)
		{
			fsm = new StateMachine<States, Triggers>(InitialState);

			// OnHook => Calling => Proceeding => Ringing => In Progress 

			INVITETrigger = fsm.SetTriggerParameters<Request>(Triggers.INVITE);
			ACKTrigger = fsm.SetTriggerParameters<Request>(Triggers.ACK);

			fsm.Configure(States.OnHook)
				.PermitIf(INVITETrigger, States.Calling, (Request) => CallIdIsValid(Request));

			fsm.Configure(States.Calling)
			.PermitIf(ACKTrigger, States.InProgress, (Request) => CallIdIsValid(Request));

			/*fsm.Configure(States.OnHook)
				.OnEntry(t => StartCallTimer())
				.OnExit(t => StopCallTimer())
				.InternalTransition(Trigger.MuteMicrophone, t => OnMute())
				.InternalTransition(Trigger.UnmuteMicrophone, t => OnUnmute())
				.InternalTransition<int>(_setVolumeTrigger, (volume, t) => OnSetVolume(volume))
				.Permit(Trigger.LeftMessage, State.OffHook)
				.Permit(Trigger.PlacedOnHold, State.OnHold);*/


			CallID = callID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.FromURI = FromURI;
			this.ToURI = ToURI;
		}

		public Call Clone()
		{
			return new Call(this.CallID, this.SourceAddress, this.DestinationAddress,this.FromURI, this.ToURI, this.State);
						
		}
		private bool CallIdIsValid(Request Request)
		{
			return CallID== Request.GetHeader<CallIDHeader>()?.Value;
		}

		public void Update(Request Request)
		{
			switch(Request.RequestLine.Method)
			{
				case "INVITE":
					fsm.Fire(INVITETrigger, Request);
					break;
				case "ACK":
					fsm.Fire(ACKTrigger, Request);
					break;
				default: throw new InvalidOperationException($"Unsupported call transition ({Request.RequestLine.Method})");
			}
		}


	}
}
