using System;
using System.Resources;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Wiki.Tools
{
	/// <summary>
	/// Summary description for GenLexer.
	/// </summary>
	public class Colorizer
	{
		private Lexer lexer;
		private string[] patternArray;
		public Colorizer()
		{
		}
		
		public Colorizer(LanguageLexer language)
		{
			LoadLexer(language);
			
		}

        private void initLexer()
        {
            ArrayList arr=new ArrayList();
            Regex re = new Regex(@"<(\w+)>");

            string res="";
            foreach(Match m in re.Matches(lexer.Pattern))
            {
                res+=";" + m.Groups[1].Value;			    
            }
            patternArray = res.Substring(1).Split(';');
        
        }

		public void LoadLexer(LanguageLexer builtinLanguage)
		{
			lexer = new Lexer();
			lexer.Load(builtinLanguage);
			initLexer();
		}
		
        public void LoadLexer(string builtinLanguage)
        {
            lexer = new Lexer();
            LanguageLexer lang;
            switch(builtinLanguage.ToLower())
            {
               case "cs":
                  lang = LanguageLexer.CSharp;
                  break;
                case "vb":
                    lang = LanguageLexer.VbNet;
                    break;
                case "js":
                    lang = LanguageLexer.JScript;
                    break;
                case "py":
                    lang = LanguageLexer.Python;
                    break;
                default:
                  throw new ArgumentException("No such builtin language");
            
            }
            lexer.Load(lang);
            initLexer();
        }

		
		public void LoadLexerFromFile(string filePath)
		{
		    lexer = new Lexer();
		    lexer.Load(filePath);  		
		    initLexer();
		}
		
		public string Decorate(string sourceCode)
		{
			if (lexer == null)
				throw new ApplicationException("No lexer defined.");
			
			Regex re = new Regex(lexer.Pattern,lexer.options);
			return re.Replace(sourceCode, new MatchEvaluator(MyReplace));
        }

		public string MyReplace(Match target)
		{
			string style="";
		
			//Pabô : on veut savoir quel groupe est le bon
			GroupCollection grc = target.Groups;		
			foreach (string s in patternArray)
			{
				if (grc[s].Value.Length >0)
				{
					style = s;
					break;
				}
			}
			return  "<span class='Code" + style + "'>" + target.Value + "</span>";
		}
		
	}

	public class Lexer
	{
		public string Pattern;
		public RegexOptions options;

		public Lexer()
		{}

        
        public void Load(LanguageLexer builtinLexer)
        {
			//TODO: get the namespace by reflection instead of hardcoding it.	
            string resourceName = String.Format("SushiWikiLib.Colorizer.Config.{0}.xml",builtinLexer);
            LoadXml(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
        }
        
        private void LoadXml(Stream s)
        {
            XmlDocument doc = new XmlDocument();
            using (StreamReader sr = new StreamReader(s,System.Text.Encoding.Default,true))
            {
            doc.Load(sr);
            }
            
            StringBuilder sb = new StringBuilder();
            XmlElement mainPattern =(XmlElement)doc.SelectSingleNode("//pattern"); 
            options = RegexOptions.None;
            if (mainPattern.GetAttribute("ignoreCase") == "true")
                options |= RegexOptions.IgnoreCase;
            if (mainPattern.GetAttribute("singleLine") == "true")
                options |= RegexOptions.Singleline;
            sb.Append(mainPattern.InnerText);
            foreach(XmlElement el in doc.SelectNodes("//subPattern"))
            {
                sb.Replace("$"+el.GetAttribute("name"),el.InnerText);
            }
            Pattern = sb.ToString();        
        
        }


        public void Load(string filePath)
        {
            LoadXml(File.OpenRead(filePath));     
        }

	}
	
	public enum LanguageLexer
	{
	    CSharp,
	    VbNet,
	    JScript,
	    Python
	}
}
