//NOTES:
//hex editors
//  http://www.mobilefish.com/services/hex_editor/hex_editor.php
//  http://www.edithex.com/

//reading the bytes in but some turn out like fffd and weird stuff.
//try implementing a byte buffer to store a bunch and then print em.
//bytes may turn out better


package com.jensen.XV11LIDAR;

import java.io.IOException;
import java.util.Timer;
import java.util.TimerTask;
import android.app.Activity;
import android.os.Bundle;
import android.util.Log;

public class XV11LIDAR extends Activity {
	public static int PITTER_INTERVAL = 100; 	// time between updates
	public static String FILE_NAME = "XV11data2.txt";
	
	Timer pitter = new Timer();
	
	final String TAG = "";
	
	ReadFile dataFile = new ReadFile(FILE_NAME);
	
	String [] data = {"A0","FA","49","F7","01","27","04","2E","01","27","04","02","01","26","03","C5","01","26","03","E4","10","90"};
	
	byte [] hexData;
	
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        
        if(dataFile.isOpen() == false){
    		try {
    			dataFile.openFile();
    		} catch (IOException e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
    	}
        
        
        // set up 60 Hz refresh rate of display
		// do all processing within pitterProcess Function
		pitter.scheduleAtFixedRate(new TimerTask() {
			public void run() {	_pitterProcess();
			}
		}, 1, PITTER_INTERVAL);
        
    }//end onCreate
    
    protected void onStop(){
		super.onStop();
		
		try {
			dataFile.closeFile();
		} catch (IOException e) {
			e.printStackTrace();
		}

		
		pitter.cancel();  //kill the timer thread so it doesn't keep running on exit
	}
    
//    protected void onPause(){
//		super.onStop();
//		
//		try {
//			dataFile.closeFile();
//		} catch (IOException e) {
//			e.printStackTrace();
//		}
//
//  		pitter.cancel();  //kill the timer thread so it doesn't keep running on exit
//	}
    
    public void _pitterProcess() {
    
    	
    	int val = 0;
    	char buff [] = new char [15];
    	byte b = 0;
    	
    	if(dataFile.isOpen()){

    		
//    		val = dataFile.readValue();
//    		Log.e(TAG, "READ1 byte: " + Integer.toString(val, 10));
//    		Log.e(TAG, "READ2 byte: " + Integer.toHexString(val));
    		
    		buff = dataFile.readBuff();
//    		Log.e(TAG, "READ bytes: " + String.copyValueOf(buff));
    		
    		String s = "";
    		String s2 = "";
    		
    		for(int i = 0; i< buff.length; i++){
    			s = s + Character.toString(buff[i]);
//    			Log.e(TAG, "READ0 bytes: " + Character.digit(buff[i], 16));
    			Log.e(TAG, "READx bytes: " + Integer.toHexString(buff[i]));
    			Log.e(TAG, "READxx bytes: " + Character.valueOf(buff[i]));
    			Log.e(TAG, "READxxx bytes: " + Character.toString(buff[i]));
    			
    		}
//    		Log.e(TAG, "READ1 bytes: " + Integer.toHexString(buff[1]));
    		Log.e(TAG, "READ2 bytes: " + s);
    		Log.e(TAG, "READ3 bytes: " + String.valueOf(buff, 0, 15));
    		Log.e(TAG, "READ2 bytes: " + toHexStringMINE(buff));
    		
    	}
    	
//    	printHexValueInDec("0127"); 
    	
    	
//    	Log.e(TAG, "pitter....");
    	
//    	if(hexData == null){
////    		hexData = hexStringToByteArray(data);
//    		
//    		for(int i = 0; i< data.length()*2; i+=2){
////    			Log.d(TAG, "byte " + i + ": " + hexData[i-2]);
//    			Log.d(TAG, data.substring(i, i+2));
//    			
//    		}
    		
    	
    		
//    	}
    	
    	
    	
	
	}
    

    /**
     * Converts a byte array to hex string
     */
    public static String toHexStringMINE(char[] block) {
        StringBuffer buf = new StringBuffer();
        char[] hexChars = { 
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
            'A', 'B', 'C', 'D', 'E', 'F' };
        int len = block.length;
        int high = 0;
        int low = 0;
        for (int i = 0; i < len; i++) {
            high = ((block[i] & 0xf0) >> 4);
            low = (block[i] & 0x0f);
            buf.append(hexChars[high]);
            buf.append(hexChars[low]);
        } 
        return buf.toString();
    }
    
    
	//convert a 2 byte hex string to integer value
    public void printHexValueInDec(String val){
    	Log.e(TAG,"Hex: " + val + " is dec: " + Integer.valueOf(val, 16).intValue());
    }
    
    
    
    public static byte[] hexStringToByteArray(final String encoded) {
    	 if ((encoded.length() % 2) != 0)
    	        throw new IllegalArgumentException("Input string must contain an even number of characters");

    	    final byte result[] = new byte[encoded.length()/2];
    	    final char enc[] = encoded.toCharArray();
    	    for (int i = 0; i < enc.length; i += 2) {
    	        StringBuilder curr = new StringBuilder(2);
    	        curr.append(enc[i]).append(enc[i + 1]);
    	        result[i/2] = (byte) Integer.parseInt(curr.toString(), 16);
    	    }
    	    return result;
      
    }
    
} //end XV11LIDAR activity