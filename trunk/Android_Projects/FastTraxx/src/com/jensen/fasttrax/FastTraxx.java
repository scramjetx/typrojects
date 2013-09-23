//NOTES:

//To Do
//accel still doesn't seem to work right once on the fasttrak. hangs program sometimes
//see how long write to BT takes and bt.run takes.  maybe can speed them up.
	//seems to be very quick on the pic end..send serial commands are executed instant as far as I can tell
	//interestingly buttons are near instant, touch control seems to be the bottle neck. A bit of a lag.. another thread?
//ThinBT had a handler for the messages so maybe that's faster than waiting for it in the main UI thread
//have to learn how threads work more and pass info and share things.  BT is already a separate thread anyway but still slow
//need to translate the duty cycle to something meaningful depending on the tilt angle
//tough to figure out Left and right motor speeds based on accel angles.  Look at AccelControlPWM to implement 

//DONE
//duty cycle calc and attach to message sent out
//removed all command echos
//button control works near instantly
//accel control works pretty quickly


package com.jensen.fasttrax;

import java.io.IOException;
import java.text.DecimalFormat;
import java.util.UUID;
import android.R.string;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.Context;
import android.content.pm.ActivityInfo;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.os.Vibrator;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.CompoundButton.OnCheckedChangeListener;

public class FastTraxx extends Activity {
	 private static final String TAG = "GarageOPENER";
     private static final boolean D = false;			//false if don't want debug logs created
     private static final boolean DA = false;			//false if don't want debug log of accel data created
     private static final boolean BTconnect = true; 	//false if don't want bluetooth routines run
     private static final boolean PICcontrol = true;	//tell if pic is controlling IOs if true, false if BT module is
     private static final int VIB_LENGTH = 30;			//length to vib on button press
//     private static final int PITTER_INTERVAL = 200;		//update rate for drawing = 1/PITTER_INTERVAL (ms) = ??Hz
     private static final int LINKUPDATE_RATE = 2000;	//link update rate
     private static final int PITTERUPDATE_RATE = 100;	//pitter update for controls
     
     
     private static final int FWD_Y = 130;
     private static final int REV_Y = 260;
     
     //GPIO 7 - powerup command used to init as output and drive high
     private static final byte [] cmdLFgo =   {'S','*',',','0','2','0','0',13};
     private static final byte [] cmdLFstop = {'S','*',',','0','2','0','2',13};
	 
	 //GPIO 9 - no powerup command available but used anyway.  Starts out high.
     private static final byte [] cmdLRgo =   {'S','*',',','0','4','0','0',13};
     private static final byte [] cmdLRstop = {'S','*',',','0','4','0','4',13};
     
	//GPIO 10 - needs init command to drive high. starts as input.
     private static final byte [] cmdRFgo =   {'S','*',',','0','8','0','0',13};
     private static final byte [] cmdRFstop = {'S','*',',','0','8','0','8',13};
    
	//GPIO 11 - needs init command to drive high. starts as input.
     private static final byte [] cmdRRgo =   {'S','&',',','8','0','0','0',13};
     private static final byte [] cmdRRstop = {'S','&',',','8','0','8','0',13};

     
     //PIC motor control commands with PWM duty cycle
     private static final byte [] picLFgo =   {'#','l','f','9','9'};
     private static final byte [] picLstop =  {'#','l','s','0','0'};
     private static final byte [] picLRgo =   {'#','l','r','9','9'};
     private static final byte [] picRFgo =   {'#','r','f','9','9'};
     private static final byte [] picRstop =  {'#','r','s','0','0'};
     private static final byte [] picRRgo =   {'#','r','r','9','9'};

     
     private static final double Xmin = -2.0;
     private static final double Xmax = 2.0;
     private static final double Ymin = -2.0;
     private static final double Ymax = 2.0;
     private static final double Zmin = 9.2;
     private static final double Zmax = 9.3;
     
     int LQcount = 0;						//tracks how often to get link quality
     
     
     DecimalFormat value = new DecimalFormat("#0.0"); // format floats for printing to phone screen
     
     boolean retryConn = false;	//reflects value of checkbox to retry connection continuously or not
    
     boolean LFflag = false;	//flags for state of buttons on or off. So BT command only sent once while touching in that region
     boolean LSflag = false;
     boolean LRflag = false;	
     
     boolean RFflag = false;
     boolean RSflag = false;
     boolean RRflag = false;
     
     Button LFbutton = null;
     Button LSbutton = null;
     Button LRbutton = null;
     
     Button RFbutton = null;
     Button RSbutton = null;
     Button RRbutton = null;
     
     Button STOPbutton = null;		//kill all motors
     
     CheckBox AccelControl = null;  //enable accelerometer control via tilting phone
     String cmdLast = "";			//accels should only send the cmd once when in that region so don't overflow cmd buffer
     TextView LQtextview = null;
    
     
     //used for coordinates of multi-touch
     //init to stop button area
     float [] X = {200, 600};
     float [] Y = {150, 150};
     
     float leftTouchX = 200;
     float leftTouchY = 150;
     float rightTouchX = 600;
     float rightTouchY = 150;
     
     boolean vibLeftFlag = false;	//only vibrate once on touch of stop button.  Or send BT message once while touching in that region
     boolean vibRightFlag = false;
     
     private Handler pitterHandler = new Handler();
     private Handler linkQualPitterHandler = new Handler();
     
     SensorManager sm = null;
     float xSample = 0f; // store samples on sensor change then used when pitter fires
 	 float ySample = 0f;
 	 float zSample = 0f;
 	
  	 float DCx = 0;
  	 float DCy = 0;
	 byte[] DCxbytes = null;
	 byte[] DCybytes = null;
	
     private BluetoothAdapter mBluetoothAdapter = null;
     private BluetoothSocket btSocket = null;
     
     private ConnectedThread myBT;
     
     //BT Firefly module in this mode by default
     // Well known SPP UUID (will *probably* map to
     // RFCOMM channel 1 (default) if not in use);
     // see comments in onResume().
     private static final UUID MY_UUID =
                     UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");

     // ==> hardcode your server's MAC address here <==
     private static String address = "00:06:66:01:E3:B0";   //garage door -> 00:06:66:01:E3:BD

     
     private SensorManager mSensorManager;
		private final SensorEventListener mSensorListener = new SensorEventListener() {

			public void onSensorChanged(SensorEvent se) {
				synchronized (this) {

					if (se.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
						if(DA)
							Log.e(TAG, "Accels X: " + se.values[0] + "  Y: " + se.values[1] + "  Z: " + se.values[2]);

							xSample = se.values[0];
							ySample = se.values[1];
							zSample = se.values[2];

						}
				}// end synchronize
			}// end on sensor changed

			@Override
			public void onAccuracyChanged(Sensor arg0, int arg1) {
				
			}
	};
	
     /** Called when the activity is first created. */
     @Override
     public void onCreate(Bundle savedInstanceState) {
             super.onCreate(savedInstanceState);
             setContentView(R.layout.main);

             mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);

     		// Can Register other sensors in here then read values in
     		// onSensorChanged above
     		mSensorManager.registerListener(mSensorListener, mSensorManager
     				.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
     				SensorManager.SENSOR_DELAY_GAME);

     		// get reference to SensorManager
     		sm = (SensorManager) getSystemService(SENSOR_SERVICE);
     		
     		
           //---change to landscape mode---
             setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
             
            initPitter();
             
             
             FrameLayout main = (FrameLayout) findViewById(R.id.FrameLayoutMain);
             
             main.setOnTouchListener(new View.OnTouchListener() {
         		public boolean onTouch(View v, MotionEvent e) {
         			
         			
         			//set a system time touch event tag and on a timer pitter if it gets too old then reset back to zero
         			//how do you know your touching if your finger doesn't move? 
         			for (int i=0; i< e.getPointerCount(); i++) { 
         				X[i] = e.getX(i);
         				Y[i] = e.getY(i);
         				
         				
	         			if(X[i] < 370){						//Left stick command
	         				leftTouchX = X[i];
	         				leftTouchY = Y[i];
	         			}
	         			else if(X[i] > 550){				//Right stick command
	         				rightTouchX = X[i];
	         				rightTouchY = Y[i];
	         			}
         			}  //end for loop
         			
         			if(D)
         				Log.e("Pointer", "1stCoord: x= " + X[0] + ", y=" + Y[0] + "|**| " + "2ndCoord: x= " + X[1] + ", y=" + Y[1]); 
         			
             			
             			
         			//not sure what this is for but ontouch returns a boolean
         			return true;
     			}
             });
             
             if (D)
                     Log.e(TAG, "+++ ON CREATE +++");

                             
             //don't do anything bluetooth if debugging the user interface
             if(BTconnect){
             	
             	mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
             	
             	if (mBluetoothAdapter == null) {
             		Toast.makeText(this,
             				"Bluetooth is not available.",
             				Toast.LENGTH_LONG).show();
             		finish();
             		return;
             	}
             	
             	if (!mBluetoothAdapter.isEnabled()) {
             		Toast.makeText(this,
             				"Please enable your BT and re-run this program.",
             				Toast.LENGTH_LONG).show();
             		finish();
             		return;
             	}
             	
             	if (D)
             		Log.e(TAG, "+++ DONE IN ON CREATE, GOT LOCAL BT ADAPTER +++");
             	
             }

             LQtextview = (TextView) findViewById(R.id.TextViewLQ);
             
             STOPbutton = (Button) findViewById(R.id.ButtonStop);
             
             AccelControl = (CheckBox) findViewById(R.id.CheckBoxAccelControl);
             AccelControl.setTextColor(Color.RED);
             AccelControl.setTextSize(22);
             
             LFbutton = (Button) findViewById(R.id.ButtonLF);
             LSbutton = (Button) findViewById(R.id.ButtonLS);
             LRbutton = (Button) findViewById(R.id.ButtonLR);
             RFbutton = (Button) findViewById(R.id.ButtonRF);
             RSbutton = (Button) findViewById(R.id.ButtonRS);
             RRbutton = (Button) findViewById(R.id.ButtonRR);
             
             LSbutton.setBackgroundColor(Color.RED);
             RSbutton.setBackgroundColor(Color.RED);
             
             //disable buttons for now but maintain for visual reference
//             LFbutton.setEnabled(false);
//             LSbutton.setEnabled(false);
//             LRbutton.setEnabled(false);
//             RFbutton.setEnabled(false);
//             RSbutton.setEnabled(false);
//             RRbutton.setEnabled(false);
             
//             LFbutton.offsetLeftAndRight(pixels);
//             LFbutton.performClick();  //manual way to click
             
             STOPbutton.setOnClickListener(new OnClickListener() {
      			public void onClick(View v) {
      				
      				//disable accel control so doesn't command to drive again accidentally
      				AccelControl.setChecked(false);
      				
      				//command stop all motors
      				DriveDir("ls", true);
      				DriveDir("rs", true);
      				
      			// Get instance of Vibrator from current Context
      				Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
      				vib.vibrate(VIB_LENGTH + 30);  // Vibrate for 50 milliseconds
      				
      				vib.vibrate(VIB_LENGTH + 30);  // Vibrate for 50 milliseconds
      			}
      		}); // end listener
             
             LFbutton.setOnClickListener(new OnClickListener() {
     			public void onClick(View v) {
     				
     				// Get instance of Vibrator from current Context
     				Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
     				vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds

     				//flip flag
//     				if(LFflag == false)
//     					LFflag = true;
     				//else make it so you can push button again and turn it off. put stop button in place instead.
//     				else
//     					LFflag = false;
     				
     				//command current state
     				//DriveDir("lf", true);
     				picLFgo[3] = '1';
     				picLFgo[4] = '0';
     				
     				String value = new String(picLFgo); 
     		    	Log.e(TAG, "MESSAGE SENT:  " + value);
     		    	
     				myBT.write(picLFgo);

     			}
     		}); // end listener
             
             LSbutton.setOnClickListener(new OnClickListener() {
 	  			public void onClick(View v) {
// 	  				boolean tempFlag = true;	//command to stop active direction
 	  				
 	  				// Get instance of Vibrator from current Context
 	  				Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
 	  				vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds
 	
 	  				//determine which direction is active
 	  				//then stop that direction by sending a false command
// 	  				if(LFflag == true){
// 	  					LFflag = false;	//need to flip flag state
 	  					DriveDir("ls", true);
// 	  				}
// 	  				else{
// 	  					LRflag = false;	//need to flip flag state	
// 	  					DriveDir("LR", tempFlag);
// 	  				}
 	  			}
 	  		}); // end listener
             
	         LRbutton.setOnClickListener(new OnClickListener() {
	  			public void onClick(View v) {
	  				
	  				// Get instance of Vibrator from current Context
	  				Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
	  				vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds
	
	  				//flip flag
//	  				if(LRflag == false)
//	  					LRflag = true;
	  			//else make it so you can push button again and turn it off. put stop button in place instead.
//	  				else
//	  					LRflag = false;
	  				
	  				//command current state
	  				DriveDir("lr", true);
	
	  			}
	  		}); // end listener
	         
	         
	         
			 RFbutton.setOnClickListener(new OnClickListener() {
				public void onClick(View v) {
						
					// Get instance of Vibrator from current Context
					Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
					vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds
			
					//flip flag
//					if(RFflag == false)
//						RFflag = true;
					//else make it so you can push button again and turn it off. put stop button in place instead.
//					else
//						RFflag = false;
					
					//command current state
					DriveDir("rf", true);
			
				}
			}); // end listener
			 
            RSbutton.setOnClickListener(new OnClickListener() {
 	  			public void onClick(View v) {
// 	  				boolean tempFlag = false;   //active command to be stopped
 	  				
 	  				// Get instance of Vibrator from current Context
 	  				Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
 	  				vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds
 	
 	  				//determine which direction is active
 	  				//then stop that direction by sending a false command
// 	  				if(RFflag == true){
// 	  					RFflag = false;  //need to flip flag state
 	  					DriveDir("rs", true);
// 	  				}
// 	  				else{
// 	  					RRflag = false;	//need to flip flag state
// 	  					DriveDir("RR", tempFlag);
// 	  				}
 	  			}
 	  		}); // end listener
            
			RRbutton.setOnClickListener(new OnClickListener() {
				public void onClick(View v) {
						
					// Get instance of Vibrator from current Context
					Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
					vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds
			
					//flip flag
//					if(RRflag == false)
//						RRflag = true;
					//else make it so you can push button again and turn it off. put stop button in place instead.
//					else
//						RRflag = false;
					
					//command current state
					DriveDir("rr", true);
			
				}
			}); // end listener
	
			
			
			
     }//end onCreate 

     @Override
     public void onStart() {
             super.onStart();
             if (D)
                     Log.e(TAG, "++ ON START ++");
     }

     @Override
     public void onResume() {
             super.onResume();
             
           //don't do anything bluetooth if debugging the user interface
           if(BTconnect) 
         	  initBT();
           
           initPitter();
           
           mSensorManager.registerListener(mSensorListener, mSensorManager
   				.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
   				SensorManager.SENSOR_DELAY_GAME);
     }

     @Override
     public void onPause() {
             super.onPause();
             
           //don't do anything bluetooth if debugging the user interface
           if(BTconnect)
         	  myBT.cancel();
           
           //kill timer handler
           killPitter();
     }

     @Override
     public void onStop() {
             super.onStop();
             if (D)
             	Log.e(TAG, "-- ON STOP --");
             
             killPitter();
             
             mSensorManager.unregisterListener(mSensorListener);
     }

     @Override
     public void onDestroy() {
             super.onDestroy();
             if (D)
             	Log.e(TAG, "--- ON DESTROY ---");
     }
    
     //Vib for millisecond count that is a constant
     public void Vib(){
    	// Get instance of Vibrator from current Context
			Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
			vib.vibrate(VIB_LENGTH);  // Vibrate for 50 milliseconds
     }
     
     //stall system for spec'd amount of time
     public void sleep(long millis){
     	
    	 long timeOld = System.currentTimeMillis();
        long timeNew = 0;
        long temp = timeNew - timeOld;
        
        while(temp < millis){
     	   timeNew = System.currentTimeMillis();
     	   temp = timeNew - timeOld;
        }
    	
    }   

    //init routine for bluetooth with commands to setup IO port correctly
    public void initBT(){
     	 if (D) {
              Log.e(TAG, "+ ON RESUME +");
              Log.e(TAG, "+ ABOUT TO ATTEMPT CLIENT CONNECT +");
	         }
	
     	 
	         // When this returns, it will 'know' about the server,
	         // via it's MAC address.
	         BluetoothDevice device = mBluetoothAdapter.getRemoteDevice(address);
	
	         //returns firefly e350
	         Log.e(TAG, "device name: " + device.getName());
	         
	         
	         // We need two things before we can successfully connect
	         // (authentication issues aside): a MAC address, which we
	         // already have, and an RFCOMM channel.
	         // Because RFCOMM channels (aka ports) are limited in
	         // number, Android doesn't allow you to use them directly;
	         // instead you request a RFCOMM mapping based on a service
	         // ID. In our case, we will use the well-known SPP Service
	         // ID. This ID is in UUID (GUID to you Microsofties)
	         // format. Given the UUID, Android will handle the
	         // mapping for you. Generally, this will return RFCOMM 1,
	         // but not always; it depends what other BlueTooth services
	         // are in use on your Android device.
	         try {
	                 btSocket = device.createRfcommSocketToServiceRecord(MY_UUID);
	                 Log.e(TAG, "ON RESUME: Socket created!");
	         } catch (IOException e) {
	        	 if(D)
	        		 Log.e(TAG, "ON RESUME: Socket creation failed.", e);
	        	 
	         }
	        

	         
	         
	         // Discovery may be going on, e.g., if you're running a
	         // 'scan for devices' search from your handset's Bluetooth
	         // settings, so we call cancelDiscovery(). It doesn't hurt
	         // to call it, but it might hurt not to... discovery is a
	         // heavyweight process; you don't want it in progress when
	         // a connection attempt is made.
	         mBluetoothAdapter.cancelDiscovery();
	         
	         retryConn = false;
	         myBT = new ConnectedThread(btSocket, retryConn);

	        if(!PICcontrol){
	        	
	        	//ST,255 enables remote configuration forever...need this if resetting
	        	//PIO4 is held high on powerup then toggled 3 times to reset
	        	
	        	
	        	//GPIO Commands to BT device page 15 of commands datasheet
	        	byte [] cmdMode = {'$','$','$'};
	        	myBT.write(cmdMode);
	        	myBT.run();
	        	
	        	//  S@,8080 temp sets GPIO-7 to an output
	        	byte [] cmd1 = {'S','@',',','8','0','8','0',13};
	        	myBT.write(cmd1);
	        	myBT.run(); 
	        	
	        	// S&,8000 drives GPIO-7 high
	        	byte [] cmd2 = {'S','&',',','8','0','8','0',13};
	        	myBT.write(cmd2);
	        	myBT.run();
	        	
	        	//make it so cmd mode won't timeout even after factory reset 
	        	byte [] cmd3 = {'S','T',',','2','5','5',13};
	        	myBT.write(cmd3);
	        	myBT.run();
	        	
	        	//GPIO 8 doesn't seem to work..only an output?
	        	
	        	//**GPIO 9,10,11 already set to outputs on startup by bluetooth firmware
	        	//init GPIO 9 to high so motor doesn't turn
	        	byte [] cmd5 = {'S','*',',','0','2','0','2',13};
	        	myBT.write(cmd5);
	        	myBT.run();
	        	
	        	//init GPIO 10 to high so motor doesn't turn
	        	byte [] cmd6 = {'S','*',',','0','4','0','4',13};
	        	myBT.write(cmd6);
	        	myBT.run();
	        	
	        	//init GPIO 11 to high so motor doesn't turn
	        	byte [] cmd7 = {'S','*',',','0','8','0','8',13};
	        	myBT.write(cmd7);
	        	myBT.run();
	        	
	        }
	         
     }
      
    
     public void flipSwitch(){
//        S&,8080 drives GPIO-7 high
        byte [] cmd2 = {'S','&',',','8','0','8','0',13};
        myBT.write(cmd2);
        myBT.run();
       
        sleep(100);
      
//        S&,8000 drives GPIO-7 low
        byte [] cmd3 = {'S','&',',','8','0','0','0',13};
        myBT.write(cmd3);
        myBT.run();
        
    }

     public void initPitter(){
//     	mStartTime = System.currentTimeMillis();
         pitterHandler.removeCallbacks(mUpdateTimeTask);
         pitterHandler.postDelayed(mUpdateTimeTask, PITTERUPDATE_RATE);
     	
         linkQualPitterHandler.removeCallbacks(m2UpdateTimeTask);
         linkQualPitterHandler.postDelayed(m2UpdateTimeTask, LINKUPDATE_RATE);
         
     }
     
     public void killPitter(){
     	pitterHandler.removeCallbacks(mUpdateTimeTask);
     	linkQualPitterHandler.removeCallbacks(m2UpdateTimeTask);
     }
     
     
     
 //************************************
 //***   MAIN LOOP   ******************
 //************************************
     private Runnable mUpdateTimeTask = new Runnable() {
    	   public void run() {
    		   long start = SystemClock.uptimeMillis();
    		   long end = 0;
    		   long diff = 0;
    		   
    		   if(D)
    			   Log.e(TAG, "Pitter....");
    		   
//    		   if(LFbutton.isSelected())
//    			   LFbutton.performClick();
    		   
    		   //touch control scheme.  can disable and go back to button if enable the buttons
    		   //TouchControl();
    		   
    		   
    		   //allow control via tilting phone if checkbox is checked
    		   if(AccelControl.isChecked())
    			   AccelControlPWM();
    		   
    		     		   
    		   end = SystemClock.uptimeMillis();
    		   diff = end - start;
    		   
    		   if(D)
    			   Log.e(TAG, "Pitter....took " + diff + " ms" );
    		   
    		   if(diff>PITTERUPDATE_RATE)
    			   pitterHandler.postAtTime(this, start + 1);
    		   else
    			   pitterHandler.postAtTime(this, start + PITTERUPDATE_RATE);
    	      
    	   }
    	};

 //************************************
 //***  END MAIN LOOP   ******************
 //************************************
	

    	
//************************************
 //***   Link Quality Update LOOP   ******************
 //************************************
     private Runnable m2UpdateTimeTask = new Runnable() {
    	   public void run() {
    		   long start = SystemClock.uptimeMillis();
//    		   long end = 0;
//    		   long diff = 0;
    		   
//    		   if(D)
    			   Log.e(TAG, "Link Quality Pitter....");
    		   
    		   
    		   //grab link quality
    	    
    		
    		   if(AccelControl.isChecked()) 
//    			   getLinkQual();
    		   
//    		   if(AccelControl.isChecked()){ 
    			
//    			   if(LQcount > LQCOUNTMAX){
//    				   LQcount = 0;
//    			   }
//    		   }
//    		   LQcount++;
    		   
//    		   end = SystemClock.uptimeMillis();
//    		   diff = end - start;
    		   
//    		   if(D)
//    			   Log.e(TAG, "LinkQual Pitter....took " + diff + " ms" );
    		   
//    		   if(diff>LINKUPDATE_RATE)
//    			   linkQualPitterHandler.postAtTime(this, start + 1);
//    		   else
    			   linkQualPitterHandler.postAtTime(this, start + LINKUPDATE_RATE);
    	      
    	   }
    	};

 //************************************
 //***  END Link Quality LOOP   ******************
 //************************************
    	
	public void getLinkQual(){
		
		//init link quality report at 5hz
        byte [] cmd1 = {'l',13};
        myBT.write(cmd1);
        myBT.run();			//tells you RSSI ON
    	myBT.run();			//spits out stream of link qual
        myBT.parseLinkQuality(); 	//parse if from that stream
        
        LQtextview.setText(Integer.toString(myBT.linkQual));
        Log.e(TAG, "Link Quality: " + myBT.linkQual);
        
        //send again to disable
        myBT.write(cmd1);
    	myBT.run();
    }
        
        
    	
    	
    public void AccelControl(){
        	
    	if(cmdLast != "lsrs" && xSample < 2 && xSample > -2 && ySample < 2 && ySample > -2 && zSample > Zmax){     //STOP
    		DriveDir("ls", true);
    		DriveDir("rs", true);
    		cmdLast = "lsrs";
    	}
    	else if(cmdLast != "lfrf" && xSample < Xmin && ySample < 2 && ySample > -2 && zSample < Zmax){				//FWD
    		DriveDir("lf", true);
    		DriveDir("rf", true);
    		cmdLast = "lfrf";
    	}
    	else if(cmdLast != "rfls" && xSample < Xmin && ySample < Ymin && zSample < Zmin){							//FWD LEFT
    		DriveDir("rf", true);
    		DriveDir("ls", true);
    		cmdLast = "rfls";
    	}
    	else if(cmdLast != "lfrs" && xSample < Xmin && ySample > Ymax && zSample < Zmin){							//FWD RIGHT
    		DriveDir("lf", true);
    		DriveDir("rs", true);
    		cmdLast = "lfrs";
    	}
    	else if(cmdLast != "lrrf" && xSample < 2 && xSample > -2 && ySample < Ymin && zSample < Zmin){ 			    //LEFT FAST
    		DriveDir("lr", true);
    		DriveDir("rf", true);
    		cmdLast = "lrrf";
    	}
    	else if(cmdLast != "lrrr" && xSample > Xmax && ySample < 2 && ySample > -2 && zSample < Zmax){ 			    //REV 
    		DriveDir("lr", true);
    		DriveDir("rr", true);
    		cmdLast = "lrrr";
    	}
    	else if(cmdLast != "lsrr" && xSample > Xmax && ySample < Ymin && zSample < Zmin){							//REV LEFT
    		DriveDir("ls", true);
    		DriveDir("rr", true);
    		cmdLast = "lsrr";
    	}
    	else if(cmdLast != "lrrs" && xSample > Xmax && ySample > Ymax && zSample < Zmin){							//REV RIGHT
    		DriveDir("lr", true);
    		DriveDir("rs", true);
    		cmdLast = "lrrs";
    	}
    	else if(cmdLast != "lfrr" && xSample < 2 && xSample > -2 && ySample > Ymax && zSample < Zmin){ 			    //RIGHT FAST
    		DriveDir("lf", true);
    		DriveDir("rr", true);
    		cmdLast = "lfrr";
    	}
    	
    	
    }
    
    public void AccelControlPWM(){
    	   	
    	//calc duty cycle for x and y
    	DCx = xSample * 14;
    	if(DCx<0) DCx = -DCx;
    	if(DCx>99) DCx = 99;
    	DCxbytes = Float.toString(DCx).getBytes();
    	if(DCx < 10){
    		DCxbytes[1] = DCxbytes[0];
    		DCxbytes[0] = '0';
    	}

    	
    	DCy = ySample * 14;
    	if(DCy<0) DCy = -DCy;
    	if(DCy>99) DCy = 99;
    	DCy = 99-DCy;
    	DCybytes = Float.toString(DCy).getBytes();
    	if(DCy < 10){
    		DCybytes[1] = DCybytes[0];
    		DCybytes[0] = '0';
    	}
    	
    	
    	//accel control PWM reduced to 4 quadrants vs. the on/off control of each motor has more quadrants
    	if(xSample < 2 && xSample > -2 && ySample < 2 && ySample > -2 && zSample > Zmax){     //STOP
 //   		DriveDirPWM("ls", true);
 //   		DriveDirPWM("rs", true);
    		cmdLast = "lsrs";
    	}
    	else if(xSample < Xmin && ySample < 0  ){							//FWD LEFT
    		DriveDirPWM("rf", DCxbytes, true);
    		DriveDirPWM("lf", DCybytes, true);
    		cmdLast = "rfls";
    	}
    	else if(xSample < Xmin && ySample > 0){							//FWD RIGHT
    		DriveDirPWM("lf", DCybytes, true);
    		DriveDirPWM("rf", DCxbytes, true);
    		cmdLast = "lfrs";
    	}
    	else if(xSample < 2 && xSample > -2 && ySample < Ymin && zSample < Zmin){ 			    //LEFT FAST
 //   		DriveDirPWM("lr", true);
 //   		DriveDirPWM("rf", true);
    		cmdLast = "lrrf";
    	}
    	else if(xSample > Xmax && ySample < Ymin && zSample < Zmin){							//REV LEFT
//    		DriveDirPWM("lr", true);
//    		DriveDirPWM("rr", true);
    		cmdLast = "lsrr";
    	}
    	else if(xSample > Xmax && ySample > Ymax && zSample < Zmin){							//REV RIGHT
//    		DriveDirPWM("lr", true);
//    		DriveDirPWM("rr", true);
    		cmdLast = "lrrs";
    	}
    	else if(xSample < 2 && xSample > -2 && ySample > Ymax && zSample < Zmin){ 			    //RIGHT FAST
//    		DriveDirPWM("lf", true);
//    		DriveDirPWM("rr", true);
    		cmdLast = "lfrr";
    	}
    	
    	
    }
    
    
    public void TouchControl(){
    	//gotta get the heavy stuff outta the touch listener but what
		   //like the button color drawing is probably pretty heavy. only do that on the pitter cycle instead
		   //of everytime someone touches a button.
		   //when call drive dir it sends the BT command
		   //so somehow in touch flag the command to be sent and then the pitter only sends
		   //the command if it hasn't been sent once before.  That is built into if statements
		   //in the touchlistener but don't want to navigate that if statement deal more than once
		   //for speed purposes do we? 
		   //make another flag that says when the command has been sent and only is reset if an opposing
		   //command is sent after ..do that portion of If statements in this pitter.
		   if(leftTouchY < FWD_Y){						//Left Forward
				
				if(LFflag==false){
//					LFbutton.setBackgroundColor(Color.RED);
// 				LSbutton.setBackgroundColor(Color.TRANSPARENT);
// 				LRbutton.setBackgroundColor(Color.TRANSPARENT);
// 				vibLeftFlag = false;
 				
					DriveDir("lf", true);
					LFflag = true;
				}
				
				LSflag = false;
				LRflag = false;
			}
	   		else if(leftTouchY > FWD_Y && leftTouchY < REV_Y){  //Left Stop
				
				if(LSflag == false){
//					LFbutton.setBackgroundColor(Color.TRANSPARENT);
// 				LSbutton.setBackgroundColor(Color.RED);
// 				LRbutton.setBackgroundColor(Color.TRANSPARENT);
 				
					DriveDir("ls",true);
					Vib();
					LSflag = true;
				}
				
				LFflag = false;
				LRflag = false;
			}
			else if(leftTouchY > REV_Y){ 					//Left Reverse
				
				if(LRflag == false){
//					LFbutton.setBackgroundColor(Color.TRANSPARENT);
// 				LSbutton.setBackgroundColor(Color.TRANSPARENT);
// 				LRbutton.setBackgroundColor(Color.RED);
 				
					DriveDir("lr", true);
					LRflag = true;
				}
				
				LFflag = false;
				LSflag = false;
				
			}
			
		   if(rightTouchY < FWD_Y){					//Right Forward
				
				if(RFflag==false){
//					RFbutton.setBackgroundColor(Color.RED);
// 				RSbutton.setBackgroundColor(Color.TRANSPARENT);
// 				RRbutton.setBackgroundColor(Color.TRANSPARENT);
 				
					DriveDir("rf", true);
					RFflag = true;
				}
				
				RSflag = false;
				RRflag = false;
			}
 		else if(rightTouchY > FWD_Y && rightTouchY < REV_Y){  //Right Stop
 			
				if(RSflag == false){
//					RFbutton.setBackgroundColor(Color.TRANSPARENT);
// 				RSbutton.setBackgroundColor(Color.RED);
// 				RRbutton.setBackgroundColor(Color.TRANSPARENT);
 				
					DriveDir("rs",true);
					Vib();
					RSflag = true;
				}
				
				RFflag = false;
				RRflag = false;

 		}
 		else if(rightTouchY > REV_Y){ 					//Right Reverse
 			
				if(RRflag == false){
//					RFbutton.setBackgroundColor(Color.TRANSPARENT);
// 				RSbutton.setBackgroundColor(Color.TRANSPARENT);
// 				RRbutton.setBackgroundColor(Color.RED);
 				
					DriveDir("rr", true);
					RRflag = true;
				}
				
				RFflag = false;
				RSflag = false;
 		}
    }
    
    
    
     public void DriveDir(String button, boolean state){
    	 //state reflects whether button is on or off which a high value sent turns off the motor
    	 
    	 //commands sent based on boolean if pic is attached or not
    	 
    	 byte [] cmdSent = null;
    	 byte [] cmdSentStop = null;  	//used to make sure if LF commanded that LR isn't also commanded. Would ruin car.

    	 //state machine for all possible button presses
    	 //sets the value for cmdSent
    	 if(button == "lf"){
    		 if(state == true){
    			 if(!PICcontrol){
    				 cmdSent = cmdLFgo;
    				 cmdSentStop = cmdLRstop;
    			 }else{
    				 cmdSent = picLFgo;	
    			 }
    		 }
    		 else{
    			 if(!PICcontrol)
    				 cmdSent = cmdLFstop;
    		 }
    	 }else if(button == "ls"){
    		 if(state == true){
    			 if(!PICcontrol){
    				 cmdSent = cmdLFstop;
    				 cmdSentStop = cmdLRstop;
    			 }else{
    				 cmdSent = picLstop; 
    			 }
    		 }
    	 }else if(button == "lr"){
    		 if(state == true){
    			 if(!PICcontrol){
    				 cmdSent = cmdLRgo;
    				 cmdSentStop = cmdLFstop;
    			 }else{
    				 cmdSent = picLRgo;
    			 }
    		 }
    		 else{
    			 if(!PICcontrol)
    				 cmdSent = cmdLRstop;
    		 }
    	 }else if(button == "rf"){
    		 if(state == true){
    			 if(!PICcontrol){
    				 cmdSent = cmdRFgo;
    				 cmdSentStop = cmdRRstop;
    			 }else{
    				 cmdSent = picRFgo;
    			 }
    		 }
    		 else{
    			 if(!PICcontrol)
    				 cmdSent = cmdRFstop;
    		 }
    	 }else if(button == "rs"){
    		 if(state == true){
    			 if(!PICcontrol){
    				 cmdSent = cmdRFstop;
    				 cmdSentStop = cmdRRstop;
    			 }else{
    				 cmdSent = picRstop;
    			 }
    		 }
    	 }else if(button == "rr"){
    		 if(state == true){
    			 if(!PICcontrol){
    				 cmdSent = cmdRRgo;
    				 cmdSentStop = cmdRFstop;
    			 }else{
    				 cmdSent = picRRgo;
    			 }
    		 }
    		 else{
    			 if(!PICcontrol)
    				 cmdSent = cmdRRstop;
    		 }
    	 }
    	
    	
    	 //state machine to make sure forward and reverse not commanded on same motor at same time
    	 //motor damage can result.  Not needed for PIC control because it does it inherently
    	 //sets the value for cmdSentStop

    	 if(!PICcontrol){
    		 if(cmdSentStop != null){
    			 //stop Forward and reverse from being sent at same time
    			 myBT.write(cmdSentStop);
    			 myBT.run();
    		 }
    	 }

    	 //send command chosen
    	 myBT.write(cmdSent);
    	 //myBT.run();			//no commands echo'd from PIC when a cmd is sent
    	 
    	 
     }
     
     public void DriveDirPWM(String button, byte [] DC, boolean state){
    	 //state reflects whether button is on or off which a high value sent turns off the motor
    	 
    	 //commands sent based on boolean if pic is attached or not
    	 
    	 byte [] cmdSentPWM = null;
    	
    	 
    	 //state machine for all possible button presses
    	 //sets the value for cmdSent
    	 if(button == "lf"){
   			 cmdSentPWM = picLFgo;	
    	 }else if(button == "ls"){
			 cmdSentPWM = picLstop; 
    	 }else if(button == "lr"){
			 cmdSentPWM = picLRgo;
    	 }else if(button == "rf"){
			 cmdSentPWM = picRFgo;
    	 }else if(button == "rs"){
			 cmdSentPWM = picRstop;
    	 }else if(button == "rr"){
			 cmdSentPWM = picRRgo;
    	 }
    	
    	 
    	
    	cmdSentPWM[3] = DC[0];
    	cmdSentPWM[4] = DC[1];
    	
    	String value = new String(cmdSentPWM); 
    	Log.e(TAG, "MESSAGE SENT:  " + value);
    	 
    	 //send command chosen
    	 myBT.write(cmdSentPWM);
    	 //myBT.run();			//no commands echo'd from PIC when a cmd is sent
    	 
    	 
     }
}