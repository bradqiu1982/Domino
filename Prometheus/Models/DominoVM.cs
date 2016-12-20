using System;
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
        public string ECOkey { set; get; }
        public string Cardkey { set; get; }
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
                return cardexist[0].Cardkey;
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
                tempitem.ECOkey = Convert.ToString(line[0]);
                tempitem.Cardkey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.Cardkey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.Cardkey);

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
                tempitem.ECOkey = Convert.ToString(line[0]);
                tempitem.Cardkey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.Cardkey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.Cardkey);

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
                tempitem.ECOkey = Convert.ToString(line[0]);
                tempitem.Cardkey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                //tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.Cardkey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.Cardkey);

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
                tempitem.ECOkey = Convert.ToString(line[0]);
                tempitem.Cardkey = Convert.ToString(line[1]);
                tempitem.CardType = Convert.ToString(line[2]);
                tempitem.CardStatus = Convert.ToString(line[3]);
                tempitem.CardNo = idx.ToString();
                tempitem.CardContent = tempitem.ECOContentDict[tempitem.CardType];
                //tempitem.EBaseInfo = baseinfo;

                tempitem.CommentList = RetrieveCardComments(tempitem.Cardkey);
                tempitem.AttachList = RetrieveCardAttachment(tempitem.Cardkey);

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

        private static void UpdateECOPendingInfo_UpdateInfo(string cardkey, string weeklyupdate)
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
                ret.Cardkey = Convert.ToString(dbret[0][0]);
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

            if (string.IsNullOrEmpty(infoexist.Cardkey))
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
                ret.Cardkey = Convert.ToString(dbret[0][0]);
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


        private static Dictionary<string, string> GetSysConfig(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/DominoCfg.txt"));
            var ret = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains(":::"))
                {
                    var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                    ret.Add(kvpair[0].Trim(), kvpair[1].Trim());
                }
            }
            return ret;
        }

        public static void UpdateECOWeeklyUpdate(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = GetSysConfig(ctrl);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            var vm = RetrieveECOPendingInfo(cardkey);
            if (!string.IsNullOrEmpty(vm.WeeklyUpdateTime))
            {
                if ((DateTime.Now - DateTime.Parse(vm.WeeklyUpdateTime)).Hours < 48)
                {
                    return;
                }
            }


            var desfolder = syscfgdict["WEEKLYUPDATE"];
            var sheetname = syscfgdict["MINIPIPSHEETNAME"];
            try
            {
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (Directory.Exists(desfolder))
                {
                    var fds = Directory.EnumerateFiles(desfolder);
                    foreach (var fd in fds)
                    {
                        try
                        {
                            var fn = imgdir + Path.GetFileName(fd);
                            System.IO.File.Copy(fd, fn, true);
                            var data = ExcelReader.RetrieveDataFromExcel(fn, sheetname);
                            foreach (var line in data)
                            {
                                if (string.Compare(line[2], baseinfo.PNDesc, true) == 0
                                    && string.Compare(DateTime.Parse(line[12]).ToString("yyyy-MM-dd"), DateTime.Parse(baseinfo.InitRevison).ToString("yyyy-MM-dd"), true) == 0)
                                {
                                    var update = line[85];
                                    DominoVM.UpdateECOPendingInfo_UpdateInfo(cardkey, update);
                                    return;
                                }//end if
                            }//end foreach
                        }
                        catch (Exception ex) { }
                    }//end foreach
                }
            }
            catch (Exception ex) { }

        }

        private static string ConvertToDate(string datestr)
        {
            if (string.IsNullOrEmpty(datestr))
            {
                return string.Empty;
            }
            try
            {
                return DateTime.Parse(datestr).ToString();
            }
            catch (Exception ex) { return string.Empty; }
        }

        private static void updateecolist(List<List<string>> data, Controller ctrl,string localdir,string urlfolder)
        {
            var syscfgdict = GetSysConfig(ctrl);
            var baseinfos = ECOBaseInfo.RetrieveAllECOBaseInfo();

            foreach (var line in data)
            {
                if (!string.IsNullOrEmpty(line[2]) 
                    && !string.IsNullOrEmpty(line[12])
                    && !string.IsNullOrEmpty(line[23])
                    &&(string.Compare(line[23],"C",true) == 0
                    || string.Compare(line[23], "W", true) == 0)
                    && string.Compare(line[81],"canceled",true) != 0)
                {
                    var initialmini = string.Empty;
                    try
                    {
                        initialmini = DateTime.Parse(line[12]).ToString();
                    }
                    catch (Exception ex)
                    { initialmini = string.Empty; }

                    if (string.IsNullOrEmpty(initialmini) 
                        || DateTime.Parse(initialmini) > DateTime.Parse("2016-9-15 10:00:00")
                        || DateTime.Parse(initialmini) < DateTime.Parse("2016-9-7 10:00:00"))
                    {
                        continue;
                    }

                    var baseinfo = new ECOBaseInfo();
                    baseinfo.PNDesc = line[2];
                    baseinfo.Customer = line[5];
                    baseinfo.Complex = line[6];
                    baseinfo.RSM = line[7];
                    baseinfo.PE = line[8];
                    baseinfo.RiskBuild = line[9];

                    baseinfo.InitRevison = ConvertToDate(line[12]);
                    baseinfo.FinalRevison = ConvertToDate(line[13]);
                    baseinfo.TLAAvailable = ConvertToDate(line[14]);
                    baseinfo.OpsEntry = ConvertToDate(line[17]);
                    baseinfo.TestModification = ConvertToDate(line[18]);
                    baseinfo.ECOSubmit = ConvertToDate(line[19]);
                    baseinfo.ECOReviewSignoff = ConvertToDate(line[79]);
                    baseinfo.ECOCCBSignoff = ConvertToDate(line[80]);
                    baseinfo.ECONum = line[81];
                    baseinfo.QTRInit = line[82];

                    bool ecoexist = false;
                    foreach (var item in baseinfos)
                    {
                        if (string.Compare(item.PNDesc, baseinfo.PNDesc, true) == 0
                            && string.Compare(DateTime.Parse(item.InitRevison).ToString("yyyy-MM-dd"), DateTime.Parse(baseinfo.InitRevison).ToString("yyyy-MM-dd"), true) == 0)
                        {
                            ecoexist = true;

                            item.Customer = baseinfo.Customer;
                            item.Complex = baseinfo.Complex;
                            item.RSM = baseinfo.RSM;
                            item.PE = baseinfo.PE;
                            item.RiskBuild = baseinfo.RiskBuild;

                            item.FinalRevison = baseinfo.FinalRevison;
                            item.TLAAvailable = baseinfo.TLAAvailable;
                            item.OpsEntry = baseinfo.OpsEntry;
                            item.TestModification = baseinfo.TestModification;
                            item.ECOSubmit = baseinfo.ECOSubmit;
                            item.ECOReviewSignoff = baseinfo.ECOReviewSignoff;
                            item.ECOCCBSignoff = baseinfo.ECOCCBSignoff;
                            item.ECONum = baseinfo.ECONum;
                            item.QTRInit = baseinfo.QTRInit;
                            item.UpdateECO();

                            var pendingcard = RetrieveSpecialCard(item, DominoCardType.ECOPending);

                            if (pendingcard.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(item.ECONum)
                                && string.Compare(pendingcard[0].CardStatus,DominoCardStatus.working) == 0)
                                {
                                    DominoVM.UpdateCardStatus(pendingcard[0].Cardkey, DominoCardType.ECOPending);
                                }

                                var allattach = DominoVM.RetrieveCardExistedAttachment(pendingcard[0].Cardkey);

                                var MiniPIPDocFolder = syscfgdict["MINIPIPECOFOLDER"] + "\\" + baseinfo.Customer + "\\" + baseinfo.PNDesc;
                                if (Directory.Exists(MiniPIPDocFolder))
                                {
                                    var minidocfiles = Directory.EnumerateFiles(MiniPIPDocFolder);
                                    foreach (var minifile in minidocfiles)
                                    {
                                        var fn = System.IO.Path.GetFileName(minifile);
                                        fn = fn.Replace(" ", "_").Replace("#", "")
                                                .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
                                        
                                        bool attachexist = false;
                                        foreach (var att in allattach)
                                        {
                                            if (att.Contains(fn))
                                            {
                                                attachexist = true;
                                                break;
                                            }
                                        }//end foreach

                                        if (!attachexist)
                                        {
                                            var desfile = localdir + fn;
                                            try
                                            {
                                                System.IO.File.Copy(minifile, desfile, true);
                                                var url = "/userfiles/docs/" + urlfolder + "/" + fn;
                                                DominoVM.StoreCardAttachment(pendingcard[0].Cardkey, url);
                                            }
                                            catch (Exception ex) { }
                                        }
                                    }//end foreach
                                }//try to get mini doc file
                            }//end if

                            break;
                        }
                    }

                    if (!ecoexist)
                    {
                        baseinfo.ECOKey = DominoVM.GetUniqKey();
                        baseinfo.CreateECO();

                        var CardKey = DominoVM.GetUniqKey();
                        
                        if (string.IsNullOrEmpty(baseinfo.ECONum))
                        {
                            DominoVM.CreateCard(baseinfo.ECOKey, CardKey, DominoCardType.ECOPending, DominoCardStatus.working);
                        }
                        else
                        {
                            DominoVM.CreateCard(baseinfo.ECOKey, CardKey, DominoCardType.ECOPending, DominoCardStatus.pending);
                        }

                        var MiniPIPDocFolder = syscfgdict["MINIPIPECOFOLDER"] + "\\" + baseinfo.Customer + "\\" + baseinfo.PNDesc;
                        if (Directory.Exists(MiniPIPDocFolder))
                        {
                            var minidocfiles = Directory.EnumerateFiles(MiniPIPDocFolder);
                            foreach(var minifile in minidocfiles)
                            {
                                var fn  = System.IO.Path.GetFileName(minifile);
                                fn = fn.Replace(" ", "_").Replace("#", "")
                                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
                                var desfile = localdir + fn;

                                try
                                {
                                    System.IO.File.Copy(minifile, desfile,true);
                                    var url = "/userfiles/docs/" + urlfolder + "/" + fn;
                                    DominoVM.StoreCardAttachment(CardKey, url);
                                }
                                catch (Exception ex) { }

                            }//end foreach
                        }//try to get mini doc file
                    }

                }
            }
        }

        public static void SetForceECORefresh(Controller ctrl)
        {
            var syscfgdict = GetSysConfig(ctrl);
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = imgdir + syscfgdict["MINIPIPECOFILENAME"];
            if (System.IO.File.Exists(desfile))
            {
                try
                {
                    System.IO.File.Delete(desfile);
                }
                catch (Exception ex) { }
            }
        }

        public static void RefreshECOList(Controller ctrl)
        {
            var syscfgdict = GetSysConfig(ctrl);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = imgdir + syscfgdict["MINIPIPECOFILENAME"];
            try
            {
                if (!Directory.Exists(imgdir)|| !System.IO.File.Exists(desfile))
                {
                    if(!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);

                    var srcfile = syscfgdict["MINIPIPECOFOLDER"] + "\\" + syscfgdict["MINIPIPECOFILENAME"];
                
                    if (System.IO.File.Exists(srcfile) && !System.IO.File.Exists(desfile))
                    {
                        System.IO.File.Copy(srcfile, desfile,true);
                    }
                }

                if (System.IO.File.Exists(desfile))
                    {
                        var data = ExcelReader.RetrieveDataFromExcel(desfile, syscfgdict["MINIPIPSHEETNAME"]);
                        updateecolist(data,ctrl,imgdir, datestring);
                    }
            }
            catch (Exception ex) { }
        }

        private static string RMSpectialCh(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static void RefreshQAEEPROMFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {

            var syscfgdict = GetSysConfig(ctrl);
            var srcrootfolder = syscfgdict["QAEEPROMFAI"];
            var eepromfilter = syscfgdict["QAEEPROMCHECKLISTFILTER"];

            if (Directory.Exists(srcrootfolder))
            {
                var currentcard  = RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = DominoVM.RetrieveCardExistedAttachment(currentcard[0].Cardkey);

                var destfolderlist = new List<string>();
                var srcfolders = Directory.EnumerateDirectories(srcrootfolder);
                foreach (var fd in srcfolders)
                {
                    if (fd.ToUpper().Contains(baseinfo.PNDesc.ToUpper()))
                    {
                        destfolderlist.Add(fd);
                    }
                }//end foreach get folder contains PNDESC

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = Directory.EnumerateFiles(fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(eepromfilter))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            System.IO.File.Copy(desf, attpath,true);
                            DominoVM.StoreCardAttachment(CardKey, url);
                        }
                        catch (Exception ex){ }
                    }

                }//end foreach

            }//end if

        }

        private static void RefreshQALabelFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            var syscfgdict = GetSysConfig(ctrl);
            var srcrootfolder = syscfgdict["QALABELFAI"];
            var labelfilter = syscfgdict["QALABELFILTER"];

            if (Directory.Exists(srcrootfolder))
            {
                var currentcard = RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = DominoVM.RetrieveCardExistedAttachment(currentcard[0].Cardkey);

                var firstleveldirs = new List<string>();
                var ffold = Directory.EnumerateDirectories(srcrootfolder);
                firstleveldirs.AddRange(ffold);

                var seconfleveldir = new List<string>();
                foreach (var ffd in ffold)
                {
                    var sfd = Directory.EnumerateDirectories(ffd);
                    seconfleveldir.AddRange(sfd);
                }

                var destfolderlist = new List<string>();
                foreach (var desf in seconfleveldir)
                {
                    if (desf.ToUpper().Contains(baseinfo.PNDesc.ToUpper()))
                    {
                        destfolderlist.Add(desf);
                    }
                }

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = Directory.EnumerateFiles(fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(labelfilter))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            System.IO.File.Copy(desf, attpath, true);
                            DominoVM.StoreCardAttachment(CardKey, url);
                        }
                        catch (Exception ex) { }
                    }
                }//end foreach

            }//end if

        }

        public static void RefreshQAFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            RefreshQAEEPROMFAI(baseinfo, CardKey, ctrl);
            RefreshQALabelFAI(baseinfo, CardKey, ctrl);
        }

    }
}