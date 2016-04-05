namespace Wiki.GUI.Templates
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Wiki.GUI;
	using Wiki.Tools;
	using Wiki.Render;

	/// <summary>
	///		Summary description for Footer.
	/// </summary>
	public abstract class Footer : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblSignature;
		protected System.Web.UI.WebControls.Label lVersion;

		private void Page_Load(object sender, System.EventArgs e)
		{
			lVersion.Text  = "v" + WikiManager.v + " R" 
				+ Convert.ToString(
				+ WikiGui.r 
				+ WikiManager.r 
				+ Wiki.Storage.SQL.WikiStorageSql.r
				+ Wiki.Storage.XML.WikiStorageXml.r
				+ WikiRender.r 
				+ WikiRenderWiki.r 
				+WikiRobot.r 
				+ WikiSettings.r) ; 
			lblSignature.Text = WikiSettings.Singleton().Signature;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
