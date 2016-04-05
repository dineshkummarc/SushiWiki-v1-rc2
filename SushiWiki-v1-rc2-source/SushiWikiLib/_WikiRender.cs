namespace Wiki.Render
{
	using System;
	using System.Text.RegularExpressions;
	using System.Diagnostics;
	using System.Data.OleDb;
	using System.Text;
	using Wiki.GUI;
	using Wiki.Tools;
	using Wiki.Storage.SQL;
	using Wiki.Storage.XML;
	using System.IO;

	//////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Wiki pages parsing. It provides static methods for parsing pages.
	/// This class can't be instanciated.
	/// 
	/// Transormations are mainly done using RegEx expression. Most of them where 
	/// found in TWiki perl source code.
	/// 
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 17/10/2002 | EGE         | Initial version from scratch
	/// | 0.2   | 09/03/2003 | EGE         | HTML Render 
	/// | 0.3   | 03/04/2003 | EGE         | Moved ParseWikiKeywords from WikiRenderWiki
	/// | 0.4   | 11/06/2003 | YZ          | Impacted changes in WikiGui.cs
	/// </code>
	/// </summary>
	//////////////////////////////////////////////////////////////////////////////
	public class WikiRender
	{
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.4";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 4;
		
		private WikiRender()
		{
			throw new InvalidOperationException ("Can't construct a statics-only class.") ;
		}

		public static BaseRenderer GetRenderer(string type, string content, string title)
		{
			BaseRenderer renderer = null;
			switch(type)
			{
				case "WIKI":
					renderer = new WikiRenderer(content,title);
					break ;

				case "ASCII":
					renderer = new AsciiRenderer(content,title);
					break ;

				case "HTML":
					renderer = new HtmlRenderer(content,title);
					break ;
				default:
					throw new WikiException(String.Format("Unknown page type {0}",type));
			}
			return renderer;
		}


		/// <summary>
		/// Wiki formating FormatPageAsWiki
		/// </summary>
		/// <param name="content">WIKI source page</param>
		/// <param name="pagename">page name</param>
		/// <returns>HTML source page</returns>


		/// <summary>
		/// Wiki formating.
		/// RenderPageAsWiki
		/// Preview mode removes tooltips. It is used by the preview page (WikiPreview.aspx)
		/// </summary>
		/// <param name="content">WIKI source page</param>
		/// <param name="pagename">page name</param>
		/// <param name="previewMode">preview mode flag</param>
		/// <returns>HTML page source</returns>

		/// <summary>
		/// Replace HTML special characters (&amp; &lt; &gt;) 
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>html text</returns>
		public static string ReplaceHtmlSpecialCharacters (string str)
		{	
			// replace HTML special characters with character entities
			str = Regex.Replace (str, "&", "&amp;") ;
			str = Regex.Replace (str, "<", "&lt;") ;
			str = Regex.Replace (str, ">", "&gt;") ;
			return str;
		}

	}
		/// <summary>
		/// This class hold the context during page parsing (for tables, lists ...).
		/// It also manages 'string hidding' in order to prevent parsing of page parts :
		///  - the page part is removed from the page source and replaced by an unique id
		///  - after page parsing, unique id are replaced back by the initial page part
		/// </summary>
		public class ParsingContext
		{	
			public enum inTableStatus 
			{ none, header, one , two };
		
			public inTableStatus inTable;
			public int inListLevel;
			public bool inMultiLineVerbatim;
			public bool inVerbatim;
			public int currentTableId;
			public int currentTableLine;
			public string pageName;

			private static string keyprefix = "jcrpnysutvcoenqdfg";
			private static string keysuffix = "kswpif";

			private Regex HidePattern = new Regex(keyprefix + @"\d+" + keysuffix, RegexOptions.Compiled);

			public const string hideStart = "(((";
			public const string hideStop = ")))";
			public const string hideStartRegex = @"\(\(\(";
			public const string hideStopRegex = @"\)\)\)";

			private int hiddenStringCounter;
			private System.Collections.Hashtable hiddenStrings;

			public ParsingContext(string pagename)
			{
				inTable = inTableStatus.none;
				inListLevel = 0;
				inVerbatim = false;
				inMultiLineVerbatim = false;
				hiddenStringCounter = 0;
				hiddenStrings = new System.Collections.Hashtable();
				currentTableId = -1;
				currentTableLine = -1;
				pageName = pagename;
			}

			public string HideString(string str)
			{
				hiddenStrings.Add(GetKeyForHiddenString(hiddenStringCounter),str);
				hiddenStringCounter++;
				return GetKeyForHiddenString(hiddenStringCounter-1);
			}

			public string AskToHideString(string str)
			{
				return hideStart + str + hideStop;
			}

			public string HideAskedString(Match m)
			{
				return HideString(m.Value.Substring(hideStart.Length,m.Value.Length-hideStart.Length-hideStop.Length));
			}

			public string HideAskedStrings(string str)
			{
				return Regex.Replace(str,@"\(\(\((?:[^\(]|\((?!\())*\)\)\)" ,new MatchEvaluator(this.HideAskedString));
			}


			private string UnhideEvaluator(Match m)
			{
				string res = hiddenStrings[m.ToString()] as string;
				if (res == null)
					return m.ToString();
				else
					return res;
			
			}

			public string UnhideAllStrings(string str)
			{
				return HidePattern.Replace(str,new MatchEvaluator(this.UnhideEvaluator));
			}


			public void UnhideAllStrings(ref StringBuilder strb)
			{
				string res = HidePattern.Replace(strb.ToString(), new MatchEvaluator(this.UnhideEvaluator));
				strb = new StringBuilder(res); //UGLY			
			}
		
			private string GetKeyForHiddenString(int i)
			{
				return keyprefix + Convert.ToString(i) + keysuffix;
			}

		}

	
		public interface IMacroProvider
		{
			string PageName {get;}
			ParsingContext ParsingContext{get;}
			string Evaluator(Match m);    
		}
    
		public class MacroColorize : IMacroProvider
		{
			#region IMacroProvider Members

			private string pageName;
			private ParsingContext parsingContext;
        

			public string PageName
			{
				get
				{  return pageName;}
			}

			public ParsingContext ParsingContext
			{
				get
				{  return parsingContext;}
			}

			public MacroColorize(string pageName, ParsingContext parsingContext)
			{
				this.pageName = pageName;
				this.parsingContext = parsingContext;
			}
        
			public string Evaluator(Match m)
			{
				string fileName = m.Groups[1].Value;
				string lang =  Path.GetExtension(fileName).Substring(1).ToLower();

				Colorizer col = new Colorizer();
				col.LoadLexer(lang);
				fileName = Path.GetFileName(fileName); //escaping path traversal...
            
				System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            
				if (ctx == null)
					throw new ApplicationException("Must be run in web app");
            
                      
            
				string filePath = ctx.Server.MapPath(Path.Combine("~/pub/" + pageName,fileName));
				string source;
				using (StreamReader sr = new StreamReader(filePath,System.Text.Encoding.Default,true))
				{
					source = sr.ReadToEnd();
				}      	       
           	       
				return 
					parsingContext.AskToHideString("<pre class=codeBackground>" +
					col.Decorate(source.Replace("&","&amp;").Replace("<","&lt;")) +
					"</pre>");
			}

			#endregion

		}

	}


