<%@ Register TagPrefix="uc1" TagName="WikiFooter" Src="WikiFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="WikiHeader" Src="WikiHeader.ascx" %>
<%@ Page language="c#" Codebehind="WikiVisitStats.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiVisitStats" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiVisitStats</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="WikiVisitStats" method="post" runat="server">
			<uc1:wikiheader id="WikiHeader1" runat="server"></uc1:wikiheader>
			<P><asp:radiobutton id="rbGraphics" runat="server" GroupName="view" Checked="True" AutoPostBack="True"
					Text="Graphics"></asp:radiobutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<asp:radiobutton id="rbVisits" runat="server" GroupName="view" AutoPostBack="True" Text="Visits"></asp:radiobutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<asp:radiobutton id="rbHits" runat="server" GroupName="view" AutoPostBack="True" Text="Hits" TextAlign="Left"></asp:radiobutton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</P>
			<P><asp:label id="lblResults" runat="server" EnableViewState="False"></asp:label></P>
			<P><asp:linkbutton id="lbPurge" runat="server"> Purge stats</asp:linkbutton>&nbsp;
				<asp:label id="lblPurge" runat="server"></asp:label><BR>
				<BR>
				<uc1:wikifooter id="WikiFooter1" runat="server"></uc1:wikifooter></P>
		</form>
	</body>
</HTML>
