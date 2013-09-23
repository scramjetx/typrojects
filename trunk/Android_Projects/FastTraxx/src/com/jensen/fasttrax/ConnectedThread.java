//NOTES:
//set this up so there is a function that merely grabs the 2 bytes needed.
//then after link is shut off parse the value to be displayed to user because takes some time for parseint function

package com.jensen.fasttrax;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Arrays;

import android.bluetooth.BluetoothSocket;
import android.util.Log;

public class ConnectedThread extends Thread {
    private final BluetoothSocket mmSocket;
    private final InputStream mmInStream;
    private final OutputStream mmOutStream;
    private String TAG = "GarageOPENER";
    private boolean D = false;		//tracks if debug messages are printed
    private boolean persistantConn; //should the program keep trying to connect to the device
    
    byte[] buffer = new byte[25];  // buffer store for the stream
    int bytes; // # of bytes returned from read()
    public int linkQual = 0;		//ranges seems best: 255,FF ; worst: 223,DF  before loss of link
    
    public ConnectedThread(BluetoothSocket socket, boolean retry) {
        mmSocket = socket;
        persistantConn = retry;
        InputStream tmpIn = null;
        OutputStream tmpOut = null;
        boolean conn = false;
        
        do{
        	conn = connectSocket();

        	//sleep thread then try again
        	try {
				sleep(1000);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
        
			//forces while loop to execute once of persistant conn isn't enabled
			if(persistantConn == false)
				break;
			
        }while(conn != true);
        
        	
        // Get the input and output streams, using temp objects because
        // member streams are final
        try {
            tmpIn = socket.getInputStream();
            tmpOut = socket.getOutputStream();
            
            if(D)
            	Log.e(TAG,"GOT STREAMS!");
            
        } catch (IOException e) { }

        mmInStream = tmpIn;
        mmOutStream = tmpOut;
    }

    public boolean connectSocket(){
    	// Blocking connect, for a simple client nothing else can
        // happen until a successful connection is made, so we
        // don't care if it blocks.
        try {
        	mmSocket.connect();
        	
        	if(D)
        		Log.e(TAG, "ON RESUME: BT connection established, data transfer link open.");
        	
        	return true;
        	
        } catch (IOException e) {
        	if(D)
        		Log.e(TAG,"Socket NOT connected.");
        	
        	try {
	        	  mmSocket.close();
	        	  Log.e(TAG, "ON RESUME: Socket Connection Failure.");
	          } catch (IOException e2) {
	        	  if(D)
	        		  Log.e(TAG,"ON RESUME: Unable to close socket during connection failure", e2);
	          }
	          
	          return false;
        }
    	
    }
    
    
    public void run() {
       
    	byte[] Tempbuffer = new byte[25];  // buffer store for the stream

        // Keep listening to the InputStream until an exception occurs
        //this stream is in integer format when it returns the value
        //so look at an ascii table if its text under dec returned to find the ascii value.
        //for sending $$$ you get back 5,67,77,68,13,10 which is Enquiry,C,M,D,CarriageReturn,LineFeed
            try {

            	// Read from the InputStream
                bytes = mmInStream.read(Tempbuffer);
                
                if(D)
                	Log.e(TAG, "Bytes to read: " + Integer.toString(bytes));
                
                if(bytes == -1){
                	bytes = 0;
                }
                for(int i = 0; i< bytes; i++){
                	char c;
                	c = (char)Tempbuffer[i];
                	
                	//prints the integer value..use ascii lookup table
                	if(D)
                		Log.e(TAG, "Integer: " + String.valueOf(Tempbuffer[i]) + "  ASCII: " + String.valueOf(c));
                	
                	//prints ascii char of integer
//                	Log.e(TAG, String.valueOf(c));
                    
                }
                
                buffer = Tempbuffer;
                
                // Send the obtained bytes to the UI Activity
//                mHandler.obtainMessage(MESSAGE_READ, bytes, -1, buffer)
//                        .sendToTarget();
            } catch (IOException e) {
            	if(D)
            		Log.e(TAG, "ON RESUME: Exception during read.", e);
            }
    }

    /* Call this from the main Activity to send data to the remote device */
    public void write(byte[] bytes) {
        try {
            mmOutStream.write(bytes);
            
            if(D)
            	Log.e(TAG,"SENT BYTES!");
            
        } catch (IOException e) {
        	if(D)
        		Log.e(TAG, "ON RESUME: Exception during write.", e);
        }
    }

    
    public void parseLinkQuality(){
    	
    	String ff = (Character.toString((char)buffer[5])) + (Character.toString((char)buffer[6]));
    	int LQnum = 0;
    	
    	try {
    		LQnum = Integer.parseInt(ff,16);
    	} catch (NumberFormatException e) {
    		LQnum = 999;  //catch an exception if link doesn't come through that time
    	}

    	//set global value of linkQuality 
    	linkQual = LQnum;

    	//gets us 15 out of byte from link
//    	String s4 = Character.toString((char)buffer[5]);  //converts char to f
//    	int decimalNumber = Integer.parseInt(s4, 16);
    	
//    	int decimalNumber = Integer.parseInt("F", 16);
//    	int decimalNumber = Integer.parseInt("FF", 16);
//    	int decimalNumber = Integer.parseInt("ff", 16);
    	
    	Log.e(TAG, "Link Qual => " + Integer.toHexString(buffer[6]));   //prints f in hex = 66
    	Log.e(TAG, "Link Qual => " + (char)buffer[5]);  //prints f
//    	Log.e(TAG, "Link Qual => " + decimalNumber);    //prints 15 decimal if "F" or "f" is string value or 255 if "FF"
    	
    	Log.e(TAG, "Link Qual => " + ff);
    	Log.e(TAG, "Link Qual => " + LQnum);
    	

    }
    
    /* Call this from the main Activity to shutdown the connection */
    public void cancel() {
    	
    	if(D)
    		Log.e(TAG, "- ON PAUSE -");

	    if (mmOutStream != null) {
	            try {
	                   mmOutStream.flush();
	            } catch (IOException e) {
	            	if(D)
	                    Log.e(TAG, "ON PAUSE: Couldn't flush output stream.", e);
	            }
	    }
	
	    try     {
	            mmSocket.close();
	            if(D)
	            	Log.e(TAG, "++ON PAUSE: Socket Closed.");
	    } catch (IOException e2) {
	    	if(D)
	            Log.e(TAG, "ON PAUSE: Unable to close socket.", e2);
	    }
    }
    
    
}