<%@ Page language="c#" Codebehind="WikiEvents.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiEvents" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Import namespace="Wiki.GUI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiEvents</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<uc1:WikiHeader id="WikiHeader1" runat="server"></uc1:WikiHeader>
			<table width="100%" class="page_title">
				<tr>
					<td>
						<img border="0" src="images/icon_calendar.gif">&nbsp;
						<asp:Label id="lblTitle" runat="server"></asp:Label>&nbsp;<asp:Label id="lblMonth" runat="server" DESIGNTIMEDRAGDROP="90"></asp:Label>
					</td>
				</tr>
			</table>
			<%=WikiGui.GetString("Gui.Wiki.YouAreIn")%>
			&nbsp;<asp:label id="lblHisto" runat="server" EnableViewState="False" CssClass="status_bar"></asp:label>
			<br>
			<table class="input_panel">
				<TR>
					<TD>&nbsp;</TD>
					<TD>
						<asp:Calendar id="CalDate" runat="server" Width="350px" Height="180px" BorderColor="#999999" BackColor="White"
							CellPadding="4" ForeColor="Black" DayNameFormat="FirstLetter" Font-Size="8pt" Font-Names="Verdana"
							ShowGridLines="True">
							<TodayDayStyle ForeColor="Black" BackColor="#CCCCCC"></TodayDayStyle>
							<SelectorStyle BackColor="#CCCCCC"></SelectorStyle>
							<NextPrevStyle VerticalAlign="Bottom"></NextPrevStyle>
							<DayHeaderStyle Font-Size="7pt" Font-Bold="True" BackColor="#CCCCCC"></DayHeaderStyle>
							<SelectedDayStyle Font-Bold="True" ForeColor="White" BackColor="#666666"></SelectedDayStyle>
							<TitleStyle Font-Bold="True" BorderColor="Black" BackColor="#999999"></TitleStyle>
							<WeekendDayStyle BackColor="#FFFFCC"></WeekendDayStyle>
							<OtherMonthDayStyle ForeColor="#808080"></OtherMonthDayStyle>
						</asp:Calendar></TD>
				</TR>
				<tr>
					<td style="TEXT-ALIGN: right"><asp:Label id="lblDate" runat="server"></asp:Label></td>
					<td>
						<asp:TextBox id="tbDate" runat="server"></asp:TextBox>
						<asp:RequiredFieldValidator id="validDate" runat="server" Font-Bold="True" ErrorMessage="!!!" ControlToValidate="tbDate"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td style="TEXT-ALIGN: right"><asp:Label id="lblPage" runat="server"></asp:Label></td>
					<td><asp:Label id="lblPageValue" runat="server" Font-Bold="True"></asp:Label></td>
				<tr>
					<td valign="top" style="TEXT-ALIGN: right"><asp:Label id="lblSubject" runat="server"></asp:Label></td>
					<td><asp:TextBox id="tbSubject" runat="server" Columns="50" Rows="1" MaxLength="40"></asp:TextBox>
						<asp:RequiredFieldValidator id="validSubject" runat="server" Font-Bold="True" ErrorMessage="!!!" ControlToValidate="tbSubject"></asp:RequiredFieldValidator></td>
				<tr>
					<td valign="top" style="TEXT-ALIGN: right"><asp:Label id="lblComments" runat="server"></asp:Label></td>
					<td><asp:TextBox id="tbComments" runat="server" TextMode="MultiLine" Rows="4" Columns="50"></asp:TextBox></td>
				</tr>
			</table>
			<BR>
			<asp:Button id="butAddEvent" runat="server"></asp:Button>
			<P></P>
			<P>
				<asp:DataGrid id="dgEvents" runat="server" AutoGenerateColumns="False">
					<AlternatingItemStyle CssClass="wikitable_pyjama"></AlternatingItemStyle>
					<ItemStyle CssClass="wikitable"></ItemStyle>
					<HeaderStyle CssClass="wikitable_header"></HeaderStyle>
					<Columns>
						<asp:ButtonColumn Text="<img src=images/icon_event.gif border=0>"></asp:ButtonColumn>
						<asp:BoundColumn DataField="date" HeaderText="Date" DataFormatString="{0:d}"></asp:BoundColumn>
						<asp:BoundColumn DataField="subject" HeaderText="Subject"></asp:BoundColumn>
						<asp:BoundColumn DataField="comments" HeaderText="Comments"></asp:BoundColumn>
						<asp:ButtonColumn Text="Delete" CommandName="Delete"></asp:ButtonColumn>
					</Columns>
				</asp:DataGrid>
				<uc1:WikiFooter id="WikiFooter1" runat="server"></uc1:WikiFooter></P>
		</form>
	</body>
</HTML>
