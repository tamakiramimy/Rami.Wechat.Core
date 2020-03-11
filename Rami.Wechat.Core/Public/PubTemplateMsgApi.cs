using Rami.Wechat.Core.Comm;
using System.Collections.Generic;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 发送模板信息
    /// </summary>
    public class PubTemplateMsgApi
    {
        /// <summary>
        /// 设置所属行业
        /// </summary>
        public const string SET_INDUSTRY = "https://api.weixin.qq.com/cgi-bin/template/api_set_industry?access_token={0}";
        /// <summary>
        /// 获取设置的行业
        /// </summary>
        public const string GET_INDUSTRY = "https://api.weixin.qq.com/cgi-bin/template/get_industry?access_token={0}";
        /// <summary>
        /// 根据模板Id获取模板
        /// </summary>
        private const string GET_TEMPLATE_ID = "https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token={0}";
        /// <summary>
        /// 获取模板列表
        /// </summary>
        private const string GET_ALL_TEMPLATE = "https://api.weixin.qq.com/cgi-bin/template/get_all_private_template?access_token={0}";
        /// <summary>
        /// 删除模板
        /// </summary>
        private const string DEL_TEMPLATE = "https://api.weixin.qq.com/cgi-bin/template/del_private_template?access_token={0}";
        /// <summary>
        /// 发送模板消息
        /// </summary>
        private const string SEND_TEMPLATE_URL = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";

        /// <summary>
        /// 设置所属行业
        /// </summary>
        /// <param name="industryId1">公众号模板消息所属行业编号</param>
        /// <param name="industryId2">公众号模板消息所属行业编号</param>
        /// <returns></returns>
        public static PubApiResult SetIndustry(int industryId1, int industryId2)
        {
            var url = string.Format(SET_INDUSTRY, PubInterface.AccessToken);
            var jsonData = new { industry_id1 = industryId1, industry_id2 = industryId2 };
            return WebApiHelper.PostAsync<PubApiResult>(url, jsonData);
        }

        /// <summary>
        /// 获取设置的行业
        /// </summary>
        /// <returns></returns>
        public static PubIndustryResult GetIndustry()
        {
            var url = string.Format(GET_INDUSTRY, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubIndustryResult>(url);
        }

        /// <summary>
        /// 根据模板库中模板的编号获取模板Id
        /// </summary>
        /// <param name="templateIdShort">模板库中模板的编号</param>
        /// <returns></returns>
        public static PubTemplateIdResult GetTemplateId(string templateIdShort)
        {
            var url = string.Format(GET_TEMPLATE_ID, PubInterface.AccessToken);
            var jsonData = new { template_id_short = templateIdShort };
            return WebApiHelper.PostAsync<PubTemplateIdResult>(url, jsonData);
        }

        /// <summary>
        /// 获取模板消息列表
        /// </summary>
        /// <returns></returns>
        public static PubTemplatesResult GetAllTemplate()
        {
            var url = string.Format(GET_ALL_TEMPLATE, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubTemplatesResult>(url);
        }

        /// <summary>
        /// 根据模板Id删除模板
        /// </summary>
        /// <param name="templateId">模板消息模板Id</param>
        /// <returns></returns>
        public static PubApiResult DelTemplate(string templateId)
        {
            var url = string.Format(DEL_TEMPLATE, PubInterface.AccessToken);
            var jsonData = SerializeHelper.JsonSerialize(new { template_id = templateId });
            return WebApiHelper.PostAsync<PubApiResult>(url, jsonData);
        }

        /// <summary>
        /// 发送模块消息
        /// </summary>
        /// <param name="msg">模板消息主体</param>
        /// <returns></returns>
        public static PubApiResult SendTemplateMsg(PubTemplateMessage msg)
        {
            var url = string.Format(SEND_TEMPLATE_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, msg);
        }

        /// <summary>
        /// 行业字典信息
        /// </summary>
        public static Dictionary<int, PubIndustryInfo> DicIndustryInfos = new Dictionary<int, PubIndustryInfo>()
        {
            { 1, new PubIndustryInfo { first_class="IT科技", second_class="互联网/电子商务" } },
            { 2, new PubIndustryInfo { first_class="IT科技", second_class="IT软件与服务" } },
            { 3, new PubIndustryInfo { first_class="IT科技", second_class="IT硬件与设备" } },
            { 4, new PubIndustryInfo { first_class="IT科技", second_class="电子技术" } },
            { 5, new PubIndustryInfo { first_class="IT科技", second_class="通信与运营商" } },
            { 6, new PubIndustryInfo { first_class="IT科技", second_class="网络游戏" } },
            { 7, new PubIndustryInfo { first_class="金融业", second_class="银行" } },
            { 8, new PubIndustryInfo { first_class="金融业", second_class="基金|理财|信托" } },
            { 9, new PubIndustryInfo { first_class="金融业", second_class="保险" } },
            { 10, new PubIndustryInfo { first_class="餐饮", second_class="餐饮" } },
            { 11, new PubIndustryInfo { first_class="酒店旅游", second_class="酒店" } },
            { 12, new PubIndustryInfo { first_class="酒店旅游", second_class="旅游" } },
            { 13, new PubIndustryInfo { first_class="运输与仓储", second_class="快递" } },
            { 14, new PubIndustryInfo { first_class="运输与仓储", second_class="物流" } },
            { 15, new PubIndustryInfo { first_class="运输与仓储", second_class="仓储" } },
            { 16, new PubIndustryInfo { first_class="教育", second_class="培训" } },
            { 17, new PubIndustryInfo { first_class="教育", second_class="院校" } },
            { 18, new PubIndustryInfo { first_class="政府与公共事业", second_class="学术科研" } },
            { 19, new PubIndustryInfo { first_class="政府与公共事业", second_class="交警" } },
            { 20, new PubIndustryInfo { first_class="政府与公共事业", second_class="博物馆" } },
            { 21, new PubIndustryInfo { first_class="政府与公共事业", second_class="公共事业|非盈利机构" } },
            { 22, new PubIndustryInfo { first_class="医药护理", second_class="医药医疗" } },
            { 23, new PubIndustryInfo { first_class="医药护理", second_class="护理美容" } },
            { 24, new PubIndustryInfo { first_class="医药护理", second_class="保健与卫生" } },
            { 25, new PubIndustryInfo { first_class="交通工具", second_class="汽车相关" } },
            { 26, new PubIndustryInfo { first_class="交通工具", second_class="摩托车相关" } },
            { 27, new PubIndustryInfo { first_class="交通工具", second_class="火车相关" } },
            { 28, new PubIndustryInfo { first_class="交通工具", second_class="飞机相关" } },
            { 29, new PubIndustryInfo { first_class="房地产", second_class="建筑" } },
            { 30, new PubIndustryInfo { first_class="房地产", second_class="物业" } },
            { 31, new PubIndustryInfo { first_class="消费品", second_class="消费品" } },
            { 32, new PubIndustryInfo { first_class="商业服务", second_class="法律" } },
            { 33, new PubIndustryInfo { first_class="商业服务", second_class="会展" } },
            { 34, new PubIndustryInfo { first_class="商业服务", second_class="中介服务" } },
            { 35, new PubIndustryInfo { first_class="商业服务", second_class="认证" } },
            { 36, new PubIndustryInfo { first_class="商业服务", second_class="审计" } },
            { 37, new PubIndustryInfo { first_class="文体娱乐", second_class="传媒" } },
            { 38, new PubIndustryInfo { first_class="文体娱乐", second_class="体育" } },
            { 39, new PubIndustryInfo { first_class="文体娱乐", second_class="娱乐休闲" } },
            { 40, new PubIndustryInfo { first_class="印刷", second_class="印刷" } },
            { 41, new PubIndustryInfo { first_class="其它", second_class="其它" } },
        };
    }

    #region 实体

    /// <summary>
    /// 查询行业返回结果
    /// </summary>
    public class PubIndustryResult : PubApiResult
    {
        /// <summary>
        /// 帐号设置的主营行业
        /// </summary>
        public PubIndustryInfo primary_industry { get; set; }
        /// <summary>
        /// 帐号设置的副营行业
        /// </summary>
        public PubIndustryInfo secondary_industry { get; set; }
    }

    /// <summary>
    /// 行业对象
    /// </summary>
    public class PubIndustryInfo
    {
        /// <summary>
        /// 公众号模板消息所属行业编号
        /// </summary>
        public string first_class { get; set; }
        /// <summary>
        /// 帐号设置的副营行业
        /// </summary>
        public string second_class { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}({1})", first_class, second_class);
        }
    }

    /// <summary>
    /// 模板Id返回结果
    /// </summary>
    public class PubTemplateIdResult : PubApiResult
    {
        /// <summary>
        /// template_id
        /// </summary>
        public string template_id { get; set; }
    }

    /// <summary>
    /// 模板消息列表结果
    /// </summary>
    public class PubTemplatesResult : PubApiResult
    {
        /// <summary>
        /// template_list
        /// </summary>
        public List<PubTemplateInfo> template_list { get; set; }
    }

    /// <summary>
    /// 模板消息对象
    /// </summary>
    public class PubTemplateInfo
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 模板标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 模板所属行业的一级行业
        /// </summary>
        public string primary_industry { get; set; }
        /// <summary>
        /// 模板所属行业的二级行业
        /// </summary>
        public string deputy_industry { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 模板示例
        /// </summary>
        public string example { get; set; }
    }

    /// <summary>
    /// 公众号模板消息
    /// </summary>
    public class PubTemplateMessage
    {
        /// <summary>
        /// PubTemplateMessage
        /// </summary>
        public PubTemplateMessage()
        {
            topcolor = "#FF0000";
        }

        /// <summary>
        /// 接收者微信OpenId
        /// </summary>
        public string touser { get; set; }

        /// <summary>
        /// 模板Id
        /// </summary>
        public string template_id { get; set; }

        /// <summary>
        /// 跳转url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 顶部颜色
        /// </summary>
        public string topcolor { get; set; }

        /// <summary>
        /// 具体模板数据
        /// </summary>
        public object data { get; set; }
    }

    /// <summary>
    /// PubTemplateDataItem
    /// </summary>
    public class PubTemplateDataItem
    {
        /// <summary>
        /// 项目值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 16进制颜色代码，如：#FF0000
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="v">value</param>
        /// <param name="c">color</param>
        public PubTemplateDataItem(string v, string c = "#173177")
        {
            value = v;
            color = c;
        }
    }

    #endregion
}
