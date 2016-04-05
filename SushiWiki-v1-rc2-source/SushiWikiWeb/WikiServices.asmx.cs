using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using Wiki.Render;

namespace Wiki.WebServices
{
	/// <summary>
	/// Summary description for WebServicesEdition.
	/// </summary>
	[WebService(Namespace="Wiki.WebServices")]
	public class WikiEditionWebServices : System.Web.Services.WebService
	{
		/// <summary>
		/// 
		/// </summary>
		public WikiEditionWebServices()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/// <summary>
		/// Get wiki parsed page
		/// </summary>
		[WebMethod(Description="Get the rendered page (after HTML rendering)")]
		public string GetRenderedPage(string name)
		{
			WikiManager.PageData currentPage = GetPageRawData(name);
			BaseRenderer renderer = WikiRender.GetRenderer(currentPage.type,currentPage.pageData,currentPage.title); 
			return renderer.Format(false); 
		}
		/// <summary>
		/// Gets a wiki page
		/// </summary>
		[WebMethod(Description="Get the page RAW data (before HMLT rendering).")]
		public WikiManager.PageData GetPageRawData(string name)
		{
			try
			{
				return WikiManager.Singleton().GetWikiPage(name);
			}
			catch
			{
				WikiManager.PageData data = new WikiManager.PageData();
				return data;
			}
		}
	}
}
