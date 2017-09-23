using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Heijden.DNS;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Management;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

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
            new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53), //google
            new IPEndPoint(IPAddress.Parse("114.114.114.114"), 53), //114
            new IPEndPoint(IPAddress.Parse("223.5.5.5"), 53),   // ali
            new IPEndPoint(IPAddress.Parse("8.8.4.4"), 53),
            new IPEndPoint(IPAddress.Parse("223.6.6.6"), 53),
            new IPEndPoint(IPAddress.Parse("114.114.115.115"), 53),
            new IPEndPoint(IPAddress.Parse("180.76.76.76"), 53),    // baidu
            new IPEndPoint(IPAddress.Parse("101.226.4.6"), 53),
            new IPEndPoint(IPAddress.Parse("112.124.47.27"), 53)
        };

        private static double AverageResolveCost(Resolver res)
        {
            List<long> elps = new List<long>();
            foreach (Website site in WebSites)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var ss = res.Query(site.Url, QType.A);
                if (ss.Answers.Any())
                    elps.Add(sw.ElapsedMilliseconds);
            }
            return elps.Any() ? elps.Average() : 0;
        }

        private static bool CheckDns(Resolver res)
        {
            var elps = AverageResolveCost(res);
            return 0 < elps && elps < 1000;
        }

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
            while (true)
            {
                try
                {
                    Console.WriteLine(">> START DNS VALID...");
                    var validDns = new List<IPEndPoint>();
                    long validDnsCnt = 0;
                    long validThreadCnt = 0;
                    var synobj = new object();
                    if (!CheckDns(new Resolver()))
                    {
                        Console.WriteLine("DNS invalid, begin searching...");
                        foreach (var dns in DefaultDnsServers)
                        {
                            new Task(() => {
                                Console.WriteLine("search {0}...", dns.Address.ToString());
                                Interlocked.Increment(ref validThreadCnt);
                                try
                                {
                                    if (CheckDns(new Resolver(new[] { dns })))
                                    {
                                        lock (synobj)
                                        {
                                            validDns.Add(dns);
                                            Interlocked.Increment(ref validDnsCnt);
                                        }
                                    }
                                }
                                catch (Exception)  // check err
                                {
                                    // do nothing
                                }
                                Interlocked.Decrement(ref validThreadCnt);
                            }).Start();
                        }
                        Thread.Sleep(500);
                        while (Interlocked.Read(ref validDnsCnt) < 2 && Interlocked.Read(ref validThreadCnt) > 0)
                        {
                            Thread.Sleep(5);
                        }
                        lock (synobj)
                        {
                            var dnses = validDns.Take(2).Select(d => d.Address.ToString()).ToArray();
                            if (dnses.Any())
                            {
                                SetIpInfo(null, null, null, dnses);
                                Console.WriteLine("change dns to {0} successful", string.Join(",", dnses));
                            }
                        }
                    }   // not valid
                    GetDnsInfo();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
#if DEBUG
                Thread.Sleep(10 * 1000);
#else
                Thread.Sleep(5 * 60 * 1000);
#endif
            }
        }

        public static void TestDns()
        {
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
                if (ip != null && SubMark != null)
                {
                    inPar = mo.GetMethodParameters("EnableStatic");
                    inPar["IPAddress"] = ip;//ip地址
                    inPar["SubnetMask"] = SubMark; //子网掩码   
                    mo.InvokeMethod("EnableStatic", inPar, null);//执行  
                }

                if (GateWay != null)
                {
                    inPar = mo.GetMethodParameters("SetGateways");
                    inPar["DefaultIPGateway"] = GateWay; //设置网关地址 1.网关;2.备用网关  
                    outPar = mo.InvokeMethod("SetGateways", inPar, null);//执行  
                }

                if (DNS != null)
                {
                    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    inPar["DNSServerSearchOrder"] = DNS; //设置DNS  1.DNS 2.备用DNS  
                    mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);// 执行  
                }
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
