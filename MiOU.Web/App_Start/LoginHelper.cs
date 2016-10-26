using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeChat.Adapter;
using System.Configuration;
using Microsoft.AspNet.Identity;
using MiOU.BL;
using MiOU.Entities;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Web.Helper;
using WeChat.Adapter.Authorization;

namespace MiOU.Web
{
    public class LoginHelper
    {
        public static WeChatUserInfo GetLoginInfo(HttpRequestBase Request,HttpSessionStateBase session)
        {
            string mode = ConfigurationManager.AppSettings["mode"];
            WeChatUserInfo info = null;
            AccessToken weChatAccessToken = null;
            if (mode == "debug")
            {
                //info = new UserLoginInfo("WeChat","234233242342342sfaasdfhuwear242304825");
                info = new WeChatUserInfo() { UnionId="12343!!!234234**sdfsdf34533", Name="BoboTest", Country="CN", City="上海",Province="上海", District="闵行区", Gendar=1 };
            }
            else
            {
                string code = Request.QueryString["code"];
                if (string.IsNullOrEmpty(code))
                {
                    weChatAccessToken = (AccessToken)session["wechatAccessToken"];
                }
                else
                {
                    weChatAccessToken = AuthHelper.GetAccessToken(PersistentValueManager.config, code);
                }
                
                if(session["wechatAccessToken"]==null)
                {
                    session["wechatAccessToken"] = weChatAccessToken;
                }
                if (weChatAccessToken == null || string.IsNullOrEmpty(weChatAccessToken.Access_Token) || string.IsNullOrEmpty(weChatAccessToken.OpenId))
                {
                    throw new MiOUException(MessageConstants.WECHAT_AUTH_ERROR);
                }
                //info = new UserLoginInfo("WeChat", weChatAccessToken.OpenId);
                info = AuthHelper.GetUserInfo(PersistentValueManager.config, weChatAccessToken);
            }
            
            return info;
        }
    }
}