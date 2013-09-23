//NOTES:
//
//***CURRENT STATUS:
//now will connect once on startup.
//if box checked then will continue to try regardless of screen state. makes connection when bt is powerup up after a couple tries
//will display connected, connecting, NOT Connected
//will fire once on connection if retry conn is active
//will work if screen is turned off. hit back arrow to kill threads and everything. hitting home then running app again won't work.
//will flip switch with accel bump perp to screen
//now the program will enable bluetooth on start and disable on shutdown
//accel double tap works! Just double tap the phone on z axis twice within .5sec and it will open the door with screen on or off.
//also will shut down the bluetooth and exit the app on a tapping which makes it so you don't have to handle the phone
//trying out the triple tap. double seems to be false hitting too often as you walk.

//***TO BE COMPLETED:
//flipping retry conn multiple times might cause problems... pause at beginning is annoying cause its trying to connect
//maybe do popup progress bar so blocks user interface while connecting

//change to new project name instead of ThinBTClient

//make button only click not open and close state. flash color briefly somehow

//save state of checkbox persistantly on running program again later

//anything you want to happen if the program is minimized and started again should be put in onResume as it gets run after onCreate
//and when it is minimized and gets focus again without being removed from memory

package com.jensen.ThinBTClient;

import java.io.IOException;
import java.util.UUID;

import android.R.color;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.graphics.drawable.Drawable;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.os.Vibrator;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.CompoundButton.OnCheckedChangeListener;

public class ThinBTClient extends Activity {

	private static final String TAG = "GarageOPENER";
	private static final int SWITCH_DURATION = 150; 		// time to hold switch closed
	private static final boolean D = true; 					// false if don't want debug logs created
	private static final boolean DA = false; 				// debug accel enabled, print accel values
	private static final boolean BTconnect = true; 			// false if don't want bluetooth routines run
	// private boolean BTconnected = false; 				//for telling whether attempt to connect succeeded
	private boolean BTstartupConn = false; 					// only try to connect once at program start
	boolean persistantConn = true; 							// should the program keep trying to connect to the device
	boolean BumpEnable = true; 								// checkbox for enabling double tap
	boolean turnOffBluetoothShutdown = true;				// shutoff bluetooth automatically on program shutdown?
	boolean turnOffProgramOnDoubleTap = true;				// shutoff program if it gets a double tap cause usually going inside anyway
	private Handler mHandler = new Handler();				// used to keep retrying connection to bluetooth
	private Handler bumpHandler = new Handler(); 			// used to only listen to bumps within X amount of milliseconds apart
															// walking shouldn't trigger a bump which opens the door

	Button OpenCloseButton = null;
	LinearLayout mainLayout = null;
	CheckBox RetryConnBox = null;
	CheckBox BumpCheckBox = null;
	TextView StatusBox = null;

	private BluetoothAdapter mBluetoothAdapter = null;
	private BluetoothSocket btSocket = null;

	private ConnectedThread myBT;

	// BT Firefly module in this mode by default
	// Well known SPP UUID (will *probably* map to
	// RFCOMM channel 1 (default) if not in use);
	// see comments in onResume().
	private static final UUID MY_UUID = UUID
			.fromString("00001101-0000-1000-8000-00805F9B34FB");

	// ==> hardcode your bluetooth module MAC address here <==
	private static String address = "00:06:66:01:E3:BD";

	SensorManager sm = null; // for accel
	float xSample = 0f; // store samples on sensor change then used when pitter fires
	float ySample = 0f;
	float zSample = 0f;
	float zSampleLast = 0f;

	boolean accelFlipSwitch = false; // for tapping pocket to fire switch
	boolean accelWait = false; // after flipping switch needs to wait 3 sec before z axis can trip flip again
	boolean disableAccel = false; // stop z trip if vib is active

	int bumpCount = 0; // count the number of bumps
	int bumpsToActivate = 3;  //how many bumps to flip the switch. 
	int bumpTimeWindow = 700; //millisecs to complete the required bump count
	
	Bundle outState = new Bundle();

	private SensorManager mSensorManager;
	private final SensorEventListener mSensorListener = new SensorEventListener() {

		public void onSensorChanged(SensorEvent se) {
			synchronized (this) {

				if (se.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
					if (DA)
						Log.e(TAG, "Accels X: " + se.values[0] + "  Y: "
								+ se.values[1] + "  Z: " + se.values[2]);

					xSample = se.values[0];
					ySample = se.values[1];
					zSample = se.values[2];

					// for vibe event that triggers accels 
					//logic of this area is: if bump in range pos or neg then trigger the first bump.
					// then wait for the bump to settle to normal levels and don't count another bump till then
					// then when the 2nd bump is counted make sure its the same sign so you know phone was tapped and not
					// triggered from walking around which causes pos and neg bumps close together instead of pos pos or neg neg.
					// then if all this happens in < time in postdelayed call the bump won't reset and we'll flip the switch
					//***still doesn't work all that well but better than nothing
					if (BumpEnable && !disableAccel) {
						if ((zSample > 15 && zSample < 40 && accelWait == false) || (zSample < -15 && zSample > -40 && accelWait == false)) {
//							Log.e(TAG, "Z TRIP!!  " + zSample);
							if (bumpCount == 0) {
//								Log.e(TAG, "post delay FIRED");
								bumpHandler.removeCallbacks(bumpTimeTask);
								bumpHandler.postDelayed(bumpTimeTask, bumpTimeWindow);
								
							}

							accelWait = true;

							if (bumpCount <= (bumpsToActivate - 2)) {
								bumpCount++;
								zSampleLast = zSample;
							} else if (bumpCount > (bumpsToActivate - 2)) {
								if (zSampleLast > 15 && zSampleLast < 40 && zSample > 15) {
									bumpCount++;
								} else if (zSampleLast < -15 && zSampleLast > -40 && zSample < -15) {
									bumpCount++;
								}
							
//								Log.e(TAG,"BUMP COUNT: " + bumpCount);
							}
						} else if ((zSample < 8 && zSample > -8 && accelWait == true)) {
							accelWait = false;
						}

						// if a double tap happens before the bumpTimeTask fires
						// and resets the counter then the door will open
						if (bumpCount >= bumpsToActivate) {
							bumpCount = 0;
							bumpHandler.removeCallbacks(bumpTimeTask);
							
//							Log.e(TAG, "++++TRIPLE TAP+++  " + zSample);

							if(BTconnect){
								flipSwitch();
								
								if(turnOffProgramOnDoubleTap){
									//stops the app and calls onStop and onDestroy so everything is shutoff
									finish();
								}
							}
									
						}
					}
				}
			}// end synchronize
		}// end on sensor changed

		@Override
		public void onAccuracyChanged(Sensor arg0, int arg1) {

		}
	};

	// this ensures the accel stays on and the program still samples even when
	// the screen is off
	protected BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(Intent.ACTION_SCREEN_OFF)) {
				mSensorManager.unregisterListener(mSensorListener);
				mSensorManager.registerListener(mSensorListener, mSensorManager
						.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
						SensorManager.SENSOR_DELAY_NORMAL);
			}
		}
	};

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);

		if (D)
			Log.e(TAG, "+++ ON CREATE +++");

		mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);

		// Can Register other sensors in here then read values in
		// onSensorChanged above
		mSensorManager.registerListener(mSensorListener,
				mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
				SensorManager.SENSOR_DELAY_FASTEST);

		// get reference to SensorManager
		sm = (SensorManager) getSystemService(SENSOR_SERVICE);

		// ---change to portrait mode---
		setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);

		StatusBox = (TextView) findViewById(R.id.TextViewStatus);
		RetryConnBox = (CheckBox) findViewById(R.id.CheckBoxRetryConn);
		BumpCheckBox = (CheckBox) findViewById(R.id.CheckBoxBump);

		// don't do anything bluetooth if debugging the user interface
		if (BTconnect) {

			mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();

			if (mBluetoothAdapter == null) {
				Toast.makeText(this, "Bluetooth is not available.",
						Toast.LENGTH_LONG).show();
				finish();
				return;
			}

			if (!mBluetoothAdapter.isEnabled()) {

				// technically should ask user if its ok to enable bluetooth but
				// since i'm the user I say its ok
				mBluetoothAdapter.enable();
				//
				// Toast.makeText(this,
				// "Please enable your BT and re-run this program.",
				// Toast.LENGTH_LONG).show();
				// finish();
				// return;
			}

			if (D)
				Log.e(TAG, "+++ DONE IN ON CREATE, GOT LOCAL BT ADAPTER +++");

		}

		OpenCloseButton = (Button) findViewById(R.id.ButtonOpenClose);
		OpenCloseButton.setText("OPEN");

		// green shade for button to start 0xFF00FF00
		OpenCloseButton.getBackground().setColorFilter(0xFF00FF00,
				PorterDuff.Mode.MULTIPLY);

		OpenCloseButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				CharSequence tempC;

				disableAccel = true;
				// Get instance of Vibrator from current Context
				Vibrator vib = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
				vib.vibrate(50); // Vibrate for 50 milliseconds

				tempC = OpenCloseButton.getText();

				if (tempC.toString() == "OPEN") {
					OpenCloseButton.setText("CLOSE");
					OpenCloseButton.getBackground().setColorFilter(0xFFFF0000,
							PorterDuff.Mode.MULTIPLY);

					// OpenCloseButton.setBackgroundColor(Color.RED);
				} else {
					OpenCloseButton.setText("OPEN");
					OpenCloseButton.getBackground().setColorFilter(0xFF00FF00,
							PorterDuff.Mode.MULTIPLY);
					// OpenCloseButton.setBackgroundColor(Color.GREEN);
				}

				// don't do anything bluetooth if debugging the user interface
				if (BTconnect) {
					// flip LED/door on and off
					flipSwitch();
				}

			}
		}); // end listener

		RetryConnBox.setChecked(persistantConn); // persistent connect on
													// startup.

		RetryConnBox.setOnCheckedChangeListener(new OnCheckedChangeListener() {
			public void onCheckedChanged(CompoundButton buttonView,
					boolean isChecked) {
				if (isChecked)
					persistantConn = true;
				else {
					persistantConn = false;

					if (!myBT.BTconnStatus)
						StatusBox.setText("NOT Connected");
				}
			}
		});

		BumpCheckBox.setChecked(BumpEnable); // set state of button on startup
												// from above

		BumpCheckBox.setOnCheckedChangeListener(new OnCheckedChangeListener() {
			public void onCheckedChanged(CompoundButton buttonView,
					boolean isChecked) {
				if (isChecked)
					BumpEnable = true;
				else {
					BumpEnable = false;
				}
			}
		});

		// pitter for retrying connection every 2 seconds if enabled
		initBTpitter();

	}// end on create

	// read component lifecycles for indepth flow chart of these calls

	// this runs after onCreate but if leave and come back this doesn't run
	@Override
	public void onStart() {
		super.onStart();
		// super.onRestoreInstanceState(outState);

		if (D)
			Log.e(TAG, "++ ON START ++");
	}

	// @Override
	// public void onRestoreInstanceState(Bundle savedInstanceState) {
	// super.onRestoreInstanceState(savedInstanceState);
	// // Restore UI state from the savedInstanceState.
	// // This bundle has also been passed to onCreate.
	// BumpCheckBox.setChecked(savedInstanceState.getBoolean("BumpEnable"));
	// RetryConnBox.setChecked(savedInstanceState.getBoolean("RetryConnEnable"));
	//
	// if (D)
	// Log.e(TAG, "++ ON RESTORE STATE ++");
	// }

	// if you leave the activity then come back this is run
	@Override
	public void onResume() {
		super.onResume();

		// register sensor
		mSensorManager.registerListener(mSensorListener,
				mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
				SensorManager.SENSOR_DELAY_FASTEST);

		// don't do anything bluetooth if debugging the user interface
		// if(BTconnect)
		// initBT();
	}

	// if leave activity this is run
	@Override
	public void onPause() {
		super.onPause();

		// don't do anything bluetooth if debugging the user interface
		// if(BTconnect)
		// myBT.cancel();

		// killBTpitter();
	}

	// @Override
	// public void onSaveInstanceState(Bundle savedInstanceState) {
	// // Save UI state changes to the savedInstanceState.
	// // This bundle will be passed to onCreate if the process is
	// // killed and restarted.
	//
	// if (D)
	// Log.e(TAG, "++ ON SAVE STATE ++");
	//
	// savedInstanceState.putBoolean("BumpEnable", BumpCheckBox.isChecked());
	// savedInstanceState.putBoolean("RetryConnEnable",
	// RetryConnBox.isChecked());
	// super.onSaveInstanceState(savedInstanceState);
	//
	// }

	// when you hit home the program stops
	@Override
	public void onStop() {
		super.onStop();
		// super.onSaveInstanceState(outState);

		if (D)
			Log.e(TAG, "-- ON STOP --");

		// don't do anything bluetooth if debugging the user interface
		if (BTconnect)
			myBT.cancel();

		// kill bt pitter and bump timer
		killHandlers();

		// unregister the sensor listener
		mSensorManager.unregisterListener(mSensorListener);

		// shutoff bluetooth
		if (turnOffBluetoothShutdown) {
			mBluetoothAdapter.disable();
		}

	}

	// when hit the back arrow it destroys everything so can start the program
	// new
	@Override
	public void onDestroy() {
		super.onDestroy();

		if (D)
			Log.e(TAG, "--- ON DESTROY ---");
	}

	public void initBTpitter() {
		// mStartTime = System.currentTimeMillis();
		mHandler.removeCallbacks(mUpdateTimeTask);
		mHandler.postDelayed(mUpdateTimeTask, 1000);

	}

	public void killHandlers() {
		mHandler.removeCallbacks(mUpdateTimeTask);
		bumpHandler.removeCallbacks(bumpTimeTask);
	}

	// *******************************************************************************
	// ******* MAIN LOOP ******************************
	// *******************************************************************************
	private Runnable mUpdateTimeTask = new Runnable() {
		public void run() {

			final long start = SystemClock.uptimeMillis();

			Log.e(TAG, "pitter...");

			disableAccel = false; // this is set when button is pressed. cleared
									// at later time so pressing button doesn't
									// trip accel via vib

			// **annoying because startup makes you feel like the program hangs.
			// Always want to push button on connect the first time.
			// **so disabled it by init BTstartupConn to false
			// try to connect once on startup
			// then only keep trying if the box is checked
			if (BTconnect) {

				if (BTstartupConn) {
					StatusBox.setText("Connecting....");
					initBT();
					BTstartupConn = false;

					if (!myBT.BTconnStatus)
						StatusBox.setText("NOT Connected");
				}

				// sets flag once its waited after accel bump event
				// if(!accelFlipSwitch && !accelWait)
				// accelWait = true;

				// if checkbox is set then keep retrying connection. Check for
				// null because first try there is NO myBT cause init not called
				// yet.
				if (persistantConn == true && myBT == null
						|| !myBT.BTconnStatus) {
					StatusBox.setText("Connecting....");
					initBT();

					// flip switch once on connection for auto retry
					if (myBT.BTconnStatus) {
						flipSwitch();
						persistantConn = false; // reset flag
						RetryConnBox.setChecked(false); // reset once connected
					}
				}
				// else if(accelFlipSwitch==true && myBT.BTconnStatus == true){
				// //flip switch if accel bump even while in pocket but flip the
				// switch immediately on the bump
				// //if delay resetting flag until now then switch won't flip
				// numerous times
				// accelFlipSwitch = false;
				// flipSwitch();
				// mHandler.removeCallbacks(mUpdateTimeTask);
				// }
			}

			// run this loop again in xx milliseconds
			mHandler.postAtTime(this, start + 2300);
		}
	};

	// *******************************************************************************
	// ******* END MAIN LOOP ******************************
	// *******************************************************************************

	// once a bump is detected if another bump must be detected before this
	// routing fires and resets the bump count
	private Runnable bumpTimeTask = new Runnable() {
		public void run() {

//			Log.e(TAG, "++bump count reset++");
			bumpCount = 0;
		}
	};

	// init routine for bluetooth with commands to setup IO port correctly
	public void initBT() {

		if (D) {
			Log.e(TAG, "+ ON RESUME +");
			Log.e(TAG, "+ ABOUT TO ATTEMPT CLIENT CONNECT +");
		}

		// When this returns, it will 'know' about the server,
		// via it's MAC address.
		BluetoothDevice device = mBluetoothAdapter.getRemoteDevice(address);

		// returns firefly e350
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
			if (D)
				Log.e(TAG, "ON RESUME: Socket creation failed.", e);

		}

		// Discovery may be going on, e.g., if you're running a
		// 'scan for devices' search from your handset's Bluetooth
		// settings, so we call cancelDiscovery(). It doesn't hurt
		// to call it, but it might hurt not to... discovery is a
		// heavyweight process; you don't want it in progress when
		// a connection attempt is made.
		mBluetoothAdapter.cancelDiscovery();

		myBT = new ConnectedThread(btSocket);

		// don't write to the streams unless they are created
		if (myBT.BTconnStatus) {
			// ST,255 enables remote configuration forever...need this if
			// resetting
			// PIO4 is held high on powerup then toggled 3 times to reset

			// GPIO Commands to BT device page 15 of commands datasheet
			byte[] cmdMode = { '$', '$', '$' };
			myBT.write(cmdMode);
			myBT.run();

			// S@,8080 temp sets GPIO-7 to an output
			byte[] cmd1 = { 'S', '@', ',', '8', '0', '8', '0', 13 };

			// S%,8080 perm sets GPIO-7 to an output on powerup. only done once
			// byte [] cmd1 = {'S','%',',','8','0','8','0',13};

			myBT.write(cmd1);
			myBT.run();

			// S&,8000 drives GPIO-7 low
			// byte [] cmd2 = {'S','^',',','8','0','0','0',13};

			// make it so cmd mode won't timeout even after factory reset
			byte[] cmd3 = { 'S', 'T', ',', '2', '5', '5', 13 };
			myBT.write(cmd3);
			myBT.run();

			StatusBox.setText("CONNECTED");
		}
	}// end initBT

	public void flipSwitch() {

		if (D)
			Log.e(TAG, "***Switch FLIPPED!");

		// don't write to the streams unless they are created
		if (myBT.BTconnStatus) {
			// S&,8080 drives GPIO-7 high
			byte[] cmd2 = { 'S', '&', ',', '8', '0', '8', '0', 13 };
			myBT.write(cmd2);
			myBT.run();

			sleep(SWITCH_DURATION);

			// S&,8000 drives GPIO-7 low
			byte[] cmd3 = { 'S', '&', ',', '8', '0', '0', '0', 13 };
			myBT.write(cmd3);
			myBT.run();
		}
	}

	// stall system for spec'd amount of time
	public void sleep(long millis) {

		long timeOld = System.currentTimeMillis();
		long timeNew = 0;
		long temp = timeNew - timeOld;

		while (temp < millis) {
			timeNew = System.currentTimeMillis();
			temp = timeNew - timeOld;
		}

	}

}
