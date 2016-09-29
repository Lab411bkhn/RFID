#include "stm32f10x_rcc.h"
#include "stm32f10x_gpio.h"
#include "stm32f10x_usart.h"
#include "stm32f10x.h"
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
void USER_GPIO_Init(void);
void USER_RCC_Init(void);
void USER_NVIC_Init(void);
void USER_UART_Init(void);
void Usart2_PutString(char *c);
void Usart3_PutString(char *c);
void Usart1_Put(char c);
void Usart2_Put(char c);
void Usart3_Put(char c);
