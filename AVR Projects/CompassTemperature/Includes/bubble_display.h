#define NUM_OF_DISPLAY_DIGITS 4


//defines ports that the led segments are on so we can clear them which resets the display
#define SEG_PORTS1 PORTB
#define SEG_PORTS2 PORTC
#define SEG_PORTS3 PORTF

//holds port assignment for each pin
#define SEG_A_PORT PORTC
#define SEG_A_PIN  6

#define SEG_B_PORT PORTC
#define SEG_B_PIN  7

#define SEG_C_PORT PORTF
#define SEG_C_PIN  7

#define SEG_D_PORT PORTF
#define SEG_D_PIN  6

#define SEG_E_PORT PORTF
#define SEG_E_PIN  5

#define SEG_F_PORT PORTF
#define SEG_F_PIN  4

#define SEG_G_PORT PORTF
#define SEG_G_PIN  1

#define SEG_DP_PORT PORTF
#define SEG_DP_PIN  0


//Segment Cathodes
#define DIGIT1_CATHODE_PORT PORTB
#define DIGIT1_CATHODE_PIN 4

#define DIGIT2_CATHODE_PORT PORTB
#define DIGIT2_CATHODE_PIN 5

#define DIGIT3_CATHODE_PORT PORTB
#define DIGIT3_CATHODE_PIN 6

#define DIGIT4_CATHODE_PORT PORTB
#define DIGIT4_CATHODE_PIN 7


//segment bit order: A, B, C, D, E, F, G, DP
uint8_t digits [19] = {
		0b11111100, // 0
		0b01100000, // 1
		0b11011010, // 2
		0b11110010, // 3
		0b01100110, // 4
		0b10110110, // 5
		0b10111110, // 6
		0b11100000, // 7
		0b11111110, // 8
		0b11110110, // 9
		//0b1110111, // 65 'A'
		//0b0011111, // 66 'B'
		0b10011100, // 12  "C"
		//0b0111101, // 68 'D'
		0b10011110, // 69 'E'
		0b10001110, // 15  "F"
		//0b1011110, // 71 'G'
		//0b0110111, // 72 'H'
		//0b0110000, // 73 'I'
		//0b0111000, // 74 'J'
		//0b0000000, // 75 'K'  NO DISPLAY
		//0b0001110, // 76 'L'
		//0b0000000, // 77 'M'  NO DISPLAY
		0b00101010, // 78 'N'
		//0b1111110, // 79 'O'
		//0b1100111, // 80 'P'
		//0b1110011, // 81 'Q'
		//0b0000101, // 82 'R'
		0b10110110, // 83 'S'
		//0b0001111, // 84 'T'
		//0b0111110, // 85 'U'
		//0b0000000, // 86 'V'  NO DISPLAY
		0b01111110, // W
		//0b0000000, // 88 'X'  NO DISPLAY
		//0b0111011, // 89 'Y'
		//0b0000000, // 90 'Z'  NO DISPLAY
		0b00010000, // 95 '_'
		0b00000000,  // \0 or NULL
		0b00000010	// 45 '-'
};

void charArrayDisplay(char*);
void computeDigitDisplay(char);
void initDisplay();
void lightUpDigitDisplay(uint8_t);
void testDisplay();

