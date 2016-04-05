using System;
using my.tools;
using System.Text;

namespace Wiki.Tools
{
    /// <summary>
    /// An helper class to give a more user friendly access to the Diff class by
    /// Matthias Helter
    /// History :
    /// <code>
    /// | Vers. | Date       | Developper  | Description
    /// | 0.1   | 17/06/2003 | YZ          | Initial version
    /// </code>
    /// </summary>
    /// 
    public class DiffWrapper
    {
        private string _firstText;
        private string _otherText;
        private DiffOptions _options = DiffOptions.None;	    
	
        /// <summary>
        /// The list of differing items between the two files
        /// </summary>
        public DiffItem[] Items;
	        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstText">string to compare from</param>
        /// <param name="otherText">string to compare to</param>
        /// <param name="options">diff options</param>
        public DiffWrapper(string firstText, string otherText, DiffOptions options)
        {
            if (firstText == null || otherText == null)
                throw new ArgumentNullException();
	            
            _firstText = firstText;
            _otherText = otherText;
            _options = options;	    	    
        }
	    
        /// <summary>
        /// Launches the diff
        /// </summary>
        public void Execute()
        {
            my.tools.Diff diff = new my.tools.Diff();
        
            my.tools.Diff.Item[] items = diff.DiffText(_firstText, _otherText,(_options & DiffOptions.TrimSpaces) != 0,
                (_options & DiffOptions.IgnoreSpaces) !=0,
                (_options & DiffOptions.IgnoreCase) != 0);

            Items = new DiffItem[items.Length];
            
            string[]aLines = _firstText.Split('\n');
            string[]bLines = _otherText.Split('\n');

            int n = 0; //current line number
           
            for (int fdx = 0; fdx < items.Length; fdx++) 
            {
                string insertedText = null;
                string deletedText = null;
                int startI = 0;
                int startD = 0;
                int nbI = 0;
                int nbD = 0;
                
                Diff.Item aItem = items[fdx];
                
                n = aItem.StartB;

                // write deleted lines
                if (aItem.deletedA != 0)
                {
                    deletedText = String.Join("\r\n",aLines,aItem.StartA,aItem.deletedA);                 
                    startD = aItem.StartA;
                    nbD = aItem.deletedA;
                }

                // write inserted lines
                if (aItem.insertedB != 0)
                {
                    insertedText=String.Join("\r\n",bLines,n,aItem.insertedB);
                    startI = n;
                    nbI = aItem.insertedB;
                }
                            
                DiffItem diffit = new DiffItem(insertedText,deletedText,aItem);
                diffit.InsertedLines = nbI;
                diffit.startInserted = startI;
                diffit.DeletedLines = nbD;
                diffit.startDeleted = startD;
                Items[fdx] = diffit;
            } // for
        }
	    
        public DiffWrapper()
        { 
        }
		
        /// <summary>
        /// Diff options
        /// </summary>
        [Flags]		
            public enum DiffOptions
        {
            /// <summary>
            /// Default options, the strings are diffed as is
            /// </summary>
            None,
            /// <summary>
            /// Every combination ou whitespace is converted into a single space before diffing
            /// </summary>
            IgnoreSpaces,
            /// <summary>
            /// The strings are made lowercase before diffing
            /// </summary>
            IgnoreCase,
            /// <summary>
            /// Every leading or trailing whitespaces are trimed before diffing
            /// </summary>
            TrimSpaces		
        }
		
        /// <summary>
        /// A single diff item representing a block of deleted lines and/or a block of inserted lines
        /// </summary>
        public class DiffItem
        {
            /// <summary>
            /// The inserted text (lines joined), null if no insertion
            /// </summary>
            public string InsertedText;
            /// <summary>
            /// The line index (in the original string) of the deletion
            /// </summary>
            public int startDeleted;
            /// <summary>
            /// The number of deleted lines
            /// </summary>
            public int DeletedLines;
            /// <summary>
            /// The line index (in the new string) of the insertion
            /// </summary>
            public int startInserted;
            /// <summary>
            /// The number of inserted lines
            /// </summary>		    
            public int InsertedLines;		    		    
            /// <summary>
            /// The deleted text (lines joined), null of no deletion
            /// </summary>
            public string DeletedText;
		    
            /// <summary>
            /// The original (low level) diff result
            /// </summary>
            public my.tools.Diff.Item Item;	    

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="insertedText">The inserted text (lines joined), null if no insertion</param>
            /// <param name="deletedText">The deleted text (lines joined), null of no deletion</param>
            /// <param name="item">The original (low level) diff result</param>
		    
            public DiffItem(string insertedText, string deletedText,my.tools.Diff.Item item)
            {
                InsertedText = insertedText;
                DeletedText = deletedText;
                Item = item;
            }
        }
		
    }
    
    public class DiffRenderer
    {
        
        public static string Render(string firstString, string otherString)
        {
            DiffWrapper diff = new DiffWrapper(firstString,otherString,DiffWrapper.DiffOptions.None);
            diff.Execute();
            return Render(diff);
        }
    
        public static string Render(DiffWrapper diff)
        {
        
            StringBuilder sb = new StringBuilder();
            foreach(DiffWrapper.DiffItem i in diff.Items)
            {        
                sb.Append("----\r\n");
                if (i.DeletedText != null)
                {
                    sb.Append("*red*=======Deleted==========*\r\n");
                    sb.Append(i.DeletedText + "\r\n");
                }
                    
                if (i.InsertedText != null)
                {
                    sb.Append("*green*=======Added==========*\r\n");
                    sb.Append(i.InsertedText + "\r\n");                   
                }
            }
            return sb.ToString();
            
        }
        
    
    }
}
