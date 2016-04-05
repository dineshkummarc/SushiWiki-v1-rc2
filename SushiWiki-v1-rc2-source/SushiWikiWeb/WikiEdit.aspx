<%@ Page language="c#" Codebehind="WikiEdit.aspx.cs" AutoEventWireup="false" Inherits="Wiki.GUI.wfWikiEdit" validateRequest="false" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="WikiHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="WikiFooter.ascx" %>
<%@ Import namespace="Wiki.GUI" %>
<HTML>
	<HEAD>
		<script>

	function PreviewPage() 
	{
		day = new Date();
		id = day.getTime();
		eval("page" + id + " = window.open('WikiPreview.aspx?id=' + editform.ddlVersions.value ,id,'toolbar=0,scrollbars=1,location=0,statusbar=0,menubar=0,resizable=1,width=600,height=400,left=300,top=300');");
	}	
	function QuickHelp() 
	{
		day = new Date();
		id = day.getTime();
		eval("page" + id + " = window.open('WikiEditQuickHelp.aspx' ,id,'toolbar=0,scrollbars=1,location=0,statusbar=0,menubar=0,resizable=1,width=600,height=400,left=300,top=300');");
	}	
		</script>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
	</HEAD>
	<body>
		<form method="post" runat="server" id="editform">
			<INPUT id="richtextsource" type="hidden" name="richtextsource">
			<uc1:header id="Header1" runat="server"></uc1:header>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="page_title"><img src="images/icon_edit.gif" border="0">&nbsp;
						<asp:Label id="lblPageTitle" runat="server" CssClass="page_title"></asp:Label><asp:Label id="lblPageName" runat="server" CssClass="page_title"></asp:Label></td>
				</tr>
			</table>
			<table class="menu" width="100%">
				<tr class="menu">
					<td class="menu"><%=WikiGui.GetString("Gui.WikiEdit.Edit")%>
						:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%=WikiGui.GetString("Gui.WikiEdit.Format")%>
						:<asp:dropdownlist id="ddlPageType" runat="server" autopostback="True">
							<asp:ListItem Value="WIKI" Selected="True">Wiki</asp:ListItem>
							<asp:ListItem Value="ASCII">Ascii</asp:ListItem>
							<asp:ListItem Value="HTML">HTML</asp:ListItem>
						</asp:dropdownlist>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<%=WikiGui.GetString("Gui.WikiEdit.Template")%>
						:
						<asp:DropDownList id="ddlTemplates" runat="server" Width="288px" Height="24px"></asp:DropDownList>
						<asp:LinkButton id="lbCopyTemplate" runat="server"></asp:LinkButton>
					</td>
				</tr>
			</table>
			<asp:panel id="PanelHTMLEditor" runat="server" Width="665px" Visible="false">
<SCRIPT src="js/richedit.js" type="text/javascript"></SCRIPT>
<IMG id="imgSetBold" onclick="document.all.richedit.setBold(); document.all.richedit.frameWindow.focus();"
					src="images/htmleditor_bold.gif" border="1"> <IMG id="imgSetItalic" onclick="document.all.richedit.setItalic(); document.all.richedit.frameWindow.focus();"
					src="images/htmleditor_italic.gif" border="1"> <IMG id="imgSetUnderline" onclick="document.all.richedit.setUnderline(); document.all.richedit.frameWindow.focus();"
					src="images/htmleditor_underline.gif" border="1">&nbsp; <INPUT onclick="DisplayHTML();" type="radio" name="editor"> 
<asp:Label id="lblHtmlEditor" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp; <INPUT onclick="DisplayRichText();" type="radio" CHECKED name="editor"> 
<asp:Label id="lblRichTextEditor" runat="server"></asp:Label><BR><TEXTAREA class="editZone" id="rawEdit" onblur="RawOnBlur();" style="DISPLAY: none" name="rawEdit"
					rows="24" cols="115"></TEXTAREA> 
<IFRAME class="richEdit" id="richedit" onblur="RichTextOnBlur();" frameBorder="1" width="100%"
					height="300" runat="server" >
					<%=this.RichTextSource%>
				</IFRAME>
<SCRIPT language="jscript">
function RichTextOnBlur()
{
	editform.richtextsource.value = document.all.richedit.getHTML();
}

function RawOnBlur()
{
	editform.richtextsource.value = editform.rawEdit.value;
}

function DisplayHTML()
{
	editform.rawEdit.style.display="";
	document.all.richedit.style.display = "none";
	editform.rawEdit.value = document.all.richedit.getHTML();
	editform.imgSetBold.style.display="none";
	editform.imgSetItalic.style.display="none";
	editform.imgSetUnderline.style.display="none";
}

function DisplayRichText()
{
	editform.rawEdit.style.display="none";
	document.all.richedit.style.display = "";
	document.all.richedit.setHTML(editform.rawEdit.value);
	editform.imgSetBold.style.display="";
	editform.imgSetItalic.style.display="";
	editform.imgSetUnderline.style.display="";
}

editform.richtextsource.value = document.all.richedit.innerHTML;
				</SCRIPT>
</asp:panel>
			<asp:textbox id="txtPageContent" runat="server" TextMode="MultiLine" Rows="24" Columns="115"
				CssClass="editZone" BackColor="#FFFFC0"></asp:textbox><br>
			<asp:label id="lblInfo" runat="server" CssClass="wikiinfo"></asp:label><A href="javascript:QuickHelp()"><IMG src="images/icon_help.gif" border="0">
				<%=WikiGui.GetString("Gui.WikiEdit.Help")%>
			</A>
			<br>
			<P align="left"><asp:button id="cmdSaveAndReturn" runat="server" Width="200" Height="24" ForeColor="Green"></asp:button>&nbsp;
				<asp:button id="cmdSave" runat="server" Width="131" Height="24" ForeColor="Green"></asp:button>&nbsp;
				<asp:button id="cmdCancel" runat="server" Width="77px" Height="24"></asp:button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
				&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<asp:button id="cmdDelete" runat="server" Width="131" Height="24" ForeColor="Red"></asp:button></P>
			<P align="left"><asp:panel id="panelVersions" runat="server"><asp:Label id="lblPreviousVersions" runat="server"></asp:Label><asp:dropdownlist id="ddlVersions" runat="server" DataTextField="label" DataValueField="id"></asp:dropdownlist>&nbsp; 
<asp:linkbutton id="lbSelectVersion" runat="server"></asp:linkbutton>&nbsp;&nbsp; 
<A href="javascript:PreviewPage()"><IMG src="images/icon_preview.gif" border="0">
						<asp:Label id="lblPreviewVersion" runat="server"></asp:Label></A></P>
			</asp:panel><asp:Label id="lblHiddenFields" runat="server"></asp:Label></form>
		<table class="menu" cellSpacing="1" width="100%" border="0">
			<tr class="menu">
				<td class="menu" align="right">&nbsp;
				</td>
			</tr>
		</table>
		<asp:label id="lblOwner" runat="server" Visible="False"></asp:label>
		<uc1:footer id="Footer1" runat="server"></uc1:footer>
	</body>
</HTML>
