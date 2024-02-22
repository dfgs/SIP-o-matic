﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;



namespace SIP_o_matic.corelib.Models
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
			using (FileStream stream = new FileStream(Path, FileMode.Create))
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

		public async Task ExportSIPAsync(string Path)
		{
			StreamWriter writer;

			using (Stream stream = new FileStream(Path, FileMode.Create))
			{
				writer = new StreamWriter(stream);

				await writer.WriteLineAsync("[Devices]");
				await writer.WriteLineAsync("");
				foreach (Device device in Devices)
				{
					await writer.WriteLineAsync(device.Name + " (" + string.Join(", ",device.Addresses) +" )");
				}
				await writer.WriteLineAsync("");
				await writer.WriteLineAsync("");

				await writer.WriteLineAsync("[Messages]");
				await writer.WriteLineAsync("");
				foreach (Message message in Messages)
				{
					//2023 - 01 - 02 14:03:33.2225 from 192.168.0.1 to 192.168.0.2
					await writer.WriteLineAsync("----------------------------------------------------------------------------------------");
					await writer.WriteLineAsync(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffff") + " from " + message.SourceAddress + " to " + message.DestinationAddress);
					await writer.WriteLineAsync("----------------------------------------------------------------------------------------");
					await writer.WriteLineAsync(message.Content);
				}

			}

		}



	}
}