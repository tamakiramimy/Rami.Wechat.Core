using Rami.Wechat.Core.Comm;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 企业号接口
    /// </summary>
    public class EntInterface
    {
        /// <summary>
        /// 配置
        /// </summary>
        public static EnterpriseConf Conf { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        static EntInterface()
        {
            Conf = WechatConfigHelper.WechatConf.Enterprise;
        }

        #region AccessToken

        /// <summary>
        /// token
        /// </summary>
        private static EntTokenResult acToken = new EntTokenResult();

        /// <summary>
        /// 调用令牌
        /// </summary>
        /// <returns></returns>
        public static EntTokenResult GetToken(string secret)
        {
            var url = string.Format(Conf.TokenUrl, Conf.AppId, secret);
            return WebApiHelper.GetAsync<EntTokenResult>(url);
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
                    var token = GetToken(Conf.AppSecret);
                    WechatHelper.Log.Error($"AccessToken：获取到AccessToken：{SerializeHelper.JsonSerialize(token)}");
                    if (token.access_token != acToken.access_token)
                    {
                        jsSdkTicket.Overtime = DateTime.MinValue;
                    }

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
            acToken = new EntTokenResult();
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
        private static EntTicketResult jsSdkTicket = new EntTicketResult();

        /// <summary>
        /// 请求ticket
        /// </summary>
        private static EntTicketResult GetJsSdkTicket()
        {
            var url = string.Format(Conf.TicketUrl, AccessToken);
            var res = WebApiHelper.GetAsync<EntTicketResult>(url);
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

        #region 外部联系人Token

        /// <summary>
        /// 外部联系人Token
        /// </summary>
        private static EntTokenResult outToken = new EntTokenResult();

        /// <summary>
        /// 外部联系人Token
        /// </summary>
        public static string OutAccessToken
        {
            get
            {
                if (outToken.IsOvertime)
                {
                    WechatHelper.Log.Error($"OutAccessToken:超时进入：{outToken.Overtime}");
                    var token = GetToken(Conf.OutSecret);
                    WechatHelper.Log.Error($"OutAccessToken：获取到外部联系人Token：{SerializeHelper.JsonSerialize(token)}");

                    // 超时前60秒提前更新token
                    outToken = token;
                    outToken.Overtime = DateTime.Now.AddSeconds(-60);
                }

                return outToken.access_token;
            }
        }

        #endregion

        #region 获取用户身份

        /// <summary>
        /// 获取用户Id
        /// </summary>
        private const string GET_USERID_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}";

        /// <summary>
        /// 读取用户ID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static EntCodeResult GetUserID(string code)
        {
            var url = string.Format(GET_USERID_URL, AccessToken, code);
            return WebApiHelper.GetAsync<EntCodeResult>(url);
        }

        #endregion

        #region OpenId和UserId互换

        /// <summary>
        /// UserId转OpenId
        /// </summary>
        private const string ConvertToOpenidURL = "https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_openid?access_token={0}";

        /// <summary>
        /// OpenId转UserId
        /// </summary>
        private const string ConvertToUseridURL = "https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_userid?access_token={0}";

        /// <summary>
        /// UserId转OpenId
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="agentid">应用ID，默认为空</param>
        /// <returns></returns>
        public static EntCodeResult ConvertToOpenid(string userid, int? agentid = null)
        {
            var url = string.Format(ConvertToOpenidURL, AccessToken);
            if (agentid == null)
            {
                return WebApiHelper.PostAsync<EntCodeResult>(url, new { userid = userid });
            }
            else
            {
                return WebApiHelper.PostAsync<EntCodeResult>(url, new { userid = userid, agentid = agentid });
            }
        }

        /// <summary>
        /// OpenId转UserId
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static EntCodeResult ConvertToUserid(string openid)
        {
            var url = string.Format(ConvertToUseridURL, AccessToken);
            return WebApiHelper.PostAsync<EntCodeResult>(url, new { openid = openid });
        }

        #endregion

        #region 文件上传

        /// <summary>
        /// 文件界限
        /// </summary>
        public static string Boundary = "-------------------------acebdf13572468";

        /// <summary>
        /// form文件上传
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MultipartFormDataContent BuildFromContent(string filePath)
        {
            return BuildFromContent(Path.GetFileName(filePath), File.OpenRead(filePath));
        }

        /// <summary>
        /// form文件头
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static MultipartFormDataContent BuildFromContent(string fileName, Stream file)
        {
            MultipartFormDataContent form = new MultipartFormDataContent(Boundary);
            // 文件头
            form.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data;boundary=" + Boundary);
            var content = BuildStreamContent(fileName, file);
            form.Add(content);
            return form;
        }

        /// <summary>
        /// 文件流
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static StreamContent BuildStreamContent(string fileName, Stream file)
        {
            StreamContent fileContent = new StreamContent(file);
            // 文件流
            string contentDisposition = string.Format("form-data; name=\"media\";filename=\"{0}\"; filelength={1}", fileName, file.Length);
            fileContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(contentDisposition);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return fileContent;
        }

        #endregion
    }

    /// <summary>
    /// 结果
    /// </summary>
    public class EntApiResult
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
                    EntInterface.InitToken();
                }

                return errcode == 0;
            }
        }
    }

    /// <summary>
    /// 令牌设置
    /// </summary>
    public class EntTokenResult : EntApiResult
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public EntTokenResult()
        {
            Overtime = DateTime.MinValue;
        }

        /// <summary>
        /// access_token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// expires_in
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// Overtime
        /// </summary>
        public DateTime Overtime { get; set; }

        /// <summary>
        /// IsOvertime
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
    /// EntTicketResult
    /// </summary>
    public class EntTicketResult : EntApiResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EntTicketResult()
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
    /// Code转UserID 
    /// </summary>
    public class EntCodeResult : EntApiResult
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// DeviceId
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Appid
        /// </summary>
        public string Appid { get; set; }
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum EntMsgType
    {
        /// <summary>
        /// 文本
        /// </summary>
        text,

        /// <summary>
        /// 图片
        /// </summary>
        image,

        /// <summary>
        /// 语音
        /// </summary>
        voice,

        /// <summary>
        /// 视频
        /// </summary>
        video,

        /// <summary>
        /// 卡券
        /// </summary>
        wxcard,

        /// <summary>
        /// 图文
        /// </summary>
        mpnews,

        /// <summary>
        /// 事件
        /// </summary>
        @event,

        /// <summary>
        /// 短视频
        /// </summary>
        shortvideo,

        /// <summary>
        /// 定位
        /// </summary>
        location,

        /// <summary>
        /// 链接
        /// </summary>
        link,

        /// <summary>
        /// 图文消息
        /// </summary>
        news,

        /// <summary>
        /// 音乐消息
        /// </summary>
        music,
    }

    /// <summary>
    /// 企业号JsSdk
    /// </summary>
    public class EntJsSdkData
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="url"></param>
        public EntJsSdkData(string url)
        {
            Url = url;
        }

        /// <summary>
        /// AppID
        /// </summary>
        public string AppID = EntInterface.Conf.AppId;

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
                return WechatHelper.Sha1Sign("&", "timestamp=" + Timestamp, "noncestr=" + Noncestr, "jsapi_ticket=" + EntInterface.JsSdkTicket, "url=" + Url);
            }
        }
    }
}
