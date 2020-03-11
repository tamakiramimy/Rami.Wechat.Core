using Rami.Wechat.Core.Comm;
using System.Collections.Generic;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 标签管理
    /// </summary>
    public class EntTagApi
    {
        #region 接口常量
        
        /// <summary>
        /// 创建标签
        /// </summary>
        private const string CREATE_TAG_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/create?access_token={0}";

        /// <summary>
        /// 更新标签
        /// </summary>
        private const string UPDATE_TAG_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/update?access_token={0}";

        /// <summary>
        /// 删除标签
        /// </summary>
        private const string DELETE_TAG_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/delete?access_token={0}&tagid={1}";

        /// <summary>
        /// 获取标签成员
        /// </summary>
        private static string GET_TAGUSER_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/get?access_token={0}&tagid={1}";

        /// <summary>
        /// 添加标签成员
        /// </summary>
        private const string ADD_TAGUSER_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/addtagusers?access_token={0}";

        /// <summary>
        /// 删除标签成员
        /// </summary>
        private const string DEL_TAGUSER_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/deltagusers?access_token={0}";

        /// <summary>
        /// 读取标签列表
        /// </summary>
        private const string GET_TAG_LIST_URL = "https://qyapi.weixin.qq.com/cgi-bin/tag/list?access_token={0}";

        #endregion

        #region 用户分组

        /// <summary>
        /// 创建标签
        /// </summary>
        public static EntTagResult Create(string tagname, int tagid)
        {
            var url = string.Format(CREATE_TAG_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntTagResult>(url, new { tagname = tagname, tagid = tagid });
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        public static EntApiResult Update(string tagname, int tagid)
        {
            var url = string.Format(UPDATE_TAG_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, new { tagname = tagname, tagid = tagid });
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        public static EntApiResult Delete(int tagid)
        {
            var url = string.Format(DELETE_TAG_URL, EntInterface.AccessToken, tagid);
            return WebApiHelper.GetAsync<EntApiResult>(url);
        }

        /// <summary>
        /// GetTagUsers
        /// </summary>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public static EntTagItemResult GetTagUsers(int tagID)
        {
            var url = string.Format(GET_TAGUSER_URL, EntInterface.AccessToken, tagID);
            return WebApiHelper.GetAsync<EntTagItemResult>(url);
        }

        /// <summary>
        /// AddTagUsers
        /// </summary>
        /// <param name="tagID"></param>
        /// <param name="users"></param>
        /// <param name="partys"></param>
        /// <returns></returns>
        public static EntTagsResult AddTagUsers(int tagID, List<string> users, List<string> partys)
        {
            var url = string.Format(ADD_TAGUSER_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntTagsResult>(url, new { tagid = tagID, userlist = users, partylist = partys });
        }

        /// <summary>
        /// DelTagUsers
        /// </summary>
        /// <param name="tagID"></param>
        /// <param name="users"></param>
        /// <param name="partys"></param>
        /// <returns></returns>
        public static EntTagsResult DelTagUsers(int tagID, List<string> users, List<string> partys)
        {
            var url = string.Format(DEL_TAGUSER_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntTagsResult>(url, new { tagid = tagID, userlist = users, partylist = partys });
        }

        /// <summary>
        /// GetTagList
        /// </summary>
        /// <returns></returns>
        public static EntTagsResult GetTagList()
        {
            var url = string.Format(GET_TAG_LIST_URL, EntInterface.AccessToken);
            return WebApiHelper.GetAsync<EntTagsResult>(url);
        }

        #endregion
    }

    /// <summary>
    /// 标签返回结果
    /// </summary>
    public class EntTagResult : EntApiResult
    {
        /// <summary>
        /// tagid
        /// </summary>
        public int tagid { get; set; }
    }

    /// <summary>
    /// EntTagsResult
    /// </summary>
    public class EntTagsResult : EntApiResult
    {
        /// <summary>
        /// taglist
        /// </summary>
        public List<EntTag> taglist { get; set; }

        /// <summary>
        /// invalidlist
        /// </summary>
        public List<string> invalidlist { get; set; }

        /// <summary>
        /// invalidparty
        /// </summary>
        public List<int> invalidparty { get; set; }
    }

    /// <summary>
    /// EntTagItemResult
    /// </summary>
    public class EntTagItemResult : EntApiResult
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<EntTagUser> userlist { get; set; }

        /// <summary>
        /// 部分列表
        /// </summary>
        public List<int> partylist { get; set; }
    }

    /// <summary>
    /// 标签用户
    /// </summary>
    public class EntTagUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// 标签
    /// </summary>
    public class EntTag
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public int tagid { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string tagname { get; set; }
    }
}
