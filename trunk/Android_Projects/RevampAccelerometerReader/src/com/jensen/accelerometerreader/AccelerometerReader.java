package com.jensen.accelerometerreader;

import java.io.BufferedWriter;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException; 
import java.text.DecimalFormat;
import java.util.*;
import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.os.Environment;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.TextView;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;


public class AccelerometerReader extends Activity {
	public static final int UPDATE_INTERVAL = 40;  	//in millisec
												   	//appears it can handle 30ms losing 4 in 100, with +/-2ms variance	
	public static final float MIN_PTOP = 1f;	 	//set min peak to peak for allowing active axis. If don't staionary phone counts steps
	public static final float SLOW_STEP = 2f; 		//slowest time between steps allowed
	public static final float FAST_STEP = 0.2f;		//fastest time between steps allowed
	
	final String tag = "";
    SensorManager sm = null;
    TextView xViewA = null;
    TextView yViewA = null;
    TextView zViewA = null;
    TextView xViewO = null;
    TextView yViewO = null;
    TextView zViewO = null;
    TextView xThresh = null;
    TextView yThresh = null;
    TextView zThresh = null;
    
    CheckBox repeatCheckBox = null;
    
    Button startLogButton = null;
    Button stopLogButton = null;
    Button resetButton = null;
    
    WriteFile data = new WriteFile();
    boolean log = false;
    
    Timer pitter = new Timer();
    
    float xSample = 0f;	//store samples on sensor change then used when pitter fires
    float ySample = 0f;
    float zSample = 0f;
    
    Sample sx = new Sample(0.3f, 10f);  //initialize with filtering coeff, and rejection tolerance
    Sample sy = new Sample(0.3f, 10f);  //initialize with filtering coeff, and rejection tolerance
    Sample sz = new Sample(0.3f, 10f);  //initialize with filtering coeff, and rejection tolerance
    
    DecimalFormat value = new DecimalFormat("#0.000");   //format floats for printing to phone screen
    
    
    private SensorManager mSensorManager;
    private final SensorEventListener mSensorListener = new SensorEventListener() {

    	public void onSensorChanged(SensorEvent se) {
        	synchronized (this) {
            //    Log.d(tag, "onSensorChanged: " + sensor + ", x: " + 
            //    		values[0] + ", y: " + values[1] + ", z: " + values[2]);
                //Log.d(tag, ", x: " + values[0] + ", y: " + values[1] + ", z: " + values[2]);
              //  if (sensor == SensorManager.SENSOR_ORIENTATION) {
              //      xViewO.setText("Orientation X: " + values[0]);
              //      yViewO.setText("Orientation Y: " + values[1]);
              //      zViewO.setText("Orientation Z: " + values[2]);
              //  }
                if (se.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
                    xViewA.setText("Accel X: " + se.values[0]);
                    yViewA.setText("Accel Y: " + se.values[1]);
                    zViewA.setText("Accel Z: " + se.values[2]);
                    
                    //stores the value constantly then available for pitter
                    xSample = se.values[0];
                    ySample = se.values[1];
                    zSample = se.values[2];
                   
                    xThresh.setText("X Max: " + value.format(sx.max) + ", X Min: " + value.format(sx.min) + ", MID: " + value.format(sx.threshold) + ", S#: " + sx.step);
                    yThresh.setText("Y Max: " + value.format(sy.max) + ", Y Min: " + value.format(sy.min) + ", MID: " + value.format(sy.threshold) + ", S#: " + sy.step);
                    zThresh.setText("Z Max: " + value.format(sz.max) + ", Z Min: " + value.format(sz.min) + ", MID: " + value.format(sz.threshold) + ", S#: " + sz.step);	
                   
                    
                } //end if            
            }//end synchronize
        }//end on sensor changed
        
    	public void onAccuracyChanged(Sensor sensor, int accuracy) {
    		//Log.d(tag,"onAccuracyChanged: " + sensor + ", accuracy: " + accuracy);
        }

		        
    };
    
    
    @Override
    protected void onResume() {
      super.onResume();
      mSensorManager.registerListener(mSensorListener, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_FASTEST);
    }

    
    @Override
    protected void onStop() {
      mSensorManager.unregisterListener(mSensorListener);
      super.onStop();
    }
    
    
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
       
        mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        
        //Can Register other sensors in here then read values in onSensorChanged above
        mSensorManager.registerListener(mSensorListener, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_FASTEST);
        
        // get reference to SensorManager
        sm = (SensorManager) getSystemService(SENSOR_SERVICE);
        setContentView(R.layout.main);
        xViewA = (TextView) findViewById(R.id.xbox);
        yViewA = (TextView) findViewById(R.id.ybox);
        zViewA = (TextView) findViewById(R.id.zbox);
        xViewO = (TextView) findViewById(R.id.xboxo);
        yViewO = (TextView) findViewById(R.id.yboxo);
        zViewO = (TextView) findViewById(R.id.zboxo);
        
        xThresh = (TextView) findViewById(R.id.xthresholdbox);
        yThresh = (TextView) findViewById(R.id.ythresholdbox);
        zThresh = (TextView) findViewById(R.id.zthresholdbox);
        
        
        startLogButton = (Button) findViewById(R.id.StartLogging);
        stopLogButton = (Button) findViewById(R.id.StopLogging);
        resetButton = (Button) findViewById(R.id.Reset);
        
        stopLogButton.setEnabled(false);  //disable stop log by default
        
        startLogButton.setOnClickListener(new OnClickListener() {
            public void onClick(View v) {

            	startLogButton.setEnabled(false);
            	stopLogButton.setEnabled(true);
            	log = true; //start logging
            	
            	//only for the case where you reset while the program is still running and start logging again
            	if (data.isOpen() == false){
            		data.openFile();
            	}
            
            	
            	
            }
          });  //end listener
        
        
        stopLogButton.setOnClickListener(new OnClickListener() {
            public void onClick(View v) {
            	try {
            		log = false;  //stop logging
					data.closeFile();
					startLogButton.setEnabled(true);
	            	stopLogButton.setEnabled(false);
				} catch (IOException e) {
					
					e.printStackTrace();
				}
			}
          });  //end listener
        
        
        resetButton.setOnClickListener(new OnClickListener() {
        	public void onClick(View v) {
				sx = new Sample(0.3f, 10f);  //initialize with filtering coeff, and rejection tolerance
			    sy = new Sample(0.3f, 10f);  //initialize with filtering coeff, and rejection tolerance
			    sz = new Sample(0.3f, 10f);  //initialize with filtering coeff, and rejection tolerance
			    
			    try {
            		log = false;  //stop logging
					data.closeFile();
					startLogButton.setEnabled(true);
	            	stopLogButton.setEnabled(false);
				} catch (IOException e) {
					
					e.printStackTrace();
				}
			}
		});
        
        //open the file
        data.openFile();
        
        //set up ?? Hz sampling of the sensors
        //do all processing within pitterProcess Function
        pitter.scheduleAtFixedRate(
        	      new TimerTask() {
        	        public void run() {
        	          _pitterProcess();
        	        }
        	      },
        	      1,
        	      UPDATE_INTERVAL);
       
        
        
        
        
       
        
        
    } //end on create
    
       
    public void _pitterProcess(){
    	
    	//Store current axis value which increments sample count
    	sx.StoreSample(xSample);
    	sy.StoreSample(ySample);
    	sz.StoreSample(zSample);
    	
    	//Filter all Axis
    	//move filtered value to dynamic threshold and precision stage array
    	sx.Filter();
    	sy.Filter();
    	sz.Filter();
    	    	
    	//Reject any outliers by keeping the previous sample in favor of the new sample
    	sx.Reject();
    	sy.Reject();
    	sz.Reject();
    	
    	//max and min updated continuously but stored in maxTemp and minTemp until 50th sample
    	sx.FindMax();
    	sx.FindMin();
    	sy.FindMax();
    	sy.FindMin();
    	sz.FindMax();
    	sz.FindMin();
    	
    	//threshold every UPDATE_INTERVAL samples
    	if(sx.sampleCount >= UPDATE_INTERVAL || sy.sampleCount >= UPDATE_INTERVAL || sz.sampleCount >= UPDATE_INTERVAL){
	    	
    		//reset step counters
    		sx.SampleCountReset();
    		sy.SampleCountReset();
    		sz.SampleCountReset();
    		
    		sx.FindThreshold();
	    	sy.FindThreshold();
	    	sz.FindThreshold();
	    	
	    	//find most active axis to count steps on
	    	sx.FindPtoP();
	    	sy.FindPtoP();
	    	sz.FindPtoP();
    	}
    	
    	//only one axis can be active at once
    	//min Peak to Peak must be enforced or stationary phone will count steps
    	if(sx.PtoP > MIN_PTOP || sy.PtoP > MIN_PTOP || sz.PtoP > MIN_PTOP){
    		if(sx.PtoP >= sy.PtoP && sx.PtoP >= sz.PtoP){
	    		sx.stepAxis = true;
	    		sy.stepAxis = false;
	    		sz.stepAxis = false;
	    	}else if(sy.PtoP >= sx.PtoP && sy.PtoP >= sz.PtoP){
	    		sx.stepAxis = false;
	    		sy.stepAxis = true;
	    		sz.stepAxis = false;
	    	}else if(sz.PtoP >= sx.PtoP && sz.PtoP >= sy.PtoP){
	    		sx.stepAxis = false;
	    		sy.stepAxis = false;
	    		sz.stepAxis = true;
	    	}
    	}else{
    		sx.stepAxis = false;
    		sy.stepAxis = false;
    		sz.stepAxis = false;
    	}
    	
    	//count the steps on the active axis when the slope is negative and crossing the threshold
    	if(sx.stepAxis==true){
    		sx.countSteps();
    	}else if(sy.stepAxis==true){
    		sy.countSteps();
        }else if(sz.stepAxis==true){
        	sz.countSteps();
        }
    	
    	
    	if(log==true){
      		printLog();  //print to the log
      	}
    	
      
      
    }// end _pitterProcess
    
    
    
    //format of the data log file
    public void printLog(){
    	try {
			data.writeToFile(Float.toString(sx.axisArray[2]));
			data.writeToFile(",");
			data.writeToFile(Float.toString(sx.max));
			data.writeToFile(",");
			data.writeToFile(Float.toString(sx.min));
			data.writeToFile(",");
			data.writeToFile(Float.toString(sx.threshold));
			//data.writeToFile(",");
			//data.writeToFile(Float.toString(sx.interval));
			//data.writeToFile(",");
			//data.writeToFile(Long.toString(SystemClock.uptimeMillis()));
			data.writeToFile("\n");
			
			
			
		} catch (IOException e) {
			
			e.printStackTrace();
		}
    }
    

    
  //****************************************************** 
  //****Class for operating on Accelerometer Samples******

    public class Sample{
    	private float [] sampleArray = new float[3];  //order: x raw data, x prev filt value, x filtered value
    	private float [] axisArray = new float[3];    //order: x, x_sample, x_new, x_old 
    	
    	private float a;  						//holds value < 1 filter coefficient. smaller < filtering and > response
    	private float p;						//max change allowed before rejection
    	private float max = 0f;  				//store max threshold of axis
    	private float min = 0f;					//store min threshold of axis
    	private float maxTemp, minTemp = 0f;	//store temp min max in between 50 samples so it only updates AT fiftieth sample
    	private float PtoP = 0f;				//the swing between max and min in order to find most active axis to count steps on
    	private float threshold = 0f;			//stores midpoint of min and max for calculating when a step is taken
    	//private float thresholdTemp = 0f;		//stores threshold inbetween 50 samples until threshold is updated on 50th sample
    	private int step = 0;					//stores amount of steps on this axis
    	private boolean stepAxis = false;		//track if best axis to count steps on, ie has greatest swing in values
    	private boolean stepFlag = false;		//only allow one step before count until threshold is crossed above again
    	private int sampleCount = 0;			//track how many samples up to fifty
    	private int interval = 0;				//track how long between steps. Can be as fast as .2 sec or slow as 2 seconds.
    	
    	//constructor
    	public Sample(float filtCoeff, float precision){
    		a = filtCoeff;
    		p = precision;
    	}
    	
    	
    	//put initial sample into class per axis for processing
    	public void StoreSample(float value){
    		sampleArray[0] = value;
    		sampleCount++;		
    	}
    	
    	//function for applying first order filter to incoming samples
		//      do
		//      read new data x[0]
		//      f[2] = f[1] + a*(f[0]-f[1])
		//      f[1] = f[2]
		//      loop
		 //
		//  	 	f[0] is the raw data
		//      	f[1] is the previous filtered value
		//      	f[2] is the final filtered value
		//      	a is the filter factor (alpha)
    	public void Filter(){
    	    
    		//Apply first order filter to 3 samples. 
    		sampleArray[2] = sampleArray[1] + a*(sampleArray[0] - sampleArray[1]);
    	    sampleArray[1] = sampleArray[2];
    	    
    	    //move final filtered value to dynamic threshold and precision stage array
    	    axisArray[0] = sampleArray[2];
    	    
    	    
    	}
    	
    	//function for rejecting outlier and if not moving into sample new position
    	public void Reject(){
			   
			axisArray[2] = axisArray[1]; //sample new shifted to old unconditionally
	    	
			//!!!!possible problem for first 3 samples because f[1] is zero and new sample can be as large as 9 easily, need to account for that with sample counter? 
	    	if((Math.max(axisArray[0], axisArray[1]) - Math.min(axisArray[0],axisArray[1])) < p){
	    		axisArray[1] = axisArray[0]; 	//if the difference between new and sample is < precision then move it into sample new
	    					 					//otherwise the sample stays the same
	    	}
		}
    	
    	
    	//keep running track of max and min of each axis
    	public void FindMax(){
    		
        	if(axisArray[2] > maxTemp){
        		maxTemp = axisArray[2];
        	}
        	
        	if(sampleCount >= UPDATE_INTERVAL){
        		max = maxTemp;
        		maxTemp = -99999;
        	}
        	
        	
    	}
    	
    	public void FindMin(){
    		
        	if(axisArray[2] < minTemp){
        		minTemp = axisArray[2];
        	}
        	
        	if(sampleCount >= UPDATE_INTERVAL){
        		min = minTemp;
        		minTemp = 99999;
        	}
    	}
    	
    	public void FindThreshold(){
    		threshold = (max + min) / 2;
    	}
    	
    	public void FindPtoP(){
    		PtoP = max - min;
    	}
    	
    	public void countSteps(){
    		interval++;			//increment interval at each step for comparison to make sure step isn't too fast or too slow
    		
    		if(axisArray[2] > threshold){
    			stepFlag = true;
    		}
    		
    		//if incoming sample is less than past sample and incoming is below threshold it crossed threshold
    		//and the steps have moved above the threshold
    		//and the interval between steps is between .2s and 2s @ 50hz 10 = .2s and 100 = 2s
    		if((axisArray[1] < axisArray[2]) && (axisArray[1] < threshold) && (stepFlag == true)){
				if(interval > (UPDATE_INTERVAL * FAST_STEP) && interval < (UPDATE_INTERVAL * SLOW_STEP) ){
					interval = 0;  	  	//reset interval to time next step
        			step++;
            		stepFlag = false; 	//reset flag till above threshold again	
				}else{
	    			interval = 0;		//if it was out of the interval reject the step and reset to try again on the next step
	    		}
			}
    		
    			
    	}
    	
    	public void SampleCountReset(){
    		sampleCount = 0;
    	}
    	
    }//end FilterSample class
    
    
   //********************************************* 
   //****File Writing Class for Data Logging****** 
    public class WriteFile {
    	private boolean isOpen = false;
    	private boolean append = false;
    	private File root;
    	private File Logfile;
    	private FileWriter filewriter;
    	private BufferedWriter out;
    	//private FileOutputStream fos;
    	//private DataOutputStream dos;
    	
    	//constructor
    	public WriteFile(){
    	
    	}
    	
    	//constructor
    	public WriteFile(boolean append_value){
    		append = append_value;
    	}
    	
    	public boolean isOpen() {
    		//Log.d(tag, "IsOpen Function:" + isOpen);
    		return isOpen;
		}
    	
    	//open the file
    	public void openFile(){
    		Log.d(tag, "file Opened");
    		try {
    			root = Environment.getExternalStorageDirectory();
                if (root.canWrite()){
                	Logfile = new File(root, "LogFile.txt");
                	filewriter = new FileWriter(Logfile);
                	out = new BufferedWriter(filewriter);
                	//fos = new FileOutputStream(Logfile);
                	//dos = new DataOutputStream(fos);
                	
                	
                	isOpen = true;
                }
			} catch (IOException e) {
				e.printStackTrace();
			}
			
    		
    	}
    	//file writer
    	public void writeToFile( String s ) throws IOException {
    		out.write(s);
    		//out.write('\n');
    		//out.write(',');
    		
    		
    		
    	}
    	
    	public void closeFile() throws IOException{
    		isOpen=false;
    		out.close();	
    		
    		Log.d(tag, "File State:" + isOpen);
    		Log.d(tag, "Closed File");
    	}
		
		
    	
    	
    } //end WriteFile class
    
    
    
    
}//end accelerometer reader