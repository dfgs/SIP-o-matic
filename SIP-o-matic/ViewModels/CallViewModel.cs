using LogLib;
using SIP_o_matic.Models;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class CallViewModel : ViewModel<Call>
	{
		public string CallID
		{
			get => Model.CallID;
		}

		public string SourceDevice
		{
			get=>Model.SourceDevice;
		}

		public string DestinationDevice
		{
			get=> Model.DestinationDevice;
		}

		public IEnumerable<string> Devices
		{
			get
			{
				yield return SourceDevice;
				yield return DestinationDevice;
			}
		}
		public string Caller
		{
			get=>Model.Caller;
		}

		public string Callee
		{
			get => Model.Callee;
		}
		public Call.States State
		{
			get => Model.State;
		}
		public bool IsAck
		{
			get =>Model.IsAck;
		}

		public string LegName
		{
			get;
			set;
		}

		public string LegDescription
		{
			get;
			set;
		}

		public string Color
		{
			get;
			set;
		}

		public uint[] MessageIndices
		{
			get => Model.MessageIndices;
		}

		public string MessageIndicesDescription
		{
			get;
			set;
		}

		public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(CallViewModel), new PropertyMetadata(false));
		public bool IsFlipped
		{
			get { return (bool)GetValue(IsFlippedProperty); }
			set { SetValue(IsFlippedProperty, value); }
		}
		public CallViewModel(ILogger Logger) : base(Logger)
		{
			LegName = "Undefined";
			LegDescription = "Undefined";
			Color = "Black";
			MessageIndicesDescription = "";
		}
	}
}
