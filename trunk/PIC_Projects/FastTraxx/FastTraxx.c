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

//use timer0,1,3,5 for the 4 channels when doing pwm at fast rate.



//done
//50hz control loop with heartbeat working
//will parse all messages in buffer, may run into trouble if alot in the faster time frames when pwming the outputs

/** Header Files ***************************************************/
#include<p18f26k22.h>

#include <timers.h>
#include <pwm.h>
#include <pconfig.h>
#include <delays.h>
#include <usart.h>
#include <stdio.h> 
#include <string.h>



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
//#define LEDPin LATAbits.LATA0 		//define LEDpin as PortA pin 0
#define LEDTris TRISAbits.TRISA3	//dfine LEDTris as TRISA pin 0
#define LEDPin LATAbits.LATA3 		//define LEDpin as PortA pin 0

#define LFTris TRISBbits.TRISB4		//pin22 direction
#define LRTris TRISBbits.TRISB5		//pin23	direction
#define RFTris TRISAbits.TRISA0		//pin24	direction
#define RRTris TRISAbits.TRISA1		//pin25 direction

#define LFpin LATBbits.LATB4
#define LRpin LATBbits.LATB5
#define RFpin LATAbits.LATA0
#define RRpin LATAbits.LATA1


/** Local Function Prototypes **************************************/
void high_isr(void);  	
void low_isr(void);  
void printToUART(int data, int carriageReturn);
void getCMD(void);			//grabs command from buffer
void parseCMD(void);		//parses commands sent from getCMD
void heartBeat(void);		//check heartbeat on LED of the system


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
char cmd[2] = {'0','0'};
int speed = 0;		//at some point add pwm with this byte telling how fast
					//the pwm > 20khz and the speed being the pulsewidth between off and fully on.  0 is on for FastTraxx
int cmdFlag = 0;			//1 if cmd avail 0 otherwise
int echoCMD = 0;	//prints cmd recieved or not

volatile char ready = 0;	//for flagging the period of the PWM

int pitter = 0;				//count so can toggle pin once per second to check heart beat

//RX buffer
const int BUFF_SIZE = 45;		//multiples of 3 for the buffer size cause message 3 bytes long	
char RXbuffer[45] = { 0 };
int RXhead = 0;
int RXtail = 0;
int RXcount = 0;



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


//***PORTB Init
LFTris = 0;  	//set as outputs for driving fast traxx
LRTris = 0;
RFTris = 0;
RRTris = 0;

LFpin = 1;		//set as initially high which disables driving		
LRpin = 1;
RFpin = 1;
RRpin = 1;


//***PORTC Init



//***PWM Init
//DS p184
//CCP1 = RC2 = pin13
//PIE1bits.TMR2IE = 1;

//****seems to work but don't know why.. and for four channels might be tough
////1
//TRISCbits.RC2 = 1;	//output for pwm CCP1 pin13
//
////2
//CCPTMRS0bits.C2TSEL = 0b11;
//
////3
//PR2 = 255; 	//period value??  does this set the high AND low byte?
//
////4
//CCP1CONbits.CCP1M = 0b1111;		//set mode to pwm
//
////5   load duty cycle
//CCPR1L = 0b1111;			//low byte
//CCP1CONbits.DC1B = 0b1111;  //high byte
//
////6
//PIR1bits.TMR2IF = 0;		//clear interrupt flag
//T2CONbits.T2CKPS = 0b11;	//select prescale value
//T2CONbits.T2OUTPS = 0b1111;	//select post scaler value
//T2CONbits.TMR2ON = 1;		//turn timer2 on
//
////7
////wait until TMR2 over flows and sets Interrupt flag
////then enable the TRISbits pin for RC2
//****end seems to work


//***Timer Setups  //can't use Timer1 because disables RC0 & RC1
TRISCbits.RC2 = 0;

//pwm period
//OpenTimer0( TIMER_INT_ON &
//T0_16BIT &
//T0_SOURCE_INT &
//T0_PS_1_1 );

OpenTimer2( TIMER_INT_ON &
T2_PS_1_16 &
T2_POST_1_10 );

//pwm duty cycle
//OpenTimer2( TIMER_INT_ON &
//T2_PS_1_16 &
//T2_POST_1_4);  


//***Interrupt Init
RCONbits.IPEN = 1;  		// Enable interrupt priority levels
INTCONbits.GIE = 1; 		// Enable global interrupts 
INTCONbits.PEIE = 1; 		//enable peripheral interrupt sources
PIE1bits.RC1IE = 1;			//enable RX UART interrupts
RCSTA1bits.CREN1 = 1;		//enables RX UART circuitry
PIR1bits.RC1IF = 0;			//clear RX UART interrupt flag

//INTCONbits.PEIE_GIEL = 1;
//INTCONbits.GIE_GIEH = 1;
//INTCONbits.GIEL = 1;
//INTCONbits.GIEH = 1;


INTCON2bits.TMR0IP = 0;  	//set Timer 0 low priority
IPR1bits.TMR2IP = 1;		//set Timer 2 high priority
//IPR1bits.TMR1IP = 0;		//set Timer 1 low priority




//***************************************
//MAIN LOOP
//***************************************
while (1)
{


//	LFpin = !LFpin;		
//	LRpin = !LRpin;
//	RFpin = !RFpin;
//	RRpin = !RRpin;


//	Delay10KTCYx(250);	//delay 250k cycles  (1 sec at 1MHz since each instruction takes 4 cycles

    //sets up the timing for the 50hz cycle 
	while(ready)
	{
		//process all commands in buffe, if buffer gets big might need to tweak this to not take so much time
		while(RXcount > 0)		
			getCMD();		
		
			while (Busy1USART());
			Write1USART('B');
			Write1USART('D');
		ready = 0; 		//after processing is done then disable
		
		heartBeat();

	
//debug stuff
//printToUART(RCSTA1bits.OERR, 1);
	
		if(RCSTA1bits.OERR){
			RCSTA1bits.CREN = 0;
			RCSTA1bits.CREN = 1;

		}
	}	


//		putrs1USART("Baud");
//		printToUART(BRGVAL, 1);
		
	
//	if(cmdFlag){

		//get rid of echo to speed things up?
		//while (Busy1USART());
		//Write1USART(cmd[0]);
		//Write1USART(cmd[1]);

		//now call some parsing function to decipher the command
		//parseCMD();

				

//		cmdFlag = 0;		//cmd processed so clear
//	}

 

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
int i = 9;
//putc1USART('h');
	
	//Timer2 interrupt Poll
	if(PIR1bits.TMR2IF && PIE1bits.TMR2IE){
	
			WriteTimer2(80);				//timer2 T2_PS_1_16, T2_POST_1_10, write(80) gives 50.4 Hz 
			
			ready = 1;						//process when the ready flag is enabled so only does it 50 times a sec

		//	LEDPin = !LEDPin; 	//toggle LED Pin

//	putc1USART('H');
	//	TRISCbits.RC2 = 0;

//		switch (TMR2state)                  	//select TMR2state machine for PWM 
//	    {
//	    case 0: PR2 = 1;						//stall from tmr2 init to make more accurate calc
//	        break;
//	    case 1: PR2 = pulseWidth;				//set pulseWidth desired by user
//			PORTCbits.RC2 = !PORTCbits.RC2;
//	        break;
//	    case 2: T2CONbits.TMR2ON = 0;
//	        PORTCbits.RC2 = !PORTCbits.RC2;
//			TMR2state = 0;
//			T2CONbits.TMR2ON = 0;  				//disable timer2 till next 50hz cycle		
//			break;
//	    }
//		TMR2state++; 							//increment pwm state machine
//
		PIR1bits.TMR2IF = 0;					//clear Timer 2 interrupt
//	
//
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
	//putc1USART('T');			
//		servoTimer++;						//running at 50hz so 50 ticks = 1 sec
		LATCbits.LATC2 = !LATCbits.LATC2;		//64916 @ 128 post scale
		WriteTimer0(65000);   				// 65225 @ 256 post scale Sets Timer for 50Hz Interrupt @ 128 prescaler
											//Timer0 16bit, ps_1_1, Write(26000) gives 50Hz pitter
											//Timer0 16bit, ps_1_1, Write(56530) gives 4.4kHz pitter
											//65536 max counts for 16 bit
			
		
//	LEDPin = !LEDPin; 	//toggle LED Pin

		INTCONbits.TMR0IF = 0;				//clear Timer 0 interrupt
	}// END TMR0 interrupt Poll



	
//***UART RX interrupt Poll
	if ((PIR1bits.RC1IF) && (PIE1bits.RC1IE)) // act only on eusart rx interrupts
	{
		PIR1bits.RC1IF = 0;  //reset interrupt flag
		
		//putc1USART('L');
		
		

//seems to freeze if 3 consecutive messages sent but not 2. may not ever happen that fast in practice
		RXbuffer[RXhead] = RCREG1;
		RXcount++;
		RXhead++;
		if(RXhead >= BUFF_SIZE){
			RXhead = RXhead - BUFF_SIZE;
		}
	

//old routine without buffer or message header
		//store two command bytes.  Motor and direction.  LF, LS, LR, RF, RS, RR
//		if(cmdFlag == 0){
//			if(cmd[0] == '0')
//				cmd[0] = RCREG1;
//			else{
//				cmd[1] = RCREG1;
//				cmdFlag = 1;		//cmd ready
//				
//			}
//		}


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

	sprintf(dataArray,"%d", data);
	
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
	if(RXcount >= 3){

		if(RXbuffer[RXtail] == '#'){
			cmd[0] = RXbuffer[RXtail+1];
			cmd[1] = RXbuffer[RXtail+2];
			
			if(echoCMD){			
				while (Busy1USART());
					Write1USART(cmd[0]);
					Write1USART(cmd[1]);
			}

//while (Busy1USART());
//	printToUART(RXtail,1);

			//increment tail but wrap around to begining if end of buffer
			RXtail = RXtail + 3;
			if(RXtail >= BUFF_SIZE){
				RXtail = RXtail - BUFF_SIZE;
			}
			
			RXcount = RXcount-3;	//3 bytes for one valid cmd

			parseCMD();
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



/*******************************************************************
* Function:			void heartBeat(void)
* Input Variables:	check system health for parsing messages at 1 sec
* Output Return:	none
* Overview:			flips led on and off
********************************************************************/

void heartBeat(){
//show heartbeat of the control loop.  stays on 1 sec then off 1 sec
		pitter++;
		if(pitter >=100)
		{
			LEDPin = !LEDPin; 	//toggle LED Pin
			pitter = 0;
		}


}
