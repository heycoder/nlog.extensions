using System.Linq;
using System.Net;

namespace HeyCoder.NLog.Extensions.Utils
{
    internal class HostUtil
    {
        private const string DefaultIp = "未知ip";
        public static string GetHostIp()
        {
            try
            {
                string hostName = Dns.GetHostName(); //本机名   
                IPAddress[] addressList = Dns.GetHostAddresses(hostName); //会返回所有地址，包括IPv4和IPv6   
                return addressList.Where(ip => !ip.IsIPv6LinkLocal).Aggregate(string.Empty, (current, ip) => current + ip + ",").TrimEnd(',');
            }
            catch
            {
                return DefaultIp;
            }

        }

    }
}
