using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SACNPackets
{
	public class SACNSender
	{

		public static int SACN_PORT = 5568;

		public Guid UUID { get; set; }
		public UdpClient UdpClient { get; set; }
		public IPAddress UnicastAddress { get; set; }
		public bool Multicast { get { return UnicastAddress == null; } }
		public int Port { get; set; }
		public string SourceName { get; set; }

		static Guid Light4Life_Id = Guid.NewGuid();

		//byte[] SyncPacket;      //PDU Length = 11
		//byte[] UniversePacket;

		byte sequenceID = 0;

		public SACNSender(Guid uuid, string sourceName, int port)
		{
			SourceName = sourceName;
			UUID = uuid;
			UdpClient = new UdpClient();
			UdpClient.DontFragment = false;
			UdpClient.Client.SendBufferSize = 2560000;
			Port = port;    //5568
		}

		public SACNSender(Guid uuid, string sourceName) : this(uuid, sourceName, SACN_PORT) { }

		public byte Send(Int16 universeID, byte[] data/*, Int16 syncAddress = 0, bool syncPacket = false*/)
		{

			SACNPacket packet = new SACNPacket(universeID, SourceName, UUID, sequenceID++, data/*, syncAddress, syncPacket*/);
			byte[] packetBytes = packet.ToArray();
			//SACNPacket parsed = SACNPacket.Parse(packetBytes);
			IPEndPoint endPoint = GetEndPoint(universeID, Port);

			UdpClient.Send(packetBytes, packetBytes.Length, endPoint);
			//Console.WriteLine($"Universe {universeID} PacketLength {packetBytes.Length} Sended {bytes}");
			return sequenceID;
		}

		public byte Send(IPAddress targetIP, int repeatCount = 1)
		{
			MemoryStream stream = new MemoryStream();
			IPEndPoint endPoint = new IPEndPoint(targetIP, Port);
			for (int i = 0; i < repeatCount; i++)
			{
				SACNPacket packet = new SACNPacket((short)(i+1), "LightLego", UUID, sequenceID++, new byte[512]);
				byte[] packetBytes = packet.ToArray();
				SACNPacket parsed = SACNPacket.Parse(packetBytes);
				stream.Write(packetBytes, 0, packetBytes.Length);
			}
			UdpClient.Send(stream.ToArray(), stream.ToArray().Length, endPoint);
			return sequenceID;
		}

		private IPEndPoint GetEndPoint(Int16 universeID, int port)
		{
			if (Multicast)
			{
				return new IPEndPoint(IPAddress.Parse("239.255.0.11"), port);
			}
			else
			{
				return new IPEndPoint(UnicastAddress, port);
			}
		}

		public void Close()
		{
			UdpClient.Close();
		}
	}

}
