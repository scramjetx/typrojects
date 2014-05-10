//ATTINY 4313 chip

//Design Notes:
//PortA not working...PA0 and PA1.  Has something to do with XTAL pins..suppose to change to IO if using internal oscillator.

#define F_CPU 8000000UL
#include <avr/io.h>
#include <util/delay.h>

int main(void) {

	DDRA = (1<<PA0)|(1<<PA1); //Anodes
	DDRD = (1<<PD2)|(1<<PD3)|(1<<PD4)|(1<<PD5)|(1<<PD6); //Anodes

	DDRB = (1<<PB1)|(1<<PB2)|(1<<PB3)|(1<<PB4); /* Cathodes set PB0 to output */
    PORTB = (0<<PB1)|(0<<PB2)|(0<<PB3)|(0<<PB4); /* Cathodes V low */

    while(1) {
        PORTD = (0<<PD2)|(0<<PD3)|(0<<PD4)|(0<<PD5)|(0<<PD6); /* LED on or Pin High */
        _delay_ms(200);
        PORTD = (1<<PD2)|(1<<PD3)|(1<<PD4)|(1<<PD5)|(1<<PD6); /* LED off or Pin Low */
        _delay_ms(200);
    }
    return 0;
}
