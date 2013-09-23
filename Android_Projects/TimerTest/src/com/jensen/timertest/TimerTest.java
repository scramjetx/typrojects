package com.jensen.timertest;

import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.util.Log;


public class TimerTest extends Activity {
   
	String TAG = "TEST";
	private Handler mHandler = new Handler();
//	long mStartTime = 0;
	
	
	
	
	
	
	/** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        initPitter();
        
    }
    
    @Override
    public void onPause() {
            super.onPause();
          
          //kill timer handler
          mHandler.removeCallbacks(mUpdateTimeTask);
            
    }

    @Override
    public void onResume() {
            super.onResume();

            initPitter();
    }
    
    @Override
    public void onStop() {
            super.onStop();
            	Log.e(TAG, "-- ON STOP --");
            	 //kill timer handler
                mHandler.removeCallbacks(mUpdateTimeTask);
    }

    @Override
    public void onDestroy() {
            super.onDestroy();
            	Log.e(TAG, "--- ON DESTROY ---");
    }
    public void initPitter(){
//    	mStartTime = System.currentTimeMillis();
        mHandler.removeCallbacks(mUpdateTimeTask);
        mHandler.postDelayed(mUpdateTimeTask, 1000);
    	
    }
    
    public void killPitter(){
    	mHandler.removeCallbacks(mUpdateTimeTask);
    }
    
    private Runnable mUpdateTimeTask = new Runnable() {
    	   public void run() {
    		  
    		   final long start = SystemClock.uptimeMillis();
    		   
    		   Log.e(TAG, "pitter...");

    	     
    	       mHandler.postAtTime(this, start + 1000);
    	   }
    	};
}