<%@ Register TagPrefix="uc1" TagName="Footer" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiError.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiError" %>
<HTML>
	<HEAD>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
	</HEAD>
	<body bgcolor="#fffaf0">
		<form method="post" runat="server">
			<uc1:Header id="Header1" runat="server"></uc1:Header>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr valign=middle>
					<td class="page_title"><IMG src="images/icon_warning.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label>
					</td>
				</tr>
			</table>
			<P><IMG src="images/icon_message.gif" border="0">&nbsp;<FONT face=Arial color="red" size="4">
					<asp:Label id="lblError" runat="server">Error</asp:Label></FONT></P>
			<P align="left">
				<asp:Label id="lblErrMessage" runat="server" ForeColor="Black"></asp:Label></P>
			<P align="left">
				<asp:Label id="lblInfo" runat="server" ForeColor="Gray"></asp:Label></P>
		</form>
		<uc1:Footer id="Footer1" runat="server"></uc1:Footer>
	</body>
</HTML>
