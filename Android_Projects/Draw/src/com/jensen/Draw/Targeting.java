package com.jensen.Draw;

import java.lang.reflect.Array;

import android.graphics.Point;
import android.util.Log;

public class Targeting {

	private Ball missile;
	private Ball target;
	private double bearing;
	private double [] slopeToTarget = {0,0};
	private double [] slopeToTargetPrev = {0,0};
	private float accuracy = -0.02f;					//decide what accuracy value really means
	
	String TAG = "**BtoT CLASS:  ";
	//constructor
	public Targeting(){
		
	}
	
	public void engageTarget(Ball m, Ball t){
		
		this.missile = m;
		this.target = t;
		
		this.calcSlopeToTarget();
		
		this.accuracyControl();
		
		
	}
	
	public void accuracyControl(){
		
		slopeToTarget[0] = ((slopeToTargetPrev[0] - slopeToTarget[0]) * accuracy) + slopeToTargetPrev[0];
		slopeToTarget[1] = ((slopeToTargetPrev[1] - slopeToTarget[1]) * accuracy) + slopeToTargetPrev[1];
	}
	
	public void calcSlopeToTarget(){
//		missile = m;
//		target = t;
		double x1 = (double)missile.getX();
		double y1 = (double)missile.getY();
		double x2 = (double)target.getX();
		double y2 = (double)target.getY();
		
		double dx = x2 - x1;
		double dy = y2 - y1;
		double xMag = 0;
		double yMag = 0;
		
		double distance = Math.sqrt(Math.pow(dx, 2) + Math.pow(dy, 2));
		
		xMag = dx/distance;
		yMag = dy/distance;

		System.arraycopy(slopeToTarget, 0, slopeToTargetPrev, 0, 2);
		
		
		slopeToTarget[0] = xMag;
		slopeToTarget[1] = yMag;
		
		Log.d(TAG,"X MAG: " + xMag + " :: Y MAG: " + yMag + " :: Distance: " + distance);
		
		
	}

	public double [] getSlopeToTarget(){
		return slopeToTarget;
	}
	
	public double [] getSlopeToTargetPrev(){
		return slopeToTargetPrev;
	}
	
	public void calcBearing(Ball m, Ball t){
		missile = m;
		target = t;
		
		double x1 = (double)missile.getX();
		double y1 = (double)missile.getY();
		double x2 = (double)target.getX();
		double y2 = (double)target.getY();
		
		double dx = x2 - x1;
		double dy = y2 - y1;
		
		
		
		bearing = Math.atan2(dx, dy);
		
		bearing = bearing * 57.29578;
		
		
		//maybe don't want to do this translation because atan2 gives 0-180 or negative
		//so you'd be able to tell shortest direction to turn to target by that... 
		//maybe more useful to tell what direction to move based on that number
		
		
		//translate bearing to correspond to NSEW on a compass
//		if(dx > 0){
//			bearing = 180 + bearing;
//		
//		}
//		if(dx < 0){
//			bearing = bearing + 180;
//		}
//		bearing = 360 - bearing;
		
//		Log.d(TAG,"DeltaX: " + dx + " :: DeltaY: " + dy + " :: Bearing: " + Math.round(bearing));
		
		
//		     N
//		    000
//     W           E
//    270         090
//		     S
//          180	
		
		
	}
	
	public double getBearing(){
		return bearing;
	}
	
	public int gcf(int u, int v){
     int shift;
 
     /* GCD(0,x) := x */
     if (u == 0 || v == 0)
       return u | v;
 
     /* Let shift := lg K, where K is the greatest power of 2
        dividing both u and v. */
     for (shift = 0; ((u | v) & 1) == 0; ++shift) {
         u >>= 1;
         v >>= 1;
     }
 
     while ((u & 1) == 0)
       u >>= 1;
 
     /* From here on, u is always odd. */
     do {
         while ((v & 1) == 0)  /* Loop X */
           v >>= 1;
 
         /* Now u and v are both odd, so diff(u, v) is even.
            Let u = min(u, v), v = diff(u, v)/2. */
         if (u < v) {
             v -= u;
         } else {
             int diff = u - v;
             u = v;
             v = diff;
         }
         v >>= 1;
     } while (v != 0);
 
     return u << shift;
}
}

