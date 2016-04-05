<%@ Page language="c#" Codebehind="WikiUserSettings.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfUserSettings" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="WikiHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="WikiFooter.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>UserSettings</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="UserSettings" method="post" runat="server">
			<uc1:Header id="Header1" runat="server"></uc1:Header>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_user.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<asp:Panel id="pnlSettings" runat="server">
				<H4>General :</H4>
				<P>
					<asp:Label id="lblPersonnalPage" runat="server"></asp:Label></P>
				<P>
					<asp:Label id="lblRole" runat="server"></asp:Label></P>
				<P>
					<asp:CheckBox id="cbFullScreen" runat="server" Text="Start in fullscreen mode"></asp:CheckBox>&nbsp;&nbsp;
					<asp:LinkButton id="lbSaveFullScreen" runat="server">Save changes</asp:LinkButton><BR>
				</P>
				<H4>Selected pages:</H4>
				<asp:Label id="lblSelectedPages" runat="server"></asp:Label>
				<BR>
				<H4>Mailer:</H4>
				<P>Your eMail
					<asp:TextBox id="txtEMail" runat="server" Width="327px"></asp:TextBox><BR>
					Receive mail : Never
					<asp:RadioButton id="rbNone" runat="server" GroupName="frequence"></asp:RadioButton>&nbsp;&nbsp;&nbsp; 
					Every week
					<asp:RadioButton id="rbWeek" runat="server" GroupName="frequence"></asp:RadioButton>&nbsp;&nbsp;&nbsp; 
					Every day
					<asp:RadioButton id="rbDay" runat="server" GroupName="frequence"></asp:RadioButton>&nbsp;&nbsp;&nbsp; 
					In real time
					<asp:RadioButton id="rbRealTime" runat="server" GroupName="frequence"></asp:RadioButton>&nbsp;&nbsp;&nbsp;
					<BR>
					For all pages
					<asp:RadioButton id="rbAll" runat="server" GroupName="pages"></asp:RadioButton>&nbsp;&nbsp;&nbsp; 
					For selected pages only
					<asp:RadioButton id="rbSelected" runat="server" GroupName="pages"></asp:RadioButton>&nbsp;&nbsp;&nbsp;
					<asp:LinkButton id="saveEMailSettings" runat="server">Save changes</asp:LinkButton><BR>
					<asp:LinkButton id="lbCheckNow" runat="server">Check for mail now !</asp:LinkButton>&nbsp;
					<asp:Label id="lblEMail" runat="server"></asp:Label></P>
				<H4>Style :</H4>
				<asp:DropDownList id="ddlStyles" runat="server"></asp:DropDownList>
				<asp:LinkButton id="lbSaveStyle" runat="server">Save change</asp:LinkButton>
			</asp:Panel>
			<asp:Label id="lblInfo" runat="server" Visible="False"></asp:Label>
			<br>
			<uc1:Footer id="Footer1" runat="server"></uc1:Footer>
		</form>
	</body>
</HTML>
