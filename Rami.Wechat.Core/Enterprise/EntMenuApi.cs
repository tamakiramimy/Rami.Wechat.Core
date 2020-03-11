using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Linq;

namespace Rami.Wechat.Core.Enterprise
{
    /// <summary>
    /// 企业号菜单Api
    /// </summary>
    public class EntMenuApi
    {
        #region 常量地址

        /// <summary>
        /// 读取菜单
        /// </summary>
        private const string MENU_GET_URL = "https://qyapi.weixin.qq.com/cgi-bin/menu/get?access_token={0}&agentid={1}";

        /// <summary>
        /// 保存菜单
        /// </summary>
        private const string MENU_SAVE_URL = " https://qyapi.weixin.qq.com/cgi-bin/menu/create?access_token={0}&agentid={1}";

        /// <summary>
        /// 删除菜单
        /// </summary>
        private const string MENU_DEL_URL = "https://qyapi.weixin.qq.com/cgi-bin/menu/delete?access_token={0}&agentid={1}";

        #endregion

        #region 菜单管理

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public static EntApiResult SaveMenu(EntMenus menus)
        {
            // 菜单特殊处理
            foreach (var lv1 in menus.button)
            {
                if (lv1.sub_button == null || (lv1.sub_button != null && lv1.sub_button.Count == 0))
                {
                    lv1.sub_button = null;
                }
                else
                {
                    // 判断是否全是空的子菜单
                    var cNullCount = lv1.sub_button.Where(x => string.IsNullOrEmpty(x.name)).ToList().Count;
                    if (cNullCount == lv1.sub_button.Count)
                    {
                        lv1.sub_button = null;
                        continue;
                    }

                    foreach (var lv2 in lv1.sub_button)
                    {
                        lv2.sub_button = null;
                    }
                }
            }

            var url = string.Format(MENU_SAVE_URL, EntInterface.AccessToken, EntInterface.Conf.AgentId);
            var json = SerializeHelper.JsonSerializeNoNull(menus);
            return WebApiHelper.PostAsync<EntApiResult>(url, json);
        }

        /// <summary>
        /// 读取菜单
        /// </summary>
        /// <returns></returns>
        public static EntMenuResult GetMenu()
        {
            var url = string.Format(MENU_GET_URL, EntInterface.AccessToken, EntInterface.Conf.AgentId);
            return WebApiHelper.GetAsync<EntMenuResult>(url);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        public static EntApiResult DelMenu()
        {
            var url = string.Format(MENU_DEL_URL, EntInterface.AccessToken, EntInterface.Conf.AgentId);
            return WebApiHelper.GetAsync<EntApiResult>(url);
        }

        #endregion
    }

    #region 实体

    /// <summary>
    /// 微信菜单
    /// </summary>
    public class EntMenu
    {
        /// <summary>
        /// type
        /// </summary>
        public string type { set; get; }

        /// <summary>
        /// name
        /// </summary>
        public string name { set; get; }

        /// <summary>
        /// key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// sub_button
        /// </summary>
        public List<EntMenu> sub_button { get; set; }
    }

    /// <summary>
    /// 菜单组
    /// </summary>
    public class EntMenus
    {
        /// <summary>
        /// button
        /// </summary>
        public List<EntMenu> button { get; set; }
    }

    /// <summary>
    /// 自定义菜单
    /// </summary>
    public class EntMenuResult : EntApiResult
    {
        /// <summary>
        /// button
        /// </summary>
        public List<EntMenu> button { get; set; }
    }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum EntMenuType
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
        location_select
    }

    #endregion
}
