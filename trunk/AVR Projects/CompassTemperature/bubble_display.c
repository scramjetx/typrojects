
#include <avr/io.h>
#include <util/delay.h>

#include "includes/bubble_display.h"

//void initDisplay(uint8_t segAport, uint8_t segBport, uint8_t segCport, uint8_t segDport, uint8_t segEport, uint8_t segFport, uint8_t segGport, uint8_t segDPport,
//		uint8_t segApin, uint8_t segBpin, uint8_t segCpin, uint8_t segDpin, uint8_t segEpin, uint8_t segFpin, uint8_t segGpin, uint8_t segDPpin,
//		uint8_t displayDigit1port, uint8_t displayDigit2port, uint8_t displayDigit3port, uint8_t displayDigit4port,
//		uint8_t displayDigit1pin, uint8_t displayDigit2pin, uint8_t displayDigit3pin, uint8_t displayDigit4pin
//		)
void initDisplay()
{
	//got to OR them together. Otherwise it clears all the other bits

	SEG_A_PORT = 1<<SEG_A_PIN;
	_delay_ms(100);
	SEG_B_PORT = SEG_A_PORT | 1<<SEG_B_PIN;
	_delay_ms(100);
	SEG_C_PORT = 1<<SEG_C_PIN;
	_delay_ms(100);
	SEG_D_PORT = 1<<SEG_D_PIN;
	_delay_ms(100);
	SEG_E_PORT = SEG_B_PORT | 1<<SEG_E_PIN;
	_delay_ms(100);
	SEG_F_PORT = SEG_C_PORT | 1<<SEG_F_PIN;
	_delay_ms(100);
	SEG_G_PORT = SEG_D_PORT | 1<<SEG_G_PIN;
	_delay_ms(100);
	SEG_DP_PORT = SEG_E_PORT | 1<<SEG_DP_PIN;
	_delay_ms(100);

//	SEG_A_PORT = 0<<SEG_A_PIN;
//	_delay_ms(100);
//	SEG_B_PORT = 0<<SEG_B_PIN;
//	_delay_ms(100);
//	SEG_C_PORT = 0<<SEG_C_PIN;
//	_delay_ms(100);
//	SEG_D_PORT = 0<<SEG_D_PIN;
//	_delay_ms(100);
//	SEG_E_PORT = 0<<SEG_E_PIN;
//	_delay_ms(100);
//	SEG_F_PORT = 0<<SEG_F_PIN;
//	_delay_ms(100);
//	SEG_G_PORT = 0<<SEG_G_PIN;
//	_delay_ms(100);
//	SEG_DP_PORT = 0<<SEG_DP_PIN;
//	_delay_ms(100);





}
