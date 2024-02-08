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
		public enum Triggers {INVITE,BYE,UPDATE,ACK,Prov100,Prov180,Final200 };

		private StateMachine<States, Triggers> fsm;
		private StateMachine<States, Triggers>.TriggerWithParameters<Request> INVITETrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Request> ACKTrigger;

		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Prov100Trigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Prov183Trigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Final200Trigger;


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

		public required bool IsAck
		{
			get;
			set;
		}

		public  States State
		{
			get => fsm.State;
		}

		
		[SetsRequiredMembers]
		public Call(string callID, string SourceAddress,string DestinationAddress, Address FromURI, Address ToURI, States InitialState,bool IsAck)
		{
			fsm = new StateMachine<States, Triggers>(InitialState);

			// OnHook => Calling => Proceeding => Ringing => In Progress 

			INVITETrigger = fsm.SetTriggerParameters<Request>(Triggers.INVITE);
			ACKTrigger = fsm.SetTriggerParameters<Request>(Triggers.ACK);
			Prov100Trigger = fsm.SetTriggerParameters<Response>(Triggers.Prov100);
			Prov183Trigger = fsm.SetTriggerParameters<Response>(Triggers.Prov180);
			Final200Trigger = fsm.SetTriggerParameters<Response>(Triggers.Final200);

			fsm.Configure(States.OnHook)
				.PermitIf(INVITETrigger, States.Calling, (Request) => CallIdIsValid(Request));

			fsm.Configure(States.Calling)
				.PermitIf(Prov100Trigger, States.Proceeding, (Response) => CallIdIsValid(Response))
				.PermitIf(Prov183Trigger, States.Ringing, (Response) => CallIdIsValid(Response))
				.PermitIf(Final200Trigger, States.InProgress, (Response) => CallIdIsValid(Response));

			fsm.Configure(States.Proceeding)
				.PermitReentryIf(Prov100Trigger, (Response) => CallIdIsValid(Response))
				.PermitIf(Prov183Trigger, States.Ringing, (Response) => CallIdIsValid(Response))
				.PermitIf(Final200Trigger, States.InProgress, (Response) => CallIdIsValid(Response));
			
			fsm.Configure(States.Ringing)
				.PermitIf(Final200Trigger, States.InProgress, (Response) => CallIdIsValid(Response));

			fsm.Configure(States.InProgress)
				.PermitReentryIf(ACKTrigger, (Request) => CallIdIsValid(Request)).OnEntryFrom(Triggers.ACK,()=>this.IsAck=true) ;

			/*fsm.Configure(States.Proceeding)
				.PermitReentryIf(Prov100Trigger, (Request) => CallIdIsValid(Request))
				.PermitIf(Prov183Trigger, States.Ringing, (Request) => CallIdIsValid(Request))
				.PermitIf(Final200Trigger, States.InProgress, (Request) => CallIdIsValid(Request));//*/


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
			this.IsAck = IsAck;
		}

		public Call Clone()
		{
			return new Call(this.CallID, this.SourceAddress, this.DestinationAddress,this.FromURI, this.ToURI, this.State,this.IsAck);
						
		}
		private bool CallIdIsValid(Request Request)
		{
			return CallID== Request.GetHeader<CallIDHeader>()?.Value;
		}
		private bool CallIdIsValid(Response Response)
		{
			return CallID == Response.GetHeader<CallIDHeader>()?.Value;
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
		public void Update(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case "100":
					fsm.Fire(Prov100Trigger, Response);
					break;
				case "180":
					fsm.Fire(Prov183Trigger, Response);
					break;
				case "200":
					fsm.Fire(Final200Trigger, Response);
					break;
				default: throw new InvalidOperationException($"Unsupported call transition ({Response.StatusLine.StatusCode})");
			}
		}

	}
}
