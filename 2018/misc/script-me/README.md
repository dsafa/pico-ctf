## Description
After connecting to the server, we are presented with a challenge. Each next question gets longer and longer.
```
Rules:
() + () = ()()                                      => [combine]
((())) + () = ((())())                              => [absorb-right]
() + ((())) = (()(()))                              => [absorb-left]
(())(()) + () = (())(()())                          => [combined-absorb-right]
() + (())(()) = (()())(())                          => [combined-absorb-left]
(())(()) + ((())) = ((())(())(()))                  => [absorb-combined-right]
((())) + (())(()) = ((())(())(()))                  => [absorb-combined-left]
() + (()) + ((())) = (()()) + ((())) = ((()())(())) => [left-associative]

Example:
(()) + () = () + (()) = (()())
```

The formula works by checking the max depth of each operand, and either
1. Absorb left if left.maxdepth < right.maxdepth
2. Absorb right if right.maxdepth > left.maxdepth
3. Combine if both are the same max depth.

C# solution provided in `Program.cs`. I did not bother to automate the connection to the server and instead just copied and pasted the questions and answers.