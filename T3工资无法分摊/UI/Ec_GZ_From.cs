using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HongHu;
using HongHu.DAL.DBUtility;
using UFTReferC;
using System.Data.SqlClient;
using log4net;
using System.Reflection;
namespace Ec_Changjie
{
    public partial class Ec_GZ_from : System.Windows.Forms.AbnormityFrame.AbnormityForm
    {
        #region 属性 "config

        /// <summary>
        /// 多线程操作
        /// </summary>
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        /// <summary>
        ///配置文件
        /// </summary>
        HongHu.DLL.Config.ConfigItemXML config = new HongHu.DLL.Config.ConfigItemXML();

        /// <summary>
        /// 工资系统最新可修改月份
        /// </summary>
        private int GzNewMend;

        /// <summary>
        /// 登录控件
        /// </summary>
        UFLoginSQL.LoginClass m_Login;
        /// <summary>
        /// 要操作的母科目
        /// </summary>
        private CodeItem do_codeitem;
        /// <summary>
        /// 欲换成的辅助核算方式
        /// </summary>
        codefuzhu codefz = codefuzhu.bdept;
        #endregion 属性

        #region 默认声明函数
        
        /// <summary>
        /// 默认声明函数
        /// </summary>
        public Ec_GZ_from()
        {
            DLL_Sql.log.Info("声明函数");
            //设置不捕获异常控件调用
            Control.CheckForIllegalCrossThreadCalls = false;
            // 登录帐套_得到连接
            DLL_Sql.log.Info("登录用友");
            Login_Uf();
            //何不让DataGridView自动生成列
            InitializeComponent();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            InitializeBackgoundWorker();

            DLL_Sql.log.Info("窗体加载函数");
            Get_NewMend();
        }
        #endregion

        #region 初始化函数

        /// <summary>
        /// 初始化多线程组件
        /// </summary>
        private void InitializeBackgoundWorker()
        {
            backgroundWorker1.WorkerReportsProgress = false;
            //执行
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
          //完成
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
           //报告
         //   backgroundWorkerfrom_loading.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerfrom_loading_ProgressChanged);
        }

        #endregion  初始化函数

        #region 窗体加载
        
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.AutoGenerateColumns = false;
            MessageBox.Show("操作有一定危险行!!请先做好备份.....");
        }
        #endregion
        #region 查询工资系统最新可修改月份
        
        /// <summary>
        /// 查询工资系统最新可修改月份
        /// </summary>
        private void Get_NewMend()
        {
            DLL_Sql.log.Info("查询工资系统最新可修改月份");
            object _tempobj=DLL_Sql.GetNewMend("bflag_WA");
            if(_tempobj==null)
            {
                MessageBox.Show("不能查询工资系统最新可修改月份 请确认工资系统是否启用   程序退出");
                DLL_Sql.log.Info("不能查询工资系统最新可修改月份 请确认工资系统是否启用   程序退出");
                return;
            }
            GzNewMend = int.Parse(_tempobj.ToString());
            DLL_Sql.log.Info("工资系统最新可修改月份为" + HongHu.DigitToChnText.monthtoUpper(GzNewMend) + "月份");
            tb_NewMend.Text = HongHu.DigitToChnText.monthtoUpper(GzNewMend) + "月份";
        }
        #endregion

        #region 登录帐套_得到连接 Login_Uf()

        /// <summary>
        /// 登录帐套_得到连接
        /// </summary>
        private void Login_Uf()
        {
            SetString.SQLConn = null;
            DLL_Sql.log.Info("开始登录用友");
            m_Login = new UFLoginSQL.LoginClass();
            // m_Login.Login()
            String sSubId = "WA";	 //登陆总账
            String sAccID = null;
            String sYear = null;
            String sUserID = null;
            String sPassword = null;
            String sDate = null;
            String sServer = null;
            //String sSerial = "";
            try
            {
                if (!m_Login.Login(ref sSubId, ref sAccID, ref sYear, ref sUserID, ref sPassword, ref sDate, ref sServer))
                {
                    DLL_Sql.log.Info("登陆失败，原因：" + m_Login.ShareString);
                    MessageBox.Show("登陆失败，原因：" + m_Login.ShareString);
                    this.Close();
                    //Marshal.FinalReleaseComObject(m_Login);
                    return;
                }

            }
            catch (Exception ex)
            {
                DLL_Sql.log.Error("控件操作失败! 程序退出!", ex);
                MessageBox.Show("登录控件操作失败!!");
                this.Close();
                return;
                //throw;
            }

            DLL_Sql.log.Info("登录成功");
            //SysDataLog.jsonwriteobj(m_Login);
            //SysDataLog.jsonwriteobj(m_Login.UfSystemDb);
            //DLL_Sql.log.Info(m_Login.UfSystemDb);
            //SysDataLog.jsonwriteobj(m_Login.UfSystemDb);
            SetString.SQLConn = m_Login.UfDbName.Substring(17);
            HongHu.DBUtility.DbHelperSQL.connectionString = SetString.SQLConn;
            DLL_Sql.log.Info("SQLConn=" + SetString.SQLConn);
            //测试连接
            if (SqlHelper.TestConnStr(SetString.SQLConn) != null)
            {
                MessageBox.Show("连接数据库失败!");
                DLL_Sql.log.Info("连接数据库失败");
                this.Close();
                return;
            }
            Objectm_Login = m_Login;
        }
        #endregion 登录帐套_得到连接 Login_Uf()

        #region 窗体关闭

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DLL_Sql.log.Info("用友软件登出");
            //在进行完成相应的登陆流程后，必须通过通过该方法来关闭该控件，否则会浪费系统资源。
            m_Login.ShutDown();
            DLL_Sql.log.Info(" 窗体关闭,程序结束");
        }
        #endregion 窗体关闭

        #region 关于窗口

        /// <summary>
        /// 关于窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_About_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
        #endregion 关于窗口


        #region 屏蔽非数字输入
        /// <summary>
        /// 屏蔽非数字输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_do_code_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion 屏蔽非数字输入

        #region 执行修改操作

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glassButton1_Click_1(object sender, EventArgs e)
        {
            #region 禁用其他相关控件

            tb_NewMend.Enabled = false;
            Bt_About.Enabled = false;
            //Bt_st_do_code.Enabled = false;
            //groupBox1.Enabled = false;
            //groupBox2.Enabled = false;
            glassButton1.Enabled = false;
            #endregion 禁用其他相关控件

            DLL_Sql.log.Info("对用户进行确认修改操作!");
            if (MessageBox.Show(
                "请确认执行操作"
                , "提示!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                DLL_Sql.log.Info("检查是否有已经生成的凭证!"); 
                //检查是否有已经生成的凭证
                DataSet dataread = DLL_Sql.Get_WA_PZ(m_Login.cIYear, GzNewMend);
                if (dataread.Tables[0].Rows.Count != 0)
                {
                    DLL_Sql.log.Info("凭证已存在\n"+SetString.SetJson(dataread.Tables[0]));
                    MessageBox.Show("当月已有以生成的凭证.请删除后在操作.");
                }
                else
                {
                    DLL_Sql.log.Info("执行修改操作!");
                    try
                    {
                        DLL_Sql.Exec_Gzbnzd(m_Login.cIYear, GzNewMend);
                    }
                    catch(Exception ex)
                    {
                        DLL_Sql.log.Info("执行修改操作出错" + ex.Message);
                        MessageBox.Show("执行修改操作出错" + ex.Message);
                        return;
                    }
                    DLL_Sql.log.Info("操作成功!");
                    MessageBox.Show("操作成功" );
                }
            }
            DLL_Sql.log.Info("修改结束!!!");
            MessageBox.Show("修改结束!!!");

            #region 启用其他相关控件
            tb_NewMend.Enabled = true;
            Bt_About.Enabled = true;
            //Bt_st_do_code.Enabled = true;
            //groupBox1.Enabled = true;
            //groupBox2.Enabled = true;
            glassButton1.Enabled = true;
            #endregion 启用其他相关控件
        }
        /// <summary>
        /// 修改科目凭证明细账
        /// </summary>
        /// <param name="mccode">母科目</param>
        /// <param name="zccode">子科目</param>
        /// <param name="bmId">辅助编码</param>
        /// <param name="hslb">类别字段</param>
        /// <returns></returns>
        public delegate void ExecCodeFz(string mccode, string zccode, string bmId, string hslb);
        #endregion 执行修改操作
        #region 执行线程

        #region 执行工作主线程
        /// <summary>
        /// 执行工作主线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // BackgroundWorker worker = sender as BackgroundWorker;
            //e.Result = ((HongHuWorkArgs)e.Argument).WorkDoEnumWork;
            //backgroundWorkerfrom_loading = worker;
            ComputeFibonacci(e.Argument ); //e.Result = ComputeFibonacci((DataGridViewCellEventArgs)e.Argument, worker, e);
        }
        /// <summary>
        /// 执行主要工作
        /// </summary>
        /// <param name="dgvcea"></param>
        /// <param name="worker"></param>
        /// <param name="dwea"></param>
        void ComputeFibonacci(object dwea)
        {
            //DataGridViewCellEventArgs e = (DataGridViewCellEventArgs)dwea;

            //    DLL_Sql.log.Info("点击选择辅助按钮");
            //    UFReferClientClass ufRef = new UFReferClientClass();
            //    string Tbname = "";
            //    switch (codefz)
            //    {
            //        case codefuzhu.bcus://客户
            //            Tbname = "Customer";
            //            break;
            //        case codefuzhu.bdept://部门
            //            Tbname = "Department";
            //            break;
            //        //case codefuzhu.bitem:
            //        //    Tbname="Customer";
            //        //    break;
            //        case codefuzhu.bperson://个人
            //            Tbname = "Person";
            //            break;
            //        case codefuzhu.bsup://供应商
            //            Tbname = "Vendor";
            //            break;
            //    }

            //    // LoLogin            Login对象 
            //    //ReferDisplayMode    枚举类型：显示模式，树型加列表为2；仅有Grid列表为1 
            //    //bMulti              是否多选  
            //    //EnumId_Table        变体类型，建议直接用表名（支持枚举类型值和数值）  
            //    //sFilter             过滤串  
            //    //bPage               是否分页  
            //    //lPageSize           页大小  
            //    //lPageCount          总页数  
            //    //lCurPage            当前页 

            //    ufRef.EnumRefInit(ref Objectm_Login, DisplayMode.enuGrid, false, Tbname, "", false, 1, ref lCurPageInt, 1);

            //    ufRef.Show();
            //    if (ufRef.recmx == null)
            //    {
            //        DLL_Sql.log.Info("未选择辅助!");
            //        return;
            //    }
            //    this.dataGridView1["辅助编码", e.RowIndex].Value = DLL_Sql.GetFzId(" and " + ufRef.recmx.Filter.ToString(), codefz);
            //    //if (!DLL_Sql.GetCodeItem(" and " + ufRef.recmx.Filter.ToString(), ref do_codeitem))
            //    //{
            //    //    DLL_Sql.log.Info("读取科目失败!");
            //    //    return;
            //    //}
            //    //项目参照()
            //    // ufRef.ItemRefInit(Objectm_Login,
            
            ////取消后台工作
            ////if (worker.CancellationPending)
            ////{
            ////    dwea.Cancel = true;
            ////}
            ////报告进度
            ////worker.ReportProgress(percentComplete);
            ////return result;
        }

        #endregion

        #endregion 执行线程

        #region 线程结束
        /// <summary>
        /// 线程结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Error != null)
            //{
            //    MessageBox.Show(e.Error.Message);
            //}
            //else if (e.Cancelled)
            //{
            //    //resultLabel.Text = "Canceled";
            //}
            //else
            //{
            //    //resultLabel.Text = e.Result.ToString();
            //}
            ////this.P3_sx_bt_Click(sender, new EventArgs());

            //dataGridView1.Enabled = true;
        }
        #endregion 线程结束

        #region dataGridView1内容被单击事件 
        /// <summary>
        /// dataGridView1内容被单击事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if ((e.RowIndex >= 0) && (e.ColumnIndex == this.dataGridView1.Columns["选择"].Index))
            //{
            //    dataGridView1.Enabled = false;
            //    //开始后台线程
            //    backgroundWorker1.RunWorkerAsync(e);
            //}
        }
        int lCurPageInt = 1;
        Object Objectm_Login;
        #endregion dataGridView1内容被单击事件

        #region 更改欲改为核算方式
        /// <summary>
        /// 更改欲改为核算方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FzRDbt_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton temprb = (RadioButton)sender;
            switch (temprb.Name)
            {
                case "RB_bdept":
                    codefz = codefuzhu.bdept;
                    break;
                case "RB_bperson":
                    codefz = codefuzhu.bperson;
                    break;
                case "RB_bcus":
                    codefz = codefuzhu.bcus;
                    break;
                case "RB_bsup":
                    codefz = codefuzhu.bsup;
                    break;
                case "RB_bitem":
                    codefz = codefuzhu.bitem;
                    MessageBox.Show("请选择项目大类编码!");
                    break;

            }
        }

        #endregion 更改欲改为核算方式

    }
}