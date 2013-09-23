

/** Header Files ***************************************************/     
#include <p18f2580.h>
#include <timers.h>
#include <pwm.h>
#include <pconfig.h>
#include <delays.h>
#include <usart.h>
#include <spi.h>
#include <stdio.h> 
#include <adc.h>


/** Configuration Bits *********************************************/     
#pragma config OSC = HSPLL  //  EC= External 8MHz Oscillator w/ osc/4 on RA6
#pragma config WDT = OFF
#pragma config LVP = OFF
//#pragma config BOREN = OFF
//#pragma config MCLRE = ON



/** Define Constants Here ******************************************/
#define FOSC 32000000 						//Pin 7 is Fosc/4 
#define delayTime 200
#define BAUDRATE 115200
#define BRGVAL (((FOSC/BAUDRATE)/16) - 1) 	// this is the seed value I am using to open the PIC rs232 port 

//Servo Positions  #2- 186, 210, 244, 161, 133
//				   #3- 180, 206, 240, 158, 130	
#define CENTER 180 				//servo center position. 26 - 66 range
#define STATE1 206				//deploy gripper 1
#define STATE2 240				//release gripper 1
#define STATE3 158				//deploy gripper 2
#define STATE4 130				//release gripper 2

#define PULSE_WIDTH_MIN		56			
#define PULSE_WIDTH_MAX		255	

/** Local Function Prototypes **************************************/
void high_isr(void);  	
void low_isr(void);   	
void Timer1(void);
void printToUART(int data);
void ECANparse(void);			//ECAN message parse routine
void ADCsample(void);			//ADC conversion of Vsense and Isense
void centerServo(void);			//center servo using Isense to see when servo is loaded
void setGripperState(void);		//set the gripper state based on CAN message/GCS camera toggle

/** Global Variables ***********************************************/
int x,y, temp;
int result;
volatile char count = 0;				//postscale timer to get from 100hz to 50hz
volatile char ready = 0;
volatile char pulse = 0;
char loadFlag = 0; 			//Tracks whether servo is under load for resetting to center position
char pulseDouble = 0;		//double the pulsewidth of the timer through software manipulation
int pulseWidth = CENTER;    //37 = 1.5ms
int pulseWidthPrev = CENTER;//
int Isense = 0;				//board current consumption
int Vsense = 0;				//board battery supply voltage
int IsenseMax = 0;			//track max current draw for calibration
int servoTimer = 0;			//tracks time servo has been commanded to non zero position so it can be returned to zero, 50 ticks = 1 sec
char gripperState = 0;		//track gripper position state 1-4
char dataPrev = 0;			//track transitions between cameraFront and rear to increment gripper state
char camSwitch = 0;			//keeps looking for can messages until the cam switch comes across
char TMR2state = 0;			//track timer2 state for pwm modulation
char initServo = 1;			//initialize servo after PWM has started for more stable servo startup
int startup = 0;			//track gripper state startup to allow any camera to be selected on startup


/** Declare Interrupt Vector Sections ****************************/
#pragma code high_vector=0x08
void interrupt_at_high_vector(void)
{
   _asm goto high_isr _endasm
}
#pragma code

#pragma code low_vector=0x18
void interrupt_at_low_vector(void)
{
   _asm goto low_isr _endasm
}
#pragma code

unsigned long id;
BYTE data[4];
BYTE dataLen;
ECAN_RX_MSG_FLAGS flags;
	
/*******************************************************************
* Function:        void main(void)
********************************************************************/
#pragma code
void main (void)
{

//***ADC init
//use SetChanADC(ADC_CH0, ADC_CH9) to switch between desired channels
TRISAbits.TRISA0 = 1;	//currentSense pin input
TRISAbits.TRISA7 = 1;	//Vsense pin input	
ADCON1 = 0b00000101;  	//AVDD/AVSS source, AN0-9 analog inputs
ADCON0 = 0b00000000;	//ANO, enable/disable ADC
ADCON2 = 0b10000000;	//Data right justified, TAD acquisition time, AD conversion clock select
PIR1bits.ADIF = 0;
PIE1bits.ADIE = 0;
IPR1bits.ADIP = 0;
ADRESH = 0x00;                      // Clear AD register
ADRESL = 0x00; 

//***UART init	
OpenUSART( USART_TX_INT_OFF &
  USART_RX_INT_ON &
  USART_ASYNCH_MODE &
  USART_EIGHT_BIT &
  USART_CONT_RX &
  USART_BRGH_HIGH,
  BRGVAL );
	
IPR1bits.RCIP = 0;			//UART RX interrupt low priority 
IPR1bits.TXIP = 0;			//UART TX interrupt low priority

//***PORTC Init
TRISCbits.TRISC2 = 0;
LATCbits.LATC2 = 0;
TRISCbits.TRISC1 = 0;  			//set C1 as output for servo on/off
PORTCbits.RC1 = 1;				//set servo to initially OFF, servo is active low
TRISCbits.TRISC3 = 0;			//set as output
TRISCbits.TRISC5 = 0;			//set as output


//***Interrupt Init
RCONbits.IPEN = 1;  		// Enable interrupt priority levels
INTCONbits.GIE = 1; 		// Enable all high priority interrupts 
INTCONbits.PEIE = 1; 		//enable peripheral interrupt sources
INTCON2bits.TMR0IP = 0;  	//set Timer 0 low priority
IPR1bits.TMR2IP = 1;		//set Timer 2 high priority
//IPR1bits.TMR1IP = 0;		//set Timer 1 low priority

//***Timer Setups  //can't use Timer1 because disables RC0 & RC1
OpenTimer0( TIMER_INT_ON &
T0_16BIT &
T0_SOURCE_INT &
T0_PS_1_256 );

OpenTimer2( TIMER_INT_ON &
T2_PS_1_16 &
T2_POST_1_4);  

//T2CONbits.TMR2ON = 0;  //disable timer2


//ECAN Setup
// ECAN.def file must be set up correctly.
TRISBbits.TRISB3 = 1; 		//set RX as input
ECANInitialize();

PIR3bits.RXB0IF = 0;		//clear interrupt Flag
PIE3bits.RXB0IE = 1;		//enable CAN RX interrupts for RXB1 RX register

IPR3bits.RXB0IP = 0;		//set low priority for CAN interrupts

//BRGCON1 = 0b00000000;
//BRGCON2 = 0b10111010;
//BRGCON3 = 0b00000011;


while (1)
{


    //sets up the timing for the 50hz cycle by toggling ready flag, any leftover time is spent in here and looking for CAN messages	
	while(!ready) if(ECANReceiveMessage(&id, data, &dataLen, &flags));		//ECANSendMessage(0x123, data, 0, ECAN_TX_STD_FRAME);
	ready = 0;


//Enable Timer2 PWM 	
	T2CONbits.TMR2ON = 1;  //enable timer2 for PWM interrupt driven, state machine in low_isr

//ADC conversion of Isense and Vsense
	ADCsample();		//Convert analog voltages of Isense and Vsense
	//printToUART(Isense);
	//WriteUSART('\t');
	//printToUART(IsenseMax);
	//WriteUSART('\t');
	//printToUART(pulseWidth);


//Move servo back to center by sensing current drawn by servo
	//centerServo();


	//Increment gripper position based on recieving GCS camera toggle message on CAN bus
	 if(camSwitch)
	{
		servoTimer = 0;	
		if(startup>10) setGripperState();
	}

	//This returns the servo to Center position after each stage after ~2sec
	if(servoTimer>100)	pulseWidth=CENTER;

	//turn off servo inbetween each servo state to conserve power
	if(pulseWidth == CENTER && servoTimer>150) PORTCbits.RC1 = 1;	//servo OFF

	//This allows several PWM pulses to be generated to allow for stable servo powerup
	if(servoTimer> 10 && initServo == 1) 
	{
		PORTCbits.RC1 = 0;
		initServo = 0;
	}

if(startup<100) startup++; 			//don't look for valid CAN camera toggle messages until after 100 cycles...2 sec after startup

}//end while
CloseTimer0();

}//end main



/*******************************************************************
* Additional Helper Functions
********************************************************************/


void ADCsample()
{

		ADCON0bits.ADON = 1; 		//enable ADC
		SetChanADC(ADC_CH0);
		ConvertADC();
		while( BusyADC() ); 		// Wait for completion
		Isense = ReadADC(); 		// Read result
			
		SetChanADC(ADC_CH9);
		ConvertADC();
		while( BusyADC() ); 		// Wait for completion
		Vsense = ReadADC(); 		// Read result
					
		ADCON0bits.ADON = 0; 		//disable ADC
			
		if(Isense > IsenseMax) IsenseMax = Isense; 


}//end ADCsample

/*****************************************************************
* Function:        void high_isr(void)
* PreCondition:    None
* Input:
* Output:
* Side Effects:
* Overview:
******************************************************************/
#pragma interrupt high_isr			// declare function as high priority isr
void high_isr(void)
{

	//Timer2 interrupt Poll
	if(PIR1bits.TMR2IF && PIE1bits.TMR2IE)
	{
		switch (TMR2state)                  	//select TMR2state machine for PWM 
	    {
	    case 0: PR2 = 1;						//stall from tmr2 init to make more accurate calc
	        break;
	    case 1: PR2 = pulseWidth;				//set pulseWidth desired by user
			PORTCbits.RC2 = !PORTCbits.RC2;
	        break;
	    case 2: T2CONbits.TMR2ON = 0;
	        PORTCbits.RC2 = !PORTCbits.RC2;
			TMR2state = 0;
			T2CONbits.TMR2ON = 0;  				//disable timer2 till next 50hz cycle		
			break;
	    }
		TMR2state++; 							//increment pwm state machine

		PIR1bits.TMR2IF = 0;					//clear Timer 2 interrupt
	

	}

}//end high_isr

/******************************************************************
* Function:        void low_isr(void)
* PreCondition:    None
* Input:
* Output:
* Side Effects:
* Overview:
********************************************************************/
#pragma interruptlow low_isr		// declare function as low priority isr
void low_isr(void)
{

//**Timer0 interrupt Poll
	if(INTCONbits.TMR0IF && INTCONbits.TMR0IE)
	{
		servoTimer++;						//running at 50hz so 50 ticks = 1 sec
		PORTCbits.RC5 = !PORTCbits.RC5;		//64916 @ 128 post scale
		WriteTimer0(65225);   				// 65225 @ 256 post scale Sets Timer for 50Hz Interrupt @ 128 prescaler
		if(PORTCbits.RC5) ready = 1;		
		
		INTCONbits.TMR0IF = 0;				//clear Timer 0 interrupt
	}// END TMR0 interrupt Poll



	
//***UART RX interrupt Poll
	if ((PIR1bits.RCIF) && (PIE1bits.RCIE)) // act only on eusart rx interrupts
	{
		PIR1bits.RCIF = 0;
		
		if(RCREG == 'f') pulseWidth++;
		if(RCREG == 'b') pulseWidth--;
		if(RCREG == 's') PORTCbits.RC1 = !PORTCbits.RC1;  	//toggle servo on off,
		if(RCREG == '0') pulseWidth=CENTER;  
		if(RCREG == '1') pulseWidth=STATE1;	
		if(RCREG == '2') pulseWidth=STATE2;
		if(RCREG == '3') pulseWidth=STATE3;
		if(RCREG == '4') pulseWidth=STATE4;
		//keep pwm in bounds 1ms to 2ms
		if(pulseWidth < PULSE_WIDTH_MIN) pulseWidth = PULSE_WIDTH_MIN;
		if(pulseWidth > PULSE_WIDTH_MAX) pulseWidth = PULSE_WIDTH_MAX;
printToUART(pulseWidth);

	}// end if bits



//***CAN interrupt Poll
	if(PIR3bits.RXB0IF && PIE3bits.RXB0IE)
	{
		if(id==0x10 && data[0]==0x06 && data[1]==0x01)
		{
			//don't look for valid CAN camera toggle messages until after 100 cycles...2 sec after startup
			if(initServo || startup<100) dataPrev = data[3];  	//allows GCS to be in any camera position on startup, 
																//then toggle camera switches gripper state
	
			if(data[3] != dataPrev)
			{	
				camSwitch = 1; 
				gripperState++;							//detect edge transition and increment state
			}
			if(gripperState==5) gripperState=0;  		//reset back to zero
			dataPrev = data[3];							//store prev camera state for edge detection		
		}// end if id

		PIR3bits.RXB0IF=0;	
	} //end CAN interrupt

}//end low_isr


/*******************************************************************
* Function:			void printToUart(int data)
* Input Variables:	data to be printed
* Output Return:	none
* Overview:			Simple way to print to UART
********************************************************************/

//TX interrupts are off for this function
void printToUART(int data)
{
	int x;
	char dataArray[10];

	sprintf(dataArray,"%d", data);
	
	for(x = 0; x<strlen(dataArray); x++)
	{
		while (BusyUSART());  		//If the uart is busy wait
		WriteUSART(dataArray[x]);   //transmit a char
	}

	while (BusyUSART());
	WriteUSART('\n');
	WriteUSART('\r');

}//end printToUART


/*******************************************************************
* Function:			void centerServo(void)
* Input Variables:	Move servo back to center by sensing current drawn by servo
* Output Return:	none
* Overview:			Simple way to center servo by sensing current drawn by payload
********************************************************************/
void centerServo()
{
	
	if(pulseWidth!=CENTER && Isense>310) loadFlag = 1;
	else if(loadFlag && Isense < 100)
	{
		loadFlag = 0;
		pulseWidth = CENTER; 
	}
}//end centerServo

/*******************************************************************
* Function:			void setGripperState(void)
* Input Variables:	Move servo to hard coded gripper position based on GCS toggle of front/side camera
* Output Return:	none
* Overview:			helper function for tracking servo state
********************************************************************/
void setGripperState()
{
	PORTCbits.RC1 = 0;	//servo ON
	//Assign gripperState based on Camera switch message
	if(gripperState==0) pulseWidth=CENTER;
	if(gripperState==1) pulseWidth=STATE1;
	if(gripperState==2) pulseWidth=STATE2;
	if(gripperState==3) pulseWidth=STATE3;
	if(gripperState==4) pulseWidth=STATE4;
	
	camSwitch = 0;
}//end setGripperState()
