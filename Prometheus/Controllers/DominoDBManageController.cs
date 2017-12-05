using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;
using System.Data.SqlClient;
using System.Data;

namespace Domino.Controllers
{
    public class DominoDBManageController : Controller
    {
        // GET: DbManage
        public ActionResult ExecuteSQLs()
        {
            return View();
        }

        [HttpPost, ActionName("ExecuteSQLs")]
        [ValidateAntiForgeryToken]
        public ActionResult ExecuteSQLsPost()
        {
            var querystr = Request.Form["querystring"];
            if (!querystr.Contains("select")
                && !querystr.Contains("insert")
                && !querystr.Contains("update")
                && !querystr.Contains("delete"))
            {
                ViewBag.ExecuteRes = "invalidate sql strings";
                return View();
            }

            var ret = true;
            var sqls = querystr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in sqls)
            {
                if (!DBUtility.ExeLocalSqlNoRes(s.Trim()))
                {
                    ret = false;
                }
            }

            if(ret)
                ViewBag.ExecuteRes = "Execute sucessfully";
            else
                ViewBag.ExecuteRes = "Sqls have error";

            return View();
        }

        public ActionResult MoveDataBase()
        {
            //var sourcedb = "Server=WUX-D80008792;User ID=DominoNPI;Password=abc@123;Database=DominoTrace;Connection Timeout=300;";
            //var targetdb = "Server=wux-engsys01;User ID=DominoNPI;Password=abc@123;Database=DominoTrace;Connection Timeout=300;";

            //var tablelist = new List<string>();
            //tablelist.Add("ECOBaseInfo");
            //tablelist.Add("ECOCard");
            //tablelist.Add("ECOCardAttachment");
            //tablelist.Add("ECOCardComment");
            //tablelist.Add("ECOCardContent");
            //tablelist.Add("ECOJOCheckInfo");
            //tablelist.Add("ECOJOOrderInfo");
            //tablelist.Add("ECOPendingUpdate");
            //tablelist.Add("UserMatrix");
            //tablelist.Add("UserRank");
            //tablelist.Add("UserTable");

            //var sourcedb = "Server=WUX-D80008792;User ID=dbg;Password=dbgpwd;Database=DebugDB;Connection Timeout=120;";
            //var targetdb = "Server=wux-engsys01;User ID=dbg;Password=dbgpwd;Database=DebugDB;Connection Timeout=120;";

            //var tablelist = new List<string>();
            //tablelist.Add("AlignmentPower");
            //tablelist.Add("BIError");
            //tablelist.Add("BIROOTCAUSE");
            //tablelist.Add("BITestData");
            //tablelist.Add("BITestDataField");
            //tablelist.Add("BITestResult");
            //tablelist.Add("BITestResultDataField");
            //tablelist.Add("BookReportRecord");
            //tablelist.Add("ErrorComments");
            //tablelist.Add("FileLoadedData");
            //tablelist.Add("Issue");
            //tablelist.Add("IssueAttachment");
            //tablelist.Add("IssueAttribute");
            //tablelist.Add("IssueComments");
            //tablelist.Add("IssueIcare");
            //tablelist.Add("IssueOBA");
            //tablelist.Add("IssueRMA");
            //tablelist.Add("IssueType");
            //tablelist.Add("LinkVM");
            //tablelist.Add("Log");
            //tablelist.Add("MachineLink");
            //tablelist.Add("ModuleTXOData");
            //tablelist.Add("NeoMapData");
            //tablelist.Add("OSAFailureVM");
            //tablelist.Add("PJErrorAttachment");
            //tablelist.Add("Project");
            //tablelist.Add("ProjectCriticalError");
            //tablelist.Add("ProjectError");
            //tablelist.Add("ProjectErrorICare");
            //tablelist.Add("ProjectEvent");
            //tablelist.Add("ProjectException");
            //tablelist.Add("ProjectMembers");
            //tablelist.Add("ProjectMesTable");
            //tablelist.Add("ProjectModelID");
            //tablelist.Add("ProjectMoveHistory");
            //tablelist.Add("ProjectPn");
            //tablelist.Add("ProjectStation");
            //tablelist.Add("ProjectSumDataSet");
            //tablelist.Add("ProjectTestData");
            //tablelist.Add("ProjectWorkflow");
            //tablelist.Add("RELBackupData");
            //tablelist.Add("RELMapData");
            //tablelist.Add("RMABackupData");
            //tablelist.Add("RMAMapData");
            //tablelist.Add("SameAsDBTVM");
            //tablelist.Add("ShareDoc");
            //tablelist.Add("ShareTags");
            //tablelist.Add("UserBlog");
            //tablelist.Add("UserCacheInfo");
            //tablelist.Add("UserGroupVM");
            //tablelist.Add("UserKPIVM");
            //tablelist.Add("UserLearn");
            //tablelist.Add("UserMatrix");
            //tablelist.Add("UserNet");
            //tablelist.Add("UserRank");
            //tablelist.Add("UserReviewedItems");
            //tablelist.Add("UserTable");
            //tablelist.Add("VCSELUsageTable");
            //tablelist.Add("WaferRecord");
            //tablelist.Add("WeeklyReport");
            //tablelist.Add("WeeklyReportSetting");

            //var sourcedb = "Server=WUX-D80008792;User ID=NebulaNPI;Password=abc@123;Database=NebulaTrace;Connection Timeout=120;";
            //var targetdb = "Server=wux-engsys01;User ID=NebulaNPI;Password=abc@123;Database=NebulaTrace;Connection Timeout=120;";

            //var tablelist = new List<string>();
            //tablelist.Add("AgileAffectItem");
            //tablelist.Add("AgileAttach");
            //tablelist.Add("AgileHistory");
            //tablelist.Add("AgilePageThree");
            //tablelist.Add("AgileWorkFlow");
            //tablelist.Add("BRAgileBaseInfo");
            //tablelist.Add("BRComment");
            //tablelist.Add("JOBaseInfo");
            //tablelist.Add("JOComponentInfo");
            //tablelist.Add("JOScheduleEventDataVM");
            //tablelist.Add("JOShipTrace");
            //tablelist.Add("JOSNStatus");
            //tablelist.Add("MachineColumn");
            //tablelist.Add("PMUpdateTime");
            //tablelist.Add("PNErrorDistribute");
            //tablelist.Add("PNWorkflow");
            //tablelist.Add("ProjectColumn");
            //tablelist.Add("ProjectLog");
            //tablelist.Add("ProjectVM");
            //tablelist.Add("UserCustomizeTheme");
            //tablelist.Add("UserTable");

            var sourcedb = "Server=WUX-D80008792;User ID=NPI;Password=NPI@NPI;Database=NPITrace;Connection Timeout=120;";
            var targetdb = "Server=wux-engsys01;User ID=NPI;Password=NPI@NPI;Database=NPITrace;Connection Timeout=120;";

            var tablelist = new List<string>();
            //tablelist.Add("AlignmentPower");
            //tablelist.Add("BIError");
            //tablelist.Add("BIROOTCAUSE");
            //tablelist.Add("BITestData");
            //tablelist.Add("BITestDataField");
            //tablelist.Add("BITestResult");
            //tablelist.Add("BITestResultDataField");
            //tablelist.Add("BookReportRecord");
            //tablelist.Add("ErrorComments");
            //tablelist.Add("FileLoadedData");
            //tablelist.Add("Issue");
            //tablelist.Add("IssueAttachment");
            //tablelist.Add("IssueAttribute");
            //tablelist.Add("IssueComments");
            //tablelist.Add("IssueIcare");
            //tablelist.Add("IssueOBA");
            //tablelist.Add("IssueRMA");
            //tablelist.Add("IssueType");
            //tablelist.Add("LinkVM");
            //tablelist.Add("Log");
            //tablelist.Add("MachineLink");
            //tablelist.Add("ModuleTXOData");
            //tablelist.Add("NeoMapData");
            //tablelist.Add("OSAFailureVM");
            //tablelist.Add("PJErrorAttachment");
            //tablelist.Add("Project");
            //tablelist.Add("ProjectCriticalError");
            //tablelist.Add("ProjectError");
            //tablelist.Add("ProjectErrorICare");
            //tablelist.Add("ProjectEvent");
            //tablelist.Add("ProjectException");
            //tablelist.Add("ProjectMembers");
            //tablelist.Add("ProjectMesTable");
            //tablelist.Add("ProjectModelID");
            //tablelist.Add("ProjectMoveHistory");
            //tablelist.Add("ProjectPn");
            //tablelist.Add("ProjectStation");
            //tablelist.Add("ProjectSumDataSet");
            //tablelist.Add("ProjectTestData");
            //tablelist.Add("ProjectWorkflow");
            //tablelist.Add("RELBackupData");
            //tablelist.Add("RELMapData");
            //tablelist.Add("RMABackupData");
            //tablelist.Add("RMAMapData");
            //tablelist.Add("SameAsDBTVM");
            //tablelist.Add("ShareDoc");
            //tablelist.Add("ShareTags");
            //tablelist.Add("UserBlog");
            //tablelist.Add("UserCacheInfo");
            //tablelist.Add("UserGroupVM");
            //tablelist.Add("UserKPIVM");
            //tablelist.Add("UserLearn");
            //tablelist.Add("UserMatrix");
            //tablelist.Add("UserNet");
            //tablelist.Add("UserRank");
            //tablelist.Add("UserReviewedItems");
            //tablelist.Add("UserTable");
            //tablelist.Add("VCSELUsageTable");
            //tablelist.Add("WaferRecord");
            //tablelist.Add("WeeklyReport");
            //tablelist.Add("WeeklyReportSetting");

            foreach (var tab in tablelist)
            {
                SqlConnection sourcecon = null;
                SqlConnection targetcon = null;

                try
                {
                    sourcecon = DBUtility.GetConnector(sourcedb);

                    targetcon = DBUtility.GetConnector(targetdb);
                    //var tempsql = "delete from " + tab;
                    //DBUtility.ExeSqlNoRes(targetcon, tempsql);

                    for (int idx = 0; ;)
                    {
                        var endidx = idx + 100000;

                            //load data to memory
                            var sql = "select * from(select ROW_NUMBER() OVER(order by(select null)) as mycount, * from " + tab + ") s1 where s1.mycount > "+ idx.ToString() +" and s1.mycount <= "+endidx.ToString();
                            var dt = DBUtility.ExecuteSqlReturnTable(sourcecon,sql);
                        if (dt.Rows.Count == 0)
                        {
                            break;
                        }

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(targetcon))
                                {
                                    bulkCopy.DestinationTableName = tab;
                                    try
                                    {
                                        for (int i = 1; i < dt.Columns.Count; i++)
                                        {
                                            bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                                        }
                                        bulkCopy.WriteToServer(dt);
                                        dt.Clear();
                                    }
                                    catch (Exception ex){}
                                }//end using
                            }//end if

                        idx = idx + 100000;
                    }//end for
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    if (targetcon != null)
                    {
                        DBUtility.CloseConnector(targetcon);
                        targetcon = null;
                    }

                    if (sourcecon != null)
                    {
                        DBUtility.CloseConnector(sourcecon);
                        sourcecon = null;
                    }
                }

                if (targetcon != null)
                {
                    DBUtility.CloseConnector(targetcon);
                }

                if (sourcecon != null)
                {
                    DBUtility.CloseConnector(sourcecon);
                }
            }
            return View();
        }
    }
}