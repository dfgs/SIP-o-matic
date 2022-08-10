using EthernetFrameReaderLib;
using PcapngFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public class WiresharkDataSource : IDataSource
	{
		private string fileName;
		public WiresharkDataSource(string FileName)
		{
			this.fileName = FileName;
		}
		public async IAsyncEnumerable<string> EnumerateMessagesAsync()
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
			
			using (var reader = new Reader(fileName))
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
						) yield return message;
					//await Task.Delay(2000);

				}

				reader.Reset();
			}
		}





	}
}
