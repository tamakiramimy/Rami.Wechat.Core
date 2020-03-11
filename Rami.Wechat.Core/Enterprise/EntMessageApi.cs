using Rami.Wechat.Core.Comm;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// EntMessageApi
    /// </summary>
    public class EntMessageApi
    {
        #region 常量

        /// <summary>
        /// TextMsg
        /// </summary>
        public const string TextMsg = @"<xml>
                                         <ToUserName><![CDATA[{0}]]></ToUserName>
                                         <FromUserName><![CDATA[{1}]]></FromUserName>
                                         <CreateTime>{2}</CreateTime>
                                         <MsgType><![CDATA[text]]></MsgType>
                                         <Content><![CDATA[{3}]]></Content>
                                   </xml>";

        /// <summary>
        /// IamgeFormat
        /// </summary>
        public const string IamgeFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[image]]></MsgType>
                                                <Image>
                                                <MediaId><![CDATA[{3}]]></MediaId>
                                                </Image>
                                             </xml>";

        /// <summary>
        /// VoiceFormat
        /// </summary>
        public const string VoiceFormat = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[voice]]></MsgType>
                                                <Voice>
                                                <MediaId><![CDATA[{3}]]></MediaId>
                                                </Voice>
                                             </xml>";

        /// <summary>
        /// VideoFormat
        /// </summary>
        public const string VideoFormat = @"<xml>
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
        /// MusicFormat
        /// </summary>
        public const string MusicFormat = @"<xml>
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

        /// <summary>
        /// SEND_MESSAGE_URL
        /// </summary>
        private const string SEND_MESSAGE_URL = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";

        #endregion

        #region 消息模块

        /// <summary>
        /// 回复文本信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="toUserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildTextMsg(string content, string toUserName, int createTime)
        {
            return string.Format(TextMsg, toUserName, EntInterface.Conf.AppId, createTime, content);
        }

        /// <summary>
        /// 回复图片信息
        /// </summary>
        /// <param name="mediaID"></param>
        /// <param name="toUserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildImageMsg(string mediaID, string toUserName, int createTime)
        {
            return string.Format(IamgeFormat, toUserName, EntInterface.Conf.AppId, createTime, mediaID);
        }

        /// <summary>
        /// 回复语音信息
        /// </summary>
        /// <param name="mediaID"></param>
        /// <param name="toUserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildVoiceMsg(string mediaID, string toUserName, int createTime)
        {
            return string.Format(VoiceFormat, toUserName, EntInterface.Conf.AppId, createTime, mediaID);
        }

        /// <summary>
        /// 回复视频信息
        /// </summary>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="toUserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildVideoMsg(string mediaID, string title, string description, string toUserName, int createTime)
        {
            return string.Format(VideoFormat, toUserName, EntInterface.Conf.AppId, createTime, mediaID, title, description);
        }

        /// <summary>
        /// 回复音乐信息
        /// </summary>
        /// <param name="musicUrl"></param>
        /// <param name="hqMusicUrl"></param>
        /// <param name="thumbMediaId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="toUserName"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public static string BuildMusicMsg(string musicUrl, string hqMusicUrl, string thumbMediaId, string title, string description, string toUserName, int createTime)
        {
            return string.Format(MusicFormat, toUserName, EntInterface.Conf.AppId, createTime, title, description, musicUrl, hqMusicUrl, thumbMediaId);
        }

        /// <summary>
        /// 回复图文消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="createTime"></param>
        /// <param name="articles"></param>
        /// <returns></returns>
        public static string BuildArticleMsg(string toUserName, int createTime, List<EntNewsArticle> articles)
        {
            return string.Format(ArticleFormat, toUserName, EntInterface.Conf.AppId, createTime, articles.Count, ArticlesToXML(articles));
        }

        /// <summary>
        /// ArticlesToXML
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string ArticlesToXML(List<EntNewsArticle> list)
        {
            var result = string.Empty;
            foreach (var item in list)
            {
                result += string.Format(ArticleItemLFormat, item.title, item.description, item.picurl, item.url);
            }

            return result;
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="content"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendTextMsg(string touser, string toparty, string totag, int agentid, string content, int safe = 0)
        {
            return SendMsg(new { touser = touser, toparty = toparty, totag = totag, agentid = agentid, msgtype = EntMsgType.text.ToString(), text = new { content = content }, safe = safe });
        }

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="media_id"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendImageMsg(string touser, string toparty, string totag, int agentid, string media_id, int safe = 0)
        {
            return SendMsg(new { touser = touser, toparty = toparty, totag = totag, agentid = agentid, msgtype = EntMsgType.image.ToString(), image = new { media_id = media_id }, safe = safe });
        }

        /// <summary>
        /// 发送音频消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="media_id"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendVoiceMsg(string touser, string toparty, string totag, int agentid, string media_id, int safe = 0)
        {
            return SendMsg(new
            {
                touser = touser,
                toparty = toparty,
                totag = totag,
                agentid = agentid,
                msgtype = EntMsgType.voice.ToString(),
                voice = new { media_id = media_id },
                safe = safe
            });
        }

        /// <summary>
        /// 发送视频消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="media_id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendVedioMsg(string touser, string toparty, string totag, int agentid, string media_id, string title = "", string description = "", int safe = 0)
        {
            return SendMsg(new
            {
                touser = touser,
                toparty = toparty,
                totag = totag,
                agentid = agentid,
                msgtype = EntMsgType.video.ToString(),
                video = new { media_id = media_id, title = title, description = description },
                safe = safe
            });
        }

        /// <summary>
        /// 发送文件消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="media_id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendFileMsg(string touser, string toparty, string totag, int agentid, string media_id, string title, string description, int safe = 0)
        {
            return SendMsg(new
            {
                touser = touser,
                toparty = toparty,
                totag = totag,
                agentid = agentid,
                msgtype = EntMsgType.voice.ToString(),
                news = new { media_id = media_id, title = title, description = description },
                safe = safe
            });
        }

        /// <summary>
        /// 发送图文消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="articles"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendNewsMsg(string touser, string toparty, string totag, int agentid, List<EntNewsArticle> articles, int safe = 0)
        {
            return SendMsg(new
            {
                touser = touser,
                toparty = toparty,
                totag = totag,
                agentid = agentid,
                msgtype = EntMsgType.news.ToString(),
                news = new { articles = articles },
                safe = safe
            });
        }

        /// <summary>
        /// 发送公众号图片文消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="articles"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendMpNewsMsg(string touser, string toparty, string totag, int agentid, List<EntMediaArticle> articles, int safe = 0)
        {
            return SendMsg(new
            {
                touser = touser,
                toparty = toparty,
                totag = totag,
                agentid = agentid,
                msgtype = EntMsgType.mpnews.ToString(),
                mpnews = new { articles = articles },
                safe = safe
            });
        }

        /// <summary>
        /// 发送公众号图片文消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="toparty"></param>
        /// <param name="totag"></param>
        /// <param name="agentid"></param>
        /// <param name="media_id"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public static EntMessageResult SendMpNewsMsg(string touser, string toparty, string totag, int agentid, string media_id, int safe = 0)
        {
            return SendMsg(new
            {
                touser = touser,
                toparty = toparty,
                totag = totag,
                agentid = agentid,
                msgtype = EntMsgType.mpnews.ToString(),
                mpnews = new { media_id = media_id },
                safe = safe
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static EntMessageResult SendMsg(object msg)
        {
            var url = string.Format(SEND_MESSAGE_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntMessageResult>(url, msg);
        }

        #endregion
    }

    #region 被动响应实体

    /// <summary>
    /// EntBaseMsg
    /// </summary>
    [XmlRootAttribute("xml")]
    public partial class EntBaseMsg
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        [XmlIgnore]
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        [XmlIgnore]
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public int CreateTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [XmlIgnore]
        public EntMsgType MsgType { get; set; }
    }

    /// <summary>
    /// 消息接收体
    /// </summary>
    [XmlRootAttribute("xml")]
    public class EntReceiveMsg : EntBaseMsg
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
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
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
        public string Event { get; set; }

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
        /// AgentID
        /// </summary>
        public int AgentID { get; set; }
    }

    /// <summary>
    /// EntReceiveMsgCData
    /// </summary>
    [XmlRootAttribute("xml")]
    public class EntReceiveMsgCData : EntReceiveMsg
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
                this.MsgType = (EntMsgType)Enum.Parse(typeof(EntMsgType), value.Value, true);
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
    /// EntNewsArticle
    /// </summary>
    public class EntNewsArticle
    {
        /// <summary>
        /// 图文消息标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 图文消息描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        public string picurl { get; set; }

        /// <summary>
        /// 点击图文消息跳转链接
        /// </summary>
        public string url { get; set; }
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class EntMessageResult : EntApiResult
    {
        /// <summary>
        /// invaliduser
        /// </summary>
        public string invaliduser { get; set; }

        /// <summary>
        /// invalidparty
        /// </summary>
        public string invalidparty { get; set; }

        /// <summary>
        /// invalidtag
        /// </summary>
        public string invalidtag { get; set; }
    }

    #endregion
}
