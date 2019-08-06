using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace script_me
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "test")
            {
                Tests();
                return;
            }

            Console.WriteLine(ParseString(args[0]));
        }

        static IEnumerable<Token> ExtractTokens(string s)
        {
            return s.Split('+').Select(x => x.Trim()).Select(x => new Token(x));
        }

        static Token ParseString(string s)
        {
            return ExtractTokens(s).Aggregate((t1, t2) => t1 + t2);
        }

        static void Tests()
        {
            Assert.Equal("()()", ParseString("() + ()").Value);

            Assert.Equal("((())())", ParseString("((())) + ()").Value);
            Assert.Equal("(()(()))", ParseString("() + ((()))").Value);

            Assert.Equal("(())(()())", ParseString("(())(()) + ()").Value);
            Assert.Equal("(()())(())", ParseString("() + (())(())").Value);

            Assert.Equal("((())(())(()))", ParseString("(())(()) + ((()))").Value);
            Assert.Equal("((())(())(()))", ParseString("((())) + (())(())").Value);

            Assert.Equal("((()())(()))", ParseString("() + (()) + ((()))").Value);

            Assert.Equal("(()())", ParseString("() + (())").Value);
            Assert.Equal("(()()()(())())", ParseString("()() + (()(())) + ()").Value);
            Assert.Equal("(()()())(()()())(()()())", ParseString("()() + (()) + (()) + ()() + (()()())").Value);
            Assert.Equal("()()()()", ParseString("()()() + ()").Value);
            Assert.Equal("((()())(())())", ParseString("(()()) + ((())())").Value);
            Assert.Equal("(()()(())())", ParseString("() + () + ((())())").Value);
            Assert.Equal("(()(((()()())()())()()))(()(((()()())()())()())()()()()()(()))", ParseString("(()(((()()())()())()())) + (()(((()()())()())()())) + ()()() + ()() + (())").Value);
        }

        class Token
        {
            public Token(string tokenString)
            {
                Value = tokenString;
                ParseTokenString(tokenString);
            }

            public String Value { get; set; }
            public int MaxDepth { get; set;}

            private void ParseTokenString(string tokenString)
            {
                var brackets = new Stack<object>();

                foreach (var c in tokenString)
                {
                    switch (c)
                    {
                    case '(':
                        brackets.Push(null);
                        if (brackets.Count > MaxDepth)
                        {
                            MaxDepth = brackets.Count;
                        }
                        break;
                    case ')':
                        brackets.Pop();
                        break;
                    }
                }
            }

            public override string ToString()
            {
                return Value;
            }

            public static Token operator +(Token left, Token right)
            {
                if (left.MaxDepth > right.MaxDepth)
                {
                    return AbsorbRight(left, right);
                }
                else if (left.MaxDepth < right.MaxDepth)
                {
                    return AbsorbLeft(left, right);
                }
                else
                {
                    return Combine(left, right);
                }
            }

            public static Token Combine(Token first, Token second)
            {
                return new Token(first.Value + second.Value);
            }

            public static Token AbsorbRight(Token left, Token right)
            {
                var newValue = left.Value.Insert(left.Value.Length - 1, right.Value);
                return new Token(newValue);
            }

            public static Token AbsorbLeft(Token left, Token right)
            {
                var newValue = right.Value.Insert(1, left.Value);
                return new Token(newValue);
            }
        }
    }
}
