<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiEditTable.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.WikiEditTable" enableViewState="True" validateRequest="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiEditTable</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<uc1:wikiheader id="WikiHeader1" runat="server"></uc1:wikiheader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><IMG src="images/icon_edit.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" EnableViewState="False" CssClass="page_title"></asp:label></td>
				</tr>
			</table>
			<br>
			<br>
			<table class="input_panel">
				<asp:label id="lblInput" runat="server"></asp:label></table>
			<asp:placeholder id="phInput" runat="server"></asp:placeholder><br>
			<asp:label id="lblInfo" runat="server" CssClass="wikiinfo"></asp:label><br><br>
			<asp:button id="butSave" runat="server"></asp:button><br>
			<uc1:WikiFooter id="WikiFooter1" runat="server"></uc1:WikiFooter></form>
	</body>
</HTML>
