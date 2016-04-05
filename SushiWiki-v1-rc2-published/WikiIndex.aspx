<%@ Import namespace="Wiki.GUI" %>
<%@ Page language="c#" Codebehind="WikiIndex.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiIndex" enableViewState="False" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="WikiHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="WikiFooter.ascx" %>
<HTML>
	<HEAD>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
	</HEAD>
	<body bgcolor="#fffaf0">
		<form method="post" runat="server">
			<uc1:Header id="Header1" runat="server"></uc1:Header>
			<table class="menu" height="27" cellSpacing="0" width="100%" border="0">
				<tr class="menu_actions">
					<td class="menu" height="21">
						<asp:hyperlink id="hlPages" runat="server" CssClass="main_menu" EnableViewState="False"></asp:hyperlink>&nbsp;
						<asp:hyperlink id="hlUsers" runat="server" CssClass="main_menu" EnableViewState="False"></asp:hyperlink>&nbsp;
					</td>
				</tr>
			</table>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_index.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<asp:Label id="lblIndex" runat="server" EnableViewState="False"></asp:Label>
			<table class="menu" cellSpacing="1" width="100%" border="0">
				<tr class="menu">
					<td align="right" class="menu">&nbsp;
					</td>
				</tr>
			</table>
			<uc1:Footer id="Footer1" runat="server"></uc1:Footer>
		</form>
	</body>
</HTML>
