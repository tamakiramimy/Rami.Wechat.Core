using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Rami.Wechat.Core.Comm
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    internal static class SerializeHelper
    {
        #region Json序列化

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        /// 将对象转换成json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerializeNoNull(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        /// Json反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 获取字符串Json的属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="josnString"></param>
        /// <returns></returns>
        public static string GetJosnValueByName(string name, string josnString)
        {
            var returnString = string.Empty;
            try
            {
                var jo = JObject.Parse(josnString);
                var list = jo.Properties().Where(it => it.Name.ToLower() == name.ToLower()).Select(item => item.Value.ToString()).FirstOrDefault();
                if (list != null)
                {
                    returnString = list;
                }
            }
            catch (Exception ex)
            {
                WechatHelper.Log.Error("GetJosnValueByName：获取字符串Json属性失败：" + ex.Message + "     " + ex.StackTrace);
            }

            return returnString;
        }

        #endregion

        #region Xml序列化
        
        /// <summary>
        /// 将对象转换成xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string XmlSerializer<T>(T t)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            xmlSerializer.Serialize(writer, t);
            writer.Close();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Xml反序列化  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                return (T)xmldes.Deserialize(sr);
            }
        }

        /// <summary>
        /// Xml反序列化  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return (T)xmldes.Deserialize(stream);
        }

        #endregion
    }
}
