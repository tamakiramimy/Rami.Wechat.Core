using Rami.Wechat.Core.Comm;
using System.Net;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 二维码
    /// </summary>
    public class PubQrCodeApi
    {
        /// <summary>
        /// 生成二维码地址
        /// </summary>
        private const string QRCODE_GET_URL = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";

        /// <summary>
        /// 获取二维码地址
        /// </summary>
        private const string QRCODE_GET_IMG_URL = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}";

        /// <summary>
        /// 临时二维码(id形式)
        /// </summary>
        /// <param name="scene_id"></param>
        /// <param name="expire_seconds">超时时间默认7天（604800）</param>
        /// <returns></returns>
        public static PubQRCodeResult GetQRCode(int scene_id, int expire_seconds = 604800)
        {
            var data = new { expire_seconds = expire_seconds, action_name = "QR_SCENE", action_info = new { scene = new { scene_id = scene_id } } };
            var url = string.Format(QRCODE_GET_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubQRCodeResult>(url, data);
        }

        /// <summary>
        /// 临时二维码(字符串形式)
        /// </summary>
        /// <param name="scene_str"></param>
        /// <param name="expire_seconds">超时时间默认7天（604800）</param>
        /// <returns></returns>
        public static PubQRCodeResult GetQRCode(string scene_str, int expire_seconds = 604800)
        {
            var data = new { expire_seconds = expire_seconds, action_name = "QR_STR_SCENE", action_info = new { scene = new { scene_str = scene_str } } };
            var url = string.Format(QRCODE_GET_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubQRCodeResult>(url, data);
        }

        /// <summary>
        /// 永久二维码（id形式，id最大为100000）
        /// </summary>
        /// <param name="scene_id"></param>
        /// <returns></returns>
        public static PubQRCodeResult GetLimitQRCode(int scene_id)
        {
            var data = new { action_name = "QR_LIMIT_SCENE", action_info = new { scene = new { scene_id = scene_id } } };
            var url = string.Format(QRCODE_GET_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubQRCodeResult>(url, data);
        }

        /// <summary>
        /// 永久二维码（字符串形式）
        /// </summary>
        /// <param name="scene_str"></param>
        /// <returns></returns>
        public static PubQRCodeResult GetLimitQRCode(string scene_str)
        {
            var data = new { action_name = "QR_LIMIT_STR_SCENE", action_info = new { scene = new { scene_str = scene_str } } };
            var url = string.Format(QRCODE_GET_URL, PubInterface.AccessToken);
            return WebApiHelper.PostAsync<PubQRCodeResult>(url, data);
        }

        /// <summary>
        /// 保存二维码
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="path"></param>
        public static void DownQrCode(string ticket, string path)
        {
            var downUrl = string.Format(QRCODE_GET_IMG_URL, ticket);
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(downUrl, path);
            }
        }
    }

    /// <summary>
    /// 二维码结果
    /// </summary>
    public class PubQRCodeResult : PubApiResult
    {
        /// <summary>
        /// 获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
        /// </summary>
        public string ticket { get; set; }

        /// <summary>
        /// 二维码的有效时间，以秒为单位。最大不超过1800。
        /// </summary>
        public int expire_seconds { get; set; }

        /// <summary>
        /// 二维码图片解析后的地址，开发者可根据该地址自行生成需要的二维码图片
        /// </summary>
        public string url { get; set; }
    }
}
