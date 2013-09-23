//NOTES
//should be using handlers instead of a timer apparently because timers spin off a new thread with alot of overhead
//also there is easier access to the view hierarchy.  not sure what that means other than don't call post invalidate. just invalidate 
//		see link http://developer.android.com/resources/articles/timed-ui-updates.html



//timer task won't start again on resume...or on start when you hit home then bring the app back to action.

//got it working but not quite right
//it needs to move the distance of speed each time
//so need to figure the direction it was moving by difference in current slope and prev slope then add speed to it accordingly
//right now the multiplication in the move function of ball makes it scale the speed so when its far off it slows down and turns
//then speeds up again. not quite correct behavior. or it will go directly backward


package com.jensen.Draw;

import java.util.Timer;
import java.util.TimerTask;

import android.app.Activity;
import android.graphics.Color;
import android.graphics.Point;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.FrameLayout;

public class Draw extends Activity {
	public static int PITTER_INTERVAL = 60; 	// update rate for drawing = 1/PITTER_INTERVAL (ms) = ??Hz
	Timer pitter = new Timer();
	
	Ball missileBall = null;
	Ball targetBall = null;
	
	DrawData dataLabel = null;
	
	Targeting trackTarget = new Targeting();
	
	final Activity myActivity = this;
	
	final String TAG = "";
	
	
	
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);
		
		FrameLayout main = (FrameLayout) findViewById(R.id.main_view);
		main.setBackgroundColor(Color.BLUE);

		missileBall = new Ball(myActivity,50,50,15, Color.WHITE);
		main.addView(missileBall);  //adds a new ball to the view
		
		targetBall = new Ball(myActivity, 250, 350, 25, Color.RED);
		main.addView(targetBall);  //adds a new ball to the view
		
		dataLabel = new DrawData(myActivity, 140, 700, 20, Color.WHITE);
		main.addView(dataLabel);
		
		
		
		main.setOnTouchListener(new View.OnTouchListener() {
		public boolean onTouch(View v, MotionEvent e) {
		
//			Log.d(TAG,"onAccuracyChanged: ");
			
				float x = e.getX();
				float y = e.getY();
//				FrameLayout flView = (FrameLayout) v;
//				flView.addView(missileBall);
				missileBall.setCoordinate(x,y);
				missileBall.invalidate();
				
				dataLabel.setPos(x,y);
				dataLabel.invalidate();
				
//				flView.invalidate();
				
				return false;
			}
		
			
		});
		
		
		
		// set up 60 Hz refresh rate of display
		// do all processing within pitterProcess Function
		pitter.scheduleAtFixedRate(new TimerTask() {
			public void run() {	_pitterProcess();
			}
		}, 1, PITTER_INTERVAL);
		
	}//end on create
	
//	protected void onStart(){
//		super.onStart();
//		pitter.scheduleAtFixedRate(new TimerTask() {
//			public void run() {	_pitterProcess();
//			}
//		}, 1, PITTER_INTERVAL);
//	}
	
	protected void onStop(){
		super.onStop();
		pitter.cancel();
	}
	
	public void _pitterProcess() {
		double bearingTemp = 0d;
		double [] moveTemp = {0,0};
		
		missileBall.setCoordinate(missileBall.getX(), missileBall.getY());
//		dataLabel.setPos(missileBall.getX(), missileBall.getY());
		
		
		trackTarget.engageTarget(missileBall, targetBall);
		moveTemp = trackTarget.getSlopeToTarget();
		
		missileBall.setSpeed(3);
		missileBall.move(moveTemp);
		
		
		//what data to log
		dataLabel.setDataValue("missileXpos", Math.round(missileBall.getX()));
		dataLabel.setDataValue("missileYpos", Math.round(missileBall.getY()));
		dataLabel.setDataValue("targetBearing", Math.round(bearingTemp));
		
		
		//refresh the screen after everything modified in the view
		missileBall.postInvalidate();  //calling one view update also updates the labels
	
	}
	
}//end class draw