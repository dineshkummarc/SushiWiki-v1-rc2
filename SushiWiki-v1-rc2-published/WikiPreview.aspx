<%@ Page language="c#" Codebehind="WikiPreview.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiPreview" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<LINK rel="stylesheet" type="text/css" href="styles/green.css">
<HTML>
	<HEAD>
		<title>WikiPreview</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="WikiPreview" method="post" runat="server">
			<asp:Label id="lblPageContent" runat="server"></asp:Label>
		</form>
	</body>
</HTML>
