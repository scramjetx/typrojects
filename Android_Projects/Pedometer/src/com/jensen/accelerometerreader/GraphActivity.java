package com.jensen.accelerometerreader;

import java.lang.reflect.Array;
import java.text.DecimalFormat;
import java.util.Arrays;

import android.R.string;
import android.app.Activity;
import android.os.Bundle;


public class GraphActivity extends Activity {

	DecimalFormat decFormat = new DecimalFormat("#0.0");   //format floats for printing to phone screen
	
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
				
		//grab the bundle that was passed
		Bundle b = this.getIntent().getExtras();
		
		float [] tempArray = new float [] {};
		tempArray = b.getFloatArray("axis");		//grab the array stored under key "x"
						
		float [][] values = new float[3][tempArray.length];  //adjust the size to #series x #values per series
		
		float [] tempValues = new float[tempArray.length];   //used for scaling the axis
		
		values[0] = tempArray;
		tempArray = b.getFloatArray("thresh");
		values[1] = tempArray;
		tempArray = b.getFloatArray("step");
		values[2] = tempArray;
		
		//scale y axis
		System.arraycopy(values[0],0,tempValues,0,tempArray.length);
		Arrays.sort(tempValues);
		String maxRange = Float.toString(tempValues[tempArray.length-1]);
		String midRange = Float.toString(tempValues[tempArray.length/2]);
		String minRange = Float.toString(tempValues[0]);
		
		
		
		//set labels
		String[] verlabels = new String[] { maxRange, midRange, minRange };
		String[] horlabels = new String[20];
		
		//fill x axis
		for(int i = 0; i < 20; i++){
			horlabels[i] = Integer.toString(tempArray.length/20 * i);
		}
		
		//adjust threshold to spike at midrange of data
		for(int i = 0; i< values[2].length; i++){
			values[2][i] = values[2][i] + Float.parseFloat(midRange);   
		}
		
		GraphView graphView = new GraphView(GraphActivity.this, values, "GraphViewDemo",horlabels, verlabels, GraphView.LINE);
		
		setContentView(graphView);
		
	}
	
}
