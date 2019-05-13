using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static System.Net.Mime.MediaTypeNames;

namespace SACNPackets
{
	class Program
	{
		static void Main(string[] args)
		{
			bool mustExit = false;
			IPAddress deviceIP = null;
			int repeatCount = 0;
			SACNSender sacnSender = new SACNSender(Guid.NewGuid(), "LightLego");
			sacnSender.UnicastAddress = IPAddress.Parse("192.168.1.100");
			WriteLine("Enter target device IP");
			string ipS = ReadLine();
			if (IPAddress.TryParse(ipS, out deviceIP))
			{
				while (true)
				{
					WriteLine("Enter Repeat Count or Enter to Exit");
					string rcS = ReadLine();
					if (rcS == "")
						Environment.Exit(1);
					if (int.TryParse(rcS, out repeatCount))
					{
						if (repeatCount > 2 || repeatCount < 1)
							repeatCount = 1;
						sacnSender.Send(deviceIP, repeatCount);
					}
					else
					{
						WriteLine("Not a Number. Try again");
					}
				}
			}
			else
			{
				WriteLine("Wrong IP Format. Press Enter to Exit");
				ReadLine();
			}
		}
	}
}
