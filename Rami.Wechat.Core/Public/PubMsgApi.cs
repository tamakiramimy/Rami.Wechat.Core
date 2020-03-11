using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 被动回复消息接口
    /// </summary>
    public class PubMsgApi
    {
        /// <summary>
        /// ArticleFormat
        /// </summary>
        public const string ArticleFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[news]]></MsgType>
                                                <ArticleCount>{3}</ArticleCount>
                                                <Articles>
                                                    {4}
                                                </Articles>
                                              </xml> ";

        /// <summary>
        /// ArticleItemLFormat
        /// </summary>
        public const string ArticleItemLFormat = @"<item>
                                                     <Title><![CDATA[{0}]]></Title>
                                                     <Description><![CDATA[{1}]]></Description>
                                                     <PicUrl><![CDATA[{2}]]></PicUrl>
                                                     <Url><![CDATA[{3}]]></Url>
                                                   </item>";

        #region 消息模块

        /// <summary>
        /// 回复文本信息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="createTime"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string BuildTextMsg(string toUserName, string fromUserName, int createTime, string content)
        {
            var msg = new PubTextMsg() { Content = content, ToUserName = toUserName, FromUserName = fromUserName, CreateTime = createTime };
            return msg.ToXML();
        }

        /// <summary>
        /// 回复图片信息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="mediaID"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildImageMsg(string toUserName, string fromUserName, string mediaID, int createTime)
        {
            var msg = new PubImageMsg() { ToUserName = toUserName, FromUserName = fromUserName, MediaID = mediaID, CreateTime = createTime };
            return msg.ToXML();
        }

        /// <summary>
        /// 回复语音信息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="mediaID"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildVoiceMsg(string toUserName, string fromUserName, string mediaID, int createTime)
        {
            var msg = new PubVoiceMsg() { ToUserName = toUserName, FromUserName = fromUserName, MediaID = mediaID, CreateTime = createTime };
            return msg.ToXML();
        }

        /// <summary>
        /// 回复视频信息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildVideoMsg(string toUserName, string fromUserName, string mediaID, string title, string description, int createTime)
        {
            var msg = new PubVideoMsg() { ToUserName = toUserName, FromUserName = fromUserName, MediaID = mediaID, Description = description, Title = title };
            return msg.ToXML();
        }

        /// <summary>
        /// 回复音乐信息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="musicUrl"></param>
        /// <param name="hqMusicUrl"></param>
        /// <param name="thumbMediaId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildMusicMsg(string toUserName, string fromUserName, string musicUrl, string hqMusicUrl, string thumbMediaId, string title, string description, int createTime)
        {
            var msg = new PubMusicMsg() { ToUserName = toUserName, FromUserName = fromUserName, MusicUrl = musicUrl, HQMusicUrl = hqMusicUrl, ThumbMediaId = thumbMediaId, Description = description, Title = title };
            return msg.ToXML();
        }

        /// <summary>
        /// 创建多客服消息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildKfTransferMsg(string toUserName, string fromUserName, int createTime)
        {
            var msg = new PubKfTransferMsg { ToUserName = toUserName, FromUserName = fromUserName, CreateTime = createTime };
            return msg.ToXML();
        }

        /// <summary>
        /// 创建转接指定客服消息
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="createTime"></param>
        /// <param name="kfAccount"></param>
        /// <returns></returns>
        public static string BuldKfTransferAppointMsg(string toUserName, string fromUserName, int createTime, string kfAccount)
        {
            var msg = new PubKfTransferAppointMsg { ToUserName = toUserName, FromUserName = fromUserName, CreateTime = createTime, KfAccount = kfAccount };
            return msg.ToXML();
        }

        /// <summary>
        /// BuildArticleMsg
        /// </summary>
        /// <param name="toUserName">接收人（客服）</param>
        /// <param name="fromUserName">发送人（公众号）</param>
        /// <param name="createTime"></param>
        /// <param name="lstMArticles"></param>
        /// <returns></returns>
        public static string BuildArticleMsg(string toUserName, string fromUserName, int createTime, List<PubMediaArticle> lstMArticles)
        {
            var lstAricles = ConvertMediaToNews(lstMArticles);
            return string.Format(ArticleFormat, toUserName, fromUserName, createTime, lstAricles.Count, ArticlesToXML(lstAricles));
        }

        /// <summary>
        /// 把公众号图文转换成普通图文
        /// </summary>
        /// <param name="lstMArticles"></param>
        /// <returns></returns>
        public static List<PubNewsArticle> ConvertMediaToNews(List<PubMediaArticle> lstMArticles)
        {
            var res = new List<PubNewsArticle>();
            foreach (var item in lstMArticles)
            {
                PubNewsArticle temp = new PubNewsArticle();
                temp.Title = item.title;
                temp.Description = item.digest;
                temp.PicUrl = item.thumb_url;
                temp.Url = item.url;
                res.Add(temp);
            }

            return res;
        }

        /// <summary>
        /// 回复图文消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="articles"></param>
        /// <returns></returns>
        public static string BuildArticleMsg(string toUserName, string fromUserName, int createTime, List<PubNewsArticle> articles)
        {
            return string.Format(ArticleFormat, toUserName, fromUserName, createTime, articles.Count, ArticlesToXML(articles));
        }

        /// <summary>
        /// ArticlesToXML
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string ArticlesToXML(List<PubNewsArticle> list)
        {
            var result = string.Empty;
            foreach (var item in list)
            {
                result += string.Format(ArticleItemLFormat, item.Title, item.Description, item.PicUrl, item.Url);
            }
            return result;
        }

        #endregion
    }

    #region 实体

    /// <summary>
    /// 消息基类
    /// </summary>
    [XmlRootAttribute("xml")]
    public partial class PubBaseMsg
    {
        /// <summary>
        /// 接收人（默认公众号）
        /// </summary>
        [XmlIgnore]
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送人（用户）
        /// </summary>
        [XmlIgnore]
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public int CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlIgnore]
        public DateTime? DtCreateTime
        {
            get
            {
                //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
                return startTime.AddSeconds(CreateTime);
            }
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        [XmlIgnore]
        public PubMsgType MsgType { get; set; }
    }

    /// <summary>
    /// 消息接收体
    /// </summary>
    [XmlRootAttribute("xml")]
    public class PubReceiveMsg : PubBaseMsg
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public Int64 MsgId { get; set; }

        /// <summary>
        /// 	语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string ThumbMediaId { get; set; }

        /// <summary>
        /// 地理位置维度
        /// </summary>
        public double Location_X { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Location_Y { get; set; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 事件类型，subscribe
        /// </summary>
        [XmlIgnore]
        public PubEventType? Event { get; set; }

        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public string EventKey { get; set; }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; set; }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public double Precision { get; set; }

        /// <summary>
        /// 企业应用ID
        /// </summary>
        public int AgentID { get; set; }

        /// <summary>
        /// 图文消息图片
        /// </summary>
        public string PicUrl { get; set; }

        #region 门店通知消息

        /// <summary>
        /// 商户自己内部ID，即字段中的sid
        /// </summary>
        public string UniqId { get; set; }
        /// <summary>
        /// 微信的门店ID，微信内门店唯一标示ID
        /// </summary>
        public string PoiId { get; set; }
        /// <summary>
        /// 审核结果，成功succ 或失败fail
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 成功的通知信息，或审核失败的驳回理由
        /// </summary>
        public string Msg { get; set; }

        #endregion
    }

    /// <summary>
    /// PubReceiveMsgCData
    /// </summary>
    [XmlRootAttribute("xml")]
    public class PubReceiveMsgCData : PubReceiveMsg
    {
        /// <summary>
        /// ToUserNameCData
        /// </summary>
        [XmlElement("ToUserName")]
        public XmlNode ToUserNameCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(this.ToUserName);
            }
            set
            {
                this.ToUserName = value.Value;
            }
        }

        /// <summary>
        /// FromUserNameCData
        /// </summary>
        [XmlElement("FromUserName")]
        public XmlNode FromUserNameCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(this.FromUserName);
            }
            set
            {
                this.FromUserName = value.Value;
            }
        }

        /// <summary>
        /// MsgTypeCData
        /// </summary>
        [XmlElement("MsgType")]
        public XmlNode MsgTypeCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(this.MsgType.ToString());
            }
            set
            {
                this.MsgType = (PubMsgType)Enum.Parse(typeof(PubMsgType), value.Value, true);
            }
        }

        /// <summary>
        /// EventCData
        /// </summary>
        [XmlElement("Event")]
        public XmlNode EventCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(this.Event.ToString());
            }
            set
            {
                this.Event = (PubEventType)Enum.Parse(typeof(PubEventType), value.Value, true);
            }
        }

        /// <summary>
        /// ContentCData
        /// </summary>
        [XmlElement("Content")]
        [XmlIgnore]
        public XmlNode ContentCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(this.Content);
            }
            set
            {
                this.Content = value.Value;
            }
        }
    }

    /// <summary>
    /// 文本消息
    /// </summary>
    public class PubTextMsg : PubBaseMsg
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Msg
        /// </summary>
        public const string Msg = @"<xml>
                                         <ToUserName><![CDATA[{0}]]></ToUserName>
                                         <FromUserName><![CDATA[{1}]]></FromUserName>
                                         <CreateTime>{2}</CreateTime>
                                         <MsgType><![CDATA[text]]></MsgType>
                                         <Content><![CDATA[{3}]]></Content>
                                   </xml>";
        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string content)
        {
            return string.Format(Msg, toUserName, fromUserName, createTime, content);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, Content);
        }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class PubImageMsg : PubBaseMsg
    {
        /// <summary>
        /// MsgFormat
        /// </summary>
        public const string MsgFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[image]]></MsgType>
                                                <Image>
                                                <MediaId><![CDATA[{3}]]></MediaId>
                                                </Image>
                                             </xml>";
        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="mediaID"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string mediaID)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime, mediaID);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, MediaID);
        }

        /// <summary>
        /// 通过素材管理接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaID { get; set; }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class PubVoiceMsg : PubBaseMsg
    {
        /// <summary>
        /// MsgFormat
        /// </summary>
        public const string MsgFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[voice]]></MsgType>
                                                <Voice>
                                                <MediaId><![CDATA[{3}]]></MediaId>
                                                </Voice>
                                             </xml>";

        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="mediaID"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string mediaID)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime, mediaID);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, MediaID);
        }

        /// <summary>
        /// 通过素材管理接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaID { get; set; }
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    public class PubVideoMsg : PubBaseMsg
    {
        /// <summary>
        /// MsgFormat
        /// </summary>
        public const string MsgFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[video]]></MsgType>
                                                <Video>
                                                    <MediaId><![CDATA[{3}]]></MediaId>
                                                    <Title><![CDATA[{4}]]></Title>
                                                    <Description><![CDATA[{5}]]></Description>
                                                </Video> 
                                             </xml>";

        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string mediaID, string title, string description)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime, mediaID, title, description);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, MediaID, Title, Description);
        }

        /// <summary>
        ///通过素材管理接口上传多媒体文件，得到的id 
        /// </summary>
        public string MediaID { get; set; }

        /// <summary>
        /// 视频消息的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视频消息的描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 音乐消息
    /// </summary>
    public class PubMusicMsg : PubBaseMsg
    {
        /// <summary>
        /// MsgFormat
        /// </summary>
        public const string MsgFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[music]]></MsgType>
                                                <Music>
                                                    <Title><![CDATA[{3}]]></Title>
                                                    <Description><![CDATA[{4}]]></Description>
                                                    <MusicUrl><![CDATA[{5}]]></MusicUrl>
                                                    <HQMusicUrl><![CDATA[{6}]]></HQMusicUrl>
                                                    <ThumbMediaId><![CDATA[{7}]]></ThumbMediaId>
                                                </Music>
                                             </xml>";

        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="musicUrl"></param>
        /// <param name="hQMusicUrl"></param>
        /// <param name="thumbMediaId"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string title, string description, string musicUrl, string hQMusicUrl, string thumbMediaId)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime, title, description, musicUrl, hQMusicUrl, thumbMediaId);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, Title, Description, MusicUrl, HQMusicUrl, ThumbMediaId);
        }

        /// <summary>
        /// 音乐标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 音乐描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 音乐链接
        /// </summary>
        public string MusicUrl { get; set; }

        /// <summary>
        /// 高质量音乐链接，WIFI环境优先使用该链接播放音乐
        /// </summary>
        public string HQMusicUrl { get; set; }

        /// <summary>
        /// 缩略图的媒体id，通过素材管理接口上传多媒体文件，得到的id
        /// </summary>
        public string ThumbMediaId { get; set; }
    }

    /// <summary>
    /// PubNewsArticle
    /// </summary>
    public class PubNewsArticle
    {
        /// <summary>
        /// 图文消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图文消息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// 点击图文消息跳转链接
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 转发多客服消息
    /// </summary>
    public class PubKfTransferMsg : PubBaseMsg
    {
        /// <summary>
        /// MsgFormat
        /// </summary>
        public const string MsgFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[transfer_customer_service]]></MsgType>
                                                </xml>";
        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime);
        }
    }

    /// <summary>
    /// 转发指定客服消息
    /// </summary>
    public class PubKfTransferAppointMsg : PubBaseMsg
    {
        /// <summary>
        /// MsgFormat
        /// </summary>
        public const string MsgFormat = @"<xml>
                                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                                    <CreateTime>{2}</CreateTime>
                                                    <MsgType><![CDATA[transfer_customer_service]]></MsgType>
                                                    <TransInfo>
                                                        <KfAccount>{3}</KfAccount>
                                                    </TransInfo>
                                          </xml>";
        /// <summary>
        /// ToXML
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="kfAccount"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string kfAccount)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime, kfAccount);
        }

        /// <summary>
        /// ToXML
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, KfAccount);
        }

        /// <summary>
        /// 指定的客服账号
        /// </summary>
        public string KfAccount { get; set; }
    }

    /// <summary>
    /// 客户接入客服消息
    /// </summary>
    public class PubKfCreateSessionMsg : PubBaseMsg
    {
        /// <summary>
        /// 客户接入客服消息
        /// </summary>
        public const string MsgFormat = @"<xml><ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[event]]></MsgType>
                                                <Event><![CDATA[kf_create_session]]></Event>
                                                <KfAccount><![CDATA[{3}]]></KfAccount>
                                          </xml>";
        /// <summary>
        /// 客户接入客服消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="kfAccount"></param>
        /// <returns></returns>
        public static string ToXML(string toUserName, string fromUserName, int createTime, string kfAccount)
        {
            return string.Format(MsgFormat, toUserName, fromUserName, createTime, kfAccount);
        }

        /// <summary>
        /// 客户接入客服消息
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            return ToXML(ToUserName, FromUserName, CreateTime, KfAccount);
        }

        /// <summary>
        /// 指定的客服账号
        /// </summary>
        public string KfAccount { get; set; }
    }

    #endregion
}
