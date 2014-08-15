//ATMEGA32U4 Breakout

//Design Notes:

//What's Next?
	//can't really adjust brightness...could work that if wanted
	//hookup the mag sensor and display that data
	//cleanup some parts of the code into functions in main so main is a tight group of routine calls
	//refactor state machine with enumerations instead of State = 1 like Dave does so this program can be good example in future.
	//create an ADC.c and ADC.h so can reuse that code later
	//create an timer.c and timer.h so can resuse that code later

//Optimizations on.  Properties->Build->Settings->Optimizations
//to get rid of implicit declaration -> right click function then source add includes.  And magically solves it
//to change color of highlighted text got to preferences->editors->text editors->annotations

//if project doesn't recognize DCR or PORTB etc.. right click project and do index->rebuild.  Fixes the errors.

//********************************************************************************
//Includes
//********************************************************************************

#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <stdbool.h>

#include "includes/TempSensor.h"

//********************************************************************************
// Defines
//********************************************************************************
#define F_CPU 8000000UL
#define LOOP_RATE 1				//how fast to run the loop
#define TICKS_PER_HZ 1000		//how many ticks elapse per hz to get the loop rate

#define TIMER0_SOFTWARE_PRESCALE 7

#define ADCW _SFR_MEM16(0x78) //trick I found online for combining the ADCH and ADCL into one read. Bit Shifting wasn't working

//********************************************************************************
// Global Variables
//********************************************************************************
//Main variables
char tempDegArray [] = "999";  //hold the char string of temperature reading for display
uint16_t rawTempADC = 0;
float degReading = 0;		   //hold temp reading
uint8_t numDigits = 0;		   //store the number of digits the temp reading has
char displayTempUnits = 'F';			//what units to display temp


char displayArray [] = "999F"; //holds the string to be displayed


volatile uint16_t timer0_2ndPrescaler = 0;  //timer 0 too fast so do a software prescaler to further slow it down
volatile bool processDataFlag = false;	//Volatile if in ISR.  Tells main loop to process.  Keeps it running at defined rate
volatile bool refreshDisplayFlag = false;	//Refreshes display during times when the data is not being processed.

//Function Prototypes
void parseTempReading(char *, int16_t);
uint16_t findNumDigits(int16_t);


//********************************************************************************
// Interrupt Routines
//********************************************************************************

// timer0 overflow
ISR(TIMER0_OVF_vect)
{
	// This Timer sets display refresh rate
	// CLK = 8Mhz/2^8bit count register / 1024 hardware prescale / 7 count software prescale = 4.4hz update rate
	if(timer0_2ndPrescaler == TIMER0_SOFTWARE_PRESCALE)
	{
		//USART_SendChar('S');
		processDataFlag = true;
		refreshDisplayFlag = false;
		timer0_2ndPrescaler = 0;
	}

	timer0_2ndPrescaler++;
}

// timer1 overflow
// CLK = 8mhz/2^16bit count timer / 8 prescaler = 15.3hz it trips;
ISR(TIMER1_OVF_vect)
{
	// This Timer1 overflow triggers ADC conversion which then interrupts in the ADC_Vect

	//USART_SendChar('W');

}

//ADC Interrupt
// 10 bit ADC.  5V/2^10 = 4.883mV/Count theoretical
//measured 4.883mV/count linearly with 5V sweep.  so Counts = 1V/4.88mV = 205 Counts
ISR(ADC_vect)
{
	rawTempADC = ADCW;	//ADCW is a #define combo of ADCH and ADCL

	//USART_SendChar('A');

}


//********************************************************************************
// Main
//********************************************************************************
int main(void)
{
	uint8_t STATE = 1;		// init state machine to first state

	// Segment Anodes Init
	DDRC |= (1<<PC6) | (1<<PC7);	//set direction
	DDRF |= (1<<PF7) | (1<<PF6) | (1<<PF5) | (1<<PF4) | (1<<PF1) | (1<<PF0);	//set direction

	// Segment Cathodes Init
	DDRB |= (1<<PB4) | (1<<PB5) | (1<<PB6) | (1<<PB7);		//set to outputs
	PORTB |= (1<<PB4) | (1<<PB5) | (1<<PB6) | (1<<PB7);		//keep display digit high/off.

	USART_Init();

	//*******************************
	// Timer setup
	//*******************************
	// enable timer overflow interrupt for both Timer0 and Timer1
	TIMSK0 = (1<<TOIE0);
	TIMSK1 = (1<<TOIE1);

	// set timer0 counter initial value to 0
	TCNT0 = 0x00;
	TCNT1 = 0x00;

	// start timer0 with /1024 prescaler
	TCCR0B = (1<<CS02) | (1<<CS00);

	// lets turn on 16 bit timer1 with /8 prescaler
	TCCR1B |= (1 << CS11);
	//*********************************************


	//*******************************
	// ADC setup
	//*******************************
	ADCSRA |= (1 << ADPS2) | (1 << ADPS1) | (1 << ADPS0); // Set ADC prescaler to 128 - 62.5KHz sample rate @ 8MHz. Slowest can go

	ADMUX |= (1 << REFS0); 					// Set ADC reference to AVCC
	//ADMUX |= (1 << ADLAR); 					// Left adjust ADC result to allow easy 8 bit reading

	// Mux bits set for ADC10
	ADCSRB |= (1<< MUX5);
	ADMUX |= (1<<MUX1);

	ADCSRA |= (1 << ADATE);  				// Set ADC to auto trigger, Trigger is set in ADCSRB bits
	ADCSRB |= (1 << ADTS2) | (1 << ADTS1);  // set trigger source to Timer1 Overflow

	ADCSRA |= (1 << ADEN);  				// Enable ADC

	ADCSRA |= (1 << ADIE); 					// enable ADC interrupt
	//*********************************************


	// enable interrupts
	sei();


	//********************************************************************************
	// Main LOOP
	//********************************************************************************
	while(1)
	{

		if(processDataFlag)
		{

			//USART_SendBlankline();
			//USART_SendChar('P');

			//**********************************************
			// State 1:
			// Take measurement of compass or temperature depending on which one is selected
			// Convert from counts to temperature
			//**********************************************
			if(STATE == 1 )
			{
				//USART_SendChar('1');

//USART_SendData("Counts = ", (int32_t)rawTempADC);

				degReading = TMP36SensorReadingCalc(rawTempADC);

//USART_SendData("DegC = ", (int32_t)degReading);

				degReading = ConvertTempReading(degReading, 'C');

//USART_SendData("DegF = ", (int32_t)degReading);
//USART_SendBlankline();

				parseTempReading(tempDegArray, (int16_t)degReading);
				numDigits = findNumDigits((int16_t)degReading);

				STATE = 2;  //transition to next state
			}


			//**********************************************
			// State 2:
			// Parse reading into distinct digits for display
			//**********************************************
			if(STATE == 2)
			{
				//USART_SendChar('2');

				//Celsius or Farenheit?
				displayTempUnits = 'F';

				//***************************************
				//Build temperature array to be displayed
				//and handle negative temperatures
				if(degReading >= 0)
				{
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
				}
				else
				{
					if(numDigits == 1)
					{
						displayArray[0] = '_';
						displayArray[1] = tempDegArray[0];  //negative sign
						displayArray[2] = tempDegArray[1];
					}
					else if(numDigits == 2)
					{
						displayArray[0] = tempDegArray[0];  //negative sign
						displayArray[1] = tempDegArray[1];
						displayArray[2] = tempDegArray[2];
					}
					else if(numDigits == 3)  //can't handle 3 digits correctly of negative.  Not enough digits
					{
						displayArray[0] = tempDegArray[1];
						displayArray[1] = tempDegArray[2];
						displayArray[2] = tempDegArray[3];
					}

					displayArray[3] = displayTempUnits;

				}

				//*********************************************

				STATE = 3;	//transition to next state
			}



			//**********************************************
			// State 3:
			// display data
			//**********************************************
			if(STATE == 3)
			{
				//USART_SendChar('3');

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
			//USART_SendChar('R');
			charArrayDisplay(displayArray);

		}


    } //end while(1)




	return 0;
}//end main



//********************************************************************************
//Routines
//********************************************************************************

void parseTempReading(char * c, int16_t i)
{
	//sprintf takes alot of memory to convert a int to a string/char array
	//could possibly do a modulus operation by 10 to extract digits to save memory
	sprintf(c, "%d", i);

}

uint16_t findNumDigits(int16_t num)
{

	if ( num < 10 && num > -10 )
		return 1;
    if ( num < 100 && num > -100)
	    return 2;
    if ( num < 1000 && num > -1000)
		return 3;

}
