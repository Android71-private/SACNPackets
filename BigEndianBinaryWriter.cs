using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACNPackets
{
	public class BigEndianBinaryWriter : BinaryWriter
	{
		public BigEndianBinaryWriter(Stream output) : base(output)
		{

		}

		public override void Write(short value)
		{
			short networkOrder = System.Net.IPAddress.HostToNetworkOrder(value);
			base.Write(networkOrder);
		}

		public override void Write(int value)
		{
			int networkOrder = System.Net.IPAddress.HostToNetworkOrder(value);
			base.Write(networkOrder);
		}

		public void WriteTerminatingString(string value, int length)
		{
			Write(Encoding.UTF8.GetBytes(value));
			for (int i = 0; i < length - value.Length; i++)
			{
				Write((byte)0);
			}
		}
	}
}
