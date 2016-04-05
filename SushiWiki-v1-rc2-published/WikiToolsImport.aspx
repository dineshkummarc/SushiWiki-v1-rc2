<%@ Page language="c#" Codebehind="WikiToolsImport.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiToolsImport" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiToolsImport</title>
		<meta content="False" name="vs_showGrid">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript">
	function SelectAll()
	{
		var i =0;
		var n = window.document.all.length;
		for (i=0 ; i<n ; i++)
		{
			var item = window.document.all(i);
			if ( (item.tagName == "INPUT") && (item.type == "checkbox") )
			{
				item.checked = true;
			}
		}
	}
		</script>
	</HEAD>
	<body>
		<label id="info"></label>
		<form id="WikiToolsImport" method="post" runat="server">
			<P></P>
			<uc1:wikiheader id="WikiHeader1" runat="server"></uc1:wikiheader>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_tools.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<P><br>
				<A href="javascript:SelectAll();">
					<asp:Label id="lblSelect" runat="server"></asp:Label></A>
			</P>
			<P><asp:datagrid id=dgPages runat="server" AutoGenerateColumns="False" DataSource="<%# dsPages %>" CssClass="datalist">
					<HeaderStyle Font-Bold="True" Wrap="False" HorizontalAlign="Left" ForeColor="White" BackColor="Gray"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
							<ItemTemplate>
								<asp:Checkbox ID="chkSelection" Runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="title" SortExpression="title" HeaderText="Page name" DataFormatString="{0}"></asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Existing ?">
							<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
							<ItemTemplate>
								<asp:Label ID="Label1" Runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:datagrid></P>
			<P><asp:linkbutton id="lbImport" runat="server"></asp:linkbutton></P>
			<P><br>
				<asp:Label id="lblInfo" runat="server"></asp:Label>
				<br>
			</P>
			<table class="menu" cellSpacing="1" width="100%" border="0">
				<TBODY>
					<tr class="menu">
						<td align="right" class="menu">&nbsp;
						</td>
					</tr>
				</TBODY>
			</table>
			<uc1:WikiFooter id="WikiFooter3" runat="server"></uc1:WikiFooter></form>
	</body>
</HTML>
