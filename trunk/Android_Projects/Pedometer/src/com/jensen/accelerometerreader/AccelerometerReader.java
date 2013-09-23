//NOTES:
//suspect a problem with switching between active axis that a step count is taking place that shouldn't be say if the axis has never been counted on
//before then it might have a different peak to peak or threshold that wouldn't prove to count a step accurately. look at graphs more closely.

//need to add in gui adjustment of min and max step times, and min peak to peak step

//maybe get a file with 50 steps and name it something then iterate through it a bunch of times until find settings
//that give accurate count. with min ptop, pitter and interval changing

//throws exception when you try to log at start of program.  isn't logfile left over from last run?

//some reason sim doesn't play back right. Count isn't the same on each axis. is it counting wrong and sim is right?
//sim is more repeatable on the count values

//adjusted step to midpoint of graph..needs some tweaking. not exactly midpoint

//fix y axis so a few more divisions and formated dec to 2 places

//zooming on graph?? sounds tough. maybe slider bar with end points which would be cool...text box with endpoints

//force close if hit graph and nothing has been stored

//maybe use the 4 step in a row method before counting. then go out of synch if you lose that to make step counting more reliable
//then when it does sync add those 4 steps into the count
//prob don't want the cont steps inside the class because not all gonna sync up on one axis. global? 
//but then how do you tell if step was counted on that axis? Look into more




package com.jensen.accelerometerreader;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.text.DecimalFormat;
import java.util.*;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.os.Environment;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnKeyListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.TextView;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

public class AccelerometerReader extends Activity {

	public static int PITTER_INTERVAL = 50; 	// sampling rate for sensors
	public static int UPDATE_INTERVAL = 35; 	// how many samples between updating
												// the threshold for step counting
	
	// appears it can handle 30ms losing 4 in 100, with +/-2ms variance
	public static float MIN_PTOP = 1.2f; 			// set min peak to peak for allowing active axis. 
												//If don't stationary phone counts steps
	public static float SLOW_STEP = 2f; 		// slowest time between steps allowed
	public static float FAST_STEP = 0.2f; 		// fastest time between steps allowed
	
	public static float THRESH_ABOVE = 0.3f;  	//amount above threshold the prev valid sample must move to allow travel below to be a step
	public static int MIN_SAMPLE_COUNT = UPDATE_INTERVAL*2;	//amount of samples to reject at start so the thresh and min max can settle out
																//try twice the update interval so it can get its thresh calc a couple times
	
	public static int MIN_CONT_STEPS = 4;		//min amount of continuous steps before allowing counting
												//this way develope a rythmn before counting steps. Then add on the cont steps after sync
	
	protected static final int SUB_ACTIVITY_REQUEST_CODE = 1337; // id for calling subactivity

	public static int GRAPH_SIZE = 1001; // amount of samples to plot per axis

	final String tag = "";

	SensorManager sm = null;

	TextView xViewA = null;
	TextView yViewA = null;
	TextView zViewA = null;
	TextView xThresh = null;
	TextView yThresh = null;
	TextView zThresh = null;

	Button startLogButton = null;
	Button stopLogButton = null;
	Button resetButton = null;
	Button simButton = null;

	CheckBox graphCheckBox = null;
	Button graphXButton = null;
	Button graphYButton = null;
	Button graphZButton = null;

	EditText pitterTextBox = null;
	TextView sampleFreqView = null;

	EditText intervalTextBox = null;
	TextView intervalView = null;

	WriteFile data = new WriteFile();
	ReadFile simFile = new ReadFile();

	boolean log = false;

	Timer pitter = new Timer();

	float xSample = 0f; // store samples on sensor change then used when pitter fires
	float ySample = 0f;
	float zSample = 0f;

	ArrayList<Float> xArray = new ArrayList<Float>(); 	// dynamic amount of
														// samples to graph
	ArrayList<Float> yArray = new ArrayList<Float>();
	ArrayList<Float> zArray = new ArrayList<Float>();

	ArrayList<Float> xThreshArray = new ArrayList<Float>(); // dynamic amount of
															// threshold samples
															// to graph
	ArrayList<Float> yThreshArray = new ArrayList<Float>();
	ArrayList<Float> zThreshArray = new ArrayList<Float>();

	ArrayList<Float> xStepCountedArray = new ArrayList<Float>(); 	// dynamic
																	// status of
																	// at what
																	// sample a
																	// step was
																	// counted
	
	ArrayList<Float> yStepCountedArray = new ArrayList<Float>();
	ArrayList<Float> zStepCountedArray = new ArrayList<Float>();

	Sample sx = new Sample(0.3f, 10f); 	// initialize with filtering coeff, and
										// rejection tolerance
	Sample sy = new Sample(0.3f, 10f); 	// initialize with filtering coeff, and
										// rejection tolerance
	Sample sz = new Sample(0.3f, 10f); 	// initialize with filtering coeff, and
										// rejection tolerance

	DecimalFormat value = new DecimalFormat("#0.000"); // format floats for
														// printing to phone
														// screen

	int lifeSampleCount = 0;	//records amount of samples before a reset or program exit so can reject steps under x amount of time.
								//gives time for the min/max and thresh to settle out.  MIN_SAMPLE_COUNT constant is used to provide minimum
	
	
	private SensorManager mSensorManager;
	private final SensorEventListener mSensorListener = new SensorEventListener() {

		public void onSensorChanged(SensorEvent se) {
			synchronized (this) {

				if (se.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
					xViewA.setText("Accel X: " + se.values[0]);
					yViewA.setText("Accel Y: " + se.values[1]);
					zViewA.setText("Accel Z: " + se.values[2]);

					// stores the value constantly then available for pitter
					// only if the sensor doesn't report it was an unreliable
					// reading
					// if sim file is open then store those values and simulate
					// that count
					if (se.accuracy != SensorManager.SENSOR_STATUS_UNRELIABLE
							&& simFile.runSim == false) {
						
						//enable button once sim is finished
						if(simButton.isEnabled()== false){	simButton.setEnabled(true);}
						
						xSample = se.values[0];
						ySample = se.values[1];
						zSample = se.values[2];

					}

					xThresh.setText("X Max: " + value.format(sx.max)
							+ ", X Min: " + value.format(sx.min) + ", MID: "
							+ value.format(sx.threshold) + ", S#: " + sx.step);
					yThresh.setText("Y Max: " + value.format(sy.max)
							+ ", Y Min: " + value.format(sy.min) + ", MID: "
							+ value.format(sy.threshold) + ", S#: " + sy.step);
					zThresh.setText("Z Max: " + value.format(sz.max)
							+ ", Z Min: " + value.format(sz.min) + ", MID: "
							+ value.format(sz.threshold) + ", S#: " + sz.step);

				} // end if
			}// end synchronize
		}// end on sensor changed

		public void onAccuracyChanged(Sensor sensor, int accuracy) {
			// Log.d(tag,"onAccuracyChanged: " + sensor + ", accuracy: " +
			// accuracy);
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
						SensorManager.SENSOR_DELAY_FASTEST);
			}
		}
	};

	// if you leave the activity then come back
	@Override
	protected void onResume() {
		super.onResume();
		mSensorManager.registerListener(mSensorListener, mSensorManager
				.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
				SensorManager.SENSOR_DELAY_FASTEST);
	}

	// when you hit home the program stops listening to sensor data
	@Override
	protected void onStop() {
		mSensorManager.unregisterListener(mSensorListener);
		pitter.cancel(); 	//stop the timer thread
		super.onStop();
	}

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
				SensorManager.SENSOR_DELAY_FASTEST);

		// get reference to SensorManager
		sm = (SensorManager) getSystemService(SENSOR_SERVICE);

		xViewA = (TextView) findViewById(R.id.xbox);
		yViewA = (TextView) findViewById(R.id.ybox);
		zViewA = (TextView) findViewById(R.id.zbox);

		xThresh = (TextView) findViewById(R.id.xthresholdbox);
		yThresh = (TextView) findViewById(R.id.ythresholdbox);
		zThresh = (TextView) findViewById(R.id.zthresholdbox);

		sampleFreqView = (TextView) findViewById(R.id.TextViewSampleFreq);
		pitterTextBox = (EditText) findViewById(R.id.EditTextPitter);
		pitterTextBox.setText(Integer.toString(PITTER_INTERVAL));

		// listen for the enter key to be pressed then update the constant
		// modified
		pitterTextBox.setOnKeyListener(new OnKeyListener() {
			public boolean onKey(View v, int keyCode, KeyEvent event) {
				// If the event is a key-down event on the "enter" button
				if ((event.getAction() == KeyEvent.ACTION_DOWN)
						&& (keyCode == KeyEvent.KEYCODE_ENTER)) {
					// Perform action on key press
					PITTER_INTERVAL = Integer.parseInt(pitterTextBox.getText()
							.toString());
					sampleFreqView.setText("Sample Freq (Hz) = "
							+ PITTER_INTERVAL);
					// Toast.makeText(HelloFormStuff.this, edittext.getText(),
					// Toast.LENGTH_SHORT).show();

					return true;
				}
				return false;
			}
		});

		intervalView = (TextView) findViewById(R.id.TextViewInterval);
		intervalTextBox = (EditText) findViewById(R.id.EditTextInterval);
		intervalTextBox.setText(Integer.toString(UPDATE_INTERVAL));

		// listen for the enter key to be pressed then update the constant
		// modified
		intervalTextBox.setOnKeyListener(new OnKeyListener() {
			public boolean onKey(View v, int keyCode, KeyEvent event) {
				// If the event is a key-down event on the "enter" button
				if ((event.getAction() == KeyEvent.ACTION_DOWN)
						&& (keyCode == KeyEvent.KEYCODE_ENTER)) {
					// Perform action on key press
					UPDATE_INTERVAL = Integer.parseInt(intervalTextBox
							.getText().toString());
					intervalView.setText("Interval (Samples) = "
							+ UPDATE_INTERVAL);
					// Toast.makeText(HelloFormStuff.this, edittext.getText(),
					// Toast.LENGTH_SHORT).show();

					return true;
				}
				return false;
			}
		});

		startLogButton = (Button) findViewById(R.id.StartLogging);
		stopLogButton = (Button) findViewById(R.id.StopLogging);
		resetButton = (Button) findViewById(R.id.Reset);
		simButton = (Button) findViewById(R.id.Sim);

		graphCheckBox = (CheckBox) findViewById(R.id.CheckBoxGraph);
		graphXButton = (Button) findViewById(R.id.GraphX);
		graphYButton = (Button) findViewById(R.id.GraphY);
		graphZButton = (Button) findViewById(R.id.GraphZ);

		stopLogButton.setEnabled(false); // disable stop log by default

		startLogButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {

				startLogButton.setEnabled(false);
				stopLogButton.setEnabled(true);
				log = true; // start logging

				// only for the case where you reset while the program is still
				// running and start logging again
				if (data.isOpen() == false) {
					data.openFile();
				}

			}
		}); // end listener

		stopLogButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				try {
					log = false; // stop logging
					data.closeFile();
					startLogButton.setEnabled(true);
					stopLogButton.setEnabled(false);
				} catch (IOException e) {

					e.printStackTrace();
				}
			}
		}); // end listener

		resetButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				sx = new Sample(0.3f, 10f); // initialize with filtering coeff,
											// and rejection tolerance
				sy = new Sample(0.3f, 10f); // initialize with filtering coeff,
											// and rejection tolerance
				sz = new Sample(0.3f, 10f); // initialize with filtering coeff,
											// and rejection tolerance

				lifeSampleCount = 0;  //reset lifetime sample count
				
				
				xArray.clear(); // reset the array ready to fill again
				yArray.clear();
				zArray.clear();

				xThreshArray.clear();
				yThreshArray.clear();
				zThreshArray.clear();

				xStepCountedArray.clear();
				yStepCountedArray.clear();
				zStepCountedArray.clear();

				try {
					log = false; // stop logging
					data.closeFile();
					startLogButton.setEnabled(true);
					stopLogButton.setEnabled(false);
				} catch (IOException e) {

					e.printStackTrace();
				}
			}
		});

		simButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {

				simButton.setEnabled(false);

				// only for the case where you reset while the program is still
				// running and start logging again
				if (simFile.isOpen() == false) {
					// opens and parses
					try {
						Log.d(tag, "***Sim Button Press");
						simFile.parseFile();
					} catch (Exception e) {
					
						e.printStackTrace();
					}
				}

			}
		}); // end listener

		graphXButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {

				// starting sub activity. Back button returns to previous
				// activity placed on the Activity Stack
				Intent myIntent = new Intent(AccelerometerReader.this,
						GraphActivity.class);

				// create bundle with key / value combination to pass
				Bundle b = new Bundle();

				// do all this array stuff to make it so it only graphs a
				// dynamic amount of values
				float[] axisTemp = new float[xArray.size()];
				float[] threshTemp = new float[xThreshArray.size()];
				float[] stepTemp = new float[xStepCountedArray.size()];

				for (int i = 0; i < xArray.size(); i++) {
					axisTemp[i] = xArray.get(i);
					threshTemp[i] = xThreshArray.get(i);
					stepTemp[i] = xStepCountedArray.get(i);
				}
				;

				b.putFloatArray("axis", axisTemp);
				b.putFloatArray("thresh", threshTemp);
				b.putFloatArray("step", stepTemp);

				myIntent.putExtras(b);
				startActivity(myIntent);

			}
		}); // end listener

		graphYButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {

				// starting sub activity. Back button returns to previous
				// activity placed on the Activity Stack
				Intent myIntent = new Intent(AccelerometerReader.this,
						GraphActivity.class);

				// create bundle with key / value combination to pass
				Bundle b = new Bundle();

				// do all this array stuff to make it so it only graphs a
				// dynamic amount of values
				float[] axisTemp = new float[yArray.size()];
				float[] threshTemp = new float[yThreshArray.size()];
				float[] stepTemp = new float[yStepCountedArray.size()];

				for (int i = 0; i < yArray.size(); i++) {
					axisTemp[i] = yArray.get(i);
					threshTemp[i] = yThreshArray.get(i);
					stepTemp[i] = yStepCountedArray.get(i);
				}
				;

				b.putFloatArray("axis", axisTemp);
				b.putFloatArray("thresh", threshTemp);
				b.putFloatArray("step", stepTemp);

				myIntent.putExtras(b);
				startActivity(myIntent);

			}
		}); // end listener

		graphZButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {

				// starting sub activity. Back button returns to previous
				// activity placed on the Activity Stack
				Intent myIntent = new Intent(AccelerometerReader.this,
						GraphActivity.class);

				// create bundle with key / value combination to pass
				Bundle b = new Bundle();

				// do all this array stuff to make it so it only graphs a
				// dynamic amount of values
				float[] axisTemp = new float[zArray.size()];
				float[] threshTemp = new float[zThreshArray.size()];
				float[] stepTemp = new float[zStepCountedArray.size()];

				for (int i = 0; i < zArray.size(); i++) {
					axisTemp[i] = zArray.get(i);
					threshTemp[i] = zThreshArray.get(i);
					stepTemp[i] = zStepCountedArray.get(i);
				}
				;

				b.putFloatArray("axis", axisTemp);
				b.putFloatArray("thresh", threshTemp);
				b.putFloatArray("step", stepTemp);

				myIntent.putExtras(b);
				startActivity(myIntent);

			}
		}); // end listener

		// open the file
		data.openFile();

		// set up ?? Hz sampling of the sensors
		// do all processing within pitterProcess Function
		pitter.scheduleAtFixedRate(new TimerTask() {
			public void run() {
				_pitterProcess();
			}
		}, 1, PITTER_INTERVAL);

	} // end on create

	
	
	public void _pitterProcess() {
		if (simFile.runSim == true) {
			if (simFile.runSim == true) {
				xSample = simFile.readValue();
			}
			if (simFile.runSim == true) {
				ySample = simFile.readValue();
			}
			if (simFile.runSim == true) {
				zSample = simFile.readValue();
			}
		} 

		
		
		// Store current axis value which increments sample count
		sx.StoreSample(xSample);
		sy.StoreSample(ySample);
		sz.StoreSample(zSample);

		// Filter all Axis
		// move filtered value to dynamic threshold and precision stage array
		sx.Filter();
		sy.Filter();
		sz.Filter();

		// Reject any outliers by keeping the previous sample in favor of the
		// new sample
		sx.Reject();
		sy.Reject();
		sz.Reject();

		// max and min updated continuously but stored in maxTemp and minTemp
		// until ??th sample
		sx.FindMax();
		sx.FindMin();
		sy.FindMax();
		sy.FindMin();
		sz.FindMax();
		sz.FindMin();

		// threshold every UPDATE_INTERVAL samples
		if (sx.sampleCount >= UPDATE_INTERVAL
				|| sy.sampleCount >= UPDATE_INTERVAL
				|| sz.sampleCount >= UPDATE_INTERVAL) {

			// reset step counters
			sx.SampleCountReset();
			sy.SampleCountReset();
			sz.SampleCountReset();

			sx.FindThreshold();
			sy.FindThreshold();
			sz.FindThreshold();

			// find most active axis to count steps on
			sx.FindPtoP();
			sy.FindPtoP();
			sz.FindPtoP();
		}

		// only one axis can be active at once
		// min Peak to Peak must be enforced or stationary phone will count steps
		if (sx.PtoP > MIN_PTOP || sy.PtoP > MIN_PTOP || sz.PtoP > MIN_PTOP) {
			
			lifeSampleCount++; //running total of samples since program start or reset
						
			if (sx.PtoP >= sy.PtoP && sx.PtoP >= sz.PtoP) {
				sx.stepAxis = true;
				sy.stepAxis = false;
				sz.stepAxis = false;
			} else if (sy.PtoP >= sx.PtoP && sy.PtoP >= sz.PtoP) {
				sx.stepAxis = false;
				sy.stepAxis = true;
				sz.stepAxis = false;
			} else if (sz.PtoP >= sx.PtoP && sz.PtoP >= sy.PtoP) {
				sx.stepAxis = false;
				sy.stepAxis = false;
				sz.stepAxis = true;
			}
		} else {
			sx.stepAxis = false;
			sy.stepAxis = false;
			sz.stepAxis = false;
		}

		// count the steps on the active axis when the slope is negative and
		// crossing the threshold
		if (sx.stepAxis == true) {
			sx.stepFlag = false;		//reset the step flag because if become active barely above thresh then will count step as drops across
			sx.countSteps();
		} else if (sy.stepAxis == true) {
			sy.stepFlag = false;
			sy.countSteps();
		} else if (sz.stepAxis == true) {
			sz.stepFlag = false;
			sz.countSteps();
		}

		if (log == true) {
			printLog(); // print to the log
		}

		// save sample to graph later if checkbox is checked
		if (graphCheckBox.isChecked() == true) {
			xArray.add(sx.axisArray[2]);
			yArray.add(sy.axisArray[2]);
			zArray.add(sz.axisArray[2]);

			xThreshArray.add(sx.threshold);
			yThreshArray.add(sy.threshold);
			zThreshArray.add(sz.threshold);

			xStepCountedArray.add((float) sx.stepCountedFlag);
			yStepCountedArray.add((float) sy.stepCountedFlag);
			zStepCountedArray.add((float) sz.stepCountedFlag);

			sx.stepCountedFlag = 0;
			sy.stepCountedFlag = 0;
			sz.stepCountedFlag = 0;
		}

	}// end _pitterProcess

	// format of the data log file
	public void printLog() {
		try {
			data.writeToFile(Float.toString(sx.axisArray[2]));
			data.writeToFile(",");
			// data.writeToFile(Float.toString(sx.threshold));
			// data.writeToFile(",");
			// data.writeToFile(Float.toString(sx.stepCountedFlag));
			// data.writeToFile(",");
			data.writeToFile(Float.toString(sy.axisArray[2]));
			data.writeToFile(",");
			// data.writeToFile(Float.toString(sy.threshold));
			// data.writeToFile(",");
			// data.writeToFile(Float.toString(sy.stepCountedFlag));
			// data.writeToFile(",");
			data.writeToFile(Float.toString(sz.axisArray[2]));
			data.writeToFile(",");
			// data.writeToFile(Float.toString(sz.threshold));
			// data.writeToFile(",");
			// data.writeToFile(Float.toString(sz.stepCountedFlag));

			// data.writeToFile(",");
			// data.writeToFile(Float.toString(sx.interval));
			// data.writeToFile(",");
			// data.writeToFile(Long.toString(SystemClock.uptimeMillis()));

			// data.writeToFile("\n");

		} catch (IOException e) {

			e.printStackTrace();
		}
	}

	// ******************************************************
	// ****Class for operating on Accelerometer Samples******

	public class Sample {
		private float[] sampleArray = new float[3]; // order: x raw data, x prev filt value, x filtered value
		private float[] axisArray = new float[3]; 	// order: x, x_sample, x_new, x_old

		private float a; 	// holds value < 1 filter coefficient. smaller < filtering and > response
		private float p; 	// max change allowed before rejection
		private float max = 0f; // store max threshold of axis
		private float min = 0f; // store min threshold of axis
		private float maxTemp, minTemp = 0f; 	// store temp min max in between 50
												// samples so it only updates at fiftieth sample
		private float PtoP = 0f; 	// the swing between max and min in order to
									// find most active axis to count steps on
		private float threshold = 0f; 	// stores midpoint of min and max for
										// calculating when a step is taken
		
		private int step = 0; // stores amount of steps on this axis
		private boolean stepAxis = false; // track if best axis to count steps
											// on, ie has greatest swing in values
		private boolean stepFlag = false; // only allow one step before count
											// until threshold is crossed above again
		private int sampleCount = 0; // track how many samples up to fifty
		private int interval = 0; // track how long between steps. Can be as
									// fast as .2 sec or slow as 2 seconds.
		private int stepCountedFlag = 0; // track when the axis counted a step for graphing
		
		private int contStepsCount = 0;	//amount of steps in a row counted. must be greater than MIN_CONT_STEPS
		

		// constructor
		public Sample(float filtCoeff, float precision) {
			a = filtCoeff;
			p = precision;
		}

		// put initial sample into class per axis for processing
		public void StoreSample(float value) {
			sampleArray[0] = value;
			sampleCount++;
		}

		//******************************************************************
		// function for applying first order filter to incoming samples
		// do
		// read new data x[0]
		// f[2] = f[1] + a*(f[0]-f[1])
		// f[1] = f[2]
		// loop
		//
		// f[0] is the raw data
		// f[1] is the previous filtered value
		// f[2] is the final filtered value
		// a is the filter factor (alpha)
		//******************************************************************
		public void Filter() {

			// Apply first order filter to 3 samples.
			sampleArray[2] = sampleArray[1] + a
					* (sampleArray[0] - sampleArray[1]);
			sampleArray[1] = sampleArray[2];

			// move final filtered value to dynamic threshold and precision
			// stage array
			axisArray[0] = sampleArray[2];

		}

		// function for rejecting outlier and if not moving into sample new
		// position
		public void Reject() {

			axisArray[2] = axisArray[1]; // sample new shifted to old
											// unconditionally

			// !!!!possible problem for first 3 samples because f[1] is zero and
			// new sample can be as large as 9 easily, need to account for that
			// with sample counter?
			if ((Math.max(axisArray[0], axisArray[1]) - Math.min(axisArray[0],
					axisArray[1])) < p) {
				axisArray[1] = axisArray[0]; // if the difference between new
												// and sample is < precision
												// then move it into sample new
				// otherwise the sample stays the same
			}
		}

		// keep running track of max and min of each axis
		public void FindMax() {

			if (axisArray[2] > maxTemp) {
				maxTemp = axisArray[2];
			}

			if (sampleCount >= UPDATE_INTERVAL) {
				max = maxTemp;
				maxTemp = -99999;
			}

		}

		public void FindMin() {

			if (axisArray[2] < minTemp) {
				minTemp = axisArray[2];
			}

			if (sampleCount >= UPDATE_INTERVAL) {
				min = minTemp;
				minTemp = 99999;
			}
		}

		public void FindThreshold() {
			threshold = (max + min) / 2;
		}

		public void FindPtoP() {
			PtoP = max - min;
		}

		public void countSteps() {
			interval++; // increment interval at each step for comparison to
						// make sure step isn't too fast or too slow

			//axisArray[2] is the prev valid sample so if it's above the threshold then we can count valid steps
			//implement an amount above the threshold it has to move just to tweak false steps?
			if (axisArray[2] > threshold + THRESH_ABOVE) {
				stepFlag = true;
			}

			// if incoming sample is less than past sample and incoming is below
			// threshold it crossed threshold
			// and the steps have moved above the threshold
			// and the interval between steps is between .2s and 2s @ 50hz 10 = .2s and 100 = 2s
			// and min amount of samples have been processed to allow the thresh to settle out
			//and make sure the min peak to peak is enforced not only for active axis but step counting
			if ((axisArray[1] < axisArray[2]) && (axisArray[1] < threshold)	&& (stepFlag == true) 
					&& lifeSampleCount > MIN_SAMPLE_COUNT && PtoP > MIN_PTOP) {
				if (interval > (PITTER_INTERVAL * FAST_STEP) && interval < (PITTER_INTERVAL * SLOW_STEP)) {
					interval = 0; // reset interval to time next step
					stepFlag = false; // reset flag till above threshold again
					stepCountedFlag = 2; // Set flag for graphing when each step
											// was counted on the axis
					
					//count continuous steps and only update step count when above MIN_CONT_STEPS
					//if(contStepsCount < MIN_CONT_STEPS){
					//	contStepsCount++;
					//}
					step++;
				} else {
					interval = 0; // if it was out of the interval reject the
									// step and reset to try again on the next
									// step
				}
			}

		}// end countSteps

		public void SampleCountReset() {
			sampleCount = 0;
		}

	}// end FilterSample class

	// *********************************************
	// ****File Writing Class for Data Logging******
	public class WriteFile {
		private boolean isOpen = false;
		private File root;
		private File Logfile;
		private FileWriter filewriter;
		private BufferedWriter out;

		// constructor
		public WriteFile() {

		}


		public boolean isOpen() {
			// Log.d(tag, "IsOpen Function:" + isOpen);
			return isOpen;
		}

		// open the file
		public void openFile() {
			Log.d(tag, "file Opened");
			try {
				root = Environment.getExternalStorageDirectory();
				if (root.canWrite()) {
					Logfile = new File(root, "logfile.txt");
					filewriter = new FileWriter(Logfile);
					out = new BufferedWriter(filewriter);

					isOpen = true;
				}
			} catch (IOException e) {
				e.printStackTrace();
			}

		}

		// file writer
		public void writeToFile(String s) throws IOException {
			out.write(s);
			// out.write('\n');
			// out.write(',');

		}

		public void closeFile() throws IOException {
			isOpen = false;
			out.close();

			Log.d(tag, "File State:" + isOpen);
			Log.d(tag, "Closed File");
		}

	} // end WriteFile class

	// *********************************************
	// ****File Reading Class for Simulating walking******
	public class ReadFile {
		private boolean isOpen = false;
		private File rootDir;
		private File simFile;
		private FileReader filereader;
		private BufferedReader br;
		private String strLine = "start";
		private int lineNumber = 0;
		private int tokenNumber = 0;
		ArrayList<String> storeValues = new ArrayList<String>();
		private int arrayIndex = 0; // used for index of read back of the array
		boolean runSim = false;

		// constructor
		public ReadFile() {

		}

		public boolean isOpen() {
			// Log.d(tag, "IsOpen Function:" + isOpen);
			return isOpen;
		}

		// file writer
		public void parseFile() throws IOException {

			try {

				rootDir = Environment.getExternalStorageDirectory();

				if (rootDir.canWrite()) {
					simFile = new File(rootDir, "logfile.txt"); // read from same file logged to.
																// that way can tweak values until correct count
				}

				isOpen = true;
				filereader = new FileReader(simFile);
				br = new BufferedReader(filereader);
				lineNumber = 0;
				tokenNumber = 0;

				Log.d(tag, "**OPEN PARSE FILE**");

				while ((strLine = br.readLine()) != null) {
					lineNumber++;
					String[] result = strLine.split(",");

					// while(st.hasMoreTokens()==true)
					for (int x = 0; x < result.length; x++) {
						storeValues.add(result[x]);
						tokenNumber++;
						Log.d(tag, "parsed Value: " + result[x]);
					}// end for

				}// end while readline

				// reset token number
				tokenNumber = 0;

				// capture size of array for readback
				arrayIndex = storeValues.size();

				// done with file
				Log.d(tag, "**CLOSED PARSE FILE**");
				br.close();
				isOpen = false;
				runSim = true;

			} catch (IOException e) {
				e.printStackTrace();
			}

		}

		public float readValue() {
			String temp = "0.0";

			if (storeValues.isEmpty() != true) {
				Log.d(tag, "read Value :" + Integer.toString(arrayIndex));
				temp = storeValues.remove(0); // removes first item and shifts
												// everything to the left
				arrayIndex--;
			} else {
				Log.d(tag, "**DONE READING**");
				runSim = false;

			}

			return Float.valueOf(temp);
		}

		public void closeFile() throws IOException {
			isOpen = false;
			br.close();

			Log.d(tag, "File State:" + isOpen);
			Log.d(tag, "Closed File");
		}

	} // end WriteFile class

}// end accelerometer reader