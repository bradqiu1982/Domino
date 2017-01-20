package mytest;

import java.io.*;
import java.net.*;
import java.sql.Timestamp;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.Map.Entry;

import com.agile.api.*;


class MyLog
{
	public void info(String info)
	{
		Timestamp ts = new Timestamp(System.currentTimeMillis());   
        DateFormat sdf = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        String tsStr = sdf.format(ts);    
		System.out.println(tsStr +" : "+ info);
	}
	
	public void debug(String info)
	{
		Timestamp ts = new Timestamp(System.currentTimeMillis());   
        DateFormat sdf = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        String tsStr = sdf.format(ts);    
		System.out.println(tsStr +" : "+ info);
	}
	
	public void error(String info)
	{
		Timestamp ts = new Timestamp(System.currentTimeMillis());   
        DateFormat sdf = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        String tsStr = sdf.format(ts);    
		System.out.println(tsStr +" : "+ info);
	}
	
}

class WorkFlowTable
{
	public WorkFlowTable()
	{
		StatusCode = "";
		WorkFlow = "";
		WorkFlowStatus = "";
		Action = "";
		Reqd = "";
		Reviewer = "";
		SignoffUser = "";
		StatusChangedBy = "";
		LocalTime = "";
		SignoffComment = "";
		SignoffDuration = "";
	}
	
	public void SetValue(int key,String value)
	{
		if(key == ChangeConstants.ATT_WORKFLOW_STATUS_CODE) StatusCode = value;
		if(key == ChangeConstants.ATT_WORKFLOW_WORKFLOW) WorkFlow = value;
		if(key == ChangeConstants.ATT_WORKFLOW_WORKFLOW_STATUS) WorkFlowStatus = value;
		if(key == ChangeConstants.ATT_WORKFLOW_ACTION) Action = value;
		if(key == ChangeConstants.ATT_WORKFLOW_REQ_D) Reqd = value;
		if(key == ChangeConstants.ATT_WORKFLOW_REVIEWER) Reviewer = value;
		if(key == ChangeConstants.ATT_WORKFLOW_SIGNOFF_USER) SignoffUser = value;
		if(key == ChangeConstants.ATT_WORKFLOW_STATUS_CHANGED_BY) StatusChangedBy = value;
		if(key == ChangeConstants.ATT_WORKFLOW_LOCAL_CLIENT_TIME) LocalTime = value;
		if(key == ChangeConstants.ATT_WORKFLOW_SIGNOFF_COMMENTS) SignoffComment = value;
		if(key == ChangeConstants.ATT_WORKFLOW_SIGNOFF_DURATION) SignoffDuration = value;
	}
	
	public String StatusCode;
	public String WorkFlow;
	public String WorkFlowStatus;
	public String Action;
	public String Reqd;
	public String Reviewer;
	public String SignoffUser;
	public String StatusChangedBy;
	public String LocalTime;
	public String SignoffComment;
	public String SignoffDuration;
	
}

public class BOMFileAttachment {

	//public ResourceBundle rb=ResourceBundle.getBundle("FileAttachment");
	public static final MyLog goLogger =new MyLog();//Logger.getLogger(BOMFileAttachment.class);
    
   // main method.. 
	public static void main(String[] args) throws Exception
	{
		if(args.length > 4)
		{
			for(int idx = 0;idx < args.length;idx++)
			{
				goLogger.info("java main...param "+idx+" is "+args[idx]);				
			}
			
			String Mode = args[0];
			String AgileURL = args[1];
			String LocalSitePort = args[2];
			String AgileDir = args[3];
			
			List<String> ecolist = new ArrayList<String>();
			for(int idx = 4;idx < args.length;idx++)
			{
				ecolist.add(args[idx]);
			}
			
			HashMap<String,HashMap<String,String>> localfiledict = RetrieveLocalFiles(AgileDir);
			
			for(int idx = 0;idx < ecolist.size();idx++)
			{
				boolean ret = CreateDir(AgileDir+"\\"+ecolist.get(idx));
				if(!ret)
					return;
			}

			if(Mode.equalsIgnoreCase("ATTACH"))
			{
				IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
				if(sess != null)
				{
					BOMFileAttachment gfa=new BOMFileAttachment();
					gfa.getAgileFilesByName(sess,ecolist,AgileDir,localfiledict);
					NoticDominoAttach(LocalSitePort,ecolist);
				}
				//gfa.getAgileFilesByName("E150570");
				//gfa.getAgileFilesByName("WI-MFG-318");
				//gfa.getAgileFilesByName("38200039");
			}
			else if(Mode.equalsIgnoreCase("WORKFLOW"))
			{
				IAgileSession sess =  getAgileSession(AgileURL,"mkbomctx","agiledll");
				if(sess != null)
				{
					BOMFileAttachment gfa=new BOMFileAttachment();
					gfa.getAgileWorkFlow(sess,ecolist,AgileDir);
				}
			}
		}
		else
		{
			goLogger.error("\n Usage: ATTACH/WORKFLOW AgileURL LocalSitePort AgileDir ECONUM1 ECONUM2 ECONUM3 .....");
			return;
		}
		
	}
	
	public void getAgileFilesByName(IAgileSession sess,List<String> ecolist,String AgileDir,HashMap<String,HashMap<String,String>> localfiledict) {

		boolean success = true;
		for(int idx = 0;idx < ecolist.size();idx++)
		{
			String savedlocation = AgileDir+"\\"+ecolist.get(idx)+"\\";
        	success = getFilesWithECO(sess,ecolist.get(idx),savedlocation,localfiledict);
        	if(success==true)
            	continue;
        	getFilesWithUniqueKey(sess,ecolist.get(idx),savedlocation,localfiledict);
		}
	}
	
	public void getAgileWorkFlow(IAgileSession sess,List<String> ecolist,String AgileDir) {

		for(int idx = 0;idx < ecolist.size();idx++)
		{
			String savedlocation = AgileDir+"\\"+ecolist.get(idx)+"\\";
        	getWorkFlowWithECO(sess,ecolist.get(idx),savedlocation);
		}
	}
	
	private static HashMap<String,HashMap<String,String>> RetrieveLocalFiles(String AgileDir)
	{
		HashMap<String,HashMap<String,String>> ret = new HashMap<String,HashMap<String,String>>();
        File maindir = new File(AgileDir);
        File[] subdirs = maindir.listFiles();
        for (File subdir : subdirs){
        	
            if (subdir.isDirectory())
            {
            	HashMap<String,String> value = new HashMap<String,String>();
            	File[] detailfiles = subdir.listFiles();
            	for(File detailfile : detailfiles)
            	{
            		if(detailfile.isFile())
            		{
            			value.put(detailfile.getName(), "true");
            		}
            	}//end for
            	
            	ret.put(subdir.getName(),value);
            }//end if
        }//end for
		
		return ret;
	}
	
	private static boolean AttachExist(HashMap<String,HashMap<String,String>> localfiledict,String ECONum,String FN)
	{
		HashMap<String,String> ecoexist = localfiledict.get(ECONum);
		if(ecoexist != null)
		{
			String attachexist	= ecoexist.get(FN);
			if(attachexist != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	// Getting Agile Session
	 private static IAgileSession getAgileSession(String agileurl,String user,String password) 
	 {
		//String url="http://sny-agile9app-p64:7001/Agile";
		String url = agileurl;
		String uname=user;
		String upwd=password;
		goLogger.debug("getAgileSession:"+"\nurl="+url + "  username=" + uname + "  pwd=" + upwd);
		HashMap params=new HashMap();
		params.put(AgileSessionFactory.USERNAME, uname);
		params.put(AgileSessionFactory.PASSWORD, upwd);
		
		try 
		{
			AgileSessionFactory fac=AgileSessionFactory.getInstance(url);
			IAgileSession sess =  fac.createSession(params);
			if(sess == null)
			{
				goLogger.debug("Fail to get Agile session" );
			}
			else
			{
				goLogger.debug(" Got Agile Session" );
			}
			return sess;
		}
		catch (APIException e) 
		{
		  goLogger.debug("Error while connecting to Agile\n...Msg="+e.getMessage()+"\nurl="+url );
		}
		
		return null;
	 }
	 
	private static boolean CreateDir(String dirstr)
	{
		File dir = new File(dirstr);
		if(!dir.exists())
		{
			try
			{
				dir.mkdirs();
				return true;
			}
			catch(Exception ex)
			{
				goLogger.error("\n fail to create directory : "+ dirstr);
				return false;
			}
		}
		
		return true;
	}
		
	private static void NoticDominoAttach(String LocalSitePort,List<String> ecolist)
	{
		for(int idx = 0;idx < ecolist.size();idx++)
		{
			String econum = ecolist.get(idx);
			try
			{
				goLogger.info( "try to query: "+"http://localhost:"+LocalSitePort+"/Domino/MiniPIP/AgileDownload?ECONUM="+econum);
				
				URL url = new URL("http://localhost:"+LocalSitePort+"/Domino/MiniPIP/AgileDownload?ECONUM="+econum);
		        URLConnection URLconnection = url.openConnection();  
		        HttpURLConnection httpConnection = (HttpURLConnection)URLconnection;  
		        int responseCode = httpConnection.getResponseCode();
		        if (responseCode == HttpURLConnection.HTTP_OK) {  
		        	goLogger.info("Query HTTP server successfully");
		        	
		        	InputStream urlStream = httpConnection.getInputStream();  
		            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(urlStream));  
		            String sCurrentLine = "";  
		            while ((sCurrentLine = bufferedReader.readLine()) != null) {  
		            	goLogger.info( sCurrentLine);
		            }  
		        }
		        else
		        {
		        	goLogger.error("Fail to access url:"+"http://localhost:"+LocalSitePort+"/Domino/MiniPIP/AgileDownload?ECONUM="+econum);
		        }
			}catch(Exception ex)
			{
				goLogger.error("Fail to query url:"+ex.getMessage());			
			}				
		}
	}
	
	 private boolean scanAttachement(ITable atttable,String savedlocation,String econum,HashMap<String,HashMap<String,String>> localfiledict)
	 {
		 try
		 {
			 boolean ret = true;
			 Iterator ite=atttable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					String attname=row.getValue(ItemConstants.ATT_ATTACHMENTS_FILENAME).toString();
					goLogger.debug("Got attachment... with name "+attname);
					if(!AttachExist(localfiledict,econum,attname))
					{
					   
					   try{
						   InputStream is=((IAttachmentFile)row).getFile();
						   goLogger.info("start download file "+attname);
						   createFile(is,attname,savedlocation);
					   }
					   catch(Throwable a){
							goLogger.error("Please check if the file server up and running and the specified file downloadable.");
							ret = false;
					   }						
					}
				 }
			 return ret;			 
		 }
		 catch (APIException e)
		 {
			 goLogger.error("Exception="+e.getMessage());
			 return false;
		 }
	 }
	
	private boolean getFilesWithUniqueKey(IAgileSession ses,String Bomnumber,String savedlocation,HashMap<String,HashMap<String,String>> localfiledict)
	{
    	try 
    	{
	    	IItem item=(IItem) ses.getObject(ItemConstants.CLASS_ITEM_BASE_CLASS, Bomnumber);
	    	if(item!=null)
	    	{
	    		goLogger.debug("Seraching Attachment...with unique key "+Bomnumber);
				ITable atttable=item.getTable(ItemConstants.TABLE_ATTACHMENTS);//**
				boolean success = scanAttachement(atttable, savedlocation,Bomnumber,localfiledict);
				if(!success)
				{
					goLogger.debug("There is no file with  in the BOM "+Bomnumber);
					return false;
				}
				return true;
	    	}
	    	else
	    	{
	    		goLogger.debug("The unique key  "+Bomnumber+ " is not exist in Agile");
	    	}
		}
    	catch(NullPointerException np){
		    goLogger.error("Null Exception="+np.getMessage());
		}catch (APIException e) {
			goLogger.error("Exception="+e.getMessage());
		}
		return false;
	}
	
	private boolean getFilesWithECO(IAgileSession ses,String Bomnumber,String savedlocation,HashMap<String,HashMap<String,String>> localfiledict) 
	{
    	try 
    	{
	    	IChange eco = (IChange)ses.getObject(IChange.OBJECT_TYPE, Bomnumber);
	    	if(eco == null)
			{
				goLogger.info("Fail to get class IChange from "+Bomnumber);
				return false;
			}
			else
			{
				goLogger.info("get class IChange "+eco.toString()+" "+eco.getName());
				ITable atttable = eco.getAttachments();
				if(atttable == null)
				{
					goLogger.info("Fail to get Attachments from ECO "+Bomnumber);
					return false;
				}
				else
				{
					boolean ret = scanAttachement(atttable, savedlocation,Bomnumber,localfiledict);
					return ret;
				}
			}
		}
    	catch(NullPointerException np){
		    goLogger.error("Null Exception="+np.getMessage());
		    return false;
		}catch (APIException e) {
			goLogger.error("Exception="+e.getMessage());
			return false;
		}
	}
	
	private void getWorkFlowWithECO(IAgileSession ses,String Bomnumber,String savedlocation)
	{
		try 
    	{
			IChange eco = (IChange)ses.getObject(IChange.OBJECT_TYPE, Bomnumber);
	    	if(eco == null)
			{
				goLogger.info("Fail to get class IChange from "+Bomnumber);
				return;
			}
			else
			{
				goLogger.info("get class IChange "+eco.toString()+" "+eco.getName());
				
					ITable workflowtable = eco.getTable(ChangeConstants.TABLE_WORKFLOW);
					if(workflowtable != null)
					{
						List<WorkFlowTable> wtab = new ArrayList<WorkFlowTable>();
						Iterator ite=workflowtable.iterator();
						while(ite.hasNext()){
							IRow row=(IRow) ite.next();
							
							WorkFlowTable wtabrow = new WorkFlowTable();
							
							Map<Integer,Object> mvalues = row.getValues();
							
							goLogger.debug(mvalues.toString());
							
							for(Entry<Integer,Object> e: mvalues.entrySet())
							{
								int key = e.getKey();
								if(e.getValue() != null)
								{
									String value = e.getValue().toString();
									wtabrow.SetValue(key, value.replace("'", "").replace(",", ""));
								}
							}
							wtab.add(wtabrow);
						}//end while
						
						if(wtab.size() > 0)
						{
							File f=new File(savedlocation+"WorkFlowTable.csv");
							DataOutputStream out=null;
							try {
							out = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(savedlocation+"WorkFlowTable.csv")));
							out.writeBytes("StatusCode,"+"WorkFlow,"+"WorkFlowStatus,"
									+"Action,"+"Reqd,"+"Reviewer,"
									+"SignoffUser,"+"StatusChangedBy,"+"LocalTime,"
									+"SignoffComment,"+"SignoffDuration\r\n");
							
							for(int idx = 0;idx < wtab.size();idx++)
							{
								out.writeBytes(wtab.get(idx).StatusCode+","+wtab.get(idx).WorkFlow+","+wtab.get(idx).WorkFlowStatus+","+
										wtab.get(idx).Action+","+wtab.get(idx).Reqd+","+wtab.get(idx).Reviewer+","+
										wtab.get(idx).SignoffUser+","+wtab.get(idx).StatusChangedBy+","+wtab.get(idx).LocalTime+","+
										wtab.get(idx).SignoffComment+","+wtab.get(idx).SignoffDuration+"\r\n");
							}
							out.close();
							goLogger.debug("WorkFlow is saved at location: "+f.getAbsolutePath());
							
						}catch (FileNotFoundException e) {
							goLogger.error("Exception in getting output stream  file.."+e.getMessage());
						}catch (IOException e) {
							goLogger.error("Exception in writing to file.."+e.getMessage());
						}
						}//end if
						
					}//end if
			}
		}
    	catch(NullPointerException np){
		    goLogger.error("Null Exception="+np.getMessage());
		    return ;
		}catch (APIException e) {
			goLogger.error("Exception="+e.getMessage());
			return ;
		}
		
	}
	
	private void createFile(InputStream fin,String filename,String loc) {
		goLogger.debug("Copying the attachment..");
		File f=new File(loc+filename);
		DataOutputStream out=null;
		try {
		out = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(loc+filename)));
		}catch (FileNotFoundException e) {
			goLogger.error("Exception in getting output stream  file.."+e.getMessage());
		}
		
		boolean again = true;
		try {
		while(again) {
			int readpart;
			readpart = fin.read();
			if(readpart > -1) {
				out.writeByte(readpart);
			}
			else again = false;
		}
		out.close();
		fin.close();
		} catch (IOException e) {
			goLogger.error("Exception in writing to file.."+e.getMessage());
		}
	    goLogger.debug("Attachment is saved at location: "+f.getAbsolutePath());
	}
	
}
