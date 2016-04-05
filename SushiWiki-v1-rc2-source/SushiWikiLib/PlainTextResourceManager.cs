using System;
using System.Resources;
using System.Collections;
using System.IO;
using System.Globalization;

namespace Wiki.Tools.Resources
{
    /// <summary>
    /// A custom resource manager which gives all the features of satelite assemblies
    /// without the need to compile. This resource manager just loads plain text files into
    /// a hashtable.
    /// Inspired by Christian Nagel custom localization samples (http://www.christiannagel.com)
    /// History :
    ///<code>
    /// | Vers. | Date       | Developper  | Description
    /// | 0.1   | 2003-07-13 | YZ          | Initial version
    ///</code>
    /// </summary>	
    public class PlainTextResourceManager : ResourceManager
    {
	
        private string baseDir;
        private string filePattern;

/// <summary>
/// Constructor for the plain text file based assembly
/// </summary>
/// <param name="baseDir">directory full path to look for plain text resource files</param>
/// <param name="filePattern">file name pattern (Strings.{0}.txt by default)</param>
        public PlainTextResourceManager(string baseDir,string filePattern)
        {
            this.baseDir = baseDir;
            this.filePattern = filePattern;
            ResourceSets = new Hashtable();
        }
        
	
        protected override ResourceSet InternalGetResourceSet(System.Globalization.CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            PlainTextResourceSet rs;

            if (ResourceSets.Contains(culture))
            {
                rs = ResourceSets[culture] as PlainTextResourceSet;
            }
            else
            {
                rs = new PlainTextResourceSet(baseDir,filePattern,culture);
                ResourceSets.Add(culture, rs);
            }

            return rs; 
        }

        /// <summary>
        /// Constructor for the plain text file based assembly
        /// </summary>
        /// <param name="baseDir">directory full path to look for plain text resource files</param>
        public PlainTextResourceManager(string baseDir) : this(baseDir,"Strings.{0}.txt")
        {

        }
    }
	
    
    /// <summary>
    /// A custom Resource Reader which takes as input a plain text file
    //  with key=value pairs on each line.
    /// </summary>
    public class PlainTextResourceReader : IResourceReader
    {
    
        private CultureInfo culture;
        private string resourceDir = "";
        private string resourceFilePattern = "Strings.{0}.txt";
        
        
        
        #region IResourceReader Members

        public PlainTextResourceReader(string baseDir, string filePattern, CultureInfo culture)
        {
           resourceDir = baseDir;
           resourceFilePattern = filePattern;        
           this.culture = culture;
        }
        
        public PlainTextResourceReader(string baseDir, CultureInfo culture):
                      this(baseDir,"Strings.{0}.txt",culture)
        {}


        /// <summary>
        /// Implements fallback algorithm for resources :
        /// if String.[language code]-[country-code].txt is present, returns corresponding file name
        /// else looks for Strings.[language-code].txt
        /// else looks for Strings.txt (language neutral)
        /// </summary>
        /// <returns></returns>
        private string lookForResourceFile()
        {
           string filePath;
           filePath = Path.Combine(resourceDir,
                String.Format(resourceFilePattern,culture.Name));
           
           if (File.Exists(filePath))
               return filePath;
               
            filePath = Path.Combine(resourceDir,
                String.Format(resourceFilePattern,culture.TwoLetterISOLanguageName));
           if (File.Exists(filePath))
               return filePath;
               
           return Path.Combine(resourceDir,
                String.Format(resourceFilePattern,"").Replace("..",".")); //returns default. ugly hack with ..
        
        }
        



        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
           
            Hashtable ht = new Hashtable();
            
            //TODO: neutral culture  is dealt with, but that's not pretty (see lookForResourceFile() )                                       
            string filePath = lookForResourceFile();
            
            
            //read the file into the hashtable
            using (StreamReader sr = new StreamReader(filePath,System.Text.Encoding.UTF8,true))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if (!line.Trim().StartsWith(";"))   //ignore comments
                    {
                        string[] tab = line.Split(new char[] {'='},2);    //get name-value pairs
                        if (tab.Length == 2)
                            ht.Add(tab[0],tab[1]);
                    }               
                }//while            
            }//using
            
            return ht.GetEnumerator();
        }

        public void Close()
        {
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

    }


    /// <summary>
    /// A custom ResourceSet for plain text files.
    /// </summary>
    public class PlainTextResourceSet : ResourceSet
    {
        internal PlainTextResourceSet(string baseDir,string filePattern, CultureInfo culture)
            : base(new PlainTextResourceReader(baseDir,filePattern, culture))
        {}
    
        internal PlainTextResourceSet(string baseDir, CultureInfo culture)
            : base(new PlainTextResourceReader(baseDir, culture))
        {}
    
	
        public override Type GetDefaultReader()
        {
            return typeof(PlainTextResourceReader);
        }
    
    }
}
