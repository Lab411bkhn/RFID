#include "main.h"
#include "delay.h"
extern u8 flagu1;
extern u8 flagu2;
extern u8 flagu3;
char bufferu1[300] = {0};
char bufferu2[500] = {0};
char bufferu3[500] = {0};
extern u16 counteru1;
extern u16 counteru2;
extern u16 counteru3;
int main(void)
{
	u16 i;
	u16 j;
  USER_RCC_Init();
  USER_NVIC_Init();
  USER_UART_Init();
	USER_GPIO_Init();
	printf("ready");
	//Usart2_PutString("2");
	//Usart3_PutString("3");
	delay_ms(2000);
	while(1)
	{
		j++;
		j++;
		//printf("1");
		//Usart2_PutString("2");
		//Usart3_PutString("3");
		if(flagu1==1)
		{
			//bufferu1[counteru1]= '\0';
			if(bufferu1[0] == '2')
			{
				for(i=1;i<counteru1;i++)
				{
					Usart2_Put(bufferu1[i]);
				}
			}
			else if(bufferu1[0] == '3')
			{
				for(i=1;i<counteru1;i++)
				{
					Usart3_Put(bufferu1[i]);
				}
			}
			else
			{
				for(i=0;i<counteru1;i++)
				{
					Usart2_Put(bufferu1[i]);
					Usart3_Put(bufferu1[i]);
				}
			}
			
			//Usart3_PutString(bufferu1);
			memset(bufferu1,0,sizeof(bufferu1));
			flagu1=0;
			counteru1=0;
		}
		
		if(flagu2==1)
		{
			for(i=0;i<counteru2;i++)
			{
				Usart1_Put(bufferu2[i]);
				//Usart3_Put(bufferu2[i]);
			}
			memset(bufferu2,0,sizeof(bufferu2));
			flagu2=0;
			counteru2=0;
		}
		if(flagu3==1)
		{
			for(i=0;i<counteru3;i++)
			{
				Usart1_Put(bufferu3[i]);
				//Usart2_Put(bufferu3[i]);
			}
			memset(bufferu3,0,sizeof(bufferu3));
			flagu3=0;
			counteru3=0;
		}
	}
}
