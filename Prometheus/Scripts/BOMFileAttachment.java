package mytest;

import java.io.BufferedOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.Console;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.LinkOption;
import java.nio.file.attribute.FileAttribute;
import java.sql.Timestamp;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.HashMap;
import java.util.Iterator;
import java.util.ResourceBundle;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

import com.agile.api.IAgileSession;
import com.agile.api.APIException;
import com.agile.api.AgileSessionFactory;
import com.agile.api.ChangeConstants;
import com.agile.api.IAgileObject;
import com.agile.api.IAttachmentFile;
import com.agile.api.IChange;
import com.agile.api.IItem;
import com.agile.api.IRow;
import com.agile.api.ITable;
import com.agile.api.ItemConstants;

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

public class BOMFileAttachment {

	//public ResourceBundle rb=ResourceBundle.getBundle("FileAttachment");
	public static final MyLog goLogger =new MyLog();//Logger.getLogger("C:\\Agile\\mylog.txt");//Logger.getLogger(BOMFileAttachment.class);
	public IAgileSession ses=null;
    
   // main method.. 
	public static void main(String[] args) throws Exception
	{
		goLogger.info("java main...param count is "+args.length);
		if(args.length == 1)
		{
			goLogger.info("\njava main...param 1 is "+args[0]);
			
			File dir = new File("d:\\Agile\\"+args[0]);
			if(!dir.exists())
			{
				try
				{
					dir.mkdir();
				}
				catch(Exception ex)
				{
					goLogger.error("\njava main... exit for fail to create directory");
				}
			}
			
			BOMFileAttachment gfa=new BOMFileAttachment();
			//int flag = gfa.getAgileFilesByName(args[0],args[1],args[2],"mkbomctx","agiledll");
			//int flag = gfa.getAgileFilesByName("E150570","Checklist","d:\\Agile\\Temp\\","mkbomctx","agiledll");
			//int flag = gfa.getAgileFilesByName("WI-MFG-318","WI-MFG-318 assembly","d:\\Agile\\Temp\\","mkbomctx","agiledll");
			//int flag = gfa.getAgileFilesByName("38200039","Cfg_Avanex_Modulator_MPL.xml","d:\\Agile\\Temp\\","mkbomctx","agiledll");
			int flag = gfa.getAgileFilesByName(args[0],"d:\\Agile\\"+args[0]+"\\","mkbomctx","agiledll");
			goLogger.info("getFileAttachment() returned "+flag);
		}
		else
		{
			goLogger.error("\njava main... exit for arg problem");
		}
	}
	
	// Getting Agile Session
	 public void getAgileSession(String user,String password) 
	 {
		//String url=rb.getObject("agile.url").toString();
		String url="http://sny-agile9app-p64:7001/Agile";
		String uname=user;
		String upwd=password;
		goLogger.debug("getAgileSession:"+"\nurl="+url + "  username=" + uname + "  pwd=" + upwd);
		HashMap params=new HashMap();
		params.put(AgileSessionFactory.USERNAME, uname);
		params.put(AgileSessionFactory.PASSWORD, upwd);
		try {
		AgileSessionFactory fac=AgileSessionFactory.getInstance(url);
		ses=fac.createSession(params);
		}catch (APIException e) {
		  goLogger.debug("Error while connecting to Agile\n...Msg="+e.getMessage()+"\nurl="+url );
		}
	 }

	 private boolean scanAttachement(ITable atttable,String savedlocation)
	 {
		 try
		 {
			 boolean ret = true;
			 Iterator ite=atttable.iterator();
				while(ite.hasNext()){
					IRow row=(IRow) ite.next();
					String attname=row.getValue(ItemConstants.ATT_ATTACHMENTS_FILENAME).toString();

					   goLogger.debug("Got attachment... with name "+attname);
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
			 return ret;			 
		 }
		 catch (APIException e)
		 {
			 goLogger.error("Exception="+e.getMessage());
			 return false;
		 }
	 }
	
	private boolean getFilesWithUniqueKey(String Bomnumber,String savedlocation)
	{
    	try 
    	{
	    	IItem item=(IItem) ses.getObject(ItemConstants.CLASS_ITEM_BASE_CLASS, Bomnumber);
	    	if(item!=null)
	    	{
	    		goLogger.debug("Seraching Attachment...with unique key "+Bomnumber);
				ITable atttable=item.getTable(ItemConstants.TABLE_ATTACHMENTS);//**
				boolean success = scanAttachement(atttable, savedlocation);
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
	
	private boolean getFilesWithECO(String Bomnumber,String savedlocation) 
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
					boolean ret = scanAttachement(atttable, savedlocation);
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
	 
	public int getAgileFilesByName(String Bomnumber,String savedlocation,String user,String pwd) {
		
        getAgileSession(user,pwd);
        boolean success=false;

        if(ses!=null)
        {
        	goLogger.debug(" Got Agile Session" );
        	success = getFilesWithECO(Bomnumber,savedlocation);
        	if(success==true)
            	return 0;
        	
        	success = getFilesWithUniqueKey(Bomnumber,savedlocation);
        	if(success)
	        	return 0;
        	else
        		return -1;
		}
        else
        {
        	goLogger.info("Fail to get session from key "+Bomnumber);
        	return -1;
        }
	}
	
	private void createFile(InputStream fin,String filename,String loc) {
		goLogger.debug("Copying the attachment..");
		File f=new File(loc+filename);
		DataOutputStream out=null;;
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
	    goLogger.debug("Attachment is saved at location"+f.getAbsolutePath());
	}
	
}
