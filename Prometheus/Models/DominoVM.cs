﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Domino.Models
{
    public class DominoCardStatus
    {
        public static string pending = "pending";
        public static string working = "working";
        public static string done = "done";
        public static string info = "info";
    }

    public class DominoCardType
    {
        public static string ECOPending = "ECOPending";
        public static string ECOSignoff1 = "ECOSignoff1";
        public static string ECOComplete = "ECOComplete";
        public static string SampleOrdering = "SampleOrdering";
        public static string SampleBuilding = "SampleBuilding";
        public static string SampleShipment = "SampleShipment";
        public static string SampleCustomerApproval = "SampleCustomerApproval";
        public static string MiniPIPComplete = "MiniPIPComplete";
        public static string FACustomerApproval = "FACustomerApproval";
        public static string ECOSignoff2 = "ECOSignoff2";
        public static string CustomerApprovalHold = "CustomerApprovalHold";
    }

    public class DominoECOType
    {
        public static string DVS = "Default Vs Sample";
        public static string DVNS = "Default Vs Non-Sample";
        public static string RVS = "Revise Vs Sample";
        public static string RVNS = "Revise Vs Non-Sample";
    }

    public class DominoFlowInfo
    {
        public static string Default = "Default";
        public static string Revise = "Revise";
    }

    public class DominoPNImplement
    {
        public static string NA = "N/A";
        public static string Roll = "Rolling Change/No Purge";
        public static string CutOverImm = "Cut-Over with Stop-Ship Immediately Upon Mini-PIP Submission";
        public static string CutOverAft = "Cut-Over with Stop-Ship After New First-Article Sample Approval";
    }

    public class DominoYESNO
    {
        public static string YES = "YES";
        public static string NO = "NO";
    }

    public class DominoFACategory
    {
        public static string EEPROMFA = "EEPROM FA";
        public static string LABELFA = "LABEL FA";
        public static string LABELEEPROMFA = "LABEL/EEPROM FA";
    }


    public class ECOCardComments
    {
        public string CardKey { set; get; }

        private string sComment = "";
        public string Comment
        {
            set { sComment = value; }
            get { return sComment; }
        }

        public string dbComment
        {
            get
            {
                if (string.IsNullOrEmpty(sComment))
                {
                    return "";
                }
                else
                {
                    try
                    {
                        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sComment));
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
                    sComment = "";
                }
                else
                {
                    try
                    {
                        sComment = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    }
                    catch (Exception)
                    {
                        sComment = "";
                    }

                }

            }
        }

        public string Reporter { set; get; }

        public DateTime CommentDate { set; get; }

        public string CommentType { set; get; }
    }

    public class MiniPIPOrdeInfo
    {
        public string CardKey { set; get;}
        public string OrderType { set; get; }
        public string OrderDate { set; get; }
        public string SONum { set; get; }
        public string Item { set; get; }
        public string Description { set; get; }
        public string QTY { set; get; }
        public string SSD { set; get; }
        public string Planner { set; get; }
        public string LineID { set; get; }
    }

    public class ECOBaseInfo
    {
        public string ECOKey { set; get; }
        public string ECONum { set; get; }
        public string ECOType { set; get; }
        public string FirstArticleNeed { set; get; }
        public string FlowInfo { set; get; }
        public string PNDesc { set; get; }
        public string Customer { set; get; }
        public string Complex { set; get; }
        public string RSM { set; get; }
        public string PE { set; get; }
        public string RiskBuild { set; get; }
        public string InitRevison { set; get; }
        public string FinalRevison { set; get; }
        public string TLAAvailable { set; get; }
        public string OpsEntry { set; get; }
        public string TestModification { set; get; }
        public string ECOSubmit { set; get; }
        public string ECOReviewSignoff { set; get; }
        public string ECOCCBSignoff { set; get; }
        public string QTRInit { set; get; }
        public string MCOIssued { set; get; }
        public string PNImplement { set; get; }

        private Dictionary<string, string> namedict = new Dictionary<string, string>();
        public Dictionary<string, string> NameDict { get { return namedict; } }
        public ECOBaseInfo()
        {
            namedict.Clear();
            namedict.Add("ECONum", "ECO Number");
            namedict.Add("PNDesc", "Product Requested");
            namedict.Add("Customer", "Customer");
            namedict.Add("Complex", "Complex");
            namedict.Add("RSM", "RSM");
            namedict.Add("PE", "PE");
            namedict.Add("RiskBuild", "Risk Build");
            namedict.Add("InitRevison", "Initial Mini-PIP Received");
            namedict.Add("FinalRevison", "Final Mini-PIP Revision");
            namedict.Add("TLAAvailable", "TLA Available");
            namedict.Add("OpsEntry", "Ops Log Entry");
            namedict.Add("TestModification", "Test Modification");
            namedict.Add("ECOSubmit", "ECO Submit");
            namedict.Add("ECOReviewSignoff", "ECO TECH Review");
            namedict.Add("ECOCCBSignoff", "ECO CCB Signoff");
            namedict.Add("QTRInit", "QTR Initiation");
            namedict.Add("MCOIssued", "MCO Issued");
            namedict.Add("ECOType", "MiniPIP Flow");
            namedict.Add("FirstArticleNeed", "Order Info");

            ECONum = string.Empty;
            PNDesc = string.Empty;
            Customer = string.Empty;
            Complex = string.Empty;
            RSM = string.Empty;
            PE = string.Empty;
            RiskBuild = string.Empty;
            InitRevison = string.Empty;
            FinalRevison = string.Empty;
            TLAAvailable = string.Empty;
            OpsEntry = string.Empty;
            TestModification = string.Empty;
            ECOSubmit = string.Empty;
            ECOReviewSignoff = string.Empty;
            ECOCCBSignoff = string.Empty;
            QTRInit = string.Empty;
            ECOType = string.Empty;
            MCOIssued = string.Empty;
            FirstArticleNeed = string.Empty;
            FlowInfo = string.Empty;
            PNImplement = string.Empty;
        }

        public static List<KeyValuePair<string, string>> RetrieveBaseInfo(ECOBaseInfo info)
        {
            var ret = new List<KeyValuePair<string, string>>();
            PropertyInfo[] properties = typeof(ECOBaseInfo).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (info.NameDict.ContainsKey(property.Name))
                {
                    var val = string.Empty;
                    try { val = Convert.ToString(property.GetValue(info)); }
                    catch (Exception ex) { val = string.Empty; }

                    if (!string.IsNullOrEmpty(val)
                        && string.Compare(val,"N/A",true) != 0)
                    {
                        ret.Add(new KeyValuePair<string,string>(info.NameDict[property.Name], val));
                    }
                }//end if

            }//end foreach

            return ret;
        }

        public void SetDefaultDateValue()
        {
            if (string.IsNullOrEmpty(InitRevison))
                InitRevison = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(FinalRevison))
                FinalRevison = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(TLAAvailable))
                TLAAvailable = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(OpsEntry))
                OpsEntry = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(TestModification))
                TestModification = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(ECOSubmit))
                ECOSubmit = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(ECOReviewSignoff))
                ECOReviewSignoff = "1982-05-06 10:00:00";
            if (string.IsNullOrEmpty(ECOCCBSignoff))
                ECOCCBSignoff = "1982-05-06 10:00:00";
        }

        public void CreateECO()
        {
            SetDefaultDateValue();

            var sql = "insert into ECOBaseInfo(ECOKey,ECONum,ECOType,PNDesc,Customer,Complex,RSM,PE,RiskBuild,InitRevison,FinalRevison"
                + ",TLAAvailable,OpsEntry,TestModification,ECOSubmit,ECOReviewSignoff,ECOCCBSignoff,QTRInit,MCOIssued,FirstArticleNeed,FlowInfo,PNImplement)"
                + " values('<ECOKey>','<ECONum>','<ECOType>','<PNDesc>','<Customer>','<Complex>','<RSM>','<PE>','<RiskBuild>','<InitRevison>','<FinalRevison>'"
                + ",'<TLAAvailable>','<OpsEntry>','<TestModification>','<ECOSubmit>','<ECOReviewSignoff>','<ECOCCBSignoff>','<QTRInit>','<MCOIssued>','<FirstArticleNeed>','<FlowInfo>','<PNImplement>')";


            sql = sql.Replace("<ECOKey>", ECOKey).Replace("<ECONum>", ECONum).Replace("<ECOType>", ECOType).Replace("<PNDesc>", PNDesc).Replace("<Customer>", Customer)
                .Replace("<Complex>", Complex).Replace("<RSM>", RSM).Replace("<PE>", PE).Replace("<RiskBuild>", RiskBuild).Replace("<InitRevison>", InitRevison)
                .Replace("<FinalRevison>", FinalRevison).Replace("<TLAAvailable>", TLAAvailable).Replace("<OpsEntry>", OpsEntry).Replace("<TestModification>", TestModification)
                .Replace("<ECOSubmit>", ECOSubmit).Replace("<ECOReviewSignoff>", ECOReviewSignoff).Replace("<ECOCCBSignoff>", ECOCCBSignoff)
                .Replace("<QTRInit>", QTRInit).Replace("<MCOIssued>", MCOIssued).Replace("<FirstArticleNeed>", FirstArticleNeed).Replace("<FlowInfo>", FlowInfo).Replace("<PNImplement>", PNImplement);

            DBUtility.ExeLocalSqlNoRes(sql);
        }


        public void UpdateECO()
        {
            SetDefaultDateValue();

            var sql = "update ECOBaseInfo set ECONum='<ECONum>',ECOType='<ECOType>',PNDesc='<PNDesc>',Customer='<Customer>',Complex='<Complex>',RSM='<RSM>',PE='<PE>',RiskBuild='<RiskBuild>',InitRevison='<InitRevison>',FinalRevison='<FinalRevison>'"
                + ",TLAAvailable='<TLAAvailable>',OpsEntry='<OpsEntry>',TestModification='<TestModification>',ECOSubmit='<ECOSubmit>',ECOReviewSignoff='<ECOReviewSignoff>',ECOCCBSignoff='<ECOCCBSignoff>',QTRInit='<QTRInit>',MCOIssued='<MCOIssued>',FirstArticleNeed='<FirstArticleNeed>',FlowInfo='<FlowInfo>',PNImplement='<PNImplement>' where ECOKey='<ECOKey>'";
            
            sql = sql.Replace("<ECOKey>", ECOKey).Replace("<ECONum>", ECONum).Replace("<ECOType>", ECOType).Replace("<PNDesc>", PNDesc).Replace("<Customer>", Customer)
                .Replace("<Complex>", Complex).Replace("<RSM>", RSM).Replace("<PE>", PE).Replace("<RiskBuild>", RiskBuild).Replace("<InitRevison>", InitRevison)
                .Replace("<FinalRevison>", FinalRevison).Replace("<TLAAvailable>", TLAAvailable).Replace("<OpsEntry>", OpsEntry).Replace("<TestModification>", TestModification)
                .Replace("<ECOSubmit>", ECOSubmit).Replace("<ECOReviewSignoff>", ECOReviewSignoff).Replace("<ECOCCBSignoff>", ECOCCBSignoff).Replace("<QTRInit>", QTRInit)
                .Replace("<MCOIssued>", MCOIssued).Replace("<FirstArticleNeed>", FirstArticleNeed).Replace("<FlowInfo>", FlowInfo).Replace("<PNImplement>", PNImplement);

            DBUtility.ExeLocalSqlNoRes(sql);
        }

        private static string ConvertToDate(object obj)
        {
            try
            {
                var date = DateTime.Parse(Convert.ToString(obj));
                if (string.Compare("1982-05-06",date.ToString("yyyy-MM-dd")) == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return date.ToString("MM/dd/yyyy");
                }
            }
            catch (Exception ex) { return string.Empty; }
        }

        public static List<ECOBaseInfo> RetrieveAllECOBaseInfo()
        {
            var ret = new List<ECOBaseInfo>();
            var sql = "select ECOKey,ECONum,ECOType,PNDesc,Customer,Complex,RSM,PE,RiskBuild,InitRevison,FinalRevison"
                + ",TLAAvailable,OpsEntry,TestModification,ECOSubmit,ECOReviewSignoff,ECOCCBSignoff,QTRInit,MCOIssued,FirstArticleNeed,FlowInfo,PNImplement from ECOBaseInfo";

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempitem = new ECOBaseInfo();
                tempitem.ECOKey = Convert.ToString(line[0]);
                tempitem.ECONum = Convert.ToString(line[1]);
                tempitem.ECOType = Convert.ToString(line[2]);
                tempitem.PNDesc = Convert.ToString(line[3]);
                tempitem.Customer = Convert.ToString(line[4]);
                tempitem.Complex = Convert.ToString(line[5]);
                tempitem.RSM = Convert.ToString(line[6]);
                tempitem.PE = Convert.ToString(line[7]);
                tempitem.RiskBuild = Convert.ToString(line[8]);

                tempitem.InitRevison = ConvertToDate(line[9]);
                tempitem.FinalRevison = ConvertToDate(line[10]);
                tempitem.TLAAvailable = ConvertToDate(line[11]);
                tempitem.OpsEntry = ConvertToDate(line[12]);
                tempitem.TestModification = ConvertToDate(line[13]);
                tempitem.ECOSubmit = ConvertToDate(line[14]);
                tempitem.ECOReviewSignoff = ConvertToDate(line[15]);
                tempitem.ECOCCBSignoff = ConvertToDate(line[16]);

                tempitem.QTRInit = Convert.ToString(line[17]);
                tempitem.MCOIssued = Convert.ToString(line[18]);
                tempitem.FirstArticleNeed = Convert.ToString(line[19]);
                tempitem.FlowInfo = Convert.ToString(line[20]);
                tempitem.PNImplement = Convert.ToString(line[21]);

                ret.Add(tempitem);
            }

            return ret;
        }

        public static List<ECOBaseInfo> RetrieveECOBaseInfo(string ECOKey)
        {
            var ret = new List<ECOBaseInfo>();
            var sql = "select ECOKey,ECONum,ECOType,PNDesc,Customer,Complex,RSM,PE,RiskBuild,InitRevison,FinalRevison"
                + ",TLAAvailable,OpsEntry,TestModification,ECOSubmit,ECOReviewSignoff,ECOCCBSignoff,QTRInit,MCOIssued,FirstArticleNeed,FlowInfo,PNImplement from ECOBaseInfo where ECOKey='<ECOKey>'";
            sql = sql.Replace("<ECOKey>", ECOKey);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempitem = new ECOBaseInfo();
                tempitem.ECOKey = Convert.ToString(line[0]);
                tempitem.ECONum = Convert.ToString(line[1]);
                tempitem.ECOType = Convert.ToString(line[2]);
                tempitem.PNDesc = Convert.ToString(line[3]);
                tempitem.Customer = Convert.ToString(line[4]);
                tempitem.Complex = Convert.ToString(line[5]);
                tempitem.RSM = Convert.ToString(line[6]);
                tempitem.PE = Convert.ToString(line[7]);
                tempitem.RiskBuild = Convert.ToString(line[8]);

                tempitem.InitRevison = ConvertToDate(line[9]);
                tempitem.FinalRevison = ConvertToDate(line[10]);
                tempitem.TLAAvailable = ConvertToDate(line[11]);
                tempitem.OpsEntry = ConvertToDate(line[12]);
                tempitem.TestModification = ConvertToDate(line[13]);
                tempitem.ECOSubmit = ConvertToDate(line[14]);
                tempitem.ECOReviewSignoff = ConvertToDate(line[15]);
                tempitem.ECOCCBSignoff = ConvertToDate(line[16]);

                tempitem.QTRInit = Convert.ToString(line[17]);
                tempitem.MCOIssued = Convert.ToString(line[18]);
                tempitem.FirstArticleNeed = Convert.ToString(line[19]);
                tempitem.FlowInfo = Convert.ToString(line[20]);
                tempitem.PNImplement = Convert.ToString(line[21]);

                ret.Add(tempitem);
            }

            return ret;
        }


    }

    public class DominoVM
    {
        public ECOBaseInfo EBaseInfo { set; get; }
        public string ECOKey { set; get; }
        public string CardKey { set; get; }
        public string CardNo { set; get; }
        public string CardContent { set; get; }
        public string CardStatus { set; get; }
        public string CardType { set; get; }
        
        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        private List<string> attachlist = new List<string>();
        public List<string> AttachList
        {
            set
            {
                attachlist.Clear();
                attachlist.AddRange(value);
            }
            get
            {
                return attachlist;
            }
        }

        private List<ECOCardComments> cemlist = new List<ECOCardComments>();
        public List<ECOCardComments> CommentList
        {
            set
            {
                cemlist.Clear();
                cemlist.AddRange(value);
            }

            get
            {
                return cemlist;
            }
        }

        private Dictionary<string, string> ecocontentdict = new Dictionary<string, string>();
        public Dictionary<string, string> ECOContentDict { get { return ecocontentdict; } }
        public DominoVM()
        {
            ecocontentdict.Clear();
            ecocontentdict.Add(DominoCardType.ECOPending,"ECO Pending");
            ecocontentdict.Add(DominoCardType.ECOSignoff1,"ECO Signoff-1");
            ecocontentdict.Add(DominoCardType.ECOComplete,"ECO Complete");
            ecocontentdict.Add(DominoCardType.SampleOrdering,"Sample Ordering");
            ecocontentdict.Add(DominoCardType.SampleBuilding,"Sample Building");
            ecocontentdict.Add(DominoCardType.SampleShipment,"Sample Shipment");
            ecocontentdict.Add(DominoCardType.SampleCustomerApproval,"Sample Customer Approval");
            ecocontentdict.Add(DominoCardType.MiniPIPComplete,"Mini PIP Complete");
            ecocontentdict.Add(DominoCardType.FACustomerApproval,"FA Customer Approval");
            ecocontentdict.Add(DominoCardType.ECOSignoff2,"ECO Signoff-2");
            ecocontentdict.Add(DominoCardType.CustomerApprovalHold,"Customer Approval Hold");

            MiniPIPWeeklyUpdate = string.Empty;
            MiniPIPHold = string.Empty;
            WeeklyUpdateTime = string.Empty;

            QAEEPROMCheck = string.Empty;
            QALabelCheck = string.Empty;

            PeerReviewEngineer = string.Empty;
            PeerReview = string.Empty;

            ECOAttachmentCheck = string.Empty;
            ECOQRFile = string.Empty;
            EEPROMPeerReview = string.Empty;
            ECOTraceview = string.Empty;
            SpecCompresuite = string.Empty;

            ECOTRApprover = string.Empty;
            ECOMDApprover = string.Empty;

            MiniPVTCheck = string.Empty;
            AgileCodeFile = string.Empty;
            AgileSpecFile = string.Empty;
            AgileTestFile = string.Empty;

            FACategory = string.Empty;
            RSMSendDate = string.Empty;
            RSMApproveDate = string.Empty;

            CustomerApproveHoldDate = string.Empty;

            ECOCompleted = string.Empty;
            ECOSubmitDate = string.Empty;
            ECOTRApprovedDate = string.Empty;
            ECOCCBApprovedDate = string.Empty;
            ECOCompleteDate = string.Empty;
        }

        public static void CleanDB()
        {
            var sql = "delete from ECOBaseInfo";
            DBUtility.ExeLocalSqlNoRes(sql);
            sql = "delete from ECOCard";
            DBUtility.ExeLocalSqlNoRes(sql);
            sql = "delete from ECOCardContent";
            DBUtility.ExeLocalSqlNoRes(sql);
            sql = "delete from ECOCardComment";
            DBUtility.ExeLocalSqlNoRes(sql);
            sql = "delete from ECOCardAttachment";
            DBUtility.ExeLocalSqlNoRes(sql);
        }


        public static string CreateCard(string ECOKey,string NewCardKey,string CardType,string CardStatus)
        {
            var cardexist = DominoVM.RetrieveSpecialCard(ECOKey, CardType);
            if (cardexist.Count == 0)
            {
                var sql = "insert into ECOCard(ECOKey,CardKey,CardType,CardStatus,CardCreateTime) values('<ECOKey>','<CardKey>','<CardType>','<CardStatus>','<CardCreateTime>')";
                sql = sql.Replace("<ECOKey>", ECOKey).Replace("<CardKey>", NewCardKey).Replace("<CardType>", CardType).Replace("<CardStatus>", CardStatus).Replace("<CardCreateTime>", DateTime.Now.ToString());
                DBUtility.ExeLocalSqlNoRes(sql);
                return NewCardKey;
            }
            else
            {
                return cardexist[0].CardKey;
            }
        }

        public static List<DominoVM> RetrieveECOCards(ECOBaseInfo baseinfo)
        {
            var ret = new List<DominoVM>();

            var sql = "select ECOKey,CardKey,CardType,CardStatus from ECOCard where ECOKey = '<ECOKey>' and DeleteMark <> 'true' order by CardCreateTime ASC";
            sql = sql.Replace("<ECOKey>", baseinfo.ECOKey);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var idx = 1;
            foreach (var line in dbret)
            {
                var tempitem = new DominoVM();
                tempitem.ECOKey = Convert.ToString(line[0]);
                tempitem.CardKey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.CardKey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.CardKey);

                ret.Add(tempitem);

                idx = idx + 1;
            }

            return ret;
        }

        public static List<DominoVM> RetrieveSpecialCard(ECOBaseInfo baseinfo,string CardType)
        {
            var ret = new List<DominoVM>();

            var sql = "select ECOKey,CardKey,CardType,CardStatus from ECOCard where ECOKey = '<ECOKey>' and CardType = '<CardType>' and DeleteMark <> 'true' order by CardCreateTime ASC";
            sql = sql.Replace("<ECOKey>", baseinfo.ECOKey).Replace("<CardType>", CardType);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var idx = 1;
            foreach (var line in dbret)
            {
                var tempitem = new DominoVM();
                tempitem.ECOKey = Convert.ToString(line[0]);
                tempitem.CardKey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.CardKey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.CardKey);

                ret.Add(tempitem);

                idx = idx + 1;
            }

            return ret;
        }

        private static List<DominoVM> RetrieveSpecialCard(string ECOKey, string CardType)
        {
            var ret = new List<DominoVM>();

            var sql = "select ECOKey,CardKey,CardType,CardStatus from ECOCard where ECOKey = '<ECOKey>' and CardType = '<CardType>' and DeleteMark <> 'true' order by CardCreateTime ASC";
            sql = sql.Replace("<ECOKey>", ECOKey).Replace("<CardType>", CardType);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var idx = 1;
            foreach (var line in dbret)
            {
                var tempitem = new DominoVM();
                tempitem.ECOKey = Convert.ToString(line[0]);
                tempitem.CardKey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                //tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.CardKey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.CardKey);

                ret.Add(tempitem);

                idx = idx + 1;
            }

            return ret;
        }


        public static List<DominoVM> RetrieveCard( string CardKey)
        {
            var ret = new List<DominoVM>();

            var sql = "select ECOKey,CardKey,CardType,CardStatus from ECOCard where CardKey = '<CardKey>' and DeleteMark <> 'true' order by CardCreateTime ASC";
            sql = sql.Replace("<CardKey>", CardKey);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var idx = 1;
            foreach (var line in dbret)
            {
                var tempitem = new DominoVM();
                tempitem.ECOKey = Convert.ToString(line[0]);
                tempitem.CardKey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                //tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.CardKey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.CardKey);

                ret.Add(tempitem);

                idx = idx + 1;
            }

            return ret;
        }


        public static void RollBack2This(string ECOKey, string CardKey)
        {
            var sql = "select CardCreateTime from ECOCard where ECOKey = '<ECOKey>' and CardKey = '<CardKey>'";
            sql = sql.Replace("<ECOKey>", ECOKey).Replace("<CardKey>", CardKey);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                sql = "update ECOCard set DeleteMark = 'true' where ECOKey = '<ECOKey>' and CardCreateTime > '<CardCreateTime>'";
                sql = sql.Replace("<ECOKey>", ECOKey).Replace("<CardCreateTime>", Convert.ToString(dbret[0][0]));
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        public static void UpdateCardStatus(string CardKey, string CardStatus)
        {
            var sql = "update ECOCard set CardStatus = '<CardStatus>' where CardKey='<CardKey>'";
            sql = sql.Replace("<CardKey>", CardKey).Replace("<CardStatus>", CardStatus);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void StoreCardComment(string CardKey, string dbComment, string Reporter, string CommentDate)
        {
            var sql = "insert into ECOCardComment(CardKey,Comment,Reporter,CommentDate) values('<CardKey>','<Comment>','<Reporter>','<CommentDate>')";
            sql = sql.Replace("<CardKey>", CardKey).Replace("<Comment>", dbComment)
                .Replace("<Reporter>", Reporter).Replace("<CommentDate>", CommentDate);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        private static List<ECOCardComments> RetrieveCardComments(string CardKey)
        {
            var ret = new List<ECOCardComments>();
            var sql = "select CardKey,Comment,Reporter,CommentDate from ECOCardComment where CardKey = '<CardKey>' and DeleteMark <> 'true' order by CommentDate ASC";
            sql = sql.Replace("<CardKey>", CardKey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var r in dbret)
            {
                var tempcomment = new ECOCardComments();
                tempcomment.CardKey = Convert.ToString(r[0]);
                tempcomment.dbComment = Convert.ToString(r[1]);
                tempcomment.Reporter = Convert.ToString(r[2]);
                tempcomment.CommentDate = DateTime.Parse(Convert.ToString(r[3]));
                ret.Add(tempcomment);
            }
            return ret;
        }

        public static void DeleteCardComment(string CardKey, string Date)
        {
            var sql = "update ECOCardComment set DeleteMark = 'true' where CardKey='<CardKey>' and CommentDate='<CommentDate>'";
            sql = sql.Replace("<CardKey>", CardKey).Replace("<CommentDate>", Date);
            DBUtility.ExeLocalSqlNoRes(sql);
        }


        public static void StoreCardAttachment(string CardKey, string attachmenturl)
        {
            var sql = "insert into ECOCardAttachment(CardKey,Attachment,UpdateTime) values('<CardKey>','<Attachment>','<UpdateTime>')";
            sql = sql.Replace("<CardKey>", CardKey).Replace("<Attachment>", attachmenturl).Replace("<UpdateTime>", DateTime.Now.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<string> RetrieveCardAttachment(string CardKey)
        {
            var ret = new List<string>();
            var csql = "select Attachment from ECOCardAttachment where CardKey = '<CardKey>' and DeleteMark <> 'true' order by UpdateTime ASC";
            csql = csql.Replace("<CardKey>", CardKey);

            var cdbret = DBUtility.ExeLocalSqlWithRes(csql);
            foreach (var r in cdbret)
            {
                ret.Add(Convert.ToString(r[0]));
            }
            return ret;
        }

        public static List<string> RetrieveCardExistedAttachment(string CardKey)
        {
            var ret = new List<string>();
            var csql = "select Attachment from ECOCardAttachment where CardKey = '<CardKey>' order by UpdateTime ASC";
            csql = csql.Replace("<CardKey>", CardKey);

            var cdbret = DBUtility.ExeLocalSqlWithRes(csql);
            foreach (var r in cdbret)
            {
                ret.Add(Convert.ToString(r[0]));
            }
            return ret;
        }

        public static void DeleteCardAttachment(string CardKey, string fn)
        {
            var csql = "update ECOCardAttachment set DeleteMark = 'true' where CardKey = '<CardKey>' and Attachment like '%<cond>%'";
            csql = csql.Replace("<CardKey>", CardKey).Replace("<cond>", fn);
            DBUtility.ExeLocalSqlNoRes(csql);
        }

        public string MiniPIPWeeklyUpdate { set; get; }
        public string MiniPIPHold { set; get; }
        public string WeeklyUpdateTime { set; get; }

        public static void StoreECOPendingInfo(string cardkey,string hold)
        {
            var sql = "delete from ECOCardContent where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into ECOCardContent(CardKey,APVal2) values('<CardKey>','<APVal2>')";
            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal2>", hold);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void UpdateECOPendingInfo_UpdateInfo(string cardkey, string weeklyupdate)
        {
            var sql = "Update ECOCardContent Set APVal1 = '<APVal1>',APVal3='<APVal3>'  where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal1>", weeklyupdate).Replace("<APVal3>", DateTime.Now.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void UpdateECOPendingInfo(string cardkey, string ecohold)
        {
            var sql = "Update ECOCardContent Set APVal2 = '<APVal2>'  where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal2>", ecohold);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static DominoVM RetrieveECOPendingInfo(string cardkey)
        {
            var ret = new DominoVM();
            var sql = "select CardKey,APVal1,APVal2,APVal3 from ECOCardContent where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                ret.CardKey = Convert.ToString(dbret[0][0]);
                ret.MiniPIPWeeklyUpdate = Convert.ToString(dbret[0][1]);
                ret.MiniPIPHold = Convert.ToString(dbret[0][2]);
                ret.WeeklyUpdateTime = Convert.ToString(dbret[0][3]);
            }
            return ret;
        }




        public string QAEEPROMCheck { set; get; }
        public string QALabelCheck { set; get; }

        public string PeerReviewEngineer { set; get; }
        public string PeerReview { set; get; }

        public string ECOAttachmentCheck { set; get; }
        public string ECOQRFile { set; get; }
        public string EEPROMPeerReview { set; get; }
        public string ECOTraceview { set; get; }
        public string SpecCompresuite { set; get; }
        
        public string ECOTRApprover { set; get; }
        public string ECOMDApprover { set; get; }

        public string MiniPVTCheck { set; get; }
        public string AgileCodeFile { set; get; }
        public string AgileSpecFile { set; get; }
        public string AgileTestFile { set; get; }
        
        public string FACategory { set; get; }
        public string RSMSendDate { set; get; }
        public string RSMApproveDate { set; get; }

        public void UpdateSignoffInfo(string cardkey)
        {
            var infoexist = RetrieveSignoffInfo(cardkey);

            if (string.IsNullOrEmpty(infoexist.CardKey))
            {
                var csql = "insert into ECOCardContent(CardKey,APVal1) values('<CardKey>','<APVal1>')";
                csql = csql.Replace("<CardKey>", cardkey).Replace("<APVal1>", string.Empty);
                DBUtility.ExeLocalSqlNoRes(csql);
            }

            var sql = "Update ECOCardContent Set APVal1 = '<APVal1>',APVal2 = '<APVal2>',APVal3 = '<APVal3>',APVal4 = '<APVal4>',APVal5 = '<APVal5>',APVal6 = '<APVal6>'"
                +",APVal7 = '<APVal7>',APVal8 = '<APVal8>',APVal9 = '<APVal9>',APVal10 = '<APVal10>',APVal11 = '<APVal11>',APVal12 = '<APVal12>',APVal13 = '<APVal13>'"
                + ",APVal14 = '<APVal14>',APVal15 = '<APVal15>',APVal16 = '<APVal16>',APVal17 = '<APVal17>',APVal18 = '<APVal18>' where CardKey = '<CardKey>'";

            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal1>", QAEEPROMCheck).Replace("<APVal2>", QALabelCheck).Replace("<APVal3>", PeerReviewEngineer)
                .Replace("<APVal4>", PeerReview).Replace("<APVal5>", ECOAttachmentCheck).Replace("<APVal6>", ECOQRFile).Replace("<APVal7>", EEPROMPeerReview)
                .Replace("<APVal8>", ECOTraceview).Replace("<APVal9>", SpecCompresuite).Replace("<APVal10>", ECOTRApprover).Replace("<APVal11>", ECOMDApprover)
                .Replace("<APVal12>", MiniPVTCheck).Replace("<APVal13>", AgileCodeFile).Replace("<APVal14>", AgileSpecFile).Replace("<APVal15>", AgileTestFile)
                .Replace("<APVal16>", FACategory).Replace("<APVal17>", RSMSendDate).Replace("<APVal18>", RSMApproveDate);

            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static DominoVM RetrieveSignoffInfo(string cardkey)
        {
            var ret = new DominoVM();
            var sql = "select CardKey,APVal1,APVal2,APVal3,APVal4,APVal5,APVal6,APVal7,APVal8,APVal9,APVal10,APVal11,APVal12,APVal13,APVal14,APVal15,APVal16,APVal17,APVal18 from ECOCardContent where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                ret.CardKey = Convert.ToString(dbret[0][0]);
                ret.QAEEPROMCheck = Convert.ToString(dbret[0][1]);
                ret.QALabelCheck = Convert.ToString(dbret[0][2]);
                ret.PeerReviewEngineer = Convert.ToString(dbret[0][3]);
                ret.PeerReview = Convert.ToString(dbret[0][4]);
                ret.ECOAttachmentCheck = Convert.ToString(dbret[0][5]);
                ret.ECOQRFile = Convert.ToString(dbret[0][6]);
                ret.EEPROMPeerReview = Convert.ToString(dbret[0][7]);
                ret.ECOTraceview = Convert.ToString(dbret[0][8]);
                ret.SpecCompresuite = Convert.ToString(dbret[0][9]);
                ret.ECOTRApprover = Convert.ToString(dbret[0][10]);
                ret.ECOMDApprover = Convert.ToString(dbret[0][11]);
                ret.MiniPVTCheck = Convert.ToString(dbret[0][12]);
                ret.AgileCodeFile = Convert.ToString(dbret[0][13]);
                ret.AgileSpecFile = Convert.ToString(dbret[0][14]);
                ret.AgileTestFile = Convert.ToString(dbret[0][15]);
                ret.FACategory = Convert.ToString(dbret[0][16]);
                ret.RSMSendDate = Convert.ToString(dbret[0][17]);
                ret.RSMApproveDate = Convert.ToString(dbret[0][18]);
            }
            return ret;
        }

        public string CustomerApproveHoldDate { set; get; }

        public void UpdateCustomerApproveHoldInfo(string cardkey)
        {

            var infoexist = RetrieveCustomerApproveHoldInfo(cardkey);

            if (string.IsNullOrEmpty(infoexist.CardKey))
            {
                var csql = "insert into ECOCardContent(CardKey,APVal1) values('<CardKey>','<APVal1>')";
                csql = csql.Replace("<CardKey>", cardkey).Replace("<APVal1>", string.Empty);
                DBUtility.ExeLocalSqlNoRes(csql);
            }

            var sql = "Update ECOCardContent Set APVal1 = '<APVal1>' where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal1>", CustomerApproveHoldDate);
            DBUtility.ExeLocalSqlNoRes(sql);
        }


        public static DominoVM RetrieveCustomerApproveHoldInfo(string cardkey)
        {
            var ret = new DominoVM();
            var sql = "select CardKey,APVal1 from ECOCardContent where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                ret.CardKey = Convert.ToString(dbret[0][0]);
                ret.CustomerApproveHoldDate = Convert.ToString(dbret[0][1]);
            }
            return ret;
        }


        public string ECOCompleted { set; get; }
        public string ECOSubmitDate { set; get; }
        public string ECOTRApprovedDate { set; get; }
        public string ECOCCBApprovedDate { set; get; }
        public string ECOCompleteDate { set; get; }

        public void UpdateECOCompleteInfo(string cardkey)
        {
            var infoexist = RetrieveECOCompleteInfo(cardkey);

            if (string.IsNullOrEmpty(infoexist.CardKey))
            {
                var csql = "insert into ECOCardContent(CardKey,APVal1) values('<CardKey>','<APVal1>')";
                csql = csql.Replace("<CardKey>", cardkey).Replace("<APVal1>", string.Empty);
                DBUtility.ExeLocalSqlNoRes(csql);
            }

            var sql = "Update ECOCardContent Set APVal1 = '<APVal1>',APVal2 = '<APVal2>',APVal3 = '<APVal3>',APVal4 = '<APVal4>',APVal5 = '<APVal5>' where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal1>", ECOCompleted).Replace("<APVal2>", ECOSubmitDate)
                .Replace("<APVal3>", ECOTRApprovedDate).Replace("<APVal4>", ECOCCBApprovedDate).Replace("<APVal5>", ECOCompleteDate);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static DominoVM RetrieveECOCompleteInfo(string cardkey)
        {
            var ret = new DominoVM();
            var sql = "select CardKey,APVal1,APVal2,APVal3,APVal4,APVal5 from ECOCardContent where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                ret.CardKey = Convert.ToString(dbret[0][0]);
                ret.ECOCompleted = Convert.ToString(dbret[0][1]);
                ret.ECOSubmitDate = Convert.ToString(dbret[0][2]);
                ret.ECOTRApprovedDate = Convert.ToString(dbret[0][3]);
                ret.ECOCCBApprovedDate = Convert.ToString(dbret[0][4]);
                ret.ECOCompleteDate = Convert.ToString(dbret[0][5]);
            }
            return ret;
        }


        private List<MiniPIPOrdeInfo> ordtab = new List<MiniPIPOrdeInfo>();
        public List<MiniPIPOrdeInfo> OrderTable
        {
            set {
                ordtab.Clear();
                ordtab.AddRange(value);
            }
            get { return ordtab; }
        }

        public static void UpdateOrderInfo(List<MiniPIPOrdeInfo> ordinfos, string cardkey)
        {
            foreach (var ordinfo in ordinfos)
            {
                var dataexist = RetrieverOrderInfo(cardkey, ordinfo.LineID);
                if (!string.IsNullOrEmpty(dataexist.CardKey))
                {
                    continue;
                }

                var csql = "insert into ECOCardContent(CardKey,APVal1,APVal2,APVal3,APVal4,APVal5,APVal6,APVal7,APVal8,APVal9)  "
                    + "  values('<CardKey>','<APVal1>','<APVal2>','<APVal3>','<APVal4>','<APVal5>','<APVal6>','<APVal7>','<APVal8>','<APVal9>')";
                csql = csql.Replace("<CardKey>", cardkey).Replace("<APVal1>",ordinfo.OrderType).Replace("<APVal2>", ordinfo.OrderDate).Replace("<APVal3>", ordinfo.SONum)
                    .Replace("<APVal4>", ordinfo.Item).Replace("<APVal5>", ordinfo.Description).Replace("<APVal6>", ordinfo.QTY)
                    .Replace("<APVal7>", ordinfo.SSD).Replace("<APVal8>", ordinfo.Planner).Replace("<APVal9>", ordinfo.LineID);
                DBUtility.ExeLocalSqlNoRes(csql);
            }
        }

        public static List<MiniPIPOrdeInfo> RetrieveOrderInfo(string cardkey)
        {
            var ret = new List<MiniPIPOrdeInfo>();
            
            var sql = "select CardKey,APVal1,APVal2,APVal3,APVal4,APVal5,APVal6,APVal7,APVal8,APVal9 from ECOCardContent where CardKey = '<CardKey>'";
            sql = sql.Replace("<CardKey>", cardkey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var tempinfo = new MiniPIPOrdeInfo();
                tempinfo.CardKey = Convert.ToString(line[0]);
                tempinfo.OrderType = Convert.ToString(line[1]);
                tempinfo.OrderDate = Convert.ToString(line[2]);
                tempinfo.SONum = Convert.ToString(line[3]);
                tempinfo.Item = Convert.ToString(line[4]);
                tempinfo.Description = Convert.ToString(line[5]);
                tempinfo.QTY = Convert.ToString(line[6]);
                tempinfo.SSD = Convert.ToString(line[7]);
                tempinfo.Planner = Convert.ToString(line[8]);
                tempinfo.LineID = Convert.ToString(line[9]);
                ret.Add(tempinfo);
            }
            return ret;
        }

        private static MiniPIPOrdeInfo RetrieverOrderInfo(string cardkey, string lineid)
        {
            var ret = new MiniPIPOrdeInfo();
            var sql = "select CardKey from ECOCardContent where CardKey = '<CardKey>' and APVal9 = '<APVal9>'";
            sql = sql.Replace("<CardKey>", cardkey).Replace("<APVal9>", lineid);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                ret.CardKey = Convert.ToString(dbret[0][0]);
            }
            return ret;
        }

    }
}