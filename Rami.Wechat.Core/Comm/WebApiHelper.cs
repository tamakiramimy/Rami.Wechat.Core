using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rami.Wechat.Core.Comm
{
    /// <summary>
    /// WebApi帮助类
    /// </summary>
    internal class WebApiHelper
    {
        /// <summary>
        /// client
        /// </summary>
        private static HttpClient client = new HttpClient();

        #region 同步操作

        /// <summary>
        /// 获取HttpClient
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        private static HttpClient GetClient(HttpClient httpClient = null)
        {
            return httpClient ?? client;
        }

        /// <summary>
        /// Get请求(返回string，自行解析)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static string GetAsyncStr(string url, HttpClient httpClient = null)
        {
            return GetClient(httpClient).GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static T GetAsync<T>(string url, HttpClient httpClient = null)
        {
            var strRes = GetAsyncStr(url, httpClient);
            return SerializeHelper.JsonDeserialize<T>(strRes);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static string PostAsyncStr(string url, object data, Encoding encoding = null, HttpClient httpClient = null)
        {
            string body = (data is string) ? (string)data : SerializeHelper.JsonSerialize(data);
            var res = GetClient(httpClient).PostAsync(url, new StringContent(body, encoding)).Result.Content.ReadAsStringAsync().Result;
            return res;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static T PostAsync<T>(string url, object data, HttpClient httpClient = null)
        {
            var strRes = PostAsyncStr(url, data, httpClient: httpClient);
            return SerializeHelper.JsonDeserialize<T>(strRes);
        }

        /// <summary>
        /// Post请求HttpContent
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContent"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static string PostHttpContentStr(string url, HttpContent httpContent, HttpClient httpClient = null)
        {
            var res = GetClient(httpClient).PostAsync(url, httpContent).Result.Content.ReadAsStringAsync().Result;
            return res;
        }

        /// <summary>
        /// Post请求HttpContent
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContent"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static T PostHttpContent<T>(string url, HttpContent httpContent, HttpClient httpClient = null)
        {
            var strRes = PostHttpContentStr(url, httpContent, httpClient);
            return SerializeHelper.JsonDeserialize<T>(strRes);
        }

        #endregion

        #region 异步操作

        /// <summary>
        /// Get请求(返回string，自行解析)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<string> GetAsyncStr2(string url, HttpClient httpClient = null)
        {
            var result = await GetClient(httpClient).GetAsync(url);
            return await result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync2<T>(string url, HttpClient httpClient = null)
        {
            var strRes = await GetAsyncStr2(url, httpClient);
            return SerializeHelper.JsonDeserialize<T>(strRes);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<string> PostAsyncStr2(string url, object data, Encoding encoding = null, HttpClient httpClient = null)
        {
            string body = (data is string) ? (string)data : SerializeHelper.JsonSerialize(data);
            var result = await GetClient(httpClient).PostAsync(url, new StringContent(body, encoding));
            return await result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync2<T>(string url, object data, HttpClient httpClient = null)
        {
            var strRes = await PostAsyncStr2(url, data, httpClient: httpClient);
            return SerializeHelper.JsonDeserialize<T>(strRes);
        }

        /// <summary>
        /// Post请求HttpContent
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContent"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<string> PostHttpContentStr2(string url, HttpContent httpContent, HttpClient httpClient = null)
        {
            var result = await GetClient(httpClient).PostAsync(url, httpContent);
            return await result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post请求HttpContent
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContent"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<T> PostHttpContent2<T>(string url, HttpContent httpContent, HttpClient httpClient = null)
        {
            var strRes = await PostHttpContentStr2(url, httpContent, httpClient);
            return SerializeHelper.JsonDeserialize<T>(strRes);
        }

        #endregion
    }
}
