<%@ Register TagPrefix="uc1" TagName="Footer" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiAttach.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiAttach" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiAttach</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="WikiAttach" method="post" encType="multipart/form-data" runat="server">
			<uc1:header id="Header1" runat="server"></uc1:header>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><IMG src="images/bicon_attachements.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label>
						<asp:hyperlink id="hlPage" runat="server" CssClass="page_title"></asp:hyperlink></td>
				</tr>
			</table>
			<table class="input_panel">
				<tr>
					<td align="right"><asp:label id="lblLocalFile" runat="server"></asp:label></td>
					<td><INPUT id="txtFile" type="file" size="80" name="txtFile" runat="server">
						<asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" Font-Size="Larger" ControlToValidate="txtFile"
							Font-Bold="True" ErrorMessage="*"></asp:requiredfieldvalidator></td>
				</tr>
				<tr>
					<td align="right"><asp:label id="lblComment" runat="server"></asp:label></td>
					<td><INPUT id="txtComment" type="text" size="80" name="txtComment" runat="server"></td>
				</tr>
				<tr>
					<td><INPUT id="txtPage" type="hidden" name="txtPage" runat="server">
						<asp:button id="butSendFile" runat="server"></asp:button></td>
				</tr>
			</table>
			<asp:label id="lblInfo" runat="server" ForeColor="#FF8000"></asp:label><br>
			<asp:datagrid id=dgFiles runat="server" CssClass="datalist" AutoGenerateColumns="False" DataSource="<%# dsFiles %>" ShowHeader="False">
				<HeaderStyle Font-Bold="True"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<IMG src='images/icon_file_<%# DataBinder.Eval(Container.DataItem,"Type") %>.gif' border=0>
							<asp:LinkButton id="LinkButton1" runat="server" Text="Delete" CommandName="Delete" CausesValidation="false"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="view" DataFormatString="&lt;A href='{0}'&gt;Download/View&lt;/A&gt;&#160;&#160;"></asp:BoundColumn>
					<asp:BoundColumn DataField="name" HeaderText="Name"></asp:BoundColumn>
					<asp:BoundColumn DataField="label" HeaderText="Label"></asp:BoundColumn>
					<asp:BoundColumn DataField="Type" HeaderText="Type"></asp:BoundColumn>
					<asp:BoundColumn DataField="Size" HeaderText="Size" DataFormatString="&lt;P align=right&gt;{0:#,###} bytes&lt;/P&gt;"></asp:BoundColumn>
				</Columns>
			</asp:datagrid>
			<asp:label id="lblAttachements" runat="server" Font-Size="X-Small"></asp:label><br>
			<uc1:footer id="Footer1" runat="server"></uc1:footer>
		</form>
	</body>
</HTML>
