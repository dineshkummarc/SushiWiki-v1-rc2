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
using System.Xml;
using System.IO;

using Wiki.Storage.SQL;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiInstall.
	/// </summary>
	public class wfWikiInstall : WikiWebForm
	{
		protected System.Web.UI.WebControls.Button bProceed;
		protected System.Web.UI.WebControls.Label lStep;
		protected System.Web.UI.WebControls.Label lInfo;
	
		string[] objectsnames;
		protected System.Web.UI.WebControls.Label lProceedInfo;
		protected System.Web.UI.WebControls.Label lStep1;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.Label Label4;
		protected System.Web.UI.WebControls.Label lStep2;
		protected System.Web.UI.WebControls.Label lStep3;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lFinished;
		Hashtable objects;
 
		private void Page_Load(object sender, System.EventArgs e)
		{
			CheckUser();
			// Load object list
			if (!WikiUserSettings.IsUserAdministrator(Session))
			{
				Response.Redirect("WikiError.aspx?code=",true);
			}

			Response.Cache.SetCacheability(HttpCacheability.NoCache) ;
			lInfo.Text = "";
			if (Request.QueryString["SAVEOBJECTS"] == "true")
			{
				SaveObjects();
			}
			else 
			{
				objects = new Hashtable();
				string str = "";
				string file = Server.MapPath(".") + "\\install\\sqlobjects.xml";
				if (System.IO.File.Exists(file))
				{ // Load settings
					XmlTextReader data = new XmlTextReader(file);
					string sLastNodeName = "";
					while (data.Read())
					{
						if (data.NodeType == XmlNodeType.Element)
							sLastNodeName = data.Name;
						if (data.NodeType == XmlNodeType.Text)
						{
							objects.Add(sLastNodeName,data.Value);
							if ( (sLastNodeName != "WikiPages") 
								&& (sLastNodeName != "WikiLog") 
								&& (sLastNodeName != "WikiVisits") )
							{ str += sLastNodeName + ",";}
						}
					}
					data.Close();
				}
				else
				{
					throw new WikiException("XML file containing SQL objects is missing (" + file + ")");
				}
				objectsnames = str.Substring(0,str.Length-1).Split(',');
				if (!IsPostBack) RunInstall();
			}
		}

		private void RunInstall()
		{
			// Define current step
			int step = DetectStep();
			lStep.Text = step.ToString();
			//=====================================

			switch (step)
			{
				case 0: 
					lStep1.ForeColor = Color.Gainsboro;
					lStep2.ForeColor = Color.Gainsboro;
					lStep3.ForeColor = Color.Gainsboro;
					lFinished.ForeColor = Color.Black;
					break;

				case 1:
					lInfo.Text = "Some talbes has not been found in your database.</font><br><br>Click button to create these tables.";
					lStep1.ForeColor = Color.Gainsboro;
					lStep2.ForeColor = Color.Black;
					lStep3.ForeColor = Color.Gainsboro;
					lFinished.ForeColor = Color.Gainsboro;
					bProceed.Text = "Create missing tables";
					bProceed.Visible = true;
					break;

				case 2:
					lInfo.Text += "<br><br>Click button to create stores procedures";
					lStep1.ForeColor = Color.Gainsboro;
					lStep2.ForeColor = Color.Gainsboro;
					lStep3.ForeColor = Color.Black;
					lFinished.ForeColor = Color.Gainsboro;
					bProceed.Text = "Create stored procs";
					bProceed.Visible = true;
					break;
			}
		}

		private void SaveObjects()
		{
			string file = Server.MapPath(".") + "\\install\\sqlobjects.xml";
			XmlTextWriter data = new XmlTextWriter(file,System.Text.Encoding.UTF8);
			data.WriteStartElement("RIKIWIKI");
			data.WriteStartElement("SQL_OBJECTS");
			string[] objects = TrySQL("SELECT name FROM  sysobjects WHERE (type = 'P') AND (name LIKE 'WikiSP_%')",",").Split(',');
			foreach (string o in objects)
			{
				data.WriteElementString(o,TrySQL("sp_helptext  " + o ));
				lProceedInfo.Text += "Saving <b>" + o + "</b> from database<br>";
			}
			string table = "CREATE TABLE [WikiPages]([lastUpdated] [datetime] NOT NULL, [title] [varchar](256)  NOT NULL, [type] [varchar](16) NOT NULL DEFAULT ('WIKI'), [updatedBy] [varchar](256) NOT NULL, [pageData] [ntext] NULL, [lockedBy] [varchar](256) NOT NULL  DEFAULT (''), [id] [decimal](18, 0) IDENTITY (1, 1) NOT NULL , [ownedBy] [varchar](256) NOT NULL DEFAULT ('Wiki'), [public] [bit] NOT NULL DEFAULT (1)) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
			lProceedInfo.Text += "Saving <b>WikiPages</b> from source code (WikiInstall.aspx.cs)<br>";
			data.WriteElementString("WikiPages",table);

			table = "CREATE TABLE [wikiLog] ([date] [datetime] NOT NULL ,[type] [varchar] (64) NOT NULL DEFAULT ('unknown'),[text] [varchar] (256)  NOT NULL DEFAULT (''),[data] [text] NOT NULL DEFAULT (''),[subtype] [varchar] (64) NOT NULL DEFAULT ('')) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
			lProceedInfo.Text += "Saving <b>WikiLog</b> from source code (WikiInstall.aspx.cs)<br>";
			data.WriteElementString("WikiLog",table);
			
			table = "CREATE TABLE [WikiVisits] ([ref_wikipagetitle] [varchar] (256)  NOT NULL ,[date] [datetime] NOT NULL ,[ip] [varchar] (20) NULL ,[user] [varchar] (256) NOT NULL ) ON [PRIMARY]";
			lProceedInfo.Text += "Saving <b>WikiLog</b> from source code (WikiInstall.aspx.cs)<br>";
			data.WriteElementString("WikiVisits",table);

			data.Close();				
			lInfo.Text = "All SQL objects has been saved into "+ file;
		}

		private int DetectStep()
		{
			try
			{ TrySQL("sp_help WikiPages");	}
			catch
			{ return 1; }; // Table not found

			// Check stored procedures
			bool storeproc = true;
			int s;

			foreach (string o in objectsnames)
			{
				s = CheckObject(o);
				storeproc = (storeproc && (s==1));
			}
			if (!storeproc) return 2;

			return 0;
		}

		private int CheckObject(string name)
		{
			string sql = "";
			try
			{ sql = TrySQL("sp_helptext " + name); }
			catch 
			{
				lInfo.Text += "<font color=red>SQL object <b>" + name + "</b> not found</font><br>";
				return 0; 
			}
			string sql2 = (string)objects[name];
			if (sql == sql2)
			{
				lInfo.Text += "<font color=green>SQL object <b>" + name + "</b> found and up to date</font><br>";
				return 1;
			}
			else
			{
				lInfo.Text += "<font color=red>SQL object <b>" + name + "</b> found but not up to date</font><br>";
				return -1;
			}
		}

		public string TrySQL(string sql)
		{
			return ((WikiStorageSql)WikiManager.Singleton().GetStorageManager()).TrySQL(sql);
		}

		public string TrySQL (string sql,string separator)
		{
			return ((WikiStorageSql)WikiManager.Singleton().GetStorageManager()).TrySQL(sql,separator);
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
			this.bProceed.Click += new System.EventHandler(this.bProceed_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void bProceed_Click(object sender, System.EventArgs e)
		{	
			switch (lStep.Text)
			{
				case "1":
					TrySQL((string)objects["WikiPages"]);	
					TrySQL((string)objects["WikiVisits"]);	
					TrySQL((string)objects["WikiLog"]);	
					lProceedInfo.Text = "Missing Tables created";
					RunInstall();
					break;

				case "2":
					foreach (string o in objectsnames)
					{
						int status = CheckObject(o);
						if (status == -1) 
						{ // Objet exists, must drop it first
							TrySQL("DROP PROCEDURE " + o);
						}
						try
						{
							if (CheckObject(o) != 1)
								TrySQL((string)objects[o]);
						}
						catch (Exception ex)
						{
							throw new WikiException("Error creating " + o + " (" + (string)objects[o] + ")",ex); 
						}
					}
					lProceedInfo.Text = "Stored procedures created/updated";
					RunInstall();
					break;

			}
		}
	}
}
