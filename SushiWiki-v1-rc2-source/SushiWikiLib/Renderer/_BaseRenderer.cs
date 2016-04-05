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
	/// Base class for all page renderers
	/// </summary>
	public abstract class BaseRenderer
	{
		protected ParsingContext pc;
		protected string content;
		protected Hashtable m_items = new Hashtable();

		public virtual Hashtable Items
		{
				get{return m_items;}
		}
		
		protected Regex extractWikiNames = new Regex ( 
			@"
				(?<wikiName>)				#Dummy pattern
				\[\[						#First bracket
				(?<pageNameAlone>[^\[\]]*)	#Page name (no anchor)
				\]\]						#End
				|
				\[\[					
				(?<pageName>[^\[\]]*)		#Page name
				\]
				\[
				(?<anchor>[^\[\]]*)			#anchor text
				\]\]",RegexOptions.IgnorePatternWhitespace
					  | RegexOptions.Compiled) ;

		public abstract string Format(bool preview);

		public virtual string WikiNameEvaluator(Match m)
		{
			string plainResult = "";
			string linkName = m.Groups["wikiName"].ToString() + m.Groups["pageNameAlone"].ToString() + m.Groups["pageName"].ToString();
			string anchor = m.Groups["anchor"].ToString();

			if (linkName.ToLower().StartsWith("file://")) return (m.ToString()); //Exclude file links
			if (linkName.IndexOfAny( new Char[] {' ','\'' }) > 0) return (m.ToString());
			
			linkName = linkName.TrimEnd( new Char[] {'.',',',';',':','?','!','<','('} ); // Remove ending point/comma

			
			if (anchor == "")
				anchor = linkName;
			
			WikiManager.WikiPageShortInfo info = WikiManager.Singleton().GetPageShortInfo(linkName);
			
			if (info.pageFound)
			{
				string overlib = WikiGui.ExistingWikiPagePopup(linkName,info);
				plainResult = String.Format("<a href='Wiki.aspx?page={0}' {1}>{2}</a>",linkName,overlib,anchor);
			}
			else
			{
				string overlib = WikiGui.NewWikiPagePopup(linkName);
				plainResult = String.Format("{2}<a href='WikiEdit.aspx?page={0}' {1}>?</a>",linkName,overlib,anchor);
			}

			return this.pc.HideString(plainResult);			
		}


		private string MacroEvaluator(Match m)
		{
			Hashtable macroMap = WikiManager.Singleton().MacroMap;
			IMacro macro = macroMap[m.Groups["macroName"].ToString()] as IMacro;
			if (macro == null) //no such macro registered
				return m.ToString();
			try
			{
				macro.Init(pc,makeParams(m.Groups["params"].ToString()));
			}
			catch (Exception ex)
			{
				//should do some logging here
				return m.ToString();
			}
			try
			{
				return pc.HideString(macro.Render());			
			}
			catch
			{
				return m.ToString();
			}
		}

		private object[] makeParams(string paramstring)
		{
			return paramstring.Split(new char[]{','});
		}

		/// <summary>
		/// Keywords (%BLABLA% or %BLABLA()% )
		/// </summary>
		/// <param name="str">initial text</param>
		/// <returns>parsed text</returns>
		public string ParseWikiKeywords(string str)
		{
			Regex macroPattern = new Regex(
	@"%			#Macro start
	(?<macroName>[A-Z_\d]+)	#Macro name
	(			
	\(
	(?<params>(?:[^,]*,)*[^),]+)	#optional params
	\)
	)?
	%",RegexOptions.IgnorePatternWhitespace);
			string result = macroPattern.Replace(str,new MatchEvaluator(this.MacroEvaluator));
			
			return result;
			
		}
	
	}

}
