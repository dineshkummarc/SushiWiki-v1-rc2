<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiPlugins.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.WikiPlugins" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiPlugins</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<uc1:WikiHeader id="WikiHeader1" runat="server" EnableViewState="False"></uc1:WikiHeader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_tools.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<br>
			<asp:label id="lblInfo" runat="server" EnableViewState="False"></asp:label><br>
			<uc1:WikiFooter id="WikiFooter1" runat="server" EnableViewState="False"></uc1:WikiFooter></form>
	</body>
</HTML>
