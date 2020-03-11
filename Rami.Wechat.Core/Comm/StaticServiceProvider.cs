using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.Wechat.Core.Comm
{
    /// <summary>
    /// 静态IServiceProvider
    /// </summary>
    internal class StaticServiceProvider
    {
        /// <summary>
        /// 服务提供
        /// </summary>
        public static IServiceProvider Current;

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Configure(IServiceProvider serviceProvider)
        {
            Current = serviceProvider;
        }
    }

    /// <summary>
    /// 静态IServiceProvider
    /// </summary>
    public static class StaticServiceProviderExtensions
    {
        /// <summary>
        /// IApplicationBuilder 注入 IServiceProvider
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWxServiceProvider(this IApplicationBuilder app)
        {
            IServiceProvider applicationServices = app.ApplicationServices;
            StaticServiceProvider.Configure(applicationServices);
            return app;
        }
    }
}
