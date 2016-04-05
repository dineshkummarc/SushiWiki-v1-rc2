using System;
using System.Collections;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;

namespace Wiki.GUI
{

	/// <summary>
	/// Creates a PNG flushed in given WebForm displaying line chart.
	/// Values are added using <see cref="LineChart.AddValue"/> 
	/// and chart is generated when calling <see cref="LineChart.Draw"/>.
	/// 
	/// Source code is base on C# sample from Steve Hall, 2002.
	/// </summary>
	public class LineChart
	{
		private Bitmap b;
		private string Title="Generated @" + DateTime.Now.ToLongDateString();
		private ArrayList chartValues = new ArrayList();
		private float Xorigin=0, Yorigin=0;
		private float ScaleX, ScaleY;
		private float Xdivs=2, Ydivs=2;

		private int Width, Height;
		private Graphics g;
		private Page p;

		public void SetTitle(string title)
		{
			Title = title;
		}

		public void SetScale(float x,float y)
		{
			ScaleX = x;
			ScaleY = y;
		}

		public void SetOrigin(float x,float y)
		{
			Xorigin = x;
			Yorigin = y;
		}

		public void SetDivs(float x,float y)
		{
			Xdivs = x;
			Ydivs = y;
		}
		struct datapoint 
		{
			public float x;
			public float y;
			public bool valid;
		}

		enum DataType 
		{
			Integer,
			Date }
		//initialize

		private DataType dataType = DataType.Integer;
		DateTime dateMin = DateTime.MinValue;

		public LineChart(int myWidth, int myHeight, Page myPage) 
		{
			Width = myWidth; Height = myHeight;
			ScaleX = myWidth; ScaleY = myHeight;
			b = new Bitmap(myWidth, myHeight);
			g = Graphics.FromImage(b);
			p = myPage;
		}

		public void AddValue(DateTime d,int y)
		{
			if (dateMin == DateTime.MinValue)
			{
				dataType = DataType.Date;
				dateMin = d;
			}
			Debug.Assert(dataType == DataType.Date,"Current data type is Integer","you can't add a DateTime data at this time");
			AddValue(Convert.ToInt32(d.Subtract(dateMin).TotalDays),y);
		}


		public void AddValue(int x, int y)
		{
			datapoint myPoint;
			myPoint.x=x;
			myPoint.y=y;
			myPoint.valid=true;
			chartValues.Add(myPoint);
		}

		/// <summary>
		/// Perform chart generation
		/// </summary>
		public void Draw() 
		{
			int i;
			float x, y, x0, y0;
			string myLabel;
			Pen blackPen = new Pen(Color.Black,1);
			Pen grayPen = new Pen(Color.DarkGray,1);
			Brush blackBrush = new SolidBrush(Color.Black);
			Font axesFont = new Font("arial",10);

			//first establish working area
			p.Response.ContentType="image/png";
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillRectangle(new
				SolidBrush(Color.White),0,0,Width,Height);
			int ChartInset = 50;
			int ChartWidth = Width-(2*ChartInset);
			int ChartHeight = Height-(2*ChartInset);
			g.DrawRectangle(new
				Pen(Color.Black,1),ChartInset,ChartInset,ChartWidth,ChartHeight);

			//must draw all text items before doing the rotate below
			g.DrawString(Title, new Font("arial",10,FontStyle.Underline), Brushes.Black,
				10, 10);
			//draw X axis labels
			for(i=0; i<=Xdivs; i++) 
			{
				x=ChartInset+(i*ChartWidth)/Xdivs;
				y=ChartHeight+ChartInset;
				if (dataType == DataType.Integer)
				{
					myLabel = (Xorigin + (ScaleX*i/Xdivs)).ToString();
				}
				else
				{
					myLabel = dateMin.AddDays(Convert.ToInt32(Xorigin + (ScaleX*i/Xdivs))).ToShortDateString();
				}
				g.DrawString(myLabel, axesFont, Brushes.Black, x-5, y+10);
				g.DrawLine(Pens.Black, x, y+10, x, y-2);
			}
			//draw Y axis labels
			for(i=0; i<=Ydivs; i++) 
			{
				x=ChartInset;
				y=ChartHeight+ChartInset-(i*ChartHeight/Ydivs);
				myLabel = (Yorigin + (ScaleY*i/Ydivs)).ToString();
				g.DrawString(myLabel, axesFont, blackBrush, 5, y-6);
				g.DrawLine(blackPen, x+2, y, x-2, y);
			}

			//transform drawing coords to lower-left (0,0)
			g.RotateTransform(180);
			g.TranslateTransform(0,-Height);
			g.TranslateTransform(-ChartInset,ChartInset);
			g.ScaleTransform(-1, 1);

			//draw chart data
			datapoint prevPoint = new datapoint();
			prevPoint.valid=false;
			foreach(datapoint myPoint in chartValues) 
			{
				if(prevPoint.valid==true) 
				{
					x0=ChartWidth*(prevPoint.x-Xorigin)/ScaleX;
					y0=ChartHeight*(prevPoint.y-Yorigin)/ScaleY;
					x=ChartWidth*(myPoint.x-Xorigin)/ScaleX;
					y=ChartHeight*(myPoint.y-Yorigin)/ScaleY;
					g.DrawLine(Pens.BlueViolet,x0,y0,x,y);
					g.FillEllipse(Brushes.DarkBlue,x0-2,y0-2,4,4);
					g.FillEllipse(Brushes.DarkBlue,x-2,y-2,4,4);
				}
				prevPoint = myPoint;
			}
			//finally send graphics to browser
			MemoryStream ms = new MemoryStream();
			b.Save(ms, ImageFormat.Png);
			byte[] buffer = ms.ToArray();
			p.Response.OutputStream.Write(buffer,0,buffer.Length);
		}

		~LineChart() 
		{
			g.Dispose();
			b.Dispose();
		}
	}


}
