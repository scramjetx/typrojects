#define SOME_CONST 99

//these declarations below maybe not needed for port cause declared in function prototype and can be used in function without reassignment

//holds port assignment for each pin
//uint8_t segAport, segBport, segCport, segDport, segEport, segFport, segGport, segDPport;
#define SEG_A_PORT PORTD
#define SEG_A_PIN  5

#define SEG_B_PORT PORTD
#define SEG_B_PIN  2

#define SEG_C_PORT PORTB
#define SEG_C_PIN  6

#define SEG_D_PORT PORTA
#define SEG_D_PIN  1

#define SEG_E_PORT PORTD
#define SEG_E_PIN  3

#define SEG_F_PORT PORTB
#define SEG_F_PIN  2

#define SEG_G_PORT PORTA
#define SEG_G_PIN  0

#define SEG_DP_PORT PORTD
#define SEG_DP_PIN  6

//******Syntax error...wish this would work
//wired up in hardware
//                      A,     B,     C,     D,     E,     F,     G,     DP
//#define SEG_PORTS [] {PORTD, PORTD, PORTB, PORTA, PORTD, PORTB, PORTA, PORTD}  //8
//#define SEG_PINS []  {PD5,   PD2,   PB6,   PA1,   PD3,   PB2,   PA0,   PD6 }	//8

//holds pin # assignment when passed in
//uint8_t segApin, segBpin, segCpin, segDpin, segEpin, segFpin, segGpin, segDPpin;

//holds port assignment for each pin
//uint8_t displayDigit1port, displayDigit2port, displayDigit3port, displayDigit4port;

//holds pin# assignment when passed in
//uint8_t displayDigit1pin, displayDigit2pin, displayDigit3pin, displayDigit4pin;

uint8_t zero = 0b1111110; // 0
uint8_t one = 0b0110000; // 1
uint8_t two = 0b1101101; // 2
uint8_t three = 0b1111001; // 3
uint8_t four = 0b0110011; // 4
uint8_t five = 0b1011011; // 5
uint8_t six = 0b1011111; // 6
uint8_t seven = 0b1110000; // 7
uint8_t eight = 0b1111111; // 8
uint8_t nine = 0b1111011; // 9
//0b1110111, // 65 'A'
//0b0011111, // 66 'B'
uint8_t c = 0b1001110; // 12  "C"
//0b0111101, // 68 'D'
uint8_t e = 0b1001111; // 69 'E'
uint8_t f = 0b1000111; // 15  "F"
//0b1011110, // 71 'G'
//0b0110111, // 72 'H'
//0b0110000, // 73 'I'
//0b0111000, // 74 'J'
//0b0000000, // 75 'K'  NO DISPLAY
//0b0001110, // 76 'L'
//0b0000000, // 77 'M'  NO DISPLAY
uint8_t n = 0b0010101; // 78 'N'
//0b1111110, // 79 'O'
//0b1100111, // 80 'P'
//0b1110011, // 81 'Q'
//0b0000101, // 82 'R'
uint8_t s = 0b1011011; // 83 'S'
//0b0001111, // 84 'T'
//0b0111110, // 85 'U'
//0b0000000, // 86 'V'  NO DISPLAY
uint8_t w = 0b0111111; // W
//0b0000000, // 88 'X'  NO DISPLAY
//0b0111011, // 89 'Y'
//0b0000000, // 90 'Z'  NO DISPLAY

//void initDisplay(uint8_t segAport, uint8_t segBport, uint8_t segCport, uint8_t segDport, uint8_t segEport, uint8_t segFport, uint8_t segGport, uint8_t segDPport,
//		uint8_t segApin, uint8_t segBpin, uint8_t segCpin, uint8_t segDpin, uint8_t segEpin, uint8_t segFpin, uint8_t segGpin, uint8_t segDPpin,
//		uint8_t displayDigit1port, uint8_t displayDigit2port, uint8_t displayDigit3port, uint8_t displayDigit4port,
//		uint8_t displayDigit1pin, uint8_t displayDigit2pin, uint8_t displayDigit3pin, uint8_t displayDigit4pin
//		);

void initDisplay();
void testDisplay();


