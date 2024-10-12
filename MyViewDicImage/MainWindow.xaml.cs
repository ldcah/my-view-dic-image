using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MyViewDicImage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HObject ho_ModelImage;
        MyViewROI.MyHalconWindow myHWind = new MyViewROI.MyHalconWindow();
        string _loadImgDir = "";


        /// <summary>
        /// 启动参数信息
        /// </summary>
        /// <param name="args"></param>
        public void OnStartup(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                //获取图像与区域信息              
                loadImage(args[0]);
            }

        }

        public MainWindow()
        {
            InitializeComponent();
            windowsFormsHost1.Child = myHWind; 

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += MainWindow_KeyDown;
            this.RectHeader.MouseDown += RectHeader_MouseDown;
            this.RectHeader.MouseLeftButtonDown += Rectangle_MouseRightButtonDown;


            this.MenuShutdown.Click += MenuShutdown_Click;
            this.MenuMaximized.Click += MenuMaximized_Click;
            this.MenuMinimized.Click += MenuMinimized_Click;

            myHWind.actStr += showGray;

            moniDoubleClick();
        }
        void showGray(string str)
        {
            lblGray.Content = str;
        }

        private void MenuShutdown_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void MenuMinimized_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MenuMaximized_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;

                MenuMaximized.ToolTip = "向下还原";
            }
            else
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
                MenuMaximized.ToolTip = "最大化";
            }
        }

    
        private void MenuMaximized_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;

                MenuMaximized.ToolTip = "向下还原";
            }
            else
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
                MenuMaximized.ToolTip = "最大化";
            }
        }

        private void MenuShutdown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void RectHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            }
        }
        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            openImage();
        }
 
        private void openImage()
        {
          System.Windows.Forms.  OpenFileDialog opnDlg = new System.Windows.Forms.OpenFileDialog();
            opnDlg.Filter = "所有图像文件 | *.bmp; *.png; *.jpg; *.tif";
            opnDlg.Title = "打开图像文件";
            opnDlg.ShowHelp = false;
            opnDlg.Multiselect = false;
            if (opnDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadImage(opnDlg.FileName);
            }
        }

        private void loadImage(string nowSelImagePath)
        {
           
            _loadImgDir = nowSelImagePath.Substring(0, nowSelImagePath.LastIndexOf("\\"));
            openImagePath(_loadImgDir, nowSelImagePath);
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    doPreImage();
                    break;

                case Key.D:
                    doNetImage();
                    break;
            }

        }
        void doPreImage()
        {
            btnPre_Click(null,null);
        }

        void doNetImage()
        {
            btnNet_Click(null, null);
        }
        int imagesNum = 0;
        int nowIndex = 0;
        string nowImageName = "";
        List<string> offLineTestImageNameList = new List<string>();
        private void btn_OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            openImagePath(m_Dialog.SelectedPath.Trim());
        }

        private void openImagePath(string _imageDir,string img="")
        {
            imagesNum = 0;
            nowIndex = -1;
            nowImageName = "";            
            if (System.IO.Directory.Exists(_imageDir))
            {
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(_imageDir);
                System.IO.FileInfo[] fileInfo = dirInfo.GetFiles();
                if (offLineTestImageNameList != null)
                    offLineTestImageNameList.Clear();

               
                    //可以根据筛选条件来过滤图像
                    offLineTestImageNameList = System.IO.Directory.GetFiles(_imageDir, "*.*").
                    Where(
                    file => file.ToLower().EndsWith(".bmp")
                    || file.ToLower().EndsWith(".tif") 
                    || file.ToLower().EndsWith(".jpg") 
                    || file.ToLower().EndsWith(".png")
                    ).ToList();
              
            }

            if (offLineTestImageNameList != null)
            {
                imagesNum = offLineTestImageNameList.Count();
            }
            if (img != "" && offLineTestImageNameList.Contains(img))
            {
                //获取
                nowIndex = offLineTestImageNameList.FindIndex(v => v == img);

                loadTestImage(img);
            }
            else
            {
                label_SerialNumber.Content = string.Format("【0/{0}】",  imagesNum);
            }

           
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (imagesNum > 0)
            {
                nowIndex = 0;
                if (nowIndex < imagesNum && nowIndex >= 0)
                    loadTestImage(offLineTestImageNameList[0]);
            }
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            if (nowIndex > 0)
                nowIndex--;
            if (nowIndex < imagesNum && nowIndex >= 0)
                loadTestImage(offLineTestImageNameList[nowIndex]);

        }

        private void btnNet_Click(object sender, RoutedEventArgs e)
        {
            if (nowIndex < imagesNum - 1)
                nowIndex++;

            if (nowIndex < imagesNum && imagesNum > 0)
                loadTestImage(offLineTestImageNameList[nowIndex]);

        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {

            nowIndex = imagesNum - 1;
            if (nowIndex < imagesNum && nowIndex > 0)
                loadTestImage(offLineTestImageNameList[nowIndex]);

        }


        string imgShortName = "";
        string imageSrcPath = "";

        private void loadTestImage(string imagePath)
        {
            if (ho_ModelImage != null && ho_ModelImage.IsInitialized())
            {
                ho_ModelImage.Dispose();
                ho_ModelImage = null;
            }

            if (!System.IO.File.Exists(imagePath))
                return;

             imgShortName = imagePath.Substring(imagePath.LastIndexOf("\\") + 1);
            imageSrcPath = imagePath;

            //读取图像
            HOperatorSet.ReadImage(out ho_ModelImage, imagePath);
            //显示图像
            myHWind.ViewController.clearList();
            myHWind.ViewController.addHoImage(ho_ModelImage);

            nowImageName = imagePath;

            lblImageName.Content = imgShortName;

            //第几张
            label_SerialNumber.Content = string.Format("【{0}/{1}】", nowIndex + 1, imagesNum);
          


        
            string dicPath = imagePath.Replace(".tif", ".hdict").Replace(".bmp", ".hdict").Replace(".jpg", ".hdict").Replace(".png", ".hdict");           
            if(System.IO.File.Exists(dicPath))
                myHWind.loadOutDic(dicPath);

        }
   
        private void btnOpenDir_Click(object sender, RoutedEventArgs e)
        {
            Open(_loadImgDir);
        }

        public static string Open(string path)
        {
            path = path.TrimEnd('\\');
            if (System.IO.Directory.Exists(path))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", path);
                    return "";
                }
                catch (Exception ex)
                {
                    return "打开" + path + "异常:" + ex.Message;
                }
            }
            else
            {
                return path + "目录不存在！";
            }
        }

        private void miClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(imageSrcPath))
                return;
            System.Windows.Forms.SaveFileDialog m_Dialog = new System.Windows.Forms.SaveFileDialog();
            m_Dialog.FileName = imgShortName;
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            if (imageSrcPath.Equals(m_Dialog.FileName, StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                System.IO.File.Copy(imageSrcPath, m_Dialog.FileName);
            }
            catch(Exception ex)
            {

            }

           
        }



        private int clickCount = 0;
        private DispatcherTimer doubleClickTimer;
        private void moniDoubleClick()
        {
            doubleClickTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // 双击时间间隔
            };
            doubleClickTimer.Tick += DoubleClickTimer_Tick;
        }

        private void DoubleClickTimer_Tick(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
            clickCount = 0;
        }
        private void Rectangle_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickCount++;
            if (clickCount == 1)
            {
                doubleClickTimer.Start();
            }
            else if (clickCount == 2)
            {
                doubleClickTimer.Stop();
                clickCount = 0;

                _doRectangleDoubleClick(sender);
            }
        }

        private void _doRectangleDoubleClick(object sender)
        {
            MenuMaximized_MouseLeftButtonUp(null,null);
        }
    }
}
