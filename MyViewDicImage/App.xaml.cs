using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyViewDicImage
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            MainWindow m = new MainWindow();
            m.Show();
            if (e != null && e.Args != null && e.Args.Length == 1)
            {

                m.OnStartup(e.Args[0].Split('*'));
            }

            base.OnStartup(e);
        }
        public App()
        {
            //首先注册开始和退出事件
            this.Startup += new StartupEventHandler(App_Startup);
            this.Exit += new ExitEventHandler(App_Exit);
        }

        //启动事件对应的方法：
        void App_Startup(object sender, StartupEventArgs e)
        {
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        /// <summary>
        /// 退出事件对应的方法：
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void App_Exit(object sender, ExitEventArgs e)
        {
            //程序退出时需要处理的业务
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errmsg = string.Format("{0}\tUI捕获未处理异常: \t{1}\t\r\n{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Exception.Message, (e.Exception).StackTrace);
            //try
            //{
            //    e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出      
            //    MessageBox.Show(errmsg);
            //}
            //catch (Exception ex)
            //{               
            //    MessageBox.Show(errmsg);
            //}

            SaveLog(errmsg);
            //记录异常信息
            // MessageBox.Show(errmsg);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Text.StringBuilder sbEx = new System.Text.StringBuilder();
            if (e.IsTerminating)
            {
                //sbEx.Append("程序发生致命错误，将终止，请联系运营商！\n");
            }
            sbEx.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sbEx.Append("\t非UI捕获未处理异常:\t");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).Message);
                sbEx.Append("\t\r\n");
                sbEx.Append(((Exception)e.ExceptionObject).StackTrace);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }

            SaveLog(sbEx.ToString());
            //记录异常信息
            //MessageBox.Show(sbEx.ToString());
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string errmsg = string.Format("{0}\tTask捕获未处理异常: \t{1}\t\r\n{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Exception.Message, (e.Exception).StackTrace);

            SaveLog(errmsg);
            //task线程内未处理捕获
            //MessageBox.Show(errmsg);

            //e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
        }


        /// <summary>
        /// 保存日志信息
        /// </summary>
        /// <param name="error"></param>
        void SaveLog(string error)
        {
            string path = string.Format("d:\\ys4log\\{0}", DateTime.Now.ToString("yyyy-MM"));
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileName = string.Format("{0}\\{1}-V{2} {3}.log", path, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now.ToString("yyyy-MM-dd"));
            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(fileName, true, System.Text.Encoding.Default))
            {
                sr.WriteLine(error);
            }
        }

    }
}