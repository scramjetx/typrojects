//ATTINY 4313 chip

//Design Notes:
//PortA not working...PA0 and PA1.  Has something to do with XTAL pins..suppose to change to IO if using internal oscillator.

#define F_CPU 8000000UL
#include <avr/io.h>
#include <util/delay.h>

//#include "includes\bubble_display.h"

#define LOOP_RATE 100		//how fast to run the loop
#define TICKS_PER_HZ 500	//how many ticks elapse per hz to get the loop rate

#define SEG_A PD2

int main(void)
{
	//Segment Anodes
	DDRA = (1<<PA0)|(1<<PA1);
	DDRD = (1<<PD2)|(1<<PD3)|(1<<PD4)|(1<<PD5)|(1<<PD6);

	//Segment Cathodes
	DDRB = (1<<PB1)|(1<<PB2)|(1<<PB3)|(1<<PB4); 	//Cathodes set to output
	PORTB = (0<<PB1)|(0<<PB2)|(0<<PB3)|(0<<PB4);	//pin set low.

	uint8_t STATE = 1;
	int64_t timer = 1;  //placeholder for timer ticks to know when to start main state machine and clock how fast it loops

	//timer must be incremented by an interrupt or equivalent...then reset when bottom of if statement

	while(1)
	{


		if(timer == TICKS_PER_HZ*LOOP_RATE)
		{


			//**********************************************
			//State 1:
			// Take measurement of compass or temperature depending on which one is selected
			//**********************************************
			if(STATE == 1 )
			{


				STATE = 2;  //transition to next state
			}


			//**********************************************
			//State 2:
			//Parse reading into distinct digits for display
			//**********************************************
			if(STATE == 2)
			{


				STATE = 3;	//transition to next state
			}


			//**********************************************
			//State 3:
			//display data
			//**********************************************
			if(STATE == 3)
			{


				STATE = 1; 	//return to first state
			}

			timer = 0;
		} //end if



		timer++;

    } //end while(1)




	return 0;
}
