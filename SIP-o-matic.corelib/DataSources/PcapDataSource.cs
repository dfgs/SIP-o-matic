using EthernetFrameReaderLib;
using PcapReaderLib;
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
    public class PcapDataSource : IDataSource
	{
		private List<Device> devices;
		private List<Message> messages;
		private List<UDPStream> transmissions;

		public string Description => "Wiresharp pcap";


		public PcapDataSource()
		{
			devices = new List<Device>();
			messages = new List<Message>();
			transmissions = new List<UDPStream>();
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "pcap";
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
			PcapReader reader;
			FileHeader header;
			PacketRecord record;

			Frame frame;
			Packet packet;
			UDPSegment udpSegment;
			TCPSegment tcpSegment;
			uint index = 1;
			string content;

			Address sourceAddress, destinationAddress;
			Device? device;

			reader = new PcapReader();

			frameReader = new FrameReader();
			packetReader = new PacketReader();
			udpSegmentReader = new UDPSegmentReader();
			tcpSegmentReader = new TCPSegmentReader();

			devices.Clear();
			messages.Clear();

			using (Stream stream = new FileStream(FileName, FileMode.Open))
			{
				using (BinaryReader bReader = new BinaryReader(stream))
				{

					content = "";
					header = reader.ReadHeader(bReader);

					while(stream.Position<stream.Length)
					{
						record = reader.ReadPacketRecord(bReader);

						frame = frameReader.Read(record.PacketData);
						packet = packetReader.Read(frame.Payload);

						timeStamp = header.GetTimeTimeUTC(record).ToLocalTime();

						sourceAddress = new Address(packet.Header.SourceAddress.ToString());
						destinationAddress = new Address(packet.Header.DestinationAddress.ToString());

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



						switch (packet.Header.Protocol)
						{
							case Protocols.UDP:
								udpSegment = udpSegmentReader.Read(packet.Payload);
								content += Encoding.UTF8.GetString(udpSegment.Payload);

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
								content += Encoding.UTF8.GetString(tcpSegment.Payload);
								break;
							default: continue;
						}
						if (packet.Header.MoreFragments) continue; // reassemble fragmented packets


						if (
							content.StartsWith("SIP/2.0") ||
							content.StartsWith("INVITE") || content.StartsWith("ACK") || content.StartsWith("OPTIONS") || content.StartsWith("BYE") || content.StartsWith("CANCEL") || content.StartsWith("REGISTER")
							|| content.StartsWith("REFER") || content.StartsWith("NOTIFY") || content.StartsWith("MESSAGE") || content.StartsWith("SUBSCRIBE") || content.StartsWith("UPDATE") || content.StartsWith("PRACK")
							)
						{
							Message message = new Message(index++, timeStamp, sourceAddress, destinationAddress, content);
							messages.Add(message);
						}

						content = "";


					}

				}
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
