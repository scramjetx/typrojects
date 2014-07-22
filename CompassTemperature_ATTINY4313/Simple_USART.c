#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>

#include "includes/Simple_USART.h"






void USART_Init(void)
{
	// Set baud rate
	UBRRH = (uint8_t)(USART_UBBR_VALUE>>8);
	UBRRL = (uint8_t)USART_UBBR_VALUE;
	// Set frame format to 8 data bits, no parity, 1 stop bit
	UCSRC = (0<<USBS)|(1<<UCSZ1)|(1<<UCSZ0);
	// Enable receiver and transmitter
	UCSRB = (1<<RXEN)|(1<<TXEN);
}

void USART_SendBlankline()
{
	USART_SendString("\n\r");
}

void USART_SendChar(uint8_t data)
{
	// Wait if a byte is being transmitted
	while((UCSRA&(1<<UDRE)) == 0);
	// Transmit data
	UDR = data;
}

void USART_SendString(char s[]) {
	int i = 0;

	while(i < 64)
	{ // don't get stuck if it is a bad string
		if( s[i] == '\0' ) break; // quit on string terminator
		USART_SendChar(s[i++]);
	}
}
