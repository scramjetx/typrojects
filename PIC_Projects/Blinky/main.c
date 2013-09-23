#include<p18f26k22.h>

#pragma config FOSC = INTIO7 	//Internal oscillator block, CLKOUT function on RA6, port function on RA7 
#pragma config WDTEN = OFF		//disable watchdog timer
#pragma config DEBUG = ON

#define LEDPin LATAbits.LATA0 		//define LEDpin as PortA pin 0
#define LEDTris TRISAbits.TRISA0	//dfine LEDTris as TRISA pin 0


void main()
{

	LEDTris = 0;	//set LED pin to output
	LEDPin = 1;		//set LED pin to high

	while(1)
	{

		LEDPin = !LEDPin; 	//toggle LED Pin
		Delay10KTCYx(25);	//delay 250k cycles  (1 sec at 1MHz since each instruction takes 4 cycles

	}





//end main
} 