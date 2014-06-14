//ATTINY 4313 chip

//Design Notes:

//What's Next?
//can't really adjust brightness...could work that if wanted
//sample an ADC and pass int to display
//found out ATTINY4313 HAS NO FREAKING ADC...order ATTINY861V-10PU as a drop in replacment


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

#define TIMER0_SOFTWARE_PRESCALE 30

//Main variables
char tempDegArray [] = "999";  //hold the char string of temperature reading for display
int8_t degReading = 1;			   //hold temp reading in deg F
uint8_t numDigits = 0;		   //store the number of digits the temp reading has
char displayTempUnits = 'F';			//what units to display temp


char displayArray [] = "999F"; //holds the string to be displayed


volatile uint16_t timer0_2ndPrescaler = 0;  //timer 0 too fast so do a software prescaler to further slow it down
volatile bool processDataFlag = false;	//Volatile if in ISR.  Tells main loop to process.  Keeps it running at defined rate
volatile bool refreshDisplayFlag = false;	//Refreshes display during times when the data is not being processed.

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
	//CLK = 8Mhz/256 count register / 1024 hardware prescale / 30 count software prescale = 1hz update rate
	if(timer0_2ndPrescaler == TIMER0_SOFTWARE_PRESCALE)
	{
		USART_SendChar('0');
		processDataFlag = true;
		refreshDisplayFlag = false;
		timer0_2ndPrescaler = 0;
	}

	timer0_2ndPrescaler++;
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

	// set timer0 counter initial value to 0
	TCNT1=0x00;

	//*********************************************

	// enable interrupts
	sei();


	int8_t testTemp = 0;


	while(1)
	{


		if(processDataFlag)
		{


			USART_SendChar('P');

			//**********************************************
			//State 1:
			// Take measurement of compass or temperature depending on which one is selected
			//**********************************************
			if(STATE == 1 )
			{
				USART_SendChar('1');

				STATE = 2;  //transition to next state
			}


			//**********************************************
			//State 2:
			//Parse reading into distinct digits for display
			//**********************************************
			if(STATE == 2)
			{
				USART_SendChar('2');

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
				USART_SendChar('3');

				//build string up to 4 digits to send to display
				//so take the digits in temp deg array and build a display array that's 4 wide
				//of form '_12F' or '__5F'

				charArrayDisplay(displayArray);

				STATE = 1; 	//return to first state
			}

			processDataFlag = false;  //processing finished
			refreshDisplayFlag = true;

		} //end if

		if(refreshDisplayFlag)
		{
			USART_SendChar('R');
			charArrayDisplay(displayArray);

		}


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
