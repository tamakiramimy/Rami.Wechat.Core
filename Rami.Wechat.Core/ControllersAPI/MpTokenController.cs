using Rami.Wechat.Core.MiniProgram;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Rami.Wechat.Core.ControllersAPI
{
    /// <summary>
    /// 小程序 Token Api
    /// </summary>
    [Route("api/MpToken")]
    [ApiController]
    public class MpTokenController : ControllerBase
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
        public MpTokenResult Token(string appid, string secret)
        {
            if (appid == MpInterface.Conf.AppId && secret == MpInterface.Conf.AppSecret)
            {
                //检查一次，保证key有效,设置60秒后过时
                var token = new MpTokenResult { access_token = MpInterface.AccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new MpTokenResult { errmsg = "Token：接口验证不通过！" };
        }

        /// <summary>
        /// UpdToken
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        [Route("UpdToken")]
        [HttpGet]
        public MpTokenResult UpdToken(string appid, string secret)
        {
            if (appid == MpInterface.Conf.AppId && secret == MpInterface.Conf.AppSecret)
            {
                MpInterface.UpdToken();
                var token = new MpTokenResult { access_token = MpInterface.AccessToken, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return token;
            }

            return new MpTokenResult { errmsg = "UpdToken：接口验证不通过！" };
        }

        /// <summary>
        /// GetTicket
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getticket")]
        public MpTicketResult GetTicket(string access_token)
        {
            if (access_token == MpInterface.AccessToken)
            {
                var ticket = new MpTicketResult { ticket = MpInterface.JsSdkTicket, Overtime = DateTime.Now, expires_in = ExpiresSec };
                return ticket;
            }

            return new MpTicketResult { errmsg = "GetTicket：验证不通过！" };
        }
    }
}
