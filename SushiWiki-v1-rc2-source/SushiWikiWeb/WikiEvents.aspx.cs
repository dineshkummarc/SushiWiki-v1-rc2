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

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiEvents.
	/// </summary>
	[WikiPageHelp("WikiEvents")]
	[WikiPageSecurity(true,false)]	
	public class wfWikiEvents : WikiPage
	{
		protected System.Web.UI.WebControls.TextBox tbDate;
		protected System.Web.UI.WebControls.Label lblDate;
		protected System.Web.UI.WebControls.TextBox tbSubject;
		protected System.Web.UI.WebControls.Label lblSubject;
		protected System.Web.UI.WebControls.Label lblPage;
		protected System.Web.UI.WebControls.Label lblPageValue;
		protected System.Web.UI.WebControls.Label lblComments;
		protected System.Web.UI.WebControls.TextBox tbComments;
		protected System.Web.UI.WebControls.DataGrid dgEvents;
		protected System.Web.UI.WebControls.Label lblMonth;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.RequiredFieldValidator validDate;
		protected System.Web.UI.WebControls.RequiredFieldValidator validSubject;
		protected System.Web.UI.WebControls.Calendar CalDate;
		protected System.Web.UI.WebControls.Label lblHisto;
		protected System.Web.UI.WebControls.Button butAddEvent;
	
		public static string GetUrlForLoad(string page)
		{
			return "WikiEvents.aspx?page=" + page;
		}
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Get page name
			lblPageValue.Text = Request.QueryString["page"];
			// Update history information
			lblHisto.Text = WikiGui.GetHisto(Session);
			// Set labels
			lblTitle.Text = WikiGui.GetString("Gui.WikiEvents.Title");
			lblDate.Text = WikiGui.GetString("Gui.WikiEvents.Date");
			lblPage.Text = WikiGui.GetString("Gui.WikiEvents.Page");
			lblSubject.Text = WikiGui.GetString("Gui.WikiEvents.Subject");
			lblComments.Text = WikiGui.GetString("Gui.WikiEvents.Comments");
			butAddEvent.Text = WikiGui.GetString("Gui.WikiEvents.AddEvent");
			if (!IsPostBack)
			{
				CalDate.SelectedDate = DateTime.Now;
				tbDate.Text = CalDate.SelectedDate.ToString();
				LoadEvents();
			}
		}

		private DataTable _events;

		private void LoadEvents()
		{
			_events = WikiCalendar.Singleton().GetEventsByMonth(CalDate.SelectedDate);
			lblMonth.Text = CalDate.SelectedDate.ToString("MMMM") + " " + CalDate.SelectedDate.Year.ToString();
			dgEvents.DataSource = _events;
			dgEvents.DataBind();
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
			this.butAddEvent.Click += new System.EventHandler(this.butAddEvent_Click);
			this.dgEvents.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEvents_DeleteCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void butAddEvent_Click(object sender, System.EventArgs e)
		{
			WikiCalendar.Singleton().AddEvent(
				Convert.ToDateTime(tbDate.Text),
				tbSubject.Text,
				lblPageValue.Text,
				tbComments.Text);
			LoadEvents();
		}

		private void CalDate_SelectionChanged(object sender, System.EventArgs e)
		{
			tbDate.Text = CalDate.SelectedDate.ToString();
			LoadEvents();
		}

		private void CalDate_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
		{
			WikiGui.DayRender(e);
		}

		private void dgEvents_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			TableCellCollection cells = e.Item.Cells;
			WikiCalendar.Singleton().DeleteEvent(Convert.ToDateTime(cells[1].Text),cells[2].Text,lblPageValue.Text);
			LoadEvents();
		}

	}
}
