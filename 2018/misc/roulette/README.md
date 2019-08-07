## Description
This is a roulette game that we are supposed to win. Looking at the source, we have to win more than 1 billion AND have more than 3 wins. If we get 1 billion before winning 3 times, then we don't get the flag.

The hints mention 2 bugs which we can find. The first place to look would be at how the game generates the numbers since we need to be able to correctly guess the winning numbers.
This is the line that generates the number: 
```c
long spin = (rand() % ROULETTE_SIZE)+1;
```

`rand()` needs to be seeded otherwise it will generate the same numbers each time the program is run and that is not the case as we can test. So now we have to look for a `srand()` call and we find it in this function.
```c
long get_rand() {
  long seed;
  FILE *f = fopen("/dev/urandom", "r");
  fread(&seed, sizeof(seed), 1, f);
  fclose(f);
  seed = seed % 5000;
  if (seed < 0) seed = seed * -1;
  srand(seed);
  return seed;
}
```

We see that it calls `srand` with our seed but it also returns the value which is interesting. So next is to take a look at what is using the return value.
```c
int main(int argc, char *argv[]) {
    setvbuf(stdout, NULL, _IONBF, 0);

    cash = get_rand();

    // More excluded
}
```

We can see that our starting cash is actually the seed used, and our cash is known so now we can predict the sequence of roulette numbers. We just have to call `srand()` with our cash and then call `rand` a couple of times to get the numbers. Note that after winning, a random message is printed which also calls `rand` so make sure that when generating the numbers, you use every other number.

Now that we have the sequence we can play and win everytime. But if you try it, you'll soon run into an issue. In the main game loop, there is this check
```c
 if (wins >= MAX_WINS) {
    printf("Wow you won %lu times? Looks like its time for you cash you out.\n", wins);
    printf("Congrats you made $%lu. See you next time!\n", cash);
    exit(-1);
}
```

Given `MAX_WINS` equals to 16, and our starting cash < `5000`, we can see that at best, `5000 * 2 ^ 16`, we can only get up to `327,680,000` in cash. So now we can assume that we need to look for the other bug and we can also assume that it involves calculating the bet. The bet is given through
```c
long get_bet() {
  while(1) {
    puts("How much will you wager?");
    printf("Current Balance: $%lu \t Current Wins: %lu\n", cash, wins); 
    long bet = get_long(); 
    if(bet <= cash) {
      return bet;
    } else {
      puts("You can't bet more than you have!");
    }
  }
}
```

The first thing we can notice is the check `bet <= cash`. In the main game loop there is this calculation `cash -= bet`. So if we can get a negative bet, then we'll be able wager more than our cash. The bet itself involves this function.
```c
long get_long() {
    printf("> ");
    uint64_t l = 0;
    char c = 0;
    while(!is_digit(c))
      c = getchar();
    while(is_digit(c)) {
      if(l >= LONG_MAX) {
		l = LONG_MAX;
		break;
      }
      l *= 10;
      l += c - '0';
      c = getchar();
    }
    while(c != '\n')
      c = getchar();
    return l;
}
```

It seems normal at first: strip all non digits, then calculate the value by looping through each digit and multiplying by the corresponding power. But if we look closely, we can see that there is a bug in the check for `>= LONG_MAX`. Since the check is __before__ the multiplication of the number, imagine if the current value of `l` was between `LONG_MAX / 10` and `LONG_MAX`. Then we get past the check, and `l` is multiplied by `10` and we effectively have a number between `LONG_MAX` and `10 * LONG_MAX`. This means we are able to overflow `l` and get a negative number. Also important to note that in this case, it is a 32 bit number `(2147483647)`.

Now to combine both bugs, we need to win 3 times, then make a bet with a negative value, that gives us at least 1 billion cash. Note that we don't have to win the last bet since the negative bet is being subtracted from our cash anyways.