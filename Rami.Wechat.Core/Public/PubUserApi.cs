using Rami.Wechat.Core.Comm;
using System;
using System.Collections.Generic;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class PubUserApi
    {
        #region 接口常量

        /// <summary>
        /// 设置备注名 post {"openid":"oDF3iY9ffA-hqb2vVvbr7qxf6A0Q","remark":"pangzi"}
        /// </summary>
        private const string UPD_USER_REMARK_URL = "https://api.weixin.qq.com/cgi-bin/user/info/updateremark?access_token={0}";

        /// <summary>
        /// 获取用户基本信息（包括UnionID机制）
        /// </summary>
        private const string GET_USER_URL = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";

        /// <summary>
        /// 获取用户基本信息 批量{"user_list": [{"openid": "otvxTs4dckWG7imySrJd6jSi0CWE","lang": "zh-CN"},{"openid": "otvxTs_JZ6SEiP0imdhpi50fuSZg","lang": "zh-CN"}]}
        /// </summary>
        private const string GET_USER_BATCH_URL = " https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={0}";

        /// <summary>
        /// 获取用户列表
        /// </summary>
        private const string GET_USER_LIST_URL = " https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}";

        /// <summary>
        /// CODE获取 OpenID,access_token
        /// </summary>
        private const string GET_OPENID_URL = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        /// <summary>
        /// 通过OpenId和AccessToken获取用户信息
        /// </summary>
        private const string GET_WX_USERINFO = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";

        #endregion

        #region 用户管理

        /// <summary>
        /// 通过authorization_code读取微信openID;
        /// </summary>
        /// <param name="code"></param>
        public static string GetOpenId(string code)
        {
            var url = string.Format(GET_OPENID_URL, PubInterface.Conf.AppId, PubInterface.Conf.AppSecret, code);
            var result = WebApiHelper.GetAsyncStr(url);
            var openID = SerializeHelper.GetJosnValueByName("openid", result);
            return openID;
        }

        /// <summary>
        /// 通过Code获取AccessToken信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static PubWxAccessToken GetAccessToken(string code)
        {
            var url = string.Format(GET_OPENID_URL, PubInterface.Conf.AppId, PubInterface.Conf.AppSecret, code);
            var result = WebApiHelper.GetAsyncStr(url);
            var jsonRes = SerializeHelper.JsonDeserialize<PubWxAccessTokenJson>(result);

            PubWxAccessToken wxAccessToken = new PubWxAccessToken();
            wxAccessToken.AccessToken = jsonRes.access_token;
            wxAccessToken.ExpiresIn = jsonRes.expires_in.ToString();
            wxAccessToken.RefreshToken = jsonRes.refresh_token;
            wxAccessToken.OpenId = jsonRes.openid;
            wxAccessToken.Scope = jsonRes.scope;
            wxAccessToken.JsonData = result;
            return wxAccessToken;
        }

        /// <summary>
        /// 通过RefreshToken刷新AccessToken
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static PubUserInfo GetWxUserInfo(string accessToken, string openId)
        {
            var url = string.Format(GET_WX_USERINFO, accessToken, openId);
            var result = WebApiHelper.GetAsyncStr(url);
            var user = SerializeHelper.JsonDeserialize<PubUserInfo>(result);
            return user;
        }

        /// <summary>
        /// 设置备注名
        /// </summary>
        /// <returns></returns>
        public static PubApiResult UpdUserRemark(string openId, string remark)
        {
            var url = string.Format(UPD_USER_REMARK_URL, PubInterface.AccessToken);
            var data = new { openid = openId, remark = remark };
            return WebApiHelper.PostAsync<PubApiResult>(url, data);
        }

        /// <summary>
        /// 读取用户信息 
        /// </summary>
        /// <returns></returns>
        public static PubUserInfoResult GetUserinfo(string openId)
        {
            var url = string.Format(GET_USER_URL, PubInterface.AccessToken, openId);
            return WebApiHelper.GetAsync<PubUserInfoResult>(url);
        }

        /// <summary>
        /// 读取用户信息列表 {"user_list": [{"openid": "otvxTs4dckWG7imySrJd6jSi0CWE","lang": "zh-CN"},{"openid": "otvxTs_JZ6SEiP0imdhpi50fuSZg","lang": "zh-CN"}]}
        /// </summary>
        /// <returns></returns>
        public static PubUserInfoListResult GetUserinfoList(List<string> openIds)
        {
            var url = string.Format(GET_USER_BATCH_URL, PubInterface.AccessToken);
            var list = new List<dynamic>();
            openIds.ForEach(t =>
            {
                list.Add(new { openid = t, lang = "zh-CN" });
            });
            var data = new { user_list = list };

            return WebApiHelper.PostAsync<PubUserInfoListResult>(url, data);
        }

        /// <summary>
        /// 读取用户列表
        /// </summary>
        /// <returns></returns>
        public static PubUserResult GetUserList(string next_openid = null)
        {
            var url = string.Format(GET_USER_LIST_URL, PubInterface.AccessToken, next_openid);
            return WebApiHelper.GetAsync<PubUserResult>(url);
        }

        #endregion
    }

    /// <summary>
    /// 用户列表
    /// </summary>
    public class PubUserResult : PubApiResult
    {
        /// <summary>
        /// 关注该公众账号的总用户数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 拉取的OPENID个数，最大值为10000
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 列表数据，OPENID的列表
        /// </summary>
        public PubUser data { get; set; }
        /// <summary>
        /// 拉取列表的最后一个用户的OPENID
        /// </summary>
        public string next_openid { get; set; }
    }

    /// <summary>
    /// 用户列表
    /// </summary>
    public class PubUser
    {
        /// <summary>
        /// openid列表
        /// </summary>
        public List<string> openid { get; set; }
    }

    /// <summary>
    /// PubUserInfo
    /// </summary>
    public class PubUserInfo
    {
        /// <summary>
        /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        /// </summary>
        public int subscribe { get; set; }

        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 用户的昵称
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public int sex { get; set; }

        /// <summary>
        /// 用户所在城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 用户所在国家
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// 用户所在省份
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// 用户的语言，简体中文为zh_CN
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        public string headimgurl { get; set; }

        /// <summary>
        /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        /// </summary>
        public int subscribe_time { get; set; }

        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
        /// </summary>
        public string unionid { get; set; }

        /// <summary>
        /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 用户所在的分组ID
        /// </summary>
        public int groupid { get; set; }

        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom） "privilege":[ "PRIVILEGE1" "PRIVILEGE2"],
        /// </summary>
        public List<string> privilege { get; set; }
    }

    /// <summary>
    /// 特殊处理
    /// </summary>
    public class PubUserInfoResult : PubUserInfo
    {
        /// <summary>
        /// errcode
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// errmsg
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// IsSuss
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

                if (errcode == 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception("微信请求失败，errcode:" + errcode + "  errmsg：" + errmsg);
                }
            }
        }
    }

    /// <summary>
    /// PubUserInfoListResult
    /// </summary>
    public class PubUserInfoListResult : PubApiResult
    {
        /// <summary>
        /// user_info_list
        /// </summary>
        public List<PubUserInfo> user_info_list { get; set; }
    }

    /// <summary>
    /// PubWxAccessTokenJson
    /// </summary>
    public class PubWxAccessTokenJson
    {
        /// <summary>
        ///  "access_token":"12_4UG3Rq7PjRz0ivcziJGgJix2jq2mLtHKrA98MK_2brHt-p3Htra-l0XoWLS4ZcZdoDrEg9NOkQTYlgG0DGltgA","expires_in":7200,"refresh_token":"12_N9yOy2-2ZRktIpWEirRSWHIfXreAobfwKuvHfATfSHQAdkswoDYg7vkxpO5Z6xayY86i2zBbvfzGDdFFfUy6Cw",
        ///  "openid":"oxBFPt04uIuFrm-VBMULxuAbDlBo","scope":"snsapi_base"
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// expires_in
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// refresh_token
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// openid
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// scope
        /// </summary>
        public string scope { get; set; }
    }

    /// <summary>
    /// PubWxAccessToken
    /// </summary>
    public class PubWxAccessToken
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// ExpiresIn
        /// </summary>
        public string ExpiresIn { get; set; }

        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// JsonData
        /// </summary>
        public string JsonData { get; set; }
    }
}
