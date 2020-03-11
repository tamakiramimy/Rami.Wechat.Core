using Rami.Wechat.Core.Comm;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;

namespace Rami.Wechat.Core.Public
{
    /// <summary>
    /// 微信卡券API
    /// </summary>
    public class PubCardApi
    {
        /// <summary>
        /// 创建卡券
        /// </summary>
        private const string CREATE_CARD = "https://api.weixin.qq.com/card/create?access_token={0}";

        /// <summary>
        /// 获取卡券列表
        /// </summary>
        private const string GET_CARD_IDS = "https://api.weixin.qq.com/card/batchget?access_token={0}";

        /// <summary>
        /// 查看卡券详情
        /// </summary>
        private const string GET_CARD_DETAIL = "https://api.weixin.qq.com/card/get?access_token={0}";

        #region 创建卡券

        /// <summary>
        /// 创建卡券入口
        /// </summary>
        /// <param name="cardType"></param>
        /// <param name="deal_detail"></param>
        /// <param name="least_cost"></param>
        /// <param name="reduce_cost"></param>
        /// <param name="discount"></param>
        /// <param name="gift"></param>
        /// <param name="default_detail"></param>
        /// <param name="logo_url"></param>
        /// <param name="code_type"></param>
        /// <param name="brand_name"></param>
        /// <param name="title"></param>
        /// <param name="sub_title"></param>
        /// <param name="color"></param>
        /// <param name="notice"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <param name="date_info"></param>
        /// <returns></returns>
        public static PubCardCreateResult CreateCard(PubCardType cardType, string deal_detail, int? least_cost, int? reduce_cost, int? discount, string gift, string default_detail,
            string logo_url, PubCardCodeType code_type, string brand_name, string title, string sub_title, PubCardColorType color, string notice, string description, int quantity, PubCardDateInfo date_info)
        {
            // 基本卡券信息
            PubCardBaseInfo baseInfo = CreateBaseInfoFunc(logo_url, code_type, brand_name, title, sub_title, color, notice, description, quantity);
            // 时间
            PubCardDateInfo dateInfo = CreateDateInfoFunc(date_info);
            baseInfo.date_info = dateInfo;

            // 组装
            dynamic cardInfo = new ExpandoObject();
            cardInfo.card_type = cardType.ToString();
            switch (cardType)
            {
                case PubCardType.GROUPON:
                    cardInfo.groupon = new
                    {
                        base_info = baseInfo,
                        deal_detail = deal_detail
                    };
                    break;
                case PubCardType.CASH:
                    cardInfo.cash = new
                    {
                        base_info = baseInfo,
                        least_cost = least_cost,
                        reduce_cost = reduce_cost
                    };
                    break;
                case PubCardType.DISCOUNT:
                    cardInfo.discount = new
                    {
                        base_info = baseInfo,
                        discount = discount
                    };
                    break;
                case PubCardType.GIFT:
                    cardInfo.gift = new
                    {
                        base_info = baseInfo,
                        gift = gift
                    };
                    break;
                case PubCardType.GENERAL_COUPON:
                    cardInfo.general_coupon = new
                    {
                        base_info = baseInfo,
                        default_detail = default_detail
                    };
                    break;
                default:
                    break;
            }

            // 创建卡券
            var card = new { card = cardInfo };
            var res = CreateCardBase(card);
            return res;
        }

        /// <summary>
        /// 创建卡券基础信息
        /// </summary>
        /// <param name="logo_url"></param>
        /// <param name="code_type"></param>
        /// <param name="brand_name"></param>
        /// <param name="title"></param>
        /// <param name="sub_title"></param>
        /// <param name="color"></param>
        /// <param name="notice"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private static PubCardBaseInfo CreateBaseInfoFunc(string logo_url, PubCardCodeType code_type, string brand_name, string title, string sub_title, PubCardColorType color, string notice, string description, int quantity)
        {
            var baseInfo = new PubCardBaseInfo();
            baseInfo.logo_url = logo_url;
            baseInfo.code_type = code_type.ToString();
            baseInfo.brand_name = brand_name;
            baseInfo.title = title;
            baseInfo.sub_title = sub_title;
            baseInfo.color = color.ToString();
            baseInfo.notice = notice;
            baseInfo.description = description;
            baseInfo.sku = new PubCardSkuInfo() { quantity = quantity };
            return baseInfo;
        }

        /// <summary>
        /// 创建卡券时间字段
        /// </summary>
        /// <param name="date_info"></param>
        /// <returns></returns>
        private static PubCardDateInfo CreateDateInfoFunc(PubCardDateInfo date_info)
        {
            var dateInfo = new PubCardDateInfo();
            if (date_info.type == PubCardDateType.DATE_TYPE_FIX_TIME_RANGE.ToString())
            {
                dateInfo.type = PubCardDateType.DATE_TYPE_FIX_TIME_RANGE.ToString();
                dateInfo.begin_timestamp = date_info.begin_timestamp;
                dateInfo.end_timestamp = date_info.end_timestamp;
            }
            else if (date_info.type == PubCardDateType.DATE_TYPE_FIX_TERM.ToString())
            {
                dateInfo.type = PubCardDateType.DATE_TYPE_FIX_TERM.ToString();
                dateInfo.fixed_begin_term = date_info.fixed_begin_term;
                dateInfo.fixed_term = date_info.fixed_term;
            }

            return dateInfo;
        }

        /// <summary>
        /// 创建卡券
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public static PubCardCreateResult CreateCardBase(dynamic cardInfo)
        {
            var url = string.Format(CREATE_CARD, PubInterface.AccessToken);
            // 去除无效的字段
            var jsonData = SerializeHelper.JsonSerializeNoNull(cardInfo);
            return WebApiHelper.PostAsync<PubCardCreateResult>(url, jsonData);
        }

        #endregion

        /// <summary>
        /// 查询卡券Ids
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="status_list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static PubCardIdsResult GetCardIds(int offset, List<PubCardStateType> status_list, int count = 50)
        {
            var url = string.Format(GET_CARD_IDS, PubInterface.AccessToken);
            var jsonData = new { offset = offset, status_list = status_list.Select(x => x.ToString()), count = count };
            return WebApiHelper.PostAsync<PubCardIdsResult>(url, jsonData);
        }

        /// <summary>
        /// 获取卡券详情
        /// </summary>
        /// <param name="card_id"></param>
        /// <returns></returns>
        public static PubCardDetailResult GetCardDetail(string card_id)
        {
            var url = string.Format(GET_CARD_DETAIL, PubInterface.AccessToken);
            var jsonData = new { card_id = card_id };
            var postData = new StringContent(SerializeHelper.JsonSerialize(jsonData));
            return WebApiHelper.PostAsync<PubCardDetailResult>(url, postData);
        }
    }

    /// <summary>
    /// 卡券类型
    /// </summary>
    public enum PubCardType
    {
        /// <summary>
        /// 团购券
        /// </summary>
        GROUPON,
        /// <summary>
        /// 代金券
        /// </summary>
        CASH,
        /// <summary>
        /// 折扣券
        /// </summary>
        DISCOUNT,
        /// <summary>
        /// 兑换券
        /// </summary>
        GIFT,
        /// <summary>
        /// 优惠券
        /// </summary>
        GENERAL_COUPON,
    }

    /// <summary>
    /// Code展示类型
    /// </summary>
    public enum PubCardCodeType
    {
        /// <summary>
        /// 文本
        /// </summary>
        CODE_TYPE_TEXT,
        /// <summary>
        /// 一维码
        /// </summary>
        CODE_TYPE_BARCODE,
        /// <summary>
        /// 二维码
        /// </summary>
        CODE_TYPE_QRCODE,
        /// <summary>
        /// 二维码无code显示
        /// </summary>
        CODE_TYPE_ONLY_QRCODE,
        /// <summary>
        /// 一维码无code显示
        /// </summary>
        CODE_TYPE_ONLY_BARCODE,
        /// <summary>
        /// 不显示code和条形码类型，须开发者传入"立即使用"自定义cell完成线上券核销
        /// </summary>
        CODE_TYPE_NONE,
    }

    /// <summary>
    /// 卡券背景颜色
    /// </summary>
    public enum PubCardColorType
    {
        /// <summary>
        /// #63b359
        /// </summary>
        Color010,
        /// <summary>
        /// #2c9f67
        /// </summary>
        Color020,
        /// <summary>
        /// #509fc9
        /// </summary>
        Color030,
        /// <summary>
        /// #5885cf
        /// </summary>
        Color040,
        /// <summary>
        /// #9062c0
        /// </summary>
        Color050,
        /// <summary>
        /// #d09a45
        /// </summary>
        Color060,
        /// <summary>
        /// #e4b138
        /// </summary>
        Color070,
        /// <summary>
        /// #ee903c
        /// </summary>
        Color080,
        /// <summary>
        /// #f08500
        /// </summary>
        Color081,
        /// <summary>
        /// #a9d92d
        /// </summary>
        Color082,
        /// <summary>
        /// #dd6549
        /// </summary>
        Color090,
        /// <summary>
        /// #cc463d
        /// </summary>
        Color100,
        /// <summary>
        /// #cf3e36
        /// </summary>
        Color101,
        /// <summary>
        /// #5E6671
        /// </summary>
        Color102,
    }

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum PubCardDateType
    {
        /// <summary>
        /// 表示固定日期区间
        /// </summary>
        DATE_TYPE_FIX_TIME_RANGE,
        /// <summary>
        /// 表示固定时长（自领取后按天算)
        /// </summary>
        DATE_TYPE_FIX_TERM,
    }

    /// <summary>
    /// 卡券状态枚举
    /// </summary>
    public enum PubCardStateType
    {
        /// <summary>
        /// 验证通过
        /// </summary>
        CARD_STATUS_VERIFY_OK,
        /// <summary>
        /// 验证未通过
        /// </summary>
        CARD_STATUS_DISPATCH,
    }

    /// <summary>
    /// 卡券模型
    /// </summary>
    public class PubCardBase
    {
        /// <summary>
        /// 卡券类型(CardType枚举值)
        /// </summary>
        public string card_type { get; set; }
        /// <summary>
        /// 基本的卡券数据，见下表，所有卡券类型通用。
        /// </summary>
        public PubCardBaseInfo base_info { get; set; }
        /// <summary>
        /// 团购券专用，团购详情。
        /// </summary>
        public string deal_detail { get; set; }
        /// <summary>
        /// 代金券专用，表示起用金额（单位为分）,如果无起用门槛则填0。
        /// </summary>
        public int? least_cost { get; set; }
        /// <summary>
        /// 代金券专用，表示减免金额。（单位为分）
        /// </summary>
        public int? reduce_cost { get; set; }
        /// <summary>
        /// 折扣券专用，表示打折额度（百分比）。填30就是七折。
        /// </summary>
        public int? discount { get; set; }
        /// <summary>
        /// 兑换券专用，填写兑换内容的名称。
        /// </summary>
        public string gift { get; set; }
        /// <summary>
        /// 优惠券专用，填写优惠详情。
        /// </summary>
        public string default_detail { get; set; }
    }

    /// <summary>
    /// 基本的卡券数据
    /// </summary>
    public class PubCardBaseInfo
    {
        /// <summary>
        /// 卡券的商户logo，建议像素为300*300。
        /// </summary>
        public string logo_url { get; set; }
        /// <summary>
        /// Code展示类型(CardCodeType枚举值)
        /// </summary>
        public string code_type { get; set; }
        /// <summary>
        /// 商户名字,字数上限为12个汉字。
        /// </summary>
        public string brand_name { get; set; }
        /// <summary>
        /// 卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// （非必填）券名，字数上限为18个汉字。
        /// </summary>
        public string sub_title { get; set; }
        /// <summary>
        /// 券颜色。按色彩规范标注填写Color010-Color100
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 卡券使用提醒，字数上限为16个汉字。
        /// </summary>
        public string notice { get; set; }
        /// <summary>
        /// 卡券使用说明，字数上限为1024个汉字。
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 商品信息。
        /// </summary>
        public PubCardSkuInfo sku { get; set; }
        /// <summary>
        /// 使用日期，有效期的信息
        /// </summary>
        public PubCardDateInfo date_info { get; set; }
    }

    /// <summary>
    /// 商品信息。
    /// </summary>
    public class PubCardSkuInfo
    {
        /// <summary>
        /// 卡券库存的数量，上限为100000000
        /// </summary>
        public int quantity { get; set; }
    }

    /// <summary>
    /// 使用日期，有效期的信息
    /// </summary>
    public class PubCardDateInfo
    {
        /// <summary>
        /// 使用时间的类型，旧文档采用的1和2依然生效。
        /// DATE_TYPE_FIX_TIME_RANGE 表示固定日期区间，DATE_TYPE_FIX_TERM表示固定时长（自领取后按天算。
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// type为DATE_TYPE_FIX_TIME_RANGE时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）
        /// </summary>
        public int? begin_timestamp { get; set; }
        /// <summary>
        /// type为DATE_TYPE_FIX_TIME_RANGE时，表示卡券统一的结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）
        /// </summary>
        public int? end_timestamp { get; set; }
        /// <summary>
        /// type为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天内有效，不支持填写0。
        /// </summary>
        public int? fixed_term { get; set; }
        /// <summary>
        /// type为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天开始生效，领取后当天生效填写0。（单位为天）
        /// </summary>
        public int? fixed_begin_term { get; set; }
    }

    /// <summary>
    /// 创建卡券返回结果
    /// </summary>
    public class PubCardCreateResult : PubApiResult
    {
        /// <summary>
        /// 卡券ID。
        /// </summary>
        public string card_id { get; set; }
    }

    /// <summary>
    /// 查询卡券Id列表
    /// </summary>
    public class PubCardIdsResult : PubApiResult
    {
        /// <summary>
        /// 卡券ID列表
        /// </summary>
        public List<string> card_id_list { get; set; }
        /// <summary>
        /// 该商户名下卡券ID总数
        /// </summary>
        public int total_num { get; set; }
    }

    /// <summary>
    /// 卡券详情结果
    /// </summary>
    public class PubCardDetailResult : PubApiResult
    {
        /// <summary>
        /// 卡券对象
        /// </summary>
        public PubCardInfo card { get; set; }
    }

    /// <summary>
    /// 卡券详情
    /// </summary>
    public class PubCardInfo
    {
        /// <summary>
        /// 卡券类型
        /// </summary>
        public string card_type { get; set; }
        /// <summary>
        /// 卡券详情主体
        /// </summary>
        public dynamic cardDatas { get; set; }
    }
}
