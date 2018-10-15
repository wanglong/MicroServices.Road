using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace LZZ.DEV.WebServer
{
    public static class NetworkHelper
    {
        /// <summary>
        /// 得到本机IP
        /// </summary>
        public static string GetLocalIp()
        {
            //得到计算机名
            var strPcName = Dns.GetHostName();
            //得到本机IP地址数组
            var ipEntry = Dns.GetHostEntry(strPcName);
            //遍历数组
            foreach (var ipAddress in ipEntry.AddressList)
            {
                //判断当前字符串是否为正确IP地址
                if (!IsRightIp(ipAddress.ToString())) continue;
                //得到本地IP地址
                return ipAddress.ToString();
            }

            return "";
        }

        /// <summary>
        /// 判断是否为正确的IP地址
        /// </summary>
        /// <param name="strIpAdd">需要判断的字符串</param>
        /// <returns>true = 是 false = 否</returns>
        public static bool IsRightIp(string strIpAdd)
        {
            //利用正则表达式判断字符串是否符合IPv4格式
            if (!Regex.IsMatch(strIpAdd, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}")) return false;
            //根据小数点分拆字符串
            var ips = strIpAdd.Split('.');
            if (ips.Length == 4 || ips.Length == 6)
            {
                return int.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256;
            }

            return false;
        }

        /// <summary>
        /// 尝试Ping指定IP是否能够Ping通
        /// </summary>
        /// <param name="strIp">指定IP</param>
        /// <returns>true 是 false 否</returns>
        public static bool IsPingIp(string strIp)
        {
            try
            {
                //接受Ping返回值
                new Ping().Send(strIp, 1000);
                //Ping通
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}