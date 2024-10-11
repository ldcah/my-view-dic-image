using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.KeyDown += MainWindow_KeyDown;
        }
        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            openImage();
        }
 
        private void openImage()
        {
          System.Windows.Forms.  OpenFileDialog opnDlg = new System.Windows.Forms.OpenFileDialog();
            opnDlg.Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;" +
                "*.tif;*.tiff; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
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

                if (isNg)
                {
                    //可以根据筛选条件来过滤图像
                    offLineTestImageNameList = System.IO.Directory.GetFiles(_imageDir, "*.*").
                    Where(
                    file => file.ToLower().EndsWith("NG.bmp")
                    || file.ToLower().EndsWith("NG.tif")
                    ).ToList();
                }
                else
                {
                    //可以根据筛选条件来过滤图像
                    offLineTestImageNameList = System.IO.Directory.GetFiles(_imageDir, "*.*").
                    Where(
                    file => file.ToLower().EndsWith(".bmp")
                    || file.ToLower().EndsWith(".tif") || file.ToLower().EndsWith(".jpg")
                    ).ToList();
                }
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

  
        private void loadTestImage(string imagePath)
        {
            if (ho_ModelImage != null && ho_ModelImage.IsInitialized())
            {
                ho_ModelImage.Dispose();
                ho_ModelImage = null;
            }

            if (!System.IO.File.Exists(imagePath))
                return;
            //读取图像
            HOperatorSet.ReadImage(out ho_ModelImage, imagePath);
            //显示图像
            myHWind.ViewController.clearList();
            myHWind.ViewController.addHoImage(ho_ModelImage);

            string name = imagePath.Substring(imagePath.LastIndexOf("\\") + 1);
            lblImageName.Content = name;
            //第几张
            label_SerialNumber.Content = string.Format("【{0}/{1}】", nowIndex + 1, imagesNum);
            nowImageName = imagePath;
        
            string dicPath = imagePath.Replace(".tif", ".hdict").Replace(".bmp", ".hdict").Replace(".jpg", ".hdict");           
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


        bool isNg = false;
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isNg = chkNG.IsChecked.GetValueOrDefault();
        }
    }
}
