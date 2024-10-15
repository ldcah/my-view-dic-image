using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Diagnostics;
using VM.Halcon.Model;
using VM.Halcon.Config;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

using VS.Main.Events;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

namespace VM.Halcon
{
    public enum MyAction
    {
        ImgLoad,
        ImgSaveAs,
        ImgLocation
    }
    public partial class VMHWindowControl : UserControl
    {
        #region 与其他项目交互信息
        public Action<string> actStr = null;
        public Action<MyAction> actClick = null;
        void doAction(MyAction action)
        {
            if (actClick != null)
                actClick(action);
        }
        void doActionStr(string _s)
        {
            if (actStr != null)
                actStr(_s);
        }
        ToolStripMenuItem imgLoacation_strip;
        ToolStripMenuItem imgSaveAs_strip;
        #endregion


        #region 私有变量定义.
        public HWindow /**/                 hv_window;                      //halcon窗体控件的句柄 this.mCtrl_HWindow.HalconWindow;
        private ContextMenuStrip /**/        hv_MenuStrip;                   //右键菜单控件
        // 窗体控件右键菜单内容
        ToolStripMenuItem fitWindow_strip;//适应窗口
        ToolStripMenuItem fitImage_strip;//适应图片
        ToolStripMenuItem saveImg_strip;//保存图像
        ToolStripMenuItem saveWindow_strip;//保存截图
        public ToolStripMenuItem barVisible_strip;//显示状态
        ToolStripMenuItem crossVisible_strip;//显示十字
        ToolStripMenuItem histogram_strip;
        ToolStripMenuItem openImage_strip;//打开图片
        private HImage  /**/                 hv_image;                      //缩放时操作的图片  此处千万不要使用hv_image = new HImage(),不然在生成控件dll的时候,会导致无法序列化,去你妈隔壁,还好老子有版本控制,不然都找不到这种恶心问题
        public int /**/                     hv_imageWidth, hv_imageHeight; //图片宽,高
        private string /**/                  str_imgSize;                   //图片尺寸大小 5120X3840
        private bool    /**/                 drawModel = false;             //绘制模式下,不允许缩放和鼠标右键菜单
        #endregion
        public ViewWindow ViewController;    /**/                    //ViewWindow
        public HWindowControl hControl;   /**/


        // 当前halcon窗口

        public int positionX, positionY;
        public bool BarVisible = true;
        //鼠标单击次数
        int i = 0;
        public static bool IsDesignMode()//是否处于设计模式判断
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
        /// <summary>
        /// 初始化控件
        /// </summary>
        public VMHWindowControl()
        {
            InitializeComponent();
            ViewController = new ViewWindow(mCtrl_HWindow);
            hControl = this.mCtrl_HWindow;
            ViewController._hWndControl.PaintCrossEvent += PaintCross;
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            HOperatorSet.SetSystem("clip_region", "false");
            hv_window = this.mCtrl_HWindow.HalconWindow;
            //设定鼠标按下时图标的形状
            //'arrow'  'default' 'crosshair' 'text I-beam' 'Slashed circle' 'Size All'
            //'Size NESW' 'Size S' 'Size NWSE' 'Size WE' 'Vertical Arrow' 'Hourglass'
            // hv_window.SetMshape("Hourglass");
            fitWindow_strip = new ToolStripMenuItem("适应窗口");
            fitWindow_strip.Click += new EventHandler((s, e) => DispImageFitWindow());
            fitImage_strip = new ToolStripMenuItem("适应图片");
            fitImage_strip.Click += new EventHandler((s, e) => DispImageFitImage());
            barVisible_strip = new ToolStripMenuItem("显示/隐藏图像信息");
            barVisible_strip.CheckOnClick = true;
            barVisible_strip.CheckedChanged += new EventHandler(barVisible_strip_CheckedChanged);
            crossVisible_strip = new ToolStripMenuItem("显示/隐藏十字");
            crossVisible_strip.CheckOnClick = true;
            crossVisible_strip.CheckedChanged += new EventHandler(crossVisible_strip_CheckedChanged);
            m_CtrlHStatusLabelCtrl.Visible = false;
            mCtrl_HWindow.Height = this.Height;
            saveImg_strip = new ToolStripMenuItem("保存原始图像");
            saveImg_strip.Click += new EventHandler((s, e) => SaveImage());
            saveWindow_strip = new ToolStripMenuItem("保存缩略图像");
            saveWindow_strip.Click += new EventHandler((s, e) => SaveWindowDump());

            histogram_strip = new ToolStripMenuItem("显示直方图(H)");
            histogram_strip.CheckOnClick = true;
            histogram_strip.Checked = false;

            openImage_strip = new ToolStripMenuItem("打开图片");
            openImage_strip.Click += new EventHandler((s, e) => doAction(MyAction.ImgLoad));


            imgLoacation_strip = new ToolStripMenuItem("打开文件所在的位置");
            imgLoacation_strip.Click += new EventHandler((s, e) => doAction(MyAction.ImgLocation));


            imgSaveAs_strip = new ToolStripMenuItem("图片另存为...");
            imgSaveAs_strip.Click += new EventHandler((s, e) => doAction(MyAction.ImgSaveAs));

            //openImage_strip.Click += new EventHandler((s, e) => OpenImage());


            hv_MenuStrip = new ContextMenuStrip();
            hv_MenuStrip.Items.Add(fitWindow_strip);
            hv_MenuStrip.Items.Add(fitImage_strip);
            hv_MenuStrip.Items.Add(new ToolStripSeparator());
            hv_MenuStrip.Items.Add(crossVisible_strip);
            hv_MenuStrip.Items.Add(barVisible_strip);
            hv_MenuStrip.Items.Add(new ToolStripSeparator());
            hv_MenuStrip.Items.Add(saveImg_strip);
            hv_MenuStrip.Items.Add(saveWindow_strip);

            hv_MenuStrip.Items.Add(new ToolStripSeparator());
            hv_MenuStrip.Items.Add(openImage_strip);

            hv_MenuStrip.Items.Add(imgSaveAs_strip);
            hv_MenuStrip.Items.Add(imgLoacation_strip);

            fitWindow_strip.Enabled = false;
            fitImage_strip.Enabled = false;
            crossVisible_strip.Enabled = false;
            barVisible_strip.Enabled = false;
            histogram_strip.Enabled = false;
            saveImg_strip.Enabled = false;
            saveWindow_strip.Enabled = false;


            mCtrl_HWindow.ContextMenuStrip = hv_MenuStrip;
            mCtrl_HWindow.SizeChanged += new EventHandler((s, e) => DispImageFitImage());
            mCtrl_HWindow.HMouseDown += MCtrl_HWindow_HMouseDown;
            this.SizeChanged += VMHWindowControl_SizeChanged;
        }





        private void VMHWindowControl_SizeChanged(object sender, EventArgs e)
        {
            int width = this.Width;
            float fontSize = (float)(width / 100 + 1.5);
            this.m_CtrlHStatusLabelCtrl.Font = new System.Drawing.Font("微软雅黑", fontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        private void MCtrl_HWindow_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                return;
            }
            i += 1;

            System.Timers.Timer timer = new System.Timers.Timer();

            timer.Interval = 300;

            timer.Elapsed += (s, e1) => { timer.Enabled = false; i = 0; };

            timer.Enabled = true;

            if (i % 2 == 0)
            {
                timer.Enabled = false;
                i = 0;
                DispImageFitImage();
            }
        }



        /// <summary>
        /// 绘制模式下,不允许缩放和鼠标右键菜单
        /// </summary>
        public bool DrawModel
        {
            get { return drawModel; }
            set
            {
                //缩放控制
                ViewController.setDrawModel(value);
                //绘制模式 不现实右键
                if (value == true)
                {
                    mCtrl_HWindow.ContextMenuStrip = null;
                }
                else
                {
                    //恢复
                    mCtrl_HWindow.ContextMenuStrip = hv_MenuStrip;
                }
                drawModel = value;
            }
        }
        /// <summary>
        /// 设置image,初始化控件参数
        /// </summary>
        public HImage Image
        {
            get { return this.hv_image; }
            set
            {
                if (value != null)
                {
                    try
                    {
                        if (this.hv_image != null)
                        {
                            this.hv_image.Dispose();
                        }
                        hv_image = value;
                        hv_image.GetImageSize(out hv_imageWidth, out hv_imageHeight);
                        str_imgSize = String.Format("{0}*{1}", hv_imageWidth, hv_imageHeight);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            DispImage();
                            ChangeEnable();
                        }));
                    }
                    else
                    {
                        DispImage();
                        ChangeEnable();
                    }
                }
            }
        }
        private void DispImage()
        {
            ViewController.displayImage(hv_image);
            PaintCross();
        }
        public void ChangeEnable()
        {
            fitWindow_strip.Enabled = true;
            fitImage_strip.Enabled = true;
            barVisible_strip.Enabled = true;
            crossVisible_strip.Enabled = true;
            histogram_strip.Enabled = true;
            saveImg_strip.Enabled = true;
            saveWindow_strip.Enabled = true;
            if (barVisible_strip.Checked == true)
            {
                BarVisible = true;
                barVisible_strip.Checked = false;
            }
            if (BarVisible)
            {
                BarVisible = false;
                barVisible_strip.Checked = true;
            }
        }
        /// <summary>
        /// 获得halcon窗体控件的句柄
        /// </summary>
        public IntPtr HWindowHalconID
        {
            get { return this.mCtrl_HWindow.HalconWindow; }
        }
        public HWindowControl getHWindowControl()
        {
            return this.mCtrl_HWindow;
        }
        public void ClearROI()
        {
            if (crossVisible_strip.Checked)
            {
                PaintCross();
            }
            else
            {
                ViewController.ClearWindow();
                if (Image != null && Image.IsInitialized())
                {
                    ViewController.displayImageWithoutFit(Image);
                }
            }
        }
        void crossVisible_strip_CheckedChanged(object sender, EventArgs e)
        {
            if (crossVisible_strip.Checked)
            {
                PaintCross();
            }
            else
            {
                ViewController._hWndControl.Repaint();
            }
        }
        private void PaintCross()
        {
            if (crossVisible_strip.Checked)
            {
                //显示十字线
                HXLDCont xldCross = new HXLDCont();
                mCtrl_HWindow.HalconWindow.SetColor("green");
                HRegion hRegion = new HRegion(0, 0, (HTuple)hv_imageHeight, (HTuple)hv_imageWidth);
                HOperatorSet.AreaCenter(hRegion, out HTuple _Area, out HTuple _ROW, out HTuple _COL);
                _ROW = hv_imageHeight / 2;
                _COL = hv_imageWidth / 2;
                //小十字
                mCtrl_HWindow.HalconWindow.DispLine(_ROW - 5, _COL, _ROW + 5, _COL);
                mCtrl_HWindow.HalconWindow.DispLine(_ROW, _COL - 5, _ROW, _COL + 5);
                //中心圆
                //mCtrl_HWindow.HalconWindow.DispCircle(_ROW, _COL, 35);
                //大十字-横
                mCtrl_HWindow.HalconWindow.DispLine((double)_ROW, (double)_COL + 50, (double)_ROW, (double)_COL * 2);
                mCtrl_HWindow.HalconWindow.DispLine((double)_ROW, 0, (double)_ROW, (double)_COL - 50);
                //大十字-竖
                mCtrl_HWindow.HalconWindow.DispLine(0, (double)_COL, (double)_ROW - 50, (double)_COL);
                mCtrl_HWindow.HalconWindow.DispLine((double)_ROW + 50, (double)_COL, (double)_ROW * 2, (double)_COL);
            }
        }
        /// <summary>
        /// 状态条 显示/隐藏 CheckedChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barVisible_strip_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem strip = sender as ToolStripMenuItem;
            this.SuspendLayout();
            if (strip.Checked)
            {
                m_CtrlHStatusLabelCtrl.Visible = true;
                //mCtrl_HWindow.Height = this.Height - m_CtrlHStatusLabelCtrl.Height - m_CtrlHStatusLabelCtrl.Margin.Top - m_CtrlHStatusLabelCtrl.Margin.Bottom;
                mCtrl_HWindow.HMouseMove += HWindowControl_HMouseMove;
            }
            else
            {
                m_CtrlHStatusLabelCtrl.Visible = false;
                //mCtrl_HWindow.Height = this.Height;
                mCtrl_HWindow.HMouseMove -= HWindowControl_HMouseMove;
            }
            //DispImageFit(mCtrl_HWindow);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        public void showStatusBar()
        {
            barVisible_strip.Checked = true;
        }
        /// <summary>
        /// 保存窗体截图到本地
        /// </summary>
        private void SaveWindowDump()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "|JPG图像|*.jpg|tif图像|*.tif|BMP图像|*.bmp|PNG图像|*.png";//|所有文件|*.*
            sfd.FilterIndex = 1;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(sfd.FileName)) { return; }
                HOperatorSet.DumpWindow(hv_window, Path.GetExtension(sfd.FileName).Replace(".", "").Replace("tif", "tiff"), sfd.FileName);//截取窗口图 
            }
        }
        /// <summary>
        /// 保存原始图片到本地
        /// </summary>
        private void SaveImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "tif图像|*.tif|BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg";//|所有文件|*.*
            sfd.FilterIndex = 1;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(sfd.FileName)) { return; }
                FileInfo _FileInfo = new FileInfo(sfd.FileName);
                HOperatorSet.WriteImage(hv_image, Path.GetExtension(sfd.FileName).Replace(".", "").Replace("tif", "tiff"), 0, sfd.FileName);
            }
        }
        /// <summary>
        /// 打开图片
        /// </summary>
        public void OpenImage()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    HTuple ImagePath = openFileDialog.FileName;
                    HImage image = new HImage();
                    image.ReadImage(ImagePath);
                    ViewController.fitImage_InitFlag = true;
                    this.HobjectToHimage(image);
                }
            }
            catch (HalconException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 图片适应大小显示在窗体
        /// </summary>
        /// <param name="hw_Ctrl">halcon窗体控件</param>
        public void DispImageFitImage()
        {
            try
            {
                this.ViewController.ResetWindowImage(true);
                PaintCross();
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 图片适应大小显示在窗体
        /// </summary>
        /// <param name="hw_Ctrl">halcon窗体控件</param>
        public void DispImageFitWindow()
        {
            try
            {
                this.ViewController.ResetWindowImage(false);
                PaintCross();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 鼠标在空间窗体里滑动,显示鼠标所在位置的灰度值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (hv_image != null && hv_image.IsInitialized() && barVisible_strip.Checked)
            {
                try
                {
                    string str_value = string.Empty;
                    string zoomRatioStr = string.Empty;
                    string str_position = string.Empty;

                    hv_window.GetMposition(out positionY, out positionX, out int button_state);


                    bool _isXOut = (positionX < 0 || positionX >= hv_imageWidth);
                    bool _isYOut = (positionY < 0 || positionY >= hv_imageHeight);
                    if (!_isXOut && !_isYOut)
                    {
                        HTuple grayVal = hv_image.GetGrayval((int)positionY, (int)positionX);

                        //缩放率
                        //double zoomScale = 1 /ViewController._hWndControl.ZoomWndFactor;
                        //string zoomScaleStr1 = zoomScale.ToString("f4");
                        //double temp = Convert.ToDouble(zoomScaleStr1) * 100;
                        //string zoomScaleStr2 = " 缩放率:" + temp.ToString("f2") + "%";


                        str_value = string.Format("Gv: {0}",
                            string.Join(",", from v in grayVal.ToSArr()
                                             select v.ToString().PadLeft(3, '0')));
                        str_position = string.Format("Rc: {0}, {1}", positionY, positionX);

                        doActionStr(string.Format("[ {0} ]\t[ {1} ]\t[ {2} ]", str_imgSize, str_value, str_position));
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public void ClearWindow()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    //this.hv_image = null;
                    m_CtrlHStatusLabelCtrl.Visible = false;
                    barVisible_strip.Enabled = false;
                    fitWindow_strip.Enabled = false;
                    //histogram_strip.Enabled = false;
                    saveImg_strip.Enabled = false;
                    saveWindow_strip.Enabled = false;
                    mCtrl_HWindow.HalconWindow.ClearWindow();
                    ViewController.ClearWindow();
                    PaintCross();
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void HobjectToHimage(HObject hobject)
        {
            if (hobject == Image)
            {
                return;
            }
            if (hobject == null || !hobject.IsInitialized())
            {
                ClearWindow();
                return;
            }
            this.Image = new HImage(hobject);
        }

        #region 缩放后,再次显示传入的HObject
        /// <summary>
        /// 默认红颜色显示
        /// </summary>
        /// <param name="hObj">传入的region.xld,image</param>
        public void DispObj(HObject hObj, bool isFillDis)
        {
            lock (this)
            {
                ViewController.DispHobject(hObj, null, isFillDis);
            }
        }
        public void DispObj(HObject hObj)
        {
            lock (this)
            {
                ViewController.DispHobject(hObj, null, false);
            }
        }
        /// <summary>
        /// 重新开辟内存保存 防止被传入的HObject在其他地方dispose后,不能重现
        /// </summary>
        /// <param name="hObj">传入的region.xld,image</param>
        /// <param name="color">颜色</param>
        public void DispObj(HObject hObj, string color, bool isFillDis)
        {
            lock (this)
            {
                ViewController.DispHobject(hObj, color, isFillDis);
            }
        }
        public void DispObj(HObject hObj, string color)
        {
            lock (this)
            {
                ViewController.DispHobject(hObj, color, false);
            }
        }
        private void mCtrl_HWindow_Click(object sender, EventArgs e)
        {
            //int button_state;
            //hv_window.GetMpositionSubPix(out PositionY, out PositionX, out button_state);
        }
        #endregion
        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCtrl_HWindow_MouseLeave(object sender, EventArgs e)
        {
            //避免鼠标离开窗口,再返回的时候,图表随着鼠标移动
            ViewController.mouseleave();
        }
        /// <summary>
        /// 显示he文件
        /// </summary>
        /// <param name="he"></param>
        public void ShowHImage(RImage hobject)
        {
            try
            {
                if (hobject == null) return;
                HobjectToHimage(hobject);
                foreach (HRoi roi in hobject.mHRoi)
                {
                    if (roi != null)
                    {
                        lock (this)
                        {
                            ViewController.DispHobject(roi.hobject, roi.drawColor, false);
                        }
                    }
                    else
                    {
                        if (roi != null && roi.hobject.IsInitialized())
                        {
                            DispObj(roi.hobject, roi.drawColor, false);
                        }
                    }
                }
                foreach (HText roi in hobject.mHText)
                {
                    if (roi != null && roi.roiType == HRoiType.文字显示)
                    {
                        lock (this)
                        {
                            ViewController.DispText(roi);
                        }
                    }
                    else
                    {
                        if (roi != null && roi.hobject.IsInitialized())
                        {
                            DispObj(roi.hobject, roi.drawColor, false);
                        }
                    }
                }
            }
            catch { }
        }






        #region 涂抹部分
        /// <summary>灰度值，坐标位置</summary>
        public string message;
        ///<summary>喷涂区域</summary>
        public HRegion BrushRegion;
        ///<summary>掩膜区域</summary>
        public HRegion MaskRegion;
        private int StateView;
        public List<HObjectEntry> HObjList;
        #endregion
        /// <summary>
        /// 擦除区域
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="zoomWndFactor"></param>
        public HRegion Eraser(double Row, double Column, double zoomWndFactor)
        {
            BrushRegion = new HRegion();
            HRegion tmpDiff = new HRegion();
            tmpDiff.GenEmptyRegion();
            if (10 * zoomWndFactor < 1)
            {
                BrushRegion.GenCircle(Row, Column, 0.5);
            }
            else
            {
                BrushRegion.GenCircle(Row, Column, 10 * zoomWndFactor);
            }
            if (Row >= 0 && Column >= 0)
            {
                if (MaskRegion == null)
                {
                    MaskRegion = new HRegion();
                    MaskRegion = tmpDiff.Difference(BrushRegion);
                }

                else
                    MaskRegion = MaskRegion.Difference(BrushRegion);
                return BrushRegion;
            }
            else
            {
                return BrushRegion;
            }
        }
        /// <summary>
        ///  喷涂区域
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="zoomWndFactor"></param>
        public HRegion Paint(double Row, double Column, double zoomWndFactor)
        {
            BrushRegion = new HRegion();
            HRegion tmpAdd = new HRegion();
            tmpAdd.GenEmptyRegion();
            if (10 * zoomWndFactor < 1)
            {
                BrushRegion.GenCircle(Row, Column, 0.5);
            }
            else
            {
                BrushRegion.GenCircle(Row, Column, 10 * zoomWndFactor);
            }
            if (Row >= 0 && Column >= 0)
            {
                if (MaskRegion == null)
                {
                    MaskRegion = new HRegion();
                    MaskRegion = tmpAdd.Union2(BrushRegion);
                }

                else
                    MaskRegion = MaskRegion.Union2(BrushRegion);
                return BrushRegion;
            }
            else
            {
                return BrushRegion;
            }
        }


        //是否涂抹
        private bool blnDraw = false;
        private HRegion brushRegion = new HRegion();

        private void mCtrl_HWindow_Load(object sender, EventArgs e)
        {
            HImage imageBackground = new HImage("Background.bmp");
            this.HobjectToHimage(imageBackground);
        }

        // 获取图像的当前显示部分
        private int current_beginRow, current_beginCol, current_endRow, current_endCol;
        /// <summary>
        /// 绘制任意屏蔽区域
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public HRegion SetROI(HRegion region, int drawMode)
        {
            hv_window.SetDraw("fill");//margin
            this.mCtrl_HWindow.Focus();//必须先聚焦
            try
            {
                mCtrl_HWindow.HMouseMove -= HWindowControl_HMouseMove;
                blnDraw = true;
                double Row = 0, Column = 0;
                #region "循环,等待涂抹"
                brushRegion.GenRectangle1(0.0, 0, 3, 3);
                //鼠标状态
                int hv_Button = 0;
                // 4为鼠标右键
                while (hv_Button != 4)
                {
                    //一直在循环,需要让halcon控件也响应事件,不然到时候跳出循环,之前的事件会一起爆发触发,
                    System.Windows.Forms.Application.DoEvents();
                    //获取鼠标坐标
                    try
                    {
                        hv_window.GetMpositionSubPix(out Row, out Column, out hv_Button);
                        hv_window.GetPart(out current_beginRow, out current_beginCol, out current_endRow, out current_endCol);
                        double d = Math.Sqrt(Math.Pow(current_endRow - current_beginRow, 2) + Math.Pow(current_endCol - current_beginCol, 2));
                        //brushRegion.GenCircle(Row, Column, d / 50.0);
                        brushRegion.GenRectangle1(Row - 2, Column - 2, Row + 2, Column + 2);
                    }
                    catch (HalconException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        hv_Button = 0;
                    }
                    //check if mouse cursor is over window
                    if (Row >= 0 && Column >= 0)
                    {
                        //1为鼠标左键
                        if (hv_Button == 1)
                        {
                            //画出笔刷
                            switch (drawMode)
                            {
                                case 0:
                                    {
                                        if (region != null && region.IsInitialized())
                                            region = region.Union2(brushRegion);
                                        else
                                            region = brushRegion;
                                    }
                                    break;
                                case 1:
                                    region = region.Difference(brushRegion);
                                    break;
                                default:
                                    MessageBox.Show("设置错误");
                                    break;
                            }//end switch
                        }//end if
                    }
                    HOperatorSet.SetSystem("flush_graphic", "false");//magical 20171028 防止画面闪烁
                    hv_window.DispObj(hv_image);
                    //hv_window.SetLineWidth(1.2);
                    hv_window.SetColor("magenta");//这一段也必须放在中间  
                    //hv_window.SetRgba(255, 0, 0, 120);
                    hv_window.SetColor("orange");
                    if (region != null)
                        hv_window.DispObj(region);
                    HOperatorSet.SetSystem("flush_graphic", "true");//很奇怪必须放在这里,不能把下面的给包含进去 magical20171028

                    double a = 1; double b = 50;
                    //brushRegion.FillUpShape("red", a, b);
                    if (drawMode == 0)
                    {
                        hv_window.SetColor("red");
                    }
                    else
                        hv_window.SetColor("green");
                    if (brushRegion != null)
                        hv_window.DispObj(brushRegion);
                    hv_window.SetColor("#00ff00a0");
                    //hv_window.SetRgba(255, 0, 0, 120);
                }//end while
                #endregion
                blnDraw = false;
                mCtrl_HWindow.HMouseMove += HWindowControl_HMouseMove;
                return region;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }




        public void loadOutDic(string dicPath)
        {

            if (System.IO.File.Exists(dicPath))
            {
                HTuple outDic;
                HOperatorSet.ReadDict(dicPath, new HTuple(), new HTuple(), out outDic);

                addDic(outDic);
                // ViewController.repaint();
                if (outDic != null)
                    outDic.Dispose();
            }
        }

        public void addDic(HTuple dic)
        {
            if (dic == null)
                return;
            HTuple IsHandle, hv_exist;
            HOperatorSet.TupleIsHandle(dic, out IsHandle);
            if (IsHandle == 1)
            {
                string key = "ysMarkImage";
                addRegionColor(dic, key);
                key = "ysTempObj";
                addRegionColor(dic, key);

                //显示字
                key = "ysTempMsgIndex";
                testDicKey(dic, key, out hv_exist);
                if (hv_exist == 1)
                {
                    HTuple num, msg;
                    HOperatorSet.GetDictTuple(dic, key, out num);
                    if (num > 0)
                    {
                        for (int i = 1; i <= num; i++)
                        {
                            key = "ysTempMsgStr_" + i;
                            HOperatorSet.GetDictTuple(dic, key, out msg);
                            showMsg(msg);
                        }
                    }
                }

            }
        }



        private void addRegionColor(HTuple dic, string key)
        {
            string keyNum = key + "Index";
            string keyRegion = key + "Region_";
            string keyColor = key + "Color_";

            string keyDraw = key + "Draw_";

            HTuple hv_exist;
            testDicKey(dic, keyNum, out hv_exist);
            if (hv_exist == 1)
            {
                HObject hoRegion;
                HTuple color, num, _draw;
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
            return;
        }

        void testDicKey(HTuple hv__dic, HTuple hv__key, out HTuple hv__exist)
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
                    System.Console.WriteLine(ex.Message);
                }
            }

        }
    }




}

