﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Domino.Models
{
    public class ProjectTypeInf
    {
        public static string Parallel = "Parallel";
        public static string Tunable = "Tunable";
        public static string OSA = "OSA";
        public static string LineCard = "LineCard";
        public static string QM = "QM";
        public static string Others = "Others";
    }


    public class ProjectStation
    {
        public ProjectStation(string key, string stat)
        {
            ProjectKey = key;
            Station = stat;
        }

        public string ProjectKey { set; get; }
        public string Station { set; get; }
    }

    public class ProjectMembers
    {
        public ProjectMembers(string key, string sname,string srole)
        {
            ProjectKey = key;
            Name = sname;
            Role = srole;
        }

        public string ProjectKey { set; get; }
        public string Name { set; get; }
        public string Role { set; get; }
    }

    public class ProjectPn
    {
        public ProjectPn(string key, string p)
        {
            ProjectKey = key;
            Pn = p;
        }
        public string ProjectKey { set; get; }
        public string Pn { set; get; }
    }

    public class ProjectMesTable
    {
        public ProjectMesTable(string key,string stat,string table)
        {
            ProjectKey = key;
            Station = stat;
            TableName = table;
        }

        public string ProjectKey { set; get; }
        public string Station { set; get; }
        public string TableName { set; get; }
    }


    public class ProjectViewModels
    {
        public static string PMROLE = "PM";
        public static string ENGROLE = "ENG";

        public ProjectViewModels()
        {
            MonitorVcsel = "";
            ProjectType = "";
        }

        public ProjectViewModels(string prokey, string proname, string startdate, double finshrate, string sdescription,string monitorvcsel,string vcselwarning,string pjtype)
        {
            this.ProjectKey = prokey;
            this.dbProjectName = proname;
            this.StartDate = DateTime.Parse(startdate);
            this.FinishRating = finshrate;
            this.dbDescription = sdescription;
            this.MonitorVcsel = monitorvcsel;
            this.ProjectType = pjtype;
            if (string.IsNullOrEmpty(vcselwarning))
            {
                this.VcselWarningYield = "98.0";
            }
            else
            {
                try
                { this.VcselWarningYield = Convert.ToDouble(vcselwarning).ToString("0.0"); }
                catch (Exception ex) { this.VcselWarningYield = Convert.ToDouble(98).ToString("0.0"); }
            }
        }

        public string ProjectKey { set; get; }

        public string ProjectType { set; get; }

        public string sProjectName = "";

        public string ProjectName {
            set { sProjectName = value; }
            get { return sProjectName; }
        }

        public string dbProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(sProjectName))
                {
                    return "";
                }
                else
                {
                    try
                    {
                        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sProjectName));
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    sProjectName = "";
                }
                else
                {
                    try
                    {
                        sProjectName = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    }
                    catch (Exception)
                    {
                        sProjectName = "";
                    }

                }

            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { set; get; }


        //[StringLength(260, MinimumLength = 6)]
        //[Required]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+).*")]
        //public string Engineers { set; get; }

        public double FinishRating { set; get; }

        public string MonitorVcsel { set; get; }

        public string VcselWarningYield { set; get; }

        private string sDescription = "";

        [StringLength(90, MinimumLength = 6)]
        [Required]
        public string Description {
            get
            {
                return sDescription;
            }

            set
            {
                sDescription = value;
            }
        }

        public string dbDescription
        {
            get
            {
                if (string.IsNullOrEmpty(sDescription))
                {
                    return "";
                }
                else
                {
                    try
                    {
                        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sDescription));
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                    
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    sDescription = "";
                }
                else
                {
                    try
                    {
                        sDescription = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    }
                    catch (Exception)
                    {
                        sDescription = "";
                    }

                }
                
            }
        }

        private List<ProjectStation> lstation = new List<ProjectStation>();
        public List<ProjectStation> StationList
        {
            get
            { return lstation; }
            set
            {
                lstation.Clear();
                lstation.AddRange(value);
            }
        }

        public string Stations
        {
            set
            {
                lstation.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var tempstat = value.Replace("'", "");
                    var tempstats = tempstat.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in tempstats)
                    {
                        lstation.Add(new ProjectStation(ProjectKey, s.Trim()));
                    }
                }
            }

            get
            {
                var ret = "";
                foreach (var p in lstation)
                {
                    if (string.IsNullOrEmpty(ret))
                        ret = p.Station;
                    else
                        ret = ret + ";" + p.Station;
                }
                return ret;
            }
        }


        private List<ProjectPn> lpn = new List<ProjectPn>();
        public List<ProjectPn> PNList
        {
            get
            { return lpn; }
            set
            {
                lpn.Clear();
                lpn.AddRange(value);
            }
        }

        public string PNs {
            set
            {
                lpn.Clear();
                if(!string.IsNullOrEmpty(value))
                {
                    var temppn = value.Replace("'", "");
                    var temppns = temppn.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in temppns)
                    {
                        lpn.Add(new ProjectPn(ProjectKey, p.Trim()));
                    }
                }
            }

            get
            {
                var ret = "";
                foreach (var p in lpn)
                {
                    if (string.IsNullOrEmpty(ret))
                        ret = p.Pn;
                    else
                        ret = ret + ";" + p.Pn;
                }
                return ret;
            }
        }

        private List<ProjectPn> lmd = new List<ProjectPn>();
        public List<ProjectPn> MDIDList
        {
            get
            { return lmd; }
            set
            {
                lmd.Clear();
                lmd.AddRange(value);
            }
        }

        public string ModelIDs
        {
            set
            {
                lmd.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var temppn = value.Replace("'", "");
                    var temppns = temppn.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in temppns)
                    {
                        lmd.Add(new ProjectPn(ProjectKey, p.Trim()));
                    }
                }
            }

            get
            {
                var ret = "";
                foreach (var p in lmd)
                {
                    if (string.IsNullOrEmpty(ret))
                        ret = p.Pn;
                    else
                        ret = ret + ";" + p.Pn;
                }
                return ret;
            }
        }

        private List<ProjectStation> lsumdataset = new List<ProjectStation>();
        public List<ProjectStation> SumDatasetList
        {
            get
            { return lsumdataset; }
            set
            {
                lsumdataset.Clear();
                lsumdataset.AddRange(value);
            }
        }

        public string SumDatasets
        {
            set
            {
                lsumdataset.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var tempstat = value.Replace("'", "");
                    var tempstats = tempstat.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in tempstats)
                    {
                        lsumdataset.Add(new ProjectStation(ProjectKey, s.Trim()));
                    }
                }
            }

            get
            {
                var ret = "";
                foreach (var p in lsumdataset)
                {
                    if (string.IsNullOrEmpty(ret))
                        ret = p.Station;
                    else
                        ret = ret + ";" + p.Station;
                }
                return ret;
            }
        }

        private List<ProjectMesTable> ltab = new List<ProjectMesTable>();
        public List<ProjectMesTable> TabList
        {
            get
            { return ltab; }
            set
            {
                ltab.Clear();
                ltab.AddRange(value);
            }
        }

        private List<ProjectMembers> lmeb = new List<ProjectMembers>();
        public List<ProjectMembers> MemberList
        {
            get
            { return lmeb; }

            set
            {
                lmeb.Clear();
                lmeb.AddRange(value);

                foreach (var eg in lmeb)
                {
                    if (string.Compare(eg.Role, Domino.Models.ProjectViewModels.ENGROLE) == 0)
                    {
                        FirstEngineer = eg.Name;
                        break;
                    }
                }
            }
        }

        public string FirstEngineer { set; get; }

        //public string DVTDate { set; get; }
        //public string DVTStatus { set; get; }
        //public string DVTIssueKey { set; get; }

        //public string MVTDate { set; get; }
        //public string MVTStatus { set; get; }
        //public string MVTIssueKey { set; get; }

        public string PIP1Date { set; get; }

        public string MVTDate { set; get; }

        public string CurrentNPIProc { set; get; }
        public string CurrentNPIProcKey { set; get; }

        public double FirstYield { set; get; }
        public double RetestYield { set; get; }
        public string PendingTaskCount { set; get; }
        public string PendingFACount { set; get; }
        public string PendingRMACount { set; get; }

        private void StoreProjectBaseInfo()
        {
            var sql = "delete from Project where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", ProjectKey);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into Project(ProjectKey,ProjectName,StartDate,Description,FinishRate,APVal1,APVal2,ProjectType) values('<ProjectKey>','<ProjectName>','<StartDate>','<Description>',<FinishRate>,'<MonitorVcsel>','<VcselWarningYield>','<ProjectType>')";
            sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<ProjectName>", dbProjectName)
                .Replace("<StartDate>", StartDate.ToString("yyyy-MM-dd"))
                .Replace("<Description>", dbDescription).Replace("<FinishRate>", Convert.ToString(FinishRating))
                .Replace("<MonitorVcsel>", MonitorVcsel).Replace("<VcselWarningYield>", VcselWarningYield).Replace("<ProjectType>", ProjectType);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        private void StoreProjectMembers()
        {
            if (MemberList.Count > 0)
            {
                var sql = "delete from ProjectMembers where ProjectKey = '<ProjectKey>'";
                sql = sql.Replace("<ProjectKey>", ProjectKey);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            foreach (var item in MemberList)
            {
                var sql = "insert into ProjectMembers(ProjectKey,Name,Role) values('<ProjectKey>','<Name>','<Role>')";
                sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<Name>", item.Name).Replace("<Role>", item.Role);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        private void StoreProjectMesTable()
        {
            if (TabList.Count > 0)
            {
                var sql = "delete from ProjectMesTable where ProjectKey = '<ProjectKey>'";
                sql = sql.Replace("<ProjectKey>", ProjectKey);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            foreach (var item in TabList)
            {
                var sql = "insert into ProjectMesTable(ProjectKey,Station,TableName) values('<ProjectKey>','<Station>','<TableName>')";
                sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<Station>", item.Station).Replace("<TableName>", item.TableName);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        private void StoreProjectPN()
        {
            if (PNList.Count > 0)
            {
                var sql = "delete from ProjectPn where ProjectKey = '<ProjectKey>'";
                sql = sql.Replace("<ProjectKey>", ProjectKey);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            foreach (var item in PNList)
            {
                var sql = "insert into ProjectPn(ProjectKey,PN) values('<ProjectKey>','<PN>')";
                sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<PN>", item.Pn);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        private void StoreProjectModelID()
        {
            if (MDIDList.Count > 0)
            {
                var sql = "delete from ProjectModelID where ProjectKey = '<ProjectKey>'";
                sql = sql.Replace("<ProjectKey>", ProjectKey);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            foreach (var item in MDIDList)
            {
                var sql = "insert into ProjectModelID(ProjectKey,ModelID) values('<ProjectKey>','<ModelID>')";
                sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<ModelID>", item.Pn);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        private void StoreProjectStation()
        {
            if (StationList.Count > 0)
            {
                var sql = "delete from ProjectStation where ProjectKey = '<ProjectKey>'";
                sql = sql.Replace("<ProjectKey>", ProjectKey);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            foreach (var item in StationList)
            {
                var sql = "insert into ProjectStation(ProjectKey,Station) values('<ProjectKey>','<Station>')";
                sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<Station>", item.Station);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        private void StoreProjectSumDataSet()
        {
            if (SumDatasetList.Count > 0)
            {
                var sql = "delete from ProjectSumDataSet where ProjectKey = '<ProjectKey>'";
                sql = sql.Replace("<ProjectKey>", ProjectKey);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            foreach (var item in SumDatasetList)
            {
                var sql = "insert into ProjectSumDataSet(ProjectKey,SumDataSet) values('<ProjectKey>','<SumDataSet>')";
                sql = sql.Replace("<ProjectKey>", ProjectKey).Replace("<SumDataSet>", item.Station);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        public void StoreProject()
        {
            StoreProjectBaseInfo();
            StoreProjectMembers();
            StoreProjectMesTable();
            StoreProjectPN();
            StoreProjectStation();
            StoreProjectModelID();
            StoreProjectSumDataSet();
        }

        public bool CheckExistProject()
        {
            var sql = "select * from Project where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", ProjectKey);
            var ret = DBUtility.ExeLocalSqlWithRes(sql);
            if (ret.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<string> RetrieveAllProjectKey()
        {
            var sql = "select ProjectKey from Project order by ProjectKey ASC";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new List<string>();
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString( line[0]));
            }
            return ret;
        }

        private static List<ProjectMembers> RetrieveProjectMembers(string key)
        {
            var ret = new List<ProjectMembers>();

            var sql = "select Name,Role from ProjectMembers where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var m = new ProjectMembers(key, Convert.ToString(line[0]), Convert.ToString(line[1]));
                ret.Add(m);
            }

            return ret;
        }
        private static List<ProjectMesTable> RetrieveProjectMesTable(string key)
        {
            var ret = new List<ProjectMesTable>();

            var sql = "select Station,TableName from ProjectMesTable where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var m = new ProjectMesTable(key, Convert.ToString(line[0]), Convert.ToString(line[1]));
                ret.Add(m);
            }
            return ret;
        }

        private static List<ProjectPn> RetrieveProjectPn(string key)
        {
            var ret = new List<ProjectPn>();

            var sql = "select PN from ProjectPn where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var m = new ProjectPn(key, Convert.ToString(line[0]));
                ret.Add(m);
            }
            return ret;
        }

        private static List<ProjectStation> RetrieveProjectStation(string key)
        {
            var ret = new List<ProjectStation>();

            var sql = "select Station from ProjectStation where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var m = new ProjectStation(key, Convert.ToString(line[0]));
                ret.Add(m);
            }
            return ret;
        }

        private static List<ProjectPn> RetrieveProjectModelID(string key)
        {
            var ret = new List<ProjectPn>();

            var sql = "select ModelID from ProjectModelID where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var m = new ProjectPn(key, Convert.ToString(line[0]));
                ret.Add(m);
            }
            return ret;
        }

        private static List<ProjectStation> RetrieveProjectSumDataSet(string key)
        {
            var ret = new List<ProjectStation>();

            var sql = "select SumDataSet from ProjectSumDataSet where ProjectKey = '<ProjectKey>'";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var m = new ProjectStation(key, Convert.ToString(line[0]));
                ret.Add(m);
            }
            return ret;
        }


        public static ProjectViewModels RetrieveOneProject(string key)
        {
            var sql = "select ProjectKey,ProjectName,StartDate,FinishRate,Description,APVal1,APVal2,ProjectType from Project where ProjectKey = '<ProjectKey>' and validate = 1";
            sql = sql.Replace("<ProjectKey>", key);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                var ret = new ProjectViewModels(Convert.ToString(dbret[0][0])
                    , Convert.ToString(dbret[0][1]), Convert.ToString(dbret[0][2])
                    , Convert.ToDouble(dbret[0][3]), Convert.ToString(dbret[0][4])
                    , Convert.ToString(dbret[0][5]), Convert.ToString(dbret[0][6])
                    , Convert.ToString(dbret[0][7]));

                ret.MemberList = RetrieveProjectMembers(key);
                ret.TabList = RetrieveProjectMesTable(key);
                ret.PNList = RetrieveProjectPn(key);
                ret.StationList = RetrieveProjectStation(key);
                ret.MDIDList = RetrieveProjectModelID(key);
                ret.SumDatasetList = RetrieveProjectSumDataSet(key);
                return ret;
            }
            else
                return null;

        }

        public static List<ProjectViewModels> RetrieveAllProject()
        {
            var ret = new List<ProjectViewModels>();
            var keys = RetrieveAllProjectKey();
            foreach (var key in keys)
            {
                var r = RetrieveOneProject(key);
                if (r != null)
                {
                    ret.Add(r);
                }
            }
            return ret;
        }
    }

    }
