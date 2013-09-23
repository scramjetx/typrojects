//this is an include file.

//if you are using the delay command, this should be in the project folder
//your code should have this line: #include "myTimer.h"

//create a function for your delay
void delay_ms(uint16_t ms) {
		//while loop that creates a delay for the duration of the millisecond countdown
        while ( ms ) 
        {
                _delay_ms(1);
                ms--;
        }
}
