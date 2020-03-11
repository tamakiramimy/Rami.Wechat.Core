using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Linq;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 用户分组Api
    /// </summary>
    public class PubGroupApi
    {
        #region 接口常量

        /// <summary>
        /// 创建分组 post {"group":{"name":"test"}}
        /// </summary>
        private const string GROUP_CRT_URL = "https://api.weixin.qq.com/cgi-bin/groups/create?access_token={0}";

        /// <summary>
        /// 查询所有分组 get
        /// </summary>
        private const string GROUP_GET_URL = "https://api.weixin.qq.com/cgi-bin/groups/get?access_token={0}";

        /// <summary>
        /// 删除分组 post {"group":{"id":108}}
        /// </summary>
        private const string GROUP_DEL_URL = "https://api.weixin.qq.com/cgi-bin/groups/delete?access_token={0}";

        /// <summary>
        /// 修改分组 post {"group":{"id":108,"name":"test2_modify2"}}
        /// </summary>
        private const string GROUP_UPD_URL = "https://api.weixin.qq.com/cgi-bin/groups/update?access_token={0}";

        /// <summary>
        /// 移动用户 post {"openid":"oDF3iYx0ro3_7jD4HFRDfrjdCM58","to_groupid":108}
        /// </summary>
        private const string GROUP_MEMBER_URL = "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token={0}";

        /// <summary>
        /// 批量移动用户 post {"openid_list":["oDF3iYx0ro3_7jD4HFRDfrjdCM58","oDF3iY9FGSSRHom3B-0w5j4jlEyY"],"to_groupid":108}
        /// </summary>
        private const string GROUP_MEMBER_BATCH_URL = "https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token={0}";

        /// <summary>
        /// 查询用户分组 post {"openid":"od8XIjsmk6QdVTETa9jLtGWA6KBc"}
        /// </summary>
        private const string GROUP_SEL_URL = "https://api.weixin.qq.com/cgi-bin/groups/getid?access_token={0}";

        #endregion

        #region 用户分组

        /// <summary>
        /// 创建分组 post {"group":{"name":"test"}}
        /// </summary>
        public static PubGroupResult AddGroup(string name)
        {
            var url = string.Format(GROUP_CRT_URL, PubInterface.AccessToken);
            var data = new { group = new { name = name } };
            return WebApiHelper.PostAsync<PubGroupResult>(url, data);
        }

        /// <summary>
        /// 查询所有分组 get
        /// </summary>
        public static PubGroupsResult GetGroup()
        {
            var url = string.Format(GROUP_GET_URL, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubGroupsResult>(url);
        }

        /// <summary>
        /// 删除分组 post {"group":{"id":108}}
        /// </summary>
        public static PubApiResult DelGroup(int groupID)
        {
            var url = string.Format(GROUP_DEL_URL, PubInterface.AccessToken);
            var data = new { group = new { id = groupID } };
            return WebApiHelper.PostAsync<PubApiResult>(url, data);
        }

        /// <summary>
        /// 修改分组 post {"group":{"id":108,"name":"test2_modify2"}}
        /// </summary>
        public static PubGroupResult UpdGroup(int groupID, string name)
        {
            var url = string.Format(GROUP_UPD_URL, PubInterface.AccessToken);
            var data = new { group = new { id = groupID, name = name } };
            return WebApiHelper.PostAsync<PubGroupResult>(url, data);
        }

        /// <summary>
        /// 移动用户 post {"openid":"oDF3iYx0ro3_7jD4HFRDfrjdCM58","to_groupid":108} 
        /// </summary>
        public static PubApiResult SetUserGroup(int groupID, params string[] openids)
        {
            var url = "";
            object data;
            if (openids.Length == 1)
            {
                url = string.Format(GROUP_MEMBER_URL, PubInterface.AccessToken);
                data = new { openid = openids[0], to_groupid = groupID };
            }
            else
            {
                url = string.Format(GROUP_MEMBER_BATCH_URL, PubInterface.AccessToken);
                data = new { openid_list = openids.ToArray(), to_groupid = groupID };
            }

            return WebApiHelper.PostAsync<PubApiResult>(url, data);
        }

        /// <summary>
        /// 查询用户分组 post {"openid":"od8XIjsmk6QdVTETa9jLtGWA6KBc"}
        /// </summary>
        public static PubUserGroupResult QueryGroup(string openID)
        {
            var url = string.Format(GROUP_SEL_URL, PubInterface.AccessToken);
            var data = new { openid = openID };
            return WebApiHelper.PostAsync<PubUserGroupResult>(url, data);
        }

        #endregion
    }

    /// <summary>
    /// 所有分组
    /// </summary>
    public class PubGroupsResult : PubApiResult
    {
        /// <summary>
        /// 所有分组
        /// </summary>
        public List<PubGroup> groups { get; set; }
    }

    /// <summary>
    /// 分组详情
    /// </summary>
    public class PubGroupResult : PubApiResult
    {
        /// <summary>
        /// 分组信息
        /// </summary>
        public PubGroup group { get; set; }
    }

    /// <summary>
    /// 用户分组
    /// </summary>
    public class PubUserGroupResult : PubApiResult
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        public int groupid { get; set; }
    }

    /// <summary>
    /// 分组信息
    /// </summary>
    public class PubGroup
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 未分组
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 用户数量
        /// </summary>
        public int count { get; set; }
    }
}
