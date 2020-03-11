using Rami.Wechat.Core.Comm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 素材api
    /// </summary>
    public class PubMediaApi
    {
        #region 常量地址

        /// <summary>
        /// 新增临时素材
        /// </summary>
        private const string ADD_MEDIA_URL = "https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        /// <summary>
        /// 读取临时素材
        /// </summary>
        private const string GET_MEDIA_URL = "https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";

        /// <summary>
        /// 上传图片
        /// </summary>
        private const string UPLOAD_IMG_URL = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}";

        /// <summary>
        /// 新增永久图文素材
        /// </summary>
        private const string ADD_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token={0}";

        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        private const string UPD_NEWS_URL = "https://api.weixin.qq.com/cgi-bin/material/update_news?access_token={0}";

        /// <summary>
        /// 新增永久素材
        /// </summary>
        private const string ADD_MATERIAL_URL = "https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}";

        /// <summary>
        /// 读取永久素材
        /// </summary>
        private const string GET_MATERIAL_URL = "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token={0}";

        /// <summary>
        /// 删除永久素材
        /// </summary>
        private const string DEL_MATERIAL_URL = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token={0}";

        /// <summary>
        /// 读取永久素材统计
        /// </summary>
        private const string GET_MATERIAL_COUNT_URL = "https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token={0}";

        /// <summary>
        /// 读取永久素材列表
        /// </summary>
        private const string GET_MATERIAL_LIST_URL = "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}";

        /// <summary>
        /// 读取永久视频素材列表
        /// </summary>
        private const string GET_MATERIAL_VEDIO_LIST_URL = "http://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}";

        #endregion

        #region 素材管理

        /// <summary>
        /// 新增临时素材
        /// </summary>
        /// <param name="file">文件流</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="type">媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb）</param>
        /// <returns></returns>
        public static PubMediaResult AddMedia(string fileName, Stream file, PubMediaType type)
        {
            var url = string.Format(ADD_MEDIA_URL, PubInterface.AccessToken, type.ToString());

            var content = PubInterface.BuildFromContent(fileName, file);
            var res = WebApiHelper.PostHttpContent<PubMediaResult>(url, content);
            return res;
        }

        /// <summary>
        /// 新增临时素材
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PubMediaResult AddMedia(string filePath, PubMediaType type)
        {
            return AddMedia(Path.GetFileName(filePath), File.OpenRead(filePath), type);
        }

        /// <summary>
        /// 新增图文消息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static PubMediaResult AddNews(List<PubMediaArticle> list)
        {
            var url = string.Format(ADD_NEWS_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubMediaResult>(url, new { articles = list });
        }

        /// <summary>
        /// 上传图片文件
        /// </summary>
        /// <returns>仅url有效</returns>
        public static PubMediaResult UploadImg(string filePath)
        {
            var url = string.Format(UPLOAD_IMG_URL, PubInterface.AccessToken);
            var content = PubInterface.BuildFromContent(Path.GetFileName(filePath), File.OpenRead(filePath));
            return WebApiHelper.PostHttpContent<PubMediaResult>(url, content);
        }

        /// <summary>
        /// 修改图文消息
        /// </summary>
        /// <param name="articles">图文消息</param>
        /// <param name="mediaID">要修改的图文消息的id</param>        
        /// <param name="index">要更新的文章在图文消息中的位置（多图文消息时，此字段才有意义），第一篇为0</param>
        /// <returns></returns>
        public static PubMediaResult UpdateNews(PubMediaArticle articles, string mediaID, int index = 0)
        {
            var url = string.Format(UPD_NEWS_URL, PubInterface.AccessToken);
            var jsonData = new { media_id = mediaID, index = index, articles = articles };
            return WebApiHelper.PostAsync<PubMediaResult>(url, jsonData);
        }

        /// <summary>
        /// 新增永久素材
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        /// <param name="title"></param>
        /// <param name="introduction"></param>
        /// <returns></returns>
        public static PubMediaResult AddMaterial(PubMediaType type, string filePath, string title, string introduction)
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
        public static PubMediaResult AddMaterial(PubMediaType type, Stream file, string fileName, string title, string introduction)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");//重要，一定要标明
            if (type == PubMediaType.video)
            {
                var video = "{\"title\":\"" + title + "\", \"introduction\":\"" + introduction + "\"}";
                var videoContent = new StringContent(video);
                videoContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"description\"",
                };
                form.Add(videoContent);
            }

            form.Add(PubInterface.BuildStreamContent(fileName, file));
            var url = string.Format(ADD_MATERIAL_URL, PubInterface.AccessToken, type.ToString());
            var res = WebApiHelper.PostHttpContent<PubMediaResult>(url, form);
            return res;
        }

        /// <summary>
        /// 自行处理 有可能是下载 有可能是返回值
        /// </summary>
        public static void GetMaterial(string mediaID, string filePath, PubMediaType mType)
        {
            var data = SerializeHelper.JsonSerialize(new { media_id = mediaID });
            var url = string.Format(GET_MATERIAL_URL, PubInterface.AccessToken);
            if (mType == PubMediaType.video)
            {
                WechatHelper.Log.Error("GetMaterial:请使用GetMaterialVedio接口下载:" + mediaID);
            }
            else
            {
                DownloadFilePost(url, data, filePath);
            }
        }

        /// <summary>
        /// 下载视频素材
        /// </summary>
        /// <param name="mediaID"></param>
        /// <param name="mType"></param>
        /// <returns></returns>
        public static PubVedioMeterialResult GetMaterialVedio(string mediaID, PubMediaType mType)
        {
            var data = SerializeHelper.JsonSerialize(new { media_id = mediaID });
            var url = string.Format(GET_MATERIAL_URL, PubInterface.AccessToken);
            if (mType != PubMediaType.video)
            {
                WechatHelper.Log.Error("GetMaterialVedio:请使用GetMaterial接口下载:" + mediaID);
                return null;
            }

            var vRes = WebApiHelper.PostAsync<PubVedioMeterialResult>(url, data);
            return vRes;
        }

        /// <summary>
        /// 获取图文消息文本内容(接口待测试)
        /// </summary>
        /// <param name="mediaID"></param>
        /// <returns></returns>
        public static PubMediaNewsResult GetNewsMaterial(string mediaID)
        {
            var data = new { media_id = mediaID };
            var url = string.Format(GET_MATERIAL_URL, PubInterface.AccessToken);
            var res = WebApiHelper.PostAsync<PubMediaNewsResult>(url, data);
            return res;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="filepath"></param>
        public static void DownloadFilePost(string url, string postData, string filepath)
        {
            try
            {
                FileInfo file = new FileInfo(filepath);
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                using (var client = new HttpClient())
                {
                    var stream = client.PostAsync(url, new StringContent(postData)).Result.Content.ReadAsStreamAsync().Result;
                    using (var sw = new StreamWriter(filepath))
                    {
                        stream.CopyTo(sw.BaseStream);
                        sw.Flush();
                    }

                    stream.Close();
                }

                #region 旧方法

                //// FileStream
                //using (var client = new HttpClient())
                //{
                //    var stream = client.PostAsync(url, new StringContent(postData)).Result.Content.ReadAsStreamAsync().Result;
                //    using (var fileStream = File.Create(filepath))
                //    {
                //        byte[] buffer = new byte[1024];
                //        int bytesRead = 0;
                //        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                //        {
                //            fileStream.Write(buffer, 0, bytesRead);
                //        }
                //        fileStream.Flush();
                //    }
                //    stream.Close();
                //}

                //// BinaryWriter
                //using (var fs = File.Create(filepath))
                //{
                //    using (var bw = new BinaryWriter(fs))
                //    {
                //        byte[] bytes = new byte[stream.Length];
                //        stream.Read(bytes, 0, bytes.Length);
                //        stream.Seek(0, SeekOrigin.Begin);
                //        bw.Write(bytes);
                //    }
                //}
                //stream.Close();

                //// BinaryWriter
                //using (var fs = File.Create(filepath))
                //{
                //    using (var bw = new BinaryWriter(fs))
                //    {
                //        byte[] buffer = new byte[1024];
                //        int bytesRead = 0;
                //        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                //        {
                //            bw.Write(buffer, 0, bytesRead);
                //        }
                //        bw.Flush();
                //    }
                //}
                //stream.Close();

                //HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //byte[] b = Encoding.UTF8.GetBytes(postData);
                //request.ContentType = "application/json";
                //request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; MALCJS; rv:11.0) like Gecko";
                //request.Referer = url;
                //request.Method = "post";
                //request.ContentLength = b.Length;

                //using (Stream stream = request.GetRequestStream())
                //{
                //    stream.Write(b, 0, b.Length);
                //}

                //HttpWebResponse response = null;

                ////获取服务器返回的资源
                //using (response = request.GetResponse() as HttpWebResponse)
                //{
                //    using (Stream sr = response.GetResponseStream())
                //    {
                //        using (System.IO.StreamWriter objwrite = new System.IO.StreamWriter(filepath))
                //        {
                //            int k = 0;
                //            while (k != -1)
                //            {
                //                k = sr.ReadByte();
                //                if (k != -1)
                //                {
                //                    objwrite.BaseStream.WriteByte((byte)k);
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion
            }
            catch (Exception ex)
            {
                WechatHelper.Log.Error("DownloadFilePost下载微信文件失败：" + ex.Message + "  " + ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除永久素材
        /// </summary>
        public static PubApiResult DeleteMaterial(string mediaID)
        {
            var url = string.Format(DEL_MATERIAL_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubApiResult>(url, new { media_id = mediaID });
        }

        /// <summary>
        /// 永久素材统计
        /// </summary>
        public static PubMeterialSata GetMaterialCount()
        {
            var url = string.Format(GET_MATERIAL_COUNT_URL, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubMeterialSata>(url);
        }

        /// <summary>
        /// 读取永久素材列表
        /// </summary>
        /// <param name="type">类型 不能为news</param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 默认0</param>
        /// <param name="count">返回素材的数量，取值在1到20之间，默认20</param>
        /// <returns></returns>
        public static PubMeterialResult GetMaterialList(PubMediaType type, int offset, int count = 20)
        {
            if (type == PubMediaType.news)
            {
                throw new Exception("news请使用GetMaterialNewsList");
            }
            return GetMaterialList<PubMeterialResult>(new { type = type.ToString(), offset = offset, count = count });
        }

        /// <summary>
        /// 读取永久图文列表
        /// </summary>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 默认0</param>
        /// <param name="count">返回素材的数量，取值在1到20之间，默认20</param>
        /// <returns></returns>
        public static PubNewsResult GetMaterialNewsList(int offset, int count = 20)
        {
            return GetMaterialList<PubNewsResult>(new { type = PubMediaType.news.ToString(), offset = offset, count = count });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        private static T GetMaterialList<T>(object para) where T : PubApiResult
        {
            var url = string.Format(GET_MATERIAL_LIST_URL, PubInterface.AccessToken);
            dynamic obj = para as dynamic;
            if (obj.type == "video")
            {
                url = string.Format(GET_MATERIAL_VEDIO_LIST_URL, PubInterface.AccessToken);
            }

            return WebApiHelper.PostAsync<T>(url, para);
        }

        #endregion
    }

    #region 实体

    /// <summary>
    /// 媒体文件类型
    /// </summary>
    public enum PubMediaType
    {
        /// <summary>
        /// 图片
        /// </summary>
        image,

        /// <summary>
        /// 语音
        /// </summary>
        voice,

        /// <summary>
        /// 视频
        /// </summary>
        video,

        /// <summary>
        /// 缩略图
        /// </summary>
        thumb,

        /// <summary>
        /// 图文
        /// </summary>
        news,
    }

    /// <summary>
    /// PubMediaResult
    /// </summary>
    public class PubMediaResult : PubApiResult
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
    public class PubMediaArticle
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
        /// 图文消息的封面图片的地址，第三方开发者也可以使用这个URL下载图片到自己服务器中，然后显示在自己网站上
        /// </summary>
        public string thumb_url { get; set; }

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
        /// 图文页的URL，或者，当获取的列表是图片素材列表时，该字段是图片的URL
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 图文消息的原文地址，即点击“阅读原文”后的URL
        /// </summary>
        public string content_source_url { get; set; }
    }

    /// <summary>
    /// 图文消息结果
    /// </summary>
    public class PubNewsResult : PubApiResult
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
        public List<PubNews> item { get; set; }
    }

    /// <summary>
    /// 图文消息
    /// </summary>
    public class PubNews
    {
        /// <summary>
        /// media_id
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// content
        /// </summary>
        public PubNewsContent content { get; set; }

        /// <summary>
        /// 这篇图文消息素材的最后更新时间
        /// </summary>
        public int update_time { get; set; }
    }

    /// <summary>
    /// 获取公众号图文媒体ID结果
    /// </summary>
    public class PubMediaNewsResult : PubApiResult
    {
        /// <summary>
        /// 图文列表
        /// </summary>
        public List<PubMediaArticle> news_item { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int create_time { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int update_time { get; set; }
    }

    /// <summary>
    /// 图文消息
    /// </summary>
    public class PubNewsContent
    {
        /// <summary>
        /// news_item
        /// </summary>
        public List<PubMediaArticle> news_item { get; set; }
    }

    /// <summary>
    /// 永久素材统计
    /// </summary>
    public class PubMeterialSata : PubApiResult
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
    public class PubMeterialResult : PubApiResult
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
        public List<PubMeterial> item { get; set; }
    }

    /// <summary>
    /// 永久素材
    /// </summary>
    public class PubMeterial
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

    /// <summary>
    /// 视频素材结果
    /// </summary>
    public class PubVedioMeterialResult : PubApiResult
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        public string down_url { get; set; }
    }

    #endregion
}
