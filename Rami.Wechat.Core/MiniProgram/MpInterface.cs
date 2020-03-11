using Rami.Wechat.Core.Comm;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Rami.Wechat.Core.MiniProgram
{
    /// <summary>
    /// 小程序接口
    /// </summary>
    public class MpInterface
    {
        /// <summary>
        /// 配置
        /// </summary>
        public static MiniProgramConf Conf { get; set; }

        /// <summary>
        /// 静态初始化
        /// </summary>
        static MpInterface()
        {
            Conf = WechatConfigHelper.WechatConf.MiniProgram;
        }

        #region 令牌

        /// <summary>
        /// token
        /// </summary>
        private static MpTokenResult acToken = new MpTokenResult();

        /// <summary>
        /// 调用令牌
        /// </summary>
        /// <returns></returns>
        private static MpTokenResult GetToken()
        {
            var url = string.Format(Conf.TokenUrl, Conf.AppId, Conf.AppSecret);
            return WebApiHelper.GetAsync<MpTokenResult>(url);
        }

        /// <summary>
        /// 微信AccessToken
        /// </summary>
        public static string AccessToken
        {
            get
            {
                if (acToken.IsOvertime)
                {
                    WechatHelper.Log.Error($"AccessToken：超时进入：{acToken.Overtime}");
                    var token = GetToken();
                    WechatHelper.Log.Error($"AccessToken：获取到AccessToken：{SerializeHelper.JsonSerialize(token)}");

                    // 超时前60秒提前更新token
                    acToken = token;
                    acToken.Overtime = DateTime.Now.AddSeconds(-60);
                }

                return acToken.access_token;
            }
        }

        /// <summary>
        /// 实始化token
        /// </summary>
        public static void InitToken()
        {
            acToken = new MpTokenResult();
        }

        /// <summary>
        /// 手动更新token
        /// </summary>
        public static void UpdToken()
        {
            acToken.Overtime = DateTime.MinValue;
            var token = AccessToken;
            WechatHelper.Log.Debug($"UpdToken:{token}");
        }

        #endregion

        #region JsApiTicket

        /// <summary>
        /// JsSdkTicket
        /// </summary>
        private static MpTicketResult jsSdkTicket = new MpTicketResult();

        /// <summary>
        /// 请求ticket
        /// </summary>
        private static MpTicketResult GetJsSdkTicket()
        {
            var url = string.Format(Conf.TicketUrl, AccessToken);
            var res = WebApiHelper.GetAsync<MpTicketResult>(url);
            WechatHelper.Log.Debug($"GetJsSdkTicket:获取到JsTicekt结果:{SerializeHelper.JsonSerialize(res)}");
            return res;
        }

        /// <summary>
        /// JsSdkTicket
        /// </summary>
        public static string JsSdkTicket
        {
            get
            {
                if (jsSdkTicket.IsOvertime)
                {
                    WechatHelper.Log.Debug($"JsSdkTicket:超时进入：{jsSdkTicket.Overtime}");
                    jsSdkTicket = GetJsSdkTicket();
                    WechatHelper.Log.Debug($"JsSdkTicket：获取到ticket结果:{SerializeHelper.JsonSerialize(jsSdkTicket)}");
                    // 设置token提前60秒超时
                    jsSdkTicket.Overtime = DateTime.Now.AddSeconds(-60);
                }

                return jsSdkTicket.ticket;
            }
        }

        #endregion

        /// <summary>
        /// 小程序AES解密算法
        /// </summary>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据，详细见加密数据解密算法</param>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static string MpAesDecrypt(string encryptedData, string iv, string sessionKey)
        {
            // 待解密原文
            byte[] encryData = Convert.FromBase64String(encryptedData);

            // 解密
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Key = Convert.FromBase64String(sessionKey);
            rijndaelCipher.IV = Convert.FromBase64String(iv);
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryData, 0, encryData.Length);
            string result = Encoding.UTF8.GetString(plainText);
            return result;
        }
    }

    /// <summary>
    /// API结果
    /// </summary>
    public class MpApiRes
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// errcode为空或者为0都为成功
        /// </summary>
        public virtual bool IsSuss
        {
            get
            {
                //如果令牌超时，清除令牌缓存重新读取
                if (errcode == 42001)
                {
                    MpInterface.InitToken();
                }

                return errcode == 0;
            }
        }
    }

    /// <summary>
    /// API结果
    /// </summary>
    public class MpApiResult : MpApiRes
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public string res { get; set; }
    }

    /// <summary>
    /// 令牌结果
    /// </summary>
    public class MpTokenResult : MpApiResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MpTokenResult()
        {
            Overtime = DateTime.MinValue;
        }

        /// <summary>
        /// token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime Overtime { get; set; }

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool IsOvertime
        {
            get
            {
                return Overtime.AddSeconds(expires_in) < DateTime.Now;
            }
        }
    }

    /// <summary>
    /// ticket结果
    /// </summary>
    public class MpTicketResult : MpApiResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MpTicketResult()
        {
            Overtime = DateTime.MinValue;
        }

        /// <summary>
        /// ticket
        /// </summary>
        public string ticket { get; set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime Overtime { get; set; }

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool IsOvertime
        {
            get
            {
                return Overtime.AddSeconds(expires_in) < DateTime.Now;
            }
        }
    }

    /// <summary>
    /// 小程序JsSdk
    /// </summary>
    public class MpJsSdkData
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="url"></param>
        public MpJsSdkData(string url)
        {
            Url = url;
        }

        /// <summary>
        /// AppID
        /// </summary>
        public string AppID = MpInterface.Conf.AppId;

        /// <summary>
        /// Timestamp
        /// </summary>
        public long Timestamp = WechatHelper.EpochTime;

        /// <summary>
        /// Noncestr
        /// </summary>
        public string Noncestr = Guid.NewGuid().ToString();

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Signature
        /// </summary>
        public string Signature
        {
            get
            {
                return WechatHelper.Sha1Sign("&", "timestamp=" + Timestamp, "noncestr=" + Noncestr, "jsapi_ticket=" + MpInterface.JsSdkTicket, "url=" + Url);
            }
        }
    }
}
