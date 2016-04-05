<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiSearch.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiSearch" %>
<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<HTML>
	<HEAD>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
	</HEAD>
	<body bgcolor="#fffaf0">
		<form method="post" runat="server">
			<uc1:WikiHeader id="WikiHeader1" runat="server"></uc1:WikiHeader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_fullsearch.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<P>
				<asp:Label id="lblLabel" runat="server"></asp:Label>
				<asp:TextBox id="tbText" runat="server" Width="228px"></asp:TextBox></A>
				<asp:LinkButton id="lbSearch" runat="server">Search</asp:LinkButton></P>
			<P>
				<asp:DataGrid id="linksGrid" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="15"
					ShowHeader="False">
					<AlternatingItemStyle BackColor="White"></AlternatingItemStyle>
					<ItemStyle Font-Size="Smaller" Font-Names="Arial" BackColor="#E0E0E0"></ItemStyle>
					<HeaderStyle Font-Names="Arial"></HeaderStyle>
					<Columns>
						<asp:HyperLinkColumn DataNavigateUrlField="title" DataNavigateUrlFormatString="Wiki.aspx?page={0}" DataTextField="title"
							HeaderText="Page"></asp:HyperLinkColumn>
						<asp:BoundColumn DataField="found" HeaderText="Found text" DataFormatString="&lt;pre&gt;{0}&lt;/pre&gt;"></asp:BoundColumn>
					</Columns>
					<PagerStyle Font-Names="Arial Black" Position="TopAndBottom" CssClass="wikiinfo" Mode="NumericPages"></PagerStyle>
				</asp:DataGrid></P>
			<P>
				<asp:Label id="lblSearch" runat="server" CssClass="wikiinfo"></asp:Label></P>
			<P>
				<table class="menu" cellSpacing="1" width="100%" border="0">
					<tr class="menu">
						<td align="right" class="menu">&nbsp;
						</td>
					</tr>
				</table>
				<uc1:WikiFooter id="WikiFooter1" runat="server"></uc1:WikiFooter></P>
		</form>
	</body>
</HTML>
