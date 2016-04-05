using System;
using System.IO;
using System.Data;
using System.Threading;
using System.Web.Caching;

namespace Wiki
{
	/// <summary>
	/// This class manages the events.
	/// Events are stored in XML files. There is one file for each month, 
	/// and they are stored in the calendar sub directory.
	/// An event is always linked to a page. Unique id = date+page+subject
	/// 
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 1.0   | 08/09/2003 | EGE         | Initial version 
	/// | 1.1   | 08/10/2003 | EGE         | Added caching and sorting
	/// </summary>
	public class WikiCalendar
	{
		#region Version management
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.1";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 1;
		#endregion
		
		#region Singleton stuff
		private static WikiCalendar singleton = null;

		/// <summary>
		/// Get the singleton instance
		/// </summary>
		/// <returns>singleton instance</returns>
		public static WikiCalendar Singleton()
		{
			if (singleton == null) 
			{
				singleton = new WikiCalendar();
			}
			return singleton;
		}
#endregion

		private string _path;
		
		private static string chachePrefix = "CALENDAR__";

		public WikiCalendar()
		{
			// Create directory if it doesn't exist
			string cpath = Path.Combine(WikiSettings.Singleton().LocalPath,"calendar");
			try
			{
				if (!Directory.Exists(cpath)) Directory.CreateDirectory(cpath);
			}
			catch
			{
				throw new WikiException("Application can't create CALENDAR directory. Please create it manually, or give fix application security settings");	
			}
			_path = cpath;
		}

		/// <summary>
		/// Returns a DataTable containing all events for given month defined by a date.
		/// Columns are : 
		///		DateTime date
		///		string subject
		///		string page
		///		string comments
		/// </summary>
		/// <param name="date">date defining the month we must return</param>
		/// <returns>Events</returns>
		public DataRow[] GetEventsForNext30Days(string pageName,DateTime date)
		{
			int year = date.Year;
			int month = date.Month;
			DateTime maxDate = date.AddDays(30);
			DataRow[] rows1 = null;
			DataSet data1 = LoadMonth(year,month);
			if (data1 != null) rows1 = data1.Tables[0].Select("page='" + pageName + "'");
			if (month == 12) { year++; month=1; } 
			else { month++; }
			DataRow[] rows2 = null;
			DataSet data2 = LoadMonth(year,month);
			if (data2 != null) rows2 = data2.Tables[0].Select("page='" + pageName + "' AND date < '" + maxDate.ToString() + "'","date");

			DataRow[] rows = new DataRow[( (data1 != null) ? rows1.Length : 0) + ( (data2 != null) ? rows2.Length : 0 )];
			if (data1 != null) rows1.CopyTo(rows,0);
			if (data2 != null) rows2.CopyTo(rows,( (data1 !=null) ? rows1.Length : 0));

			return rows;
		}

		/// <summary>
		/// Returns a DataTable containing all events for given month defined by a date.
		/// Columns are : 
		///		DateTime date
		///		string subject
		///		string page
		///		string comments
		/// </summary>
		/// <param name="date">date defining the month we must return</param>
		/// <returns>Events</returns>
		public DataTable GetEventsByMonth(DateTime date)
		{
			int year = date.Year;
			int month = date.Month;
			DataSet data = LoadMonth(year,month);
			if (data != null) return data.Tables[0];
			else return null;
		}

		/// <summary>
		/// Returns a DataSet containing events for given month and year
		/// </summary>
		/// <param name="year">year</param>
		/// <param name="month">month</param>
		/// <returns>events </returns>
		private DataSet LoadMonth(int year,int month)
		{
			string name = year.ToString()+"."+month.ToString();
			// Is DataSet already in the cache ?
			DataSet cachedData = (DataSet)WikiManager.GetObjectFromApplicationCache(chachePrefix + name);
			if (cachedData != null)
			{
				return cachedData;
			}
			else
			{
				// Load the data
				string file = Path.Combine(_path,name+".xml");
				if (File.Exists(file))
				{
					DataSet data = new DataSet();
					data.ReadXmlSchema(file + ".xsl");
					data.ReadXml(file);//,XmlReadMode.IgnoreSchema);
					WikiManager.AddObjectToApplicationCache(chachePrefix + name,data);
					return data;
				}
				else return null;
			}
		}

		/// <summary>
		/// Adds an event
		/// </summary>
		/// <param name="date">event date</param>
		/// <param name="subject">event subject</param>
		/// <param name="page">event linked page</param>
		/// <param name="comments">event comments</param>
		public void AddEvent(DateTime date, string subject, string page, string comments)
		{	
			int year = date.Year;
			int month = date.Month;
			string name = year.ToString()+"."+month.ToString();
			lock (this) // Prevent multi-user concurrency
			{
				// Empty cache
				WikiManager.RemoveObjectFromApplicationCache(chachePrefix + name);
				// Load data
				DataSet data = LoadMonth(date.Year,date.Month);
				// Add event
				if (data == null)
				{
					data = new DataSet();
					DataTable table = data.Tables.Add();
					table.Columns.Add("date",typeof(DateTime));
					table.Columns.Add("subject",typeof(string));
					table.Columns.Add("page",typeof(string));
					table.Columns.Add("comments",typeof(string));
				}
				data.Tables[0].Rows.Add(new object[] {date,subject,page,comments});
				// Save data
				string file = Path.Combine(_path,date.Year.ToString()+"."+date.Month.ToString()+".xml");
				data.WriteXml(file);
				data.WriteXmlSchema(file + ".xsl");
				// Add DataSet to cache
				WikiManager.AddObjectToApplicationCache(chachePrefix + name,data);
			}
		}

		/// <summary>
		/// Deletes an event
		/// </summary>
		/// <param name="date">event date</param>
		/// <param name="subject">event subject</param>
		/// <param name="page">event page</param>
		public void DeleteEvent(DateTime date,string subject,string page)
		{
			lock (this)
			{
				int year = date.Year;
				int month = date.Month;
				string name = year.ToString()+"."+month.ToString();
				// Empty cache
				WikiManager.RemoveObjectFromApplicationCache(chachePrefix + name);
				// Load data
				DataSet data = LoadMonth(date.Year,date.Month);
				DataRow[] rows = data.Tables[0].Select("date='" + date.ToString() + "' and page='" + page + "' and subject='" + subject + "'");
				// Remove event
				data.Tables[0].Rows.Remove(rows[0]);
				// Save data
				string file = Path.Combine(_path,date.Year.ToString()+"."+date.Month.ToString()+".xml");
				data.WriteXml(file);
				data.WriteXmlSchema(file + ".xsl");
				// Add DataSet to cache
				WikiManager.AddObjectToApplicationCache(chachePrefix + name,data);
			}
		}
	}
}
