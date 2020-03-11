using Rami.Wechat.Core.Enterprise;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Rami.Wechat.Core.ControllersAPI
{
    /// <summary>
    /// 企业号 Token Api
    /// </summary>
    [Route("api/QyToken")]
    [ApiController]
    public class QyTokenController : ControllerBase
    {
        /// <summary>
        /// 超时秒数(需要大于60)
        /// </summary>
        private const int ExpiresSec = 120;

        /// <summary>
        /// Hello
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Hello")]
        public string Hello()
        {
            return "Hello";
        }

        /// <summary>
        /// GetToken
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettoken")]
        public EntTokenResult GetToken(string corpid, string corpsecret)
        {
            if (corpid == EntInterface.Conf.AppId && corpsecret == EntInterface.Conf.AppSecret)
            {
                //检查一次，保证key有效,设置60秒后过时
                var token = new EntTokenResult { access_token = EntInterface.AccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new EntTokenResult { errmsg = "GetToken：接口验证不通过！" };
        }

        /// <summary>
        /// UpdToken
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        [Route("UpdToken")]
        [HttpGet]
        public EntTokenResult UpdToken(string corpid, string corpsecret)
        {
            if (corpid == EntInterface.Conf.AppId && corpsecret == EntInterface.Conf.AppSecret)
            {
                EntInterface.UpdToken();
                var token = new EntTokenResult { access_token = EntInterface.AccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new EntTokenResult { errmsg = "UpdToken：接口验证不通过！" };
        }

        /// <summary>
        /// GetJsapiTicket
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get_jsapi_ticket")]
        public EntTicketResult GetJsapiTicket(string access_token)
        {
            if (access_token == EntInterface.AccessToken)
            {
                var ticket = new EntTicketResult { ticket = EntInterface.JsSdkTicket, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return ticket;
            }

            return new EntTicketResult { errmsg = "GetTicket：验证不通过！" };
        }

        /// <summary>
        /// GetOutAccess
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getoutaccesstoken")]
        public EntTokenResult GetOutAccessToken(string corpid, string corpsecret)
        {
            if (corpid == EntInterface.Conf.AppId && corpsecret == EntInterface.Conf.AppSecret)
            {
                //检查一次，保证key有效,设置60秒后过时
                var token = new EntTokenResult { access_token = EntInterface.OutAccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new EntTokenResult { errmsg = "GetOutAccessToken：验证不通过！" };
        }
    }
}
