using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Text;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 外部联系人Api
    /// </summary>
    public class EntExternalContactAPI
    {
        /// <summary>
        /// 获取外部联系人信息
        /// </summary>
        private const string GET_EXTERNAL_CONTACT = "https://qyapi.weixin.qq.com/cgi-bin/crm/get_external_contact?access_token={0}&external_userid={1}";

        /// <summary>
        /// 转移外部联系人
        /// </summary>
        private const string TRANSFER_EXTERNAL_CONTACT = "https://qyapi.weixin.qq.com/cgi-bin/crm/transfer_external_contact?access_token={0}";

        /// <summary>
        /// 获取外部联系人信息
        /// </summary>
        /// <param name="exUserId"></param>
        /// <returns></returns>
        public static string GetOutUserInfoStr(string exUserId)
        {
            var url = string.Format(GET_EXTERNAL_CONTACT, EntInterface.OutAccessToken, exUserId);
            var res = WebApiHelper.GetAsyncStr(url);
            return res;
        }

        /// <summary>
        /// 获取外部联系人信息
        /// </summary>
        /// <param name="exUserId"></param>
        /// <returns></returns>
        public static EntExternalResult GetOutUserInfo(string exUserId)
        {
            var strRes = GetOutUserInfoStr(exUserId);
            var res = SerializeHelper.JsonDeserialize<EntExternalResult>(strRes);
            return res;
        }

        /// <summary>
        /// 转移外部联系人
        /// </summary>
        /// <param name="exUserId"></param>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public static EntApiResult TransferOutUser(string exUserId, string fromUserId, string toUserId)
        {
            var url = string.Format(TRANSFER_EXTERNAL_CONTACT, EntInterface.OutAccessToken);
            var postData = new { external_userid = exUserId, handover_userid = fromUserId, takeover_userid = toUserId };
            var strRes = WebApiHelper.PostAsyncStr(url, postData, Encoding.UTF8);
            var res = SerializeHelper.JsonDeserialize<EntExternalResult>(strRes);
            return res;
        }
    }

    /// <summary>
    /// 外部联系人结果
    /// </summary>
    public class EntExternalResult : EntApiResult
    {
        /// <summary>
        /// 外部联系人信息
        /// </summary>
        public EntExternalContact external_contact { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public List<EntFollowUser> follow_user { get; set; }
    }

    /// <summary>
    /// 外部联系人信息
    /// </summary>
    public class EntExternalContact
    {
        /// <summary>
        /// 外部联系人的userid
        /// </summary>
        public string external_userid { get; set; }
        /// <summary>
        /// 外部联系人的姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 外部联系人的职位，如果外部企业或用户选择隐藏职位，则不返回，仅当联系人类型是企业微信用户时有此字段
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 外部联系人头像，第三方不可获取
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 外部联系人所在企业的简称，仅当联系人类型是企业微信用户时有此字段
        /// </summary>
        public string corp_name { get; set; }
        /// <summary>
        /// 外部联系人所在企业的主体名称，仅当联系人类型是企业微信用户时有此字段
        /// </summary>
        public string corp_full_name { get; set; }
        /// <summary>
        /// 外部联系人的类型，1表示该外部联系人是微信用户（暂时仅内测企业有此类型），2表示该外部联系人是企业微信用户
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 外部联系人性别 0-未知 1-男性 2-女性
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 外部联系人在微信开放平台的唯一身份标识（微信unionid），通过此字段企业可将外部联系人与公众号/小程序用户关联起来。仅当联系人类型是微信用户，且企业绑定了微信开发者ID有此字段。查看绑定方法
        /// </summary>
        public string unionid { get; set; }
        /// <summary>
        /// 外部联系人的自定义展示信息，可以有多个字段和多种类型，包括文本，网页和小程序，仅当联系人类型是企业微信用户时有此字段，字段详情见对外属性；
        /// </summary>
        public EntExternalProfile external_profile { get; set; }
    }

    /// <summary>
    /// 外部联系人的自定义展示信息
    /// </summary>
    public class EntExternalProfile
    {
        /// <summary>
        /// 外部联系人的自定义展示属性
        /// </summary>
        public List<EntExternalAttr> external_attr { get; set; }
    }

    /// <summary>
    /// 外部联系人的自定义展示属性
    /// </summary>
    public class EntExternalAttr
    {
        /// <summary>
        /// 属性类型: 0-本文 1-网页 2-小程序
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 属性名称： 需要先确保在管理端有创建改属性，否则会忽略
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// "text":{"value":"文本"}
        /// </summary>
        public EntText text { get; set; }
        /// <summary>
        /// "web":{"url":"http://www.test.com","title":"标题"}
        /// </summary>
        public EntWeb web { get; set; }
        /// <summary>
        /// "miniprogram":{"appid": "wx8bd80126147df384","pagepath": "/index","title": "my miniprogram"}
        /// </summary>
        public EntMiniprogram miniprogram { get; set; }
    }

    /// <summary>
    /// 文本类型的属性
    /// </summary>
    public class EntText
    {
        /// <summary>
        /// 文本属性内容,长度限制12个UTF8字符
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// 网页类型的属性，url和title字段要么同时为空表示清除该属性，要么同时不为空
    /// </summary>
    public class EntWeb
    {
        /// <summary>
        /// 网页的url,必须包含http或者https头
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 网页的展示标题,长度限制12个UTF8字符
        /// </summary>
        public string title { get; set; }
    }

    /// <summary>
    /// 小程序类型的属性，appid和title字段要么同时为空表示清除改属性，要么同时不为空
    /// </summary>
    public class EntMiniprogram
    {
        /// <summary>
        /// 小程序appid，必须是有在本企业安装授权的小程序，否则会被忽略
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 小程序的展示标题,长度限制12个UTF8字符
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 小程序的页面路径
        /// </summary>
        public string pagepath { get; set; }
    }

    /// <summary>
    /// 添加了此外部联系人的企业成员
    /// </summary>
    public class EntFollowUser
    {
        /// <summary>
        /// 添加了此外部联系人的企业成员userid
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 该成员对此外部联系人的备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 该成员对此外部联系人的描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 该成员添加此外部联系人的时间
        /// </summary>
        public int createtime { get; set; }
    }
}
