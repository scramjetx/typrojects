/*
 * Simple_USART.h
 *
 *  Created on: Jun 3, 2014
 *      Author: JensenT
 */

#ifndef SIMPLE_USART_H_
#define SIMPLE_USART_H_

#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>

//NOTE to get this baud have to disable divide clock by 8 command inside the fuse bits via settings menu.
#define BAUD
#define USART_BAUD 9600L
#define USART_UBBR_VALUE ((F_CPU/(USART_BAUD<<4))-1)


void USART_Init(void);
void USART_SendChar(uint8_t data);
void USART_SendString(char s[]);
void USART_SendBlankline();



#endif /* SIMPLE_USART_H_ */
