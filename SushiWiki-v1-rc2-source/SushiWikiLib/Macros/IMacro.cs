using System;
using System.Text.RegularExpressions;

namespace Wiki.Macros
{
	/// <summary>
	/// Summary description for IMacro.
	/// </summary>
	public interface IMacro
	{
		void Init(Wiki.Render.ParsingContext pc, object[] parameters);
		string Render();
	}

	[AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
	public class WikiMacroAttribute : Attribute
	{
		private string _macroName;
		public WikiMacroAttribute(string MacroName)
		{
			_macroName = MacroName;
		}
		public WikiMacroAttribute()
		{
			
		}

		public string Name
		{
			get {return _macroName;}
		}
	}

}
