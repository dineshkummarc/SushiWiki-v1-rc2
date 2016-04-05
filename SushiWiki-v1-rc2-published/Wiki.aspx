<%@ Import namespace="Wiki" %>
<%@ Import namespace="Wiki.GUI" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="Wiki.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWiki" enableViewState="True" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>
			<%= Wiki.WikiSettings.Singleton().ApplicationTitle %>
			>
			<%= Request.QueryString["page"] %>
		</title>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<link href="http://eurohost.webmatrixhosting.net/egroise/favicon.ico" rel="SHORTCUT ICON">
		<LINK href="styles/lexer.css" type="text/css" rel="stylesheet">
		<LINK rel="stylesheet" type="text/css" href='styles/<%=(string)Session["userstyle"]%>'>
	</HEAD>
	<body>
		<form method="post" runat="server">
			<% if (Session["fullscreen"] == "ON") { %>
			<div id="overDiv" style="Z-INDEX: 1000; VISIBILITY: hidden; POSITION: absolute"></div>
			<script language="JavaScript" src="js/overlib_mini.js" type="text/javascript"></script>
			<img src="images/logo_verysmall.gif">&nbsp;&nbsp;
			<%#WikiGui.GetString("Gui.Wiki.YouAreIn")%>
			:<asp:label id="lblHistoForFullScreen" runat="server" EnableViewState="False" CssClass="status_bar"></asp:label>
			&nbsp;&nbsp;&nbsp;<asp:linkbutton id="lbEndFullString" runat="server" EnableViewState="False" CssClass="main_menu"><img src="images/icon_fullscreen.gif" border="0"><%#WikiGui.GetString("Gui.Wiki.ViewMenu")%></asp:linkbutton><br>
			<% } else {%>
			<uc1:header id="Header1" runat="server" EnableViewState="False"></uc1:header>
			<table class="menu" height="27" cellSpacing="0" width="100%" border="0">
				<tr class="menu_actions">
					<td class="menu" style="HEIGHT: 21px"><asp:hyperlink id="hlEditPage" runat="server" EnableViewState="False" CssClass="main_menu">
							<img src="images/icon_edit.gif" border="0"><%# WikiGui.GetString("Gui.Wiki.Edit")%></asp:hyperlink>&nbsp;
						<asp:hyperlink id="hlEvents" runat="server" EnableViewState="False" CssClass="main_menu">
							<img src="images/icon_events.gif" border="0"><%# WikiGui.GetString("Gui.Wiki.Events")%></asp:hyperlink>&nbsp;
						<asp:hyperlink id="hlAttach" runat="server" EnableViewState="False" CssClass="main_menu">
							<img border="0" src="images/icon_attach.gif"><%# WikiGui.GetString("Gui.Wiki.Attachments")%></asp:hyperlink>&nbsp;
						<asp:hyperlink id="hlLastChanges" runat="server" EnableViewState="False" CssClass="main_menu">
							<img border="0" src="images/icon_changes.gif"><%# WikiGui.GetString("Gui.Wiki.LastChanges")%>
						</asp:hyperlink>&nbsp;
						<asp:hyperlink id="hlSearch" runat="server" EnableViewState="False" CssClass="main_menu">
							<img src="images/icon_search.gif" border="0"><%# WikiGui.GetString("Gui.Wiki.ContextSearch")%>
						</asp:hyperlink>&nbsp;
						<asp:hyperlink id="hlRefresh" runat="server" CssClass="main_menu">
							<img src="images/icon_refresh.gif" border="0"><%# WikiGui.GetString("Gui.Wiki.Refresh")%>
						</asp:hyperlink>&nbsp;
						<asp:linkbutton id="lSelect" runat="server" EnableViewState="False" CssClass="main_menu">
							<%# WikiGui.GetString("Gui.Wiki.Select")%>
						</asp:linkbutton>&nbsp;
						<asp:linkbutton id="lPublic" runat="server" EnableViewState="False" CssClass="main_menu">
							<%# WikiGui.GetString("Gui.Wiki.MakeItPrivate")%>
						</asp:linkbutton><asp:linkbutton id="lbFullscreen" runat="server" EnableViewState="False" CssClass="main_menu">
							<img src="images/icon_fullscreen.gif" border="0"><%# WikiGui.GetString("Gui.Wiki.FullScreen")%>
						</asp:linkbutton></td>
				</tr>
				<tr class="menu">
					<td class="menu">
						<%# WikiGui.GetString("Gui.Wiki.YouAreIn")%>
						&nbsp;<asp:label id="lblHisto" runat="server" EnableViewState="False" CssClass="status_bar"></asp:label>
					</td>
				</tr>
			</table>
			<% } %>
			<div class="pagezone">
				<!-- Page zone start ---------------------------------------------------------------->
				<table cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td class="page_title"><asp:label id="lblTitle" runat="server" EnableViewState="False" CssClass="page_title"></asp:label>
						</td>
						<td class="page_title_info" align="right"><asp:Label id="lblTitleInfo" runat="server"></asp:Label></td>
					</tr>
				</table>
				<asp:label id="lblPageContent" runat="server" EnableViewState="False"></asp:label><br>
				<!-- Page zone end ------------------------------------------------------------------></div>
			<asp:label id="lblAttachements" runat="server" EnableViewState="False"></asp:label>
			<asp:label id="lblEvents" runat="server" EnableViewState="false"></asp:label>
			<table class="page_info" width="100%">
				<tr>
					<td align="right">
						<%#WikiGui.GetString("Gui.Wiki.PageInfo",pageTitle,
					currentPage.ownedBy,
					currentPage.lastUpdated.ToString(WikiSettings.Singleton().DateFormat, null),
					currentPage.updatedBy)%>
					</td>
				</tr>
			</table>
			<uc1:footer id="Footer1" runat="server" EnableViewState="False"></uc1:footer><LINK href="styles/lexer.css" type="text/css" rel="stylesheet"></form>
	</body>
</HTML>
