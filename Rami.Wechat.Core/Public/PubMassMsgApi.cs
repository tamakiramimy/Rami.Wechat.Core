using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Dynamic;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 群发消息
    /// </summary>
    public class PubMassMsgApi
    {
        #region 接口常量

        /// <summary>
        /// 上传图文消息素材
        /// </summary>
        private const string UPLOAD_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token={0}";

        /// <summary>
        /// 根据分组进行群发
        /// </summary>
        private const string GROUP_SEND_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}";

        /// <summary>
        /// 根据OpenID列表群发
        /// </summary>
        private const string USER_SEND_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token={0}";

        /// <summary>
        /// 删除群发
        /// </summary>
        private const string DEL_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/message/mass/delete?access_token={0}";

        /// <summary>
        /// 消息预览
        /// </summary>
        private const string PREVIEW_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/message/mass/preview?access_token={0}";

        /// <summary>
        /// 消息查询
        /// </summary>
        private const string GET_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/message/mass/get?access_token={0}";

        #endregion

        #region 发送消息

        /// <summary>
        /// 群发消息
        /// </summary>
        /// <param name="groupID">isToAll为true时间groupID无效,isToAll默认为false</param>
        /// <param name="msgType"></param>
        /// <param name="content"></param>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="isToAll">isToAll为true时间groupID无效,isToAll默认为false</param>
        /// <returns></returns>
        public static PubMsgResult SendMsg(int groupID, PubMsgType msgType, string content, string mediaID, string title, string description, bool isToAll = false)
        {
            var url = string.Format(GROUP_SEND_NEWS_URL, PubInterface.AccessToken);
            dynamic data = new ExpandoObject();
            data.filter = new { is_to_all = isToAll, group_id = groupID };
            BuildMsg(msgType, content, mediaID, title, description, data);
            return WebApiHelper.PostAsync<PubMsgResult>(url, data);
        }

        /// <summary>
        /// 群发消息
        /// </summary>
        /// <param name="openIDs"></param>
        /// <param name="msgType"></param>
        /// <param name="content"></param>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static PubMsgResult SendMsg(List<string> openIDs, PubMsgType msgType, string content, string mediaID, string title, string description)
        {
            var url = string.Format(USER_SEND_NEWS_URL, PubInterface.AccessToken);
            dynamic data = new ExpandoObject();
            data.filter = new { touser = openIDs };
            BuildMsg(msgType, content, mediaID, title, description, data);
            return WebApiHelper.PostAsync<PubMsgResult>(url, data);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msgID"></param>
        /// <returns></returns>
        public static PubApiResult DeleteMsg(int msgID)
        {
            var data = new { msg_id = msgID };
            var url = string.Format(DEL_NEWS_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, data);
        }

        /// <summary>
        /// 预览消息 每天限100次
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="msgType"></param>
        /// <param name="content"></param>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static PubMsgResult PreViewMsg(string openID, PubMsgType msgType, string content, string mediaID, string title, string description)
        {
            dynamic data = new ExpandoObject();
            data.touser = openID;
            BuildMsg(msgType, content, mediaID, title, description, data);
            var url = string.Format(PREVIEW_NEWS_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync(url, data);
        }

        /// <summary>
        /// 查询消息发送状态
        /// </summary>
        /// <param name="msgID"></param>
        /// <returns></returns>
        public static PubApiResult GetMsgState(int msgID)
        {
            var data = new { msg_id = msgID };
            var url = string.Format(GET_NEWS_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, data);
        }

        /// <summary>
        /// 发送文本信息
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="content"></param>
        /// <param name="mediaID"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static dynamic BuildMsg(PubMsgType msgType, string content, string mediaID, string title, string description, dynamic data)
        {
            switch (msgType)
            {
                case PubMsgType.image:
                    data.msgtype = "image";
                    data.image = new { media_id = mediaID }; break;
                case PubMsgType.mpnews:
                    data.msgtype = "mpnews";
                    data.mpnews = new { media_id = mediaID }; break;
                case PubMsgType.text:
                    data.msgtype = "text";
                    data.text = new { content = content }; break;
                case PubMsgType.video:
                    data.msgtype = "video";
                    data.video = new { media_id = mediaID, title = title, description = description }; break;
                case PubMsgType.voice:
                    data.msgtype = "voice";
                    data.voice = new { media_id = mediaID }; break;
                case PubMsgType.wxcard:
                    data.msgtype = "wxcard";
                    data.wxcard = new { media_id = mediaID }; break;
                default:
                    break;
            }
            return data;
        }

        #endregion
    }

    /// <summary>
    /// 群发消息结果
    /// </summary>
    public class PubMsgResult : PubApiResult
    {
        /// <summary>
        /// 消息发送任务的ID
        /// </summary>
        public int msg_id { get; set; }

        /// <summary>
        /// 消息的数据ID，该字段只有在群发图文消息时，才会出现。可以用于在图文分析数据接口中，获取到对应的图文消息的数据，是图文分析数据接口中的msgid字段中的前半部分，详见图文分析数据接口中的msgid字段的介绍
        /// </summary>
        public int msg_data_id { get; set; }
    }
}
