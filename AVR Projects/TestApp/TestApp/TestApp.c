/*
 * TestApp.c
 *
 * Created: 2/19/2013 5:26:23 PM
 *  Author: JensenT
 */ 


#define F_CPU 4000000UL
//these are the include files. They are outside the project folder
#include <avr/io.h>
#include <util/delay.h>
//this include is in your project folder
#include "myTimer.h"

int main (void)
{
	//Set PORTC to all outputs
	DDRD = 0xFF;
	//create an infinite loop
	while(1) {
		//this turns pin C0 on and off
		//turns C0 HIGH
		PORTD |=(1<<0);
		//PAUSE 250 miliseconds
		delay_ms(3000);
		//turns C0 LOW
		PORTD &= ~(1 << 0);
		//PAUSE 250 miliseconds
		delay_ms(3000);
	};
}