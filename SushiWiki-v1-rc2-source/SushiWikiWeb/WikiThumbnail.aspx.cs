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
using Wiki.Tools;
using System.IO;

namespace Wiki.GUI
{
	/// <summary>
	/// This page holds the cache management for accessing thumbnails.
	/// They are stored in ASP.NET chache using "thumb_" prefix.
	/// This cache is also 
	/// </summary>
	[WikiPageSecurity(false,false)]
	public class wfWikiThumbnail : WikiPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			string page = Request.QueryString["page"];
			string file = Request.QueryString["file"];

			if ( (page != null) && (file != null) )
			{
				// Get size
				string smax = Request.QueryString["size"];
				double max = 100;
				if (smax != null) max = Convert.ToInt32(smax);
				// create an image object respecting ratio
				System.Drawing.Image image = System.Drawing.Image.FromFile(Server.MapPath("pub/" + page + "/" + file));
				// create the actual thumbnail image
				double xrate = max / Math.Max(image.Width,image.Height);
				System.Drawing.Image thumbnailImage = image.GetThumbnailImage(Convert.ToInt32(image.Width * xrate),Convert.ToInt32(image.Height * xrate),  new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
				// make a memory stream to work with the image bytes
				MemoryStream imageStream = new MemoryStream();
				// put the image into the memory stream
				thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
				// make byte array the same size as the image
				byte[] imageContent = new Byte[imageStream.Length];
				// rewind the memory stream
				imageStream.Position = 0;
				// load the byte array with the image
				imageStream.Read(imageContent, 0, (int)imageStream.Length);
				// return byte array to caller with image type
				Response.ContentType = "image/jpeg";
				Response.BinaryWrite(imageContent);
			}
		}

		/// <summary>
		/// Required, but not used
		/// </summary>
		/// <returns>true</returns>
		public bool ThumbnailCallback()
		{
			return true;
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
