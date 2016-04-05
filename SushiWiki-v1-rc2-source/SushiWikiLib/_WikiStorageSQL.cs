using System;
using System.Data;
using System.Data.OleDb;
using Wiki;
using System.Diagnostics;
using System.Text;

namespace Wiki.Storage.SQL
{

	/// <summary>
	/// SQLServer storage management layer. To be used with SQL7,SQL2000 or MSDE.
	/// OLEDB connection string is given by WikiSettings singleton.
	///
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 10/01/2003 | EGE         | This code was originaly in WikiManager class.
	/// | 0.2   | 24/05/2003 | YZ          | Ensures Dispose() is always called.
	/// | 0.3   | 20/08/2003 | EGE         | Added title to WikiPageShortInfo
	/// 
	/// </code>
	/// 
	/// </summary>

	public class WikiStorageSql : IStorageInterface,IDisposable
	{
		#region Version management
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.3";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 3;
		#endregion

		/// <summary>
		/// Get a new database connection
		/// </summary>
		/// <returns>New connection</returns>
		public OleDbConnection GetDbConnection()
		{
			OleDbConnection dbc = new OleDbConnection(WikiSettings.Singleton().DbConnectionString);
			dbc.Open();
			return dbc;
		}

		public void Dispose()
		{

		}

		/// <summary>
		/// Switch public access page status
		/// </summary>
		/// <param name="pageTitle">wiki page name</param>
		/// <returns>new status</returns>
		public bool SwitchPublicAccessStatus(string pageTitle)
		{
			WikiManager.PageData wPage = GetPage(pageTitle) ;
			wPage.publicAccess = (!wPage.publicAccess);
			wPage.lastUpdated = DateTime.Now;
			bool status = wPage.publicAccess;
			WriteNewPage(wPage);
			return status;
		}

		/// <summary>
		/// Stores a new page
		/// </summary>
		/// <param name="newPage">Page data</param>
		public void WriteNewPage (WikiManager.PageData newPage)
		{
			OleDbParameter  cmdParam ;
			OleDbCommand      command ;

            try
            {
			using (OleDbConnection con = GetDbConnection())
			{
				// now set up the command
				command = new OleDbCommand ("WikiSP_WriteNewWikiPage",con);
				command.CommandType = CommandType.StoredProcedure;

				// set up command parameters
				cmdParam = new OleDbParameter("inpLastUpdated", SqlDbType.DateTime) ; // lastUpdated
				cmdParam.Value = newPage.lastUpdated ;
				command.Parameters.Add(cmdParam);
				cmdParam = new OleDbParameter ("title", SqlDbType.VarChar) ;           // title
				cmdParam.Value = newPage.title ;
				command.Parameters.Add(cmdParam);
				cmdParam = new OleDbParameter ("inpType", SqlDbType.VarChar) ;           // state
				cmdParam.Value = newPage.type ;
				command.Parameters.Add(cmdParam);
				cmdParam = new OleDbParameter ("inpUpdatedBy", SqlDbType.VarChar) ;       // updatedBy
				cmdParam.Value = newPage.updatedBy ;
				command.Parameters.Add(cmdParam);
				cmdParam = new OleDbParameter ("inpOwner", SqlDbType.VarChar) ;       // updatedBy
				cmdParam.Value = newPage.ownedBy ;
				command.Parameters.Add(cmdParam);
				cmdParam= new OleDbParameter ("inpPageData", SqlDbType.Text) ;    // pageData
				cmdParam.Value = newPage.pageData ;
				command.Parameters.Add(cmdParam);
				cmdParam= new OleDbParameter ("preserve", SqlDbType.Int) ;
				cmdParam.Value = -(WikiSettings.Singleton().PreserveTime) ;
				command.Parameters.Add(cmdParam);
				cmdParam= new OleDbParameter ("public", SqlDbType.Int) ;
				cmdParam.Value = newPage.publicAccess ;
				command.Parameters.Add(cmdParam);

				// put the values into them
				command.ExecuteNonQuery();
			}
			}
			catch (Exception e)
			{
				throw new WikiException ("An error has occurred creating a new page.", e) ;
			}
		}

		/// <summary>
		/// Get quick information on requested page.
		/// The WikiHome page is automaticaly created if it doesn't exist in database.
		/// </summary>
		/// <param name="title">Page name</param>
		/// <returns>Page quick informations</returns>
		public WikiManager.WikiPageShortInfo GetPageShortInfo (string title)
		{
			OleDbParameter  cmdParam ;
			OleDbCommand      command ;
			OleDbDataReader   returnedData ;
			WikiManager.WikiPageShortInfo info = new WikiManager.WikiPageShortInfo();

			info.pageFound = false;
			info.publicAccess = true; // default value for new page

			try 
			{
			
				using (OleDbConnection con = GetDbConnection())
				{
				// set up command parameters
				cmdParam = new OleDbParameter ("inpTitle", SqlDbType.VarChar) ;   // title
				cmdParam.Value = title ;

				// now set up and call the stored procedure
				command = new OleDbCommand ("WikiSP_CheckForWikiName",con);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.Add(cmdParam);
				returnedData = command.ExecuteReader (CommandBehavior.CloseConnection) ;
		
				if (returnedData.RecordsAffected == -1)
				{
					returnedData.Read();
					info.lastUpdated = returnedData.GetDateTime(0);
					info.updatedBy = returnedData.GetString(1);
					info.ownedBy = returnedData.GetString(2);
					info.publicAccess = returnedData.GetBoolean(3);
					info.pageFound = true;
					info.pageTitle = title;
				}
				return info;
				}
			}
			catch (Exception e)
			{
				throw new WikiException ("An unexpected error has occurred checking page [" +title+ "]", e) ;
			}
		}

		/// <summary>
		/// Return wiki page full data
		/// </summary>
		/// <param name="name">Wiki page name</param>
		/// <returns>page data</returns>
		public WikiManager.PageData GetPage(string name)
		{
			return _GetWikiPage(false,name);
		}

		/// <summary>
		/// Return wiki page full data using internal ID 
		/// (in order to get all page versions)
		/// </summary>
		/// <param name="id">Page internal id</param>
		/// <returns>page data</returns>
		public WikiManager.PageData GetPageById(string id)
		{
			return _GetWikiPage(true,id);
		}

		/// <summary>
		/// Return all page data.
		/// </summary>
		/// <param name="ById">True if given value is an Id, false if it is the page name</param>
		/// <param name="val">Key value</param>
		/// <returns>Page data</returns>
		private WikiManager.PageData _GetWikiPage (bool ById, string val)
		{
			OleDbParameter    cmdParam ;
			OleDbCommand      command ;
			OleDbDataReader   returnedData ;
			WikiManager.PageData thisPage = new WikiManager.PageData();

			
			
			try 
			{
			    using (OleDbConnection con = GetDbConnection())
			    {
				    if (ById)
				    {
					    cmdParam = new OleDbParameter ("inpId", System.Data.SqlDbType.VarChar) ;   // title
					    cmdParam.Value = val ;
					    command = new OleDbCommand("WikiSP_GetWikiPageById",con);
				    }
				    else
				    {
					    cmdParam = new OleDbParameter ("inpTitle", System.Data.SqlDbType.VarChar) ;   // title
					    cmdParam.Value = val ;
					    command = new OleDbCommand("WikiSP_GetWikiPage",con);
				    }
				    cmdParam.Direction = ParameterDirection.Input;
				    command.CommandType = System.Data.CommandType.StoredProcedure;
				    command.Parameters.Add(cmdParam);

				    returnedData = command.ExecuteReader(CommandBehavior.CloseConnection) ;

				    returnedData.Read() ;
				    thisPage.lastUpdated    = returnedData.GetDateTime(0) ;
				    thisPage.title          = returnedData.GetString (1) ;
				    thisPage.type           = returnedData.GetString (2) ;
				    thisPage.updatedBy      = returnedData.GetString (3) ;
				    thisPage.ownedBy        = returnedData.GetString (4) ;
				    thisPage.pageData       = returnedData.GetString (5) ;
				    thisPage.lockedBy       = returnedData.GetString (6) ;
				    thisPage.publicAccess   = returnedData.GetBoolean (7) ;
			    }
			}
			catch (Exception e)
			{
				throw new WikiException ("An unexpected error has occurred while searching for a Wiki name.", e) ;
			}
			return thisPage;
		}

		/// <summary>
		/// Simple FullText search
		/// </summary>
		/// <param name="str">String to search</param>
		/// <returns>DataSet containing search results</returns>
		public DataSet FullTextSearch(string str)
		{

            using (OleDbConnection con = GetDbConnection())
			{
			OleDbCommand command = new OleDbCommand ("WikiSP_SimpleFullTextSearch", con) ;
			command.CommandType = CommandType.StoredProcedure ;
			command.Parameters.Add ("@inpTitle", OleDbType.VarChar, 64) ;
			command.Parameters["@inpTitle"].Value = str ;
			OleDbDataAdapter adapt = new OleDbDataAdapter(command);
			DataSet ds = new DataSet() ;
			adapt.Fill(ds, "Results") ;
			return ds ;
			}
		}

		/// <summary>
		/// Return all pages list
		/// </summary>
		/// <returns>string array containing all page names</returns>
		public string[] GetPageList ()
		{
			using (OleDbConnection con = GetDbConnection())
			{
			    OleDbCommand command = new OleDbCommand("WikiSP_GetWikiPageList", con) ;
			    command.CommandType = CommandType.StoredProcedure ;
			    OleDbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
			    StringBuilder strb = new StringBuilder();
			    while (dr.Read())
			    {	
				    strb.Append( dr.GetString(0) + ",") ;
			    }
			    con.Close();
			    if (strb.Length>0) strb.Remove(strb.Length-1,1);
			    return strb.ToString().Split(',');
			}
		}

		/// <summary>
		/// Return TOP lists.
		/// WEEK_TOP10 = 10 last updated las week 
		/// NEW10 = 10 last created pages
		/// </summary>
		/// <param name="top">Top name</param>
		/// <returns>Array of wiki page names</returns>
		public string[] GetTop (string top)
		{
			string sqlproc = "WikiSP_GetWekkTop10";  // Default
			switch (top)
			{
				case "WEEK_TOP10" : sqlproc = "WikiSP_GetWeekTop10"; break;
				case "NEW10" : sqlproc = "WikiSP_GetTop10New"; break;
			}

			using (OleDbConnection con = GetDbConnection())
			{
			    OleDbCommand command = new OleDbCommand(sqlproc, con) ;
			    command.CommandType = CommandType.StoredProcedure ;
			    OleDbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
			    StringBuilder strb = new StringBuilder();
			    while (dr.Read())
			    {	
				    strb.Append( dr.GetString(0) + ",") ;
			    }
					    if (strb.Length>0) strb.Remove(strb.Length-1,1);
			    return strb.ToString().Split(',');
		}
			    
		}

		/// <summary>
		/// Returns all wiki page data (for export)
		/// </summary>
		/// <returns>Wiki pages</returns>
		public DataSet ExportAllWikiPages()
		{
			return ExportWikiPages("ALL","");
		}

		/// <summary>
		/// Returns given wiki page data (for export)
		/// </summary>
		/// <param name="name">Wiki page name</param>
		/// <returns>Wiki page</returns>
		public DataSet ExportMyWikiPages(string name)
		{
			return ExportWikiPages("MY",name);
		}

		/// <summary>
		/// Returns selected wiki page data (for export)
		/// </summary>
		/// <param name="selection">string containing selection</param>
		/// <returns>Wiki pages</returns>
		public DataSet ExportSelectedWikiPages(string selection)
		{
			return ExportWikiPages("SELECTED",selection);
		}

		private DataSet ExportWikiPages (string type, string criteria)
		{
			using (OleDbConnection con = GetDbConnection())
            {
			    OleDbCommand command = new OleDbCommand("WikiSP_ExportWikiPages", con) ;
			    command.CommandType = CommandType.StoredProcedure ;
			    command.Parameters.Add ("@inpExportType", OleDbType.VarChar, 256) ;
			    command.Parameters["@inpExportType"].Value = type ;
			    command.Parameters.Add ("@inpExportCriteria", OleDbType.VarChar, 256) ;
			    command.Parameters["@inpExportCriteria"].Value = criteria ;

			    OleDbDataAdapter adapt = new OleDbDataAdapter(command);
               
			    DataSet ds = new DataSet() ;
			    adapt.Fill(ds, "PageList") ;
			    return ds ;
			}
		}

		/// <summary>
		/// Return wiki page change history
		/// </summary>
		/// <param name="page">Wiki page name</param>
		/// <returns>Wiki page history</returns>
		public DataSet GetWikiPageHistory (string page)
		{
			using (OleDbConnection con = GetDbConnection())
            {
			    OleDbCommand command = new OleDbCommand("WikiSP_GetWikiPageHistory", con) ;
			    command.CommandType = CommandType.StoredProcedure ;
			    command.Parameters.Add ("@inpTitle", OleDbType.VarChar, 64) ;
			    command.Parameters["@inpTitle"].Value = page ;
			    OleDbDataAdapter adapt = new OleDbDataAdapter(command);
                
			    DataSet ds = new DataSet() ;
			    adapt.Fill(ds, "PageList") ;
			    return ds ;
			}
		}

		#region Pages to Email
		/// <summary>
		/// Returns pages updated since the given date
		/// </summary>
		/// <param name="last">Date</param>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmail(DateTime last)
		{
			OleDbConnection con = GetDbConnection();
			OleDbCommand command = new OleDbCommand ("WikiSP_GetPagesToEmail", con) ;
			command.CommandType = CommandType.StoredProcedure ;
			command.Parameters.Add ("@inpLast", OleDbType.Date, 64) ;
			command.Parameters["@inpLast"].Value = last ;
			OleDbDataAdapter adapt = new OleDbDataAdapter(command);
			DataSet ds = new DataSet() ;
			adapt.Fill(ds, "Pages") ;
			con.Close();
			return ds ;
		}		

		/// <summary>
		/// Returns pages updated during the day
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForDay()
		{
			OleDbConnection con = GetDbConnection();
			OleDbCommand command = new OleDbCommand ("WikiSP_GetPagesToEmailDay", con) ;
			command.CommandType = CommandType.StoredProcedure ;
			OleDbDataAdapter adapt = new OleDbDataAdapter(command);
			DataSet ds = new DataSet() ;
			adapt.Fill(ds, "Pages") ;
			con.Close();
			return ds ;
		}

		/// <summary>
		/// Returns pages updated during the last hout
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForHour()
		{
			OleDbConnection con = GetDbConnection();
			OleDbCommand command = new OleDbCommand ("WikiSP_GetPagesToEmailHour", con) ;
			command.CommandType = CommandType.StoredProcedure ;
			OleDbDataAdapter adapt = new OleDbDataAdapter(command);
			DataSet ds = new DataSet() ;
			adapt.Fill(ds, "Pages") ;
			con.Close();
			return ds ;
		}

		/// <summary>
		/// Returns pages updated during last week
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForWeek()
		{
			OleDbConnection con = GetDbConnection();
			OleDbCommand command = new OleDbCommand ("WikiSP_GetPagesToEmailWeek", con) ;
			command.CommandType = CommandType.StoredProcedure ;
			OleDbDataAdapter adapt = new OleDbDataAdapter(command);
			DataSet ds = new DataSet() ;
			adapt.Fill(ds, "Pages") ;
			con.Close();
			return ds ;
		}
		#endregion

		/// <summary>
		/// Deletes wiki page
		/// </summary>
		/// <param name="str">Wiki page name</param>
		public void DeletePage(string str)
		{
			using (OleDbConnection con = GetDbConnection())
			{
			    OleDbCommand command = new OleDbCommand ("WikiSP_DeleteWikiPage", con) ;
			    command.CommandType = CommandType.StoredProcedure ;
			    command.Parameters.Add ("@inpTitle", OleDbType.VarChar, 64) ;
			    command.Parameters["@inpTitle"].Value = str ;
			    command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Runs given SQL query.
		/// Used only for installation process.
		/// </summary>
		/// <param name="sql">sql query</param>
		/// <returns>String concatenend lines</returns>
		public string TrySQL (string sql)
		{ return TrySQL(sql,"");}

		/// <summary>
		/// Runs given SQL query.
		/// Used only for installation process.
		/// </summary>
		/// <param name="sql">sql query</param>
		/// <param name="separator">separator</param>
		/// <returns>String containing first column resuts seperated by given separator string</returns>
		public string TrySQL (string sql,string separator)
		{
			OleDbCommand      command ;
			OleDbDataReader   throwaway ;
			string str = "";
			

			try
			{
			using (OleDbConnection con = GetDbConnection())
			{
				// now set up the command
				command = new OleDbCommand (sql,con);
				command.CommandType = CommandType.Text;

				// put the values into them
				throwaway = command.ExecuteReader (CommandBehavior.CloseConnection) ;
				if (throwaway != null)
				{
					while (throwaway.Read())
					{
						str += throwaway.GetString(0) + separator;
					}
				}
			}
			}
			catch (Exception e)
			{
				throw new WikiException ("An error has occurred executing your SQL query.", e) ;
			}
			if (str.Length > 0) str = str.Substring(0,str.Length-(separator.Length));
			return str;
		}

		#region Log
		/// <summary>
		/// Application logging
		/// </summary>
		/// <param name="type">Log message type</param>
		/// <param name="subtype">Log message subtype</param>
		/// <param name="text">Log message text</param>
		/// <param name="data">Log message data</param>
		public void Log(char type,string subtype,string text,string data)
		{
			OleDbParameter cmdParam ;
			OleDbCommand command ;
			

			try
			{
			    using(OleDbConnection con = GetDbConnection())
			    {
				    // now set up the command
				    command = new OleDbCommand ("WikiSP_Log",con);
				    command.CommandType = CommandType.StoredProcedure;

				    // set up command parameters
				    cmdParam = new OleDbParameter ("@date", SqlDbType.DateTime) ;           
				    cmdParam.Value = DateTime.Now ;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@type", SqlDbType.VarChar) ;         
				    cmdParam.Value = Convert.ToString(type) ;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@subtype", SqlDbType.VarChar) ;      
				    cmdParam.Value = subtype ;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@text", SqlDbType.VarChar) ;      
				    cmdParam.Value = text ;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@data", SqlDbType.VarChar) ;      
				    cmdParam.Value = data ;
				    command.Parameters.Add(cmdParam);
				    // put the values into them
				    command.ExecuteNonQuery() ;
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message,"Error while logging !!!");
			}
		}

		/// <summary>
		/// Logs Wiki page visits
		/// </summary>
		/// <param name="pagetitle">Wiki name</param>
		/// <param name="ipadress">User IP adress</param>
		/// <param name="username">User logon name</param>
		public void LogVisit(string pagetitle,string ipadress,string username)
		{
			OleDbParameter cmdParam ;
			OleDbCommand command ;

			try
			{
			    using (OleDbConnection con = GetDbConnection())
				{
				    // now set up the command
				    command = new OleDbCommand ("WikiSP_LogVisit",con);
				    command.CommandType = CommandType.StoredProcedure;

				    // set up command parameters
				    cmdParam = new OleDbParameter("@WikiPageTitle", SqlDbType.VarChar) ; 
				    cmdParam.Value = pagetitle;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@date", SqlDbType.DateTime) ;           
				    cmdParam.Value = DateTime.Now ;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@ip", SqlDbType.VarChar) ;         
				    cmdParam.Value = ipadress ;
				    command.Parameters.Add(cmdParam);
				    cmdParam = new OleDbParameter ("@user", SqlDbType.VarChar) ;      
				    cmdParam.Value = username ;
				    command.Parameters.Add(cmdParam);
				    // put the values into them
				    command.ExecuteNonQuery() ;
				}
			}
			catch (Exception e)
			{
				throw new WikiException ("An error has occurred logging a page. Try diseabling PAGELOG in Web.Config", e) ;
			}
		}
		#endregion

	}
}
