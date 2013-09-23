
/** Header Files ***************************************************/
#include<p18f26k22.h>

#include <timers.h>
#include <pwm.h>
#include <pconfig.h>
#include <delays.h>
#include <usart.h>
#include <stdio.h> 
#include <string.h>

#define BAUDRATE 115200					//but actually gives 38400 baud at these settings

//#define BRGVAL (((FOSC/BAUDRATE)/16) - 1) 	// this is the seed value I am using to open the PIC rs232 port 
#define BRGVAL 8								//some reason seed rounds down to 7 with formula but should be 8 which works at 115200


void initUART(void){

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

}
