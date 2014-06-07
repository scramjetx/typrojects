//ATTINY 4313 chip

//Design Notes:

//What's Next?
//then can change test temp to larger number so it prints 003F or 072F for the temp...or maybe no leading 0's
//figure out how to do display loop so it samples at some slow rate 5hz...then displays the rest of the time at some rate..if want to adjust brightness
//figure out why merge temp display isn't displaying what I think it should. Passing arrays to functions is killing me.  Data looks like it's static.




//Optimizations on.  Properties->Build->Settings->Optimizations

#define F_CPU 8000000UL
#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>

//#include "includes\bubble_display.h"

#define LOOP_RATE 1				//how fast to run the loop
#define TICKS_PER_HZ 1000		//how many ticks elapse per hz to get the loop rate


//Main variables
char tempDegArray [] = "999";  //hold the char string of temperature reading for display
int8_t degReading = 123;			   //hold temp reading in deg F
uint8_t numDigits = 0;		   //store the number of digits the temp reading has
char displayTempUnits = 'F';			//what units to display temp

char displayArray [] = "999F"; //holds the string to be displayed

//Function Prototypes
void parseTempReading(char *, int8_t);
uint8_t findNumDigits(uint8_t);
char * buildTempDisplayArray(char [], uint8_t, char);

int main(void)
{
	//Segment Anodes set to outputs
	DDRA |= (1<<PA0) | (1<<PA1);
	DDRB |= (1<<PB6) | (1<<PB2);
	DDRD |= (1<<PD2) | (1<<PD3) | (1<<PD5) | (1<<PD6);

	//Segment Cathodes set to outputs
	DDRB |= (1<<PB1) | (1<<PB3) | (1<<PB4);
	DDRD |= (1<<PD4);
	PORTB |= (1<<PB1) | (1<<PB3) | (1<<PB4);	//keep display digit high/off.
	PORTD |= (1<<PD4);							//keep display digit high/off.

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

				parseTempReading(tempDegArray, degReading);
				numDigits = findNumDigits(degReading);

				//not working
				USART_SendChar(numDigits);
				USART_SendBlankline();

				//Celsius or Farenheit?
				displayTempUnits = 'F';

				//Build temperature array to be displayed
				if(numDigits == 1)
				{
					displayArray[2] = tempDegArray[0];
				}
				else if(numDigits == 2)
				{
					displayArray[1] = tempDegArray[0];
					displayArray[2] = tempDegArray[1];
				}
				else if(numDigits == 2)
				{
					displayArray[0] = tempDegArray[0];
					displayArray[1] = tempDegArray[1];
					displayArray[2] = tempDegArray[2];
				}

				displayArray[3] = displayTempUnits;

				USART_SendString(displayArray);
				USART_SendBlankline();

				//test code
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

				//build string up to 4 digits to send to display
				//so take the digits in temp deg array and build a display array that's 4 wide
				//of form '_12F' or '__5F'


				charArrayDisplay(displayArray);

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

uint8_t findNumDigits(uint8_t num)
{

	if ( num < 10 )
		return 1;
    if ( num < 100 )
	    return 2;
    if ( num < 1000 )
		return 3;

}
