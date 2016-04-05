<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiLogin.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiLogin" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiLogin</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="WikiLogin" method="post" runat="server">
			<uc1:wikiheader id="WikiHeader1" runat="server"></uc1:wikiheader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_user_guest.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<asp:Label id="lblInfo" runat="server"></asp:Label>
			<asp:Panel id="pnlLogin" runat="server">
<TABLE class="input_panel">
					<TR>
						<TD>
							<asp:Label id="lblUserName" runat="server"></asp:Label></TD>
						<TD>
							<asp:TextBox id="tbUser" runat="server"></asp:TextBox>&nbsp;
							<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="*" Font-Bold="True" Font-Size="Larger"
								ControlToValidate="tbUser"></asp:RequiredFieldValidator></TD>
					</TR>
					<TR>
						<TD>
							<asp:Label id="lblPassword" runat="server"></asp:Label></TD>
						<TD>
							<asp:TextBox id="tbPassword" runat="server" TextMode="Password"></asp:TextBox>&nbsp;
							<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="*" Font-Bold="True" Font-Size="Larger"
								ControlToValidate="tbPassword"></asp:RequiredFieldValidator></TD>
					</TR>
					<TR>
						<TD colSpan="2">
							<ASP:CHECKBOX id="cbPersist" Runat="server"></ASP:CHECKBOX></TD>
					</TR>
				</TABLE>
<asp:Button id="butSubmit" runat="server"></asp:Button>&nbsp; 
<asp:LinkButton id="lbNewUser" runat="server" CausesValidation="False" Visible="False"></asp:LinkButton>
			</asp:Panel>
			<asp:Panel id="pnlNewUser" runat="server" Visible="False">
				<TABLE class="input_panel">
					<TR>
						<TD>
							<asp:Label id="lblNewUser" runat="server"></asp:Label></TD>
						<TD>
							<asp:TextBox id="tbNewUser" runat="server"></asp:TextBox>&nbsp;
							<asp:RequiredFieldValidator id="Requiredfieldvalidator3" runat="server" ErrorMessage="*" Font-Bold="True" Font-Size="Larger"
								ControlToValidate="tbNewUser"></asp:RequiredFieldValidator></TD>
					</TR>
					<TR>
						<TD>
							<asp:Label id="lblNewPassword" runat="server"></asp:Label></TD>
						<TD>
							<asp:TextBox id="tbNewPassword" runat="server" TextMode="Password"></asp:TextBox>&nbsp;
							<asp:RequiredFieldValidator id="Requiredfieldvalidator4" runat="server" ErrorMessage="*" Font-Bold="True" Font-Size="Larger"
								ControlToValidate="tbNewPassword"></asp:RequiredFieldValidator></TD>
					</TR>
					<TR>
						<TD>
							<asp:Label id="lblRetypePassword" runat="server"></asp:Label></TD>
						<TD>
							<asp:TextBox id="tbRetypePassword" runat="server" TextMode="Password"></asp:TextBox>&nbsp;
							<asp:RequiredFieldValidator id="Requiredfieldvalidator5" runat="server" ErrorMessage="*" Font-Bold="True" Font-Size="Larger"
								ControlToValidate="tbRetypePassword"></asp:RequiredFieldValidator></TD>
					</TR>
					<TR>
						<TD>
							<asp:Label id="lblEmail" runat="server"></asp:Label></TD>
						<TD>
							<asp:TextBox id="tbEmail" runat="server"></asp:TextBox>&nbsp;
							<asp:RequiredFieldValidator id="Requiredfieldvalidator6" runat="server" ErrorMessage="*" Font-Bold="True" Font-Size="Larger"
								ControlToValidate="tbEmail"></asp:RequiredFieldValidator>
							<asp:Label id="lblEmailInfo" runat="server"></asp:Label></TD>
					</TR>
				</TABLE>
				<asp:Button id="butNewUser" runat="server"></asp:Button>
			</asp:Panel>
			<P>
				<asp:Label id="outMessage" runat="server" Width="356px"></asp:Label></P>
		</form>
		<uc1:WikiFooter id="WikiFooter1" runat="server"></uc1:WikiFooter>
	</body>
</HTML>
