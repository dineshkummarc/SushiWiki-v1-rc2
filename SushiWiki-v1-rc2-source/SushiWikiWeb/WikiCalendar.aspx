<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiCalendar.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiCalendar" %>
<%@ Import namespace="Wiki.GUI" %>
<%@ Import namespace="Wiki" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiCalendar</title>
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
					<td class="page_title"><img src="images/icon_calendar.gif" border="0">&nbsp;<asp:label id="lblTitle" runat="server" CssClass="page_title" EnableViewState="False"></asp:label></td>
				</tr>
			</table>
			<table>
				<tr>
					<td><asp:calendar id="calMonth1" runat="server" CellPadding="4" BorderColor="#999999" Font-Names="Verdana"
							Font-Size="8pt" Height="180px" ForeColor="Black" DayNameFormat="FirstLetter" Width="200px"
							BackColor="White" ShowGridLines="True">
							<TodayDayStyle ForeColor="Black" BackColor="#CCCCCC"></TodayDayStyle>
							<SelectorStyle BackColor="#CCCCCC"></SelectorStyle>
							<NextPrevStyle VerticalAlign="Bottom"></NextPrevStyle>
							<DayHeaderStyle Font-Size="7pt" Font-Bold="True" BackColor="#CCCCCC"></DayHeaderStyle>
							<SelectedDayStyle Font-Bold="True" ForeColor="White" BackColor="#666666"></SelectedDayStyle>
							<TitleStyle Font-Bold="True" BorderColor="Black" BackColor="#999999"></TitleStyle>
							<WeekendDayStyle BackColor="#FFFFCC"></WeekendDayStyle>
							<OtherMonthDayStyle ForeColor="Gray"></OtherMonthDayStyle>
						</asp:calendar></td>
					<td><asp:calendar id="calMonth2" runat="server" CellPadding="4" BorderColor="#999999" Font-Names="Verdana"
							Font-Size="8pt" Height="180px" ForeColor="Black" DayNameFormat="FirstLetter" Width="200px"
							BackColor="White" ShowGridLines="True">
							<TodayDayStyle ForeColor="Black" BackColor="#CCCCCC"></TodayDayStyle>
							<SelectorStyle BackColor="#CCCCCC"></SelectorStyle>
							<NextPrevStyle VerticalAlign="Bottom"></NextPrevStyle>
							<DayHeaderStyle Font-Size="7pt" Font-Bold="True" BackColor="#CCCCCC"></DayHeaderStyle>
							<SelectedDayStyle Font-Bold="True" ForeColor="White" BackColor="#666666"></SelectedDayStyle>
							<TitleStyle Font-Bold="True" BorderColor="Black" BackColor="#999999"></TitleStyle>
							<WeekendDayStyle BackColor="#FFFFCC"></WeekendDayStyle>
							<OtherMonthDayStyle ForeColor="Gray"></OtherMonthDayStyle>
						</asp:calendar></td>
					<td><asp:calendar id="calMonth3" runat="server" CellPadding="4" BorderColor="#999999" Font-Names="Verdana"
							Font-Size="8pt" Height="180px" ForeColor="Black" DayNameFormat="FirstLetter" Width="200px"
							BackColor="White" ShowGridLines="True">
							<TodayDayStyle ForeColor="Black" BackColor="#CCCCCC"></TodayDayStyle>
							<SelectorStyle BackColor="#CCCCCC"></SelectorStyle>
							<NextPrevStyle VerticalAlign="Bottom"></NextPrevStyle>
							<DayHeaderStyle Font-Size="7pt" Font-Bold="True" BackColor="#CCCCCC"></DayHeaderStyle>
							<SelectedDayStyle Font-Bold="True" ForeColor="White" BackColor="#666666"></SelectedDayStyle>
							<TitleStyle Font-Bold="True" BorderColor="Black" BackColor="#999999"></TitleStyle>
							<WeekendDayStyle BackColor="#FFFFCC"></WeekendDayStyle>
							<OtherMonthDayStyle ForeColor="Gray"></OtherMonthDayStyle>
						</asp:calendar></td>
				</tr>
			</table>
			<br>
			<img src="images/icon_event.gif" border="0"><asp:label id="lblMonth1" runat="server" Font-Size="Larger" Font-Bold="True"></asp:label><br>
			<asp:datagrid id="dgEvents1" runat="server" CssClass="datalist" AutoGenerateColumns="False">
				<AlternatingItemStyle CssClass="wikitable_pyjama"></AlternatingItemStyle>
				<ItemStyle CssClass="wikitable"></ItemStyle>
				<HeaderStyle CssClass="wikitable_header"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="date" HeaderText="Date"></asp:BoundColumn>
					<asp:BoundColumn DataField="subject" HeaderText="Subject"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="page" DataNavigateUrlFormatString="wiki.aspx?page={0}" DataTextField="page"
						HeaderText="Page" DataTextFormatString="{0}"></asp:HyperLinkColumn>
					<asp:BoundColumn DataField="comments" HeaderText="Comments"></asp:BoundColumn>
				</Columns>
			</asp:datagrid><br>
			<img src="images/icon_event.gif" border="0"><asp:label id="lblMonth2" runat="server" Font-Size="Larger" Font-Bold="True"></asp:label><br>
			<asp:datagrid id="dgEvents2" runat="server" CssClass="datalist" AutoGenerateColumns="False">
				<AlternatingItemStyle CssClass="wikitable_pyjama"></AlternatingItemStyle>
				<HeaderStyle CssClass="wikitable_header"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="date" HeaderText="Date"></asp:BoundColumn>
					<asp:BoundColumn DataField="subject" HeaderText="Subject"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="page" DataNavigateUrlFormatString="wiki.aspx?page={0}" DataTextField="page"
						HeaderText="Page" DataTextFormatString="{0}"></asp:HyperLinkColumn>
					<asp:BoundColumn DataField="comments" HeaderText="Comments"></asp:BoundColumn>
				</Columns>
			</asp:datagrid><br>
			<img src="images/icon_event.gif" border="0"><asp:label id="lblMonth3" runat="server" Font-Size="Larger" Font-Bold="True"></asp:label><br>
			<asp:datagrid id="dgEvents3" runat="server" AutoGenerateColumns="False" CssClass="datalist">
				<Columns>
					<asp:BoundColumn DataField="date" HeaderText="Date"></asp:BoundColumn>
					<asp:BoundColumn DataField="subject" HeaderText="Subject"></asp:BoundColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="page" DataNavigateUrlFormatString="wiki.aspx?page={0}" DataTextField="page"
						HeaderText="Page" DataTextFormatString="{0}"></asp:HyperLinkColumn>
					<asp:BoundColumn DataField="comments" HeaderText="Comments"></asp:BoundColumn>
				</Columns>
			</asp:datagrid><br>
			<uc1:wikifooter id="WikiFooter1" runat="server"></uc1:wikifooter></form>
	</body>
</HTML>
