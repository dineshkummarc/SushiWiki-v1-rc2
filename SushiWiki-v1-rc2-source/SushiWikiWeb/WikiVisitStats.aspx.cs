using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using System.Text;
using Wiki;
using Wiki.Storage.SQL;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiVisitStats.
	/// </summary>
	[WikiPageSecurity(true,false)]
	public class wfWikiVisitStats : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblResults;
		protected System.Web.UI.WebControls.RadioButton rbVisits;
		protected System.Data.OleDb.OleDbCommand sqlCommandGetNbrVisitorsPerDay;
		protected System.Data.OleDb.OleDbCommand sqlCommandGetNbrHitsPerDay;
		protected System.Data.OleDb.OleDbCommand sqlCommandGetDownloadsPerDay;
		protected System.Web.UI.WebControls.RadioButton rbGraphics;
		protected System.Web.UI.WebControls.LinkButton lbPurge;
		protected System.Data.OleDb.OleDbCommand sqlCommandPurgeVisits;
		protected System.Web.UI.WebControls.Label lblPurge;
		protected System.Web.UI.WebControls.RadioButton rbHits;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!WikiUserSettings.IsUserAdministrator(Session))
			{

				Response.Redirect(wfWikiError.GerUrlForPageAccessDenied(),true);
			}
			if (WikiSettings.Singleton().Storage != "SQL")
			{
				Response.Redirect(wfWikiError.GerUrlForSQLStorageRequired(),true);
			}

			LoadData();
		}

		private void LoadData()
		{
			string action = Request.QueryString["action"];
			switch (action)
			{
				case "VisitorsPerDay":
					GeneratePicture("VisitorsPerDay");
					break;
				case "HitsPerDay":
					GeneratePicture("HitsPerDay");
					break;
				default:
					if (rbGraphics.Checked)
					{
						lblResults.Text = "Visitor (unique IP) per day :<br>";
						lblResults.Text += "<img src=WikiVisitStats.aspx?action=VisitorsPerDay><BR><BR>";
						lblResults.Text += "HITS per day :<br>";
						lblResults.Text += "<img src=WikiVisitStats.aspx?action=HitsPerDay><BR><BR>";
					}
					else LoadDetails();
					break;
			}
			lblPurge.Text = "(keep last <B>" + WikiSettings.Singleton().VisitsPreserveTime.ToString() + "</B> days)";
		}

		private void LoadDetails()
		{
			WikiStorageSql manager = (WikiStorageSql)WikiManager.Singleton().GetStorageManager();
			OleDbConnection  sqlConnection = manager.GetDbConnection();
			sqlCommandGetDownloadsPerDay.Connection = sqlConnection;
			sqlCommandGetNbrHitsPerDay.Connection = sqlConnection;
			sqlCommandGetNbrVisitorsPerDay.Connection = sqlConnection;

			OleDbDataReader data = null;
			if (rbVisits.Checked) data = sqlCommandGetNbrVisitorsPerDay.ExecuteReader();
			if (rbHits.Checked) data = sqlCommandGetNbrHitsPerDay.ExecuteReader();
			ArrayList dates = new ArrayList();
			ArrayList nbrs = new ArrayList();
			DateTime datemin = DateTime.Now;
			DateTime datemax = DateTime.Now;
			float max = 0;
			while (data.Read())
			{
				DateTime date = data.GetDateTime(0);
				int nbr = data.GetInt32(1);
				if (date<datemin) datemin = date;
				if (date>datemax) datemax = date;
				if (nbr>max) max = nbr;
				dates.Add(date);
				nbrs.Add(nbr);
			}
			sqlConnection.Close();
			DateTime curdate = datemin;
			StringBuilder str = new StringBuilder("<table border=1><thead><tr><td>Date</td><td>Count</td><td></td></tr></thead><tbody>");
			int i = 0;
			foreach (DateTime d in dates)
			{
				while (curdate < d)
				{ 
					curdate = curdate.AddDays(1);
					str.Append("</td></tr><tr><td>");
					str.Append(curdate.ToShortDateString());
					str.Append("</td><td>0</td><td>");
				}

				str.Append("<tr><td>");
				str.Append(d.ToShortDateString());
				str.Append("</td><td>");
				str.Append((int)nbrs[i]);
				str.Append("</td><td>");
				str.Append("<table width=");
				str.Append( Convert.ToInt32((((int)nbrs[i])/max)*100));
				str.Append(" bgcolor=000000><tr><td>&nbsp;</td></tr></table>");
				str.Append("</td></tr>");
				i++;
				curdate = curdate.AddDays(1);
			}
			str.Append("</tbody></table>");
			lblResults.Text = str.ToString();

		}

		private void GeneratePicture(string type)
		{
			// Connect to database and run proc
			WikiStorageSql manager = (WikiStorageSql)WikiManager.Singleton().GetStorageManager();
			OleDbConnection  sqlConnection = manager.GetDbConnection();
			sqlCommandGetDownloadsPerDay.Connection = sqlConnection;
			sqlCommandGetNbrHitsPerDay.Connection = sqlConnection;
			sqlCommandGetNbrVisitorsPerDay.Connection = sqlConnection;
			OleDbDataReader data = null;
			if (type=="VisitorsPerDay") data = sqlCommandGetNbrVisitorsPerDay.ExecuteReader();
			if (type=="HitsPerDay") data = sqlCommandGetNbrHitsPerDay.ExecuteReader();
			// Retreive Data in ArrayLists and calculate min and max
			ArrayList dates = new ArrayList();
			ArrayList nbrs = new ArrayList();
			DateTime datemin = DateTime.Now;
			DateTime datemax = DateTime.Now;
			float max = 0;
			while (data.Read())
			{
				DateTime date = data.GetDateTime(0);
				int nbr = data.GetInt32(1);
				if (date<datemin) datemin = date;
				if (date>datemax) datemax = date;
				if (nbr>max) max = nbr;
				dates.Add(date);
				nbrs.Add(nbr);
			}
			sqlConnection.Close();
			// Draw chart
			int delta = Convert.ToInt32(datemax.Subtract(datemin).TotalDays);
			LineChart c = new LineChart(980, 270, Page);
			c.SetTitle("Stats from " + datemin.ToShortDateString() + " to " + datemax.ToShortDateString() + " (" + Convert.ToInt32(datemax.Subtract(datemin).TotalDays).ToString() + " days)");
			c.SetScale(delta,max);
			c.SetOrigin(0,0);
			if (dates.Count>0)
			{
				if (delta>35) c.SetDivs(3,5) ; else	c.SetDivs(delta,5);
				int i=0;
				DateTime previous = datemin;
				foreach (DateTime d in dates)
				{
					int day = Convert.ToInt32(d.Subtract(datemin).TotalDays);
					int prevday = Convert.ToInt32(previous.Subtract(datemin).TotalDays);
					// Add missing days (for zero visit days)
					for (int missingday=prevday+1 ; missingday<day ; missingday++)
					{
						c.AddValue(missingday,0);
					}
					previous = d;
					c.AddValue(d,(int)nbrs[i]);
					i++;
				}
			}
			c.Draw();
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.sqlCommandGetNbrVisitorsPerDay = new System.Data.OleDb.OleDbCommand();
			this.sqlCommandGetNbrHitsPerDay = new System.Data.OleDb.OleDbCommand();
			this.sqlCommandGetDownloadsPerDay = new System.Data.OleDb.OleDbCommand();
			this.sqlCommandPurgeVisits = new System.Data.OleDb.OleDbCommand();
			this.rbVisits.CheckedChanged += new System.EventHandler(this.viewChanged);
			this.rbHits.CheckedChanged += new System.EventHandler(this.viewChanged);
			this.lbPurge.Click += new System.EventHandler(this.lbPurge_Click);
			// 
			// sqlCommandGetNbrVisitorsPerDay
			// 
			this.sqlCommandGetNbrVisitorsPerDay.CommandText = "WikiSP_GetNbrVisitorsPerDay";
			this.sqlCommandGetNbrVisitorsPerDay.CommandType = System.Data.CommandType.StoredProcedure;
			this.sqlCommandGetNbrVisitorsPerDay.Parameters.Add(new System.Data.OleDb.OleDbParameter("@RETURN_VALUE", System.Data.OleDb.OleDbType.Integer, 4, System.Data.ParameterDirection.ReturnValue, false, ((System.Byte)(10)), ((System.Byte)(0)), "", System.Data.DataRowVersion.Current, null));
			// 
			// sqlCommandGetNbrHitsPerDay
			// 
			this.sqlCommandGetNbrHitsPerDay.CommandText = "WikiSP_GetNbrHitsPerDay";
			this.sqlCommandGetNbrHitsPerDay.CommandType = System.Data.CommandType.StoredProcedure;
			this.sqlCommandGetNbrHitsPerDay.Parameters.Add(new System.Data.OleDb.OleDbParameter("@RETURN_VALUE", System.Data.OleDb.OleDbType.Integer, 4, System.Data.ParameterDirection.ReturnValue, false, ((System.Byte)(10)), ((System.Byte)(0)), "", System.Data.DataRowVersion.Current, null));
			// 
			// sqlCommandGetDownloadsPerDay
			// 
			this.sqlCommandGetDownloadsPerDay.CommandText = "WikiSP_GetDownloadsPerDay";
			this.sqlCommandGetDownloadsPerDay.CommandType = System.Data.CommandType.StoredProcedure;
			this.sqlCommandGetDownloadsPerDay.Parameters.Add(new System.Data.OleDb.OleDbParameter("@RETURN_VALUE", System.Data.OleDb.OleDbType.Integer, 4, System.Data.ParameterDirection.ReturnValue, false, ((System.Byte)(10)), ((System.Byte)(0)), "", System.Data.DataRowVersion.Current, null));
			// 
			// sqlCommandPurgeVisits
			// 
			this.sqlCommandPurgeVisits.CommandText = "WikiSP_PurgeVisits";
			this.sqlCommandPurgeVisits.CommandType = System.Data.CommandType.StoredProcedure;
			this.sqlCommandPurgeVisits.Parameters.Add(new System.Data.OleDb.OleDbParameter("@RETURN_VALUE", System.Data.OleDb.OleDbType.Integer, 4, System.Data.ParameterDirection.ReturnValue, false, ((System.Byte)(10)), ((System.Byte)(0)), "", System.Data.DataRowVersion.Current, null));
			this.sqlCommandPurgeVisits.Parameters.Add(new System.Data.OleDb.OleDbParameter("@preserve", System.Data.OleDb.OleDbType.Integer, 4, System.Data.ParameterDirection.Input, false, ((System.Byte)(10)), ((System.Byte)(0)), "", System.Data.DataRowVersion.Current, null));
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void viewChanged(object sender, System.EventArgs e)
		{
			LoadData();
		}

		private void lbPurge_Click(object sender, System.EventArgs e)
		{
			WikiStorageSql manager = (WikiStorageSql)WikiManager.Singleton().GetStorageManager();
			OleDbConnection  sqlConnection = manager.GetDbConnection();
			sqlCommandPurgeVisits.Connection = sqlConnection;
			sqlCommandPurgeVisits.Parameters["@preserve"].Value = WikiSettings.Singleton().VisitsPreserveTime;
			sqlCommandPurgeVisits.ExecuteScalar();
			sqlConnection.Close();
		}
	}
}
