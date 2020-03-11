using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 企业群聊
    /// </summary>
    public class EntChatApi
    {
        #region 常量地址

        /// <summary>
        /// 创建会话
        /// </summary>
        private const string ADD_CHAT_URL = "https://qyapi.weixin.qq.com/cgi-bin/appchat/create?access_token={0}";

        /// <summary>
        /// 获取会话
        /// </summary>
        private const string GET_CHAT_URL = "https://qyapi.weixin.qq.com/cgi-bin/appchat/get?access_token={0}&chatid={1}";

        /// <summary>
        /// 更新会话
        /// </summary>
        private const string UPD_CHAT_URL = "https://qyapi.weixin.qq.com/cgi-bin/chat/update?access_token={0}";

        /// <summary>
        /// 退出会话
        /// </summary>
        private const string QUIT_CHAT_URL = "https://qyapi.weixin.qq.com/cgi-bin/chat/quit?access_token={0}";

        /// <summary>
        /// 清除会话未读状态
        /// </summary>
        private const string CLEAR_CHAT_URL = "https://qyapi.weixin.qq.com/cgi-bin/chat/clearnotify?access_token={0}";

        /// <summary>
        /// 发消息
        /// </summary>
        private const string SEND_CHAT_URL = "https://qyapi.weixin.qq.com/cgi-bin/chat/send?access_token={0}";

        /// <summary>
        /// 设置成员新消息免打扰
        /// </summary>
        private const string SET_MUTE_URL = "https://qyapi.weixin.qq.com/cgi-bin/chat/setmute?access_token={0}";

        #endregion

        #region 会话管理

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EntApiResult Create(EntChat data)
        {
            var url = string.Format(ADD_CHAT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 读取会话
        /// </summary>
        /// <returns></returns>
        public static EntChatResult Get(string chatID)
        {
            var url = string.Format(GET_CHAT_URL, EntInterface.AccessToken, chatID);
            return WebApiHelper.GetAsync<EntChatResult>(url);
        }

        /// <summary>
        /// 更新会话
        /// </summary>
        /// <returns></returns>
        public static EntApiResult Update(EntChatUpd data)
        {
            var url = string.Format(UPD_CHAT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 退出会话
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="opUserID"></param>
        /// <returns></returns>
        public static EntApiResult QuitChat(string chatID, string opUserID)
        {
            var url = string.Format(QUIT_CHAT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, new { chatid = chatID, op_user = opUserID });
        }

        /// <summary>
        /// 清除会话未读状态
        /// </summary>
        /// <param name="op_user">是	会话所有者的userid</param>
        /// <param name="type">是	会话类型：single|group，分别表示：群聊|单聊</param>
        /// <param name="id">是	会话值，为userid|chatid，分别表示：成员id|会话id</param>
        /// <returns></returns>
        public static EntApiResult ClearChat(string op_user, string type, string id)
        {
            var url = string.Format(CLEAR_CHAT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, new { op_user = op_user, chat = new { type = type, id = id } });
        }

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static EntApiResult SendTextChat(string type, string id, string sender, string content)
        {
            var data = new { receiver = new { type = type, id = id }, sender = sender, msgtype = "text", text = new { content = content } };
            return SendChat(data);
        }

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public static EntApiResult SendImageChat(string type, string id, string sender, string media_id)
        {
            var data = new { receiver = new { type = type, id = id }, sender = sender, msgtype = "image", text = new { media_id = media_id } };
            return SendChat(data);
        }

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public static EntApiResult SendFileChat(string type, string id, string sender, string media_id)
        {
            var data = new { receiver = new { type = type, id = id }, sender = sender, msgtype = "file", file = new { media_id = media_id } };
            return SendChat(data);
        }

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EntApiResult SendChat(object data)
        {
            var url = string.Format(SEND_CHAT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 新消息免打扰
        /// </summary>
        /// <param name="enableds">消息不提醒</param>
        /// <param name="disableds">消息提醒</param>
        /// <returns></returns>
        public static EntApiResult SetMute(List<string> enableds, List<string> disableds)
        {
            var list = new List<object>();
            enableds.ForEach(t =>
            {
                list.Add(new { userid = t, status = 1 });
            });

            disableds.ForEach(t =>
            {
                list.Add(new { userid = t, status = 1 });
            });

            var data = new { user_mute_list = list };
            var url = string.Format(SET_MUTE_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        #endregion
    }

    #region 实体

    /// <summary>
    /// 企业号聊天
    /// </summary>
    public class EntChat
    {
        /// <summary>
        /// 会话标题
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 管理员userid，必须是该会话userlist的成员之一
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// 会话成员列表，成员用userid来标识。会话成员必须在3人或以上，1000人以下
        /// </summary>
        public List<string> userlist { get; set; }

        /// <summary>
        /// 会话id。字符串类型，最长32个字符。只允许字符0-9及字母a-zA-Z, 如果值内容为64bit无符号整型：要求值范围在[1, 2^63)之间，[2^63, 2^64)为系统分配会话id区间
        /// </summary>
        public string chatid { get; set; }
    }

    /// <summary>
    /// EntChatUpd
    /// </summary>
    public class EntChatUpd
    {
        /// <summary>
        /// 会话id。字符串类型，最长32个字符。只允许字符0-9及字母a-zA-Z, 如果值内容为64bit无符号整型：要求值范围在[1, 2^63)之间，[2^63, 2^64)为系统分配会话id区间
        /// </summary>
        public string chatid { get; set; }

        /// <summary>
        /// 	是	操作人userid
        /// </summary>
        public string op_user { get; set; }

        /// <summary>
        /// 	否	会话标题
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 否	管理员userid，必须是该会话userlist的成员之一
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// 否	会话新增成员列表，成员用userid来标识
        /// </summary>
        public List<string> add_user_list { get; set; }

        /// <summary>
        /// 否	会话退出成员列表，成员用userid来标识
        /// </summary>
        public List<string> del_user_list { get; set; }
    }

    /// <summary>
    /// EntChatResult
    /// </summary>
    public class EntChatResult : EntApiResult
    {
        /// <summary>
        /// EntChat
        /// </summary>
        public EntChat chat_info { get; set; }
    }

    /// <summary>
    /// ChatMessage
    /// </summary>
    [XmlRoot("xml")]
    public class ChatMessage
    {
        /// <summary>
        /// AgentType
        /// </summary>
        public string AgentType { get; set; }

        /// <summary>
        /// ToUserName
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// ItemCount
        /// </summary>
        public string ItemCount { get; set; }

        /// <summary>
        /// PackageId
        /// </summary>
        public int PackageId { get; set; }

        /// <summary>
        /// Item
        /// </summary>
        [XmlElement]
        public List<dynamic> Item;
    }

    #endregion
}
