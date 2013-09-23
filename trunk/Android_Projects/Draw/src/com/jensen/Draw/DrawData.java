package com.jensen.Draw;

import java.util.ArrayList;
import java.util.Properties;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.view.View;

public class DrawData extends View {
	private float xLabelPos;
	private float yLabelPos;
	private float x;
	private float y;
	
	private Paint mPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
	
	private Properties dataTable = new Properties();
	
	public DrawData(Context context, float x, float y, float size, int color) {
		super(context);
		mPaint.setColor(color);
		mPaint.setTextSize(size);
		this.xLabelPos = x;
		this.yLabelPos = y;
		
	}
	
	public void setLabelPos(float mx, float my){
		this.xLabelPos = mx;
		this.yLabelPos = my;
		
	}
	
	public void setPos(float mx, float my){
		this.x = mx;
		this.y = my;
	}
	
	public float getX(){
		return this.x;
	}
	
	public float getY(){
		return this.y;
	}
	
	public void setDataValue(String key, float val){
		//store value to plot in table to key/value combo
		dataTable.setProperty(key, Float.toString(val));
	
	}
	
	@Override
	protected void onDraw(Canvas canvas) {
		super.onDraw(canvas);
		
		String xTemp = dataTable.getProperty("missileXpos");
		String yTemp = dataTable.getProperty("missileYpos");
		String bearingTemp = dataTable.getProperty("targetBearing");
		
	 	canvas.drawText("Missile Pos: [" + xTemp + ", " + yTemp + "]", xLabelPos, yLabelPos, mPaint);
		canvas.drawText("Bearing to Target: " + bearingTemp , xLabelPos, (yLabelPos + 20), mPaint);
	}
}
