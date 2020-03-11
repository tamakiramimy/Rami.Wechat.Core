using Rami.Wechat.Core.Comm;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Rami.Wechat.Core.Merchant.Enterprise
{
    /// <summary>
    /// 企业支付数据（签名）
    /// </summary>
    public class EntPayData
    {
        /// <summary>
        /// 支付配置
        /// </summary>
        private MerchantConf EntPayConf { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public EntPayData()
        {
            this.EntPayConf = McPayConf.EntPayConf;
        }

        /// <summary>
        /// 采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        /// </summary>
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /// <summary>
        /// 获取Dictionary
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }

        /// <summary>
        /// 设置某个字段的值
        /// </summary>
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /// <summary>
        /// 根据字段名获取某个字段的值
        /// </summary>
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAsStringValue(string key)
        {
            object o = null;
            return m_values.TryGetValue(key, out o) ? o.ToString() : string.Empty;
        }

        /// <summary>
        /// 判断某个字段是否已设置
        /// </summary>
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 将Dictionary转成xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                WechatHelper.Log.Error("WxPayData数据为空!");
                throw new Exception("WxPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    WechatHelper.Log.Error("WxPayData内部含有值为null的字段!");
                    throw new Exception(pair.Key + "为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    //xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                    xml += "<" + pair.Key + ">" + "" + pair.Value + "</" + pair.Key + ">";
                }
                else
                {
                    //除了string和int类型不能含有其他数据类型
                    WechatHelper.Log.Error("WxPayData字段数据类型错误!");
                    throw new Exception("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /// <summary>
        /// 将xml转为WxPayData对象并返回对象内部的数据
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                //WechatHelper.Log.Error("将空的xml串转换为WxPayData不合法!");
                throw new Exception("将空的xml串转换为WxPayData不合法!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }

            try
            {
                //2015-06-29 错误是没有签名
                if (m_values["return_code"].ToString() != "SUCCESS")
                {
                    return m_values;
                }

                //CheckSign();//验证签名,不通过会抛异常 sign不会返回
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return m_values;
        }

        /// <summary>
        /// Dictionary格式转化成url参数格式
        /// </summary>
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    WechatHelper.Log.Error("WxPayData内部含有值为null的字段!");
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }

            buff = buff.Trim('&');
            return buff;
        }

        /// <summary>
        /// Dictionary格式化成Json
        /// </summary>
        public string ToJson()
        {
            return SerializeHelper.JsonSerialize(m_values);
        }

        /// <summary>
        /// 微信支付签名
        /// </summary>
        public string MakeSignWx()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + EntPayConf.PayKey;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }

            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 企业支付签名
        /// </summary>
        /// <param name="type">1:红包；2：收/收款</param>
        /// <returns></returns>
        public string MakeSignQy(int type = 1)
        {
            // 红包签名
            var lstNeeds = new List<string>();
            if (type == 1)
            {
                lstNeeds = new List<string> { "act_name", "mch_billno", "mch_id", "nonce_str", "re_openid", "total_amount", "wxappid" };
            }
            else if (type == 2)
            {
                lstNeeds = new List<string> { "amount", "appid", "desc", "mch_id", "nonce_str", "openid", "partner_trade_no", "ww_msg_type" };
            }

            string str = string.Empty;
            foreach (var key in lstNeeds)
            {
                if (m_values.ContainsKey(key))
                {
                    str += key + "=" + m_values[key].ToString() + "&";
                }
            }

            str = str.TrimEnd('&');
            //在string后加入API KEY
            str += "&secret=" + EntPayConf.PaySecret;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }

            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        ///// <summary>
        ///// 检测签名是否正确
        ///// </summary>
        //public bool CheckSign()
        //{
        //    //如果没有设置签名，则跳过检测
        //    if (!IsSet("sign"))
        //    {
        //        //WechatHelper.Log.Error("WxPayData签名存在但不合法!");
        //        throw new Exception("WxPayData签名存在但不合法!");
        //    }
        //    //如果设置了签名但是签名为空，则抛异常
        //    else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
        //    {
        //        //WechatHelper.Log.Error("WxPayData签名存在但不合法!");
        //        throw new Exception("WxPayData签名存在但不合法!");
        //    }

        //    //获取接收到的签名
        //    string return_sign = GetValue("sign").ToString();

        //    //在本地计算新的签名
        //    string cal_sign = MakeSign();

        //    if (cal_sign == return_sign)
        //    {
        //        return true;
        //    }

        //    //WechatHelper.Log.Error("WxPayData签名验证错误!");
        //    throw new Exception("WxPayData签名验证错误!");
        //}
    }
}
