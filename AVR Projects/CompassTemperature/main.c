//ATTINY 4313 chip

//Design Notes:
//PortA not working...PA0 and PA1.  Has something to do with XTAL pins..suppose to change to IO if using internal oscillator.

#define F_CPU 8000000UL
#include <avr/io.h>
#include <util/delay.h>

#include "Includes/bubble_display.h"

#define LOOP_RATE 100		//how fast to run the loop
#define TICKS_PER_HZ 500	//how many ticks elapse per hz to get the loop rate

int main(void)
{

	uint8_t STATE = 1;
	int32_t timer = 1;  //placeholder for timer ticks to know when to start main state machine and clock how fast it loops

	//timer must be incremented by an interrupt or equivalent...then reset when bottom of if statement

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





    }




	return 0;
}
