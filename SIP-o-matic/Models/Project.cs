using SIP_o_matic.DataSources;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using ViewModelLib;

namespace SIP_o_matic.Models
{
	public class Project
	{
		
		
		public List<Device> Devices
		{
			get;
			set;
		}
		public List<Message> Messages
		{
			get;
			set;
		}

		[XmlIgnore]
		public List<KeyFrame> KeyFrames
		{
			get;
			set;
		}

		public Project()
		{
			Devices= new List<Device>();
			Messages = new List<Message>();
			KeyFrames = new List<KeyFrame>();
		}
		public async Task SaveAsync(string Path)
		{
			XmlSerializer serializer;

			serializer = new XmlSerializer(typeof(Project));
			using (FileStream stream = new FileStream(Path, FileMode.OpenOrCreate))
			{
				await Task.Run(() => serializer.Serialize(stream, this));
			}
		}
		public static async Task<Project> LoadAsync(string Path)
		{
			XmlSerializer serializer;
			Project result;
			object? data;

			serializer = new XmlSerializer(typeof(Project));
			using (FileStream stream = new FileStream(Path, FileMode.Open))
			{
				data = await Task.Run<object?>(() => serializer.Deserialize(stream));
				if (data == null) throw new InvalidOperationException("Failed to deserialize project");
				result = (Project)data;
			}
			return result;
		}



	}
}
