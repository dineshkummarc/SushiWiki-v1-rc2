using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiLog.
	/// </summary>
	[WikiPageSecurity(true,true)]
	public class wfWikiLog : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.Label lblLogFile;
		protected System.Web.UI.WebControls.DropDownList ddlLogFile;
		protected System.Web.UI.WebControls.Label lblFilter;
		protected System.Web.UI.WebControls.DropDownList ddlFilter;
		protected System.Web.UI.WebControls.LinkButton lbDelete;
		protected System.Web.UI.WebControls.Label lblTitle;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiLog.Title");
			lblLogFile.Text = WikiGui.GetString("Gui.WikiLog.LogFile");
			lbDelete.Text = WikiGui.GetString("Gui.WikiLog.DeleteLogFile");
			lblFilter.Text = WikiGui.GetString("Gui.WikiLog.Filter");
			if (!IsPostBack) InitDdlLogFile();
			// Load log
			StringBuilder buffer = new StringBuilder("<table>");
			string filter = ddlFilter.SelectedValue;
			if (!IsPostBack) ddlFilter.Items.Add("");
			TextReader tr = null;
			if (ddlLogFile.SelectedValue.Length >0)
			{
				tr = File.OpenText(WikiSettings.Singleton().LocalPath + "\\log\\" + ddlLogFile.SelectedValue);
			}
			if (tr == null) tr = WikiManager.Singleton().GetLog().GetLogTextReader();
			using (tr)
			{
				string line;
				while ( (line = tr.ReadLine()) != null)
				{
					string[] data = Server.HtmlEncode(line).Replace(" ","&nbsp;").Split('|');
					// data[0] = log date
					// data[1] = log level
					// data[2] = log type
					// data[3] = log category
					// data[4] = log description
					if ( (!IsPostBack) && (ddlFilter.Items.FindByValue(data[2]) == null) ) ddlFilter.Items.Add(data[2]);
					if ( (filter == "") || (data[2] == filter))
					{
						string font = "face=courier size=1 color=";
						string color = "000000";
						if (data[1] == "w") color="0000FF";
						if ( (data[1] == "c") || (data[1] == "e") )color="FF0000";
						if (data[1] == "d") color="00FF00";
						if (data[1] == "s") color="888888";
						buffer.Append(String.Format("<tr><td><font {0}>{1}</font></td><td><font {0}>{2}</font></td><td><font {0}>{3}</font></td><td><font {0}>{4}</font></td></tr>",font + color,data[0],data[2],data[3],data[4]));
					}
				}
				ddlFilter.SelectedValue = filter;
			}
			buffer.Append("</table>");
			lblInfo.Text = buffer.ToString();
		}

		private void InitDdlLogFile()
		{
			// Init LogFile ddl
			ddlLogFile.Items.Clear();
			ddlLogFile.Items.Add("");
			DirectoryInfo di = new DirectoryInfo(WikiSettings.Singleton().LocalPath + "\\log");
			foreach (FileInfo fi in di.GetFiles("mainlog_*.log"))
			{
				ddlLogFile.Items.Add(fi.Name);
			}
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
			this.lbDelete.Click += new System.EventHandler(this.lbDelete_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void lbDelete_Click(object sender, System.EventArgs e)
		{
			lblInfo.Text = "";
			string file = ddlLogFile.SelectedValue;
			if (file.Length>0)
			{
				File.Delete(WikiSettings.Singleton().LocalPath + "\\log\\" + file);
				WikiManager.Singleton().Log('w',"ADMIN","LOG","Log file <" + file + "> has been deleted");
			}
			InitDdlLogFile();
			ddlLogFile.SelectedValue = "";
		}
	}
}
