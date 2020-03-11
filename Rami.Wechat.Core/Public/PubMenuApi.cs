using Rami.Wechat.Core.Comm;
using System.Collections.Generic;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 公众号菜单
    /// </summary>
    public class PubMenuApi
    {
        #region 常量地址

        /// <summary>
        /// 读取菜单
        /// </summary>
        private const string MENU_GET_URL = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}";

        /// <summary>
        /// 保存菜单
        /// </summary>
        private const string MENU_SAVE_URL = " https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";

        /// <summary>
        /// 删除菜单
        /// </summary>
        private const string MENU_DEL_URL = "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}";

        #endregion

        #region 菜单管理

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public static PubApiResult SaveMenu(PubMenus menus)
        {
            var url = string.Format(MENU_SAVE_URL, PubInterface.AccessToken);

            // 处理空的sub_button
            foreach (var pMenu in menus.button)
            {
                if (pMenu.sub_button != null && pMenu.sub_button.Count == 0)
                {
                    pMenu.sub_button = null;
                }

                if (pMenu.sub_button != null && pMenu.sub_button.Count > 0)
                {
                    int cNullMenuCount = 0;
                    foreach (var cMenu in pMenu.sub_button)
                    {
                        if (string.IsNullOrEmpty(cMenu.name))
                        {
                            cNullMenuCount += 1;
                        }
                        cMenu.sub_button = null;
                    }

                    // 如果子节点全是空的
                    if (cNullMenuCount == pMenu.sub_button.Count)
                    {
                        pMenu.sub_button = null;
                    }
                }
            }

            // 序列化json数据
            var jsonValue = SerializeHelper.JsonSerializeNoNull(menus);
            jsonValue.Replace("[{}]", string.Empty);
            return WebApiHelper.PostAsync<PubApiResult>(url, jsonValue);
        }

        /// <summary>
        /// 读取菜单
        /// </summary>
        /// <returns></returns>
        public static PubMenuResult GetMenu()
        {
            var url = string.Format(MENU_GET_URL, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubMenuResult>(url);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        public static PubApiResult DelMenu()
        {
            var url = string.Format(MENU_DEL_URL, PubInterface.AccessToken);
            return WebApiHelper.GetAsync<PubApiResult>(url);
        }

        #endregion
    }

    #region 实体

    /// <summary>
    /// 微信菜单
    /// </summary>
    public class PubMenu
    {
        /// <summary>
        /// 菜单的响应动作类型，view表示网页类型，click表示点击类型，miniprogram表示小程序类型
        /// </summary>
        public string type { set; get; }
        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过60个字节
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 网页 链接，用户点击菜单可打开链接，不超过1024字节。 type为miniprogram时，不支持小程序的老版本客户端将打开本url
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// </summary>
        public string media_id { get; set; }
        /// <summary>
        /// 小程序的appid（仅认证公众号可配置）
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 小程序的页面路径
        /// </summary>
        public string pagepath { get; set; }
        /// <summary>
        /// 二级菜单数组，个数应为1~5个
        /// </summary>
        public List<PubMenu> sub_button { get; set; }
    }

    /// <summary>
    /// 菜单组
    /// </summary>
    public class PubMenus
    {
        /// <summary>
        /// 一级菜单数组，个数应为1~3个
        /// </summary>
        public List<PubMenu> button { get; set; }
    }

    /// <summary>
    /// 自定义菜单
    /// </summary>
    public class PubMenuResult : PubApiResult
    {
        /// <summary>
        /// 菜单返回结果
        /// </summary>
        public PubMenus menu { get; set; }
    }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum PubMenuType
    {
        /// <summary>
        /// 用户点击click类型按钮后，微信服务器会通过消息接口推送消息类型为event	的结构给开发者（参考消息接口指南），并且带上按钮中开发者填写的key值，开发者可以通过自定义的key值与用户进行交互
        /// </summary>
        click,
        /// <summary>
        /// 用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息
        /// </summary>
        view,
        /// <summary>
        /// 用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后显示扫描结果（如果是URL，将进入URL），且会将扫码的结果传给开发者，开发者可以下发消息
        /// </summary>
        scancode_push,
        /// <summary>
        /// 用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后，将扫码的结果传给开发者，同时收起扫一扫工具，然后弹出“消息接收中”提示框，随后可能会收到开发者下发的消息
        /// </summary>
        scancode_waitmsg,
        /// <summary>
        /// 用户点击按钮后，微信客户端将调起系统相机，完成拍照操作后，会将拍摄的相片发送给开发者，并推送事件给开发者，同时收起系统相机，随后可能会收到开发者下发的消息
        /// </summary>
        pic_sysphoto,
        /// <summary>
        /// 用户点击按钮后，微信客户端将弹出选择器供用户选择“拍照”或者“从手机相册选择”。用户选择后即走其他两种流程
        /// </summary>
        pic_photo_or_album,
        /// <summary>
        /// 用户点击按钮后，微信客户端将调起微信相册，完成选择操作后，将选择的相片发送给开发者的服务器，并推送事件给开发者，同时收起相册，随后可能会收到开发者下发的消息
        /// </summary>
        pic_weixin,
        /// <summary>
        /// 用户点击按钮后，微信客户端将调起地理位置选择工具，完成选择操作后，将选择的地理位置发送给开发者的服务器，同时收起位置选择工具，随后可能会收到开发者下发的消息
        /// </summary>
        location_select,
        /// <summary>
        /// 小程序
        /// </summary>
        miniprogram,
    }

    #endregion
}
