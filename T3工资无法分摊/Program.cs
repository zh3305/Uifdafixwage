using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ec_Changjie
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //捕获未处理异常
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
 


           DLL_Sql.log.Info("程序开始..");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           DLL_Sql.log.Info("开始载入主窗体..");
            Application.Run(new Ec_GZ_from());
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
           DLL_Sql.log.Error(e.Exception);
            //throw new Exception("线程未知异常", e.Exception);
            MessageBox.Show(e.Exception.Message, "线程异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
           DLL_Sql.log.Error(ex);
            MessageBox.Show(ex.Message, "应用程序异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

    }
}
