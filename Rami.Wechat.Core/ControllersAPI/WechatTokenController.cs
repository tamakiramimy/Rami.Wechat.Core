using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Rami.Wechat.Core.ControllersAPI
{
    /// <summary>
    /// 公众号 Token Api
    /// </summary>
    [Route("api/WechatToken")]
    [ApiController]
    public class WechatTokenController : ControllerBase
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
        /// Token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("token")]
        public PubTokenResult Token(string appid, string secret)
        {
            if (appid == PubInterface.Conf.AppId && secret == PubInterface.Conf.AppSecret)
            {
                //检查一次，保证key有效,设置60秒后过时
                var token = new PubTokenResult { access_token = PubInterface.AccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new PubTokenResult { errmsg = "GetToken：接口验证不通过！" };
        }

        /// <summary>
        /// UpdToken
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        [Route("UpdToken")]
        [HttpGet]
        public PubTokenResult UpdToken(string appid, string secret)
        {
            if (appid == PubInterface.Conf.AppId && secret == PubInterface.Conf.AppSecret)
            {
                PubInterface.UpdToken();
                var token = new PubTokenResult { access_token = PubInterface.AccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new PubTokenResult { errmsg = "UpdToken：接口验证不通过！" };
        }

        /// <summary>
        /// GetTicket
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getticket")]
        public PubTicketResult GetTicket(string access_token)
        {
            if (access_token == PubInterface.AccessToken)
            {
                var ticket = new PubTicketResult { ticket = PubInterface.JsSdkTicket, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return ticket;
            }

            return new PubTicketResult { errmsg = "GetTicket：验证不通过！" };
        }
    }
}
