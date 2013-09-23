//NOTES:

package com.jensen.GarageOpener;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import android.bluetooth.BluetoothSocket;
import android.util.Log;

public class ConnectedThread extends Thread {
    private BluetoothSocket mmSocket = null;
    private InputStream mmInStream = null;
    private OutputStream mmOutStream = null;
    private String TAG = "GarageOPENER";
    private boolean D = false;					//tracks if debug messages are printed
    boolean BTconnStatus = false;		//whether valid connection made to slave device
    
    public ConnectedThread(BluetoothSocket socket) {
    	mmSocket = socket;
        InputStream tmpIn = null;
        OutputStream tmpOut = null;
        boolean conn = false;

        //connects the socket and prepares it for input/output stream connection	
        conn = connectSocket();

        
    	// Get the input and output streams, using temp objects because
        // member streams are final
        //only create streams if socket connected
        if(conn)
        {
	        try {
	            tmpIn = socket.getInputStream();
	            tmpOut = socket.getOutputStream();
	            
	            mmInStream = tmpIn;
	            mmOutStream = tmpOut;
	            
	            if(D)
	            	Log.e(TAG,"GOT STREAMS");
	            
	            BTconnStatus = true;
	            
	        } catch (IOException e) {
	        	if(D)
	        		Log.e(TAG,"STREAM CREATION FAILED!!");
	        	 
	        	BTconnStatus = false;
	        	
	        }
        }
        
        	
       
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
        byte[] buffer = new byte[1024];  // buffer store for the stream
        int bytes; // bytes returned from read()

        // Keep listening to the InputStream until an exception occurs
        //this stream is in integer format when it returns the value
        //so look at an ascii table if its text under dec returned to find the ascii value.
        //for sending $$$ you get back 5,67,77,68,13,10 which is Enquiry,C,M,D,CarriageReturn,LineFeed
            try {

            	// Read from the InputStream
                bytes = mmInStream.read(buffer);
                
                if(D)
                	Log.e(TAG, "Bytes to read: " + Integer.toString(bytes));
                
                if(bytes == -1){
                	bytes = 0;
                }
                for(int i = 0; i< bytes; i++){
                	char c;
                	c = (char)buffer[i];
                	
                	//prints the integer value..use ascii lookup table
                	if(D)
                		Log.e(TAG, "Integer: " + String.valueOf(buffer[i]) + "  ASCII: " + String.valueOf(c));
                	
                	//prints ascii char of integer
//                	Log.e(TAG, String.valueOf(c));
                    
                }
                
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