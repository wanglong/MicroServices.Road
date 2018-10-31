using System.Net;

namespace Rpc.Common.Easy.Rpc.Communally.Entitys.Address
{
    /// <summary>
    /// ip地址模型
    /// </summary>
    public sealed class IpAddressModel : AddressModel
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的ip地址模型实例
        /// </summary>
        public IpAddressModel()
        {
        }

        /// <summary>
        /// 初始化一个新的ip地址模型实例
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        public IpAddressModel(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        #endregion Property

        #region Overrides of AddressModel

        /// <summary>
        /// 创建终结点
        /// </summary>
        /// <returns></returns>
        public override EndPoint CreateEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Ip), Port);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Ip}:{Port}";
        }

        #endregion Overrides of AddressModel
    }
}