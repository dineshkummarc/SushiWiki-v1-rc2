<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiCache.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiCache" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>wfWikiCache</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="wfWikiCache" method="post" runat="server">
			<uc1:WikiHeader id="WikiHeader1" runat="server" EnableViewState="False"></uc1:WikiHeader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_tools.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<P>
				<asp:DataGrid id="DataGridCache" runat="server" EnableViewState="False" AutoGenerateColumns="False"
					AllowSorting="True" CssClass="wikitable">
					<AlternatingItemStyle CssClass="wikitable_pyjama"></AlternatingItemStyle>
					<HeaderStyle CssClass="wikitable_header"></HeaderStyle>
					<Columns>
						<asp:BoundColumn DataField="icon" DataFormatString="&lt;img src=images/icon_{0}.gif border=0&gt;"></asp:BoundColumn>
						<asp:BoundColumn DataField="name" HeaderText="Name"></asp:BoundColumn>
						<asp:BoundColumn DataField="type" HeaderText="Type"></asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
				<asp:Label id="lblInfo" runat="server" Visible="False"></asp:Label></P>
			<uc1:WikiFooter id="WikiFooter1" runat="server" EnableViewState="False"></uc1:WikiFooter>
		</form>
	</body>
</HTML>
