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
	/// ��ܸ޴�
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
        public string result = ""; //������ ��ư Ȱ��ȭ ��Ȱ���� üũ 

		// �ι���Ż ����
		public string PortalMenuID = "";
		public string PortalMenuXml = "";


		// ���� ����� �˸�â ǥ��
		public int pollNum = 0;

	   	public string _script1;
        // 2012.07.13 : ����� �˾�
        public string _script2;

        // 2014.03.13 : ȸ������ �˾�
        public string _script3;

        //2019.01.18 �빫���ɰź�/���༭ �߰�
        public string _script4;
        public string _script5;

        //topmenu ���̾ƿ� ���� 
        public string TopMNID = "skin";
        public string cookies ;
        public string currSkin = "";

        // ������ ī��Ʈ
        public int pReceiveCnt = 0;
        public int pReceiveGCnt = 0;

        //select box �ʱ�ȭ 
        public string p_Morebox = "false";

        
        string DeptPathCode = "";

        //2013.06.12 : ȣȯ�����⼳�� ȸ�纰�� üũ
        public string pBroserEmulation = "";

        //2016.09.08 ��ü�˾�����
        public string PopupNotice = "NO";

        // ������(20211006) Bizoffice �˾� ����
        public string PopupNotice2 = "NO";

        //20180427 Ÿ�Ժ� css ����
        protected string cssUrl = string.Empty;
        protected string lastLogIn = string.Empty;

        protected string currentUserLang = string.Empty;
		protected string bodyStyle = "tbody"; //20180627 ahh
        protected string BaseType = string.Empty;

        //20180724 �ڵ��α׾ƿ� �߰�
        public string pAutoLogOutUse = "";
        public int pAutoLogOutTime = 0;

        //2020.05.13 ����IP ���� 10�д����� IPüũ�Ͽ� �α׾ƿ� ó��
        public string pIPuse = "";
        //2022.01.17 �̽��� �����۾� �α��� ���� �߰�
        public string inspection_IP = "N";

        protected void Page_Load(object sender, System.EventArgs e)
		{
			CreateUserInfo();

            //20180724 �ڵ��α׾ƿ� �߰�
            GetAutoLogOutTime();

            //2022.01.17 �̽��� �����۾� �α��� ���� �߰�
            if (GetRequestRemoteAddr() == "222.106.242.2")
                inspection_IP = "Y";

            DeptPathCode = userinfo.UserID + ",top," + userinfo.CompanyID + "," + userinfo.DeptID;
            
            mode = "edit";
            			
			Kaoni.ezStandard.ezPortal.TopMenu _ezPortal = new Kaoni.ezStandard.ezPortal.TopMenu();
            Kaoni.ezStandard.ezPortal.PortalPages _ezPortalPage = new Kaoni.ezStandard.ezPortal.PortalPages();

            //2013.06.12 : ȣȯ�����⼳�� ȸ�纰�� üũ
            pBroserEmulation = GetCompanyExpInfo("BROWSEREMULATION", userinfo.CompanyID);
            //2019.04.08 �̽��� XSS ����ó��
            if (ReplaceXSS(Request.QueryString["pageid"], "") != null)
            {
                //2019.04.08 �̽��� XSS ����ó��
                PageID = ReplaceXSS(Request.QueryString["pageid"], "");                
                string _TopMNID = _ezPortal.TopUseSkin(PageID, userinfo.CompanyID);
              
                XmlDocument xmlStr = new XmlDocument();
                xmlStr.LoadXml(_TopMNID);

                //20180530:TopMNID ���� ������ ��� �ε��ӵ��� ������
                if (xmlStr.GetElementsByTagName("TOPMNID").Item(0).InnerText != "")
                    TopMNID = xmlStr.GetElementsByTagName("TOPMNID").Item(0).InnerText;
            }
            else PageID = Guid.NewGuid().ToString();
            //2019.04.08 �̽��� XSS ����ó��
            if (ReplaceXSS(Request.QueryString["parentpageid"], "") != null) ParentPageID = ReplaceXSS(Request.QueryString["parentpageid"], "");
			else 
			{
                //2019.04.08 �̽��� XSS ����ó��
                if (ReplaceXSS(Request.QueryString["pageid"], "") != null) ParentPageID = _ezPortal.GetTopMenuConfigItem(PageID, "ParentUID");
				else ParentPageID = "top";
			}
            //2019.04.08 �̽��� XSS ����ó��
            if (ReplaceXSS(Request.QueryString["mode"], "") != null) mode = ReplaceXSS(Request.QueryString["mode"], "");
			if (mode == "edit") CheckAdmin();

			if (mode == "edit")
			{
                //2019.04.08 �̽��� XSS ����ó��
                if (ReplaceXSS(Request.QueryString["pageid"], "") == null && ReplaceXSS(Request.QueryString["parentpageid"], "") != null) 
				{
                    //2019.04.08 �̽��� XSS ����ó��
                    if (ReplaceXSS(Request.QueryString["parentpageid"], "").Trim() != "" && ReplaceXSS(Request.QueryString["parentpageid"], "").Trim().ToLower() != "top") editmode = "new_inherit";
				}
			}

            lastLogIn = GetLastLogout();

            // �̸�����
            //2019.04.08 �̽��� XSS ����ó��
            viewmode = ReplaceXSS(Request.QueryString["viewmode"], "");

			// �̸������� ��� �ڱ��� ĳ�������� �����Ѵ�.
			if (viewmode == "preview")
			{
                _ezPortalPage.DeleteCacheValue(PageID, this.UserInfoXML);
                
                //�ش� pageid�� basetype �� �����ͼ� skinpath�� ���Ѵ�. 2007-12-02
                string pre_TopMNID = _ezPortal.TopUseSkin(PageID, userinfo.CompanyID);
                XmlDocument xmlSkinpath = new XmlDocument();
                xmlSkinpath.LoadXml(pre_TopMNID);
                TopMNID = xmlSkinpath.GetElementsByTagName("TOPMNID").Item(0).InnerText;
                //2019.04.08 �̽��� XSS ����ó��
                if (ReplaceXSS(Request.QueryString["call"], "") == "client") // ����� ȯ�漳�������� �̸�����
                { 
                    //����ϴ� ��������� userinfo ���̺��� skinid�� �����´�. 2007-12-02
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

            // topmenu��Ų �� ����
            //2019.04.08 �̽��� XSS ����ó��
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

            //��Żȭ�� ��Ų���� 
            //2019.04.08 �̽��� XSS ����ó��
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


            //����üũ_2007-09-18 �߰� 
            result = _ezPortal.ezACL_Check(userinfo.UserID, userinfo.CompanyID, userinfo.CompanyName); //1:�����ī������, 2:ȸ�������, 3:�Ϲݻ����.          
            
            string CK_AdminACL = "";
            string CK_AdminACL2 = "";

            if (result == "3")
            {
                //2017.05.30 ������ ���� �ƴ����� üũ�Ͽ� �����ī Ȩ ��ư�� Ȱ��ȭ
                CK_AdminACL2 = ezCk_AdminACL2(userinfo.UserID, PageID, result);

                //2017.06.19 ������ ��� �и� �۾�
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
                
                // ���� ���� ���� (id �� pageid �� ) 
                CK_AdminACL = _ezPortal.ezCk_AdminACL(userinfo.UserID, PageID, result);
            }
            else
            {
                // üũ ���� (id�� pageid ���� �޾Ƽ� .. )
                CK_AdminACL = _ezPortal.ezCk_AdminACL(userinfo.UserID, PageID, result);
                //2017.05.30 ������ ���� �ƴ����� üũ�Ͽ� �����ī Ȩ ��ư�� Ȱ��ȭ
                CK_AdminACL2 = ezCk_AdminACL2(userinfo.UserID, PageID, result);
            }    
            

			// 20071025
			// ex) �ѱ��� ��� skin1 ����
			//     ������ ��� skin1_2 ����
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

            //20071031_ �Ѱ��� top���̾ƿ��� ���� �ϴ� ���� �ƴ϶� �پ� �ϰ� ���� �ϱ� ���� ����ϴ� skin ������ �����Ѵ�.
            //ex) ǥ�ظ���� ��� skin 
            //    �����ī �ű��� ��� skin1

            
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


                

			// ���θ����
			if (mode == "new")
			{
				strHTML = _ezPortal.GetDefaultTopMenu();
			}
			else	// ����: ����HTML, width, height������ �����´�
			{
                if (editmode == "new_inherit")
                {   //ĳ�������Ҷ� ȸ����̵� ��ϵǵ��� ���� 2007-10-30                   
                    strHTML = _ezPortal.GetRenderedTopMenuHTML(ParentPageID, DeptPathCode, mode, skinnum, UserInfoXML, userinfo.CompanyID, userinfo.UserID);
                    width = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(ParentPageID), "width");
                    height = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(ParentPageID), "height");
                }
                // mode: view, edit
                else
                {
                    ////test 20180423
                    //if(userinfo.UserID == "dev03@withkt") Response.Redirect("/myoffice/ezPortal/TopMenu_renewal.aspx?skinnum=" + currSkin + "&TOPMNID=" + TopMNID, false);  //���� topmenu �׽�Ʈ�� 

                    //�빮�� ȸ����̵��� ȸ�縦 ���� �߰� 2008-10-22
                    if (PageID == "simple@" + userinfo.CompanyID.ToLower() || PageID == "simple@" + userinfo.CompanyID.ToUpper()) //userinfo���̺� ����Ǿ� �ִ� pageuid�� ������ ���
                    {
                        Response.Redirect("/myoffice/ezPortal/simple_TopMenu_Cross.aspx?skinnum=" + currSkin + "&TOPMNID=" + TopMNID, false);  //���� topmenu �׽�Ʈ�� 
                    }
                    else
                    {
                        //ĳ�� �����Ҷ� ȸ�� ���̵� ��ϵǵ��� ���� 2007-10-30
                        strHTML = _ezPortal.GetRenderedTopMenuHTML(PageID, DeptPathCode, mode, skinnum, UserInfoXML, userinfo.CompanyID, userinfo.UserID);
                        width = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(PageID), "width");
                        height = _ezPortal.GetTopMenuConfigItem(_ezPortal.GetTopParentPageID(PageID), "height");

                        //WriteTextLog("ddd", _ezPortal.GetMenuItemHTML(PageID, _ezPortal.GetTopParentPageID(PageID),UserInfoXML), "ezPortal");

                        //20180426 Ÿ�� 1,2 ����
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
                            //2021.04.21 �̽��� N��Ż HŸ�� �߰��� ó��_�۾���
                            else if (BaseType.ToUpper().Equals("TYPE4") == true)
                            {
                                cssUrl = "/css/type4/main_cross.css";
                            }
                            else
                            {
                                cssUrl = "/css/type3/main_cross.css";								
                            }
                        }

                        //2021.12.07 ������ǽ� �б�ó�� - �ΰ� �б�ó��
                        SqlCommand cmd_l = new SqlCommand(" SELECT * FROM [DBO].[COMPANYCLOUDINFO] WITH(NOLOCK) WHERE (MEMSQ LIKE 'M%' OR MEMSQ LIKE 'T%') and CompanyID=@PCOMPANYID ");
                        cmd_l.CommandType = CommandType.Text;
                        cmd_l.Parameters.Add("@PCOMPANYID", SqlDbType.VarChar, 20).Value = userinfo.CompanyID;
                        string result_l = GetQueryResultSP(ref cmd_l, "entumadmin", false);
                        cmd_l.Dispose();
                        cmd_l = null;
                        XmlDocument xmldom_l = new XmlDocument();
                        xmldom_l.LoadXml(result_l);

                        //2021.12.07 ������ǽ� �б�ó�� - �ΰ� �б�ó��
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
			

			// ����� ���������� �˾� ���������� �����Ѵ�.
			if (mode == "view" && viewmode != "preview")
			{
				// �˾� ��������
                //ezPersonal.PopUp _ezPersonal = new ezPersonal.PopUp();
                //string infoXML = _ezPersonal.GetPopUpListUser(userinfo.CompanyID);
                //_ezPersonal.Dispose();
                //_ezPersonal = null;

                //XmlDocument xmldom = new XmlDocument();
                //xmldom.LoadXml(infoXML);

                // �˾� �������� _ Sp �� ���������� ���� 2008-08-29
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

                //2019.03.19 �̽��� �˾� �������� ���� �� ���� ������ ���� �� �˾��� �� ��ġ������ ������ ������ ���� ����
                int openTop = 200; int openLeft = 250;

                for (int i=0; i<xmldom.DocumentElement.ChildNodes.Count; i++)
				{
					string itemseq = xmldom.DocumentElement.ChildNodes.Item(i).SelectSingleNode("ITEMSEQ").InnerText;

                    if (Request.Cookies["POPUP_" + itemseq] == null || Request.Cookies["POPUP_" + itemseq].Value == "")
					{
						string popupWidth = xmldom.DocumentElement.ChildNodes.Item(i).SelectSingleNode("WIDTH").InnerText;
						string popupHeight = xmldom.DocumentElement.ChildNodes.Item(i).SelectSingleNode("HEIGHT").InnerText;
                        // 20111024 : �˾� �������� �������� ó��, resizable=0 -> 1
                        //2019.03.19 �̽��� �˾� �������� ���� �� ���� ������ ���� �� �˾��� �� ��ġ������ ������ ������ ���� ����
                        //popup += "window.open('/myoffice/ezPersonal/PopUp/ShowPopUp.aspx?itemseq=" + itemseq + "', '', 'height=" + popupHeight + "px,width=" + popupWidth + "px,top=200px,left=250px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";
                        popup += "window.open('/myoffice/ezPersonal/PopUp/ShowPopUp.aspx?itemseq=" + itemseq + "', '', 'height=" + popupHeight + "px,width=" + popupWidth + "px,top="+ openTop + "px,left="+ openLeft + "px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";

                        //2019.03.19 �̽��� �˾� �������� ���� �� ���� ������ ���� �� �˾��� �� ��ġ������ ������ ������ ���� ����
                        openTop += 15; openLeft += 100;
                    }
				}
				xmldom = null;

                //2021.11.17 �����ī ��ü���� ��� ����
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
			
				// ǥ�ظ�� (2007.03.15) ����: .NET Framework 2.0������ RegisterStartupScript �޼��� �������� ����.
				if (popup != "")
					_script1 = "<script language='javascript'>" + popup + "</script>";
					//Page.RegisterStartupScript("PopUp", "<script>" + popup + "</script>");
                // �˾� �������� ��

                /**** 2012.07.13 : ������˾� ���� ****/
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
                    //2012.08.02 : ����� �˾�ũ�� ��ɰ���
                    string AnnivarsaryWidth = xmldoc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("WIDTH").InnerText;
                    string AnnivarsaryHeight = xmldoc.DocumentElement.ChildNodes.Item(i).SelectSingleNode("HEIGHT").InnerText;

                    if (Request.Cookies["POPUP_" + schID] == null ||
                        Request.Cookies["POPUP_" + schID].Value == "")
                    {
                        popupAnnivarsary += "window.open('/myoffice/ezSchedule/ShowPopUpAnniversary.aspx?itemseq=" + schID + "', '', 'height=" + AnnivarsaryHeight + "px,width=" + AnnivarsaryWidth + "px,top=200px,left=250px,  status = no, toolbar=no, menubar=no,location=no, resizable=1');";
                    }
                }
                xmldoc = null;

                // ǥ�ظ�� (2007.03.15) ����: .NET Framework 2.0������ RegisterStartupScript �޼��� �������� ����.
                if (popupAnnivarsary != "")
                    _script2 = "<script language='javascript'>" + popupAnnivarsary + "</script>";
                /**** 2012.07.13 : ������˾� �� ****/

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /**** 2014.03.13 : ȸ������ �˾� ���� ****/
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
                /**** 2014.03.13 : ȸ������ �˾� �� ****/
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //2019.01.18 �빫���ɰź�/���༭ �߰�
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
			
			// �н����� ��ȣȭ
			_password = EncryptString(Request.ServerVariables["AUTH_PASSWORD"].Trim());
						
			// ��Ų����
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



            //����ϴ� toppage�� main�޴� ������ ������ 9�� ���� ũ�� true ���� �־��ش�. 
            string pMainXml = "";
            string p_Count = "";

            XmlDocument xmlACL = new XmlDocument();


            if (PageID != "simple@" + userinfo.CompanyID.ToLower() || PageID != "simple@" + userinfo.CompanyID.ToUpper()) //�빮�� ȸ����̵��� ȸ�縦 ���� �߰� 2008-10-22
            {
                if (editmode == "new_inherit")
                {
                    pMainXml = _ezPortal.GetMainMenu_ACLCount(ParentPageID, DeptPathCode);
                }
                else
                {
                    if (PageID != "simple@" + userinfo.CompanyID.ToLower() || PageID != "simple@" + userinfo.CompanyID.ToUpper()) //�빮�� ȸ����̵��� ȸ�縦 ���� �߰� 2008-10-22
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

            // 20071030 table fixed ����
            strHTML = strHTML.Replace("table-layout:fixed;", "");
            strHTML = strHTML.Replace("img_onMouseOver(", "img_onMouseOver(event,");
            strHTML = strHTML.Replace("img_onMouseOut()", "img_onMouseOut(event)");
            strHTML = strHTML.Replace("index_environment.htm", "index_environment_Cross.htm");
            strHTML = strHTML.Replace("myportal.aspx", "myportal_Cross.aspx");

            //20180724:�ڵ��α׾ƿ� �ӽ� �׽�Ʈ
            if(pAutoLogOutUse == "Y")
                strHTML = strHTML.Replace("<span id='spanLastLogin'></span>", "<span id='spanLastLogin'></span><span id='spanAutoLogout' class='logoutAuto'></span>");

            currentUserLang = GetUserLang();

            string tempHtml = ChangeLocalString(strHTML);

            if (string.IsNullOrEmpty(tempHtml) == false)
            {
                strHTML = tempHtml;
            }

            // 2008.05.13
            // ���� �������ʴ� �˾��� �α��� ������ �����ֱ�
            //pReceiveCnt = GetReceiveCount(); 20081113 �ּ�ó�� ���Ϸ� ���� üũ ���ϵ��� ��. 
            // ���� �����ڱ׷��ʴ� �˾��� �α��� ������ �����ֱ�
            //pReceiveGCnt = GetReceiveGropuCount(); 20081113 �ּ�ó�� ���Ϸ� ���� üũ ���ϵ��� ��. 

            // 2016.09.08 ��ü�˾�����
            string pToday = DateTime.Now.ToString("yyyy-MM-dd");
            if (DateTime.Now > DateTime.Parse("2016-09-08 18:00:00") && DateTime.Parse(pToday) < DateTime.Parse("2016-09-17 23:00:00"))
            {
                PopupNotice = "OK";
            }

            // ������(20211006) Bizoffice �˾� ����
            if (DateTime.Now > DateTime.Parse("2021-10-05 11:00:00") && DateTime.Parse(pToday) < DateTime.Parse("2021-10-20 23:00:00"))
            {
                PopupNotice2 = "OK";
            }

            //2020.05.13 ����IP ���� 10�д����� IPüũ�Ͽ� �α׾ƿ� ó��
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

        //20180515 ����� ��� ���� �� ���
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

        //20180511 �� �� �ؽ�Ʈ ����
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

		#region Web Form �����̳ʿ��� ������ �ڵ�
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �� ȣ���� ASP.NET Web Form �����̳ʿ� �ʿ��մϴ�.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
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


            // 20070530 TimeZone ó��
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

        //20180724 �ڵ��α׾ƿ� �߰�
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

        //2017.05.30 ������ ���� �ƴ����� üũ�Ͽ� �����ī Ȩ ��ư�� Ȱ��ȭ
        public string ezCk_AdminACL2(string pCN, string pPageID, string pACL)
        {
            try
            {
                string strSQL = ""; //UID�� ������
                string tmpSQL = ""; //���� ó�� 

                strSQL = "Select UID FROM TBL_MenuItem_Items_MenuItems \n";
                strSQL += "WHERE ownerpageid = @pPageID and parentuid='202' AND DISPLAYNAME='�����ī Ȩ'";

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

