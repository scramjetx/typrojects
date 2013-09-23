
/** Header Files ***************************************************/
#include<p18f26k22.h>

#include <timers.h>
#include <pwm.h>
#include <pconfig.h>
#include <delays.h>
#include <usart.h>
#include <stdio.h> 
#include <string.h>


void OpenPWM1( char period )
{
  
  	CCP1CON |= 0b00001100;    //ccpxm3:ccpxm0 11xx=pwm mode  

	TRISCbits.TRISC2 = 0;
//---------------------------------------------------
  	T2CONbits.TMR2ON = 0;  // STOP TIMER2 registers to POR state
  	PR2 = period;          // Set period
  	T2CONbits.TMR2ON = 1;  // Turn on PWM1

}


void ClosePWM1(void)
{
  CCP1CON=0;
//  TRISCbits.TRISC2=1;		//NO need to tristate the port and disable pwm output on it.  Commented to maintain consistency with respect to other colse pwm functions

}

void SetOutputPWM1(unsigned char outputconfig, unsigned char outputmode)
{
  char eccpmx; /* will be set iff bit 1 of __CONFIG3H is set */
  unsigned char TBLPTR_U, TBLPTR_L;

_asm
movff TBLPTRU, TBLPTR_U
movff TBLPTRL, TBLPTR_L
_endasm

  /* set P1M1 and P1M0 */
  	outputconfig |= 0b00111111;
  	outputmode |= 0b11111100;
  	CCP1CON = (CCP1CON | 0b11000000) & outputconfig;
  /* set CCP1M3, CCP1M2, CCP1M1, CCP1M0 */
  CCP1CON = (CCP1CON | 0b00001111) & outputmode;
  
#if defined (PWM_CONFIG3L_V1) || defined (PWM_CONFIG3L_V2)
#if defined (PWM1_IO_V4)
  if (((*(unsigned char far rom *)__CONFIG3H) & 0b00000010))
     eccpmx=0xff;
  else
     eccpmx = 0;
#endif
#endif
//--------------------------------------

#if defined PWM1_IO_V1	
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISCbits.TRISC2 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISCbits.TRISC2 = 0;
			TRISDbits.TRISD5 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISCbits.TRISC2 = 0;
			TRISDbits.TRISD5 = 0;
			TRISDbits.TRISD6 = 0;
			TRISDbits.TRISD7 = 0;
  	}
#elif defined PWM1_IO_V2 
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISCbits.TRISC2 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISCbits.TRISC2 = 0;
			TRISEbits.TRISE6 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISCbits.TRISC2 = 0;
			TRISEbits.TRISE6 = 0;
			TRISEbits.TRISE5 = 0;
			TRISGbits.TRISG4 = 0;
  	}
#elif defined PWM1_IO_V3 
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISCbits.TRISC2 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISCbits.TRISC2 = 0;
			TRISDbits.TRISD0 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISCbits.TRISC2 = 0;
			TRISDbits.TRISD0 = 0;
			TRISEbits.TRISE5 = 0;
			TRISGbits.TRISG4 = 0;
  	}
	
#elif defined PWM1_IO_V4
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISCbits.TRISC2 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISCbits.TRISC2 = 0;
		if(eccpmx)
			TRISEbits.TRISE6 = 0;
		else
			 TRISHbits.TRISH7 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISCbits.TRISC2 = 0;
		if(eccpmx)	{TRISEbits.TRISE6 = 0;TRISEbits.TRISE5 = 0;}
		else		{TRISHbits.TRISH7 = 0;TRISHbits.TRISH6 = 0;}
			TRISGbits.TRISG4 = 0;
  	}
	
#elif defined PWM1_IO_V6
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISCbits.TRISC5 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISCbits.TRISC5 = 0;
			TRISCbits.TRISC4 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISCbits.TRISC5 = 0;
			TRISCbits.TRISC4 = 0;
			TRISCbits.TRISC3 = 0;
			TRISCbits.TRISC2 = 0;

  	}	
//---------------------------------------------
#endif

_asm
movff TBLPTR_U, TBLPTRU
movff TBLPTR_L, TBLPTRL
_endasm
}


void SetDCPWM1(unsigned int dutycycle)
{
  union PWMDC DCycle;

  // Save the dutycycle value in the union
  DCycle.lpwm = dutycycle << 6;

  // Write the high byte into CCPR1L
  CCPR1L = DCycle.bpwm[1];

  // Write the low byte into CCP1CON5:4
  CCP1CON = (CCP1CON & 0xCF) | ((DCycle.bpwm[0] >> 2) & 0x30);
}





void OpenPWM2( char period )
{
#define __CONFIG3L 0x300004
#define __CONFIG3H 0x300005
char pmmode; /* will be set iff either bit 0 AND bit 1 or bit 5 AND bit 4 of __CONFIG3L are set */
char ccp2mx; /* will be set iff bit 0 of __CONFIG3H is set */
unsigned char TBLPTR_U, TBLPTR_L;

_asm
movff TBLPTRU, TBLPTR_U
movff TBLPTRL, TBLPTR_L
_endasm

  	CCP2CON = 0b00001100;    //ccpxm3:ccpxm0 11xx=pwm mode

 	if (((*(unsigned char far rom *)__CONFIG3H) & 0b00000001))
    	ccp2mx=0xff;
 	else
     	ccp2mx = 0;

#if defined PWM_CONFIG3L_V1
  	if (((*(unsigned char far rom *)__CONFIG3L) & 0b00000011) == 0b00000011)
     	pmmode=0xff;
  	else
     	pmmode = 0;
#elif defined PWM_CONFIG3L_V2
	if (((*(unsigned char far rom *)__CONFIG3L) & 0b00110000) == 0b00110000)
     	pmmode=0xff;
  	else
    	pmmode = 0;
#endif

#if defined CCP2_V0
	TRISCbits.TRISC1 = 0;
#elif defined PWM2_IO_V1
	if(ccp2mx)	TRISCbits.TRISC1 = 0;
	else 		TRISBbits.TRISB3 = 0;
#elif defined PWM2_IO_V7
	TRISBbits.TRISB3 = 0;
#elif defined PWM2_IO_V8
	TRISEbits.TRISE7 = 0;
#elif defined PWM2_IO_V2
	if(ccp2mx)	TRISCbits.TRISC1 = 0;
	else 		TRISEbits.TRISE7 = 0;
#elif defined PWM2_IO_V4
	if(ccp2mx)	TRISCbits.TRISC1 = 0;
	else if(pmmode)		TRISEbits.TRISE7 = 0;		// Microcontroller mode
	else				TRISBbits.TRISB3 = 0;
#endif
//-------------------------------------
  		T2CONbits.TMR2ON = 0;  // STOP TIMER2 registers to POR state
  		PR2 = period;          // Set period
  		T2CONbits.TMR2ON = 1;  // Turn on PWM1
		
_asm
movff TBLPTR_U, TBLPTRU
movff TBLPTR_L, TBLPTRL
_endasm
}

void ClosePWM2(void)
{
  CCP2CON=0;          // Disable PWM2
}
void SetOutputPWM2(unsigned char outputconfig, unsigned char outputmode)
{
  char pmmode; /* will be set iff bit 0 AND bit 1 of __CONFIG3L are set */
  char ccp2mx; /* will be set iff bit 0 of __CONFIG3H is set */
  unsigned char TBLPTR_U, TBLPTR_L;

_asm
movff TBLPTRU, TBLPTR_U
movff TBLPTRL, TBLPTR_L
_endasm

  /* set P1M1 and P1M0 */
  CCP2CON = (CCP2CON | 0b11000000) & outputconfig;
  /* set CCP1M3, CCP1M2, CCP1M1, CCP1M0 */
  CCP2CON = (CCP2CON | 0b00001111) & outputmode;

#if defined (PWM_CONFIG3L_V1) || defined (PWM_CONFIG3L_V2)
 	if (((*(unsigned char far rom *)__CONFIG3H) & 0b00000001))
    	ccp2mx=0xff;
 	else
     	ccp2mx = 0;
#endif

#if defined PWM_CONFIG3L_V1
  	if (((*(unsigned char far rom *)__CONFIG3L) & 0b00000011) == 0b00000011)
     	pmmode=0xff;
  	else
     	pmmode = 0;
#elif defined PWM_CONFIG3L_V2
	if (((*(unsigned char far rom *)__CONFIG3L) & 0b00110000) == 0b00110000)
     	pmmode=0xff;
  	else
    	pmmode = 0;
#endif

#if defined PWM2_IO_V5
	
	if (SINGLE_OUT == outputconfig)
  	{
   	 	if(ccp2mx) 	TRISCbits.TRISC1 = 0;
		else 		TRISEbits.TRISE7 = 0; 
  	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{
		if(ccp2mx) 	TRISCbits.TRISC1 = 0;
		else 		TRISEbits.TRISE7 = 0;
					TRISEbits.TRISE2 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
		if(ccp2mx) 	TRISCbits.TRISC1 = 0;
		else 		TRISEbits.TRISE7 = 0;
					TRISEbits.TRISE2 = 0;
					TRISEbits.TRISE1 = 0;
					TRISEbits.TRISE0 = 0;
  	}	
#elif defined PWM2_IO_V3
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISCbits.TRISC1 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISCbits.TRISC1 = 0;
			TRISEbits.TRISE2 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISCbits.TRISC1 = 0;
			TRISEbits.TRISE2 = 0;
			TRISEbits.TRISE1 = 0;
			TRISEbits.TRISE0 = 0;
  	}
#elif defined PWM2_IO_V6
	if (SINGLE_OUT == outputconfig)
  	{
		if(ccp2mx) 			TRISCbits.TRISC1 = 0;
		else if(pmmode)		TRISEbits.TRISE7 = 0; 
		else				TRISBbits.TRISB3 = 0;
  	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{
		if(ccp2mx) 			TRISCbits.TRISC1 = 0;
		else if(pmmode)		TRISEbits.TRISE7 = 0; 
		else				TRISBbits.TRISB3 = 0;
							TRISEbits.TRISE2 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
		if(ccp2mx) 			TRISCbits.TRISC1 = 0;
		else if(pmmode)		TRISEbits.TRISE7 = 0; 
		else				TRISBbits.TRISB3 = 0;
							TRISEbits.TRISE2 = 0;
							TRISEbits.TRISE1 = 0;
							TRISEbits.TRISE0 = 0;
  	}	
#endif

_asm
movff TBLPTR_U, TBLPTRU
movff TBLPTR_L, TBLPTRL
_endasm

}


void SetDCPWM2(unsigned int dutycycle)
{
  union PWMDC DCycle;

  // Save the dutycycle value in the union
  DCycle.lpwm = dutycycle << 6;

  // Write the high byte into CCPR2L
  CCPR2L = DCycle.bpwm[1];

  // Write the low byte into CCP2CON5:4
  CCP2CON = (CCP2CON & 0xCF) | ((DCycle.bpwm[0] >> 2) & 0x30);
}


void OpenPWM3( char period )
{
        CCP3CON=0b00001100;    //ccpxm3:ccpxm0 11xx=pwm mode

  		T2CONbits.TMR2ON = 0;  // STOP TIMER2 registers to POR state
  		PR2 = period;          // Set period
  		T2CONbits.TMR2ON = 1;  // Turn on PWM3
}

void ClosePWM3(void)
{
  CCP3CON=0;            // Turn off PWM3

}

void SetOutputPWM3(unsigned char outputconfig, unsigned char outputmode)
{
  #define __CONFIG3H 0x300005
  char eccpmx; /* will be set iff bit 1 of __CONFIG3H is set */
  unsigned char TBLPTR_U, TBLPTR_L;

_asm
movff TBLPTRU, TBLPTR_U
movff TBLPTRL, TBLPTR_L
_endasm

  /* set P1M1 and P1M0 */
  CCP3CON = (CCP3CON | 0b11000000) & outputconfig;
  /* set CCP1M3, CCP1M2, CCP1M1, CCP1M0 */
  CCP3CON = (CCP3CON | 0b00001111) & outputmode;
     
#if defined (PWM1_IO_V4)
  if (((*(unsigned char far rom *)__CONFIG3H) & 0b00000010))
     eccpmx=0xff;
  else
     eccpmx = 0;
#endif

#if defined PWM3_IO_V1
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISDbits.TRISD1 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISDbits.TRISD1 = 0;
			TRISEbits.TRISE4 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISDbits.TRISD1 = 0;
			TRISEbits.TRISE4 = 0;
			TRISEbits.TRISE3 = 0;
			TRISDbits.TRISD2 = 0;
  	}
#elif defined PWM3_IO_V2
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISGbits.TRISG0 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISGbits.TRISG0 = 0;
			TRISEbits.TRISE4 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISGbits.TRISG0 = 0;
			TRISEbits.TRISE4 = 0;
			TRISEbits.TRISE3 = 0;
			TRISGbits.TRISG3 = 0;
  	}
#elif defined PWM3_IO_V2
	if (SINGLE_OUT == outputconfig)
  	{
   	 	 	TRISGbits.TRISG0 = 0;
	}
  	else if (IS_DUAL_PWM(outputconfig))
  	{				
			TRISGbits.TRISG0 = 0;
			if(eccpmx)	TRISEbits.TRISE4 = 0;
			else 		TRISHbits.TRISH5 = 0;
  	}
  	else if (IS_QUAD_PWM(outputconfig))
  	{
			TRISGbits.TRISG0 = 0;
			if(eccpmx)	{TRISEbits.TRISE4 = 0;TRISEbits.TRISE3 = 0;}
			else 		{TRISHbits.TRISH5 = 0;TRISHbits.TRISH4 = 0;}
			TRISGbits.TRISG3 = 0;
  	}
#endif

_asm
movff TBLPTR_U, TBLPTRU
movff TBLPTR_L, TBLPTRL
_endasm

}
void SetDCPWM3(unsigned int dutycycle)
{
  union PWMDC DCycle;

  // Save the dutycycle value in the union
  DCycle.lpwm = dutycycle << 6;

  // Write the high byte into CCPR3L
  CCPR3L = DCycle.bpwm[1];

  // Write the low byte into CCP3CON5:4
  CCP3CON = (CCP3CON & 0xCF) | ((DCycle.bpwm[0] >> 2) & 0x30);
}


void OpenPWM4 ( unsigned char period, unsigned char timer_source )
{

  CCP4CON=0b00001100;    //ccpxm3:ccpxm0 11xx=pwm mode

  //configure timer source for CCP
  CCPTMRS1 &= 0b11111100;
  CCPTMRS1 |= ((timer_source&0b00110000)>>4);   
  
    PWM4_TRIS = 0;

  if( (CCPTMRS1&0x03)==0x00)
  {
  T2CONbits.TMR2ON = 0;  // STOP TIMERx registers to POR state
  PR2 = period;          // Set period
  T2CONbits.TMR2ON = 1;  // Turn on PWMx
  }
  
  else if( (CCPTMRS1&0x03)==0x01)
  {
  T4CONbits.TMR4ON = 0;  // STOP TIMERx registers to POR state
  PR4 = period;          // Set period
  T4CONbits.TMR4ON = 1;  // Turn on PWMx
  }
  
  else if( (CCPTMRS1&0x03)==0x02)
  {
  T6CONbits.TMR6ON = 0;  // STOP TIMERx registers to POR state
  PR6 = period;          // Set period
  T6CONbits.TMR6ON = 1;  // Turn on PWMx  
  }
}

void ClosePWM4(void)
{  
	CCP4CON=0;            // Turn off PWM4
	
}

void SetDCPWM4(unsigned int dutycycle)
{
  union PWMDC DCycle;

  // Save the dutycycle value in the union
  DCycle.lpwm = dutycycle << 6;

  // Write the high byte into CCPR4L
  CCPR4L = DCycle.bpwm[1];

  // Write the low byte into CCP4CON5:4
  CCP4CON = (CCP4CON & 0xCF) | ((DCycle.bpwm[0] >> 2) & 0x30);
}
