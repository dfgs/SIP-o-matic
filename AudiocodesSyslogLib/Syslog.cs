namespace AudiocodesSyslogLib
{
	public abstract class Syslog
	{
		public DateTime Timestamp
		{
			get;
			private set;
		}

		public string Address
		{
			get;
			private set;
		}

		public string Severity
		{
			get;
			private set;
		}

		public ulong SequenceNumber
		{
			get;
			private set;
		}

		public string Content
		{
			get;
			private set;
		}
		public Syslog(DateTime Timestamp, string Address, string Severity, ulong SequenceNumber, string Content)
		{
			this.Timestamp = Timestamp;
			this.Address = Address;
			this.Severity = Severity;
			this.SequenceNumber = SequenceNumber;
			this.Content = Content;
		}
	}
}