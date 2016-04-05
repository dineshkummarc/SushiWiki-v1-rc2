using System;
using System.IO;
using Wiki.Tools;
using Wiki.Render;


namespace Wiki.Macros
{
	[WikiMacro("ATTACHEDCODE")]
	[WikiMacro("CODE")]
	public class MacroColorize : AttachmentMacro
	{
		public  override string Render()
		{
			string lang =  Path.GetExtension(fileName).Substring(1).ToLower();

			Colorizer col = new Colorizer();
			col.LoadLexer(lang);
            
			System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            
			if (ctx == null)
				throw new ApplicationException("Must be run in web app");           
                     
            
			string filePath = ctx.Server.MapPath(Path.Combine("~/pub/" + parsingContext.pageName,fileName));
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
	}


}



