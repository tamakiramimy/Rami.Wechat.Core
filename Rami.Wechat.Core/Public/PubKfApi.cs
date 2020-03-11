using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Net.Http;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 客服Api
    /// </summary>
    public class PubKfApi
    {
        #region 接口地址常量

        /// <summary>
        /// 新增客服 post {"kf_account" : "test1@test","nickname" : "客服1","password" : "pswmd5"}
        /// </summary>
        private const string KF_CRT_URL = "https://api.weixin.qq.com/customservice/kfaccount/add?access_token={0}";

        /// <summary>
        /// 修改客服 post {"kf_account" : "test1@test","nickname" : "客服1","password" : "pswmd5"}
        /// </summary>
        private const string KF_UPD_URL = "https://api.weixin.qq.com/customservice/kfaccount/update?access_token={0}";

        /// <summary>
        /// 删除客服 post {"kf_account" : "test1@test","nickname" : "客服1","password" : "pswmd5"}
        /// </summary>
        private const string KF_DEL_URL = "https://api.weixin.qq.com/customservice/kfaccount/del?access_token={0}&kf_account={1}";

        /// <summary>
        /// 设置客服头像 post 图片
        /// </summary>
        private const string KF_SET_HEAD_URL = "https://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?access_token={0}&kf_account={1}";

        /// <summary>
        /// 读取客服列表 GET {"kf_list": [{"kf_account": "test1@test","kf_nick": "ntest1","kf_id": "1001","kf_headimgurl": "url"}]}
        /// </summary>
        private const string KF_GET_LIST_URL = "https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token={0}";

        /// <summary>
        /// 邀请微信客服 post {"kf_account" : "test1@test","invite_wx" : "test_kfwx"}
        /// </summary>
        public const string KF_SEND_INVITE_URL = "https://api.weixin.qq.com/customservice/kfaccount/inviteworker?access_token={0}";

        /// <summary>
        /// 发送消息
        /// </summary>
        private const string KF_SEND_MSG_URL = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";

        /// <summary>
        /// 获取在线客服列表
        /// </summary>
        private const string KF_ONLINE_LIST = "https://api.weixin.qq.com/cgi-bin/customservice/getonlinekflist?access_token={0}";

        #endregion

        #region 客服管理

        /// <summary>
        /// 新增客服 post {"kf_account" : "test1@test","nickname" : "客服1","password" : "pswmd5"}
        /// </summary>
        public static PubApiResult AddKF(PubKf kf)
        {
            var url = string.Format(KF_CRT_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, kf);
        }

        /// <summary>
        /// 修改客服 post {"kf_account" : "test1@test","nickname" : "客服1","password" : "pswmd5"}
        /// </summary>
        public static PubApiResult UpdateKF(PubKf kf)
        {
            var url = string.Format(KF_UPD_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, kf);
        }

        /// <summary>
        /// 删除客服 kf_account	是	完整客服账号，格式为：账号前缀@公众号微信号
        /// </summary>
        public static PubApiResult DeleteKF(string account)
        {
            var url = string.Format(KF_DEL_URL, PubInterface.AccessToken, account);
            return WebApiHelper.GetAsync<PubApiResult>(url);
        }

        /// <summary>
        /// 设置客服头像 post 图片
        /// </summary>
        public static PubApiResult SetKFHeadImg(string filePath, string kfAccount)
        {
            var url = string.Format(KF_SET_HEAD_URL, PubInterface.AccessToken, kfAccount);
            MultipartFormDataContent content = PubInterface.BuildFromContent(filePath);
            using (var client = new HttpClient())
            {
                var postReq = client.PostAsync(url, content).Result;
                var postResp = postReq.Content.ReadAsStringAsync().Result;
                var res = SerializeHelper.JsonDeserialize<PubApiResult>(postResp);
                return res;
            }
        }

        /// <summary>
        /// 读取客服列表 GET {"kf_list": [{"kf_account": "test1@test","kf_nick": "ntest1","kf_id": "1001","kf_headimgurl": "url"}]}
        /// </summary>
        public static PubKfResult GetKFList()
        {
            var url = string.Format(KF_GET_LIST_URL, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubKfResult>(url);
        }

        /// <summary>
        /// 获取在线客服列表
        /// </summary>
        /// <returns></returns>
        public static PubKfOlResult GetKFOLList()
        {
            var url = string.Format(KF_ONLINE_LIST, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubKfOlResult>(url);
        }

        /// <summary>
        /// 发送客服邀请
        /// </summary>
        /// <param name="kfAcount"></param>
        /// <param name="inviteWx"></param>
        /// <returns></returns>
        public static PubApiResult SendKfInvite(string kfAcount, string inviteWx)
        {
            var postData = new { kf_account = kfAcount, invite_wx = inviteWx };
            var url = string.Format(KF_SEND_INVITE_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, postData);
        }

        #endregion

        #region 发送消息

        /// <summary>
        /// 发送文本信息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="content"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendTextMsg(string openID, string content, string kf_account = null)
        {
            var msg = new PubKfTextMsg { touser = openID, text = new PubKfTextContent { content = content }, customservice = new PubKfCustomAccount { kf_account = kf_account } };
            return SendMsg(msg);
        }

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="mediaID"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendImageMsg(string openID, string mediaID, string kf_account = null)
        {
            var msg = new PubKfImgMsg { touser = openID, image = new PubKfImgContent { media_id = mediaID }, customservice = new PubKfCustomAccount { kf_account = kf_account } };
            return SendMsg(msg);
        }

        /// <summary>
        /// 发送语音消息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="mediaID"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendVoiceMsg(string openID, string mediaID, string kf_account = null)
        {
            var msg = new PubKfVoiceMsg { touser = openID, voice = new PubKfVoiceContent { media_id = mediaID }, customservice = new PubKfCustomAccount { kf_account = kf_account } };
            return SendMsg(msg);
        }

        /// <summary>
        /// 发送视频消息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="mediaID"></param>
        /// <param name="thumb_media_id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendVideoMsg(string openID, string mediaID, string thumb_media_id, string title, string description, string kf_account = null)
        {
            var msg = new PubKfVideoMsg
            {
                touser = openID,
                video = new PubKfVideoContent { media_id = mediaID, thumb_media_id = thumb_media_id, title = title, description = description },
                customservice = new PubKfCustomAccount { kf_account = kf_account }
            };

            return SendMsg(msg);
        }

        /// <summary>
        /// 发送音乐消息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="musicurl"></param>
        /// <param name="hqmusicurl"></param>
        /// <param name="thumb_media_id"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendMusicMsg(string openID, string title, string description, string musicurl, string hqmusicurl, string thumb_media_id, string kf_account = null)
        {
            var msg = new PubKfMusicMsg
            {
                touser = openID,
                music = new PubKfMusicContent { title = title, thumb_media_id = thumb_media_id, description = description, musicurl = musicurl, hqmusicurl = hqmusicurl },
                customservice = new PubKfCustomAccount { kf_account = kf_account }
            };

            return SendMsg(msg);
        }

        /// <summary>
        /// 发送图文消息 图文消息条数限制在10条以内，注意，如果图文数超过10，则将会无响应。
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="articles">图文数组 格式 {title="标题",description="说明",url="链接地址",picurl="图片地址"}</param>
        /// <param name="kf_account">默认为null</param>
        /// <returns></returns>
        public static PubApiResult SendNewsMsg(string openID, List<PubKfArticle> articles, string kf_account = null)
        {
            var msg = new PubKfNewsMsg { touser = openID, news = new PubKfNewsContent { articles = articles }, customservice = new PubKfCustomAccount { kf_account = kf_account } };
            return SendMsg(msg);
        }

        /// <summary>
        /// 发送媒体Id图文消息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="media_id"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendMpNewsMsg(string openID, string media_id, string kf_account = null)
        {
            var msg = new PubKfMpNewsMsg { touser = openID, mpnews = new PubKfMpnewsContent { media_id = media_id }, customservice = new PubKfCustomAccount { kf_account = kf_account } };
            return SendMsg(msg);
        }

        /// <summary>
        /// 发送卡券 查看card_ext字段详情及签名规则，特别注意客服消息接口投放卡券仅支持非自定义Code码的卡券。
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="cardCode"></param>
        /// <param name="cardID"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendWxCardMsg(string openID, string cardCode, string cardID, string kf_account = null)
        {
            var msg = new PubKfWxCardMsg
            {
                touser = openID,
                customservice = new PubKfCustomAccount { kf_account = kf_account },
                wxcard = new PubKfWxCardContent { card_id = cardID, card_ext = new PubKfWxCardExt { card_id = cardID, code = cardCode, openid = openID } }
            };

            return SendMsg(msg);
        }

        /// <summary>
        /// 发送客服卡券，仅支持非自定义Code卡券
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="cardId"></param>
        /// <param name="kf_acount"></param>
        /// <returns></returns>
        public static PubApiResult SendWxCardNoCodeMsg(string openID, string cardId, string kf_acount = null)
        {
            var msg = new PubKfWxCardNoCodeMsg { touser = openID, wxcard = new PubKfWxCardNoCodeContent { card_id = cardId }, customservice = new PubKfCustomAccount { kf_account = kf_acount } };
            return SendMsg(msg);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PubApiResult SendMsg(object data)
        {
            var url = string.Format(KF_SEND_MSG_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, data);
        }

        /// <summary>
        /// 如果需要以某个客服帐号来发消息（在微信6.0.2及以上版本中显示自定义头像），则需在JSON数据包的后半部分加入customservice参数.
        /// 即kf_account不为空时，添加customservice
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="openID"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendMsg(dynamic data, PubMsgType type, string openID, string kf_account = null)
        {
            data.touser = openID;
            data.msgtype = type.ToString();
            if (!string.IsNullOrEmpty(kf_account))
            {
                data.customservice = new { kf_account = kf_account };
            }

            return SendMsg(data);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="openID"></param>
        /// <param name="kf_account"></param>
        /// <returns></returns>
        public static PubApiResult SendMsg(PubKfBaseMsg data, string openID, string kf_account = null)
        {
            data.touser = openID;
            if (!string.IsNullOrEmpty(kf_account))
            {
                data.customservice.kf_account = kf_account;
            }

            return SendMsg(data);
        }
        
        #endregion
    }

    #region 实体类

    /// <summary>
    /// 客服账号
    /// </summary>
    public class PubKf
    {
        /// <summary>
        /// 完整客服账号，格式为：账号前缀@公众号微信号
        /// </summary>
        public string kf_account { get; set; }

        /// <summary>
        /// 客服昵称，最长6个汉字或12个英文字符
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码
        /// </summary>
        public string password { get; set; }
    }

    /// <summary>
    /// 客服图文
    /// </summary>
    public class PubKfArticle
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 文章链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 图文图片链接
        /// </summary>
        public string picurl { get; set; }
    }

    /// <summary>
    /// 客服邀请
    /// </summary>
    public class PubKfBody
    {
        /// <summary>
        /// 客服账号
        /// </summary>
        public string kf_account { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string kf_nick { get; set; }
        /// <summary>
        /// 客服id
        /// </summary>
        public string kf_id { get; set; }
        /// <summary>
        /// 客服头像
        /// </summary>
        public string kf_headimgurl { get; set; }
        /// <summary>
        /// 客服微信
        /// </summary>
        public string kf_wx { get; set; }
        /// <summary>
        /// 邀请的客服微信号
        /// </summary>
        public string invite_wx { get; set; }
        /// <summary>
        /// 邀请超时时间
        /// </summary>
        public int? invite_expire_time { get; set; }
        /// <summary>
        /// 邀请状态
        /// </summary>
        public string invite_status { get; set; }
    }

    /// <summary>
    /// 客服列表同步
    /// </summary>
    public class PubKfResult : PubApiResult
    {
        /// <summary>
        /// 客服列表
        /// </summary>
        public List<PubKfBody> kf_list { get; set; }
    }

    /// <summary>
    /// 客服在线
    /// </summary>
    public class PubKfOlResult : PubApiResult
    {
        /// <summary>
        /// 在线客服列表
        /// </summary>
        public List<PubKfOlBody> kfol_list { get; set; }
    }

    /// <summary>
    /// 客服在线
    /// </summary>
    public class PubKfOlBody
    {
        /// <summary>
        /// 客服账号
        /// </summary>
        public string kf_account { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string statusText
        {
            get
            {
                string res = string.Empty;
                switch (status)
                {
                    case 1:
                        res = "pc在线";
                        break;
                    case 2:
                        res = "手机在线";
                        break;
                    case 3:
                        res = "pc、手机在线";
                        break;
                    default:
                        break;
                }
                return res;
            }
        }
        /// <summary>
        /// 客服ID
        /// </summary>
        public string kf_id { get; set; }
        /// <summary>
        /// 是否自动接入
        /// </summary>
        public int auto_accept { get; set; }
        /// <summary>
        /// 接入方式
        /// </summary>
        public int accepted_case { get; set; }
    }

    /// <summary>
    /// 客服消息类型
    /// </summary>
    public enum PubKfMsgType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        text,
        /// <summary>
        /// 图片消息
        /// </summary>
        image,
        /// <summary>
        /// 语音消息
        /// </summary>
        voice,
        /// <summary>
        /// 视频消息
        /// </summary>
        video,
        /// <summary>
        /// 音乐消息
        /// </summary>
        music,
        /// <summary>
        /// 图文消息(articles)
        /// </summary>
        news,
        /// <summary>
        /// 图文消息(media_id)
        /// </summary>
        mpnews,
        /// <summary>
        /// 卡券
        /// </summary>
        wxcard,
    }

    /// <summary>
    /// 客服消息基类
    /// </summary>
    public class PubKfBaseMsg
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string msgtype { get; set; }
        /// <summary>
        /// 客服账号信息
        /// </summary>
        public PubKfCustomAccount customservice { get; set; }
    }

    /// <summary>
    /// 客服账号
    /// </summary>
    public class PubKfCustomAccount
    {
        /// <summary>
        /// 客服账号
        /// </summary>
        public string kf_account = null;
    }

    /// <summary>
    /// 客服文本消息
    /// </summary>
    public class PubKfTextMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.text.ToString();
            }
        }
        /// <summary>
        /// 文本内容
        /// </summary>
        public PubKfTextContent text { get; set; }
    }

    /// <summary>
    /// 客服文本消息文本内容
    /// </summary>
    public class PubKfTextContent
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        public string content { get; set; }
    }

    /// <summary>
    /// 客服图片消息
    /// </summary>
    public class PubKfImgMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.image.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfImgContent image { get; set; }
    }

    /// <summary>
    /// 客服图片消息内容
    /// </summary>
    public class PubKfImgContent
    {
        /// <summary>
        /// 图片媒体ID
        /// </summary>
        public string media_id { get; set; }
    }

    /// <summary>
    /// 客服音频消息
    /// </summary>
    public class PubKfVoiceMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.voice.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfVoiceContent voice { get; set; }
    }

    /// <summary>
    /// 客服音频消息内容
    /// </summary>
    public class PubKfVoiceContent
    {
        /// <summary>
        /// 音频媒体ID
        /// </summary>
        public string media_id { get; set; }
    }

    /// <summary>
    /// 客服视频消息
    /// </summary>
    public class PubKfVideoMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.video.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfVideoContent video { get; set; }
    }

    /// <summary>
    /// 客服视频消息内容
    /// </summary>
    public class PubKfVideoContent
    {
        /// <summary>
        /// 视频媒体ID
        /// </summary>
        public string media_id { get; set; }
        /// <summary>
        /// 视频缩略图媒体ID
        /// </summary>
        public string thumb_media_id { get; set; }
        /// <summary>
        /// 视频标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 视频描述
        /// </summary>
        public string description { get; set; }
    }

    /// <summary>
    /// 客服音乐消息
    /// </summary>
    public class PubKfMusicMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.music.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfMusicContent music { get; set; }
    }

    /// <summary>
    /// 客服音乐消息内容
    /// </summary>
    public class PubKfMusicContent
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 音乐url(低音质)
        /// </summary>
        public string musicurl { get; set; }
        /// <summary>
        /// 音乐url(高音质)
        /// </summary>
        public string hqmusicurl { get; set; }
        /// <summary>
        /// 音乐缩略图媒体ID
        /// </summary>
        public string thumb_media_id { get; set; }
    }

    /// <summary>
    /// 客服图文消息
    /// </summary>
    public class PubKfNewsMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.news.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfNewsContent news { get; set; }
    }

    /// <summary>
    /// 客服图文消息内容
    /// </summary>
    public class PubKfNewsContent
    {
        /// <summary>
        /// 图文缩略图媒体ID
        /// </summary>
        public List<PubKfArticle> articles { get; set; }
    }

    /// <summary>
    /// 客服媒体图文消息
    /// </summary>
    public class PubKfMpNewsMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.mpnews.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfMpnewsContent mpnews { get; set; }
    }

    /// <summary>
    /// 客服媒体图文消息内容
    /// </summary>
    public class PubKfMpnewsContent
    {
        /// <summary>
        /// 媒体图文媒体ID
        /// </summary>
        public string media_id { get; set; }
    }

    /// <summary>
    /// 客服微信卡券消息
    /// </summary>
    public class PubKfWxCardMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.wxcard.ToString();
            }
        }
        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfWxCardContent wxcard { get; set; }
    }

    /// <summary>
    /// 客服微信卡券消息内容
    /// </summary>
    public class PubKfWxCardContent
    {
        /// <summary>
        /// 微信卡券媒体ID
        /// </summary>
        public string card_id { get; set; }
        /// <summary>
        /// 卡券扩展信息
        /// </summary>
        public PubKfWxCardExt card_ext { get; set; }
    }

    /// <summary>
    /// 微信客服卡券扩展信息
    /// </summary>
    public class PubKfWxCardExt
    {
        /// <summary>
        /// 卡券号码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 用户OpenID
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long timestamp
        {
            get
            {
                return WechatHelper.EpochTime;
            }
        }
        /// <summary>
        /// 卡券ID（不序列化）
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string card_id { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string signature
        {
            get
            {
                return WechatHelper.Sha1Sign(PubInterface.CardTicket, timestamp.ToString(), card_id, code, openid);
            }
        }
    }

    /// <summary>
    /// 非自定义Code客服卡券消息
    /// </summary>
    public class PubKfWxCardNoCodeMsg : PubKfBaseMsg
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public new string msgtype
        {
            get
            {
                return PubKfMsgType.wxcard.ToString();
            }
        }

        /// <summary>
        /// 图片资源
        /// </summary>
        public PubKfWxCardNoCodeContent wxcard { get; set; }
    }

    /// <summary>
    /// 非自定义Code客服卡券消息内容
    /// </summary>
    public class PubKfWxCardNoCodeContent
    {
        /// <summary>
        /// 微信卡券ID（非自定义Code）
        /// </summary>
        public string card_id { get; set; }
    }

    #endregion
}
