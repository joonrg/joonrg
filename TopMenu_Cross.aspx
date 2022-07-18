<%@ Page Language="c#" Inherits="Kaoni.ezStandard.Web.ezPortal.TopMenu" CodeFile="TopMenu_Cross.aspx.cs" EnableViewStateMac="false" %>

<%@ OutputCache Duration="1" Location="None" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>TopMenu</title>

    <%
        // 수정(2007.03.22) : 스킨설정 적용되도록 수정
        string strCssFile = "top1.css";

        if (mode != "view")
        {
    %>
    <!-- 관리자 -->
    <link href="/css/<%=RM.GetString("i2_c")%>" rel="stylesheet" type="text/css">
    <%
        }
        else
        {
            switch (skinnum)
            {
                case "1":
                    strCssFile = "top1.css";
                    break;
                case "2":
                    strCssFile = "top2.css";
                    break;
                case "3":
                    strCssFile = "top3.css";
                    break;
                case "7":
                    strCssFile = "top4.css";
                    break;
            }
    %>

    <!-- 서브메뉴 스킨설정 -->
    <style>
        .menu2 {
            width: 100%;
            height: 26px;
            <%= SkinBgString %>
        }

        A:link {
            FONT-SIZE: 9pt;
            COLOR: <%= SkinFontColor %>;
            TEXT-DECORATION: none
        }

        A:visited {
            FONT-SIZE: 9pt;
            COLOR: <%= SkinFontColor %>;
            TEXT-DECORATION: none
        }

        A:active {
            FONT-SIZE: 9pt;
            COLOR: <%= SkinFontColor %>;
            TEXT-DECORATION: none
        }

        A:hover {
            FONT-SIZE: 9pt;
            COLOR: <%= SkinFontOverColor %>;
            TEXT-DECORATION: none
        }
    </style>
    <% } %>
    <link href="/<%=TopMNID%>/skin_<%= currSkin %>/skin.css" rel="stylesheet" type="text/css">
    <link href="<%= cssUrl %>" rel="stylesheet" type="text/css">
    <script type="text/javascript" src="js/string_component.js"></script>
    <script type="text/javascript" src="js/functionLib.js"></script>
    <script type="text/javascript" src="/myoffice/common/XmlHttpRequest.js"></script>
    <script type="text/javascript">

        var skinnum = "<%=skinnum%>";

        var selectedCell = "";
        var selectedSubCell = "";
        var previousSubCell = null;
        var previousCell = null;
        var count = 1000;
        var pageid = "<%= PageID %>";
        var parentpageid = "<%= ParentPageID %>";
        var mode = "<%= mode %>";
        var editmode = "<%= editmode %>";
        var viewmode = "<%= viewmode %>";
        var bInherit = false;
        var pressCount = 0;
        var selObjClass = "";
        var SkinExist = "<%= SkinExist %>";
        var pReceiveCnt = "<%= pReceiveCnt %>";
        var pReceiveGCnt = "<%= pReceiveGCnt %>";
        var p_Morebox = "<%=p_Morebox%>";

        //2016.09.08 전체팝업공지
        var popupNotice = "<%=PopupNotice%>";

        // 정은하(20211006) Bizoffice 팝업 공지
        var popupNotice2 = "<%=PopupNotice2%>";

        var QuickcurNum = 0;
        var QuickBlockNum = 8;

        //20180724 자동로그아웃 추가
        var pAutoLogOutUse = "<%=pAutoLogOutUse%>";
        var pAutoLogOutTime = "<%=pAutoLogOutTime%>";

        //20180823 서버이름 표시 추가
        var ServerName = "<%=Server.MachineName.Substring(Server.MachineName.Length - 5, 5)%>";

        //2020.05.13 접속IP 차단 10분단위로 IP체크하여 로그아웃 처리
        var pIPuse = "<%=pIPuse%>";

        //2022.05.18 yn 타임존 처리 
        var userOffsetHour = "<%=userinfo.Offset.Split('|')[1].Split(':')[0]%>"; //2020.07.07 이승중 근태관리 출퇴근/ 기타근태 정보 타임존 처리
        var userOffsetMin = "<%=userinfo.Offset.Split('|')[1].Split(':')[1]%>"; //2020.07.07 이승중 근태관리 출퇴근/ 기타근태 정보 타임존 처리

        window.onload = function ()
        {
            // 20180327 스크립트 표준화try
            //{
            // var pAlertCheck = false;

            // // 20100219 : ezIcd에 IE Setting 기능 추가
            // var TrustSite = icdbho.IsStartIESetting(document.location.protocol + "//" + document.location.hostname, "0");

            // if (TrustSite)
            // {
            //  icdbho.excuteIESetting(document.location.protocol + "//" + document.location.hostname);
            //  pAlertCheck = true;
            // }

            //          //20180129 http -> document.location.protocol 로 수정
            //          //var TrustSite2010 = icdbho.IsStartIESetting("http://gwx.bizmeka.com", "0");
            //          var TrustSite2010 = icdbho.IsStartIESetting(document.location.protocol + "//" +"gwx.bizmeka.com", "0");

            // if (TrustSite2010)
            //          {
            //              //20180129 http -> document.location.protocol 로 수정
            //              //icdbho.excuteIESetting("http://gwx.bizmeka.com");
            //              icdbho.excuteIESetting(document.location.protocol + "//" + "gwx.bizmeka.com");
            // }

            // if (pAlertCheck)
            //  alert("인터넷 익스플로러 설정이 변경되었습니다. 재시작하여 주십시오.");
            //} catch (e) { }

            if (editmode == "new_inherit") bInherit = true;
            if (mode == "edit") AttachEvents(main_table);

            // 2008.05.13   참석자 초대 팝업 관련 추가
            if (pReceiveCnt != "0")
            {
                var ret = window.showModalDialog("/myoffice/ezSchedule/schedule_receive_attendant.aspx", this, "dialogHeight:420px; dialogWidth:730px; status:no;scroll:no; help:no; edge:sunken");
            }

            if (pReceiveGCnt != "0")
            {
                var ret = window.showModalDialog("/myoffice/ezSchedule/schedule_receive_member.aspx", this, "dialogHeight:420px; dialogWidth:730px; status:no;scroll:no; help:no; edge:sunken");
            }

            // 검색UI 설정
            try
            {
                if (typeof (searchTD) == "object")
                {
                    searchTD.parentNode.parentNode.parentNode.style.width = "100%";
                    searchTD.innerHTML = "<table border=\"0\" height=\"41\" cellspacing=\"0\" cellpadding=\"0\"><tr>" +
                        "<td width=\"44\"   height=\"40\" valign=\"middle\"><img src=\"/images/main/totalsearch.gif\" align=\"absmiddle\"></td>" +
                        "<td width=\"140\"   height=\"40\" valign=\"middle\">   <input name=\"txtSearch\" type=\"text\"  class=\"search\" onkeydown=entercheck()></td>" +
                        /*	"<td width=\"184\" valign=\"middle\"><img src=\"/images/main/totalsearch.gif\" align=\"absmiddle\"><input name=\"txtSearch\" type=\"text\" class=\"search\" onkeydown=entercheck()></td>" +*/
                        "<td width=\"31\"  height=\"40\" valign=\"middle\"><img src=\"/images/top/bt_search.gif\" width=\"31\" height=\"21\" style='cursor:pointer' onclick=Search()></td>" +
                        "</tr></table>";
                }
            }
            catch (e) { }

            // 수정(2007.03.14) : 사용자 정보 영역 UI 설정
            try
            {
                if (typeof (userInfoTD) == "object")
                {
                    userInfoTD.parentNode.parentNode.parentNode.style.marginTop = "0px";
                    userInfoTD.innerHTML = "<iframe width=300 height=31 border=0 src='/myoffice/ezPortal/filter/UserInfoPortlet.aspx' frameborder=0 scrolling=no marginheight=0 marginwidth=0></iframe>";
                }
            }
            catch (e) { }


            // 보기모드에서 미리보기가 아닌 경우 실행
            if (mode == "view" && viewmode != "preview")
            {
                var agentObj;
                //20180328 스크립트 표준화
                //         if (!CrossYN()) {
                //             GetObject();
                //    ezNotieSetting();
                //    pBorseEmulationSet();
                //}
                window.setInterval("update_connectinfo()", 30000);
            }

            //2016.09.08 전체팝업공지
            if (popupNotice == "OK" && GetCookie("popupNotice_20160908") == "")
            {
                var pos6x = 380;
                var pos6y = 300;
                window.open("/myoffice/ezPersonal/PopUp/popupNotice_20160908.aspx", "", "top=" + pos6y + ",left=" + pos6x + ",width=672,height=650,scrollbars=no");
            }

            // 정은하(20211006) Bizoffice 팝업 공지
            if (popupNotice2 == "OK" && (<%=userinfo.RollInfo.IndexOf("c=1")%> > -1 || <%=userinfo.RollInfo.IndexOf("k=1")%> > -1) && GetCookie("popupNotice_20211006") == "")
            {
                var pos6x = 380;
                var pos6y = 160;
                window.open("/myoffice/ezPersonal/PopUp/popupNotice_20211006.aspx", "", "top=" + pos6y + ", left=" + pos6x + ", width=700, height=738, scrollbars=no");
            }

            //20180427 사용자 정보 스크립트 추가
            if (document.getElementById("pUserInfo") != null)
            {

                if ("<%=userinfo.lang%>" == "1")
                {
					//20180823 서버이름 표시 추가
					//document.getElementById("pUserInfo").innerHTML = "<%=userinfo.DisplayName%>" + " " + "<%=userinfo.Title%>" + document.getElementById("pUserInfo").innerHTML;
                    document.getElementById("pUserInfo").innerHTML = "<%=userinfo.DisplayName%>" + " " + "<%=userinfo.Title%>" + document.getElementById("pUserInfo").innerHTML + "<div style='float:right; margin-left:10px; color: White'><%=Server.MachineName.Substring(Server.MachineName.Length - 5, 5)%></div>";
                }
                else
                {
					//20180823 서버이름 표시 추가
					//document.getElementById("pUserInfo").innerHTML = "<%=userinfo.DisplayName1%>" + " " + "<%=userinfo.Title1%>" + document.getElementById("pUserInfo").innerHTML;
                    document.getElementById("pUserInfo").innerHTML = "<%=userinfo.DisplayName1%>" + " " + "<%=userinfo.Title1%>" + document.getElementById("pUserInfo").innerHTML + "<p style='float:right; margin-left:10px; color: White'><%=Server.MachineName.Substring(Server.MachineName.Length - 5, 5)%></p>";
                }

                if (document.getElementById("spanLastLogin") != null)
                {

                    var nd = new Date("<%=lastLogIn%>");
                    nd.setHours(nd.getHours() + userOffsetHour);
                    nd.setMinutes(nd.getMinutes() + nd.getTimezoneOffset());
                    if (userOffsetHour < 0)
                        nd.setMinutes(nd.getMinutes() - userOffsetMin);
                    else
                        nd.setMinutes(nd.getMinutes() + userOffsetMin);

                    var tmpDate = new Date();
                    var tmpSec = nd.getSeconds();
                    tmpDate.setHours(nd.getHours());
                    tmpDate.setMinutes(nd.getMinutes());   

                    document.getElementById("spanLastLogin").innerHTML = "<%=lastLogIn%>";
                }

                if (pAutoLogOutUse == "Y")
                {
                    //20180724:자동로그아웃 임시 테스트
                    window.setInterval("GetAutoLogout()", 1000);
                }
                
                <%//2022.01.17 이승중 서버작업 로그인 차단 추가%>
                let now = new Date();
                let endCheck_Date = new Date("2022-01-21T06:00:00") 
                if (now.getTime() < endCheck_Date.getTime() && "<%=inspection_IP%>" == "N")
                {
                    window.setInterval("inspection_logout()", 10000);
                }
            }

            var tempUserLang = "<%= currentUserLang %>";

            if ("<%=BaseType%>" == "TYPE1")
            {
                if (tempUserLang == "1")
                {
                    document.getElementById("input_search").style.background = "url(/images/type1/basic/input_search_bg.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "2")
                {
                    document.getElementById("input_search").style.background = "url(/images/type1/basic/input_search_bg_en.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "3")
                {
                    document.getElementById("input_search").style.background = "url(/images/type1/basic/input_search_bg_ja.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "4")
                {
                    document.getElementById("input_search").style.background = "url(/images/type1/basic/input_search_bg_zh.gif) no-repeat 0 0";
                }
            }
            else if ("<%=BaseType%>" == "TYPE2")
            {
                if (tempUserLang == "1")
                {
                    document.getElementById("input_search").style.background = "url(/images/type2/basic/input_search_bg.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "2")
                {
                    document.getElementById("input_search").style.background = "url(/images/type2/basic/input_search_bg_en.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "3")
                {
                    document.getElementById("input_search").style.background = "url(/images/type2/basic/input_search_bg_ja.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "4")
                {
                    document.getElementById("input_search").style.background = "url(/images/type2/basic/input_search_bg_zh.gif) no-repeat 0 0";
                }
            }
            else if ("<%=BaseType%>" == "TYPE3")
            {
                document.getElementById("top").className = "mainbg";
                if (tempUserLang == "1")
                {
                    document.getElementById("input_search").style.background = "url(/images/type3/basic/input_search_bg.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "2")
                {
                    document.getElementById("input_search").style.background = "url(/images/type3/basic/input_search_bg_en.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "3")
                {
                    document.getElementById("input_search").style.background = "url(/images/type3/basic/input_search_bg_ja.gif) no-repeat 0 0";
                }
                else if (tempUserLang == "4")
                {
                    document.getElementById("input_search").style.background = "url(/images/type3/basic/input_search_bg_zh.gif) no-repeat 0 0";
                }
            }

            document.getElementById("input_search").style.display = "";

            //2019.01.18 노무수령거부/서약서 추가
            if ("<%=_script4%>" == "Y")
                parent.vow_popup("", "Y");
            else if ("<%=_script5%>" != "")
                parent.vow_popup("<%=_script5%>", "N");

            //2020.05.13 접속IP 차단 10분단위로 IP체크하여 로그아웃 처리
            if (pIPuse == "Y")
            {
                setInterval(function() {
                var xmlHTTP = createXMLHttpRequest();
                var xmlRtn = createXmlDom();
                var xmlpara = createXmlDom();

                var objNode;
                createNodeInsert(xmlpara, objNode, "DATA");
                createNodeAndInsertText(xmlpara, objNode, "pIPuse", pIPuse);

                xmlHTTP.open("POST", "/myoffice/ezPortal/GetIPuse.aspx", false);
                xmlHTTP.send(xmlpara);

                xmlRtn = loadXMLString(xmlHTTP.responseText);

                var dataNodes = GetChildNodes(xmlRtn);
                result = getNodeText(dataNodes[0]);
                    //alert(result);
                if (result != "Y")
                {
                    window.open("/logout.aspx", "top", "");
                }
                }, 600000);
            }
        }

        //20180724:자동로그아웃 임시 테스트
        function GetAutoLogout()
        {
            if (document.getElementById("spanAutoLogout") != null)
            {
                var today = new Date();
                var t = get_cookie('LOGOUT');
                var timespan = 0;
                if (t != '') timespan = Math.floor((today.getTime() - t) / 1000);

                //2시간
                //var cur = 30 - timespan;
                var cur = pAutoLogOutTime - timespan;

                var min = parseInt(cur / 60, 10);
                var second = parseInt(cur % 60, 10);
                if (second < 10) second = '0' + second;

                document.getElementById("spanAutoLogout").innerHTML = min + ':' + second;

                if (cur < 1)
                {
                    //alert("자동 로그아웃 하였습니다.");
                    //alert(t+','+timespan+","+cur);
                    window.top.location.href = "/Logout.aspx";
                }
            }
        }

        function GetCookie(name)
        {
            var nameOfCookie = name + "=";
            var x = 0;
            while (x <= document.cookie.length)
            {
                var y = (x + nameOfCookie.length);
                if (document.cookie.substring(x, y) == nameOfCookie)
                {
                    if ((endOfCookie = document.cookie.indexOf(";", y)) == -1)
                        endOfCookie = document.cookie.length;
                    return unescape(document.cookie.substring(y, endOfCookie));
                }
                x = document.cookie.indexOf(" ", x) + 1;
                if (x == 0)
                    break;
            }
            return "";
        }

        function pBorseEmulationSet()
        {
            //20180404 스크립트 표준화
	       <%-- //2013.06.12 : 호환성보기설정 회사별로 체크
	        if("<%=pBroserEmulation%>"!="Y"){
	            //20130604:폼프로세서 문제, 호환성처리, IE9/IE10
	            try {
	                var ver = navigator.userAgent;
	                if (ver.indexOf("MSIE 7.0") > -1 || ver.indexOf("MSIE 8.0") > -1 || ver.indexOf("MSIE 9.0") > -1 || ver.indexOf("MSIE 10.0") > -1)
	                {
	                    var ezUtil = new ActiveXObject("ezUtil.RegScript");
	                    if(ezUtil.ReadValueEx(1, "Software\\Microsoft\\Internet Explorer\\BrowserEmulation", "AllSitesCompatibilityMode") != "1")
	                        ezUtil.WriteValueEx(1, "Software\\Microsoft\\Internet Explorer\\BrowserEmulation", "AllSitesCompatibilityMode", "1");
	                }
	            } catch (e) {}
	        }--%>
        }
        function ezNotieSetting()
        {
            //20180404 스크립트 표준화
	         <%--   var g_serverpath = document.location.protocol + "//" + document.location.hostname + "/LoginToRedirect.aspx"; try {
	                var ezUtil = new ActiveXObject("ezUtil.MiscFunc");
	                ezUtil.ExecuteNoti3("", "<%= userinfo.UserID %>", pwd, "", g_serverpath);
	                ezUtil = null;
                } catch (e) {
                }--%>
        }

        function GetObject()
        {
            var pComponentlist = "componentlist_transfer.aspx";
            var pProgress = "Progress.aspx";
            if ("<%=isBrowserCheck()%>".toLowerCase() == "true" && "<%=isCrossBrowser(userinfo.CompanyID)%>".toLowerCase() == "true")
            {
                pComponentlist = "componentlist_transfer_cross.aspx";
                pProgress = "Progress_cross.aspx";
            }

            var agentObj;
            i_icd2.SetDocumentDisp(window.document);
            //20180129 http -> document.location.protocol 로 수정
            //i_icd2.xmlURL = "http://" + document.location.hostname + "/binary/" + pComponentlist;
            i_icd2.xmlURL = document.location.protocol + "//" + document.location.hostname + "/binary/" + pComponentlist;
            i_icd2.CheckVersion();
            var nCount = i_icd2.nNeedDownload;
            if (nCount)
            {
                //if_Progress.StartOn();
                window.showModalDialog("/binary/" + pProgress, "", "scroll:no;status:no;dialogHeight:140px;dialogWidth:395px;");
            }
        }

        var xmlHTTP = null;
        var blogout = false;
        function OpenInformationUI(pInformationContent)
        {
            var parameter = pInformationContent;
            var url = "/myoffice/ezApproval/ezAPROPINION.htm";
            var feature = "status:no;dialogWidth:330px;dialogHeight:180px;help:no;scroll:no;edge:sunken";
            var RtnVal = window.showModalDialog(url, parameter, feature);
            return RtnVal;
        }

        function load()
        {
            var ret = window.showModalDialog("TopMenu_search.aspx?mode=load");
            if (typeof (ret) == "undefined") return;

            document.location.href = "TopMenu_Cross.aspx?pageid=" + ret[0];
        }

        function inherit()
        {
            var ret = window.showModalDialog("TopMenu_search.aspx?mode=inherit");
            if (typeof (ret) == "undefined") return;

            document.location.href = "TopMenu_Cross.aspx?parentpageid=" + ret[0];
        }

        function savesub(pObject, pPageID, pParentPageID, pDisplayName)
        {
            var strXML = "<DATA>";
            strXML += "<DISPLAYNAME>" + pDisplayName + "</DISPLAYNAME>";
            strXML += "<WIDTH>" + GetAttribute(pObject, "width").toString().replace("px", "").replace("100%", "-1") + "</WIDTH>";
            strXML += "<HEIGHT>" + GetAttribute(pObject, "height").toString().replace("px", "").replace("100%", "-1") + "</HEIGHT>";
            strXML += "<PARENTPAGEID>" + pParentPageID + "</PARENTPAGEID>";

            // 대상테이블의 최상위td count
            for (var i = 0; i < pObject.children.item(0).children.item(0).children.length; i++)
            {
                // 최상위td
                if (pObject.children.item(0).children.item(0).children.item(i).id == "") continue;
                if (pObject.children.item(0).children.item(0).children.item(i).id.substr(0, 2) == "td")
                {
                    strXML += "<CELL>";
                    var td_item = pObject.children.item(0).children.item(0).children.item(i);
                    strXML += "<WIDTH>" + td_item.style.width.toString().replace("px", "") + "</WIDTH>";

                    // 해당td내의 tr의 카운트 (TABLE/TBODY/TR)
                    for (var j = 0; j < td_item.children.item(0).children.item(0).children.length; j++)
                    {
                        // 해당 tr내의 td
                        var tdsub_item = td_item.children.item(0).children.item(0).children.item(j).children.item(0);

                        if (tdsub_item.id == "") continue;

                        // td안에 컨텐츠가 존재하는 경우
                        if (tdsub_item.children.length > 0 && tdsub_item.children.item(0).id.toLowerCase().substr(0, 4) != "main")
                        {
                            strXML += "<ROW>";
                            strXML += "<TYPE>0</TYPE>";
                            strXML += "<UID>" + GetAttribute(tdsub_item, "uid") + "</UID>";
                            strXML += "<PAGEUID>" + GetAttribute(tdsub_item, "pageuid") + "</PAGEUID>";
                            strXML += "<HEIGHT>" + tdsub_item.parentElement.style.height.toString().replace("px", "") + "</HEIGHT>";
                            strXML += "<DISPLAYNAME>" + tdsub_item.innerText + "</DISPLAYNAME>";
                            strXML += "<CANREMOVE>" + GetAttribute(tdsub_item, "canremove") + "</CANREMOVE>";
                            strXML += "<CANRESIZE>" + GetAttribute(tdsub_item, "canresize") + "</CANRESIZE>";
                            strXML += "<CANREPLACE>" + GetAttribute(tdsub_item, "canreplace") + "</CANREPLACE>";
                            strXML += "<ROOTPAGEID>" + pageid + "</ROOTPAGEID>";
                            strXML += "</ROW>";
                        }
                        // td안에 테이블이 존재하는 경우
                        else
                        {
                            strXML += "<ROW>";
                            strXML += "<TYPE>1</TYPE>";
                            strXML += "<UID>" + GetAttribute(tdsub_item, "uid") + "</UID>";
                            strXML += "<PAGEUID>" + GetAttribute(tdsub_item, "pageuid") + "</PAGEUID>";
                            strXML += "<HEIGHT>" + tdsub_item.parentElement.style.height.toString().replace("px", "") + "</HEIGHT>";
                            strXML += "<DISPLAYNAME>" + GetAttribute(tdsub_item, "pageuid") + "</DISPLAYNAME>";
                            strXML += "<CANREMOVE>" + GetAttribute(tdsub_item, "canremove") + "</CANREMOVE>";
                            strXML += "<CANRESIZE>" + GetAttribute(tdsub_item, "canresize") + "</CANRESIZE>";
                            strXML += "<CANREPLACE>" + GetAttribute(tdsub_item, "canreplace") + "</CANREPLACE>";
                            strXML += "<ROOTPAGEID>" + pageid + "</ROOTPAGEID>";
                            strXML += "</ROW>";

                            // 하위테이블의 정보를 저장
                            savesub(tdsub_item.children.item(0), GetAttribute(tdsub_item, "uid"), "top", GetAttribute(tdsub_item, "uid"));
                        }
                    }
                    strXML += "</CELL>";
                }
            }
            strXML += "</DATA>";

            //20180404 스크립트 표준화
            //var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            var xmlhttp = createXMLHttpRequest();
            //20190820 이행점검 수정 및 추가
            xmlhttp.open("POST", "general/remote/portal_SaveTopMenu.aspx?pageid=" + pPageID + "&parentpageid=" + pParentPageID, false);
            xmlhttp.send(strXML);
            xmlhttp = null;
        }

        function save()
        {
            if (txtDisplayName.value == "")
            {
                alert("<%=RM.GetString("t361")%>");
                txtDisplayName.focus();
                return;
            }

            savesub(main_table, pageid, parentpageid, txtDisplayName.value);

            // 스킨정보 생성
            if (SkinExist == "NO")
                SaveSkin(pageid);

            //저장시 캐쉬 삭제 되도록 수정 2007-12-01

            var xmlhttp = createXMLHttpRequest();
            //20190820 이행점검 수정 및 추가
            xmlhttp.open("POST", "/myoffice/ezPortal/general/remote/portal_DeleteCache.aspx", "false");
            xmlhttp.send("<DATA><UID>CompanyID</UID></DATA>");

            var result = xmlhttp.responseText;

            if (result == "OK")
            {
                //alert("사용자 캐쉬가 삭제되었습니다.");
                alert("<%=RM.GetString("t84")%>");
                document.location.href = "TopMenu_Cross.aspx?pageid=" + pageid;
            }
            else
            {
                alert(result);
            }
        }

        function SaveSkin(pPageID)
        {
            var xmlhttp = createXMLHttpRequest();
            //20190820 이행점검 수정 및 추가
            xmlhttp.open("POST", "general/remote/portal_SaveSkin.aspx?pageid=" + pPageID, false);
            xmlhttp.send();
            xmlhttp = null;
        }

        function CheckDuplicate(pUID)
        {
            for (var i = 0; i < main_table.getElementsByTagName("td").length; i++)
            {
                if (GetAttribute(main_table.getElementsByTagName("td").item(i), "uid") == pUID) return true;
            }
            return false;
        }

        // 편집모드에서만 실행
        document.onselectstart = function ()
        {
            if (mode != "view")
            {
                event.cancelBubble = true;
                event.returnValue = false;
            }
        }

        function OpenEditWindow(pUID)
        {
            //20190820 이행점검 수정 및 추가
            if (pUID == "201") window.open("general/edit/LogoArea_Edit.aspx?pageid=" + pageid, "", "height = 356px, width = 390px, status = no, toolbar=no, menubar=no,location=no, resizable=1");
            if (pUID == "202") window.open("general/edit/UtilMenuArea_Edit.aspx?pageid=" + pageid, "", "height = 356px, width = 390px, status = no, toolbar=no, menubar=no,location=no, resizable=1");
            if (pUID == "203") window.open("general/edit/MainMenuArea_Edit.aspx?pageid=" + pageid, "", "height = 356px, width = 390px, status = no, toolbar=no, menubar=no,location=no, resizable=1");
            if (pUID == "205") window.open("general/edit/SearchArea_Edit.aspx?pageid=" + pageid, "", "height = 356px, width = 390px, status = no, toolbar=no, menubar=no,location=no, resizable=1");
        }

        function dblclicksubcell()
        {
            var obj = null;
            if (event.srcElement.id == "") obj = event.srcElement.parentElement;
            else obj = event.srcElement;

            if (typeof (obj.uid) != "undefined" && obj.uid != "") 
            {
                event.cancelBubble = true;
                OpenEditWindow(obj.uid);
            }
        }

        function AttachEvents(pObject, pPageID)
        {
            var prevpageid = "";
            var count = 0;

            for (var i = 0; i < pObject.getElementsByTagName("td").length; i++)
            {

                if (pObject.getElementsByTagName("td").item(i).id == "") continue;
                if (pObject.getElementsByTagName("td").item(i).id.indexOf("sub") > -1)
                {
                    if (prevpageid != GetAttribute(pObject.getElementsByTagName("td").item(i), "pageuid")) count++;
                    prevpageid = GetAttribute(pObject.getElementsByTagName("td").item(i), "pageuid");
                    pObject.getElementsByTagName("td").item(i).setAttribute("onclick", "selectsubcell(event)");
                    //pObject.all.tags("td").item(i).ondblclick = dblclicksubcell;
                    pObject.getElementsByTagName("td").item(i).setAttribute("onkeydown", "cellkeydown(event)");
                    pObject.getElementsByTagName("td").item(i).setAttribute("onkeyup", "cellkeyup()");
                    pObject.getElementsByTagName("td").item(i).style.cursor = "pointer";
                }
                else
                {
                    pObject.getElementsByTagName("td").item(i).setAttribute("onclick", "selectcell(event)");
                    pObject.getElementsByTagName("td").item(i).setAttribute("onkeydown", "cellkeydown(event)");
                    pObject.getElementsByTagName("td").item(i).setAttribute("onkeyup", "cellkeyup()");
                }
            }
            if (count > 1) bInherit = false;
        }

        // 영역 선택시 처리
        function selectcell(e)
        {
            var Event = e ? e : window.event;
            var Element = Event.target ? Event.target : Event.srcElement;
            if (Element.id == "") return;
            if (Element.id.indexOf("sub") > -1) return;
            selectedCell = Element.id;
            if (previousCell != null) previousCell.style.backgroundColor = "white";
            previousCell = Element.children.item(0).children.item(0).children.item(0).children.item(0);
            previousCell.style.backgroundColor = "lightblue";


            // 현재 선택된 cell
            var cell = eval(selectedCell);

            // 선택된 cell의 table
            var tblObject = eval(GetMainTable(eval(selectedCell)));

            if (typeof (tblObject) == "undefined")
                return;

            var maxHeight = 0;
            var compareHeight = 0;

            if (GetAttribute(tblObject, "height") != "")
                maxHeight = parseInt(GetAttribute(tblObject, "height").replace("px", ""), 10);

            // 해당 table의 height를 구한다.
            for (var i = 0; i < tblObject.getElementsByTagName("tr").length; i++)
            {
                try
                {
                    compareHeight = tblObject.getElementsByTagName("tr").item(i).style.height.replace("px", "");

                    if (compareHeight != "")
                    {
                        if (parseInt(compareHeight, 10) > maxHeight)
                            maxHeight = parseInt(compareHeight, 10);
                    }

                } catch (e) { }
            }

            document.getElementById("txtWidth").value = cell.style.width.replace("px", "");
            document.getElementById("txtHeight").value = maxHeight;

            document.getElementById("txtWidth").disabled = false;
            if (document.getElementById("txtWidth").value == "")
            {
                document.getElementById("txtWidth").value = "*";
                document.getElementById("txtWidth").disabled = true;
            }

            // 선택한 개체의 종류
            selObjClass = "TABLE";
        }

        function selectcellTitle(e)
        {
            var Event = e ? e : window.event;
            var Element = Event.target ? Event.target : Event.srcElement;
            selectcell2(Element.parentElement.parentElement.parentElement.parentElement);
        }
        function selectcell2(obj)
        {


            if (GetAttribute(obj, "id") == "") return;
            if (GetAttribute(obj, "id").indexOf("sub") > -1) return;
            selectedCell = GetAttribute(obj, "id");
            if (previousCell != null) previousCell.style.backgroundColor = "white";
            previousCell = obj.children.item(0).children.item(0).children.item(0).children.item(0);
            previousCell.style.backgroundColor = "lightblue";


            // 현재 선택된 cell
            var cell = eval(selectedCell);

            // 선택된 cell의 table
            var tblObject = eval(GetMainTable(eval(selectedCell)));

            if (typeof (tblObject) == "undefined")
                return;

            var maxHeight = 0;
            var compareHeight = 0;

            if (GetAttribute(tblObject, "height") != "")
                maxHeight = parseInt(GetAttribute(tblObject, "height").replace("px", ""), 10);

            // 해당 table의 height를 구한다.
            for (var i = 0; i < tblObject.getElementsByTagName("tr").length; i++)
            {
                try
                {
                    compareHeight = tblObject.getElementsByTagName("tr").item(i).style.height.replace("px", "");

                    if (compareHeight != "")
                    {
                        if (parseInt(compareHeight, 10) > maxHeight)
                            maxHeight = parseInt(compareHeight, 10);
                    }

                } catch (e) { }
            }

            document.getElementById("txtWidth").value = cell.style.width.replace("px", "");
            document.getElementById("txtHeight").value = maxHeight;


            document.getElementById("txtWidth").disabled = false;
            if (document.getElementById("txtWidth").value == "")
            {
                document.getElementById("txtWidth").value = "*";
                document.getElementById("txtWidth").disabled = true;
            }

            // 선택한 개체의 종류
            selObjClass = "TABLE";
        }
        // 컨텐츠 선택시 처리
        function selectsubcell(e)
        {
            var Event = e ? e : window.event;
            var eventItem = Event.target ? Event.target : Event.srcElement;
            //var eventItem = event.srcElement;

            if (GetAttribute(eventItem, "id") == null)
            {
                eventItem = eventItem.parentElement;
            }
            selectedSubCell = GetAttribute(eventItem, "id");

            try
            {
                if (previousSubCell != null) previousSubCell.parentElement.style.backgroundColor = "white";
            } catch (e) { }

            if (selectedSubCell.substr(0, 2).toLowerCase() != "su") 
            {
                selectedSubCell = "";
                return;
            }

            eventItem.parentElement.style.backgroundColor = "#FFDEB5";
            previousSubCell = eventItem;


            var cell = eval(selectedSubCell);
            var curHeight = parseInt(cell.parentElement.style.height.replace("px", ""));
            document.getElementById("txtHeight").value = curHeight;

            // 컨텐츠 선택시는 너비 입력필드를 disabled
            document.getElementById("txtWidth").value = "*";
            document.getElementById("txtWidth").disabled = true;

            // 선택한 개체의 종류
            selObjClass = "CONTENTS";
        }

        function cellkeyup()
        {
            pressCount = 0;
        }

        function cellkeydown(e)
        {
            if (!e.ctrlKey)
            {
                switch (e.keyCode)
                {
                    case 37:
                        swaprow("left");
                        break;
                    case 38:
                        swaprow("up");
                        break;
                    case 39:
                        swaprow("right");
                        break;
                    case 40:
                        swaprow("down");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e.keyCode)
                {
                    case 37:
                        resizecell("left");
                        break;
                    case 38:
                        resizerow("up");
                        break;
                    case 39:
                        resizecell("right");
                        break;
                    case 40:
                        resizerow("down");
                        break;
                    default:
                        break;
                }
            }
        }

        function GetPageID(pCell)
        {
            if (typeof (GetAttribute(pCell.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement, "uid")) != "undefined") return GetAttribute(pCell.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement, "uid");
            else return pageid;
        }

        function insertrow()
        {
            if (selectedCell == "")
            {
                alert("<%=RM.GetString("t347")%>");
                return;
            }

            if (eval(selectedCell).children.item(0).children.item(0).children.length > 9)
            {
                alert("<%=RM.GetString("t348")%>");
                return;
            }

            var ret = window.showModalDialog("MenuItem_search.aspx", "", "dialogHeight:340px; dialogWidth:290px; status:no;scroll:auto; help:no; edge:sunken");

            if (typeof (ret) == "undefined") return;

            if (CheckDuplicate(ret[0]) && ret[0] != "206")
            {
                alert("<%=RM.GetString("t349")%>");
                return;
            }

            if (ret[0] == "206") 
            {
                ret[0] = GetGUID();
                ret[1] = "";
            }

            var newrow = eval(selectedCell).children.item(0).children.item(0).insertRow(eval(selectedCell).children[0].children[0].children.length);
            newrow.style.width = "100%";
            newrow.style.height = "100px";

            var subtdGetid = "subtd" + GetGUID().substr(0, 4);
            var strInnerHTML = "<td id=\"" + subtdGetid + "\"uid=\"" + ret[0] + "\" style=\"width:100%\"  ownerpageuid='" + pageid + "' align=\"center\" onclick=\"selectsubcell(event)\" ondblclick=\"dblclicknotice()\" onkeydown=\"cellkeydown(event)\" canremove=\"1\"  canresize=\"1\"  canreplace=\"1\"><b> " + ret[1] + "</b></td>";
            newrow.innerHTML = strInnerHTML;

            //var newcell = newrow.insertCell();
            //newcell.id = "subtd" + GetID();
            //newcell.uid = ret[0];
            //newcell.pageuid = GetPageID(newcell);
            //newcell.canremove = 1;
            //newcell.canresize = 1;
            //newcell.canreplace = 1;
            //newcell.style.width = "100%";
            //newcell.align = "center";
            //newcell.innerHTML = "<b>" + ret[1] + "</b>";
            //newcell.onclick = selectsubcell;
            //newcell.onkeydown = cellkeydown;
            //selectedSubCell = "";
            //newcell.focus();

            var pageuid = "";
            if (GetPageID(document.getElementById(subtdGetid)) == null)
                pageuid = pageid;
            else
                pageuid = GetPageID(document.getElementById(subtdGetid));

            document.getElementById(subtdGetid).setAttribute("pageuid", pageuid);
            document.getElementById(subtdGetid).focus();
        }

        function insertcell()
        {
            if (bInherit)
            {
                alert("<%=RM.GetString("t294")%>");
                return;
            }

            if (selectedCell == "")
            {
                alert("<%=RM.GetString("t295")%>");
                return;
            }

            var newcell = document.createElement("td");
            var row = eval(selectedCell).parentElement;
            row.insertBefore(newcell, eval(selectedCell));

            newcell.style.width = "100px";
            newcell.vAlign = "top";
            newcell.innerHTML = "<table border=1 cellpadding=0 cellspacing=0 width=100% valign=top><tbody><TR style='WIDTH: 100%; HEIGHT: 10px' onclick='selectcellTitle(event)'><td align=center>100px</td></TR></tbody></table>";
            newcell.id = "td" + GetID();
            newcell.setAttribute("onclick", "selectcell(event)");
            newcell.setAttribute("onkeydown", "cellkeydown(event)");
            //newcell.onclick = selectcell;
            //newcell.onkeydown = cellkeydown;
            selectedSubCell = "";
        }

        function removecell()
        {
            if (bInherit)
            {
                alert("<%=RM.GetString("t350")%>");
                return;
            }

            if (selectedCell == "")
            {
                alert("<%=RM.GetString("t297")%>");
                return;
            }

            if (selectedCell == "td0") return;

            if (selectedCell.substr(0, 3) == "td0")
            {
                if (confirm("<%=RM.GetString("t298")%>"))
                {
                    eval(selectedCell).parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.removeChild(eval(selectedCell).parentElement.parentElement.parentElement.parentElement.parentElement);
                    selectedCell = "";
                    selectedSubCell = "";
                }
                return;
            }

            var row = eval(selectedCell).parentElement;

            for (var i = 0; i < row.children.length; i++)
            {
                if (row.children.item(i).id == selectedCell)
                {
                    row.removeChild(row.children.item(i));
                    break;
                }
            }
            selectedCell = "";
            selectedSubCell = "";
        }

        function removerow()
        {
            if (selectedSubCell == "")
            {
                alert("<%=RM.GetString("t351")%>");
                return;
            }

            var cell = eval(selectedSubCell);

            //if (cell.getAttribute("canremove") != 1)
            if (GetAttribute(cell, "canremove") != 1)
            {
                alert("<%=RM.GetString("t352")%>");
                return;
            }

            var parentPageid = GetPageID(cell);
            if (parentPageid == null)
                parentPageid = pageid;
            //if (cell.getAttribute("pageuid") != parentPageid)
            if (GetAttribute(cell, "pageuid") != parentPageid)
            {
                alert("<%=RM.GetString("t353")%>");
                return;
            }

            cell.parentElement.parentElement.removeChild(cell.parentElement);
            selectedSubCell = "";
            selectedCell = "";
        }
        function swapNodes(item1, item2)
        {
            var itemtmp = item1.cloneNode(1);
            var parent = item1.parentNode;
            item2 = parent.replaceChild(itemtmp, item2);
            parent.replaceChild(item2, item1);
            parent.replaceChild(item1, itemtmp);
            itemtmp = null;
        }
        function getNextSibling(node)
        {

            while (node.nodeType != 1)
            {
                node = node.nextSibling;
            }

            return node;
        }
        function getPreviousSibling(node)
        {

            while (node.nodeType != 1)
            {
                node = node.previousSibling;
            }

            return node;
        }

        //크로스용 함수 추가
        function swaprow(pDirection)
        {
            if (selectedSubCell == "")
            {
                alert("<%=RM.GetString("t354")%>");
                return;
            }

            var cell = eval(selectedSubCell);

            if (GetAttribute(cell, "canreplace") != 1)
            {
                alert("<%=RM.GetString("t355")%>");
                return;
            }
            var parentPageid = GetPageID(cell);
            if (parentPageid == null)
                parentPageid = pageid;
            if (GetAttribute(cell, "pageuid") != parentPageid)
            {
                alert("<%=RM.GetString("t356")%>");
                return;
            }

            var obj = null;

            if (pDirection == "up")
            {
                if (getPreviousSibling(cell.parentElement.previousSibling) == null || getPreviousSibling(cell.parentElement.previousSibling).children.item(0).id == "")
                {
                    if (cell.pageuid == pageid) return;
                    try
                    {
                        obj = cell.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.insertAdjacentElement("beforeBegin", cell.parentElement);
                        //obj.children.item(0).getAttribute("pageuid") = GetPageID(obj.children.item(0));
                        SetAttribute(obj.children.item(0), "pageuid", GetPageID(obj.children.item(0)));
                    } catch (e) { return; }
                }
                else 
                {
                    if (getPreviousSibling(cell.parentElement.previousSibling).outerHTML.toLowerCase().indexOf("table") > -1)
                    {
                        try
                        {
                            obj = getPreviousSibling(cell.parentElement.previousSibling).children.item(0).children.item(0).children.item(0).children.item(0).lastChild.children.item(0).children.item(0).insertAdjacentElement("beforeEnd", cell.parentElement);
                            //obj.children.item(0).getAttribute("pageuid") = GetPageID(obj.children.item(0));
                            SetAttribute(obj.children.item(0), "pageuid", GetPageID(obj.children.item(0)));
                        } catch (e) { return; }
                    }
                    else
                    {
                        //cell.parentElement.swapNode(cell.parentElement.previousSibling);
                        swapNodes(cell.parentElement, getPreviousSibling(cell.parentElement.previousSibling));
                    }
                }
            }
            else if (pDirection == "down")
            {
                if (getNextSibling(cell.parentElement.nextSibling) == null || getNextSibling(cell.parentElement.nextSibling).children.item(0).id == "")
                {
                    if (cell.pageuid == pageid) return;
                    try
                    {
                        obj = cell.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.insertAdjacentElement("afterEnd", cell.parentElement);
                        obj.children.item(0).pageuid = GetPageID(obj.children.item(0));
                    } catch (e) { return; }
                }
                else
                {
                    if (getNextSibling(cell.parentElement.nextSibling).outerHTML.toLowerCase().indexOf("table") > -1)
                    {
                        try
                        {
                            obj = getNextSibling(cell.parentElement.nextSibling).children.item(0).children.item(0).children.item(0).children.item(0).firstChild.children.item(0).children.item(0).firstChild.insertAdjacentElement("afterEnd", cell.parentElement);
                            //obj.children.item(0).getAttribute("pageuid") = GetPageID(obj.children.item(0));
                            SetAttribute(obj.children.item(0), "pageuid", GetPageID(obj.children.item(0)));
                        } catch (e) { return; }
                    }
                    else
                    {
                        //cell.parentElement.swapNode(cell.parentElement.nextSibling);
                        //cell.parentElement.swapNode(cell.parentElement.nextSibling);
                        swapNodes(cell.parentElement, getNextSibling(cell.parentElement.nextSibling));
                    }

                }
            }
            else if (pDirection == "left")
            {
                if (getPreviousSibling(cell.parentElement.parentElement.parentElement.parentElement.previousSibling) == null) return;

                if (getPreviousSibling(cell.parentElement.parentElement.parentElement.parentElement.previousSibling).children.item(0).children.item(0).children.length > 9)
                {
                    alert("<%=RM.GetString("t348")%>");
                    return;
                }
                getPreviousSibling(cell.parentElement.parentElement.parentElement.parentElement.previousSibling).children.item(0).children.item(0).appendChild(cell.parentElement);
            }
            else if (pDirection == "right")
            {
                if (getNextSibling(cell.parentElement.parentElement.parentElement.parentElement.nextSibling) == null) return;

                if (getNextSibling(cell.parentElement.parentElement.parentElement.parentElement.nextSibling).children.item(0).children.item(0).children.length > 9)
                {
                    alert("<%=RM.GetString("t348")%>");
                    return;
                }
                getNextSibling(cell.parentElement.parentElement.parentElement.parentElement.nextSibling).children.item(0).children.item(0).appendChild(cell.parentElement);
            }
            cell.focus();
        }

        function resizecell(pDirection)
        {
            if (selectedCell == "")
            {
                alert("<%=RM.GetString("t354")%>");
                return;
            }

            if (bInherit)
            {
                alert("<%=RM.GetString("t305")%>");
                return;
            }

            var cell = eval(selectedCell);

            var curWidth = parseInt(cell.style.width.replace("px", ""));

            //			pressCount++;
            var increase = 1;
            //			if (pressCount > 100) increase = 5;

            if (pDirection == "right")
            {
                curWidth += increase;
                try
                {
                    cell.style.width = curWidth.toString();
                    cell.children.item(0).children.item(0).children.item(0).children.item(0).innerHTML = curWidth.toString() + "px";
                } catch (e) { }
            }
            else if (pDirection == "left")
            {
                curWidth -= increase;
                try
                {
                    cell.style.width = curWidth.toString();
                    cell.children.item(0).children.item(0).children.item(0).children.item(0).innerHTML = curWidth.toString() + "px";
                } catch (e) { }
            }

            event.cancelBubble = true;
        }

        function resizerow(pDirection)
        {
            if (selectedSubCell == "")
            {
                alert("<%=RM.GetString("t306")%>");
                return;
            }

            var cell = eval(selectedSubCell);

            if (cell.canresize != 1)
            {
                alert("<%=RM.GetString("t357")%>");
                return;
            }

            if (cell.pageuid != GetPageID(cell))
            {
                alert("<%=RM.GetString("t358")%>");
                return;
            }

            var curHeight = parseInt(cell.parentElement.style.height.replace("px", ""));

            //			pressCount++;
            var increase = 1;
            //			if (pressCount > 100) increase = 5;

            if (pDirection == "up")
            {
                curHeight += increase;
                try
                {
                    cell.parentElement.style.height = curHeight.toString();
                } catch (e) { }
            }
            else if (pDirection == "down")
            {
                curHeight -= increase;
                try
                {
                    cell.parentElement.style.height = curHeight.toString();
                } catch (e) { }
            }
            //event.cancelBubble = true;
        }

        function GetMainTable(pCell)
        {
            try
            {
                return pCell.parentElement.parentElement.parentElement.id;
            } catch (e) { }
        }

        function resizepage(pDirection)
        {
            if (selectedCell == "")
            {
                alert("<%=RM.GetString("t311")%>");
                return;
            }

            var tblObject = eval(GetMainTable(eval(selectedCell)));

            if (bInherit)
            {
                alert("<%=RM.GetString("t305")%>");
                return;
            }

            if (tblObject.width == "100%" && (pDirection == "left" || pDirection == "right"))
            {
                alert("<%=RM.GetString("t309")%>");
                return;
            }
            if (tblObject.height == "100%" && (pDirection == "up" || pDirection == "down"))
            {
                alert("<%=RM.GetString("t310")%>");
                return;
            }

            try
            {
                if (pDirection == "left")
                {
                    tblObject.width = parseInt(tblObject.width.toString().replace("px", "")) - 10;
                }
                if (pDirection == "right")
                {
                    tblObject.width = parseInt(tblObject.width.toString().replace("px", "")) + 10;
                }
                if (pDirection == "down")
                {
                    tblObject.height = parseInt(tblObject.height.toString().replace("px", "")) + 10;
                    tblObject.parentElement.parentElement.style.height = tblObject.height;
                }
                if (pDirection == "up")
                {
                    tblObject.height = parseInt(tblObject.height.toString().replace("px", "")) - 10;
                    tblObject.parentElement.parentElement.style.height = tblObject.height;
                }
            } catch (e) { }

        }

        function S4()
        {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        }
        function GetGUID()
        {
            return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
        }

        function GetID()
        {
            return count++;
        }

        function preview()
        {
            window.open("TopMenu_cross.aspx?mode=view&viewmode=preview&pageid=" + pageid);
        }

        function OpenMaxURL(pURL)
        {
            if (pURL == "") return;
            location.href = pURL;
        }

        function insertpage()
        {
            if (selectedCell == "")
            {
                alert("<%=RM.GetString("t312")%>");
                return;
            }

            if (eval(selectedCell).children.item(0).children.item(0).children.length > 9)
            {
                alert("<%=RM.GetString("t348")%>");
                return;
            }

            var strHTML = "<table id='main_table_" + GetGUID().substr(0, 4) + "' border=1 cellpadding=0 cellspacing=0 width=100% height=110px style='table-layout:fixed;'>";
            strHTML += "<tr id='main_row'>";
            strHTML += "<TD id='td0" + GetGUID().substr(0, 3) + "' vAlign=top><table border=1 cellpadding=0 cellspacing=0 width=100% valign=top>";
            strHTML += "<TBODY><TR style='WIDTH: 100%; HEIGHT: 10px' onclick='selectcellTitle()'><td align=center>*</td></TR></tbody>";
            strHTML += "</table></td></tr></table>";

            var newrow = eval(selectedCell).children.item(0).children.item(0).insertRow(eval(selectedCell).children[0].children[0].children.length);
            newrow.style.width = "100%";
            newrow.style.height = "100";

            var subGetId = "subtd" + GetID();
            var strInnerHTML = "<td id=\"" + subGetId + "\"uid=\"" + GetGUID() + "\" style=\"width:100%\" pageuid='" + GetGUID() + "' ownerpageuid='" + pageid + "' align=\"center\" onclick=\"selectsubcell(event)\" ondblclick=\"dblclicknotice()\" onkeydown=\"cellkeydown(event)\" canremove=\"1\"  canresize=\"1\"  canreplace=\"1\">" + strHTML + "</td>";
            newrow.innerHTML = strInnerHTML;
            document.getElementById(subGetId).focus();
            AttachEvents(document.getElementById(subGetId))
        }

        function newpage()
        {
            location.href = "TopMenu_Cross.aspx?mode=new";
        }


        /* 현재 이미지 관리 및 롤오버시 이미지 변환 함수 */
        var curImg = new Image;
        var oldPath = "";

        function img_onMouseOver(e, pNewPath)
        {
            var Event = e ? e : window.event;
            newImg = new Image();
            newImg = Event.target ? Event.target : Event.srcElement;
            //newImg = event.srcElement;

            if (curImg == newImg)
            {
                return;
            } else
            {
                oldPath = newImg.src;
                newImg.src = pNewPath;
            }
        }

        function img_onMouseOut(e)
        {
            var Event = e ? e : window.event;
            newImg = new Image();
            newImg = Event.target ? Event.target : Event.srcElement;

            if (curImg == newImg)
            {
                return;
            } else
            {
                newImg.src = oldPath;
            }
        }

        function sub_toggle(subfolder)
        {
            try
            {
                for (var i = 0; i < subfolder.parentElement.children.length; i++)
                {
                    subfolder.parentElement.children.item(i).style.display = "none";
                }

                subfolder.style.display = "block";
                //subfolder.firstChild.firstChild.firstChild.click();
            } catch (e) { }

        }

        function submenuclick(pSubMenuID)
        {
            //alert(pSubMenuID);
		    /* 
			if (pSubMenuID == 'de0616aa-fd6e-4d95-a82c-e87e80ea0d04') 
			{
				event.cancelBubble = true;
				event.returnValue = false;
				sso_go("ESS", "ACCESS", "OPEN_WIN", "DEF");
			}
			*/
        }

        function layoutmode()
        {
            for (var i = 0; i < document.getElementsByTagName("tr").length; i++)
            {
                var evtHandler = document.getElementsByTagName("tr").item(i).onclick;
                if (evtHandler != null && evtHandler.toString().indexOf("selectcellTitle") > -1)
                {
                    document.getElementsByTagName("tr").item(i).style.display = "none";
                }
            }
        }

        function editingmode()
        {
            for (var i = 0; i < document.getElementsByTagName("tr").length; i++)
            {
                var evtHandler = document.getElementsByTagName("tr").item(i).onclick;
                if (evtHandler != null && evtHandler.toString().indexOf("selectcellTitle") > -1)
                {
                    document.getElementsByTagName("tr").item(i).style.display = "";
                }
            }
        }

        function resizeTable()
        {
            if (selObjClass == "TABLE")
            {
                if (selectedCell == "")
                {
                    alert("<%=RM.GetString("t314")%>");
                    return;
                }

                // 현재 선택된 cell
                var cell = eval(selectedCell);

                if (txtWidth.value != "*" && txtWidth.value != "")
                {
                    if (!is_num(txtWidth.value))
                    {
                        alert("<%=RM.GetString("t315")%>");
                        return;
                    }
                    cell.style.width = document.getElementById("txtWidth").value;
                    cell.children.item(0).children.item(0).children.item(0).children.item(0).innerHTML = document.getElementById("txtWidth").value + "px";
                }

                var tblObject = eval(GetMainTable(eval(selectedCell)));

                if (typeof (tblObject) == "undefined")
                    return;

                if (!is_num(txtHeight.value))
                {
                    alert("<%=RM.GetString("t316")%>");
                    return;
                }
                tblObject.height = document.getElementById("txtHeight").value;
                tblObject.parentElement.parentElement.style.height = document.getElementById("txtHeight").value + "px";
            }
            else if (selObjClass == "CONTENTS")
            {
                if (selectedSubCell == "")
                {
                    alert("<%=RM.GetString("t306")%>");
                    return;
                }

                var cell = eval(selectedSubCell);

                if (GetAttribute(cell, "canresize") != 1)
                {
                    alert("<%=RM.GetString("t317")%>");
                    return;
                }

                try
                {
                    cell.parentElement.style.height = document.getElementById("txtHeight").value + "px";
                } catch (e) { alert }
            }
            else
            {
                alert("<%=RM.GetString("t318")%>");
            }
            //event.cancelBubble = true;
        }

        // 통합검색
        function Search()
        {
            txtSearch.value = TrimText(ReplaceText(txtSearch.value, "'", ""));
            var pSearchString = txtSearch.value;

            parent.frames["main"].location.href = "/myoffice/ezsearch/index_search.aspx?Keyword=" + escape(pSearchString);
        }

        function entercheck()
        {
            if (window.event.keyCode == 13)
                Search();
        }

        //메인메뉴가 10개 이상일때 생성된 select 박스 함수 
        function GoFunc(pUrl)
        {
            var dropdown = servicselist;
            var index = servicselist.selectedIndex;
            var arrUrls = pUrl.split(";");
            var GoUrl = "";

            GoUrl = arrUrls[index - 1];

            if (!GoUrl) return;

            // 2011.02.26 그룹웨어간 연동
            if (GoUrl.indexOf("sso=ok") > -1)
                GoUrl = "groupware_sso.aspx?gourl=" + GoUrl;

            //2013.02.28 주소에 '/myoffice/' 포함시 'http://'가 붙지않도록 추가
            if (GoUrl.indexOf("/myoffice/") <= -1)
            {
                if (GoUrl.indexOf("http://") > -1)
                {
                    window.open(GoUrl, "");
                }
                //2013.02.13 http,https 둘다 등록 가능하도록 추가
                else if (GoUrl.indexOf("https://") > -1)
                {
                    window.open(GoUrl, "");
                }
                //2013.02.21 주소에 http,http 미 포함시에도 작동하도록 추가
                else if (GoUrl.indexOf("http://") <= -1 || GoUrl.indexOf("https://") <= -1)
                {
                    var GoUrl2 = "http://" + GoUrl
                    window.open(GoUrl2, "");
                }
            }
            else 
            {
                window.open(GoUrl, "main");
            }
        }

        function SelectBoxReset()
        {
            // 셀렉트 메뉴 초기화
            if (p_Morebox == "true")
            {
                servicselist[0].selected = true;
            }
        }


        //20180328 스크립트 표준화 하기 내용이 없어 500 에러 나고 있었음.
        var xmlHTTP = null;
        var blogout = false;
        function update_connectinfo()
        {
            if (blogout)
                return;

            xmlHTTP = createXMLHttpRequest();
            xmlHTTP.open("POST", "/myoffice/main/update_connectinfo.aspx", true);
            xmlHTTP.onreadystatechange = event_update_connectinfo;
            xmlHTTP.send();
        }

        var bLogOutNOTICE = false;
        function event_update_connectinfo()
        {
            if (xmlHTTP.readyState != 4)
                return;

            if (xmlHTTP.status == 200 && xmlHTTP.responseText == "LOGOUT")
            {
                blogout = true;
                alert("<%=RM.GetString("t346")%>");
                window.top.location.href = "/Logout.aspx";
            }
        }

        //20180424 sub menu 
        function submenuover(obj)
        {
            parent.document.getElementById("topFrame").style.position = "relative";

            if (IE(10))
            {
                var tempDivArr = obj.getElementsByTagName("div");

                for (var t = 0; t < tempDivArr.length; ++t)
                {
                    if (tempDivArr[t].className == "navsubList" || tempDivArr[t].className == "my_site_list")
                    {
                        tempDivArr[t].style.display = "block";
                        break;
                    }
                }
            }
        }

        function submenuout(obj)
        {
            parent.document.getElementById("topFrame").style.position = "";

            if (IE(10))
            {
                var tempDivArr = obj.getElementsByTagName("div");

                for (var t = 0; t < tempDivArr.length; ++t)
                {
                    if (tempDivArr[t].className == "navsubList" || tempDivArr[t].className == "my_site_list")
                    {
                        tempDivArr[t].style.display = "none";
                        break;
                    }
                }
            }
        }


        function clickTotalMenu()
        {
            parent.document.getElementById("topFrame").style.position = "relative";
            document.getElementById("topMenu_bg_div").style.display = "";
        }

        function closeTotalMenu()
        {
            parent.document.getElementById("topFrame").style.position = "";
            document.getElementById("topMenu_bg_div").style.display = "none"
        }



        //20180523 check ie version 
        function IE(v)
        {
            return RegExp('msie' + (!isNaN(v) ? ('\\s' + v) : ''), 'i').test(navigator.userAgent);
        }

        // 직원조회
        function Emp_Search()
        {
            if (document.getElementById('input_search').value != "")
            {
                var wHeight = 560;
                var wWidth = 750;
                var wVertical = Math.floor(screen.height / 2) - (wHeight / 2);
                var wHorizontal = Math.floor(screen.width / 2) - (wWidth / 2);

                window.open("/myoffice/ezPersonal/PersonSearch/PersonSearch_cross.aspx?SearchString=" + encodeURI(document.getElementById('input_search').value), "", "height=" + wHeight + "px,width=" + wWidth + "px, left=" + wHorizontal + "px, top=" + wVertical + "px, status=no, toolbar=no, menubar=no,location=no, resizable=0");
                document.getElementById('input_search').value = '';
            }
        }

        function keyword_Clear(obj)
        {
            obj.value = "";
        }

        function Key_event(e, obj)
        {
            var curevent = (typeof event == 'undefined' ? e : event)
            if (curevent.keyCode == "13")
            {
                Emp_Search();
            }
        }

        var manageuserlink_dialogArguments = new Array();
        function btnModify_click()
        {
            manageuserlink_dialogArguments[0] = "";
            manageuserlink_dialogArguments[1] = btnModify_click_Complete;

            //20161222:즐겨찾기 파라미터 추가, 439 -> 539
            var OpenWin = window.open("/myoffice/ezPersonal/Link/ManageUserLink.aspx", "", GetOpenWindowfeature(410, 539));//2020.10.08 즐겨찾기 순서변경
            try { OpenWin.focus(); } catch (e) { }
            //window.open("/myoffice/ezPersonal/Link/ManageUserLink.aspx", "", "dialogHeight:439px;dialogwidth:390px;dialogleft:100px;dialogtop:100px;status:no;toolbar:no;location:no;scroll:no;edge:sunken");
        }

        function btnModify_click_Complete(rtnValue)
        {
            if (rtnValue != 1)
                return;
            else
                window.location.reload(false);
        }

        function show_link(objthis)
        {
            //20161222:즐겨찾기 파라미터 추가
            if (objthis.length > 7)
            {
                var sValues = objthis;
                var sLinkurl = TrimText(sValues.split("|")[1]);
                var sMethod = TrimText(sValues.split("|")[3]);//POST, GET
                var sP1N = TrimText(sValues.split("|")[4]);
                var sP1V = TrimText(sValues.split("|")[5]);
                var sP2N = TrimText(sValues.split("|")[6]);
                var sP2V = TrimText(sValues.split("|")[7]);

                //20161222:즐겨찾기 파라미터 추가, userid 값이 입력될 경우 id로 변환
                if (sP1V.toLowerCase() == "userid")
                    sP1V = "<%=userinfo.UserID.Split('@')[0].ToString()%>";
                if (sP2V.toLowerCase() == "userid")
                    sP2V = "<%=userinfo.UserID.Split('@')[0].ToString()%>";

                if (sMethod == "GET")
                {
                    var sPara = "";
                    if (sP1N != "" && sP1V != "")
                        sPara = "?" + sP1N + "=" + sP1V;
                    if (sP2N != "" && sP2V != "")
                        sPara += "&" + sP2N + "=" + sP2V;

                    window.open(sLinkurl + sPara, "");
                }
                else if (sMethod == "POST")
                {
                    Standard_POST("My Site", sLinkurl, sMethod, sP1N, sP1V, sP2N, sP2V);
                }
                else
                {
                    window.open(sLinkurl, "");
                }
            }
        }

        //20161222:즐겨찾기 파라미터 추가
        function TrimText(pStr)
        {
            return pStr.replace(/(^\s*)|(\s*$)/g, "");
        }

        //20161222:즐겨찾기 파라미터 추가
        function Standard_POST(sName, sLinkurl, sMethod, sP1N, sP1V, sP2N, sP2V)
        {
            document.all("PURL").value = sLinkurl;
            document.all("PNAME").value = sName;
            document.all("PMETHOD").value = sMethod;
            document.all("P1NAME").value = sP1N;
            document.all("P1VALUE").value = sP1V;
            document.all("P2NAME").value = sP2N;
            document.all("P2VALUE").value = sP2V;

            //20181005:POST 시 빈창 뜨는 현상 처리, iframe 추가 및 연결, standardPost -> framePost
            //window.open("", "standardPost", "toolbar=no, width=1, height=1, directories=no, status=no, scrollorbars=no, resizable=no");
            document.all("formPost").action = "/myoffice/ezPersonal/SSO/Standard_POST.aspx";
            document.all("formPost").method = "post";
            document.all("formPost").target = "framePost";
            document.all("formPost").submit();
        }


        function QuickMove(value)
        {
            var totalcnt = document.getElementById('QuickUl').getElementsByTagName('dd').length;

            if (value == "DOWN")
            {
                if (totalcnt > QuickcurNum + QuickBlockNum)
                {
                    document.getElementById('QuickUl').getElementsByTagName('dd')[QuickcurNum].style.display = "none";
                    document.getElementById('QuickUl').getElementsByTagName('dd')[QuickcurNum + QuickBlockNum].style.display = "block";
                    QuickcurNum++;
                }
            } else
            {
                if (QuickcurNum > 0)
                {
                    QuickcurNum--;
                    document.getElementById('QuickUl').getElementsByTagName('dd')[QuickcurNum].style.display = "block";
                    document.getElementById('QuickUl').getElementsByTagName('dd')[QuickcurNum + QuickBlockNum].style.display = "none";
                }
            }
        }

        <%//2022.01.17 이승중 서버작업 로그인 차단 추가%>
        function inspection_logout()
        {
            let now = new Date();
            let startChkDate = new Date("2022-01-20T22:00:00");
            let EndChkDate = new Date("2022-01-21T06:00:00");
            if ( startChkDate.getTime() < now.getTime() && now.getTime() < EndChkDate.getTime() &&  "<%=inspection_IP%>" == "N")
            {
                javascript:window.open("/logout.aspx","top","")
            }
        }

    </script>
</head>
<body class="<%=bodyStyle%>">
    <%--  20180327 스크립트 표준화      <%if (Request.UserAgent.IndexOf("Trident") > -1 || Request.UserAgent.IndexOf("MSIE") > -1){%>
        <object id="icdbho" style="DISPLAY: none" codebase="/ezIcd.cab#version=1,0,2,15" data="data:application/x-oleobject;base64,GvFdR8IrqUGKl+mJ4CPlFwADAADYEwAA2BMAAA==" classid="CLSID:475DF11A-2BC2-41A9-8A97-E989E023E517" viewastext></object>
        <%}%>--%>
    <% if (mode != "view")
        { %>
    <!-- 메뉴 -->
    <h1><%=RM.GetString("t363")%></h1>
    <div id="mainmenu">
        <ul>
            <li><span onclick="save()"><%=RM.GetString("t62")%></span></li>
            <li><span onclick="layoutmode()"><%=RM.GetString("t322")%></span></li>
            <li><span onclick="editingmode()"><%=RM.GetString("t323")%></span></li>
            <li><span onclick="preview()"><%=RM.GetString("t63")%></span></li>
            <li><span onclick="insertpage()"><%=RM.GetString("t325")%></span></li>
            <li><span onclick="removecell()"><%=RM.GetString("t326")%></span></li>
            <li><span onclick="insertcell()"><%=RM.GetString("t327")%></span></li>
            <li><span onclick="removecell()"><%=RM.GetString("t328")%></span></li>
            <br>
            <li><span onclick="insertrow()"><%=RM.GetString("t329")%></span></li>
            <li><span onclick="removerow()"><%=RM.GetString("t330")%></span></li>
            <li><span onclick="swaprow('up')"><%=RM.GetString("t331")%></span></li>
            <li><span onclick="swaprow('down')"><%=RM.GetString("t332")%></span></li>
            <li><span onclick="swaprow('left')"><%=RM.GetString("t72")%></span></li>
            <li><span onclick="swaprow('right')"><%=RM.GetString("t74")%></span></li>
        </ul>
    </div>
    <table class="content">
        <tr>
            <th>메뉴레이아웃명</th>
            <td>
                <input type="text" id="txtDisplayName" value="<%= displayname %>"></td>
        </tr>
    </table>
    <br>
    <table class="content" style="width: 100%">
        <tr>
            <th><%=RM.GetString("t334")%><input type="text" name="txtWidth" id="txtWidth" style="width: 50px">
                px * <%=RM.GetString("t335")%><input type="text" name="txtHeight" id="txtHeight" style="width: 50px">
                px <a href="#" class="imgbtn"><span onclick="resizeTable()"><%=RM.GetString("t336")%></span></a></th>
        </tr>
    </table>

    <div>
        <%= strHTML %>
    </div>
    <% }
        else
        { %>
    <%= strHTML %>
    <% } %>
    <!-- 표준모듈 (2007.03.15) 수정: .NET Framework 2.0에서는 RegisterStartupScript 메서드 지원하지 않음. -->
    <%= _script1 %>
    <%=_script2 %>
    <%=_script3 %>

    <!-- //20140109:갱신 -->
    <% if (GetCompanyExpInfo("EXPIRECOOKIE", userinfo.CompanyID) == "Y" || GetCompanyExpInfo("EXPIRECOOKIE", userinfo.DeptID) == "Y" || GetCompanyExpInfo("EXPIRECOOKIE", userinfo.UserID) == "Y")
        { %>
    <iframe id="pRefresh" src="/myoffice/CookieRefresh.aspx" style="display: none"></iframe>
    <% } %>

    <!-- 2007_12_04 윤진규 sso 작업 - Ksign의 인증정보(세션)을 클라이언트가 내려받게 하기 위해 -->
    <iframe id="iframe1" src="/net/login_proc.aspx?uid=<%=userinfo.UserID%>" style="display: none"></iframe>
    <!--<iframe id=ifmpopup  src="/binary/Progress1.htm"></iframe>-->
    <form id="formPost" method="post" runat="server">
        <!--//20161222:즐겨찾기 파라미터 추가-->
        <div style="display: none">
            <!--standard_post-->
            <input type="text" id="PURL" runat="server" />
            <input type="text" id="PNAME" runat="server" />
            <input type="text" id="PMETHOD" runat="server" />
            <input type="text" id="P1NAME" runat="server" />
            <input type="text" id="P1VALUE" runat="server" />
            <input type="text" id="P2NAME" runat="server" />
            <input type="text" id="P2VALUE" runat="server" />
        </div>
    </form>
    <iframe id="framePost" name="framePost" style="display: none"></iframe>
</body>
</html>



