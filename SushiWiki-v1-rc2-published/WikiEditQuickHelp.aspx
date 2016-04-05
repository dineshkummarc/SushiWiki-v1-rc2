<%@ Page language="c#" Codebehind="WikiEditQuickHelp.aspx.cs" AutoEventWireup="false" Inherits="Wiki.WikiEditQuickHelp" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WikiEditQuickHelp</title>
		<LINK rel="stylesheet" type="text/css" href='styles/<%=(string)Session["userstyle"]%>'>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<P>
				You can use formats :
				<BR>
				- Wiki formatting<BR>
				- Plain text formatting
				<BR>
				- or HTML formatting.</P>
			<P><STRONG><U>Wiki formating :</U></STRONG></P>
			<table>
				<tr bgcolor="lightgrey">
					<td>Format</td>
					<td>Example</td>
					<td>Result</td>
				</tr>
				<tr>
					<td>Bold</td>
					<td><FONT color="gray">*blabla*</FONT></td>
					<td><b>blabla</b></td>
				</tr>
				<tr>
					<td>Red</td>
					<td><FONT color="gray">*red*blabla*</FONT></td>
					<td><font color="red">blabla</font></td>
				</tr>
				<tr>
					<td>Blue</td>
					<td><FONT color="gray">*blue*blabla*</FONT></td>
					<td><font color="blue">blabla</font></td>
				</tr>
				<tr>
					<td>Green</td>
					<td><FONT color="gray">*green*blabla*</FONT></td>
					<td><font color="green">blabla</font></td>
				</tr>
				<tr>
					<td>Italic</td>
					<td><FONT color="gray">_blabla_</FONT></td>
					<td><i>blabla</i></td>
				</tr>
				<tr>
					<td>Bold Italic</td>
					<td><FONT color="gray">__blabla__</FONT></td>
					<td><b><i>blabla</i></b></td>
				</tr>
				<tr>
					<td>Fixed font</td>
					<td><FONT color="gray">=blabla=</FONT></td>
					<td><code>blabla</code></td>
				</tr>
				<tr>
					<td>Bold fixed font</td>
					<td><FONT color="gray">==blabla==</FONT></td>
					<td><b><code>blabla</code></b></td>
				</tr>
			</table>
			<P>
				<BR>
				Level 1 Title : <FONT color="gray">---+ My Title<BR>
				</FONT>Level&nbsp;2 Title : <FONT color="gray">---++ My Title</FONT><BR>
				...<BR>
				Level&nbsp;6 Title : <FONT color="gray">---++++++ My Title </FONT>
			</P>
			<P>Table :
			</P>
			<P>|Header One|Header Two| Header Three|<BR>
				|First cell|Second Cell|Third Cell|</P>
			<P>And many more</P>
			<P>&nbsp;</P>
			<P><STRONG><U>Plain text formatting :</U></STRONG></P>
			<P>What you type is what is displayed. Nothing else !</P>
			<P><STRONG><U>HTML formatting :</U></STRONG></P>
			<P>You can use all HTML tags. To create a link to a Wiki page put the name between 
				double [].
				<BR>
				Example : <FONT color="gray">[[MyPage]]</FONT></P>
			<P><STRONG><U>Wiki Keywords :</U></STRONG></P>
			<P>These keywords works for Wiki formating and HTML formating.</P>
			<P>To add an attached image : <FONT color="gray">%ATTACHEDIMAGE(mypicture.gif)%<BR>
				</FONT>To add inline and colorize an attached code source file (.cs, .py, .vb 
				or .js) : <FONT color="gray">%ATTACHEDCODE(foobar.py)%<BR>
				</FONT>To add an attached file : <FONT color="gray">%ATTACHEDFILE(myfile.doc,file 
					link label)%<BR>
				</FONT><FONT color="#000000">To insert a RSS feed : </FONT><FONT color="gray">%RSS(url)%
				</FONT>
			</P>
			<P><FONT color="#808080"></FONT>&nbsp;</P>
			<P><FONT color="gray"><BR>&nbsp;</P>
			</FONT>
		</form>
	</body>
</HTML>
