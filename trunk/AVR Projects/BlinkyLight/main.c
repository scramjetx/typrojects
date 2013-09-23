/*
 * main.c
 *
 *  Created on: Feb 26, 2013
 *      Author: JensenT
 */

#define F_CPU 16000000UL
//these are the include files. They are outside the project folder
#include <avr/io.h>
#include <util/delay.h>

int main (void)
{
	//Set PORTC to all outputs
	DDRB = 0xFF;
	//create an infinite loop
	while(1) {
		//this turns pin D0 on and off
		//turns B0 LOW
		PORTB &= ~(1<<PB0);
		//PAUSE 250 miliseconds
		_delay_ms(100);
		//turns B0 HIGH
		PORTB |= 1<<0;
		//PAUSE 250 miliseconds
		_delay_ms(100);
	};
}
