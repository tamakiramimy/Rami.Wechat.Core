﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Rami.Wechat.Core.Public.Tencent
{
    /// <summary>
    /// WXBizMsgCrypt
    /// </summary>
    public class WXBizMsgCrypt
    {
        /// <summary>
        /// AppId
        /// </summary>
        string m_sAppID;

        /// <summary>
        /// Token
        /// </summary>
        string m_sToken;

        /// <summary>
        /// Aes Key
        /// </summary>
        string m_sEncodingAESKey;

        /// <summary>
        /// 错误定义
        /// </summary>
        enum WXBizMsgCryptErrorCode
        {
            WXBizMsgCrypt_OK = 0,
            /// <summary>
            /// 签名验证错误
            /// </summary>
            WXBizMsgCrypt_ValidateSignature_Error = -40001,
            /// <summary>
            /// xml解析失败
            /// </summary>
            WXBizMsgCrypt_ParseXml_Error = -40002,
            /// <summary>
            /// sha加密生成签名失败
            /// </summary>
            WXBizMsgCrypt_ComputeSignature_Error = -40003,
            /// <summary>
            /// AESKey 非法
            /// </summary>
            WXBizMsgCrypt_IllegalAesKey = -40004,
            /// <summary>
            /// appid 校验错误
            /// </summary>
            WXBizMsgCrypt_ValidateAppid_Error = -40005,
            /// <summary>
            /// AES 加密失败
            /// </summary>
            WXBizMsgCrypt_EncryptAES_Error = -40006,
            /// <summary>
            /// AES 解密失败
            /// </summary>
            WXBizMsgCrypt_DecryptAES_Error = -40007,
            /// <summary>
            /// 解密后得到的buffer非法
            /// </summary>
            WXBizMsgCrypt_IllegalBuffer = -40008,
            /// <summary>
            /// base64加密异常
            /// </summary>
            WXBizMsgCrypt_EncodeBase64_Error = -40009,
            /// <summary>
            /// base64解密异常
            /// </summary>
            WXBizMsgCrypt_DecodeBase64_Error = -40010
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sToken">公众平台上，开发者设置的Token</param>
        /// <param name="sEncodingAESKey">公众平台上，开发者设置的EncodingAESKey</param>
        /// <param name="sAppID">公众帐号的appid</param>
        public WXBizMsgCrypt(string sToken, string sEncodingAESKey, string sAppID)
        {
            m_sToken = sToken;
            m_sAppID = sAppID;
            m_sEncodingAESKey = sEncodingAESKey;
        }

        /// <summary>
        /// 检验消息的真实性，并且获取解密后的明文
        /// </summary>
        /// <param name="sMsgSignature">签名串，对应URL参数的msg_signature</param>
        /// <param name="sTimeStamp">时间戳，对应URL参数的timestamp</param>
        /// <param name="sNonce">随机串，对应URL参数的nonce</param>
        /// <param name="sPostData">密文，对应POST请求的数据</param>
        /// <param name="sMsg">解密后的原文，当return返回0时有效</param>
        /// <returns>成功0，失败返回对应的错误码</returns>
        public int DecryptMsg(string sMsgSignature, string sTimeStamp, string sNonce, string sPostData, ref string sMsg)
        {
            if (m_sEncodingAESKey.Length != 43)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_IllegalAesKey;
            }

            XmlDocument doc = new XmlDocument();
            XmlNode root;
            string sEncryptMsg;
            try
            {
                doc.LoadXml(sPostData);
                root = doc.FirstChild;
                sEncryptMsg = root["Encrypt"].InnerText;
            }
            catch (Exception)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_ParseXml_Error;
            }

            // 验证签名
            int ret = 0;
            ret = VerifySignature(m_sToken, sTimeStamp, sNonce, sEncryptMsg, sMsgSignature);
            if (ret != 0)
            {
                return ret;
            }

            // 解密消息
            string cpid = "";
            try
            {
                sMsg = Cryptography.AES_decrypt(sEncryptMsg, m_sEncodingAESKey, ref cpid);
            }
            catch (FormatException)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_DecodeBase64_Error;
            }
            catch (Exception)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_DecryptAES_Error;
            }

            if (cpid != m_sAppID)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_ValidateAppid_Error;
            }

            return 0;
        }

        /// <summary>
        /// 将回复用户的消息加密打包
        /// </summary>
        /// <param name="sReplyMsg">企业号待回复用户的消息，xml格式的字符串</param>
        /// <param name="sTimeStamp">时间戳，可以自己生成，也可以用URL参数的timestamp</param>
        /// <param name="sNonce">随机串，可以自己生成，也可以用URL参数的nonce</param>
        /// <param name="sEncryptMsg">加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串,当return返回0时有效</param>
        /// <returns>成功0，失败返回对应的错误码</returns>
        public int EncryptMsg(string sReplyMsg, string sTimeStamp, string sNonce, ref string sEncryptMsg)
        {
            if (m_sEncodingAESKey.Length != 43)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_IllegalAesKey;
            }

            string raw = "";
            try
            {
                raw = Cryptography.AES_encrypt(sReplyMsg, m_sEncodingAESKey, m_sAppID);
            }
            catch (Exception)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_EncryptAES_Error;
            }

            string MsgSigature = "";
            int ret = 0;
            ret = GenarateSinature(m_sToken, sTimeStamp, sNonce, raw, ref MsgSigature);
            if (0 != ret)
            {
                return ret;
            }
                
            sEncryptMsg = "";
            string EncryptLabelHead = "<Encrypt><![CDATA[";
            string EncryptLabelTail = "]]></Encrypt>";
            string MsgSigLabelHead = "<MsgSignature><![CDATA[";
            string MsgSigLabelTail = "]]></MsgSignature>";
            string TimeStampLabelHead = "<TimeStamp><![CDATA[";
            string TimeStampLabelTail = "]]></TimeStamp>";
            string NonceLabelHead = "<Nonce><![CDATA[";
            string NonceLabelTail = "]]></Nonce>";
            sEncryptMsg = sEncryptMsg + "<xml>" + EncryptLabelHead + raw + EncryptLabelTail;
            sEncryptMsg = sEncryptMsg + MsgSigLabelHead + MsgSigature + MsgSigLabelTail;
            sEncryptMsg = sEncryptMsg + TimeStampLabelHead + sTimeStamp + TimeStampLabelTail;
            sEncryptMsg = sEncryptMsg + NonceLabelHead + sNonce + NonceLabelTail;
            sEncryptMsg += "</xml>";

            return 0;
        }

        /// <summary>
        /// 字典排序
        /// </summary>
        public class DictionarySort : System.Collections.IComparer
        {
            /// <summary>
            /// 字典对比实现
            /// </summary>
            /// <param name="oLeft"></param>
            /// <param name="oRight"></param>
            /// <returns></returns>
            public int Compare(object oLeft, object oRight)
            {
                string sLeft = oLeft as string;
                string sRight = oRight as string;
                int iLeftLength = sLeft.Length;
                int iRightLength = sRight.Length;
                int index = 0;
                while (index < iLeftLength && index < iRightLength)
                {
                    if (sLeft[index] < sRight[index])
                        return -1;
                    else if (sLeft[index] > sRight[index])
                        return 1;
                    else
                        index++;
                }

                return iLeftLength - iRightLength;
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="sToken"></param>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="sMsgEncrypt"></param>
        /// <param name="sSigture"></param>
        /// <returns></returns>
        public static int VerifySignature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, string sSigture)
        {
            string hash = "";
            int ret = 0;
            ret = GenarateSinature(sToken, sTimeStamp, sNonce, sMsgEncrypt, ref hash);
            if (ret != 0)
            {
                return ret;
            }

            if (hash == sSigture)
            {
                return 0;
            }
            else
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_ValidateSignature_Error;
            }
        }

        /// <summary>
        /// 数据签名
        /// </summary>
        /// <param name="sToken"></param>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="sMsgEncrypt"></param>
        /// <param name="sMsgSignature"></param>
        /// <returns></returns>
        public static int GenarateSinature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, ref string sMsgSignature)
        {
            ArrayList AL = new ArrayList();
            AL.Add(sToken);
            AL.Add(sTimeStamp);
            AL.Add(sNonce);
            AL.Add(sMsgEncrypt);
            AL.Sort(new DictionarySort());
            string raw = "";
            for (int i = 0; i < AL.Count; ++i)
            {
                raw += AL[i];
            }

            SHA1 sha;
            ASCIIEncoding enc;
            string hash = "";
            try
            {
                sha = new SHA1CryptoServiceProvider();
                enc = new ASCIIEncoding();
                byte[] dataToHash = enc.GetBytes(raw);
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();
            }
            catch (Exception)
            {
                return (int)WXBizMsgCryptErrorCode.WXBizMsgCrypt_ComputeSignature_Error;
            }

            sMsgSignature = hash;
            return 0;
        }
    }
}
