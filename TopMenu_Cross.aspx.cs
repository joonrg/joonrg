using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

namespace Kaoni.ezStandard.Web.ezPortal
{
	/// <summary>
	/// 상단메뉴
	/// </summary>
	public partial class TopMenu : ezWebBase
	{
		public string strHTML = "";
		public string PageID = "";
		public string ParentPageID = "";
		public string mode = "";
		public string editmode = "";
		public string viewmode = "";
		public string displayname = "";
		public string width = "";
		public string height = "";
		public string _password;
		public string skinnum = "1";
        public string HomeSkin = "1";
		public string SkinBgFlag = "";
		public string SkinBgColor = "";
		public string SkinBgImage = "";
		public string SkinFontColor = "";
		public string SkinFontOverColor = "";
		public string SkinBgString = "";
		public string SkinExist = "NO";
        public string result = ""; //관리자 버튼 활성화 비활성과 체크 

		// 부문포탈 관련
		public string PortalMenuID = "";
		public string PortalMenuXml = "";


		// 설문 존재시 알림창 표시
		public int pollNum = 0;

	   	public string _script1;
        // 2012.07.13 : 기념일 팝업
        public string _script2;

        // 2014.03.13 : 회사일정 팝업
        public string _script3;

        //2019.01.18 노무수령거부/서약서 추가
        public string _script4;
        public string _script5;

        //topmenu 레이아웃 다중 
        public string TopMNID = "skin";
        public string cookies ;
        public string currSkin = "";

        // 참석자 카운트
        public int pReceiveCnt = 0;
        public int pReceiveGCnt = 0;

        //select box 초기화 
        public string p_Morebox = "false";

        
        string DeptPathCode = "";

        //2013.06.12 : 호환성보기설정 회사별로 체크
        public string pBroserEmulation = "";

        //2016.09.08 전체팝업공지
        public string PopupNotice = "NO";

        // 정은하(20211006) Bizoffice 팝업 공지
        public string PopupNotice2 = "NO";

        //20180427 타입별 css 구분
        protected string cssUrl = string.Empty;
        protected string lastLogIn = string.Empty;

        protected string currentUserLang = string.Empty;
		protected string bodyStyle = "tbody"; //20180627 ahh
        protected string BaseType = string.Empty;

        //20180724 자동로그아웃 추가
        public string pAutoLogOutUse = "";
        public int pAutoLogOutTime = 0;

        //2020.05.13 접속IP 차단 10분단위로 IP체크하여 로그아웃 처리
        public string pIPuse = "";
        //2022.01.17 이승중 서버작업 로그인 차단 추가
        public string inspection_IP = "N";

        protected void Page_Load(object sender, System.EventArgs e)
		{
			CreateUserInfo();

            //20180724 자동로그아웃 추가
            GetAutoLogOutTime();

            //2022.01.17 이승중 서버작업 로그인 차단 추가
            if (GetRequestRemoteAddr() == "222.106.242.2")
                inspection_IP = "Y";

            DeptPathCode = userinfo.UserID + ",top," + userinfo.CompanyID + "," + userinfo.DeptID;
            
            mode = "edit";
            			
			Kaoni.ezStandard.ezPortal.TopMenu _ezPortal = new Kaoni.ezStandard.ezPortal.TopMenu();
            Kaoni.ezStandard.ezPortal.PortalPages _ezPortalPage = new Kaoni.ezStandard.ezPortal.PortalPages();

            //2013.06.12 : 호환성보기설정 회사별로 체크
            pBroserEmulation = GetCompanyExpInfo("BROWSEREMULATION", userinfo.CompanyID);
            //2019.04.08 이승중 XSS 보안처리
            if (ReplaceXSS(Request.QueryString["pageid"], "") != null)
            {
                //2019.04.08 이승중 XSS 보안처리
                PageID = ReplaceXSS(Request.QueryString["pageid"], "");                
                string _TopMNID = _ezPortal.TopUseSkin(PageID, userinfo.CompanyID);
              
                XmlDocument xmlStr = new XmlDocument();
                xmlStr.LoadXml(_TopMNID);

                //20180530:TopMNID 값이 공백일 경우 로딩속도가 느려짐
                if (xmlStr.GetElementsByTagName("TOPMNID").Item(0).InnerText != "")
                    TopMNID = xmlStr.GetElementsByTagName("TOPMNID").Item(0).InnerText;
            }
            else PageID = Guid.NewGuid().ToString();
            //2019.04.08 이승중 XSS 보안처리
            if (ReplaceXSS(Request.QueryString["parentpageid"], "") != null) ParentPageID = ReplaceXSS(Request.QueryString["parentpageid"], "");
			else 
			{
                //2019.04.08 이승중 XSS 보안처리
                if (ReplaceXSS(Request.QueryString["pageid"], "") != null) ParentPageID = _ezPortal.GetTopMenuConfigItem(PageID, "ParentUID");
				else ParentPageID = "top";
			}
            //2019.04.08 이승중 XSS 보안처리
            if (ReplaceXSS(Request.QueryString["mode"], "") != null) mode = ReplaceXSS(Request.QueryString["mode"], "");
			if (mode == "edit") CheckAdmin();

			if (mode == "edit")
			{
                //2019.04.08 이승중 XSS 보안처리
                if (ReplaceXSS(Request.QueryString["pageid"], "") == null && ReplaceXSS(Request.QueryString["parentpageid"], "") != null) 
				{
                    //2019.04.08 이승중 XSS 보안처리
                    if (ReplaceXSS(Request.QueryString["parentpageid"], "").Trim() != "" && ReplaceXSS(Request.QueryString["parentpageid"], "").Trim().ToLower() != "top") editmode = "new_inherit";
				}
			}

            lastLogIn = GetLastLogout();

            // 미리보기
            //2019.04.08 이승중 XSS 보안처리
            viewmode = ReplaceXSS(Request.QueryString["viewmode"], "");

			// 미리보기인 경우 자기의 캐쉬정보를 삭제한다.
			if (viewmode == "preview")
			{
                _ezPortalPage.DeleteCacheValue(PageID, this.UserInfoXML);
                
                //해당 pageid의 basetype 을 가져와서 skinpath를 정한다. 2007-12-02
                string pre_TopMNID = _ezPortal.TopUseSkin(PageID, userinfo.CompanyID);
                XmlDocument xmlSkinpath = new XmlDocument();
                xmlSkinpath.LoadXml(pre_TopMNID);
                TopMNID = xmlSkinpath.GetElementsByTagName("TOPMNID").Item(0).InnerText;
                //2019.04.08 이승중 XSS 보안처리
                if (ReplaceXSS(Request.QueryString["call"], "") == "client") // 사용자 환경설정에서의 미리보기
                { 
                    //사용하는 페이지라면 userinfo 테이블에서 skinid를 가져온다. 2007-12-02
                    string strUseSkin = _ezPortal.UserUseTopskin(userinfo.UserID, userinfo.lang);
                    XmlDocument xmlUseSkin = new XmlDocument();
                    xmlUseSkin.LoadXml(strUseSkin);
                    
                    if (0 < xmlUseSkin.GetElementsByTagName("UID").Count)
                    {
                        if (PageID == xmlUseSkin.GetElementsByTagName("UID").Item(0).InnerText.Trim())
                        {
                            skinnum = xmlUseSkin.GetElementsByTagName("SKINNUM").Item(0).InnerText;
                        }
                        else
                        {
                            skinnum = "1";
                        }
                        
                    }                
                    else
                    {
                        skinnum = "1";
                    } 
                }
                else
                {  
                    skinnum = "1";
                }


				_ezPortalPage.Dispose();
				_ezPortalPage = null;
			}

            // topmenu스킨 색 정보
            //2019.04.08 이승중 XSS 보안처리
            if (ReplaceXSS(Request.QueryString["skinnum"], "") != null)
            {                
                //skinnum = Request.QueryString["skinnum"];

                string strSkin = _ezPortal.UserUseTopskin(userinfo.UserID, userinfo.lang);                                
                XmlDocument xmlSkin = new XmlDocument();
                xmlSkin.LoadXml(strSkin);
              

                if (0 < xmlSkin.GetElementsByTagName("SKINNUM").Count)
                {
                    skinnum = xmlSkin.GetElementsByTagName("SKINNUM").Item(0).InnerText;
                    
                }
                else
                    skinnum = "1"; 
            }

            //포탈화면 스킨정보 
            //2019.04.08 이승중 XSS 보안처리
            if (ReplaceXSS(Request.QueryString["skinnum"], "") != null)
            {
                //skinnum = Request.QueryString["skinnum"];

                string strSkin = _ezPortal.UserUsePortalskin(userinfo.UserID, userinfo.lang);
                XmlDocument xmlSkin1 = new XmlDocument();
                xmlSkin1.LoadXml(strSkin);


                if (0 < xmlSkin1.GetElementsByTagName("BASETYPE").Count)
                {
                    HomeSkin = xmlSkin1.GetElementsByTagName("BASETYPE").Item(0).InnerText;

                }
                else
                    HomeSkin = "1";
            }


            //권한체크_2007-09-18 추가 
            result = _ezPortal.ezACL_Check(userinfo.UserID, userinfo.CompanyID, userinfo.CompanyName); //1:비즈메카관리자, 2:회사관리자, 3:일반사용자.          
            
            string CK_AdminACL = "";
            string CK_AdminACL2 = "";

            if (result == "3")
            {
                //2017.05.30 관리자 인지 아닌지를 체크하여 비즈메카 홈 버튼을 활성화
                CK_AdminACL2 = ezCk_AdminACL2(userinfo.UserID, PageID, result);

                //2017.06.19 관리자 기능 분리 작업
                SqlCommand cmd;
                cmd = new SqlCommand(" SELECT codename FROM [dbo].[TBL_ADM_Menu_ACL] WHERE [companyid]=@companyid and [userID]=@userID and [flag]='Y' ");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@companyid", SqlDbType.NVarChar, 30).Value = userinfo.CompanyID;
                cmd.Parameters.Add("@userID", System.Data.SqlDbType.NVarChar, 30).Value = userinfo.UserID;

                string rtnResult = GetQueryResultSP("ezhrmaster", ref cmd);

                XmlDocument xmldom = new XmlDocument();
                xmldom.LoadXml(rtnResult);

                if (xmldom.GetElementsByTagName("CODENAME").Count > 0)
                {
                    result = "2";
                }
                
                // 삭제 쿼리 실행 (id 와 pageid 값 ) 
                CK_AdminACL = _ezPortal.ezCk_AdminACL(userinfo.UserID, PageID, result);
            }
            else
            {
                // 체크 쿼리 (id와 pageid 값을 받아서 .. )
                CK_AdminACL = _ezPortal.ezCk_AdminACL(userinfo.UserID, PageID, result);
                //2017.05.30 관리자 인지 아닌지를 체크하여 비즈메카 홈 버튼을 활성화
                CK_AdminACL2 = ezCk_AdminACL2(userinfo.UserID, PageID, result);
            }    
            

			// 20071025
			// ex) 한글일 경우 skin1 폴더
			//     영어일 경우 skin1_2 폴더
			if (userinfo.lang == "1")
			{   
				Response.Cookies["SKINNUM"].Value = skinnum;
                Response.Cookies["SKINNUM"].Domain = GetCookieDomain(); //2007-12-02 


                currSkin = skinnum;
			}
			else
			{
				Response.Cookies["SKINNUM"].Value = skinnum + "_2";
                Response.Cookies["SKINNUM"].Domain = GetCookieDomain(); //2007-12-02 


                currSkin = skinnum + "_2";
			}

            //20071031_ 한개의 top레이아웃을 제공 하는 것이 아니라 다양 하게 제공 하기 위해 사용하는 skin 폴더를 구분한다.
            //ex) 표준모듈일 경우 skin 
            //    비즈메카 신규일 경우 skin1

            
            Response.Cookies["TOPMNID"].Value = TopMNID;
            Response.Cookies["TOPMNID"].Domain = GetCookieDomain(); //2007-12-02 



            if (HomeSkin == "2")
            {
                Response.Cookies["Homeskin"].Value = "skin1";
                Response.Cookies["Homeskin"].Domain = GetCookieDomain(); //2007-12-02 

            }
            else if (HomeSkin == "3")
            {
                Response.Cookies["Homeskin"].Value = "skin2";
                Response.Cookies["Homeskin"].Domain = GetCookieDomain(); //2007-12-02 

            }
            else
            {
                Response.Cookies["Homeskin"].Value = "skin";
                Response.Cookies["Homeskin"].Domain = GetCookieDomain(); //2007-12-02 

            }


  
                Response.Cookies["HOMENUM"].Value = skinnum;
                Response.Cookies["HOMENUM"].Domain = GetCookieDomain(); //2007-12-02 



                //Response.Write(userinfo.DeptPathCode);


                

			// 새로만들기
			if (mode == "new")
			{
				strHTML = _ezPortal.GetDefaultTopMenu();
			}
			else	// 열기: 본문HTML, width, height정보를 가져온다
			{
                if (editmode == "new_inherit")
                {   //캐쉬생성할때 회사아이디 등록되도록 수정 2007-10-30                   
                    strHTML = _ezPortal.GetRenderedTopMenuHTML(ParentPageID, DeptPathCode, mode, skinnum, UserInfoXML, userinfo.CompanyID, userinfo.UserID);
                    width = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(ParentPageID), "width");
                    height = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(ParentPageID), "height");
                }
                // mode: view, edit
                else
                {
                    ////test 20180423
                    //if(userinfo.UserID == "dev03@withkt") Response.Redirect("/myoffice/ezPortal/TopMenu_renewal.aspx?skinnum=" + currSkin + "&TOPMNID=" + TopMNID, false);  //심플 topmenu 테스트중 

                    //대문자 회사아이디인 회사를 위해 추가 2008-10-22
                    if (PageID == "simple@" + userinfo.CompanyID.ToLower() || PageID == "simple@" + userinfo.CompanyID.ToUpper()) //userinfo테이블에 저장되어 있는 pageuid가 심플일 경우
                    {
                        Response.Redirect("/myoffice/ezPortal/simple_TopMenu_Cross.aspx?skinnum=" + currSkin + "&TOPMNID=" + TopMNID, false);  //심플 topmenu 테스트중 
                    }
                    else
                    {
                        //캐쉬 생성할때 회사 아이디 등록되도록 수정 2007-10-30
                        strHTML = _ezPortal.GetRenderedTopMenuHTML(PageID, DeptPathCode, mode, skinnum, UserInfoXML, userinfo.CompanyID, userinfo.UserID);
                        width = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(PageID), "width");
                        height = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(PageID), "height");

                        //WriteTextLog("ddd", _ezPortal.GetMenuItemHTML(PageID, _ezPortal.GetTopParentPageID(PageID),UserInfoXML), "ezPortal");

                        //20180426 타입 1,2 선택
                        BaseType = GetTopMenuBaseType(PageID);
                        if (string.IsNullOrEmpty(BaseType) == false)
                        {
                            if (BaseType.ToUpper().Equals("TYPE1") == true)
                            {
                                cssUrl = "/css/type1/main.css";
                            }
                            else if (BaseType.ToUpper().Equals("TYPE2") == true)
							{
								cssUrl = "/css/type2/main_cross.css";
							}
                            //2021.04.21 이승중 N포탈 H타입 추가건 처리_작업중
                            else if (BaseType.ToUpper().Equals("TYPE4") == true)
                            {
                                cssUrl = "/css/type4/main_cross.css";
                            }
                            else
                            {
                                cssUrl = "/css/type3/main_cross.css";								
                            }
                        }

                        //2021.12.07 비즈오피스 분기처리 - 로고 분기처리
                        SqlCommand cmd_l = new SqlCommand(" SELECT * FROM [DBO].[COMPANYCLOUDINFO] WITH(NOLOCK) WHERE (MEMSQ LIKE 'M%' OR MEMSQ LIKE 'T%') and CompanyID=@PCOMPANYID ");
                        cmd_l.CommandType = CommandType.Text;
                        cmd_l.Parameters.Add("@PCOMPANYID", SqlDbType.VarChar, 20).Value = userinfo.CompanyID;
                        string result_l = GetQueryResultSP(ref cmd_l, "entumadmin", false);
                        cmd_l.Dispose();
                        cmd_l = null;
                        XmlDocument xmldom_l = new XmlDocument();
                        xmldom_l.LoadXml(result_l);

                        //2021.12.07 비즈오피스 분기처리 - 로고 분기처리
                        if (xmldom_l.GetElementsByTagName("COMPANYID").Count > 0)
                        {
                            if (strHTML.IndexOf("/Upload_Portal/main_logo.png") > -1)
                            {
                                strHTML = strHTML.Replace("/Upload_Portal/main_logo.png", "/Upload_Portal/main_logo_BizOffiece.png");
                            }
                            else if (strHTML.IndexOf("/Upload_Portal/main_logo_G.png") > -1)
                            {
                                strHTML = strHTML.Replace("/Upload_Portal/main_logo_G.png", "/Upload_Portal/main_logo_H.png");
                            }
                        }

                    }
                }
			}
            
			if (width == "" || width == "-1" || width == "0") width = "100%";
			if (height == "" || height == "-1" || height == "0") height = "100%";
			if (mode != "view") displayname = _ezPortal.GetTopMenuConfigItem(PageID, "DisplayName");
			

			// 사용자 영역에서만 팝업 공지사항을 오픈한다.
			if (mode == "view" && viewmode != "preview")
			{
				// 팝업 공지사항
                //ezPersonal.PopUp _ezPersonal = new ezPersonal.PopUp();
                //string infoXML = _ezPersonal.GetPopUpListUser(userinfo.CompanyID);
                //_ezPersonal.Dispose();
                //_ezPersonal = null;

                //XmlDocument xmldom = new XmlDocument();
                //xmldom.LoadXml(infoXML);

                // 팝업 공지사항 _ Sp 로 가져오도록 수정 2008-08-29
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandText = "EZSP_GETPOPUPLISTUSER";

                comm.Parameters.Add("@pCompanyID", SqlDbType.VarChar, 100);
                comm.Parameters["@pCompanyID"].Value = userinfo.CompanyID;
                comm.Parameters.Add("@pDeptID", SqlDbType.VarChar, 100);
                comm.Parameters["@pDeptID"].Value = userinfo.DeptID;
                string infoXML = this.GetQueryResultSP("ezPersonal", ref comm);

                comm.Dispose();
                comm = null;
                XmlDocument xmldom = new XmlDocument();
                xmldom.LoadXml(infoXML);

				string popup = "";

                //2019.03.19 이승중 팝업 공지사항 설정 시 여러 공지가 있을 시 팝업이 한 위치에서만 여러개 열리는 현상 수정
                int openTop = 200; int openLeft = 250;

                for (int i=0; i<xmldom.DocumentElement.ChildNodes.Count; i++)
				{
					string itemseq = xmldom.DocumentElement.ChildNodes.Item(i).SelectSingleNode("ITEMSEQ").InnerText;

                    if (Request.Cookies["POPUP_" + itemseq] == null || Request.Cookies["POPUP_" + itemseq].Value == "")
					{
						string popupWidth = xmldom.DocumentElement.ChildNodes.Item(i).SelectSingleNode("WIDTH").InnerText;
						string popupHeight = xmldom.DocumentElement.ChildNodes.Item(i).SelectSingleNode("HEIGHT").InnerText;
                        // 20111024 : 팝업 공지사항 리사이즈 처리, resizable=0 -> 1
                        //2019.03.19 이승중 팝업 공지사항 설정 시 여러 공지가 있을 시 팝업이 한 위치에서만 여러개 열리는 현상 수정
                        //popup += "window.open('/myoffice/ezPersonal/PopUp/ShowPopUp.aspx?itemseq=" + itemseq + "', '', 'height=" + popupHeight + "px,width=" + popupWidth + "px,top=200px,left=250px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";
                        popup += "window.open('/myoffice/ezPersonal/PopUp/ShowPopUp.aspx?itemseq=" + itemseq + "', '', 'height=" + popupHeight + "px,width=" + popupWidth + "px,top="+ openTop + "px,left="+ openLeft + "px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";

                        //2019.03.19 이승중 팝업 공지사항 설정 시 여러 공지가 있을 시 팝업이 한 위치에서만 여러개 열리는 현상 수정
                        openTop += 15; openLeft += 100;
                    }
				}
				xmldom = null;

                //2021.11.17 비즈메카 전체공지 기능 개선
                string chkadmin = userinfo.RollInfo;
                string pChkadmin = "";

                if (chkadmin.IndexOf("k=1;") > -1)
                {
                    pChkadmin += " or AdminAuthority like '%k=1;%' ";
                }
                if (chkadmin.IndexOf("d=1;") > -1)
                {
                    pChkadmin += " or AdminAuthority like '%d=1;%' ";
                }
                if (chkadmin.IndexOf("g=1;") > -1)
                {
                    pChkadmin += " or AdminAuthority like '%g=1;%' ";
                }

                SqlCommand cmd_p = null;
                cmd_p = new SqlCommand(" SELECT A.ItemSeq, Width, Height FROM TBLPOPUP A WITH(NOLOCK) "
                    + " LEFT OUTER JOIN [ezPersonal].[dbo].[TBLPOPUP_CompanyList] as B WITH(NOLOCK) on A.ITEMSEQ = B.ITEMSEQ "
                    + " WHERE A.CompanyID IN ('TOP') AND ISPOPUP='1' "
                    + " AND (PortalType is null Or PortalType = '' Or PortalType = 'A' Or PortalType = 'N') "
                    + " AND (AdminAuthority is null or AdminAuthority = '' " + pChkadmin + ") "
                    + " AND (CompanyAuthority is null or CompanyAuthority = '' or (CompanyAuthority = 'Y' and B.[CompanyID] = @pCompanyID))  "
                    + " AND CONVERT(NVARCHAR(10),StartDate,120) <= CONVERT(NVARCHAR(10),GETDATE(),120) "
                    + " AND CONVERT(NVARCHAR(10),EndDate,120) >= CONVERT(NVARCHAR(10),GETDATE(),120) ");
                cmd_p.CommandType = CommandType.Text;

                cmd_p.Parameters.Add("@pCompanyID", SqlDbType.NVarChar, 30).Value = userinfo.CompanyID;

                string result_p = this.GetQueryResultSP(ref cmd_p, "ezPersonal", true);

                cmd_p.Dispose();
                cmd_p = null;
                XmlDocument xmlResult = new XmlDocument();
                xmlResult.LoadXml(result_p);

                for (int i = 0; i < xmlResult.DocumentElement.ChildNodes.Count; i++)
                {
                    string itemseq = xmlResult.DocumentElement.ChildNodes.Item(i).SelectSingleNode("ITEMSEQ").InnerText;

                    if (Request.Cookies["POPUP_" + itemseq] == null || Request.Cookies["POPUP_" + itemseq].Value == "")
                    {
                        string popupWidth = xmlResult.DocumentElement.ChildNodes.Item(i).SelectSingleNode("WIDTH").InnerText;
                        string popupHeight = xmlResult.DocumentElement.ChildNodes.Item(i).SelectSingleNode("HEIGHT").InnerText;
                        popup += "window.open('/myoffice/ezPersonal/PopUp/ShowPopUp.aspx?itemseq=" + itemseq + "', '', 'height=" + popupHeight + "px,width=" + popupWidth + "px,top=" + openTop + "px,left=" + openLeft + "px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";

                        openTop += 15; openLeft += 100;
                    }
                }
			
				// 표준모듈 (2007.03.15) 수정: .NET Framework 2.0에서는 RegisterStartupScript 메서드 지원하지 않음.
				if (popup != "")
					_script1 = "<script language='javascript'>" + popup + "</script>";
					//Page.RegisterStartupScript("PopUp", "<script>" + popup + "</script>");
                // 팝업 공지사항 끝

                /**** 2012.07.13 : 기념일팝업 시작 ****/
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "EZSP_GETPOPUPANIIVERSARYLIST";

                cmd.Parameters.Add("@pCompanyID", SqlDbType.NVarChar, 50);
                cmd.Parameters["@pCompanyID"].Value = userinfo.CompanyID;
                cmd.Parameters.Add("@pDate", SqlDbType.VarChar, 7);
                cmd.Parameters["@pDate"].Value = DateTime.Now.ToString("yyyy-MM");
                cmd.Parameters.Add("@pMonth", SqlDbType.VarChar, 2);
                cmd.Parameters["@pMonth"].Value = DateTime.Now.ToString("MM");

                string rtnXML = this.GetQueryResultSP("ezPims", ref cmd);

                cmd.Dispose();
                cmd = null;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(rtnXML);
                //WriteTextLog("ddd", rtnXML, "min");
                string popupAnnivarsary = "";
                for (int i = 0; i < xmldoc.DocumentElement.ChildNodes.Count; i++)
                {
                    string schID = xmldoc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("SCHEDULEID").InnerText;
                    //2012.08.02 : 기념일 팝업크기 기능개선
                    string AnnivarsaryWidth = xmldoc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("WIDTH").InnerText;
                    string AnnivarsaryHeight = xmldoc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("HEIGHT").InnerText;

                    if (Request.Cookies["POPUP_" + schID] == null ||
                        Request.Cookies["POPUP_" + schID].Value == "")
                    {
                        popupAnnivarsary += "window.open('/myoffice/ezSchedule/ShowPopUpAnniversary.aspx?itemseq=" + schID + "', '', 'height=" + AnnivarsaryHeight + "px,width=" + AnnivarsaryWidth + "px,top=200px,left=250px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";
                    }
                }
                xmldoc = null;

                // 표준모듈 (2007.03.15) 수정: .NET Framework 2.0에서는 RegisterStartupScript 메서드 지원하지 않음.
                if (popupAnnivarsary != "")
                    _script2 = "<script language='javascript'>" + popupAnnivarsary + "</script>";
                /**** 2012.07.13 : 기념일팝업 끝 ****/

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /**** 2014.03.13 : 회사일정 팝업 시작 ****/
                string resultXML = "";
                string startdate = DateTime.Now.ToString("yyyy-MM-dd");
                string enddate = DateTime.Now.ToString("yyyy-MM-dd");

                startdate = DateTime.Parse(GetDBTime(DateTime.Parse(startdate).AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"))).AddDays(1).ToString().Substring(0, 10);
                enddate = DateTime.Parse(GetDBTime(DateTime.Parse(enddate).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"))).AddDays(-1).ToString().Substring(0, 10);

                ezSchedule.ScheduleInfo _ezSchedule = new ezSchedule.ScheduleInfo();
                resultXML = _ezSchedule.GetScheduleList(userinfo.CompanyID, "anniversary is null and popUpFlag='Y'", "SCHEDULEID,STARTDATE,ENDDATE,DATETYPE,REPETITION,REPETITIONDELETE,WIDTH,HEIGHT", startdate, enddate);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resultXML);

                string schedulePopUp = "";
                for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
                {
                    string schID = doc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("SCHEDULEID").InnerText;
                    string PopUpWidth = doc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("WIDTH").InnerText;
                    string PopUpHeight = doc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("HEIGHT").InnerText;
                    if (Request.Cookies["POPUP_" + schID] == null || Request.Cookies["POPUP_" + schID].Value == "")
                    {
                        schedulePopUp += "window.open('/myoffice/ezSchedule/schedule_popUp.aspx?itemseq=" + schID + "', '', 'height=" + PopUpWidth + "px,width=" + PopUpHeight + "px,top=200px,left=250px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";
                    }
                }
                doc = null;

                if (schedulePopUp != "")
                    _script3 = "<script language='javascript'>" + schedulePopUp + "</script>";
                /**** 2014.03.13 : 회사일정 팝업 끝 ****/
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //2019.01.18 노무수령거부/서약서 추가
                SqlCommand cmd1 = new SqlCommand();
                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                cmd1.CommandText = "EZSP_GET_VOW_LIST";

                cmd1.Parameters.Add("@pCompanyID", SqlDbType.NVarChar, 40);
                cmd1.Parameters["@pCompanyID"].Value = userinfo.CompanyID;
                cmd1.Parameters.Add("@pUserID", SqlDbType.NVarChar, 20);
                cmd1.Parameters["@pUserID"].Value = userinfo.UserID;
                cmd1.Parameters.Add("@pDate", SqlDbType.DateTime);
                cmd1.Parameters["@pDate"].Value = DateTime.Now;
                string result2 = this.GetQueryResultSP("ezBoardSTD", ref cmd1);
                XmlDocument xmlVOW = new XmlDocument();
                xmlVOW.LoadXml(result2);

                if (xmlVOW.GetElementsByTagName("ROW").Count > 0)
                {
                    for (int i = 0; i < xmlVOW.GetElementsByTagName("ROW").Count; i++)
                    {
                        string pItemID = xmlVOW.SelectNodes("DATA/ROW/ITEMID")[i].InnerText;
                        _script5 += pItemID + ";";
                        //vowlist += "window.showModalDialog('/myoffice/ezBoardSTD/VOW/vow_view.aspx?ItemID=" + pItemID + "', '', 'dialogHeight:920px; dialogWidth:765px; status:no;scroll:no; help:no; edge:sunken');";
                        //vowlist += "DivPopUpShow(720, 765, '/myoffice/ezBoardSTD/VOW/vow_view.aspx?ItemID=" + pItemID + "');";
                    }
                }

                string strSQL = "SELECT UserID FROM TBL_NONACCEPTANCE WHERE CompanyID = '" + userinfo.CompanyID + "' AND UserID = '" + userinfo.UserID + "' AND VacationDate = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND AgreeYN IS NULL ";
                string result1 = GetQueryResult("ezPersonal", strSQL, false);
                XmlDocument xmlNon = new XmlDocument();
                xmlNon.LoadXml(result1);

                if (xmlNon.GetElementsByTagName("USERID").Count > 0)
                {
                    //nonacceptance += "window.showModalDialog('/myoffice/ezBoardSTD/VOW/vow_view.aspx?Type=Y', '', 'dialogHeight:920px; dialogWidth:765px; status:no;scroll:no; help:no; edge:sunken');";
                    _script4 = "Y";
                }
			}
			
			// 패스워드 암호화
			_password = EncryptString(Request.ServerVariables["AUTH_PASSWORD"].Trim());
						
			// 스킨정보
			string ResultXML = _ezPortal.GetSkinInfo(PageID, skinnum);
			
			XmlDocument xmldom2 = new XmlDocument();
			xmldom2.LoadXml(ResultXML);
			
			if (xmldom2.GetElementsByTagName("UID").Count > 0)
			{
				SkinBgFlag = xmldom2.GetElementsByTagName("SKINBGFLAG").Item(0).InnerText.Trim();
				SkinBgColor = xmldom2.GetElementsByTagName("SKINBGCOLOR").Item(0).InnerText.Trim();
				SkinBgImage = xmldom2.GetElementsByTagName("SKINBGIMAGE").Item(0).InnerText.Trim();
				SkinFontColor = xmldom2.GetElementsByTagName("SKINFONTCOLOR").Item(0).InnerText.Trim();
				SkinFontOverColor = xmldom2.GetElementsByTagName("SKINFONTOVERCOLOR").Item(0).InnerText.Trim();

				// 1:color, 2:image
				if (SkinBgFlag == "1")
					SkinBgString = "background-color:" + SkinBgColor + "";
				else if (SkinBgFlag == "2")
					SkinBgString = "background-image:url(" + SkinBgImage + ")";
				
				SkinExist = "YES";
			}
			xmldom2 = null;



            //사용하는 toppage의 main메뉴 개수를 가져와 9개 보다 크면 true 값을 넣어준다. 
            string pMainXml = "";
            string p_Count = "";

            XmlDocument xmlACL = new XmlDocument();


            if (PageID != "simple@" + userinfo.CompanyID.ToLower() || PageID != "simple@" + userinfo.CompanyID.ToUpper()) //대문자 회사아이디인 회사를 위해 추가 2008-10-22
            {
                if (editmode == "new_inherit")
                {
                    pMainXml = _ezPortal.GetMainMenu_ACLCount(ParentPageID, DeptPathCode);
                }
                else
                {
                    if (PageID != "simple@" + userinfo.CompanyID.ToLower() || PageID != "simple@" + userinfo.CompanyID.ToUpper()) //대문자 회사아이디인 회사를 위해 추가 2008-10-22
                    {
                        pMainXml = _ezPortal.GetMainMenu_ACLCount(PageID, DeptPathCode);
                    }
                }

                xmlACL.LoadXml(pMainXml);

                if (xmlACL.InnerXml == "<DATA></DATA>")
                {
                    p_Morebox = "false";
                }
                else 
                {
                     p_Count = xmlACL.GetElementsByTagName("MAINMENU_COUNT").Item(0).InnerText.Trim();


                    if (int.Parse(p_Count) >  9)
                        p_Morebox = "true";
                    else
                        p_Morebox = "false";
                }
                
            }

			_ezPortal.Dispose();
			_ezPortal = null;

            //WriteTextLog("123", strHTML, "123123123jinjin");

            // 20071030 table fixed 제거
            strHTML = strHTML.Replace("table-layout:fixed;", "");
            strHTML = strHTML.Replace("img_onMouseOver(", "img_onMouseOver(event,");
            strHTML = strHTML.Replace("img_onMouseOut()", "img_onMouseOut(event)");
            strHTML = strHTML.Replace("index_environment.htm", "index_environment_Cross.htm");
            strHTML = strHTML.Replace("myportal.aspx", "myportal_Cross.aspx");

            //20180724:자동로그아웃 임시 테스트
            if(pAutoLogOutUse == "Y")
                strHTML = strHTML.Replace("<span id='spanLastLogin'></span>", "<span id='spanLastLogin'></span><span id='spanAutoLogout' class='logoutAuto'></span>");

            currentUserLang = GetUserLang();

            string tempHtml = ChangeLocalString(strHTML);

            if (string.IsNullOrEmpty(tempHtml) == false)
            {
                strHTML = tempHtml;
            }

            // 2008.05.13
            // 일정 참석자초대 팝업을 로그인 했을때 보여주기
            //pReceiveCnt = GetReceiveCount(); 20081113 주석처리 부하로 인해 체크 안하도록 함. 
            // 일정 참석자그룹초대 팝업을 로그인 했을때 보여주기
            //pReceiveGCnt = GetReceiveGropuCount(); 20081113 주석처리 부하로 인해 체크 안하도록 함. 

            // 2016.09.08 전체팝업공지
            string pToday = DateTime.Now.ToString("yyyy-MM-dd");
            if (DateTime.Now > DateTime.Parse("2016-09-08 18:00:00") && DateTime.Parse(pToday) < DateTime.Parse("2016-09-17 23:00:00"))
            {
                PopupNotice = "OK";
            }

            // 정은하(20211006) Bizoffice 팝업 공지
            if (DateTime.Now > DateTime.Parse("2021-10-05 11:00:00") && DateTime.Parse(pToday) < DateTime.Parse("2021-10-20 23:00:00"))
            {
                PopupNotice2 = "OK";
            }

            //2020.05.13 접속IP 차단 10분단위로 IP체크하여 로그아웃 처리
            SqlCommand cmd_ip;
            cmd_ip = new SqlCommand(" SELECT USEFLAG FROM DBO.TBLIPOPTION WITH(NOLOCK) WHERE COMPANYID = @PCOMPANYID ");
            cmd_ip.CommandType = CommandType.Text;
            cmd_ip.Parameters.Add("@PCOMPANYID", SqlDbType.NVarChar, 30).Value = userinfo.CompanyID;

            string rtnResult_ip = GetQueryResultSP("ezPersonal", ref cmd_ip);

            XmlDocument xmldom_ip = new XmlDocument();
            xmldom_ip.LoadXml(rtnResult_ip);

            if (xmldom_ip.GetElementsByTagName("USEFLAG").Count > 0)
            {
                pIPuse = xmldom_ip.GetElementsByTagName("USEFLAG").Item(0).InnerText;
            }

		}

        //20180515 사용자 언어 설정 값 얻기
        private string GetUserLang()        
        {
            string returnLang = string.Empty;

            Kaoni.ezStandard.ezCommon _ezCommon = new Kaoni.ezStandard.ezCommon();
            string tempResult = _ezCommon.GetUserLang(userinfo.UserID);

            XmlDocument xmldom = new XmlDocument();
            xmldom.LoadXml(tempResult);
            //WriteTextLog("555", xmldom.GetElementsByTagName("LANG").Item(0).InnerText, "ezPortal");
            if (xmldom.GetElementsByTagName("LANG").Count == 0)
            {
                returnLang = "1";
            }
            else if (xmldom.GetElementsByTagName("LANG").Item(0).InnerText.Equals("0"))
            {
                returnLang = "1";
            }
            else
            {
                returnLang = xmldom.GetElementsByTagName("LANG").Item(0).InnerText;
            }

            return returnLang;
        }

        //20180511 각 언어별 텍스트 변경
        private string ChangeLocalString(string tempStrHtml)
        {
            if (string.IsNullOrEmpty(tempStrHtml) == true)
            {
                return string.Empty;
            }
            else
            {
                string[] splitOption1 = new string[] { "<RM>" };
                string[] splitOption2 = new string[] { "</RM>" };
                ArrayList tempArr = new ArrayList();
                
                string[] splitArray = tempStrHtml.Split(splitOption1, StringSplitOptions.None);
                string[] rmArray = null;
                //WriteTextLog("555", strHtml, "ezPortal");
                //WriteTextLog("777", splitArray.Length.ToString(), "ezPortal");
                
                for (int i = 1; i < splitArray.Length; ++i)
                {
                    rmArray = splitArray[i].Split(splitOption2, StringSplitOptions.None);

                    
                    if (rmArray != null)
                    {
                        if (rmArray.Length == 2)
                        {
                            tempArr.Add(rmArray[0]);
                        }
                    }
                }
               
                for (int j = 0; j < tempArr.Count; ++j)
                {
                    tempStrHtml = tempStrHtml.Replace("<RM>" + tempArr[j].ToString() + "</RM>",GetRMString(tempArr[j].ToString()));                                        
                }
                //WriteTextLog("asd", userinfo.lang + "::" + System.Web.HttpContext.Current.Request.UserLanguages[0], "ezPortal");
            }

            return tempStrHtml;
        }

        private string GetRMString(string tempString)
        {
            string resultString = string.Empty;

            try
            {                
                if (currentUserLang == "1")
                {
                    resultString = RM.GetString(tempString, System.Globalization.CultureInfo.CreateSpecificCulture("ko-KR"));
                }
                else if (currentUserLang == "2")
                {
                    resultString = RM.GetString(tempString, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                }
                else if (currentUserLang == "3")
                {
                    resultString = RM.GetString(tempString, System.Globalization.CultureInfo.CreateSpecificCulture("ja-JP"));
                }
                else if (currentUserLang == "4")
                {
                    resultString = RM.GetString(tempString, System.Globalization.CultureInfo.CreateSpecificCulture("zh-CN"));
                }                
            }
            catch
            {
                resultString = tempString;
            }

            return resultString;
        }

		#region Web Form 디자이너에서 생성한 코드
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 이 호출은 ASP.NET Web Form 디자이너에 필요합니다.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

        public int GetReceiveCount()
        {
            int receiveCnt = 0;
            string pSQL = "";
            pSQL = string.Format(@"SELECT COUNT(ATTENDANTID) FROM EZPIMS.DBO.TBLATTENDANT WHERE ATTENDANTID='{0}' 
                AND (STATUS=0 OR STATUS=3)", userinfo.UserID);

            receiveCnt = this.GetQueryValue("ezPims", pSQL);
            return receiveCnt;
        }

        public int GetReceiveGropuCount()
        {
            int receiveGCnt = 0;
            string pSQL = "";
            pSQL = string.Format(@"SELECT COUNT(A.GROUPID) FROM EZPIMS.DBO.TBLSCHEDULEGROUP A JOIN TBLSCHEDULEGROUPMEMBER B
                   ON A.GROUPID=B.GROUPID WHERE MEMBERID='{0}' AND (STATUS=0 OR STATUS=3)", userinfo.UserID);

            receiveGCnt = this.GetQueryValue("ezPims", pSQL);
            return receiveGCnt;
        }


        private string GetTopMenuBaseType(string pTopMenuID)
        {
            string outResult = string.Empty;
            string strSQL = "SELECT BASETYPE FROM TBL_TopMenu_General with (nolock) WHERE UID = @temp";

            SqlCommand cmd = new SqlCommand(strSQL);
            cmd.Parameters.Add("@temp", SqlDbType.NVarChar, 50);
            cmd.Parameters["@temp"].Value = pTopMenuID;

            string tempResult = this.GetQueryResultSP("ezPortal", ref cmd);

            XmlDocument xmldom = new XmlDocument();
            xmldom.LoadXml(tempResult);

            if (xmldom.GetElementsByTagName("BASETYPE") == null)
            {
                outResult = "TYPE2";
            }
            else if (xmldom.GetElementsByTagName("BASETYPE").Count == 0)
            {
                outResult = "TYPE2";
            }
            else
            {
                outResult = xmldom.GetElementsByTagName("BASETYPE").Item(0).InnerText;
            }

            cmd.Dispose();
            cmd = null;

            return outResult;
        }

        private string GetLastLogout()
        {
            string lastlogin = string.Empty;
            ezPersonal.WebPart _ezPersonal = new ezPersonal.WebPart();
            string result = _ezPersonal.GetLastLogout(userinfo.UserID);
            _ezPersonal.Dispose();
            _ezPersonal = null;

            XmlDocument xmltmp = new XmlDocument();
            xmltmp.LoadXml(result);

            string LastLogin = "";
            string LastConnect = "";

            if (xmltmp.GetElementsByTagName("DATA").Item(0).InnerText != "")
            {
                LastLogin = xmltmp.GetElementsByTagName("LASTLOGIN").Item(0).InnerText;
                LastConnect = xmltmp.GetElementsByTagName("LASTCONNECT").Item(0).InnerText;


                //if (DateTime.Parse(LastLogin) == DateTime.Parse(LastConnect))
                if (Convert.ToDateTime(LastLogin) == Convert.ToDateTime(LastConnect))
                {
                    lastlogin = LastLogin;
                }
                else
                {
                    string tmpTime = "";
                    //if (DateTime.Parse(LastLogin) > DateTime.Parse(LastConnect))
                    if (Convert.ToDateTime(LastLogin) > Convert.ToDateTime(LastConnect))
                        lastlogin = LastConnect;
                    else
                        lastlogin = LastLogin;

                }
            }


            // 20070530 TimeZone 처리
            //lastlogin = GetLocalTime(lastlogin.Substring(0, 16));

            string pDateTime = lastlogin.Substring(0, 16);

            string strDateTime = "";
            if (pDateTime == "")
            {
                return strDateTime;
            }
            try
            {
                string pOffset = userinfo.Offset.Split('|')[1];
                strDateTime = DateTime.Parse(pDateTime).AddHours(double.Parse(pOffset.Split(':')[0]) - 9).AddMinutes(double.Parse(pOffset.Split(':')[1])).ToString("yyyy-MM-dd HH:mm:ss");
                if (pDateTime.Length < 19)
                    strDateTime = strDateTime.Substring(0, pDateTime.Length);
                return strDateTime;
            }
            catch (Exception e)
            {
                WriteTextLog("GetLocalTime",e.Message, "ezPortal");
                return pDateTime;
            }
        }

        //20180724 자동로그아웃 추가
        public void GetAutoLogOutTime()
        {
            SqlCommand cmd = null;

            try
            {
                cmd = new SqlCommand("EZSP_GETAUTOLOGOUT");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@COMPANYID", SqlDbType.NVarChar, 50).Value = userinfo.CompanyID;

                string pXml = GetQueryResultSP("ezCommon", ref cmd);

                XmlDocument xmldom = new XmlDocument();
                xmldom.LoadXml(pXml);

                if (xmldom.SelectNodes("DATA/ROW").Count > 0)
                {
                    pAutoLogOutUse = xmldom.SelectNodes("DATA/ROW/ISUSE").Item(0).InnerText.Trim();
                    pAutoLogOutTime = int.Parse(xmldom.SelectNodes("DATA/ROW/TIME").Item(0).InnerText.Trim());
                    pAutoLogOutTime = pAutoLogOutTime * 60;
                }
            }
            catch (Exception ex)
            {
                WriteTextLog("GetAutoLogOutTime", ex.Message, "ezPortal");
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //2017.05.30 관리자 인지 아닌지를 체크하여 비즈메카 홈 버튼을 활성화
        public string ezCk_AdminACL2(string pCN, string pPageID, string pACL)
        {
            try
            {
                string strSQL = ""; //UID를 가져옴
                string tmpSQL = ""; //권한 처리 

                strSQL = "Select UID FROM TBL_MenuItem_Items_MenuItems \n";
                strSQL += "WHERE ownerpageid = @pPageID and parentuid='202' AND DISPLAYNAME='비즈메카 홈'";

                SqlCommand cmd = new SqlCommand(strSQL);
                cmd.Parameters.Add("@pPageID", SqlDbType.NVarChar, 50);
                cmd.Parameters["@pPageID"].Value = pPageID;
                string result = GetSingleQueryResultSP("ezPortal", ref cmd);
                result = result.Trim();
                cmd.Dispose();
                cmd = null;

                tmpSQL = "IF EXISTS (select UID from TBL_Portal_ACL with (nolock) where UID = '" + result + "' AND ACCESSID = '" + userinfo.CompanyID + "') \n";
                tmpSQL += "DELETE FROM TBL_Portal_ACL  WHERE UID = '" + result + "' AND ACCESSID = '" + userinfo.CompanyID + "'";

                if (pACL == "3")
                {
                    tmpSQL += "IF EXISTS (select UID from TBL_Portal_ACL with (nolock) where UID = '" + result + "' AND ACCESSID = '" + pCN + "') \n";
                    tmpSQL += "DELETE FROM TBL_Portal_ACL  WHERE UID = '" + result + "' AND ACCESSID = '" + pCN + "'";

                }
                else
                {
                    tmpSQL += " IF NOT EXISTS (select UID from TBL_Portal_ACL with (nolock) where UID = '" + result + "' AND ACCESSID = '" + pCN + "') \n";
                    tmpSQL += " Insert into TBL_Portal_ACL (uid,accessid,accessname,view_Right,EDIT_RIGHT) values ('" + result + "','" + pCN + "','" + pCN + "','2','2') \n";
                    tmpSQL += " ELSE ";
                    tmpSQL += " update TBL_Portal_ACL set view_Right = '2', EDIT_RIGHT = '2' where UID = '" + result + "' AND ACCESSID = '" + pCN + "' \n";

                }

                string ACL_result = ExecuteSQL("ezPortal", tmpSQL);

                return ACL_result;

            }
            catch (Exception e)
            {
                WriteTextLog(e.Message, e.StackTrace);
                return "ERROR:" + e.Message;
            }

        }
    }
}

