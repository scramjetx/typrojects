//ATTINY 4313 chip

//Design Notes:

//What's Next?
//now can display digits so display them in the correct place. add digit 1-4 parameter for digitDisplay function
//then can change test temp to larger number so it prints 003F or 072F for the temp...or maybe no leading 0's

//Optimizations on.  Properties->Build->Settings->Optimizations

#define F_CPU 8000000UL
#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>

//#include "includes\bubble_display.h"

#define LOOP_RATE 1				//how fast to run the loop
#define TICKS_PER_HZ 100000		//how many ticks elapse per hz to get the loop rate


//Main variables
char tempDegArray [] = "999";  //hold the char string of temperature reading for display

//Function Prototypes
void parseTempReading(char *, int8_t);


int main(void)
{
	//Segment Anodes
	DDRA |= (1<<PA0) | (1<<PA1);
	DDRB |= (1<<PB6) | (1<<PB2);
	DDRD |= (1<<PD2) | (1<<PD3) | (1<<PD5) | (1<<PD6);

	//Segment Cathodes
	DDRB |= (1<<PB1) | (1<<PB3) | (1<<PB4); 	//Cathodes set to output
	DDRD |= (1<<PD4);							//Cathodes set to output
	PORTB |= (0<<PB1) | (0<<PB3) | (0<<PB4);	//pin set low.
	PORTD |= (0<<PD4);							//pin set low.

	uint8_t STATE = 1;
	int64_t timer = 1;  //placeholder for timer ticks to know when to start main state machine and clock how fast it loops

	//timer must be incremented by an interrupt or equivalent...then reset when bottom of if statement

	USART_Init();


	int8_t testTemp = 0;

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



				parseTempReading(tempDegArray, testTemp);

				testTemp++;
				if(testTemp>9)
					testTemp = 0;

				STATE = 3;	//transition to next state
			}


			//**********************************************
			//State 3:
			//display data
			//**********************************************
			if(STATE == 3)
			{
				//test code
//				if(tempDegArray[0] == '1')
//					testDisplay();
//
//				USART_SendString(tempDegArray);
//				USART_SendString("\n\r");
				//end test code

				computeDigitDisplay(tempDegArray[0]);



				STATE = 1; 	//return to first state
			}

			timer = 0;
		} //end if

		timer++;

    } //end while(1)




	return 0;
}

void parseTempReading(char * c, int8_t i)
{
	//sprintf takes alot of memory to convert a int to a string/char array
	//could possibly do a modulus operation by 10 to extract digits to save memory
	sprintf(c, "%d", i);

}
