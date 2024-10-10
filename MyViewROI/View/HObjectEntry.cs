using System;
using HalconDotNet;
using System.Collections;



namespace MyViewROI
{

    /// <summary>
    /// This class is an auxiliary class, which is used to 
    /// link a graphical context to an HALCON object. The graphical 
    /// context is described by a hashtable, which contains a list of
    /// graphical modes (e.g GC_COLOR, GC_LINEWIDTH and GC_PAINT) 
    /// and their corresponding values (e.g "blue", "4", "3D-plot"). These
    /// graphical states are applied to the window before displaying the
    /// object.
    /// </summary>
    public class HObjectEntry
	{
		/// <summary>Hashlist defining the graphical context for HObj</summary>
		public Hashtable	gContext;

		/// <summary>HALCON object</summary>
		public HObject		HObj;


		public string SetColor = "red";
		public string SetDraw = "margin";

		public string ObjType = "Region";



		/// <summary>Constructor</summary>
		/// <param name="obj">
		/// HALCON object that is linked to the graphical context gc. 
		/// </param>
		/// <param name="gc"> 
		/// Hashlist of graphical states that are applied before the object
		/// is displayed. 
		/// </param>
		public HObjectEntry(HObject obj, Hashtable gc,string setColor="red",string setDraw= "margin",string objType= "Region")
		{
			gContext = gc;
			HObj = obj;

			SetColor = setColor;
			SetDraw = setDraw;

			ObjType = objType;
		}

		/// <summary>
		/// Clears the entries of the class members Hobj and gContext
		/// </summary>
		public void clear()
		{
			gContext.Clear();
			HObj.Dispose();
		}

	}//end of class


	public class Txt
    {
		public Hashtable gContext;

		public HTuple SetColor = "red";

		public HTuple Row;
		public HTuple Col;

		public HTuple MSG = "";
		public HTuple Font = "ËÎÌו-16";

		public Txt(HTuple msg, Hashtable gc, HTuple row, HTuple col, string setColor = "red" ,string font= "ËÎÌו-16")
		{
			gContext = gc;
			MSG = msg;
			SetColor = setColor;
			Row = row;
			Col = col;
			Font = font;
		}
	}

}//end of namespace
