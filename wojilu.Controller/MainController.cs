/*
 * Copyright (c) 2010, www.wojilu.com. All rights reserved.
 */

using System;

using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Common;

using wojilu.Members.Users.Service;
using wojilu.Members.Users.Interface;
using wojilu.Members.Users.Domain;

using wojilu.Web.Controller.Common;
using wojilu.Web.Controller.Users.Admin;
using System.IO;
using System.Collections.Generic;
using System.Text;
using wojilu.Caching;
using wojilu.DI;
using wojilu.Common.Spider.Service;
using wojilu.Common.Spider.Domain;
using wojilu.Web.Utils;
using System.Threading;
using System.Collections;
using wojilu.Web.Context;
using wojilu.Apps.Blog.Domain;
using wojilu.Web.Mvc.Utils;
using wojilu.Web.Mvc.Routes;
using wojilu.Apps.Reader.Domain;
using wojilu.ORM;
using wojilu.Apps.Forum.Domain;
using wojilu.Members.Sites.Domain;
using wojilu.Common.Onlines;
using wojilu.Config;
using wojilu.Common.AppBase;
using System.Text.RegularExpressions;
using System.Data;
using System.Net;
using NReadability;

namespace wojilu.Web.Controller {

    public class MainController : ControllerBase {

        private static readonly ILog logger = LogManager.GetLogger( typeof( MainController ) );

        public IUserService userService { get; set; }
        public IUserConfirmService confirmService { get; set; }
        public ILoginService loginService { get; set; }
        public IFriendService friendService { get; set; }
        public IInviteService inviteService { get; set; }

        public MainController() {

            confirmService = new UserConfirmService();
            userService = new UserService();
            loginService = new LoginService();
            friendService = new FriendService();
            inviteService = new InviteService();

            HidePermission( typeof( SecurityController ) );

        }

        public void Index() {
            redirect( new SiteInitController().Index );
        }

        public void AddArticle()
        {


            int userId = ctx.web.UserId();
            if(userId == -1)
            {
                echoJson("{\"success\":true,\"message\":\"���ȵ�¼\"}");
                return;
            }
            string strUrl = ctx.Post("url");
            //string strTitle = ctx.Post("title");

            savePageDetail(strUrl, userId);
            echoJson("{\"success\":true,\"message\":\"��ӳɹ�\"}");
     
        }
        private static void savePageDetail(string strUrl, int nUserID)
        {

            string content;

            using (var wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                content = wc.DownloadString(strUrl);
            }


            var readability = Readability.Create(content);
          //  Console.WriteLine(readability.Title);
          //  Console.WriteLine(readability.Content);

            string pageBody = readability.Content;
            pageBody = Regex.Replace(pageBody, "font-size", "", RegexOptions.IgnoreCase);
            string strArcitleLink = "<div class=\"ArcitleLink\"><a href=" + strUrl + ">ԭ������</a></div>";
            pageBody = pageBody + strArcitleLink;

            Maticsoft.BLL.BlogCategory bllBlogCategory = new Maticsoft.BLL.BlogCategory();
            DataSet ds = bllBlogCategory.GetList("AppId = '" + nUserID.ToString() + "'");
            int nCateID = 1;
            if (ds.Tables[0].Rows.Count > 0)
            {
                nCateID = (int)ds.Tables[0].Rows[0]["Id"];
            }


            User user = User.findById(nUserID);
            Maticsoft.Model.BlogPost data = new Maticsoft.Model.BlogPost();


            data.CategoryId = 14;
            data.Title = readability.Title;
            data.Abstract = "";
            data.Content = pageBody;
            data.AccessStatus = 0;
            data.CommentCondition = 0;
            data.SaveStatus = 1;//�ݸ�
            data.Created = System.DateTime.Now.Date;
            data.IsTop = 0;
            data.IsPick = 0;
            data.IsPic = 0;
            data.Ip = "";
            data.OwnerId = nUserID;
            data.OwnerUrl = user.Url;
            data.OwnerType = "wojilu.Members.Users.Domain.User";
            data.CreatorUrl = user.Url;
            data.AppId = nUserID ;
            data.CreatorId = nUserID;
            data.SaveStatus = 0;
            Maticsoft.BLL.BlogPost bll = new Maticsoft.BLL.BlogPost();
            bll.Add(data);

           

        }
        //������ݿ����Ƿ��Ѿ����ڴ����ݣ�
        //private static bool isPageExist(string pageUrl, StringBuilder sb)
        //{
        //    bool isExist = false;
        //    List<SpiderArticle> list = SpiderArticle.find("Url=:url and IsDelete=0").set("url", pageUrl).list();
        //    if (list.Count > 0)
        //    {
        //        logger.Info("pass..." + pageUrl);
        //        sb.AppendLine("pass..." + pageUrl);
        //        isExist = true;
        //    }
        //    return isExist;
        //}
        //private static void savePageDetail(DetailLink lnk, StringBuilder sb)
        //{



        //    SpiderTemplate template = lnk.Template;
        //    string url = lnk.Url;
        //    string title = lnk.Title;
        //    string summary = lnk.Abstract;

        //    if (isPageExist(url, sb)) return;

        //    String pageBody = new PagedDetailSpider().GetContent(url, template, sb);


        //    if (pageBody == null) return;

        //    SpiderArticle pd = new SpiderArticle();
        //    pd.Title = title;
        //    pd.Url = strUtil.SubString(url, 250);
        //    pd.Abstract = summary;
        //    pd.Body = pageBody;
        //    pd.SpiderTemplate = template;

        //    MatchCollection matchs = Regex.Matches(pageBody, RegPattern.Img, RegexOptions.Singleline);
        //    if (matchs.Count > 0)
        //    {
        //        pd.IsPic = 1;
        //        pd.PicUrl = matchs[0].Groups[1].Value;
        //    }

        //    pd.insert();

        //    sb.AppendLine("����ɹ�..." + lnk.Title + "_" + lnk.Url);


        //    pageBody = Regex.Replace(pageBody, "font-size", "", RegexOptions.IgnoreCase);
        //    string strArcitleLink = "<div class=\"ArcitleLink\"><a href=" + pd.Url + ">ԭ������</a></div>";
        //    pageBody = pageBody + strArcitleLink;

        //    Maticsoft.BLL.BlogCategory bllBlogCategory = new Maticsoft.BLL.BlogCategory();
        //    DataSet ds = bllBlogCategory.GetList("AppId = '" + template.IsDelete.ToString() + "'");
        //    int nCateID = 1;
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        nCateID = (int)ds.Tables[0].Rows[0]["Id"];
        //    }





        //    Maticsoft.Model.BlogPost data = new Maticsoft.Model.BlogPost();


        //    data.CategoryId = 1;
        //    data.Title = title;
        //    data.Abstract = summary;
        //    data.Content = pageBody;
        //    data.AccessStatus = 0;
        //    data.CommentCondition = 0;
        //    data.SaveStatus = 1;//�ݸ�
        //    data.Created = System.DateTime.Now.Date;
        //    data.IsTop = 0;
        //    data.IsPick = 0;
        //    data.IsPic = 0;
        //    data.Ip = "";
        //    data.OwnerId = template.IsDelete;
        //    data.OwnerUrl = template.SiteName;
        //    data.OwnerType = "wojilu.Members.Users.Domain.User";
        //    data.CreatorUrl = template.SiteName;
        //    data.AppId = template.IsDelete; ;
        //    data.CreatorId = template.IsDelete;
        //    Maticsoft.BLL.BlogPost bll = new Maticsoft.BLL.BlogPost();
        //    bll.Add(data);

        //}
        public void Login() {


            Page.Title = lang( "userLogin" );
            target( CheckLogin );

            set( "returnUrl", getReturnUrl() );

            String lblUnRegisterTip = string.Format( lang( "unRegisterTip" ), to( new RegisterController().Register ) );
            set( "lblUnRegisterTip", lblUnRegisterTip );

            set( "resetPwdLink", to( new ResetPwdController().StepOne ) );

            setLoginValidationCode();
        }

        [HttpPost, DbTransaction]
        public void Logout() {
            ctx.web.UserLogout();
            OnlineStats.Instance.SubtractMemberCount();
            echoRedirect( lang( "logoutok" ), ctx.url.SiteAndAppPath );
        }

        [HttpPost, DbTransaction]
        public void CheckLogin() {

            if (ctx.viewer.IsLogin) {
                echo( "�����ʺţ������Ѿ���¼" );
                return;
            }



            if (config.Instance.Site.LoginNeedImgValidation) {

                if (Html.Captcha.CheckError( ctx )) {
                    run( Login );
                    return;
                }
            }

            String user = ctx.Post( "txtUid" );
            String pwd = ctx.Post( "txtPwd" );

            if (strUtil.IsNullOrEmpty( user )) errors.Add( lang( "exUserName" ) );
            if (strUtil.IsNullOrEmpty( pwd )) errors.Add( lang( "exPwd" ) );
            if (errors.HasErrors) { run( Login ); return; }

            User member = userService.IsNameEmailPwdCorrect( user, pwd );
            if (member == null) {
                errors.Add( lang( "exUserNamePwdError" ) );
                run( Login );
                return;
            }



            if (userService.IsUserDeleted( member )) {
                errors.Add( lang( "exUsere" ) );
                run( Login );
                return;
            }

            if (config.Instance.Site.UserNeedApprove && member.Status == MemberStatus.Approving) {
                errors.Add( "�����˺���δ������ˣ������ĵȺ�" );
                run( Login );
                return;
            }

            // ��Ҫ������ܵ�¼
            if ( member.IsEmailConfirmed==0 && config.Instance.Site.LoginType == LoginType.ActivationEmail ) {
                ActivationController.AllowSendActivationEmail( ctx, member.Id );
                redirect( new ActivationController().SendEmailButton );
                return;
            }

            LoginTime expiration;
            if (ctx.PostIsCheck( "RememberMe" ) == 1)
                expiration = LoginTime.Forever;
            else
                expiration = LoginTime.Never;

            loginService.Login( member, expiration, ctx.Ip, ctx );

            //echoToParent( lang( "loginok" ), getSavedReturnUrl() );
            echoRedirect( lang( "loginok" ), getSavedReturnUrl() );
        }


        public void ConfirmEmail() {

            String code = ctx.Get( "c" );

            User user = confirmService.Valid( code );
            if (user != null) {

                loginService.Login( user, LoginTime.Forever, ctx.Ip, ctx );

                echoRedirect( lang( "confirmok" ), sys.Path.Root );
            }
            else {
                actionContent( "<div style=\"width:300px;margin:auto;padding:50px;font-size:28px;font-weight:bold;color:red;\">" + lang( "exConfirm" ) + "</div>" );
            }
        }

        private void setLoginValidationCode() {
            IBlock cblock = getBlock( "Captcha" );
            if (config.Instance.Site.LoginNeedImgValidation) {
                cblock.Set( "ValidationCode", Html.Captcha );
                cblock.Next();
            }
        }

        private String getReturnUrl() {

            String returnUrl = ctx.Get( "returnUrl" );
            if (strUtil.HasText( returnUrl )) return returnUrl;

            returnUrl = ((ctx.web.PathReferrer == null) ? string.Empty : ctx.web.PathReferrer.ToString());

            return returnUrl;
        }

        private String getSavedReturnUrl() {
            String returnUrl = ctx.Post( "returnUrl" );
            if (strUtil.IsNullOrEmpty( returnUrl )) {
                returnUrl = sys.Path.Root;
            }

            returnUrl = returnUrl.Replace( "&amp;", "&" );

            return returnUrl;
        }

        public void lost() {
            HideLayout( typeof( LayoutController ) );
            actionContent( "wojilu.lostPage" );
        }

        public void LoginScript() {
            HideLayout( typeof( LayoutController ) );

            IBlock welcomeBlock = getBlock( "welcome" );
            IBlock loginBlock = getBlock( "login" );

            if (ctx.viewer.IsLogin) {

                welcomeBlock.Set( "user.Name", ctx.viewer.obj.Name );
                welcomeBlock.Set( "user.LogoutLink", t2( new MainController().Logout ) );
                welcomeBlock.Set( "user.HomeLink", Link.T2( ctx.viewer.obj, new FeedController().My, -1 ) );
                welcomeBlock.Set( "user.SpaceLink", Link.ToMember( ctx.viewer.obj ) );
                welcomeBlock.Next();
            }
            else {

                loginBlock.Set( "ActionLink", t2( new MainController().CheckLogin ) );
                loginBlock.Set( "regLink", t2( new RegisterController().Register ) );
                loginBlock.Next();
            }

        }




    }
}

