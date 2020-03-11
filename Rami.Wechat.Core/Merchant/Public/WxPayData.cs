using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Rami.Wechat.Core.Comm;

namespace Rami.Wechat.Core.Merchant.Public
{
    /// <summary>
    /// 微信支付协议接口数据类，所有的API接口通信都依赖这个数据结构，
    /// 在调用接口之前先填充各个字段的值，然后进行接口通信，
    /// 这样设计的好处是可扩展性强，用户可随意对协议进行更改而不用重新设计数据结构，
    /// 还可以随意组合出不同的协议数据包，不用为每个协议设计一个数据包结构
    /// </summary>
    public class WxPayData
    {
        /// <summary>
        /// 公众号支付配置
        /// </summary>
        private MerchantConf PubPayConf { get; set; }

        /// <summary>
        /// MD5
        /// </summary>
        public const string SIGN_TYPE_MD5 = "MD5";

        /// <summary>
        /// SHA256
        /// </summary>
        public const string SIGN_TYPE_HMAC_SHA256 = "HMAC-SHA256";

        /// <summary>
        /// 构造方法
        /// </summary>
        public WxPayData()
        {
            this.PubPayConf = McPayConf.PubPayConf;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="conf"></param>
        public WxPayData(MerchantConf conf)
        {
            this.PubPayConf = conf;
        }

        /// <summary>
        /// 采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        /// </summary>
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /// <summary>
        /// 设置某个字段的值
        /// </summary>
        /// <param name="key">字段名</param>
        /// <param name="value">字段值</param>
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /// <summary>
        /// 根据字段名获取某个字段的值
        /// </summary>
        /// <param name="key">字段名</param>
        /// <returns>对应的字段值</returns>
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /// <summary>
        /// 根据字段名获取某个字段的值
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
        /// <param name="key">字段名</param>
        /// <returns>若字段key已被设置，则返回true，否则返回false</returns>
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
        /// <returns>经转换得到的xml串</returns>
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
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                //除了string和int类型不能含有其他数据类型
                else
                {
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
        /// <param name="xml">待转换的xml串</param>
        /// <returns>经转换得到的Dictionary</returns>
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                WechatHelper.Log.Error("将空的xml串转换为WxPayData不合法!");
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
                if ((string)m_values["return_code"] != "SUCCESS")
                {
                    return m_values;
                }

                CheckSign();//验证签名,不通过会抛异常
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
        /// <returns>url格式串, 该串不包含sign字段值</returns>
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
        /// <returns>json串数据</returns>
        public string ToJson()
        {
            return SerializeHelper.JsonSerialize(m_values);
        }

        /// <summary>
        /// values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        /// </summary>
        /// <returns></returns>
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    WechatHelper.Log.Error("WxPayData内部含有值为null的字段!");
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }

                str += string.Format("{0}={1}\n", pair.Key, pair.Value.ToString());
            }

            str = HttpUtility.HtmlEncode(str);
            WechatHelper.Log.Debug("Print in Web Page : " + str);
            return str;
        }

        /// <summary>
        /// 生成签名，详见签名生成算法
        /// </summary>
        /// <param name="signType"></param>
        /// <returns>签名, sign字段不参加签名</returns>
        public string MakeSign(string signType)
        {
            //转url格式
            string str = ToUrl();
            var payKey = PubPayConf.PayKey;
            //在string后加入API KEY
            str += "&key=" + payKey;
            if (signType == SIGN_TYPE_MD5)
            {
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
            else if (signType == SIGN_TYPE_HMAC_SHA256)
            {
                return CalcHMACSHA256Hash(str, payKey);
            }
            else
            {
                throw new Exception("sign_type 不合法");
            }
        }

        /// <summary>
        /// 生成签名，详见签名生成算法
        /// </summary>
        /// <returns>签名, sign字段不参加签名 SHA256</returns>
        public string MakeSign()
        {
            return MakeSign(SIGN_TYPE_HMAC_SHA256);
        }

        /// <summary>
        /// 检测签名是否正确
        /// </summary>
        /// <param name="signType"></param>
        /// <returns>正确返回true，错误抛异常</returns>
        public bool CheckSign(string signType)
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                WechatHelper.Log.Error("WxPayData签名存在但不合法!");
                throw new Exception("WxPayData签名存在但不合法!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                WechatHelper.Log.Error("WxPayData签名存在但不合法!");
                throw new Exception("WxPayData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign(signType);

            if (cal_sign == return_sign)
            {
                return true;
            }

            WechatHelper.Log.Error("WxPayData签名验证错误!");
            throw new Exception("WxPayData签名验证错误!");
        }

        /// <summary>
        /// 检测签名是否正确
        /// </summary>
        /// <returns>正确返回true，错误抛异常</returns>
        public bool CheckSign()
        {
            return CheckSign(SIGN_TYPE_HMAC_SHA256);
        }

        /// <summary>
        /// 获取Dictionary
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }

        /// <summary>
        /// 计算HmacSha256哈希值
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            //var enc = Encoding.Default;
            var enc = Encoding.UTF8;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("X2")).ToArray());
            return result;
        }
    }
}
