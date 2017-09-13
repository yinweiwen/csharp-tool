using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Heijden.DNS;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Management;

namespace AutoDNS
{
    class Program
    {
        /// <summary>
        /// Default DNS port
        /// </summary>
        public const int DefaultPort = 53;
        public static readonly IPEndPoint[] DefaultDnsServers =
        {
            new IPEndPoint(IPAddress.Parse("223.5.5.5"), 53),
            new IPEndPoint(IPAddress.Parse("114.114.114.114"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53),
            new IPEndPoint(IPAddress.Parse("8.8.4.4"), 53),
            new IPEndPoint(IPAddress.Parse("223.6.6.6"), 53),
            new IPEndPoint(IPAddress.Parse("114.114.115.115"), 53)
        };

        public static readonly Website[] WebSites =
        {
            new Website("百度","www.baidu.com"),
            new Website("谷歌","www.google.com"),
            new Website("新浪","www.sina.com"),
            new Website("淘宝","www.taobao.com"),
            new Website("腾讯","www.tencent.com"),
            new Website("苹果","www.apple.com"),
        };

        static void Main(string[] args)
        {
            SetIpInfo(null, null, null, new[] { "223.5.5.89" });
            GetDnsInfo();
            Resolver _resolver = new Resolver(DefaultDnsServers);

            foreach (Website site in WebSites)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var response = _resolver.Query(site.Url, QType.A);
                Console.WriteLine("{0} {1} ms", site.Name, sw.ElapsedMilliseconds);
                Console.WriteLine(response.Server);
                if (response.header.QDCOUNT > 0)
                {
                    Console.WriteLine(";; QUESTION SECTION:");
                    foreach (Question question in response.Questions)
                        Console.WriteLine(";{0}", question);
                    Console.WriteLine("");
                }

                if (response.header.ANCOUNT > 0)
                {
                    Console.WriteLine(";; ANSWER SECTION:");
                    foreach (AnswerRR answerRR in response.Answers)
                        Console.WriteLine(answerRR);
                    Console.WriteLine("");
                }

                if (response.header.NSCOUNT > 0)
                {
                    Console.WriteLine(";; AUTHORITY SECTION:");
                    foreach (AuthorityRR authorityRR in response.Authorities)
                        Console.WriteLine(authorityRR);
                    Console.WriteLine("");
                }

                if (response.header.ARCOUNT > 0)
                {
                    Console.WriteLine(";; ADDITIONAL SECTION:");
                    foreach (AdditionalRR additionalRR in response.Additionals)
                        Console.WriteLine(additionalRR);
                    Console.WriteLine("");
                }
            }

            Console.ReadKey();
        }

        public static void GetDnsInfo()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        MACAddress = mo["MacAddress"].ToString();
                    }
                }
                mo.Dispose();
            }
            MACAddress = MACAddress.Replace(":", "-");
            Console.WriteLine("#Working Network Adapter: {0}", MACAddress);

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                bool Pd1 = (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211); //判断是否是以太网连接  
                if (Pd1 && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine("物理地址 :{0}", adapter.GetPhysicalAddress().ToString());
                    var statistics = adapter.GetIPv4Statistics();
                    Console.WriteLine("tongji :{0}", statistics);
                    IPInterfaceProperties ip = adapter.GetIPProperties();     //IP配置信息  
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        var ipv4 = ip.UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                        Console.WriteLine("IP地址 :{0}", ipv4.Address.ToString());
                        Console.WriteLine("子网掩码 :{0}", ipv4.IPv4Mask.ToString());

                    }
                    if (ip.GatewayAddresses.Count > 0)
                    {
                        Console.WriteLine("默认网关  :{0}", ip.GatewayAddresses[0].Address.ToString());
                    }
                    int DnsCount = ip.DnsAddresses.Count;
                    Console.WriteLine("DNS服务器地址：");
                    if (DnsCount > 0)
                    {
                        Console.WriteLine("主DNS  :{0}", ip.DnsAddresses[0].ToString());
                    }
                    if (DnsCount > 1)
                    {
                        Console.WriteLine("备用DNS地址 :{0}", ip.DnsAddresses[1].ToString());
                    }
                }
            }
        }

        public static void SetIpInfo(string[] ip, string[] SubMark, string[] GateWay, string[] DNS)
        {
            ManagementObjectSearcher s = new ManagementObjectSearcher(
    @"SELECT DeviceID FROM Win32_NetworkAdapter WHERE NetConnectionStatus=2 AND PNPDeviceID LIKE 'PCI%'");
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"]) continue;
                var MACAddress = mo["MacAddress"].ToString();
                Console.WriteLine(MACAddress);
                inPar = mo.GetMethodParameters("EnableStatic");
                Console.WriteLine(s);
                inPar["IPAddress"] = ip;//ip地址  
                inPar["SubnetMask"] = SubMark; //子网掩码   
                mo.InvokeMethod("EnableStatic", inPar, null);//执行  

                inPar = mo.GetMethodParameters("SetGateways");
                inPar["DefaultIPGateway"] = GateWay; //设置网关地址 1.网关;2.备用网关  
                outPar = mo.InvokeMethod("SetGateways", inPar, null);//执行  

                inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                inPar["DNSServerSearchOrder"] = DNS; //设置DNS  1.DNS 2.备用DNS  
                mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);// 执行  
                break; //只设置一张网卡，不能多张。  
            }
        }
    }

    public class Website
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public Website(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }

}
