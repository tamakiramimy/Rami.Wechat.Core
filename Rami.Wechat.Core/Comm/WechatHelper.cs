using log4net;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net.Http;
using System.IO;
using log4net.Config;

namespace Rami.Wechat.Core.Comm
{
    /// <summary>
    /// 微信帮助类
    /// </summary>
    public static class WechatHelper
    {
        /// <summary>
        /// 微信日志
        /// </summary>
        public static ILog Log
        {
            get
            {
                return LogManager.GetLogger("Rami.Wechat.Core", "wx");
            }
        }

        ///// <summary>
        ///// 微信日志
        ///// </summary>
        //internal static ILogger logger;

        /// <summary>
        /// 构造方法
        /// </summary>
        static WechatHelper()
        {
            //var logFactory = StaticServiceProvider.Current.GetRequiredService<ILoggerFactory>();
            //logger = logFactory.CreateLogger("wx");
            //logger.LogDebug("wx");

            // log4net配置
            var Repository = LogManager.CreateRepository("Rami.Wechat.Core");
            //指定配置文件，如果这里你遇到问题，应该是使用了InProcess模式，请查看Blog.Core.csproj,并删之
            var contentPath = AppContext.BaseDirectory;
            var log4Config = Path.Combine(contentPath, "Log4net.config");
            XmlConfigurator.Configure(Repository, new FileInfo(log4Config));
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public static Int64 EpochTime
        {
            get
            {
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }
        }

        /// <summary>
        /// 字符集合
        /// </summary>
        private static string[] LetterDef = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// 创建随机字符串      
        /// </summary>
        /// <returns></returns>
        public static string GetNonceStr()
        {
            Random r = new Random();
            var sb = new StringBuilder();
            var length = LetterDef.Length;
            for (int i = 0; i < 15; i++)
            {
                sb.Append(LetterDef[r.Next(length - 1)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 创建随机字符串      
        /// </summary>
        /// <returns></returns>
        public static string GetNonceStrGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 获取服务器IP
        /// </summary>
        /// <returns></returns>
        public static string GetServerIP()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] myIP = Dns.GetHostAddresses(hostName);
            IPAddress resultIp = null;
            foreach (IPAddress address in myIP)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    resultIp = address;
                    break;
                }
            }

            if (resultIp == null)
            {
                resultIp = myIP[0];
            }

            return resultIp.ToString();
        }

        #region Sha1加密

        /// <summary>
        /// Sha1加密
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string Sha1Sign(params string[] keys)
        {
            return Sha1Sign(string.Empty, keys);
        }

        /// <summary>
        /// Sha1加密（排序 小写）
        /// </summary>
        /// <param name="spit"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string Sha1Sign(string spit, params string[] keys)
        {
            Array.Sort(keys);//字典排序
            string tmpStr = string.Join(spit, keys);
            HashAlgorithm sha1 = SHA1.Create();
            byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(tmpStr));
            var result = "";
            foreach (byte iByte in data)
            {
                result += iByte.ToString("x2");
            }

            return result.ToLower();
        }

        /// <summary>
        /// Sha1加密（微信分享用）
        /// </summary>
        /// <param name="orgStr"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Sha1SignShare(string orgStr, string encode = "UTF-8")
        {
            var sha1 = new SHA1Managed();
            var sha1bytes = Encoding.GetEncoding(encode).GetBytes(orgStr);
            byte[] resultHash = sha1.ComputeHash(sha1bytes);
            string sha1String = BitConverter.ToString(resultHash).ToLower();
            sha1String = sha1String.Replace("-", "");
            return sha1String;
        }

        #endregion

        #region 获取证书

        /// <summary>
        /// 获取证书
        /// </summary>
        /// <param name="certPath"></param>
        /// <param name="certPass"></param>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCert(string certPath, string certPass, string mchId = "")
        {
            // 证书默认密码为商户号Id
            if (string.IsNullOrEmpty(certPass))
            {
                certPass = mchId;
            }

            X509Certificate2 cert = new X509Certificate2(certPath, certPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return cert;
        }

        /// <summary>
        /// 带证书的请求
        /// </summary>
        /// <param name="certPath"></param>
        /// <param name="certPass"></param>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static HttpClientHandler GetRequestCert(string certPath, string certPass, string mchId = "")
        {
            var cert = GetCert(certPath, certPass, mchId);
            var certHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                UseDefaultCredentials = false
            };

            certHandler.ClientCertificates.Add(cert);
            return certHandler;
        }

        #endregion

        #region 商户号

        /// <summary>
        /// 根据当前系统时间加随机序列来生成订单号
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static string GenerateOutTradeNo(int mchId)
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", mchId, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        #endregion
    }
}
