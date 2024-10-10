using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;


namespace MyViewROI
{
    public partial class MyHalconWindow : UserControl
    {
        public HWndCtrl ViewController;


        public HWindow HwId { get { return this.hWindowControl1.HalconWindow; } }

        /// <summary>
        /// 2021-10-24 ROI 容器
        /// </summary>
        public ROIController RoiController1 = new ROIController();
      

       
        public MyHalconWindow(bool showStatus = true)
        {
            InitializeComponent();
            ViewController = new HWndCtrl(hWindowControl1);
            ViewController.setViewState(HWndCtrl.MODE_VIEW_ZOOM_Wheel);
            NoActionToolStripMenuItem.Checked = !NoActionToolStripMenuItem.Checked;
            if (!IsDesignMode())
                hWindowControl1.HMouseMove += GetImgMessage;

            if (!showStatus)
            {
                statusStrip1.Visible = false;
            }

           

          

            //2021-10-24
            ViewController.useROIController(RoiController1);


            toolStripStatusLabel4ROI.Text = "";
            RoiController1.actTip += ROIStatus;
        }



        void ROIStatus(string tip)
        {
            toolStripStatusLabel4ROI.Text = tip;
        }

        private void NoActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
            NoActionToolStripMenuItem.Checked = !NoActionToolStripMenuItem.Checked;
            MoveAndZoomToolStripMenuItem.Checked = false;
            MoveToolStripMenuItem.Checked = false;
        }

        private void MoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            MoveToolStripMenuItem.Checked = !MoveToolStripMenuItem.Checked;
            NoActionToolStripMenuItem.Checked = false;
            MoveAndZoomToolStripMenuItem.Checked = false;
        }

        private void ZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.setViewState(HWndCtrl.MODE_VIEW_ZOOM_Wheel);
            MoveAndZoomToolStripMenuItem.Checked = !MoveAndZoomToolStripMenuItem.Checked;
            NoActionToolStripMenuItem.Checked = false;
            MoveToolStripMenuItem.Checked = false;
        }
        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsDesignMode())
                ViewController.resetAll();
        }
        private void hWindowControl1_SizeChanged(object sender, EventArgs e)
        {
            if (!IsDesignMode())
                ViewController.resetImage();
        }
        public static bool IsDesignMode()
        {
            bool returnFlag = false;

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
#endif

            return returnFlag;
        }

        void GetImgMessage(object sender, HalconDotNet.HMouseEventArgs e)
        {


            if (ViewController.Image != null)
            {
                try
                {
                    HTuple row, col, grayValue, imageWidth, imageHeight;
                   
                    row = e.Y;
                    col = e.X;
                    HOperatorSet.GetImageSize(ViewController.Image, out imageWidth, out imageHeight);
                    if (col > 0 && row > 0 && col < imageWidth && row < imageHeight)
                    {
                        HOperatorSet.GetGrayval(ViewController.Image, row, col, out grayValue);
                    }
                    else
                    {
                        grayValue = 0;
                    }

                    toolStripStatusLabel1.Text = string.Format("GV : {0}    Corrdinate : [ {1} , {2} ]", grayValue.ToString(), row.D.ToString("0"), col.D.ToString("0"));

                  
                }
                catch (Exception ex)
                {
                 
                }
            }
            else
            {
                toolStripStatusLabel1.Text = null;               
            }



        }



     

       public  void DispLocationHObject2XY(double x, double y, string des="")
        {

            if (ViewController.zoomWndFactor > 1)
            {
                ViewController.zoomByGUIHandle(400);
            }

            ViewController.DispLocationHObject2XY(x, y, des);


        }



        public void DispImage(HObject hObject = null)
        {
            ViewController.clearList();          
            ViewController.addHoImage(hObject);
        }


        /// <summary>
        /// msg:=[100,200,'yellow','宋体-16','尺寸数据']
        /// </summary>
        /// <param name="msg"></param>
        public void showMsg(HTuple msg)
        {
                if (msg.Length == 5)
                {
                    try
                    {
                        ViewController.addtxt(msg[4], msg[0], msg[1], msg[2], msg[3]);
                    }
                    catch (Exception ex)
                    {

                    }
                }
           
        }

        public void showMsgStr(string msg, int row = 0, int col = 0, string font = "宋体-16", string color = "green")
        {
            ViewController.addtxt(msg, row, col, color, font);
            ViewController.repaint();
        }


       


        public void loadRegions(string regionPath="C:\\ystemp")
        {
            try
            {
                //获取区域           
                var regions = new System.IO.DirectoryInfo(regionPath).GetFiles("*.hobj");
                string color = "";
                if (regions != null && regions.Length > 0)
                {
                    foreach (var v in regions)
                    {
                        color = v.Name.Substring(0, v.Name.IndexOf("-"));                       
                        try
                        {
                            HObject ho;
                            HOperatorSet.ReadObject(out ho, v.FullName);
                            if (ho != null && ho.IsInitialized())
                            {
                                ViewController.addIconicVar(ho, color);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
                //获取字符
                var ieMsg = new System.IO.DirectoryInfo(regionPath).GetFiles("*-msg.tup");
                HTuple msg;
                foreach (var t in ieMsg)
                {
                    HOperatorSet.ReadTuple(t.FullName, out msg);

                    if (msg.Length == 5)
                    {
                        try
                        {
                            ViewController.addtxt(msg[4], msg[0], msg[1], msg[2], msg[3]);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }



        public void addRegions(HObject hoRegion, string color="red")
        {
            if (hoRegion != null && hoRegion.IsInitialized())
            {
                ViewController.addIconicVar(hoRegion, color);
            }
        }


        public void clearTempRegionMsgWithOutImage()
        {
            ViewController.clearTempRegionMsgWithOutImage();
        }

        public void loadOutDic(string dicPath)
        {

            if (System.IO.File.Exists(dicPath))
            {
                HTuple outDic;
                HOperatorSet.ReadDict(dicPath, new HTuple(), new HTuple(), out outDic);

                addDic(outDic);

                ViewController.repaint();
                if (outDic != null)
                    outDic.Dispose();
            }
        }

        public void addDic(HTuple dic)
        {
            if (dic == null)
                return;
            HTuple IsHandle, hv_exist;
            HOperatorSet.TupleIsHandle(dic,out IsHandle);
            if(IsHandle==1)
            {
                string key = "ysMarkImage";
                addRegionColor(dic, key);
                key = "ysTempObj";
                addRegionColor(dic, key);

                //显示字
                key = "ysTempMsgIndex";
                testDicKey(dic, key, out hv_exist);
                if(hv_exist==1)
                {
                    HTuple  num,msg;
                    HOperatorSet.GetDictTuple(dic, key, out num);
                    if (num > 0)
                    {
                        for (int i = 1; i <= num; i++)
                        {
                            key = "ysTempMsgStr_"+i;
                            HOperatorSet.GetDictTuple(dic, key, out msg);
                            showMsg(msg);
                        }
                    }
                }

            }

         //  ViewController. repaint();
           
        }

        public void repaint()
        {
            ViewController.repaint();
        }

        private void addRegionColor(HTuple dic, string key)
        {
            string keyNum = key+"Index";
            string keyRegion = key+"Region_";
            string keyColor = key+"Color_";

            string keyDraw = key + "Draw_";

            HTuple hv_exist;
            testDicKey(dic, keyNum, out hv_exist);
            if (hv_exist == 1)
            {
                HObject hoRegion;
                HTuple color, num,_draw;
                HOperatorSet.GetDictTuple(dic, keyNum, out num);
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        HOperatorSet.GetDictTuple(dic, keyColor + i, out color);
                        HOperatorSet.GetDictTuple(dic, keyDraw + i, out _draw);
                        HOperatorSet.GetDictObject(out hoRegion, dic, keyRegion + i);

                        if (hoRegion != null && hoRegion.IsInitialized())
                        {
                            ViewController.addIconicVar(hoRegion, color.S, _draw.S);
                        }
                    }

                }
            }
            return ;
        }

void testDicKey (HTuple hv__dic, HTuple hv__key, out HTuple hv__exist)
  {



    // Local iconic variables 

    // Local control variables 

    HTuple hv_GenParamValue = new HTuple(), hv_Indices = new HTuple();
    HTuple hv_Exception = new HTuple();
    // Initialize local and output iconic variables 
    hv__exist = new HTuple();
    try
    {
      hv__exist.Dispose();
      hv__exist = 0;
      try
      {
        hv_GenParamValue.Dispose();
        HOperatorSet.GetDictParam(hv__dic, "keys", new HTuple(), out hv_GenParamValue);
        hv_Indices.Dispose();
        HOperatorSet.TupleFind(hv_GenParamValue, hv__key, out hv_Indices);
        if ((int)(new HTuple(hv_Indices.TupleGreaterEqual(0))) != 0)
        {
          hv__exist.Dispose();
          hv__exist = 1;
        }
      }
      // catch (Exception) 
      catch (HalconException HDevExpDefaultException1)
      {
        HDevExpDefaultException1.ToHTuple(out hv_Exception);
        hv__exist.Dispose();
        hv__exist = 0;
      }

      hv_GenParamValue.Dispose();
      hv_Indices.Dispose();
      hv_Exception.Dispose();

      return;
    }
    catch (HalconException HDevExpDefaultException)
    {

      hv_GenParamValue.Dispose();
      hv_Indices.Dispose();
      hv_Exception.Dispose();

      throw HDevExpDefaultException;
    }
  }


    }
}
