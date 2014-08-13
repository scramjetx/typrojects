#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>
#include <stdio.h>

#include "includes/Simple_USART.h"






void USART_Init(void)
{
	// Set baud rate
	UBRR1H = (uint8_t)(USART_UBBR_VALUE>>8);
	UBRR1L = (uint8_t)USART_UBBR_VALUE;
	// Set frame format to 8 data bits, no parity, 1 stop bit
	UCSR1C = (3<<UCSZ10);
	// Enable receiver and transmitter
	UCSR1B = (1<<RXEN1)|(1<<TXEN1);
}

void USART_SendBlankline()
{
	USART_SendString("\n\r");
}

void USART_SendChar(uint8_t data)
{
	// Wait if a byte is being transmitted
	while((UCSR1A&(1<<UDRE1)) == 0);
	// Transmit data
	UDR1 = data;
}

void USART_SendString(char s[]) {
	int i = 0;

	while(i < 64)
	{ // don't get stuck if it is a bad string
		if( s[i] == '\0' ) break; // quit on string terminator
		USART_SendChar(s[i++]);
	}
}

void USART_SendInt32(int32_t i)
{
	char c[10];
	sprintf(c, "%ld", i);	//%d is ONLY 16bit int whereas %ld is 16 or 32bit int.
	USART_SendString(c);
}
