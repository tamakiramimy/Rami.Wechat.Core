using Rami.Wechat.Core.Comm;
using System.Collections.Generic;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// EntDeptApi
    /// </summary>
    public class EntDeptApi
    {
        #region 常量

        /// <summary>
        /// 创建部门URL
        /// </summary>
        private const string CREATE_DEPARTMENT_URL = "https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token={0}";

        /// <summary>
        /// 更新部门URL
        /// </summary>
        private const string UPDATE_DEPARTMENT_URL = "https://qyapi.weixin.qq.com/cgi-bin/department/update?access_token={0}";

        /// <summary>
        /// 删除部门URL
        /// </summary>
        private const string DELETE_DEPARTMENT_URL = "https://qyapi.weixin.qq.com/cgi-bin/department/delete?access_token={0}&id={1}";

        /// <summary>
        /// 批量读取部门
        /// </summary>
        private const string GET_DEPARTMENT_LIST_URL = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}&id={1}";

        #endregion

        #region 部门管理

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EntDeptResult Create(EntDept data)
        {
            var url = string.Format(CREATE_DEPARTMENT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntDeptResult>(url, data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EntApiResult Update(EntDept data)
        {
            var url = string.Format(UPDATE_DEPARTMENT_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntApiResult Delete(int id)
        {
            var url = string.Format(DELETE_DEPARTMENT_URL, EntInterface.AccessToken, id);
            return WebApiHelper.GetAsync<EntApiResult>(url);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntDeptResult GetDeptList(int id)
        {
            var url = string.Format(GET_DEPARTMENT_LIST_URL, EntInterface.AccessToken, id);
            return WebApiHelper.GetAsync<EntDeptResult>(url);
        }

        #endregion
    }

    /// <summary>
    /// 部门
    /// </summary>
    public class EntDept
    {
        /// <summary>
        /// id  是  部门id  
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// name  否  更新的部门名称。长度限制为0~64个字符。修改部门名称时指定该参数
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// parentid  否  父亲部门id。根部门id为1 
        /// </summary>
        public int? parentid { get; set; }

        /// <summary>
        /// order  否  在父部门中的次序。从1开始，数字越大排序越靠后 
        /// </summary>
        public int? order { get; set; }
    }

    /// <summary>
    /// 部门返回值
    /// </summary>
    public class EntDeptResult : EntApiResult
    {
        /// <summary>
        /// 新增时返时
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 查询列表时返回
        /// </summary>
        public List<EntDept> department { get; set; }
    }
}
