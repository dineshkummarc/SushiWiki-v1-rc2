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
	/// Summary description for WikiCalendar.
	/// </summary>
	[WikiPageHelp("WikiCalendar")]
	[WikiPageSecurity(true,false)]	
	public class wfWikiCalendar : WikiPage
	{
		protected System.Web.UI.WebControls.Calendar calMonth1;
		protected System.Web.UI.WebControls.Calendar calMonth2;
		protected System.Web.UI.WebControls.DataGrid dgEvents1;
		protected System.Web.UI.WebControls.DataGrid dgEvents2;
		protected System.Web.UI.WebControls.DataGrid dgEvents3;
		protected System.Web.UI.WebControls.Label lblMonth1;
		protected System.Web.UI.WebControls.Label lblMonth3;
		protected System.Web.UI.WebControls.Label lblMonth2;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Calendar calMonth3;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			lblTitle.Text = WikiGui.GetString("Gui.WikiCalendar.Title");
			// Select current day
			if (!IsPostBack)
			{
				calMonth1.VisibleDate = DateTime.Now;
				calMonth1.SelectedDate = DateTime.Now;
				calMonth2.VisibleDate = DateTime.Now.AddMonths(1);
				calMonth3.VisibleDate = DateTime.Now.AddMonths(2);
			}
			LoadEvents();
		}

		private void LoadEvents()
		{
			lblMonth1.Text = calMonth1.VisibleDate.ToString("MMMM") + " " + calMonth1.VisibleDate.Year.ToString();			 
			dgEvents1.DataSource = WikiCalendar.Singleton().GetEventsByMonth(calMonth1.VisibleDate);
			dgEvents1.DataBind();
			lblMonth2.Text = calMonth2.VisibleDate.ToString("MMMM") + " " + calMonth2.VisibleDate.Year.ToString();			 
			dgEvents2.DataSource = WikiCalendar.Singleton().GetEventsByMonth(calMonth2.VisibleDate);
			dgEvents2.DataBind();
			lblMonth3.Text = calMonth3.VisibleDate.ToString("MMMM") + " " + calMonth3.VisibleDate.Year.ToString();			 
			dgEvents3.DataSource = WikiCalendar.Singleton().GetEventsByMonth(calMonth3.VisibleDate);
			dgEvents3.DataBind();
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
			this.calMonth1.DayRender += new System.Web.UI.WebControls.DayRenderEventHandler(this.calMonth1_DayRender);
			this.calMonth1.VisibleMonthChanged += new System.Web.UI.WebControls.MonthChangedEventHandler(this.calMonth1_VisibleMonthChanged);
			this.calMonth1.SelectionChanged += new System.EventHandler(this.calMonth1_SelectionChanged);
			this.calMonth2.DayRender += new System.Web.UI.WebControls.DayRenderEventHandler(this.calMonth1_DayRender);
			this.calMonth2.VisibleMonthChanged += new System.Web.UI.WebControls.MonthChangedEventHandler(this.calMonth2_VisibleMonthChanged);
			this.calMonth2.SelectionChanged += new System.EventHandler(this.calMonth2_SelectionChanged);
			this.calMonth3.DayRender += new System.Web.UI.WebControls.DayRenderEventHandler(this.calMonth1_DayRender);
			this.calMonth3.VisibleMonthChanged += new System.Web.UI.WebControls.MonthChangedEventHandler(this.calMonth3_VisibleMonthChanged);
			this.calMonth3.SelectionChanged += new System.EventHandler(this.calMonth3_SelectionChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void calMonth1_SelectionChanged(object sender, System.EventArgs e)
		{
			calMonth2.SelectedDate = DateTime.MinValue;
			calMonth3.SelectedDate = DateTime.MinValue;
		}

		private void calMonth2_SelectionChanged(object sender, System.EventArgs e)
		{
			calMonth1.SelectedDate = DateTime.MinValue;
			calMonth3.SelectedDate = DateTime.MinValue;
		
		}

		private void calMonth3_SelectionChanged(object sender, System.EventArgs e)
		{
			calMonth1.SelectedDate = DateTime.MinValue;
			calMonth2.SelectedDate = DateTime.MinValue;		
		}

		private void calMonth1_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
		{
			WikiGui.DayRender(e);	
			e.Cell.Text = e.Day.DayNumberText;
		}

		private void calMonth1_VisibleMonthChanged(object sender, System.Web.UI.WebControls.MonthChangedEventArgs e)
		{
			calMonth2.VisibleDate = calMonth1.VisibleDate.AddMonths(1);
			calMonth3.VisibleDate = calMonth1.VisibleDate.AddMonths(2);
		}

		private void calMonth2_VisibleMonthChanged(object sender, System.Web.UI.WebControls.MonthChangedEventArgs e)
		{
			calMonth1.VisibleDate = calMonth2.VisibleDate.AddMonths(-1);
			calMonth3.VisibleDate = calMonth2.VisibleDate.AddMonths(1);		
		}

		private void calMonth3_VisibleMonthChanged(object sender, System.Web.UI.WebControls.MonthChangedEventArgs e)
		{
			calMonth1.VisibleDate = calMonth3.VisibleDate.AddMonths(-2);
			calMonth2.VisibleDate = calMonth3.VisibleDate.AddMonths(-1);		
		}
	}
}
