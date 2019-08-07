#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <limits.h>

#define ROULETTE_SIZE 36

int main(int argc, char** argv)
{
    int seed = 0;
    printf("Enter seed: ");
    scanf("%d", &seed);

    srand(seed);
    for (int i = 0; i < 16; i++)
    {
        printf("%d, ", (rand() % ROULETTE_SIZE)+1);
        rand();
    }
}