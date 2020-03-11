using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// PubSataApi
    /// </summary>
    public class PubSataApi
    {
        #region 用户分析数据

        /// <summary>
        /// 获取用户增减数据（getusersummary）	post 	{"begin_date": "2014-12-02","end_date": "2014-12-07"} 最长时间跨度 7天 
        /// </summary>
        private const string GET_USER_SUMMARY_URL = "https://api.weixin.qq.com/datacube/getusersummary?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取累计用户数据（getusercumulate） post 	{"begin_date": "2014-12-02","end_date": "2014-12-07"} 最长时间跨度 7天 	
        /// </summary>
        private const string GET_USER_CUMULATE_URL = "https://api.weixin.qq.com/datacube/getusercumulate?access_token=ACCESS_TOKEN";

        #endregion

        #region 图文分析数据

        /// <summary>
        /// 获取图文群发每日数据（getarticlesummary）	1
        /// </summary>
        private const string GET_ARTICLE_SUMMARY_URL = "https://api.weixin.qq.com/datacube/getarticlesummary?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取图文群发总数据（getarticletotal）	1	
        /// </summary>
        private const string GET_ARTICLE_TOTAL_URL = "https://api.weixin.qq.com/datacube/getarticletotal?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取图文统计数据（getuserread）	3	
        /// </summary>
        private const string GET_USER_READ_URL = "https://api.weixin.qq.com/datacube/getuserread?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取图文统计分时数据（getuserreadhour）	1	
        /// </summary>
        private const string GET_USER_READ_HOUR_URL = "https://api.weixin.qq.com/datacube/getuserreadhour?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取图文分享转发数据（getusershare）	7	
        /// </summary>
        private const string GET_USER_SHARE_URL = "https://api.weixin.qq.com/datacube/getusershare?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取图文分享转发分时数据（getusersharehour）	1	
        /// </summary>
        private const string GET_USER_SHARE_HOUR_URL = "https://api.weixin.qq.com/datacube/getusersharehour?access_token=ACCESS_TOKEN";

        #endregion

        #region 消息统计信息

        /// <summary>
        ///获取消息发送概况数据（getupstreammsg）	7	
        /// </summary>
        private const string getupstreammsg = "https://api.weixin.qq.com/datacube/getupstreammsg?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取消息分送分时数据（getupstreammsghour）	1	
        /// </summary>
        private const string getupstreammsghour = "https://api.weixin.qq.com/datacube/getupstreammsghour?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取消息发送周数据（getupstreammsgweek）	30	
        /// </summary>
        private const string getupstreammsgweek = "https://api.weixin.qq.com/datacube/getupstreammsgweek?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取消息发送月数据（getupstreammsgmonth）	30		
        /// </summary>
        private const string getupstreammsgmonth = "https://api.weixin.qq.com/datacube/getupstreammsgmonth?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取消息发送分布数据（getupstreammsgdist）	15	
        /// </summary>
        private const string getupstreammsgdist = "https://api.weixin.qq.com/datacube/getupstreammsgdist?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取消息发送分布周数据（getupstreammsgdistweek）	30	
        /// </summary>
        private const string getupstreammsgdistweek = "https://api.weixin.qq.com/datacube/getupstreammsgdistweek?access_token=ACCESS_TOKEN";

        /// <summary>
        /// 获取消息发送分布月数据（getupstreammsgdistmonth）	30	
        /// </summary>
        private const string getupstreammsgdistmonth = "https://api.weixin.qq.com/datacube/getupstreammsgdistmonth?access_token=ACCESS_TOKEN";

        #endregion

        #region 接口分析数据

        /// <summary>
        ///获取接口分析数据（getinterfacesummary）	30	
        /// </summary>
        private const string getinterfacesummary = "https://api.weixin.qq.com/datacube/getinterfacesummary?access_token=ACCESS_TOKEN";

        /// <summary>
        ///获取接口分析分时数据（getinterfacesummaryhour）	1	
        /// </summary>
        private const string getinterfacesummaryhour = "https://api.weixin.qq.com/datacube/getinterfacesummaryhour?access_token=ACCESS_TOKEN";

        #endregion
    }
}
