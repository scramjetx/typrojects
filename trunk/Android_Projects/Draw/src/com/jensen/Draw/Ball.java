package com.jensen.Draw;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.view.View;

public class Ball extends View {
	private float x;			//x pos of ball
	private float y;			//y pos of ball
	private int r;
	private Paint mPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
	private int direction = 0;		//direction ball is moving
	private float speed = 0;		//speed of the ball
	private int bearing = 0;		//bearing to target
	
	public Ball(Context context, float x, float y, int r, int color) {
		super(context);
		mPaint.setColor(color);
		this.x = x;
		this.y = y;
		this.r = r;
	}
	
	public void setCoordinate(float mx, float my){
		this.x = mx;
		this.y = my;
	}
	
	//function moves in direction desired scaled by speed
	public void move(double [] dir){
		this.x += (dir[0] * this.speed);
		this.y += (dir[1] * this.speed);
		
	}
	
	
	public float getX(){
		return this.x;
	}
	
	public float getY(){
		return this.y;
	}
	
	public void setSpeed(float s){
		this.speed = s;
	}
	
	public float getSpeed(){
		return this.speed;
	}
	
	
	@Override
	protected void onDraw(Canvas canvas) {
		super.onDraw(canvas);
		canvas.drawCircle(x, y, r, mPaint);
		
	}
	
	
}