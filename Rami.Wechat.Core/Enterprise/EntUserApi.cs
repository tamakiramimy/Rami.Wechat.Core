using Rami.Wechat.Core.Comm;
using System.Collections.Generic;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class EntUserApi
    {
        #region 接口常量

        /// <summary>
        /// 创建成员URL
        /// </summary>
        private const string CREATE_USER_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}";

        /// <summary>
        /// 更新成员URL
        /// </summary>
        private const string UPDATE_USER_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/update?access_token={0}";

        /// <summary>
        /// 删除成员URL
        /// </summary>
        private const string DELETE_USER_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token={0}&userid={1}";

        /// <summary>
        /// 批量删除
        /// </summary>
        private static string DELETE_BATCH_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete?access_token={0}";

        /// <summary>
        /// 获取成员URL
        /// </summary>
        private static string GET_USER_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}";

        /// <summary>
        /// 获取部门成员URL
        /// </summary>
        private const string GET_SIMPLE_LIST_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token={0}&department_id={1}&fetch_child={2}&status={3}";

        /// <summary>
        /// 获取部门成员URL
        /// </summary>
        private const string GET_USER_LIST_URL = "https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token={0}&department_id={1}&fetch_child={2}&status={3}";

        /// <summary>
        /// 获取部门成员URL
        /// </summary>
        private const string INVITE_USER_URL = "https://qyapi.weixin.qq.com/cgi-bin/invite/send?access_token={0}";

        #endregion

        #region 用户管理

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EntApiResult Create(EntUser data)
        {
            var url = string.Format(CREATE_USER_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EntApiResult Update(EntUser data)
        {
            var url = string.Format(UPDATE_USER_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static EntApiResult Delete(string userID)
        {
            var url = string.Format(DELETE_USER_URL, EntInterface.AccessToken, userID);
            return WebApiHelper.GetAsync<EntApiResult>(url);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        public static EntApiResult Delete(List<string> userIDs)
        {
            var url = string.Format(DELETE_BATCH_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, new { useridlist = userIDs });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static EntUserResult GetUser(string userID)
        {
            var url = string.Format(GET_USER_URL, EntInterface.AccessToken, userID);
            return WebApiHelper.GetAsync<EntUserResult>(url);
        }

        /// <summary>
        /// 查询部门下人员
        /// </summary>
        /// <param name="department_id">获取的部门id</param>
        /// <param name="fetch_child">1/0：是否递归获取子部门下面的成员</param>
        /// <param name="status">0获取全部成员，1获取已关注成员列表，2获取禁用成员列表，4获取未关注成员列表。status可叠加</param>
        /// <returns></returns>
        public static EntUserListResult GetSimpleList(int department_id, int fetch_child, int status)
        {
            var url = string.Format(GET_SIMPLE_LIST_URL, EntInterface.AccessToken, department_id, fetch_child, status);
            return WebApiHelper.GetAsync<EntUserListResult>(url);
        }

        /// <summary>
        /// 查询部门下人员
        /// </summary>
        /// <param name="department_id">获取的部门id</param>
        /// <param name="fetch_child">1/0：是否递归获取子部门下面的成员</param>
        /// <param name="status">0获取全部成员，1获取已关注成员列表，2获取禁用成员列表，4获取未关注成员列表。status可叠加</param>
        /// <returns></returns>
        public static EntUserListResult GetUserList(int department_id, int fetch_child, int status)
        {
            var url = string.Format(GET_USER_LIST_URL, EntInterface.AccessToken, department_id, fetch_child, status);
            return WebApiHelper.GetAsync<EntUserListResult>(url);
        }

        /// <summary>
        /// InviteUser
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static EntApiResult InviteUser(string userID)
        {
            var url = string.Format(INVITE_USER_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, new { userid = userID });
        }

        #endregion
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class EntUser
    {
        /// <summary>
        /// 员工UserID。对应管理端的帐号，企业内必须唯一。长度为1~64个字符 必填
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 成员名称。长度为1~64个字符 必填
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 成员所属部门id列表。注意，每个部门的直属员工上限为1000个
        /// </summary>
        public List<int> department { get; set; }

        /// <summary>
        /// 职位信息。长度为0~64个字符
        /// </summary>
        public string position { get; set; }

        /// <summary>
        ///  手机号码。企业内必须唯一，mobile/weixinid/email三者不能同时为空
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 性别 1表示男性，2表示女性
        /// </summary>
        public int gender { get; set; }

        /// <summary>
        /// 邮箱。长度为0~64个字符。企业内必须唯一
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 微信号。企业内必须唯一  
        /// </summary>
        public string weixinid { get; set; }

        /// <summary>
        /// 1启用，0禁用
        /// </summary>
        public int enable { get; set; }

        /// <summary>
        /// 否	成员头像的mediaid，通过多媒体接口上传图片获得的mediaid
        /// </summary>
        public string avatar_mediaid { get; set; }

        /// <summary>
        /// 否	扩展属性。扩展属性需要在WEB管理端创建后才生效，否则忽略未知属性的赋值  {"attrs":[{"name":"爱好","value":"旅游"},{"name":"卡号","value":"1234567234"}]}
        /// </summary>
        public object extattr { get; set; }

        /// <summary>
        /// 关注状态: 1=已关注，2=已冻结，4=未关注
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 头像url。注：如果要获取小图将url最后的"/0"改成"/64"即可
        /// </summary>
        public string avatar { get; set; }
    }

    /// <summary>
    /// EntUserResult
    /// </summary>
    public class EntUserResult : EntUser
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
        /// errcode为空或者为0都为成功
        /// </summary>
        public virtual bool IsSuss
        {
            get
            {
                //如果令牌超时，清除令牌缓存重新读取
                if (errcode == 42001)
                {
                    EntInterface.InitToken();
                }

                return errcode == 0;
            }
        }
    }

    /// <summary>
    /// EntUserListResult
    /// </summary>
    public class EntUserListResult : EntApiResult
    {
        /// <summary>
        /// userlist
        /// </summary>
        public List<EntUser> userlist { get; set; }
    }
}
