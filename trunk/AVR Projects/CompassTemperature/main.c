//ATTINY 4313 chip

//Design Notes:

//What's Next?
//figure out how to do display loop so it samples at some slow rate 5hz...then displays the rest of the time at some rate..if want to adjust brightness




//Optimizations on.  Properties->Build->Settings->Optimizations
//to get rid of implicit declaration -> right click function then source add includes.  And magically solves it
//to change color of highlighted text got to preferences->editors->text editors->annotations

#define F_CPU 8000000UL
#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <stdbool.h>


#define LOOP_RATE 1				//how fast to run the loop
#define TICKS_PER_HZ 1000		//how many ticks elapse per hz to get the loop rate


//Main variables
char tempDegArray [] = "999";  //hold the char string of temperature reading for display
int8_t degReading = 1;			   //hold temp reading in deg F
uint8_t numDigits = 0;		   //store the number of digits the temp reading has
char displayTempUnits = 'F';			//what units to display temp
bool goFlag = false;			//tells main loop to process.  Keeps it running at defined rate

char displayArray [] = "999F"; //holds the string to be displayed

//Function Prototypes
void parseTempReading(char *, int8_t);
uint8_t findNumDigits(uint8_t);


/********************************************************************************
Interrupt Routines
********************************************************************************/
// timer1 overflw
ISR(TIMER1_OVF_vect)
{
	//USART_SendChar('1');

}

// timer0 overflow
ISR(TIMER0_OVF_vect)
{
	//USART_SendChar('0');
	goFlag = true;
}

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

	//*******************************
	//Timer setup
	//*******************************
	// enable timer overflow interrupt for both Timer0 and Timer1
	TIMSK=(1<<TOIE0) | (1<<TOIE1);

	// set timer0 counter initial value to 0
	TCNT0=0x00;

	// start timer0 with /1024 prescaler
	TCCR0B = (1<<CS02) | (1<<CS00);

	// lets turn on 16 bit timer1 also with /1024
	TCCR1B |= (1 << CS10) | (1 << CS12);

	//*********************************************

	// enable interrupts
	sei();


	int8_t testTemp = 0;


	while(1)
	{


		if(goFlag)
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

				//Celsius or Farenheit?
				displayTempUnits = 'F';

				//***************************************
				//Build temperature array to be displayed
				if(numDigits == 1)
				{
					displayArray[0] = '_';
					displayArray[1] = '_';
					displayArray[2] = tempDegArray[0];
				}
				else if(numDigits == 2)
				{
					displayArray[0] = '_';
					displayArray[1] = tempDegArray[0];
					displayArray[2] = tempDegArray[1];
				}
				else if(numDigits == 3)
				{
					displayArray[0] = tempDegArray[0];
					displayArray[1] = tempDegArray[1];
					displayArray[2] = tempDegArray[2];
				}

				displayArray[3] = displayTempUnits;

				//*********************************************


				//USART_SendString(displayArray);
				//USART_SendBlankline();

				//test code
				degReading++;
				if(degReading>999)
					degReading=100;
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

		goFlag = false;  //processing done

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
