using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SIP_o_matic.Models
{
	public class SourceFile
	{
		public required string Path
		{
			get;
			set;
		}

		[XmlIgnore]
		public string Name
		{
			get=> System.IO.Path.GetFileName(Path);
		}

		public SourceFile()
		{

		}

	}
}
