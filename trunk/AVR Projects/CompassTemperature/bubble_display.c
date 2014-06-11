
#include <avr/io.h>
#include <util/delay.h>

#include "includes/bubble_display.h"
#include "includes/Simple_USART.h"

//void initDisplay(uint8_t segAport, uint8_t segBport, uint8_t segCport, uint8_t segDport, uint8_t segEport, uint8_t segFport, uint8_t segGport, uint8_t segDPport,
//		uint8_t segApin, uint8_t segBpin, uint8_t segCpin, uint8_t segDpin, uint8_t segEpin, uint8_t segFpin, uint8_t segGpin, uint8_t segDPpin,
//		uint8_t displayDigit1port, uint8_t displayDigit2port, uint8_t displayDigit3port, uint8_t displayDigit4port,
//		uint8_t displayDigit1pin, uint8_t displayDigit2pin, uint8_t displayDigit3pin, uint8_t displayDigit4pin
//		)

void charArrayDisplay(char * charArray)
{
	uint8_t numDigits = 1;	//min of 1 digit number
	uint32_t delay = 50;

	//turn on digit 1
	DIGIT1_CATHODE_PORT &= ~(1<<DIGIT1_CATHODE_PIN);

	//display
	computeDigitDisplay(charArray[0]);
	_delay_ms(delay);

	//turn off digit
	DIGIT1_CATHODE_PORT |= 1<<DIGIT1_CATHODE_PIN;



	//turn on digit 2
	DIGIT2_CATHODE_PORT &= ~(1<<DIGIT2_CATHODE_PIN);

	//display
	computeDigitDisplay(charArray[1]);
	_delay_ms(delay);

	//turn off digit
	DIGIT2_CATHODE_PORT |= 1<<DIGIT2_CATHODE_PIN;



	//turn on digit 3
	DIGIT3_CATHODE_PORT &= ~(1<<DIGIT3_CATHODE_PIN);

	//display
	computeDigitDisplay(charArray[2]);
	_delay_ms(delay);

	//turn off digit
	DIGIT3_CATHODE_PORT |= 1<<DIGIT3_CATHODE_PIN;



	//turn on digit 4
	DIGIT4_CATHODE_PORT &= ~(1<<DIGIT4_CATHODE_PIN);

	//display
	computeDigitDisplay(charArray[3]);
	_delay_ms(delay);

	//turn off digit
	DIGIT4_CATHODE_PORT |= 1<<DIGIT4_CATHODE_PIN;


}
void computeDigitDisplay(char c)
{
	uint8_t computedDigit = '~';

//	USART_SendChar(c);

	resetDisplay();

	if(c == '0')
	{
		computedDigit = digits[0];
	}
	else if(c == '1')
	{
		computedDigit = digits[1];
	}
	else if(c == '2')
	{
		computedDigit = digits[2];
	}
	else if(c == '3')
	{
		computedDigit = digits[3];
	}
	else if(c == '4')
	{
		computedDigit = digits[4];
	}
	else if(c == '5')
	{
		computedDigit = digits[5];
	}
	else if(c == '6')
	{
		computedDigit = digits[6];
	}
	else if(c == '7')
	{
		computedDigit = digits[7];
	}
	else if(c == '8')
	{
		computedDigit = digits[8];
	}
	else if(c == '9')
	{
		computedDigit = digits[9];
	}
	else if(c == 'E')
	{
		computedDigit = digits[11];
	}
	else if(c == 'F')
	{
		computedDigit = digits[12];
	}
	else if(c == '_')
	{
		computedDigit = digits[16];
	}
	else if(c == '\0')
	{
		computedDigit = digits[17];
	}

	lightUpDigitDisplay(computedDigit);
	//USART_SendString("Computed Digit: ");
	//USART_SendChar(computedDigit);
	//USART_SendBlankline();

}

void initDisplay()
{

}

void lightUpDigitDisplay(uint8_t c)
{
	//USART_SendChar(c);

	if(c & 0b10000000)
		SEG_A_PORT |= 1<<SEG_A_PIN;
	if(c & 0b01000000)
		SEG_B_PORT |= 1<<SEG_B_PIN;
	if(c & 0b00100000)
		SEG_C_PORT |= 1<<SEG_C_PIN;
	if(c & 0b00010000)
		SEG_D_PORT |= 1<<SEG_D_PIN;
	if(c & 0b00001000)
		SEG_E_PORT |= 1<<SEG_E_PIN;
	if(c & 0b00000100)
		SEG_F_PORT |= 1<<SEG_F_PIN;
	if(c & 0b00000010)
		SEG_G_PORT |= 1<<SEG_G_PIN;
	if(c & 0b00000001)
		SEG_DP_PORT |= 1<<SEG_DP_PIN;



	//_delay_ms(10);

}

void resetDisplay()
{
	//set each segment low to clear it
	SEG_A_PORT &= ~(1<<SEG_A_PIN);
	SEG_B_PORT &= ~(1<<SEG_B_PIN);
	SEG_C_PORT &= ~(1<<SEG_C_PIN);
	SEG_D_PORT &= ~(1<<SEG_D_PIN);
	SEG_E_PORT &= ~(1<<SEG_E_PIN);
	SEG_F_PORT &= ~(1<<SEG_F_PIN);
	SEG_G_PORT &= ~(1<<SEG_G_PIN);
	SEG_DP_PORT &= ~(1<<SEG_DP_PIN);
}

void testDisplay()
{
	//got to OR them together. Otherwise it clears all the other bits
		uint8_t delay = 10;

		SEG_A_PORT |= 1<<SEG_A_PIN;
		_delay_ms(delay);
		SEG_B_PORT |= 1<<SEG_B_PIN;
		_delay_ms(delay);
		SEG_C_PORT |= 1<<SEG_C_PIN;
		_delay_ms(delay);
		SEG_D_PORT |= 1<<SEG_D_PIN;
		_delay_ms(delay);
		SEG_E_PORT |= 1<<SEG_E_PIN;
		_delay_ms(delay);
		SEG_F_PORT |= 1<<SEG_F_PIN;
		_delay_ms(delay);
		SEG_G_PORT |= 1<<SEG_G_PIN;
		_delay_ms(delay);
		SEG_DP_PORT |= 1<<SEG_DP_PIN;
		_delay_ms(delay);

		resetDisplay();


}
