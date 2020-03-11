using Rami.Wechat.Core.Comm;

namespace Rami.Wechat.Core.MiniProgram
{
    /// <summary>
    /// 小程序UserApi
    /// </summary>
    public class MpUserApi
    {
        /// <summary>
        /// 通过Code获取小程序用户信息
        /// </summary>
        private const string Url_Code2Session = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";

        /// <summary>
        /// 通过Code获取小程序用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static MpUserSession GetAppOpenId(string code)
        {
            var url = string.Format(Url_Code2Session, MpInterface.Conf.AppId, MpInterface.Conf.AppSecret, code);
            var res = WebApiHelper.GetAsync<MpUserSession>(url);
            return res;
        }

        /// <summary>
        /// 解析小程序用户加密数据
        /// </summary>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据，详细见加密数据解密算法</param>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static MpUser GetAppUserDecrypt(string encryptedData, string iv, string sessionKey)
        {
            string result = MpInterface.MpAesDecrypt(encryptedData, iv, sessionKey);
            var res = SerializeHelper.JsonDeserialize<MpUser>(result);
            return res;
        }

        /// <summary>
        /// 解析小程序手机加密数据
        /// </summary>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据，详细见加密数据解密算法</param>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static MpUserPhone GetPhoneNumber(string encryptedData, string iv, string sessionKey)
        {
            string result = MpInterface.MpAesDecrypt(encryptedData, iv, sessionKey);
            var res = SerializeHelper.JsonDeserialize<MpUserPhone>(result);
            return res;
        }
    }

    /// <summary>
    /// 小程序用户Session
    /// </summary>
    public class MpUserSession
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// 会话密钥
        /// </summary>
        public string session_key { get; set; }

        /// <summary>
        /// 用户在开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回
        /// </summary>
        public string unionid { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public string errcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSucc
        {
            get
            {
                return string.IsNullOrEmpty(errcode) || errcode == "0";
            }
        }
    }

    /// <summary>
    /// 小程序用户信息
    /// </summary>
    public class MpUser
    {
        /// <summary>
        /// openid
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// nickName
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// 性别 0：未知、1：男、2：女
        /// </summary>
        public int gender { get; set; }

        /// <summary>
        /// city
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// province
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// country
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// avatarUrl
        /// </summary>
        public string avatarUrl { get; set; }

        /// <summary>
        /// unionId
        /// </summary>
        public string unionId { get; set; }

        /// <summary>
        /// 水印
        /// </summary>
        public MpWatermark watermark { get; set; }
    }

    /// <summary>
    /// 小程序用户手机
    /// </summary>
    public class MpUserPhone
    {
        /// <summary>
        /// 用户绑定的手机号（国外手机号会有区号）
        /// </summary>
        public string phoneNumber { get; set; }

        /// <summary>
        /// 没有区号的手机号
        /// </summary>
        public string purePhoneNumber { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        public string countryCode { get; set; }

        /// <summary>
        /// 水印
        /// </summary>
        public MpWatermark watermark { get; set; }
    }

    /// <summary>
    /// 小程序用户手机水印
    /// </summary>
    public class MpWatermark
    {
        /// <summary>
        /// appid
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long timestamp { get; set; }
    }
}
