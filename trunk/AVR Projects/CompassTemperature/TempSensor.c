#include <avr/io.h>
#include <inttypes.h>
#include <util/delay.h>

#include "includes/TempSensor.h"
#include "includes/Simple_USART.h"


//***abandoning this temp sensor because the lookup table still doesn't match.  really frustrating to get the accuracy based on the lookup table
//Take counts and convert them to a Farenheit reading based on a lookup table
//int16_t ConvertCountsToF(int16_t counts)
//{
//	//Counts must be shifted to account for starting the table at 0.  38 counts = -40F which is the 0th index of the lookup table.
//	int16_t tempCounts = counts;
//
//	//bound the counts
//	if(tempCounts < 38) tempCounts = 38;
//	if(tempCounts > 984) tempCounts = 984;
//
//	tempCounts = tempCounts - 38;
//
//
//	//**test code
//			USART_SendBlankline();
//			USART_SendString("tempCounts = ");
//			USART_SendInt32((int32_t)tempCounts);
//			USART_SendBlankline();
//		//**end test code
//
//	return TempLookupTable(tempCounts);
//
//}

float TMP36SensorReadingCalc(uint16_t counts)
{
	// TMP36 Temp Calc Info
	// 2 points on the line are (50C, 1000mV) and (125C, 1750mV)
	// equation tempC = mV/10 - 50
	// equation mV = tempC*10 + 500
	// test cases: 50C = 205 counts = 1000mV; 125C = 359 counts = 1750mV

	float mVperCount = 4.88;  //4.88mV
	float v = counts*mVperCount/10 - 50;

//USART_SendBlankline();
//USART_SendString("Counts = ");
//USART_SendInt32(counts);
//USART_SendBlankline();
//USART_SendString("Counts*mV = ");
//USART_SendInt32(v);
//USART_SendBlankline();

//USART_SendString("mV/1000 - 50 = ");
//USART_SendInt32(v);
//USART_SendBlankline();
//
//int32_t num = 55000;
//USART_SendString("55,000 = ");
//USART_SendInt32(num);
//USART_SendBlankline();

	return v;
}

float ConvertTempReading(float temp, char tempUnits)
{
	float t = temp;

	if(tempUnits == 'F')
	{
		//convert F reading to C
		//C = 5/9(F-32)
		t = .556 * (t - 32);

	}
	else if(tempUnits == 'C')
	{
		// convert C reading to F
		// F = 9/5*C + 32
		t = 1.8 * t + 32;

	}

	return t;
}

