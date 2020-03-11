using Rami.Wechat.Core.Comm;
using System;
using System.Net.Http;

namespace Rami.Wechat.Core.Merchant.Public
{
    /// <summary>
    /// JsApi下单
    /// </summary>
    public class JsApiPay
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        public static WxPayData GetUnifiedOrderResult(WxPayUnifiedOrder unifiedOrder)
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", unifiedOrder.body);
            data.SetValue("attach", unifiedOrder.attach);
            data.SetValue("out_trade_no", unifiedOrder.out_trade_no);
            data.SetValue("total_fee", unifiedOrder.total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", unifiedOrder.goods_tag);
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", unifiedOrder.openid);
            data.SetValue("spbill_create_ip", unifiedOrder.spbill_create_ip);
            data.SetValue("notify_url", unifiedOrder.notify_url);

            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                throw new Exception("微信支付下单失败！");
            }

            return result;
        }

        /// <summary>
        /// BJPay
        /// </summary>
        /// <param name="unifiedOrder"></param>
        /// <returns></returns>
        public static WxPayData GetXMResult(WxPayUnifiedOrder unifiedOrder)
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", unifiedOrder.body);
            data.SetValue("attach", unifiedOrder.attach);
            data.SetValue("out_trade_no", unifiedOrder.out_trade_no);
            data.SetValue("total_fee", unifiedOrder.total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", unifiedOrder.goods_tag);
            data.SetValue("trade_type", "NATIVE");
            data.SetValue("spbill_create_ip", unifiedOrder.spbill_create_ip);
            data.SetValue("product_id", "12235413214070356458058");
            data.SetValue("notify_url", unifiedOrder.notify_url);

            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                throw new Exception("微信支付下单失败！");
            }

            return result;
        }

        /// <summary>
        /// 微信支付API参数初始化
        /// </summary>
        public static string GetJsApiParameters(WxPayData unifiedOrderResult)
        {
            var parameters = GetPayPara(unifiedOrderResult).ToJson();
            return parameters;
        }

        /// <summary>
        /// 微信支付API参数初始化
        /// </summary>
        public static WxPayData GetPayPara(WxPayData unifiedOrderResult)
        {
            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", WxPayData.SIGN_TYPE_HMAC_SHA256);
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());
            return jsApiParam;
        }

        /// <summary>
        /// 查询支付结果
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public static string GetPayResult(string no)
        {
            var url = "https://api.mch.weixin.qq.com/pay/orderquery";
            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appid", WxPayApi.PubConf.AppId);
            jsApiParam.SetValue("mch_id", WxPayApi.PubPayConf.MchId);
            jsApiParam.SetValue("nonce_str", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("out_trade_no", no);
            jsApiParam.SetValue("sign", jsApiParam.MakeSign());
            var res = WebApiHelper.PostAsyncStr(url, new StringContent(jsApiParam.ToXml()));
            return res;
        }
    }

    /// <summary>
    /// 下单结果
    /// </summary>
    public class WxPayUnifiedOrder
    {
        /// <summary>
        /// 商品描述
        /// 商品名称明细列表
        /// </summary>
        public string body { get; set; }

        /// <summary>
        /// 附加数据
        /// 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 总金额
        /// 订单总金额，单位为分
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 商品标记
        /// 商品标记，代金券或立减优惠功能的参数
        /// </summary>
        public string goods_tag { get; set; }

        /// <summary>
        /// 用户标识
        /// 用户在商户appid下的唯一标识
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 通知地址
        /// 接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。
        /// </summary>
        public string notify_url { get; set; }

        /// <summary>
        /// 商户订单号
        /// 商户系统内部的订单号,32个字符内、可包含字母
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 终端IP
        /// APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP
        /// </summary>
        public string spbill_create_ip { get; set; }
    }
}
