<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiTools.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiTools" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiTools</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="WikiTools" method="post" runat="server" enctype="multipart/form-data">
			<uc1:WikiHeader id="WikiHeader1" runat="server"></uc1:WikiHeader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_tools.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<h4>
				<asp:Label id="lblExport" runat="server"></asp:Label></h4>
			<asp:RadioButton id="rbExportAll" runat="server" GroupName="export"></asp:RadioButton>
			<asp:RadioButton id="rbExportSelected" runat="server" GroupName="export"></asp:RadioButton>
			<asp:RadioButton id="rbExportMy" runat="server" Checked="True" GroupName="export"></asp:RadioButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<asp:LinkButton id="lbExport" runat="server"></asp:LinkButton>
			<P></P>
			<P>
				<asp:Panel id="pnlImport" runat="server"></P>
			<H4>
				<asp:Label id="lblImport" runat="server"></asp:Label></H4>
			<P>
				<INPUT id="txtFile" type="file" runat="server" size="80" NAME="txtFile">
				<asp:LinkButton id="lbImport" runat="server"></asp:LinkButton></P>
			</asp:Panel><br>
			<asp:Panel id="PanelAdmin" runat="server" Visible="False">
<H4>
					<asp:Label id="lblAdmin" runat="server" ForeColor="Red"></asp:Label></H4>
<asp:Label id="lblAdminTools" runat="server" ForeColor="DimGray"></asp:Label><BR>&nbsp;<BR>
<asp:HyperLink id="hlViewVisitStats" runat="server" NavigateUrl="WikiVisitStats.aspx"></asp:HyperLink><BR>
<asp:HyperLink id="hlViewCache" runat="server" NavigateUrl="WikiCache.aspx"></asp:HyperLink><BR>
<asp:HyperLink id="hlViewLog" runat="server" NavigateUrl="WikiLog.aspx"></asp:HyperLink><BR>
<asp:HyperLink id="hlViewPlugins" runat="server" NavigateUrl="WikiPlugins.aspx"></asp:HyperLink>
</asp:Panel>
			<uc1:WikiFooter id="WikiFooter1" runat="server"></uc1:WikiFooter>
			<P></P>
		</form>
	</body>
</HTML>
