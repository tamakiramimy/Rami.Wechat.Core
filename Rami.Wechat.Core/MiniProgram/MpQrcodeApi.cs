using Rami.Wechat.Core.Comm;
using System.IO;
using System.Net.Http;

namespace Rami.Wechat.Core.MiniProgram
{
    /// <summary>
    /// 小程序二维码api
    /// </summary>
    public class MpQrcodeApi
    {
        /// <summary>
        /// 获取小程序二维码，适用于需要的码数量较少的业务场景。通过该接口生成的小程序码，永久有效，有数量限制
        /// </summary>
        private const string CREATE_WX_AQRCODE = "https://api.weixin.qq.com/cgi-bin/wxaapp/createwxaqrcode?access_token={0}";

        /// <summary>
        /// 获取小程序码，适用于需要的码数量较少的业务场景。通过该接口生成的小程序码，永久有效，有数量限制
        /// </summary>
        private const string GET_WX_ACODE = "https://api.weixin.qq.com/wxa/getwxacode?access_token={0}";

        /// <summary>
        /// 获取小程序码，适用于需要的码数量极多的业务场景。通过该接口生成的小程序码，永久有效，数量暂无限制
        /// </summary>
        private const string GET_WX_ACODE_UNLIMIT = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={0}";

        /// <summary>
        /// 创建小程序二维码（限制10W个）
        /// </summary>
        /// <param name="path">扫码进入的小程序页面路径，最大长度 128 字节，不能为空</param>
        /// <param name="width">二维码的宽度，单位 px。最小 280px，最大 1280px</param>
        public static QrResult CreateWxAQRCode(string path, int? width = null)
        {
            var url = string.Format(CREATE_WX_AQRCODE, MpInterface.AccessToken);
            var postData = SerializeHelper.JsonSerializeNoNull(new { path = path, width = width });
            return DealQrResult(url, postData);
        }

        /// <summary>
        /// 获取小程序码(限制10W个）
        /// </summary>
        /// <param name="path">扫码进入的小程序页面路径，最大长度 128 字节，不能为空</param>
        /// <param name="width">二维码的宽度，单位 px。最小 280px，最大 1280px</param>
        /// <param name="auto_color">自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调</param>
        /// <param name="line_color">auto_color 为 false 时生效，使用 rgb 设置颜色 例如 {"r":"xxx","g":"xxx","b":"xxx"} 十进制表示</param>
        /// <param name="is_hyaline">是否需要透明底色，为 true 时，生成透明底色的小程序码</param>
        public static QrResult GetWXACode(string path, int? width = null, bool auto_color = false, MpQrLineColor line_color = null, bool is_hyaline = false)
        {
            var url = string.Format(GET_WX_ACODE, MpInterface.AccessToken);
            var jsonData = new { path = path, width = width, auto_color = auto_color, line_color = line_color, is_hyaline = is_hyaline };
            var postData = SerializeHelper.JsonSerializeNoNull(jsonData);
            return DealQrResult(url, postData);
        }

        /// <summary>
        /// 获取小程序码
        /// </summary>
        /// <param name="scene">最大32个可见字符，只支持数字，大小写英文以及部分特殊字符：!#$&amp;'()*+,/:;=?@-._~，其它字符请自行编码为合法字符（因不支持%，中文无法使用 urlencode 处理，请使用其他编码方式</param>
        /// <param name="page">必须是已经发布的小程序存在的页面（否则报错），例如 pages/index/index, 根路径前不要填加 /,不能携带参数（参数请放在scene字段里），如果不填写这个字段，默认跳主页面</param>
        /// <param name="width">二维码的宽度，单位 px，最小 280px，最大 1280px</param>
        /// <param name="auto_color">自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调，默认 false</param>
        /// <param name="line_color">auto_color 为 false 时生效，使用 rgb 设置颜色 例如 {"r":"xxx","g":"xxx","b":"xxx"} 十进制表示</param>
        /// <param name="is_hyaline">是否需要透明底色，为 true 时，生成透明底色的小程序</param>
        public static QrResult GetWXACodeUnlimit(string scene, string page = "", int? width = null, bool auto_color = false, MpQrLineColor line_color = null, bool is_hyaline = false)
        {
            var url = string.Format(GET_WX_ACODE_UNLIMIT, MpInterface.AccessToken);
            var jsonData = new { scene = scene, page = page, width = width, auto_color = auto_color, line_color = line_color, is_hyaline = is_hyaline };
            var postData = SerializeHelper.JsonSerializeNoNull(jsonData);
            return DealQrResult(url, postData);
        }

        /// <summary>
        /// 处理二维码返回结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static QrResult DealQrResult(string url, string postData)
        {
            using (var client = new HttpClient())
            {
                var content = client.PostAsync(url, new StringContent(postData)).Result.Content;
                if (content.Headers.ContentType.MediaType == "image/jpeg")
                {
                    // 生成成功直接返回文件流
                    var stream = content.ReadAsStreamAsync().Result;
                    return new QrResult { errcode = 0, QrStream = stream };
                }
                else
                {
                    var result = content.ReadAsStringAsync().Result;
                    var jsonRes = SerializeHelper.JsonDeserialize<QrResult>(result);
                    return jsonRes;
                }
            }
        }

        /// <summary>
        /// 保存二维码路径
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="phyPath"></param>
        public static void SaveQrImg(Stream stream, string phyPath)
        {
            using (var fileStream = new FileStream(phyPath, FileMode.Create))
            {
                stream.CopyTo(fileStream);
                stream.Close();
            }
        }
    }

    /// <summary>
    /// 二维码结果
    /// </summary>
    public class QrResult : MpApiRes
    {
        /// <summary>
        /// 二维码文件流
        /// </summary>
        public Stream QrStream { get; set; }
    }

    /// <summary>
    /// 二维码颜色
    /// </summary>
    public class MpQrLineColor
    {
        /// <summary>
        /// 红色
        /// </summary>
        public int r { get; set; }
        /// <summary>
        /// 绿色
        /// </summary>
        public int g { get; set; }
        /// <summary>
        /// 蓝色
        /// </summary>
        public int b { get; set; }
    }
}
