// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.NetworkHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BuildingBlocks.Common;

public static class NetworkHelper
{
  public static 
#nullable disable
    string GetFQDN()
  {
    string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
    string hostName = Dns.GetHostName();
    string str = "." + domainName;
    if (!hostName.EndsWith(str, StringComparison.InvariantCultureIgnoreCase))
      hostName += str;
    return hostName;
  }

  public static IPAddress GetLocalIPAddress()
  {
    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
    {
      try
      {
        socket.Connect("10.0.2.4", 65530);
        return IPAddress.Parse((socket.LocalEndPoint as IPEndPoint).Address.ToString());
      }
      catch (SocketException ex)
      {
        return IPAddress.Parse("127.0.0.1");
      }
    }
  }

  public static IDictionary<IPAddress, string> GetLocalIPAddresses()
  {
    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
    Dictionary<IPAddress, string> localIpAddresses = new Dictionary<IPAddress, string>();
    foreach (NetworkInterface nic in networkInterfaces)
    {
      foreach (IPAddress localIpAddress in NetworkHelper.GetLocalIPAddresses(nic))
        localIpAddresses.Add(localIpAddress, nic.Name);
    }
    return (IDictionary<IPAddress, string>) localIpAddresses;
  }

  internal static IEnumerable<IPAddress> GetLocalIPAddresses(NetworkInterface nic)
  {
    foreach (UnicastIPAddressInformation unicastAddress in nic.GetIPProperties().UnicastAddresses)
    {
      if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastAddress.Address))
        yield return unicastAddress.Address;
    }
  }
}