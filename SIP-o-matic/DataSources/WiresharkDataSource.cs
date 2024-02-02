using EthernetFrameReaderLib;
using PcapngFile;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
    public class WiresharkDataSource : IDataSource
	{

		public string Description => "Wiresharp pcapng";


		public WiresharkDataSource()
		{
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "pcapng";
		}


		public async IAsyncEnumerable<Device> EnumerateDevicesAsync(string FileName)
		{
			await Task.Yield();
			yield break;
		}


		public async IAsyncEnumerable<Message> EnumerateMessagesAsync(string FileName)
		{
			FrameReader frameReader;
			PacketReader packetReader;
			UDPSegmentReader udpSegmentReader;
			TCPSegmentReader tcpSegmentReader;

			Frame frame;
			Packet packet;
			UDPSegment udpSegment;
			TCPSegment tcpSegment;

			string message;

			frameReader = new FrameReader();
			packetReader = new PacketReader();
			udpSegmentReader = new UDPSegmentReader();
			tcpSegmentReader = new TCPSegmentReader();
			
			using (var reader = new Reader(FileName))
			{
				
				await foreach (var block in reader.EnhancedPacketBlocks.ToAsyncEnumerable())
				{
					frame = frameReader.Read(block.Data);
					packet = packetReader.Read(frame.Payload);

					switch (packet.Header.Protocol)
					{
						case Protocols.UDP:
							udpSegment = udpSegmentReader.Read(packet.Payload);
							message = Encoding.UTF8.GetString(udpSegment.Payload);
							break;
						case Protocols.TCP:
							tcpSegment = tcpSegmentReader.Read(packet.Payload);
							message = Encoding.UTF8.GetString(tcpSegment.Payload);
							break;
						default:continue;
					}
					
					if (
						message.StartsWith("SIP/2.0") ||
						message.StartsWith("INVITE") || message.StartsWith("ACK") || message.StartsWith("OPTIONS") || message.StartsWith("BYE") || message.StartsWith("CANCEL") || message.StartsWith("REGISTER")
						) yield return new Message(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(block.Timestamp/1000).ToLocalTime(),packet.Header.SourceAddress.ToString(),packet.Header.DestinationAddress.ToString(),  message);
					//await Task.Delay(2000);

				}

				reader.Reset();
			}
		}





	}
}
