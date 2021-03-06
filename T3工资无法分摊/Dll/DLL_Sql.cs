﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using HongHu.DAL.DBUtility;
using HongHu;
using System.Data;

namespace Ec_Changjie
{
    public class DLL_Sql
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 　
        #region 属性
        /// <summary>
        /// 执行工资模块不能制单
        /// 0 Year
        /// 1 imonth
        /// </summary>
        private static readonly string Sql_Exec_GZBNZD = @"Delete  wa_gzft where    iyear ={0} and imonth={1} ";//cgzgradenum='001'
        /// <summary>
        /// 查询工资系统生成的凭证
        /// 0 Year
        /// 1 imonth
        /// </summary>
        private static readonly string Sql_Selec_WaPz = @"
                SELECT imonth, 
                       doutdate, 
                       cbill, 
                       dbill_date, 
                       coutno_id, 
                       ino_id, 
                       ibook, 
                       ccheck, 
                       iflag, 
                       credflag, 
                       csign, 
                       coutsign, 
                       iperiod 
                FROM   gl_accvouch 
                       INNER JOIN wa_gzft 
                         ON gl_accvouch.coutno_id = wa_gzft.cpzid 
                WHERE  YEAR(dbill_date) = {0} 
                       AND ( iflag <> 1 
                              OR iflag IS NULL ) 
                       AND coutsysname = 'WA' 
                       AND ( imonth = {1} ) 
                GROUP  BY imonth, 
                          doutdate, 
                          cbill, 
                          dbill_date, 
                          coutno_id, 
                          ino_id, 
                          ibook, 
                          ccheck, 
                          iflag, 
                          credflag, 
                          csign, 
                          coutsign, 
                          iperiod 
                ORDER  BY imonth, 
                          coutno_id ";
        /// <summary>
        ///   获得系统最新可修改月份
        ///bflag_AP (U861)  应付结账标志  bit 1  True  
        ///bflag_AR (U861)  应收结账标志  bit 1  True  
        ///bflag_CA (U861)  成本结账标志  bit 1  True  
        ///bflag_FA (U861)  固定资产结账标志  bit 1  True  
        ///bflag_FD (U861)  资金结账标志  bit 1  True  
        ///bflag_IA (U861)  存货结账标志  bit 1  True  
        ///bflag_PP (U861)  物料需求计划结账标志  nvarchar 50  True  
        ///bflag_PU (U861)  采购结账标志  bit 1  True  
        ///bflag_WA (U861)  工资结账标志  bit 1  True  
        ///bflag_ST (U861)  库存结账标志  bit 1  True  
        ///bflag_SA (U861)  销售结账标志  bit 1  True  
        ///pubufts (U861)  时间戳  timestamp 8  True  
        ///bflag_GS (U861)  GSP质量管理结帐标志  bit 1  False  
        ///bflag_WH (U861)  报账中心结账标志  bit 1  False  
        ///bflag_NB (U861)  网上银行结账标志  bit 1  True  
        ///bflag_PM (U861)  项目管理结帐标志  bit 1  False  
        ///bflag_CP (U861)  CP标志  bit 1  True  
        ///bflag_OM (U861)  委外管理标志  bit 1  False  
        /// </summary>
        private static readonly string Sql_Get_Newmend = " select top 1 iperiod+1  from  GL_mend where {0}=1 ORDER BY iperiod desc";
        /// <summary>
        /// 查询Code表
        /// </summary>
        private static readonly string Sql_Select_UfCode = "SELECT i_id, cclass, cclass_engl, cclassany, cclassany_engl, ccode, ccode_name, " +
                           "ccode_engl, igrade, bproperty, cbook_type, cbook_type_engl, chelp, cexch_name, " +
                           "cmeasure, bperson, bcus, bsup, bdept, bitem, cass_item, br, be, cgather, bend, " +
                           "bexchange, bcash, bbank, bused, bd_c, dbegin, dend, itrans, bclose, cother, " +
                           "iotherused, bcDefine1, bcDefine2, bcDefine3, bcDefine4, bcDefine5, bcDefine6, " +
                           "bcDefine7, bcDefine8, bcDefine9, bcDefine10, iViewItem, bGCJS, bCashItem FROM code where 1=1 ";
        /// <summary>
        /// 查询子科目条件_字符串带参数1为母科目编码
        /// </summary>
        private static readonly string Sql_Where_Zcode = " and ccode like '{0}%' and ccode<> '{0}'";
        /// <summary>
        /// 获取Code表记录数
        /// </summary>
        private static readonly string Sql_Select_UfCodeCount = "SELECT  count(*) from code where 1=1";
        /// <summary>
        /// 获取部门ID
        /// </summary>
        private static readonly string Sql_Select_GetDepartmentID = "SELECT cDepCode FROM Department where 1=1";
        /// <summary>
        /// 获取客户ID
        /// </summary>
        private static readonly string Sql_Select_GetCustomerID = "SELECT cCusCode FROM Customer where 1=1";
        /// <summary>
        /// 获取个人ID
        /// </summary>
        private static readonly string Sql_Select_GetPersonID = "SELECT cPersonCode FROM Person where 1=1";
        /// <summary>
        /// 获取供应商ID
        /// </summary>
        private static readonly string Sql_Select_GetVendorID = "SELECT cVenCode FROM Vendor where 1=1";

        /// <summary>
        /// 修改辅助核算方式
        /// bperson 	-- 是否个人往来核算  bit 1  False  
        /// bcus 		-- 是否客户往来核算  bit 1  False  
        /// bsup 		-- 是否供应商往来核算  bit 1  False  
        /// bdept 	    -- 是否部门核算  bit 1  False  
        /// bitem 	    -- 是否项目核算  bit 1  False  
        /// </summary>
        private static readonly string Sql_Updata_codefz = " UPDATE [code] SET {0}=1 WHERE ccode ='{1}'";
        /// <summary>
        /// 改凭证及明细账为核算
        /// 0母科目 1子科目 2辅助ID 3核算字段
        /// </summary>
        private static readonly string Sql_Updata_codefz_pzmxz_bn = "UPDATE [GL_accvouch]    SET [ccode] = {0}    ,[{3}] ={2}  WHERE [ccode] = '{1}' ";

        /// <summary>
        /// 改凭证及明细账为核算_个人往来
        /// 0母科目 1子科目 2辅助ID 3核算字段
        /// </summary>
        private static readonly string Sql_Updata_codefz_pzmxz_grwl = "UPDATE [GL_accvouch]    SET [ccode] = {0}    ,[{3}] ={2},cdept_id =  (select cDepCode from Person where cPersonCode='{2}')  WHERE [ccode] = '{1}' ";
        /// <summary>
        /// 改凭证及明细账对方科目
        /// {0}子科目{1}母科目
        /// </summary>
        private static readonly string Sql_Updata_codefz_dfkm = "  UPDATE [GL_accvouch]    set [ccode_equal] =replace([ccode_equal],'{0}','{1}') where [ccode_equal] like '%{0}%'";

        #endregion 属性

        #region 方法
        #region 执行操作
        public static void Exec_Gzbnzd(string Year, int imonth)
        {
            DLL_Sql.log.Info(@"执行操作工资系统清除不能生成凭证操作 
                               " + string.Format(Sql_Exec_GZBNZD, Year, imonth));
             HongHu.DBUtility.DbHelperSQL.ExecuteSql(string.Format(Sql_Exec_GZBNZD, Year, imonth));
        }
        #endregion
        #region 查询工资系统生成的凭证
        public static DataSet Get_WA_PZ(string Year, int imonth)
        {
            DLL_Sql.log.Info(@"查询工资系统生成的凭证 
                               " + string.Format(Sql_Selec_WaPz, Year, imonth));
            return HongHu.DBUtility.DbHelperSQL.Query(string.Format(Sql_Selec_WaPz, Year, imonth));
        }
        #endregion
        #region 获得最新可修改月份
        /// <summary>
        /// 获得最新可修改月份
        ///bflag_AP (U861)  应付结账标志  bit 1  True  
        ///bflag_AR (U861)  应收结账标志  bit 1  True  
        ///bflag_CA (U861)  成本结账标志  bit 1  True  
        ///bflag_FA (U861)  固定资产结账标志  bit 1  True  
        ///bflag_FD (U861)  资金结账标志  bit 1  True  
        ///bflag_IA (U861)  存货结账标志  bit 1  True  
        ///bflag_PP (U861)  物料需求计划结账标志  nvarchar 50  True  
        ///bflag_PU (U861)  采购结账标志  bit 1  True  
        ///bflag_WA (U861)  工资结账标志  bit 1  True  
        ///bflag_ST (U861)  库存结账标志  bit 1  True  
        ///bflag_SA (U861)  销售结账标志  bit 1  True  
        ///pubufts (U861)  时间戳  timestamp 8  True  
        ///bflag_GS (U861)  GSP质量管理结帐标志  bit 1  False  
        ///bflag_WH (U861)  报账中心结账标志  bit 1  False  
        ///bflag_NB (U861)  网上银行结账标志  bit 1  True  
        ///bflag_PM (U861)  项目管理结帐标志  bit 1  False  
        ///bflag_CP (U861)  CP标志  bit 1  True  
        ///bflag_OM (U861)  委外管理标志  bit 1  False  
        /// </summary>
        /// <param name="pSubName">模块名称</param>
        /// <returns></returns>
        public static object GetNewMend(string pSubName)
        {
            return SqlHelper.ExecuteScalar(SetString.SQLConn, CommandType.Text,string.Format(Sql_Get_Newmend,pSubName), null);
        }

        #endregion

        #region 检索指定科目的信息
    /// <summary>
        ///  检索指定科目的信息
    /// </summary>
    /// <param name="where"></param>
    /// <param name="relusCodeItem"></param>
    /// <returns></returns>
        public static bool GetCodeItem(string where, ref CodeItem relusCodeItem)
        {

            DLL_Sql.log.Info("读取取的科目数");
            if (1 != int.Parse(SqlHelper.ExecuteScalar(SetString.SQLConn, CommandType.Text, Sql_Select_UfCodeCount + where, null).ToString()))
            {
                DLL_Sql.log.Info("读取取的科目数不为1");
                return false;
            }

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SetString.SQLConn, CommandType.Text, string.Format(Sql_Select_UfCode + where), null))
            {
                while (rdr.Read())
                {
                    
                    DLL_Sql.log.Info("读取科目信息");
                    relusCodeItem = new CodeItem(
                        rdr.GetValue(8).ToString(), rdr.GetValue(1).ToString(), rdr.GetValue(2).ToString(), rdr.GetValue(3).ToString(),
                        rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetValue(6).ToString(), rdr.GetValue(7).ToString(),
                        rdr.GetValue(8).ToString(), bool.Parse(rdr.GetValue(9).ToString()), rdr.GetValue(10).ToString(),
                        rdr.GetValue(11).ToString(), rdr.GetValue(12).ToString(), rdr.GetValue(13).ToString(), rdr.GetValue(14).ToString(),
                        bool.Parse(rdr.GetValue(15).ToString()), bool.Parse(rdr.GetValue(16).ToString()), bool.Parse(rdr.GetValue(17).ToString()),
                        bool.Parse(rdr.GetValue(18).ToString()), bool.Parse(rdr.GetValue(19).ToString()), rdr.GetValue(20).ToString(),
                        bool.Parse(rdr.GetValue(21).ToString()), bool.Parse(rdr.GetValue(22).ToString()), rdr.GetValue(23).ToString(),
                        bool.Parse(rdr.GetValue(24).ToString()), bool.Parse(rdr.GetValue(25).ToString()), bool.Parse(rdr.GetValue(26).ToString()),
                        bool.Parse(rdr.GetValue(27).ToString()), bool.Parse(rdr.GetValue(28).ToString()), bool.Parse(rdr.GetValue(29).ToString()),
                        rdr.GetValue(30).ToString(), rdr.GetValue(31).ToString(), rdr.GetValue(32).ToString(),
                        bool.Parse(rdr.GetValue(33).ToString()), rdr.GetValue(34).ToString(), rdr.GetValue(35).ToString(), bool.Parse(rdr.GetValue(36).ToString()),
                        bool.Parse(rdr.GetValue(37).ToString()), bool.Parse(rdr.GetValue(38).ToString()), bool.Parse(rdr.GetValue(39).ToString()), bool.Parse(rdr.GetValue(40).ToString()),
                        bool.Parse(rdr.GetValue(41).ToString()), bool.Parse(rdr.GetValue(42).ToString()), bool.Parse(rdr.GetValue(43).ToString()), bool.Parse(rdr.GetValue(44).ToString()),
                        bool.Parse(rdr.GetValue(45).ToString()), rdr.GetValue(46).ToString(), bool.Parse(rdr.GetValue(47).ToString()), bool.Parse(rdr.GetValue(48).ToString()));
                }
                return true;
            }
        }

        ///// <summary>
        ///// 查询子科目_返回DataSet
        ///// </summary>
        ///// <param name="Mcode"></param>
        ///// <returns></returns>
        //public static DataSet GetZcodeItems(string Mcode)
        //{
        //    return SqlHelper.ExecuteDataset(SetString.SQLConn, Sql_Select_UfCode + string.Format(Sql_Where_Zcode, Mcode));

        //}
        #endregion 检索指定科目的信息

        #region 获得辅助ID
        public static object GetFzId(string where, codefuzhu fz)
        {
            string SelectSql = "";
            switch (fz)
            {
                case codefuzhu.bcus://客户
                    SelectSql = Sql_Select_GetCustomerID;
                    break;
                case codefuzhu.bdept://部门
                    SelectSql = Sql_Select_GetDepartmentID;
                    break;
                //case codefuzhu.bitem:
                //    Tbname="Customer";
                //    break;
                case codefuzhu.bperson://个人
                    SelectSql = Sql_Select_GetPersonID;
                    break;
                case codefuzhu.bsup://供应商
                    SelectSql = Sql_Select_GetVendorID;
                    break;
            }
            return SqlHelper.ExecuteScalar(SetString.SQLConn, CommandType.Text, SelectSql + where, null);
        }

        #endregion 获得辅助ID

        /// <summary>
        /// 修改科目辅助核算方式.
        /// </summary>
        /// <param name="fzstr"></param>
        /// <param name="ccode"></param>
        public static void ExecCodeFz(string fzstr, string ccode)
        {
            SqlHelper.ExecuteNonQuery(SetString.SQLConn, CommandType.Text, string.Format(Sql_Updata_codefz, fzstr, ccode), null);
        }
        /// <summary>
        /// 修改科目辅助核算方式.
        /// </summary>
        /// <param name="mccode">母科目</param>
        /// <param name="zccode">子科目</param>
        /// <param name="bmId">辅助ID</param>
        /// <param name="hslb">核算字段</param>
        public static void ExecCodeFzBm(string mccode, string zccode, string bmId,string hslb)
        {
            SqlHelper.ExecuteNonQuery(SetString.SQLConn, CommandType.Text, string.Format(Sql_Updata_codefz_pzmxz_bn, mccode, zccode, bmId,hslb), null);
        }
        /// <summary>
        /// 修改科目辅助核算方式.
        /// </summary>
        /// <param name="mccode">母科目</param>
        /// <param name="zccode">子科目</param>
        /// <param name="bmId">辅助ID</param>
        /// <param name="hslb">核算字段</param>
        public static void ExecCodeFzGr(string mccode, string zccode, string bmId, string hslb)
        {
            SqlHelper.ExecuteNonQuery(SetString.SQLConn, CommandType.Text, string.Format(Sql_Updata_codefz_pzmxz_grwl, mccode, zccode, bmId, hslb), null);
        }
        /// <summary>
        /// 改凭证及明细账对方科目
        /// </summary>
        /// <param name="mccode">母科目</param>
        /// <param name="zccode">子科目</param>
        public static void ExecCodeFzDfkm(string mccode, string zccode)
        {
            SqlHelper.ExecuteNonQuery(SetString.SQLConn, CommandType.Text, string.Format(Sql_Updata_codefz_dfkm, mccode, zccode), null);
        }
        #endregion 方法
    }
}
