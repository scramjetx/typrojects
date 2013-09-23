package com.jensen.fasttrax;

public class Car {
	
	//GPIO 7 - powerup command used to init as output and drive high
    private static final byte [] cmdLFgo =   {'S','*',',','0','2','0','0',13};
    private static final byte [] cmdLFstop = {'S','*',',','0','2','0','2',13};
	 
	 //GPIO 9 - no powerup command available but used anyway.  Starts out high.
    private static final byte [] cmdLRgo =   {'S','*',',','0','4','0','0',13};
    private static final byte [] cmdLRstop = {'S','*',',','0','4','0','4',13};
	
	//GPIO 10 - needs init command to drive high. starts as input.
    private static final byte [] cmdRFgo =   {'S','*',',','0','8','0','0',13};
    private static final byte [] cmdRFstop = {'S','*',',','0','8','0','8',13};
	 
	//GPIO 11 - needs init command to drive high. starts as input.
    private static final byte [] cmdRRgo =   {'S','&',',','8','0','0','0',13};
    private static final byte [] cmdRRstop = {'S','&',',','8','0','8','0',13};
	
    public byte [] cmdSentL = null;
  	public byte [] cmdSentStopL = null;  	//used to make sure if LF commanded that LR isn't also commanded. Would ruin car.
  	
  	public byte [] cmdSentR = null;
  	public byte [] cmdSentStopR = null;  	//used to make sure if LF commanded that LR isn't also commanded. Would ruin car.
    
  	boolean hasCmdL = false;
    boolean hasCmdR = false;
    
    
    
    
    public void DriveDir(String button, boolean state){
   	 //state reflects whether button is on or off which a high value sent turns off the motor
    	cmdSentL = null;
      	cmdSentStopL = null;  
      	cmdSentR = null;
      	cmdSentStopR = null;  	
   	 
      	
      	
   	 //state machine for all possible button presses
   	 //sets the value for cmdSent
   	 if(button == "LF"){
   		 if(state == true){
   			 cmdSentL = cmdLFgo;
   			 cmdSentStopL = cmdLRstop;
   			hasCmdL = true;
   		 }
   		 else{
   			 cmdSentL = cmdLFstop;
   		 }
   	 }else if(button == "LS"){
   		 if(state == true){
   			 cmdSentL = cmdLFstop;
   			 cmdSentStopL = cmdLRstop;
   			hasCmdL = true;
   		 }
   	 }else if(button == "LR"){
   		 if(state == true){
   			 cmdSentL = cmdLRgo;
   			 cmdSentStopL = cmdLFstop;
   			hasCmdL = true;
   		 }
   		 else{
   			 cmdSentL = cmdLRstop;
   		 }
   	 }else if(button == "RF"){
   		 if(state == true){
   			 cmdSentR = cmdRFgo;
   			 cmdSentStopR = cmdRRstop;
   			hasCmdR = true;
   		 }
   		 else{
   			 cmdSentR = cmdRFstop;
   		 }
   	 }else if(button == "RS"){
   		 if(state == true){
   			 cmdSentR = cmdRFstop;
   			 cmdSentStopR = cmdRRstop;
   			hasCmdR = true;
   		 }
   	 }else if(button == "RR"){
   		 if(state == true){
   			 cmdSentR = cmdRRgo;
   			 cmdSentStopR = cmdRFstop;
   			hasCmdR = true;
   		 }
   		 else{
   			 cmdSentR = cmdRRstop;
   		 }
   	 }
   	
   	
   	 //state machine to make sure forward and reverse not commanded on same motor at same time
   	 //motor damage can result
   	 //sets the value for cmdSentStop

   	 
   	
   	 
   	 
    }

}
