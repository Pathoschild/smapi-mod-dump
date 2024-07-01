/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net;

namespace Lidgren.Network
{
	public static partial class NetUtility
	{
		private static byte[] s_randomMacBytes;
		
		static NetUtility()
		{
		}

		[CLSCompliant(false)]
		public static ulong GetPlatformSeed(int seedInc)
		{
			ulong seed = (ulong)Environment.TickCount + (ulong)seedInc;
			return seed ^ ((ulong)(new object().GetHashCode()) << 32);
		}
		
		/// <summary>
		/// Gets my local IPv4 address (not necessarily external) and subnet mask
		/// </summary>
		public static IPAddress GetMyAddress(out IPAddress mask)
		{
			mask = null;
			try
			{
				Android.Net.Wifi.WifiManager wifi = (Android.Net.Wifi.WifiManager)Android.App.Application.Context.GetSystemService(Android.App.Activity.WifiService);
				if (!wifi.IsWifiEnabled)
					return null;

				var dhcp = wifi.DhcpInfo;
				int addr = dhcp.IpAddress;
				byte[] quads = new byte[4];
				for (int k = 0; k < 4; k++)
					quads[k] = (byte)((addr >> k * 8) & 0xFF);
				return new IPAddress(quads);
			}
			catch // Catch Access Denied errors
			{
				return null;
			}
		}

		public static byte[] GetMacAddressBytes()
		{
			if (s_randomMacBytes == null)
			{
				s_randomMacBytes = new byte[8];
				MWCRandom.Instance.NextBytes(s_randomMacBytes);
			}
			return s_randomMacBytes;
		}

		public static void Sleep(int milliseconds)
		{
			System.Threading.Thread.Sleep(milliseconds);
		}

		public static IPAddress GetBroadcastAddress()
		{
			return IPAddress.Broadcast;
		}

		public static IPAddress CreateAddressFromBytes(byte[] bytes)
		{
			return new IPAddress(bytes);
		}

		public static byte[] ComputeSHAHash(byte[] bytes, int offset, int count)
		{
			using var sha = SHA256.Create();
			return sha.ComputeHash(bytes, offset, count);
		}
	}

	public static partial class NetTime
	{
		private static readonly long s_timeInitialized = Environment.TickCount;
		
		/// <summary>
		/// Get number of seconds since the application started
		/// </summary>
		public static double Now { get { return (double)((uint)Environment.TickCount - s_timeInitialized) / 1000.0; } }
	}
}
#endif