using Rami.Wechat.Core.Comm;

namespace Rami.Wechat.Core.Merchant
{
    /// <summary>
    /// 获取支付配置
    /// </summary>
    public class McPayConf
    {
        /// <summary>
        /// 商户号支付配置
        /// </summary>
        private static MerchantConfs Confs { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        static McPayConf()
        {
            Confs = WechatConfigHelper.WechatConf.Merchants;
        }

        /// <summary>
        /// 公众号支付配置
        /// </summary>
        public static MerchantConf PubPayConf
        {
            get
            {
                return Confs.Public;
            }
        }

        /// <summary>
        /// 企业号支付配置
        /// </summary>
        public static MerchantConf EntPayConf
        {
            get
            {
                return Confs.Enterprise;
            }
        }

        /// <summary>
        /// 小程序支付配置
        /// </summary>
        public static MerchantConf MpPayConf
        {
            get
            {
                return Confs.MiniProgram;
            }
        }

        /// <summary>
        /// 获取支付配置
        /// </summary>
        /// <returns></returns>
        public static MerchantConf GetPayConf(PayConfType payType)
        {
            switch (payType)
            {
                case PayConfType.Public:
                    return PubPayConf;
                case PayConfType.Enterprise:
                    return EntPayConf;
                case PayConfType.MiniProgram:
                    return MpPayConf;
                default:
                    return PubPayConf;
            }
        }
    }
}
