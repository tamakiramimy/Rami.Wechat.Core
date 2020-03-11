using Rami.Wechat.Core.Comm;
using System.Net.Http;
using System.Xml.Serialization;

namespace Rami.Wechat.Core.Merchant.Enterprise
{
    /// <summary>
    /// 企业红吧接口
    /// </summary>
    public class EntRedpackApi
    {
        /// <summary>
        /// 公众号配置
        /// </summary>
        private static EnterpriseConf EntConf { get; set; }
        /// <summary>
        /// 公众号支付配置
        /// </summary>
        private static MerchantConf EntPayConf { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        static EntRedpackApi()
        {
            EntConf = WechatConfigHelper.WechatConf.Enterprise;
            EntPayConf = McPayConf.EntPayConf;
        }

        /// <summary>
        /// 企业号发红包URL
        /// </summary>
        private const string SEND_REDPACK_URL = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendworkwxredpack";

        /// <summary>
        /// 发送红包
        /// </summary>
        /// <param name="appId">重要，企业号请使用转openid返回的appid</param>
        /// <param name="agentId">企业应用ID</param>
        /// <param name="openId">接受红包用户OpenId</param>
        /// <param name="amount">红包金额(单位:分)</param>
        /// <param name="act_name">活动名称</param>
        /// <param name="wishing">红包祝福语</param>
        /// <param name="remark">备注信息</param>
        /// <param name="scene_id">发放红包使用场景，红包金额大于200时必传</param>
        /// PRODUCT_1: 商品促销
        /// PRODUCT_2:抽奖
        /// PRODUCT_3:虚拟物品兑奖
        /// PRODUCT_4:企业内部福利
        /// PRODUCT_5:渠道分润
        /// PRODUCT_6:保险回馈
        /// PRODUCT_7:彩票派奖
        /// PRODUCT_8:税务刮奖
        public static EntRedpackResult SendPack(string appId, int agentId, string openId, int amount, string act_name, string wishing, string remark, string scene_id = "")
        {
            // 请求参数
            var data = new EntPayData();
            data.SetValue("nonce_str", WechatHelper.GetNonceStrGuid());
            data.SetValue("mch_billno", WechatHelper.GenerateOutTradeNo(EntPayConf.MchId));
            data.SetValue("mch_id", EntPayConf.MchId);
            data.SetValue("wxappid", appId);
            // 以企业应用的名义发红包，企业应用id，整型，可在企业微信管理端应用的设置页面查看。与sender_name互斥，二者只能填一个。
            data.SetValue("agentid", agentId);
            data.SetValue("re_openid", openId);
            data.SetValue("total_amount", amount);
            data.SetValue("act_name", act_name);
            data.SetValue("wishing", wishing);
            data.SetValue("remark", remark);

            //发放红包使用场景，红包金额大于200时必传
            if (!string.IsNullOrEmpty(scene_id))
            {
                data.SetValue("scene_id", scene_id);
            }

            data.SetValue("workwx_sign", data.MakeSignQy());
            data.SetValue("sign", data.MakeSignWx());

            // 证书
            var cert = WechatHelper.GetRequestCert(EntPayConf.CertPath, EntPayConf.CertPass, EntPayConf.MchId.ToString());
            using (var client = new HttpClient(cert))
            {
                var result = client.PostAsync(SEND_REDPACK_URL, new StringContent(data.ToXml())).Result.Content.ReadAsStringAsync().Result;
                //EntPayData res = new EntPayData();
                //res.FromXml(result);
                //WechatHelper.Log.Error("数据" + data.ToXml() + "  结果：" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                //return new EntRedpackResult()
                //{
                //    err_code = res.GetAsStringValue("err_code"),
                //    err_code_des = res.GetAsStringValue("err_code_des"),
                //    result_code = res.GetAsStringValue("result_code"),
                //    return_code = res.GetAsStringValue("return_code"),
                //    return_msg = res.GetAsStringValue("return_msg"),
                //    JsonResult = result
                //};

                var res = SerializeHelper.XmlDeserialize<EntRedpackResult>(result);
                return res;
            }
        }

        /// <summary>
        /// 检查服务器是否支持DigiCert证书
        /// </summary>
        /// <returns></returns>
        public static string CheckDigiCert()
        {
            var url = "https://apitest.mch.weixin.qq.com/sandboxnew/pay/getsignkey";
            var data = new EntPayData();
            data.SetValue("mch_id", EntPayConf.MchId);
            data.SetValue("nonce_str", WechatHelper.GetNonceStrGuid());
            data.SetValue("sign", data.MakeSignWx());

            // 发送请求
            var cert = WechatHelper.GetRequestCert(EntPayConf.CertPath, EntPayConf.CertPass, EntPayConf.MchId.ToString());
            using (var client = new HttpClient(cert))
            {
                var result = client.PostAsync(url, new StringContent(data.ToXml())).Result.Content.ReadAsStringAsync().Result;
                EntPayData res = new EntPayData();
                res.FromXml(result);
                return res.ToJson();
            }
        }
    }

    /// <summary>
    /// 红包返回结果
    /// </summary>
    [XmlRoot("xml")]
    public class EntRedpackResult
    {
        /// <summary>
        /// 返回状态码(SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断)
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息(返回信息，如非空，为错误原因)
        /// </summary>
        public string return_msg { get; set; }
        /// <summary>
        /// 微信支付签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果(SUCCESS/FAIL)
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 结果Json串
        /// </summary>
        [XmlIgnore]
        public string JsonResult { get; set; }

        /// <summary>
        /// 是否发放成功
        /// </summary>
        [XmlIgnore]
        public bool IsSucc
        {
            get
            {
                if (!string.IsNullOrEmpty(return_code) && !string.IsNullOrEmpty(result_code))
                {
                    if (return_code.ToLower() == "success" && result_code.ToLower() == "success")
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
