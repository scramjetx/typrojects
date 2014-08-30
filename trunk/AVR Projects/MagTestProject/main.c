/*
hmc5883l lib sample

copyright (c) Davide Gironi, 2012

Released under GPLv3.
Please refer to LICENSE file for licensing information.
*/


//TODO
//probably ditch this UART library and get it working without it. Then try to get this more robust uart library working


#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include "math.h"

#include "Includes/hmc5883l.h"

#define MAGN_DOCALIBRATION 0 //enable or disable magnetometer calibration
#define magncal_getrawdata(mxraw, myraw, mzraw) hmc5883l_getrawdata(mxraw, myraw, mzraw); //set the function that return magnetometer raw values
#include "Includes/magn_docalibration.h"

#include "Includes/Simple_USART.h"

int main(void) {
    int16_t mxraw = 0;
    int16_t myraw = 0;
    int16_t mzraw = 0;
    double mx = 0;
    double my = 0;
    double mz = 0;

	char itmp[10];

    //init uart
	USART_Init();

	//init interrupt
	sei();

	//init hmc5883l
	hmc5883l_init();

	#if MAGN_DOCALIBRATION  == 1
	magncal_docalibrationclient(uart_putc, uart_getc);
	#endif

	for(;;) {
		hmc5883l_getrawdata(&mxraw, &myraw, &mzraw);
		hmc5883l_getdata(&mx, &my, &mz);

		//get magnetic heading (in degrees)
		float heading = 0;
		heading = atan2((double)myraw,(double)mxraw)*57.29578;
		//add magnetic declination (optional)
		//get magnetic declination (in degrees) http://magnetic-declination.com
		//if you have an EAST declination use +, if you have a WEST declination use -
		//es, my declination is 1.73 positive
		//float declination = 1.73;
		//heading += declination;
		//check 360degree heading
		if(heading < 0)
			heading = 360 + heading;

		itoa(mxraw, itmp, 10); USART_SendString(itmp); USART_SendChar(' ');
		itoa(myraw, itmp, 10); USART_SendString(itmp); USART_SendChar(' ');
		itoa(mzraw, itmp, 10); USART_SendString(itmp); USART_SendChar(' ');
		USART_SendBlankline();
		dtostrf(mx, 3, 5, itmp); USART_SendString(itmp); USART_SendChar(' ');
		dtostrf(my, 3, 5, itmp); USART_SendString(itmp); USART_SendChar(' ');
		dtostrf(mz, 3, 5, itmp); USART_SendString(itmp); USART_SendChar(' ');
		USART_SendBlankline();
		dtostrf(heading, 3, 5, itmp); USART_SendString(itmp); USART_SendChar(' ');
		USART_SendBlankline();

		_delay_ms(500);
	}

}
