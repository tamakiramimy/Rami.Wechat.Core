using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rami.Wechat.Core.Comm
{
    /// <summary>
    /// 微信配置帮助类
    /// </summary>
    public class WechatConfigHelper
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public static string BasePath = AppContext.BaseDirectory;

        /// <summary>
        /// 配置文件Json床
        /// </summary>
        public static string StrJson { get; set; }

        /// <summary>
        /// 微信配置
        /// </summary>
        public static WechatConf WechatConf { get; set; }

        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePath(string fileName)
        {
            return Path.Combine(BasePath, fileName);
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static WechatConfigHelper()
        {
            // 配置文件路径
            var jsonPath = GetFilePath("Wechat.json");
            // Json串
            StrJson = File.ReadAllText(jsonPath);
            if (!string.IsNullOrEmpty(StrJson))
            {
                WechatConf = SerializeHelper.JsonDeserialize<WechatConf>(StrJson);
            }
        }
    }

    /// <summary>
    /// 微信配置
    /// </summary>
    public class WechatConf
    {
        /// <summary>
        /// 公众号配置
        /// </summary>
        public PublicConf Public { get; set; }

        /// <summary>
        /// 企业号配置
        /// </summary>
        public EnterpriseConf Enterprise { get; set; }

        /// <summary>
        /// 小程序配置
        /// </summary>
        public MiniProgramConf MiniProgram { get; set; }

        /// <summary>
        /// 商户号配置
        /// </summary>
        public MerchantConfs Merchants { get; set; }
    }

    /// <summary>
    /// 公众号配置
    /// </summary>
    public class PublicConf
    {
        /// <summary>
        /// 公众号Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 公众号私钥
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 公众号服务器验证签名Token
        /// </summary>
        public string AppToken { get; set; }
        /// <summary>
        /// 公众号AES加密KEY
        /// </summary>
        public string EncodingAesKey { get; set; }
        /// <summary>
        /// 公众号AccessToken请求地址
        /// </summary>
        public string TokenUrl { get; set; }
        /// <summary>
        /// 公众号Ticket请求地址
        /// </summary>
        public string TicketUrl { get; set; }
        /// <summary>
        /// 公众号附属服务器验证Token
        /// </summary>
        public string TokenCode { get; set; }
    }

    /// <summary>
    /// 企业号配置
    /// </summary>
    public class EnterpriseConf
    {
        /// <summary>
        /// 企业号Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 企业号秘钥
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 企业号服务器验证Token
        /// </summary>
        public string AppToken { get; set; }
        /// <summary>
        /// 企业号AES加密KEY
        /// </summary>
        public string EncodingAesKey { get; set; }
        /// <summary>
        /// 企业号应用Id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 外部联系人密钥
        /// </summary>
        public string OutSecret { get; set; }
        /// <summary>
        /// 企业号 AccessToken 请求地址
        /// </summary>
        public string TokenUrl { get; set; }
        /// <summary>
        /// 企业号 JsTicket 请求地址
        /// </summary>
        public string TicketUrl { get; set; }
        /// <summary>
        /// 企业号 OutAccess 请求地址
        /// </summary>
        public string OutAccessUrl { get; set; }
    }

    /// <summary>
    /// 小程序配置
    /// </summary>
    public class MiniProgramConf
    {
        /// <summary>
        /// 小程序Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 小程序私钥
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 小程序服务器验证签名Token
        /// </summary>
        public string AppToken { get; set; }
        /// <summary>
        /// 小程序AES加密KEY
        /// </summary>
        public string EncodingAesKey { get; set; }
        /// <summary>
        /// 小程序AccessToken请求地址
        /// </summary>
        public string TokenUrl { get; set; }
        /// <summary>
        /// 小程序附属服务器验证Token
        /// </summary>
        public string TokenCode { get; set; }

        /// <summary>
        /// 小程序 JsTicket 请求地址
        /// </summary>
        public string TicketUrl { get; set; }
    }

    /// <summary>
    /// 商户号支付配置
    /// </summary>
    public class MerchantConfs
    {
        /// <summary>
        /// 公众号支付配置
        /// </summary>
        public MerchantConf Public { get; set; }

        /// <summary>
        /// 企业号支付配置
        /// </summary>
        public MerchantConf Enterprise { get; set; }

        /// <summary>
        /// 小程序支付配置
        /// </summary>
        public MerchantConf MiniProgram { get; set; }
    }

    /// <summary>
    /// 商户号配置
    /// </summary>
    public class MerchantConf
    {
        /// <summary>
        /// 商户号Id
        /// </summary>
        public int MchId { get; set; }
        /// <summary>
        /// 支付Key
        /// </summary>
        public string PayKey { get; set; }
        /// <summary>
        /// 支付秘钥
        /// </summary>
        public string PaySecret { get; set; }
        /// <summary>
        /// 证书地址
        /// </summary>
        public string CertPath { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPass { get; set; }
        /// <summary>
        /// 支付结果通知地址
        /// </summary>
        public string NotifyUrl { get; set; }
    }

    /// <summary>
    /// 商户号类型
    /// </summary>
    public enum PayConfType
    {
        /// <summary>
        /// 公众号
        /// </summary>
        Public = 1,
        /// <summary>
        /// 企业号
        /// </summary>
        Enterprise = 2,
        /// <summary>
        /// 小程序
        /// </summary>
        MiniProgram = 3
    }
}
