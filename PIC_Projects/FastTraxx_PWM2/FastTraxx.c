/*******************************************************************
* FileName:        FastTraxx.c
* Processor:       PIC18F26K22
* Compiler:        MPLAB C18  
*
* This file does the following....
*	Takes a serial stream from bluetooth adapter and parses commands to drive IO pins high or low to control 2 motors and 4 directions.
*	
*
* Creation and Revisions:
*      Author               Date			Comments
*     Tyson Jensen		1/21/11				1st hack at it		   
********************************************************************/

//todo
//appears to be overflow error if transmit too many consecutive chars. such as 3 messages at once. may need to check and clear overflow flag
//cause pitter still works on overflow but won't recieve anymore messages
//figure out if can just latch ccp1,2,3,4 pins over to other IOs via the interrupt or something to prevent resoldering the IOs on the pcb
//maybe combine both parse routines so # is just on/off motor control and $ is proportional control or something.  Then in phone app can select which
//tweak values for slow to make sure off is off and slow is slow, do this through hyperterm with motor connected


//done
//commands with motor direction and speed set the right motor with PWM
//can accept and parse a message with duty cycle
//can send single cmds from hyperterm if the enable bit is set to do so
//50hz pitter parses packets in circular buffer
//all 4 channels pwm'ing with function calls all on Timer2
//will parse all messages in buffer, may run into trouble if alot in the faster time frames when pwming the outputs

/** Header Files ***************************************************/
#include<p18f26k22.h>

#include <timers.h>
#include <pconfig.h>
#include <delays.h>
#include <usart.h>
#include <stdio.h> 
#include <string.h>
#include <pwm.h>


/** Configuration Bits *********************************************/     
#pragma config PRICLKEN = ON
#pragma config PLLCFG = OFF		//Use oscillator directly. No PLL
#pragma config FOSC = INTIO7 	//Internal oscillator block, CLKOUT function on RA6, port function on RA7 
#pragma config WDTEN = OFF		//disable watchdog timer
#pragma config DEBUG = ON		//turn on debug
#pragma config LVP = OFF		//low voltage programming
//#pragma config BOREN = OFF
//#pragma config MCLRE = ON




/** Define Constants Here ******************************************/
#define FOSC 16000000 						//Pin 7 is Fosc/4 
#define delayTime 200
#define BAUDRATE 115200					//but actually gives 38400 baud at these settings

//#define BRGVAL (((FOSC/BAUDRATE)/16) - 1) 	// this is the seed value I am using to open the PIC rs232 port 
#define BRGVAL 8								//some reason seed rounds down to 7 with formula but should be 8 which works at 115200

//#define BUFF_SIZE 59;		//even size of 3 byte messages, that way messages don't wrap around in middle of buffer. Simpler logic to handle

//for 
//#define LEDTris TRISAbits.TRISA0	//dfine LEDTris as TRISA pin 0
//#define LEDPin LATAbits.LATA0 	//define LEDpin as PortA pin 0
#define LEDTris TRISAbits.TRISA3	//dfine LEDTris as TRISA pin 0
#define LEDPin LATAbits.LATA3 		//define LEDpin as PortA pin 0

#define LFTris TRISBbits.TRISB4		//pin22 direction
#define LRTris TRISBbits.TRISB5		//pin23	direction
#define RFTris TRISAbits.TRISA0		//pin24	direction
#define RRTris TRISAbits.TRISA1		//pin25 direction
#define PWMTris TRISAbits.TRISA2	

#define LFpin LATBbits.LATB4
#define LRpin LATBbits.LATB5
#define RFpin LATAbits.LATA0
#define RRpin LATAbits.LATA1
#define PWMpin LATAbits.LATA2 


/** Local Function Prototypes **************************************/
void high_isr(void);  	
void low_isr(void);  
void printToUART(int data, int carriageReturn);
void getCMD(void);			//grabs command from buffer
void parseCMD(void);		//parses 3 byte bang bang commands sent from getCMD
void parseCMD5(void);		//parses 5 byte commands which contain the duty cycle as well as direction
void heartBeat(void);		//check heartbeat on LED of the system
void singleCharCMDparse(void);	//for doing any one byte cmds from hyper terminal not using the circular buffer
	
void OpenPWM1( char period );
void SetOutputPWM1(unsigned char outputconfig, unsigned char outputmode);
void SetDCPWM1(unsigned int dutycycle);
void OpenPWM2( char period );
void SetOutputPWM2(unsigned char outputconfig, unsigned char outputmode);
void SetDCPWM2(unsigned int dutycycle);
void OpenPWM3( char period );
void SetOutputPWM3(unsigned char outputconfig, unsigned char outputmode);
void SetDCPWM3(unsigned int dutycycle);
//void OpenPWM4( char period );
void OpenPWM4 ( unsigned char period, unsigned char timer_source );
//void SetOutputPWM2(unsigned char outputconfig, unsigned char outputmode);
void SetDCPWM4(unsigned int dutycycle);

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


/** Global Variables ***********************************************/

int speed = 0;				//at some point add pwm with this byte telling how fast
							//the pwm > 20khz and the speed being the pulsewidth between off and fully on.  0 is on for FastTraxx
int cmdFlag = 0;			//1 if cmd avail 0 otherwise
int echoCMD = 0;			//prints cmd recieved or not
int singleCMDenable = 0;	//enable single byte commands from hyper terminal?

int singleCMDready = 0;		//flag for telling if single command is ready

volatile char ready = 0;	//for flagging the period of the PWM

int pitter = 0;				//count so can toggle pin once per second to check heart beat
int TMR2state = 0;			//track state machine for pwm of timer2
int TMR2start = 0;			//sync the pulse width to the period
int pulseWidth = 254;    //37 = 1.5ms

//RX buffer
const int BUFF_SIZE = 45;		//multiples of 5 for the buffer size cause message 5 bytes long, #lf50 ; header:cmd:dutyCycle	
int minRXcount = 5;				//whats the min amount of bytes recieved in buffer before can parse one message
char RXbuffer[45] = { 0 };
char cmd[2] = {'0','0'};		//holds command recieved
int dutyCycle[2] = {'0','0'};	//holds duty cycle recieved
int RXbyte = 0;				//holds the byte from RCREG in the interrupt routine
int RXhead = 0;
int RXtail = 0;
int RXcount = 0;

float driveSpeed = 0; 	//holds conversion factor from duty cycle to actual value put inside pwm module

/*******************************************************************
* Function:        void main(void)
********************************************************************/
#pragma code
void main (void)
{

OSCCONbits.IRCF = 0b111;			//set internal OSC to 16MHz
//OSCCONbits.SCS = 0b11;
//OSCTUNEbits.PLLEN = 0b0;


//***UART init  
//DS p275 
//measuring 9600 on the scope gives 96us or 1/9600 = 10.42kHz which is high time of 1 bit on scope

ANSELCbits.ANSC6 = 0;	//digital io
ANSELCbits.ANSC7 = 0;	//digital io
TRISCbits.RC6 = 0;		//RC6 as output for UART1 TX 
TRISCbits.RC7 = 1;		//input

RCSTA1bits.SPEN1 = 1;

	
Open1USART( USART_TX_INT_OFF &
  USART_RX_INT_ON &
  USART_ASYNCH_MODE &
  USART_EIGHT_BIT &
  USART_CONT_RX &
  USART_BRGH_HIGH,
  BRGVAL );

BAUDCON1bits.BRG16 = 0;
		
IPR1bits.RCIP = 0;			//UART RX interrupt low priority 
IPR1bits.TXIP = 0;			//UART TX interrupt low priority



//***PORTA Init
	LEDTris = 0;	//set LED pin to output
	LEDPin = 1;		//set LED pin to high
	PWMTris = 0;	//set PWM to output for debugging
	PWMpin = 0;		//initial state of PWMpin

//***PORTB Init

	LFTris = 0;  	//set as outputs for driving fast traxx
	LRTris = 0;
	RFTris = 0;
	RRTris = 0;
	
	LFpin = 1;		//set as initially high which disables driving		
	LRpin = 1;
	RFpin = 1;
	RRpin = 1;
	
	//PWM3 channel
	ANSELBbits.ANSB5 = 0;	//digital io
	TRISBbits.TRISB5 = 0;
	LATBbits.LATB5 = 0;
	
	//PWM4 channel
	ANSELBbits.ANSB0 = 0;	//digital io
	TRISBbits.TRISB0 = 0;
	LATBbits.LATB0 = 0;


//***PORTC Init
	//PWM1 Channel
	TRISCbits.TRISC2 = 0;
	LATCbits.LATC2 = 0;
	
	//PWM2 Channel
	TRISCbits.TRISC1 = 0;
	LATCbits.LATC1 = 0;






//***PWM Init
//DS p184
//CCP1 = RC2 = pin13
//PIE1bits.TMR2IE = 1;

//****PWM the manual way, want to get the functions working
////1
//TRISCbits.RC2 = 1;	//output for pwm CCP1 pin13
//
////2
//CCPTMRS0bits.C2TSEL = 0b11;
//
////3
//PR2 = 200; 	//period value
//
////4
//CCP1CONbits.CCP1M = 0b1111;		//set mode to pwm
//
////5   load duty cycle
//CCPR1L = 0b00001000;		//low byte
//CCP1CONbits.DC1B = 0b00;  //lowest 2 bits
//
////6  p175
//PIR1bits.TMR2IF = 0;		//clear interrupt flag
//T2CONbits.T2CKPS = 0b00;	//select prescale value
//T2CONbits.T2OUTPS = 0b0000;	//select post scaler value
//T2CONbits.TMR2ON = 1;		//turn timer2 on
//
//7
//wait until TMR2 over flows and sets Interrupt flag
//then enable the TRISbits pin for RC2
//****end PWM Init




//Processing clock for pitter
	OpenTimer0( TIMER_INT_ON &
	T0_16BIT &
	T0_SOURCE_INT &
	T0_PS_1_1 );

//PWM timer for all 4 channels
	OpenTimer2( TIMER_INT_ON &
	T2_PS_1_1 &
	T2_POST_1_1 );



//PWM setup
//had to find library files and physically copy functions into my project or they wouldn't work
	OpenPWM1(200);	//sets the period
	SetOutputPWM1(SINGLE_OUT, PWM_MODE_1);	//enhanced channel so has this function
	SetDCPWM1(800);	//sets duty cycle
	
	OpenPWM2(200);
	SetOutputPWM2(SINGLE_OUT, PWM_MODE_1);  //enhanced channel so has this function
	SetDCPWM2(800);	//sets duty cycle
	
	OpenPWM3(200);
	SetOutputPWM3(SINGLE_OUT, PWM_MODE_1);  //enhanced channel so has this function
	SetDCPWM3(800);	//sets duty cycle
	
	OpenPWM4(200,2);
	//SetOutputPWM3(SINGLE_OUT, PWM_MODE_1);  
	SetDCPWM4(800);	//sets duty cycle

//***Timer Setups  //can't use Timer1 because disables RC0 & RC1
//TRISCbits.RC2 = 0;
//TRISCbits.RC1 = 0;


//T2CONbits.TMR2ON = 0;	//start disabled

//***Interrupt Init
	RCONbits.IPEN = 1;  		// Enable interrupt priority levels
	INTCONbits.GIE = 1; 		// Enable global interrupts 
	INTCONbits.PEIE = 1; 		//enable peripheral interrupt sources
	PIE1bits.RC1IE = 1;			//enable RX UART interrupts
	RCSTA1bits.CREN1 = 1;		//enables RX UART circuitry
	PIR1bits.RC1IF = 0;			//clear RX UART interrupt flag
	
	INTCONbits.PEIE_GIEL = 1;
	INTCONbits.GIE_GIEH = 1;
	INTCONbits.GIEL = 1;
	INTCONbits.GIEH = 1;
	
	
	INTCON2bits.TMR0IP = 0;  	//set Timer 0 low priority
	IPR1bits.TMR2IP = 1;		//set Timer 2 high priority
	//IPR1bits.TMR1IP = 0;		//set Timer 1 low priority




//***************************************
//MAIN LOOP
//***************************************
while (1)
{



   
	while(!ready)
	{
		//process all commands in buffe, if buffer gets big might need to tweak this to not take so much time
		while(RXcount > 0)		
			getCMD();		
		
	}
	
		
		ready = 0; 		//after processing is done then disable
		heartBeat();
		
	
		

 

}//end while

//***************************************
//END MAIN LOOP
//***************************************

}//end main



/*******************************************************************
* Additional Helper Functions
********************************************************************/


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


	
	//Timer2 interrupt Poll, PULSE WIDTH
	if(PIR1bits.TMR2IF && PIE1bits.TMR2IE){
	
		//putc1USART('h');
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

//**Timer0 interrupt Poll, PULSE PERIOD
	if(INTCONbits.TMR0IF && INTCONbits.TMR0IE)
	{
		INTCONbits.TMR0IF = 0;				//clear Timer 0 interrupt

		PWMpin = !PWMpin;
		WriteTimer0(26000);   				//Timer0 16bit, ps_1_1, Write(26000) gives 50Hz pitter
											//Timer0 16bit, ps_1_1, Write(56530) gives 4.4kHz pitter
											//65536 max counts for 16 bit
		if(PWMpin) ready = 1;
	
	}// END TMR0 interrupt Poll


	
//***UART RX interrupt Poll
	if ((PIR1bits.RC1IF) && (PIE1bits.RC1IE)) // act only on eusart rx interrupts
	{
		//putc1USART('L');

		PIR1bits.RC1IF = 0;  //reset interrupt flag
		RXbyte = RCREG1;

		//for pwm cmds and other single byte commands defined in function below if enabled
		if(singleCMDenable){
			singleCharCMDparse();
		}
		
		
//seems to freeze if 3 consecutive messages sent but not 2. may not ever happen that fast in practice
		RXbuffer[RXhead] = RXbyte;
		RXcount++;
		RXhead++;
		if(RXhead >= BUFF_SIZE){
			RXhead = RXhead - BUFF_SIZE;
		}
	
	}// end if bits
}//end low_isr



/*******************************************************************
* Function:			void printToUart(int data)
* Input Variables:	data to be printed
* Output Return:	none
* Overview:			Simple way to print to UART
********************************************************************/

//TX interrupts are off for this function
void printToUART(int data, int carriageReturn)
{
	int x;
	char dataArray[10];

	//sprintf(dataArray,"%d", data);
	
	for(x = 0; x<strlen(dataArray); x++)
	{
		while (Busy1USART());  		//If the uart is busy wait
		Write1USART(dataArray[x]);   //transmit a char
	}

	if(carriageReturn){
		while (Busy1USART());
		Write1USART('\n');
		Write1USART('\r');
	}

}//end printToUART


/*******************************************************************
* Function:			void parseCMD(void)
* Input Variables:	grab command from buffer and place in cmd array
* Output Return:	none
* Overview:			grabs valid commands... 2 bytes.  prepares them for parse command
********************************************************************/

void getCMD(void)
{
	
	//search for message header '#'
	if(RXcount >= minRXcount){

		if(RXbuffer[RXtail] == '#'){
			cmd[0] = RXbuffer[RXtail+1];
			cmd[1] = RXbuffer[RXtail+2];
			
			//convert char to int then store it
			//(char- '0') is unicode standard for converting char to decimal equivalent number
			dutyCycle[0] = RXbuffer[RXtail+3] - '0';    
			dutyCycle[1] = RXbuffer[RXtail+4] - '0';
			
			if(echoCMD){			
				while (Busy1USART());
					Write1USART(cmd[0]);
					Write1USART(cmd[1]);
				while (Busy1USART());
					Write1USART(dutyCycle[0]);
					Write1USART(dutyCycle[1]);
			}

//while (Busy1USART());
//	printToUART(RXtail,1);

			//increment tail but wrap around to begining if end of buffer
			RXtail = RXtail + minRXcount;
			if(RXtail >= BUFF_SIZE){
				RXtail = RXtail - BUFF_SIZE;
			}
			
			RXcount = RXcount - minRXcount;	//5 bytes for one valid cmd

			//parseCMD();
			parseCMD5();
			LEDPin = !LEDPin; 	//toggle LED Pin

		}else{
			RXcount--; //one byte removed
			RXtail++;
		}
			

	}

}//end getCMD

/*******************************************************************
* Function:			void parseCMD(void)
* Input Variables:	parse cmd from array filled by getCMD
* Output Return:	none
* Overview:			parses/interpets valid commands... 2 bytes
********************************************************************/

void parseCMD(void)
{

//1 is stop
//0 is go
//the fet logic defines this in the fastTraxx

	if(cmd[0] == 'l'){
		if(cmd[1] == 'f'){
			LRpin = 1;	//stop Left reverse
			LFpin = 0;	//start Left forward
		}
		else if(cmd[1] == 's'){
			LRpin = 1;	
			LFpin = 1;	
		}
		else if(cmd[1] == 'r'){
			LFpin = 1;	
			LRpin = 0;	
		}
	}else if(cmd[0] == 'r'){
		if(cmd[1] == 'f'){
			RRpin = 1;	
			RFpin = 0;	
		}
		else if(cmd[1] == 's'){
			RFpin = 1;	
			RRpin = 1;	
		}
		else if(cmd[1] == 'r'){
			RFpin = 1;	
			RRpin = 0;	
		}
	}


	//after command is executed clear it for new commands
	cmd[0] = '0';
	cmd[1] = '0';


}

//parses commands that are 5 bytes long that contain duty cycle
void parseCMD5(void)
{
	int DCtemp = 0;		//temp place for duty cycle

	float ratio = 297/99;   //is about the least usable speed so after that just max out to 800 for almost full pwm duty cycle
							//tweaked the 297 until about 94% skips to full power and anything below that scales to reasonable speeds
	
//1 is stop
//0 is go
//the fet logic defines this in the fastTraxx
//PWM1 = left forward , PWM2 = left reverse
//PWM2 = right forward, PWM3 = right reverse

//parse duty cycle
	//confusing because high duty cycle actually means slow speed because of fastraxx FET control
	DCtemp = dutyCycle[0] * 10 + dutyCycle[1];
	//proportion of full speed 99% to 200 solved for x then inverted so big is small and small is big	
	driveSpeed = DCtemp*ratio;						
	if(driveSpeed > 280) driveSpeed = 800;		//once the duty cycle gets so low just max the speed all out
	
	//putrs1USART("\n\rDUTY CYCLE \n\r");
	//printToUART(DCtemp,1);
//	printf("\n\rDUTY CYCLE \n\r");
//	printf("%d \n\r",DCtemp);
//	printf("%4.2f \n\r",driveSpeed);		//won't printf floats correctly
//	printToUART(driveSpeed,1);
	
	
	//SetDCPWM2(255);
	
//parse direction
	if(cmd[0] == 'l'){
		if(cmd[1] == 'f'){
			SetDCPWM2(800);
			SetDCPWM1(driveSpeed);
			
			//LRpin = 1;	//stop Left reverse
			//LFpin = 0;	//start Left forward
		}
		else if(cmd[1] == 's'){
			SetDCPWM2(800);
			SetDCPWM1(800);
			
			//LRpin = 1;	
			//LFpin = 1;	
		}
		else if(cmd[1] == 'r'){
			SetDCPWM2(driveSpeed);
			SetDCPWM1(800);
	
//			LRpin = 0;
//			LFpin = 1;	
		}
	}else if(cmd[0] == 'r'){
		if(cmd[1] == 'f'){
			SetDCPWM4(800);
			SetDCPWM3(driveSpeed);
			
			//RRpin = 1;	
			//RFpin = 0;	
		}
		else if(cmd[1] == 's'){
			SetDCPWM4(800);
			SetDCPWM3(800);
			
			//RRpin = 1;
			//RFpin = 1;	
		}
		else if(cmd[1] == 'r'){
			SetDCPWM4(driveSpeed);
			SetDCPWM3(800);
			
			//RRpin = 0;
			//RFpin = 1;	
		}
	}


	//after command is executed clear it for new commands
	cmd[0] = '0';
	cmd[1] = '0';
	dutyCycle[0] = 0;
	dutyCycle[1] = 0;


}



/*******************************************************************
* Function:			void heartBeat(void)
* Input Variables:	check system health for parsing messages at 1 sec
* Output Return:	none
* Overview:			flips led on and off
********************************************************************/

void heartBeat(){
//show heartbeat of the control loop.  stays on 1 sec then off 1 sec
		pitter++;
		if(pitter >=50)
		{
			LEDPin = !LEDPin; 	//toggle LED Pin
			pitter = 0;
		}


}


void singleCharCMDparse(){
	
		
		if(RXbyte == '1'){
			SetDCPWM1(10);	//sets duty cycle
			SetDCPWM2(10);	//sets duty cycle
			SetDCPWM3(10);	//sets duty cycle
			SetDCPWM4(10);	//sets duty cycle
		} 
		if(RXbyte == '2'){
			SetDCPWM1(153);	//sets duty cycle
			SetDCPWM2(153);	//sets duty cycle
			SetDCPWM3(153);	//sets duty cycle
			SetDCPWM4(153);	//sets duty cycle
		
		}
		if(RXbyte == '3'){
			SetDCPWM1(176);	//sets duty cycle
			SetDCPWM2(176);	//sets duty cycle
			SetDCPWM3(176);	//sets duty cycle
			SetDCPWM4(176);	//sets duty cycle
		
		}
		if(RXbyte == '4'){
			SetDCPWM1(200);	//sets duty cycle
			SetDCPWM2(200);	//sets duty cycle
			SetDCPWM3(200);	//sets duty cycle
			SetDCPWM4(200);	//sets duty cycle
		
		}	 	 
		if(RXbyte == 'p'){
			SetDCPWM1(++driveSpeed);	//sets duty cycle
			SetDCPWM2(++driveSpeed);	//sets duty cycle
			SetDCPWM3(++driveSpeed);	//sets duty cycle
			SetDCPWM4(++driveSpeed);	//sets duty cycle
//			printToUART(driveSpeed, 1);
		}
		if(RXbyte == 'l'){
			SetDCPWM1(--driveSpeed);	//sets duty cycle
			SetDCPWM2(--driveSpeed);	//sets duty cycle
			SetDCPWM3(--driveSpeed);	//sets duty cycle
			SetDCPWM4(--driveSpeed);	//sets duty cycle
//			printToUART(driveSpeed, 1);
		}

}
