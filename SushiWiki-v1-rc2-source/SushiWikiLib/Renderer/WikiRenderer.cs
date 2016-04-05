using System;
using System.Text.RegularExpressions;
using Wiki.Render;
using Wiki.GUI;
using System.Text;
using System.Collections;
using Wiki.Macros;
using Wiki;
using Wiki.Tools;
using Wiki.Storage;
using Wiki.Storage.SQL;
using Wiki.Storage.XML;


namespace Wiki.Render
{
	/// <summary>
	/// Wiki page renderer
	/// </summary>
	public class WikiRenderer : BaseRenderer
	{

		public override string Format(bool preview)
		{
			WikiRenderWiki.STableEditionRequest tableEditionRequest;
			tableEditionRequest = m_items["tableRequest"] as WikiRenderWiki.STableEditionRequest;
			if (tableEditionRequest == null) 
			{
				tableEditionRequest = new Wiki.Render.WikiRenderWiki.STableEditionRequest();
				tableEditionRequest.SetNothingToDoMode();
			}

			// We are going to process data line by line
			string [] lines = content.Split (new char[] {'\n'}) ;
			StringBuilder result = new StringBuilder();

			// apply parsing to each line
			foreach (string l in lines)
			{
				string line= null;
				// Verbatim
				bool previousInMultiLineVerbatim = pc.inMultiLineVerbatim;
				line = ParseMultilineVerbatim(l);
				if ( (!pc.inVerbatim) && (!pc.inMultiLineVerbatim) && (!previousInMultiLineVerbatim) )
				{
					if (tableEditionRequest.NothingToDoMode)
					{ // No action requested on tables
						line = ParseHTMLTags(line);
						line = ParseWikiKeywords(line);
						line = WikiRender.ReplaceHtmlSpecialCharacters (line) ;
					}
					line = ParseLongZones (line) ;
					if (tableEditionRequest.NothingToDoMode) 
					{ // No action requested on tables
						line = ParseTextFormating (line) ;
					}
					line = RenderTable(line);
					if ( (tableEditionRequest.GetLineMode) && (tableEditionRequest.IsContextOnTableLine(pc)) )
					{ // Return requested table line
						return l;
					}
					if (tableEditionRequest.NothingToDoMode)
					{ // No action requested on tables
						line = ParseNestedListItems(line);
						if (!preview)
						{ // No links in preview mode
							line = ParseHyperLinks (line) ;
							line = extractWikiNames.Replace(line,new MatchEvaluator(this.WikiNameEvaluator));
							// Must be last action !
						}
					}
				}
				if (pc.inTable == ParsingContext.inTableStatus.none)  
				{ // Add <br> only if we are not parsing table
					line += "<br />" ; 
				}
				if (tableEditionRequest.UpdateLineMode)
				{ // Update requested table line
					if (tableEditionRequest.IsContextOnTableLine(pc))
					{
						result.AppendFormat("{0}\n",tableEditionRequest.LineData);
						tableEditionRequest.StopSearching();
					}
					else result.AppendFormat("{0}\n",l);
				}
				else if (tableEditionRequest.AddLineMode)
				{ // Add line to requested table
					if (tableEditionRequest.IsContextAfterLastLine(pc))
					{
						result.AppendFormat("{0}\n",tableEditionRequest.LineData);
						tableEditionRequest.StopSearching();
					}
					result.AppendFormat("{0}\n",l);
				}
				else result.Append(line);
			}
			if ( (tableEditionRequest.NothingToDoMode) && (pc.inTable != ParsingContext.inTableStatus.none) )
			{ // End table if needed
				result.Append(RenderTable(""));
			}
			if ( (tableEditionRequest.AddLineMode) && (pc.inTable != ParsingContext.inTableStatus.none) )
			{// Last line is the table header -> Add requested line
				result.AppendFormat("{0}\n",tableEditionRequest.LineData);
			}
			return pc.UnhideAllStrings(result.ToString()).TrimEnd('\n');
		}




		public WikiRenderer(string content, string pageName)
		{
			extractWikiNames = new Regex ( 
				@"\b(?<wikiName>
				(?:\p{Lu}[\d\p{Ll}\p{Lu}]+\.?)+		#Cap followed by one or more lowercase (or uppercase) letter or number (with optional inlined period)
				(?:\p{Lu}[\d\p{Ll}\p{Lu}]+))\b			#Followed by same pattern (without period)
				|
				\b(?<wikiName>\p{Lu}{3,})\b	#All caps
				|							#Or
				\[\[						#First bracket
				(?<pageNameAlone>[^\[\]]*)	#Page name (no anchor)
				\]\]						#End
				|							#Or
				\[\[					
				(?<pageName>[^\[\]]*)		#Page name
				\]
				\[
				(?<anchor>[^\[\]]*)			#anchor text
				\]\]",RegexOptions.IgnorePatternWhitespace
				|RegexOptions.Compiled) ;

			this.content = content;
			pc = new ParsingContext(pageName);
		}

		public string ParseLongZones (string str)
		{
			while (spd_ContainProtected(str,"<verbatim>","</verbatim>")) { str = spd_Protect(str,"<verbatim>","</verbatim>","<CODE>","</CODE>"); }
			while (spd_ContainProtected(str,"<pre>","</pre>")) { str = spd_Protect(str,"<pre>","</pre>","",""); }

			return str;
		}
		#region ParseLongZones sub methods

		private bool spd_ContainProtected(string str,string prefix, string post)
		{
			int a = str.IndexOf(WikiRender.ReplaceHtmlSpecialCharacters(prefix));
			int b = str.IndexOf(WikiRender.ReplaceHtmlSpecialCharacters(post));
			return ( (a != -1) && (b != -1) && (a<b) );
		}

		private string spd_Protect(string str,string prefix, string post,string newprefix, string newpost)
		{
			int a = str.IndexOf(WikiRender.ReplaceHtmlSpecialCharacters(prefix));
			int b = str.IndexOf(WikiRender.ReplaceHtmlSpecialCharacters(post));
			int m = WikiRender.ReplaceHtmlSpecialCharacters(prefix).Length;
			int n = WikiRender.ReplaceHtmlSpecialCharacters(post).Length;
			return str.Substring(0,a) + pc.HideString(newprefix + str.Substring(a+m,b-a-m) + newpost) + str.Substring(b+n);
		}
		#endregion

		#region Wiki Formating Functions


		/// <summary>
		/// Long zones tags ( verbatim , pre )
		/// Uses spd_ContainProtected() and spd_Protect()
		/// </summary>
		/// <param name="str">initial text</param>
		/// <param name="pc">parsing context</param>
		/// <returns>parsed text</returns>

		/// <summary>
		/// Text formating (bold, headings ...)
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>parsed text</returns>
		public string ParseTextFormating (string str)
		{
			// In red
			str = Regex.Replace (str, @"(^|\s)\*red\*([^\s]+?|[^\s].*?[^\s])\*(\s|$|\.|,|:|;|\?|\!)", "$1<b style='color:#F00'>$2</b>$3") ;
			// In green
			str = Regex.Replace (str, @"(^|\s)\*green\*([^\s]+?|[^\s].*?[^\s])\*(\s|$|\.|,|:|;|\?|\!)", "$1<b style='color:#0F0'>$2</b>$3") ;
			// In blue
			str = Regex.Replace (str, @"(^|\s)\*blue\*([^\s]+?|[^\s].*?[^\s])\*(\s|$|\.|,|:|;|\?|\!)", "$1<b style='color:#00F'>$2</b>$3") ;
			// Bold
			str = Regex.Replace (str, @"(^|\s)\*([^\s]+?|[^\s].*?[^\s])\*(\s|$|\.|,|:|;|\?|\!)", "$1<b>$2</b>$3") ;
			// Bold Italic
			str = Regex.Replace (str, @"(^|\s)__([^\s]+?|[^\s].*?[^\s])__(\s|$|\.|,|:|;|\?|\!)", "$1<b><i>$2</i></b>$3") ;
			// Italic
			str = Regex.Replace (str, @"(^|\s)_([^\s]+?|[^\s].*?[^\s])_(\s|$|\.|,|:|;|\?|\!)", "$1<i>$2</i>$3") ;
			// Bold Fixed font
			str = Regex.Replace (str, @"(^|\s)==([^\s]+?|[^\s].*?[^\s])==(\s|$|\.|,|:|;|\?|\!)", "$1<code><b>$2</b></code>$3") ;
			// Fixed font
			str = Regex.Replace (str, @"(^|\s)=([^\s]+?|[^\s].*?[^\s])=(\s|$|\.|,|:|;|\?|\!)", "$1<code>$2</code>$3") ;
			// Heading 1-6
			str = Regex.Replace (str, @"\-{3}\+{6}(.+)\s*$", "<h6>$1</h6>") ;
			str = Regex.Replace (str, @"\-{3}\+{5}(.+)\s*$", "<h5>$1</h5>") ;
			str = Regex.Replace (str, @"\-{3}\+{4}(.+)\s*$", "<h4>$1</h4>") ;
			str = Regex.Replace (str, @"\-{3}\+{3}(.+)\s*$", "<h3>$1</h3>") ;
			str = Regex.Replace (str, @"\-{3}\+{2}(.+)\s*$", "<h2>$1</h2>") ;
			str = Regex.Replace (str, @"\-{3}\+{1}(.+)\s*$", "<h1>$1</h1>") ;
			// Horizontal line
			str = Regex.Replace (str, "^-{4,}", "<hr>") ;

			return str;
		}

		/// <summary>
		/// Some HTML tags supported in Wiki format
		/// </summary>
		/// <param name="str">initial text</param>
		/// <param name="pc">parsing context</param>
		/// <returns>parsed text</returns>
		public string ParseHTMLTags(string str)
		{
			// link
			str = Regex.Replace (str, @"<a([\s|\S]*)</a>",
				ParsingContext.hideStart + "<a$1</a>" + ParsingContext.hideStop) ;
			str = Regex.Replace (str, @"<img ([\s|\S]*)>",
				ParsingContext.hideStart + @"<img $1>" + ParsingContext.hideStop) ;
			str = pc.HideAskedStrings(str);
			return str;
		}

		/// <summary>
		/// Hyper links (http:// , mailto: , isbn: )
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>parsed text</returns>
		public string ParseHyperLinks (string str)
		{
			// image link
			str = Regex.Replace (str, "http(://\\S*(.jpg|.gif|.png|.JPG|.GIF|.PNG))",
				ParsingContext.hideStart + "<img src=\"http$1\">" + ParsingContext.hideStop) ;
			str = pc.HideAskedStrings(str);
			// web links
			str = Regex.Replace (str, @"\[\[((http|https|ftp|news)://[^\]\[]*)\]\[([^\[\]]*)\]\]"
				,ParsingContext.hideStart + "<a href=\"$1\">$3</a>"+ ParsingContext.hideStop);
			str = pc.HideAskedStrings(str);
			str = Regex.Replace (str, "((http|https|ftp|news)://\\S*)"
				,ParsingContext.hideStart + "<a href=\"$1\">$1</a>"+ ParsingContext.hideStop);
			// Windows File link
			str = Regex.Replace (str, @"\[\[file:\\\\([^\[\]]*)\]\]",pc.AskToHideString("<a href=\"file:\\\\$1\">\\\\$1</a>")) ;
			str = pc.HideAskedStrings(str);	
			str = Regex.Replace (str, @"\[\[file:\\\\([^\[\]]*)\]\[([^\[\]]*)\]\]",pc.AskToHideString("<a href=\"file:\\\\$1\">$2</a>")) ;
			str = pc.HideAskedStrings(str);			
			// Mail link
			str = Regex.Replace (str, "((mailto):(\\S*))",pc.AskToHideString("<a href=\"$1\">$3</a>")) ;
			str = pc.HideAskedStrings(str);
			str = Regex.Replace(str, @"([^\s]*@[^\s\.]*\.[^\s]*)","<a href=mailto:$1>$1</a>");
			// Amazon link
			str = Regex.Replace (str, @"\[ISBN:([0-9]{10})\]",
				"<a href=\"http://www.amazon.fr/exec/obidos/ASIN/$1"
				+ "\">ISBN: $1</a>") ;

			return str;
		}

		/// <summary>
		/// Table
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>parsed text</returns>
		public string RenderTable(string str)
		{
			if (str.IndexOf("|") == 0 )
			{
				switch (pc.inTable)
				{
					case ParsingContext.inTableStatus.none :
					{
						// Starting new table and header
						pc.currentTableId++;
						pc.currentTableLine = 0;
						pc.inTable = ParsingContext.inTableStatus.header;
						str = Regex.Replace(str,@"^\|","<tr class=wikitable_header><td>" + 
							pc.HideString("<a href=WikiEditTable.aspx?p="+pc.pageName+"&t=" + pc.currentTableId.ToString() + "&l=-1><img border=0 src=images/icon_addline.gif></a>") 
							+ "</td><td>");
						str = Regex.Replace(str,@"(.+)\|\s*$","$1</td></tr>");
						str = Regex.Replace(str,@"\|","</td><td>");
						return "<table class=wikitable cellSpacing=1 cellPadding=3 border=0>" + str;
					}

					case ParsingContext.inTableStatus.header :
					case ParsingContext.inTableStatus.one :
					case ParsingContext.inTableStatus.two :
					{
						// New table line
						pc.currentTableLine++;
						string cls;
						if (pc.inTable == ParsingContext.inTableStatus.two)
						{
							pc.inTable = ParsingContext.inTableStatus.one;
							cls = "class=wikitable_pyjama";
						}
						else
						{
							pc.inTable = ParsingContext.inTableStatus.two;
							cls = "";
						}
						str = Regex.Replace(str,@"^\|","<tr " + cls + "><td>"  
							+ pc.HideString("<a href=WikiEditTable.aspx?p="+pc.pageName+"&t=" + pc.currentTableId.ToString() + "&l=" + pc.currentTableLine.ToString() + "><img border=0 src=images/icon_edittable.gif></a>")
							+"</td><td>");
						str = Regex.Replace(str,@"(.+)\|\s*$","$1</td></tr>");
						str = Regex.Replace(str,@"\|","</td><td>");
						return str;
					}
				}
			}
			else if (pc.inTable != ParsingContext.inTableStatus.none)
			{ // No more in table
				pc.inTable = ParsingContext.inTableStatus.none;
				return "</table>" + str;
			}
			return str;
		}

		/// <summary>
		/// Nested list items
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>parsed text</returns>
		public string ParseNestedListItems(string str)
		{
			int foundLevel = 0;
			
			Regex bulletPattern = new Regex(@"((   )|\t){1,6}\*");
			string res = bulletPattern.Replace(str,"",1);
			foundLevel = (str.Length - res.Length);
			if (foundLevel > 0)
			{
				if (str.StartsWith(" "))
					foundLevel = (foundLevel - 1) / 3;
				else
					foundLevel--;
				str = res;
			}

			string s = "";
			for (int i=0 ; i< foundLevel - pc.inListLevel ; i++)
			{ s += "<ul>"; }
			for (int i=0 ; i< pc.inListLevel - foundLevel; i++)
			{ s += "</ul>"; }
			pc.inListLevel = foundLevel;
			if (foundLevel == 0) 
				return	s + str;
			else
				return s + "<li>" + str + "</li>" ;
		}

		/// <summary>
		/// Multiline verbatim
		/// context status in ParsingContext.inVerbatim
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>parsed text</returns>
		public string ParseMultilineVerbatim(string str)
		{
			pc.inVerbatim = false;
			if (str.StartsWith("<verbatim>") && !pc.inMultiLineVerbatim)
			{
				pc.inMultiLineVerbatim = true;
				pc.inVerbatim = true;
			}
			if (str.EndsWith("</verbatim>") && pc.inMultiLineVerbatim)
			{
				pc.inMultiLineVerbatim = false;
			}
			return str.Replace("<verbatim>","<VERBATIM>").Replace("</verbatim>","</VERBATIM>") ; // Seems useless..but is not.
		}

		#endregion

	}		

}