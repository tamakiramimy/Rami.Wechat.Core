using Rami.Wechat.Core.Comm;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 公众号接口
    /// </summary>
    public class PubInterface
    {
        /// <summary>
        /// 公众号配置
        /// </summary>
        public static PublicConf Conf { get; set; }

        /// <summary>
        /// 静态初始化
        /// </summary>
        static PubInterface()
        {
            Conf = WechatConfigHelper.WechatConf.Public;
        }

        #region AccessToken

        /// <summary>
        /// AccessToken
        /// </summary>
        private static PubTokenResult acToken = new PubTokenResult();

        /// <summary>
        /// 调用令牌
        /// </summary>
        /// <returns></returns>
        public static PubTokenResult GetToken()
        {
            string url = string.Format(Conf.TokenUrl, Conf.AppId, Conf.AppSecret);
            WechatHelper.Log.Debug("GetToken:url:" + url);
            return WebApiHelper.GetAsync<PubTokenResult>(url);
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
                    WechatHelper.Log.Error("超时进入：" + acToken.Overtime);
                    var token = GetToken();
                    WechatHelper.Log.Error("AccessToken：" + token.JsonSerialize());
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
            acToken = new PubTokenResult();
        }

        /// <summary>
        /// 手动更新token
        /// </summary>
        public static void UpdToken()
        {
            acToken.Overtime = DateTime.MinValue;
            var token = AccessToken;
            WechatHelper.Log.Debug("UpdAccessToken:" + token);
        }

        #endregion

        #region JsSdkTicket

        /// <summary>
        /// JsSdkTicket
        /// </summary>
        private static PubTicketResult jsSdkTicket = new PubTicketResult();

        /// <summary>
        /// 请求ticket
        /// </summary>
        private static PubTicketResult GetJsSdkTicket()
        {
            var url = string.Format(Conf.TicketUrl, AccessToken);
            using (var client = new HttpClient())
            {
                var res = WebApiHelper.GetAsync<PubTicketResult>(url);
                WechatHelper.Log.Debug($"GetJsSdkTicket:获取到JsTicekt结果:{SerializeHelper.JsonSerialize(res)}");
                return res;
            }
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
                    WechatHelper.Log.Debug($"JsSdkTicket：获取到ticket结果:{ SerializeHelper.JsonSerialize(jsSdkTicket)}");
                    // 设置token提前60秒超时
                    jsSdkTicket.Overtime = DateTime.Now.AddSeconds(-60);
                }

                return jsSdkTicket.ticket;
            }
        }

        #endregion

        #region 微信卡券Ticket

        /// <summary>
        /// CardTicket请求地址
        /// </summary>
        private static readonly string cardTicketUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=wx_card";

        /// <summary>
        /// CardTicket
        /// </summary>
        private static PubTicketResult cardTicket = new PubTicketResult();

        /// <summary>
        /// JsSdkTicket
        /// </summary>
        public static string CardTicket
        {
            get
            {
                if (cardTicket.IsOvertime)
                {
                    GetCardTicket();
                    if (!cardTicket.IsSuss)
                    {
                        GetCardTicket();
                    }

                    WechatHelper.Log.Error("请求cardticket：" + cardTicket.errcode + " " + cardTicket.errmsg);
                }

                return cardTicket.ticket;
            }
        }

        /// <summary>
        /// 请求ticket
        /// </summary>
        private static void GetCardTicket()
        {
            var url = string.Format(cardTicketUrl, AccessToken);
            cardTicket = WebApiHelper.GetAsync<PubTicketResult>(url);
            cardTicket.Overtime = DateTime.Now.AddSeconds(cardTicket.expires_in - 1200);
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
        /// form文件上传
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
        /// form文件上传
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
    /// JsSdk
    /// </summary>
    public class PubJsSdkData
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="url"></param>
        public PubJsSdkData(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        public string AppId = PubInterface.Conf.AppId;

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp = WechatHelper.EpochTime;

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string Noncestr = Guid.NewGuid().ToString();

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature
        {
            get
            {
                return JsSdkSign(Timestamp, Noncestr, Url);
            }
        }

        /// <summary>
        /// 获取JS-SDK权限验证的签名
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="nonceStr"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string JsSdkSign(long timestamp, string nonceStr, string url)
        {
            try
            {
                //对所有待签名参数按照字段名的ASCII 码从小到大排序
                var string1Builder = new StringBuilder().Append("jsapi_ticket=").Append(PubInterface.JsSdkTicket).Append("&")
                              .Append("noncestr=").Append(nonceStr).Append("&")
                              .Append("timestamp=").Append(timestamp).Append("&")
                              .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
                var string1 = string1Builder.ToString();
                return WechatHelper.Sha1SignShare(string1).ToLower();
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 公众号Api结果
    /// </summary>
    public class PubApiResult
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
        /// 返回值
        /// </summary>
        public string res { get; set; }

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
                    PubInterface.InitToken();
                }

                return errcode == 0;
            }
        }
    }

    /// <summary>
    /// 令牌结果
    /// </summary>
    public class PubTokenResult : PubApiResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PubTokenResult()
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
    public class PubTicketResult : PubApiResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PubTicketResult()
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
    /// 消息类型枚举
    /// </summary>
    public enum PubMsgType
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
        /// 定位 用户手动Location_X,Location_Y
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
    /// 事件类型枚举
    /// </summary>
    public enum PubEventType
    {
        /// <summary>
        /// 关注
        /// </summary>
        subscribe,

        /// <summary>
        /// 取消关注
        /// </summary>
        unsubscribe,

        /// <summary>
        /// 扫码
        /// </summary>
        scan,

        /// <summary>
        /// 自动收集GPS定位信息,Latitude,Longitude有效
        /// </summary>
        location,

        /// <summary>
        /// 弹出地理位置选择器
        /// </summary>
        location_select,

        /// <summary>
        /// 点击
        /// </summary>
        click,

        /// <summary>
        /// 网页
        /// </summary>
        view,

        /// <summary>
        /// 门店
        /// </summary>
        poi_check_notify,

        /// <summary>
        /// 客服信息
        /// </summary>
        kf_create_session,

        /// <summary>
        /// 客服关闭连接
        /// </summary>
        kf_close_session,

        /// <summary>
        /// 客服转发
        /// </summary>
        kf_switch_session,
    }
}
