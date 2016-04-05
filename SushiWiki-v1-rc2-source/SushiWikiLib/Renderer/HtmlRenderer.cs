using System;
using System.Text.RegularExpressions;
using Wiki.Render;
using Wiki.GUI;
using System.Text;
using System.Collections;
using Wiki.Macros;
using Wiki;
using Wiki.Tools;


namespace Wiki.Render
{
	/// <summary>
	/// Html page renderer
	/// </summary>
	public class HtmlRenderer : BaseRenderer
	{
		public HtmlRenderer(string content, string pageName)
		{
			this.content = content;
			pc = new ParsingContext(pageName);
		}

		public override string Format(bool preview)
		{
			
			string result;
			// WikiKeywords
			result = ParseWikiKeywords(content);

			
			// Detect potential wikinames
			result = extractWikiNames.Replace(result,new MatchEvaluator(this.WikiNameEvaluator));
			
			return  pc.UnhideAllStrings(result) ;

		}

	}
}
