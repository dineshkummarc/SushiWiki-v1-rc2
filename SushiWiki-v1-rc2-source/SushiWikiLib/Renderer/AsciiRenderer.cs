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
	/// Ascii page renderer
	/// </summary>
	public class AsciiRenderer : BaseRenderer
	{
		public AsciiRenderer(string content, string pageName)
		{
			this.content = content;
		}

		/// <summary>
		/// ASCII formating.
		/// - HTML special characters are stripped
		/// </summary>
		/// <returns>HTML source page</returns>
		public override string Format(bool preview)
		{
			// Strip markup
			string res = WikiRender.ReplaceHtmlSpecialCharacters (content) ;

			// And insert it on the page between preformatting tags.
			return "<pre>" + res + "</pre>" ;

		}

	}
}