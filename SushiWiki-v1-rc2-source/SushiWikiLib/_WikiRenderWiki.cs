namespace Wiki.Render
{
	using System;
	using System.Text.RegularExpressions;
	using System.Diagnostics;
	using System.Data.OleDb;
	using System.IO;
	using System.Text;
	using Wiki.GUI;


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
	/// | 0.1   | 26/07/2002 | EGE         | Initial version based on Wiki.NET from Alistair J. R. Young
	/// | 0.2   | 07/07/2002 | EGE         | First deployed release
	/// | 0.3   | 23/07/2002 | EGE         | Bug maintenance
	/// | 0.4   | 27/07/2002 | EGE         | improved [[][]] links
	/// | 0.5   | 09/09/2002 | EGE         | %ATTACHEDIMAGE()% keyword
	/// | 0.6   | 12/09/2002 | EGE         | %VERSION% keyword
	/// | 0.7   | 29/09/2002 | EGE         | Work on WikiNames detection
	/// | 0.8   | 01/10/2002 | EGE         | Bug maintenance
	/// | 0.9   | 08/10/2002 | EGE         | Work around WikiNames detection 
	/// | 0.10  | 17/10/2002 | EGE         | Is know only for Wiki format formating. (created _WikiRender.cs)
	/// | 0.11  | 23/10/2002 | EGE         | Bug found using NUnit test classes
	/// | 0.12  | 04/11/2002 | EGE         | Bug B00004 fixed, WikiKeyword %ATTACHEDFILE(...)%
	/// | 0.15  | 17/03/2003 | EGE         | Bug 697520 fixed
	/// | 0.16  | 27/03/2003 | EGE         | Added %RSS()% Wiki keyword
	/// | 0.17  | 02/04/2003 | EGE         | ParseWikiKeywords moved to WikiRender class.
	/// | 0.18  | 29/04/2003 | EGE         | Bugs 699743,724227 fixed.
	/// | 0.19  | 25/05/2003 | EGE,YZ      | Bug 743202 fixed. YZ simplified regular expression
	/// | 0.20  | 11/06/2003 | YZ          | Impacted changes in WikiGui.
	/// | 0.21  | 17/06/2003 | EGE         | Bug 754901 fixed. (Glitch with WikiNames when corresponding page doesn't exist)
	/// | 0.22  | 29/11/2003 | EGE         | Added table edition, removed Twiki migration stuff (metazone), now uses StringBuiler
	///                                    | Bug fixed : table was closed after the new line.
	/// | 0.23  | 01/02/2003 | YZ          | Refactoring :  Page rendering is now done by the renderer classes.| 
	/// </code>
	/// </summary>
	//////////////////////////////////////////////////////////////////////////////
	public class WikiRenderWiki
	{
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.23";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 23;
		
		/// <summary>
		/// Regular expression detecting Wiki names.
		/// </summary>

		private WikiRenderWiki()
		{
			throw new InvalidOperationException ("Can't construct a statics-only class.") ;
		}

		/// <summary>
		/// This structure is used as parameter for <see cref="Wiki.Render.RenderWiki.RenderPage"/>.
		/// 2 Requests (modes) are possible :
		/// - Ask to retreive a line from page data (use <see cref="Wiki.Render.RenderWiki.STableEditionRequest.SetGetLineMode"/>
		///   In this case RenderPage returns the requested line data
		/// - Ask to update a line in the page data (use <see cref="Wiki.Render.RenderWiki.STableEditionRequest.SetUpdateLineMode"/>
		///   In this case RenderPage returns the updated page data
		/// SetNothingToDoMode means that nothing is requested for table edition
		/// </summary>
		public class STableEditionRequest
		{
			public int TableId;
			public int TableLine;
			public bool UpdateLineMode;
			public bool GetLineMode;
			public bool NothingToDoMode;
			public bool AddLineMode;
			public string LineData;
			public void SetNothingToDoMode()
			{
				TableId = -1;
				TableLine = -1;
				UpdateLineMode = false;
				GetLineMode = false;
				NothingToDoMode = true;
				AddLineMode = false;
				LineData = null;

			}
			public void SetGetLineMode(int tableId,int tableLine)
			{
				TableId = tableId;
				TableLine = tableLine;
				UpdateLineMode = false;
				GetLineMode = true;
				NothingToDoMode = false;
				AddLineMode = false;
				LineData = null;
			}
			public void SetUpdateLineMode(int tableId,int tableLine,string lineToUpdate)
			{
				TableId = tableId;
				TableLine = tableLine;
				UpdateLineMode = true;
				GetLineMode = false;
				NothingToDoMode = false;
				AddLineMode = false;
				LineData = lineToUpdate;
			}
			public void SetAddLineMode(int tableId,string lineToAdd)
			{
				TableId = tableId;
				TableLine = -1;
				UpdateLineMode = false;
				GetLineMode = false;
				NothingToDoMode = false;
				AddLineMode = true;
				LineData = lineToAdd;
			}
			public bool IsContextOnTableLine(ParsingContext pc)
			{
				return ( (pc.currentTableId == TableId) && (pc.currentTableLine == TableLine) );
			}

			public bool IsContextAfterLastLine(ParsingContext pc)
			{
				return ( (pc.currentTableId == TableId) && (pc.inTable == ParsingContext.inTableStatus.none) );
			}

			public void StopSearching()
			{
				TableId = -1;
			}
		}

	}

}

