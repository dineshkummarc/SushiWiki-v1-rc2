<%@ Control Language="c#" AutoEventWireup="false" Codebehind="WikiHeader.ascx.cs" Inherits="Wiki.GUI.Templates.Header" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" enableViewState="False"%>
<%@ Import NameSpace="Wiki.GUI" %>
<LINK rel="stylesheet" type="text/css" href='styles/<%=(string)Session["userstyle"]%>'>
<!-- WikiHeader BEGIN -->
<div id="overDiv" style="Z-INDEX:1000; VISIBILITY:hidden; POSITION:absolute"></div>
<script type="text/javascript" src="js/overlib_mini.js"></script>
<table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1" class="top_menu">
	<tr>
		<td class="topzone1"><asp:HyperLink id="hlHome" runat="server" CssClass="top_menu" NavigateUrl="Wiki.aspx?page=WikiHome">
				<img src="images/logo_small.gif" border="0" runat="server"></asp:HyperLink></td>
		<td class="topzone2">
			<asp:Image id="imgUser" runat="server" ImageUrl="images/icon_user.gif"></asp:Image>
			<asp:HyperLink id="hlUser" runat="server" CssClass="top_menu" NavigateUrl="WikiUserSettings.aspx">
				<asp:Label id="lblUser" runat="server">?</asp:Label>
			</asp:HyperLink></td>
		<td align="right">
			<asp:HyperLink id="hlHelp" runat="server" CssClass="top_menu">
				<IMG border="0" SRC="images/icon_help.gif"><%#WikiGui.GetString("Gui.Header.Help")%></asp:HyperLink>
			<asp:HyperLink id="hlIndex" runat="server" CssClass="top_menu" NavigateUrl="WikiIndex.aspx">
				<img border="0" src="images/icon_index.gif"><%#WikiGui.GetString("Gui.Header.Index")%>
			</asp:HyperLink>&nbsp;
			<asp:HyperLink id="hlCalendar" NavigateUrl="WikiCalendar.aspx" runat="server">
				<img border="0" src="images/icon_calendar.gif"><%#WikiGui.GetString("Gui.Header.Calendar")%>
			</asp:HyperLink>
			<asp:HyperLink id="hlLogin" runat="server" CssClass="top_menu" NavigateUrl="WikiLogin.aspx">
				<img border="0" src="images/icon_user_guest.gif"><%#WikiGui.GetString("Gui.Header.Logon")%>
			</asp:HyperLink>
			<asp:LinkButton id="lbLogout" runat="server" CssClass="top_menu" NavigateUrl="WikiLogin.aspx">
				<img border="0" src="images/icon_user_guest.gif"><%#WikiGui.GetString("Gui.Header.Logout")%>
			</asp:LinkButton>
			<asp:HyperLink id="hlTools" runat="server" CssClass="top_menu" NavigateUrl="WikiTools.aspx">
				<img border="0" src="images/icon_tools.gif"><%#WikiGui.GetString("Gui.Header.Tools")%>
			</asp:HyperLink>
			<asp:HyperLink id="hlSearch" runat="server" CssClass="top_menu" NavigateUrl="WikiSearch.aspx">
				<img border="0" src="images/icon_fullsearch.gif"><%#WikiGui.GetString("Gui.Header.Search")%></asp:HyperLink>&nbsp;
			<asp:textbox CssClass="search" id="txtGoTo" runat="server" AutoPostBack="True"></asp:textbox><asp:ImageButton id="Go" runat="server" ImageUrl="images/button_go.gif" BorderStyle="None" ImageAlign="AbsBottom"></asp:ImageButton></A>
		</td>
	</tr>
</table>
<!-- WikiHeader END -->
