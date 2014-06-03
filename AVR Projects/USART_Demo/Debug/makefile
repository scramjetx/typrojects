################################################################################
# Automatically-generated file. Do not edit!
################################################################################

-include ../makefile.init

RM := rm -rf

# All of the sources participating in the build are defined here
-include sources.mk
-include subdir.mk
-include objects.mk

ifneq ($(MAKECMDGOALS),clean)
ifneq ($(strip $(C_DEPS)),)
-include $(C_DEPS)
endif
ifneq ($(strip $(ASM_DEPS)),)
-include $(ASM_DEPS)
endif
ifneq ($(strip $(S_DEPS)),)
-include $(S_DEPS)
endif
ifneq ($(strip $(S_UPPER_DEPS)),)
-include $(S_UPPER_DEPS)
endif
endif

-include ../makefile.defs

# Add inputs and outputs from these tool invocations to the build variables 
FLASH_IMAGE += \
USART_Demo.hex \

SIZEDUMMY += \
sizedummy \

AVRDUDEDUMMY += \
avrdudedummy \


# All Target
all: USART_Demo.elf secondary-outputs

# Tool invocations
USART_Demo.elf: $(OBJS) $(USER_OBJS)
	@echo 'Building target: $@'
	@echo 'Invoking: AVR C Linker'
	avr-gcc -Wl,-Map,USART_Demo.map -mmcu=attiny4313 -o "USART_Demo.elf" $(OBJS) $(USER_OBJS) $(LIBS)
	@echo 'Finished building target: $@'
	@echo ' '

USART_Demo.hex: USART_Demo.elf
	@echo 'Create Flash image (ihex format)'
	-avr-objcopy -R .eeprom -O ihex USART_Demo.elf  "USART_Demo.hex"
	@echo 'Finished building: $@'
	@echo ' '

sizedummy: USART_Demo.elf
	@echo 'Invoking: Print Size'
	-avr-size --format=avr --mcu=attiny4313 USART_Demo.elf
	@echo 'Finished building: $@'
	@echo ' '

avrdudedummy: USART_Demo.elf
	@echo 'Invoking: AVRDude'
	C:\WinAVR-20100110\bin\avrdude -pt4313 -cusbtiny -u -Uflash:w:USART_Demo.hex:a -Ulfuse:w:0xe4:m -Uhfuse:w:0xdf:m -Uefuse:w:0xff:m
	@echo 'Finished building: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(OBJS)$(C_DEPS)$(ASM_DEPS)$(FLASH_IMAGE)$(ELFS)$(AVRDUDEDUMMY)$(S_DEPS)$(SIZEDUMMY)$(S_UPPER_DEPS) USART_Demo.elf
	-@echo ' '

secondary-outputs: $(FLASH_IMAGE) $(SIZEDUMMY) $(AVRDUDEDUMMY)

.PHONY: all clean dependents
.SECONDARY:

-include ../makefile.targets
