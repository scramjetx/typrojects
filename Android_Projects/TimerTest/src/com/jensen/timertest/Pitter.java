package com.jensen.timertest;

import android.os.Handler;
import android.os.SystemClock;
import android.util.Log;

public class Pitter {
	
	private Handler mHandler = new Handler();
	private long mStartTime = 0;
	
	public void initPitter(){
    	mStartTime = System.currentTimeMillis();
        mHandler.removeCallbacks(mUpdateTimeTask);
        mHandler.postDelayed(mUpdateTimeTask, 1000);
    	
    }
	
	 public void killPitter(){
		    
    	mHandler.removeCallbacks(mUpdateTimeTask);
    }
	    
    private Runnable mUpdateTimeTask = new Runnable() {
    	   public void run() {
	    		  
    		   final long start = SystemClock.uptimeMillis();
    		   
    		   Log.e("PITTER CLASS", "pitter...");
	    	     
    	       mHandler.postAtTime(this, start + 1000);
    	   }
    };

}
