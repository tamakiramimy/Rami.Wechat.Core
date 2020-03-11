using Rami.Wechat.Core.Comm;
using System.Net.Http;
using System.Xml.Serialization;

namespace Rami.Wechat.Core.Merchant.Public
{
    /// <summary>
    /// 微信红包
    /// </summary>
    public class RedpackApi
    {
        /// <summary>
        /// 公众号配置
        /// </summary>
        private static PublicConf PubConf { get; set; }
        /// <summary>
        /// 公众号支付配置
        /// </summary>
        private static MerchantConf PubPayConf { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        static RedpackApi()
        {
            PubConf = WechatConfigHelper.WechatConf.Public;
            PubPayConf = McPayConf.PubPayConf;
        }

        /// <summary>
        /// 红包api链接
        /// </summary>
        private const string SEND_REDPACK_URL = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";

        /// <summary>
        /// 发送红包
        /// </summary>
        /// <param name="openId">接受红包用户OpenId</param>
        /// <param name="amount">红包金额(单位:分)</param>
        /// <param name="act_name">活动名称</param>
        /// <param name="wishing">红包祝福语</param>
        /// <param name="remark">备注信息</param>
        /// <param name="send_name">发红包公司</param>
        /// <returns></returns>
        public static PackResult SendPack(string openId, int amount, string act_name, string wishing, string remark, string send_name)
        {
            return SendPack(PubConf.AppId, openId, amount, WechatHelper.GetServerIP(), act_name, wishing, remark, send_name);
        }

        /// <summary>
        /// 发送红包
        /// </summary>
        /// <param name="appId">重要，企业号请使用转openid返回的appid</param>
        /// <param name="openId">接受红包用户OpenId</param>
        /// <param name="amount">红包金额(单位:分)</param>
        /// <param name="ip">发送红包服务器IP</param>
        /// <param name="act_name">活动名称</param>
        /// <param name="wishing">红包祝福语</param>
        /// <param name="remark">备注信息</param>
        /// <param name="send_name">发红包公司</param>
        public static PackResult SendPack(string appId, string openId, int amount, string ip, string act_name, string wishing, string remark, string send_name)
        {
            var data = new WxPayData();
            data.SetValue("mch_billno", WechatHelper.GenerateOutTradeNo(PubPayConf.MchId));
            data.SetValue("mch_id", PubPayConf.MchId);
            data.SetValue("wxappid", appId);
            data.SetValue("send_name", send_name);
            data.SetValue("re_openid", openId);
            data.SetValue("total_amount", 100);
            data.SetValue("total_num", 1);
            data.SetValue("wishing", wishing);
            data.SetValue("client_ip", ip);
            data.SetValue("act_name", act_name);
            data.SetValue("remark", remark);
            data.SetValue("nonce_str", WechatHelper.GetNonceStrGuid());
            data.SetValue("sign", data.MakeSign(WxPayData.SIGN_TYPE_MD5));

            // 证书
            var cert = WechatHelper.GetRequestCert(PubPayConf.CertPath, PubPayConf.CertPass, PubPayConf.MchId.ToString());
            using (var client = new HttpClient(cert))
            {
                var result = client.PostAsync(SEND_REDPACK_URL, new StringContent(data.ToXml())).Result.Content.ReadAsStringAsync().Result;
                WechatHelper.Log.Debug("数据" + data.ToXml() + "  结果：" + SerializeHelper.JsonSerialize(result));

                // 解析返回结果
                //WxPayData res = new WxPayData();
                //res.FromXml(result);

                //return new PackResult()
                //{
                //    err_code = res.GetAsStringValue("err_code"),
                //    err_code_des = res.GetAsStringValue("err_code_des").ToString(),
                //    result_code = res.GetAsStringValue("result_code").ToString(),
                //    return_code = res.GetAsStringValue("return_code").ToString(),
                //    return_msg = res.GetAsStringValue("return_msg").ToString()
                //};

                // 红包不返回签名，验证签名必定报错
                var res = SerializeHelper.XmlDeserialize<PackResult>(result);
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
            var data = new WxPayData();
            data.SetValue("mch_id", PubPayConf.MchId);
            data.SetValue("nonce_str", WechatHelper.GetNonceStrGuid());
            data.SetValue("sign", data.MakeSign());

            // 发送请求
            var cert = WechatHelper.GetRequestCert(PubPayConf.CertPath, PubPayConf.CertPass, PubPayConf.MchId.ToString());
            using (var client = new HttpClient(cert))
            {
                var result = client.PostAsync(url, new StringContent(data.ToXml())).Result.Content.ReadAsStringAsync().Result;
                WechatHelper.Log.Debug("数据" + data.ToXml() + "  结果：" + SerializeHelper.JsonSerialize(result));

                // 解析返回结果
                WxPayData res = new WxPayData();
                res.FromXml(result);
                return res.ToJson();
            }
        }
    }

    /// <summary>
    /// 红包支付结果
    /// </summary>
    [XmlRoot("xml")]
    public class PackResult
    {
        /// <summary>
        /// SUCCESS/FAIL
        /// 此字段是通信标识，非红包发放结果标识，
        /// 红包发放是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因
        /// 签名失败
        /// 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        /// <summary>
        /// SUCCESS/FAIL
        /// 注意：当状态为FAIL时，存在业务结果未明确的情况。所以如果状态是FAIL，请务必再请求一次查询接口[请务必关注错误代码（err_code字段），
        /// 通过查询得到的红包状态确认此次发放的结果。]，以确认此次发放的结果。
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误码信息
        /// 注意：出现未明确的错误码（SYSTEMERROR等）时，请务必用原商户订单号重试，或者再请求一次查询接口以确认此次发放的结果。
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 结果信息描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 商户订单号（每个订单号必须唯一）组成：mch_id+yyyymmdd+10位一天内不能重复的数字
        /// </summary>
        public string mch_billno { get; set; }
        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 商户appid，接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），不能为APP的appid（在open.weixin.qq.com申请的）
        /// </summary>
        public string wxappid { get; set; }
        /// <summary>
        /// 接受收红包的用户 用户在wxappid下的openid
        /// </summary>
        public string re_openid { get; set; }
        /// <summary>
        /// 付款金额，单位分
        /// </summary>
        public int total_amount { get; set; }
        /// <summary>
        /// 红包订单的微信单号
        /// </summary>
        public string send_listid { get; set; }

        /// <summary>
        /// 是否发放成功
        /// </summary>
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
