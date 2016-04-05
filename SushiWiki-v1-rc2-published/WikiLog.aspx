<%@ Page language="c#" Codebehind="WikiLog.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiLog" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiLog</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<uc1:WikiHeader id="WikiHeader1" runat="server" EnableViewState="False"></uc1:WikiHeader>
			<table class="menu" height="27" cellSpacing="0" width="100%" border="0">
				<tr class="menu_actions">
					<td class="menu" height="21">
						<asp:Label id="lblLogFile" runat="server" EnableViewState="False"></asp:Label>
						<asp:DropDownList id="ddlLogFile" runat="server" AutoPostBack="True"></asp:DropDownList>
						<asp:LinkButton id="lbDelete" runat="server" EnableViewState="False"></asp:LinkButton>
						&nbsp;&nbsp;
						<asp:Label id="lblFilter" runat="server" EnableViewState="False"></asp:Label>
						<asp:DropDownList id="ddlFilter" runat="server" AutoPostBack="True"></asp:DropDownList>
					</td>
				</tr>
			</table>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_tools.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<br>
			<asp:label id="lblInfo" runat="server" EnableViewState="False"></asp:label><br>
			<uc1:WikiFooter id="WikiFooter1" runat="server" EnableViewState="False"></uc1:WikiFooter>
		</form>
	</body>
</HTML>
