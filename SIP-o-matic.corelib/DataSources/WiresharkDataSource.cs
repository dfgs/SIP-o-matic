﻿using EthernetFrameReaderLib;
using PcapngFile;
using SIP_o_matic.corelib;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
    public class WiresharkDataSource : IDataSource
	{
		private List<Device> devices;
		private List<Message> messages;
		private List<UDPStream> transmissions;

		public string Description => "Wiresharp pcapng";


		public WiresharkDataSource()
		{
			devices = new List<Device>();
			messages = new List<Message>();
			transmissions = new List<UDPStream>();
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "pcapng";
		}

		public async Task LoadAsync(string FileName)
		{
			FrameReader frameReader;
			PacketReader packetReader;
			UDPSegmentReader udpSegmentReader;
			TCPSegmentReader tcpSegmentReader;
			UDPStream transmission;
			UDPStream? existingTransmission;
			DateTime timeStamp;

			Frame frame;
			Packet packet;
			UDPSegment udpSegment;
			TCPSegment tcpSegment;
			uint index = 1;
			string content;

			Address sourceAddress, destinationAddress;
			Device? device;

			frameReader = new FrameReader();
			packetReader = new PacketReader();
			udpSegmentReader = new UDPSegmentReader();
			tcpSegmentReader = new TCPSegmentReader();

			devices.Clear();
			messages.Clear();

			using (var reader = new Reader(FileName))
			{

				await foreach (var block in reader.EnhancedPacketBlocks.ToAsyncEnumerable())
				{
					frame = frameReader.Read(block.Data);
					packet = packetReader.Read(frame.Payload);

					timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(block.Timestamp / 1000).ToLocalTime();
					sourceAddress = new Address(packet.Header.SourceAddress.ToString());
					destinationAddress = new Address(packet.Header.DestinationAddress.ToString());

					switch (packet.Header.Protocol)
					{
						case Protocols.UDP:
							udpSegment = udpSegmentReader.Read(packet.Payload);
							content = Encoding.UTF8.GetString(udpSegment.Payload);

							transmission = new UDPStream(timeStamp, sourceAddress, destinationAddress, udpSegment.Header.DestinationPort);
							existingTransmission = transmissions.FirstOrDefault(item => item.Matches(transmission));
							if (existingTransmission != null)
							{
								existingTransmission.LastTimestamp = transmission.Timestamp;
							}
							else
							{
								transmissions.Add(transmission);
							}

							break;
						case Protocols.TCP:
							tcpSegment = tcpSegmentReader.Read(packet.Payload);
							content = Encoding.UTF8.GetString(tcpSegment.Payload);
							break;
						default: continue;
					}

					if (
						content.StartsWith("SIP/2.0") ||
						content.StartsWith("INVITE") || content.StartsWith("ACK") || content.StartsWith("OPTIONS") || content.StartsWith("BYE") || content.StartsWith("CANCEL") || content.StartsWith("REGISTER")
						)
					{
						//new Message(index++, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(block.Timestamp / 1000).ToLocalTime(), packet.Header.SourceAddress.ToString(), packet.Header.DestinationAddress.ToString(), message);

						device = devices.FirstOrDefault(item => item.Name == sourceAddress.Value);
						if (device == null)
						{
							device = new Device(sourceAddress.Value, new Address[] { sourceAddress });
							devices.Add(device);
						}

						device = devices.FirstOrDefault(item => item.Name == destinationAddress.Value);
						if (device == null)
						{
							device = new Device(destinationAddress.Value, new Address[] { destinationAddress });
							devices.Add(device);
						}


						Message message = new Message(index++, timeStamp,sourceAddress, destinationAddress, content);
						messages.Add(message);
					}

					

				}

				reader.Reset();
			}
		}

		public IEnumerable<Device> EnumerateDevices()
		{
			return devices;
		}



		public IEnumerable<Message> EnumerateMessages()
		{
			return messages;
		}

		public IEnumerable<UDPStream> EnumerateUDPStreams()
		{
			return transmissions;
		}









	}
}
