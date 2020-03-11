using Rami.Wechat.Core.Comm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 素材API
    /// </summary>
    public class EntMediaApi
    {
        #region 常量地址

        /// <summary>
        /// 新增临时素材
        /// </summary>
        private const string ADD_MEDIA_URL = "https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        /// <summary>
        /// 读取临时素材
        /// </summary>
        private const string GET_MEDIA_URL = "https://qyapi.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";

        /// <summary>
        /// 修改永久图文素材图片
        /// </summary>
        private const string UPLOAD_IMG_URL = "https://qyapi.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}";

        /// <summary>
        /// 新增永久图文素材
        /// </summary>
        private const string ADD_NEWS_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/add_news?access_token={0}";

        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        private const string UPD_NEWS_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/update_news?access_token={0}";

        /// <summary>
        /// 新增永久素材
        /// </summary>
        private const string ADD_MATERIAL_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/add_material?access_token={0}";

        /// <summary>
        /// 读取永久素材
        /// </summary>
        private const string GET_MATERIAL_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/get_material?access_token={0}";

        /// <summary>
        /// 删除永久素材
        /// </summary>
        private const string DEL_MATERIAL_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/del_material?access_token={0}";

        /// <summary>
        /// 读取永久素材统计
        /// </summary>
        private const string GET_MATERIAL_COUNT_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/get_materialcount?access_token={0}";

        /// <summary>
        /// 读取永久素材列表
        /// </summary>
        private const string GET_MATERIAL_LIST_URL = "https://qyapi.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}";

        #endregion

        #region 图文消息

        /// <summary>
        /// 新增图文消息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static EntMediaResult AddNews(List<EntMediaArticle> list)
        {
            var url = string.Format(ADD_NEWS_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntMediaResult>(url, new { articles = list });
        }

        /// <summary>
        /// 修改图文消息
        /// </summary>
        /// <param name="articles">图文消息</param>
        /// <param name="mediaID">要修改的图文消息的id</param>        
        /// <param name="index">要更新的文章在图文消息中的位置（多图文消息时，此字段才有意义），第一篇为0</param>
        /// <returns></returns>
        public static EntMediaResult UpdateNews(List<EntMediaArticle> articles, string mediaID, int index = 0)
        {
            var url = string.Format(UPD_NEWS_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntMediaResult>(url, new { articles = articles, media_id = mediaID, index = index });
        }

        #endregion

        #region 临时素材

        /// <summary>
        /// 新增临时素材
        /// </summary>
        /// <param name="file">文件流</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="type">媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb）</param>
        /// <returns></returns>
        public static EntMediaResult AddMedia(Stream file, string fileName, EntMediaType type)
        {
            MultipartFormDataContent content = EntInterface.BuildFromContent(fileName, file);

            var url = string.Format(ADD_MEDIA_URL, EntInterface.AccessToken, type.ToString());
            return WebApiHelper.PostHttpContent<EntMediaResult>(url, content);
        }

        /// <summary>
        /// 获取文件的长度
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static int GetFileLength(Stream file)
        {
            // 获取文件流的长度
            byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            file.Seek(0, SeekOrigin.Begin);
            return bytes.Length;
        }

        /// <summary>
        /// 新增临时素材
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EntMediaResult AddMedia(string filePath, EntMediaType type)
        {
            return AddMedia(File.OpenRead(filePath), Path.GetFileName(filePath), type);
        }

        /// <summary>
        /// 获取临时素材 直接为下载
        /// </summary>
        public static void GetMedia()
        {
        }

        #endregion

        #region 永久素材

        /// <summary>
        /// 新增永久素材
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="introduction"></param>
        /// <returns></returns>
        public static EntMediaResult AddMaterial(string filePath, string title, EntMediaType type, string introduction)
        {
            return AddMaterial(type, File.OpenRead(filePath), Path.GetFileName(filePath), title, introduction);
        }

        /// <summary>
        /// 新增永久素材
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="title">视频时有效</param>
        /// <param name="introduction">视频时有效</param>
        /// <returns></returns>
        public static EntMediaResult AddMaterial(EntMediaType type, Stream file, string fileName, string title, string introduction)
        {
            MultipartFormDataContent form = BuildFromContentForever(fileName, file, type, title, introduction);

            var url = string.Format(ADD_MATERIAL_URL, EntInterface.AccessToken);
            var result = WebApiHelper.PostAsyncStr(url, form);
            return SerializeHelper.JsonDeserialize<EntMediaResult>(result);
            //return WebApiHelper.PostAsync<EntMediaResult>(url, form);
        }

        /// <summary>
        /// form文件头（永久素材）
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <param name="title"></param>
        /// <param name="introduction"></param>
        /// <returns></returns>
        public static MultipartFormDataContent BuildFromContentForever(string fileName, Stream file, EntMediaType type, string title, string introduction)
        {
            var Boundary = EntInterface.Boundary;
            MultipartFormDataContent form = new MultipartFormDataContent(Boundary);
            // 文件头
            form.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data;boundary=" + Boundary);
            // 视频特殊处理
            if (type == EntMediaType.video)
            {
                var video = "{\"title\":\"" + title + "\", \"introduction\":\"" + introduction + "\"}";
                var videoContent = new StringContent(video);
                videoContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"description\"",
                };

                form.Add(videoContent);
            }

            var content = EntInterface.BuildStreamContent(fileName, file);
            form.Add(content);
            return form;
        }

        /// <summary>
        /// 自行处理 有可能是下载 有可能是返回值
        /// </summary>
        public static void GetMaterial()
        {
        }

        /// <summary>
        /// 删除永久素材
        /// </summary>
        public static EntApiResult DeleteMaterial(string mediaID)
        {
            var data = string.Format("{{\"media_id\":\"{0}\"}}", mediaID);
            var url = string.Format(GET_MATERIAL_COUNT_URL, EntInterface.Conf.AppId, EntInterface.Conf.AppSecret);
            return WebApiHelper.PostAsync<EntApiResult>(url, data);
        }

        /// <summary>
        /// 永久素材统计
        /// </summary>
        public static EntMeterialSata GetMaterialCount()
        {
            var url = string.Format(GET_MATERIAL_COUNT_URL, EntInterface.AccessToken);
            return WebApiHelper.GetAsync<EntMeterialSata>(url);
        }

        /// <summary>
        /// 读取永久素材列表
        /// </summary>
        /// <param name="type">类型 不能为news</param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 默认0</param>
        /// <param name="count">返回素材的数量，取值在1到20之间，默认20</param>
        /// <returns></returns>
        public static EntMeterialResult GetMaterialList(EntMediaType type, int offset, int count = 20)
        {
            var url = string.Format(GET_MATERIAL_LIST_URL, EntInterface.AccessToken);
            if (type == EntMediaType.news)
            {
                throw new Exception("news请使用GetMaterialNewsList");
            }

            return WebApiHelper.PostAsync<EntMeterialResult>(url, new { type = type.ToString(), offset = offset, count = count });
        }

        /// <summary>
        /// 读取永久图文列表
        /// </summary>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 默认0</param>
        /// <param name="count">返回素材的数量，取值在1到20之间，默认20</param>
        /// <returns></returns>
        public static EntNewsResult GetMaterialNewsList(int offset, int count = 20)
        {
            var url = string.Format(GET_MATERIAL_LIST_URL, EntInterface.AccessToken);
            return WebApiHelper.PostAsync<EntNewsResult>(url, new { type = EntMediaType.news.ToString(), offset = offset, count = count });
        }

        #endregion
    }

    #region 实体

    /// <summary>
    /// 媒体文件类型
    /// </summary>
    public enum EntMediaType
    {
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        image,

        /// <summary>
        /// 语音
        /// </summary>
        [Description("语音")]
        voice,

        /// <summary>
        /// 视频
        /// </summary>
        [Description("视频")]
        video,

        /// <summary>
        /// 缩略图
        /// </summary>
        [Description("缩略图")]
        thumb,

        /// <summary>
        /// 图文
        /// </summary>
        [Description("图文")]
        news,

        /// <summary>
        /// 文本
        /// </summary>
        [Description("文本")]
        text,
    }

    /// <summary>
    /// EntMediaResult
    /// </summary>
    public class EntMediaResult : EntApiResult
    {
        /// <summary>
        /// type
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// media_id
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// created_at
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string url { get; set; }
    }

    /// <summary>
    /// 永久图文素材
    /// </summary>
    public class EntMediaArticle
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 图文消息的封面图片素材id（必须是永久mediaID）
        /// </summary>
        public string thumb_media_id { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 图文消息的摘要，仅有单图文消息才有摘要，多图文此处为空
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        /// 是否显示封面，0为false，即不显示，1为true，即显示
        /// </summary>
        public int show_cover_pic { get; set; }

        /// <summary>
        /// 图文消息的具体内容，支持HTML标签，必须少于2万字符，小于1M，且此处会去除JS
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 图文消息的原文地址，即点击“阅读原文”后的URL
        /// </summary>
        public string content_source_url { get; set; }
    }

    /// <summary>
    /// EntNewsResult
    /// </summary>
    public class EntNewsResult : EntApiResult
    {
        /// <summary>
        /// total_count
        /// </summary>
        public int total_count { get; set; }

        /// <summary>
        /// item_count
        /// </summary>
        public int item_count { get; set; }

        /// <summary>
        /// item
        /// </summary>
        public List<EntNews> item { get; set; }
    }

    /// <summary>
    /// EntNews
    /// </summary>
    public class EntNews
    {
        /// <summary>
        /// media_id
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// content
        /// </summary>
        public EntNewsContent content { get; set; }

        /// <summary>
        /// 这篇图文消息素材的最后更新时间
        /// </summary>
        public int update_time { get; set; }
    }

    /// <summary>
    /// EntNewsContent
    /// </summary>
    public class EntNewsContent
    {
        /// <summary>
        /// news_item
        /// </summary>
        public List<EntMediaArticle> news_item { get; set; }
    }

    /// <summary>
    /// 永久素材统计
    /// </summary>
    public class EntMeterialSata : EntApiResult
    {
        /// <summary>
        /// 语音总数量
        /// </summary>
        public int voice_count { get; set; }

        /// <summary>
        /// 视频总数量
        /// </summary>
        public int video_count { get; set; }

        /// <summary>
        /// 图片总数量
        /// </summary>
        public int image_count { get; set; }

        /// <summary>
        /// 图文总数量
        /// </summary>
        public int news_count { get; set; }
    }

    /// <summary>
    /// 永久素材列表
    /// </summary>
    public class EntMeterialResult : EntApiResult
    {
        /// <summary>
        /// 该类型的素材的总数
        /// </summary>
        public int total_count { get; set; }

        /// <summary>
        /// 本次调用获取的素材的数量
        /// </summary>
        public int item_count { get; set; }

        /// <summary>
        /// 永久素材列表
        /// </summary>
        public List<EntMeterial> item { get; set; }
    }

    /// <summary>
    /// 永久素材
    /// </summary>
    public class EntMeterial
    {
        /// <summary>
        /// media_id
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// update_time
        /// </summary>
        public string update_time { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string url { get; set; }
    }

    #endregion
}
