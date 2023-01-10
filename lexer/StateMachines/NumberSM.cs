using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PascalCompiler.Lexer {
    public class NumberSM: StateMachine {

        public void StringToUInt(string input, int radix, out uint result) {
            uint parsedInt = Convert.ToUInt32(input, radix);
            if (parsedInt > Constants.MAX_UNSIGNED_INT) throw new OverflowException();
            result = parsedInt;
        }
        public void StringToFloat(string input, out float result) {
            float parsedFloat = Convert.ToSingle(input, CultureInfo.InvariantCulture);
            if (float.IsInfinity(parsedFloat) || float.IsNaN(parsedFloat) || parsedFloat < 1.175495E-38) throw new OverflowException();
            result = parsedFloat;
        }

        public NumberSM(StreamHandler sh): base(sh) {
            streamHandler = sh;
            rules = new Dictionary<int, Dictionary<char[], int>> {
                { 1, new Dictionary<char[], int> {
                        { new char[] { '$' }, 2 },
                        { new char[] { '&' }, 3 },
                        { new char[] { '%' }, 4 },
                        { Constants.Digits, 8 }
                    }
                },
                { 2, new Dictionary<char[], int> {
                        { Constants.HexaDigits, 5 }
                    }
                },
                { 3, new Dictionary<char[], int> {
                        { Constants.OctalDigits, 6 }
                    }
                },
                { 4, new Dictionary<char[], int> {
                        { Constants.BinDigits, 7 }
                    }
                },
                { 5, new Dictionary<char[], int> {
                        { Constants.HexaDigits, 5 }
                    }
                },
                { 6, new Dictionary<char[], int> {
                        { Constants.OctalDigits, 6 },
                        { Constants.NonOctalDigits, 13 }
                    }
                },
                { 7, new Dictionary<char[], int> {
                        { Constants.BinDigits, 7 },
                        { Constants.NonBinDigits, 13 }
                    }
                },
                { 8, new Dictionary<char[], int> {
                        { Constants.Digits, 8 },
                        { new char[] { '.' }, 9 },
                        { new char[] { 'e' }, 10 }
                    }
                },
                { 9, new Dictionary<char[], int> {
                        { Constants.Digits, 9 },
                        { new char[] { 'e' }, 10 }
                    }
                },
                { 10, new Dictionary<char[], int> {
                        { new char[] { '+', '-' }, 11 },
                        { Constants.Digits, 12 }
                    }
                },
                { 11, new Dictionary<char[], int> {
                        { Constants.Digits, 12 }
                    }
                },
                { 12, new Dictionary<char[], int> {
                        { Constants.Digits, 12 }
                    }
                }
            };
        }

        public Lexeme? GetNextLexeme(Lexer lexer) {
            var newLexeme = new Lexeme();
            var curState = 1;
            int nextState;
            char peekedChar;
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder rawLexeme = new StringBuilder();

            while (curState < 13) {
                peekedChar = Char.ToLower(streamHandler.Peek());
                nextState = getNextState(curState, peekedChar);
                
                if (curState == 1) {
                    curState = nextState;
                    if (nextState >= 2 && nextState <= 4) {
                        rawLexeme.Append(peekedChar);
                        streamHandler.GetChar();
                    }
                }
                else if (curState >= 2 && curState <= 4) curState = nextState == 0 ? 13 : nextState;
                else if (curState == 5) {
                    curState = nextState == 0 ? 15 : nextState;
                    if (nextState == 5) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                }
                else if (curState == 6) {
                    curState = nextState == 0 ? 16 : nextState;
                    if (nextState == 6) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                }
                else if (curState == 7) {
                    curState = nextState == 0 ? 17 : nextState;
                    if (nextState == 7) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                }
                else if (curState == 8) {
                    curState = nextState == 0 ? 18 : nextState;
                    if (nextState == 9 && streamHandler.Peek2() == '.') curState = 20;
                    else if (nextState >= 8 && nextState <= 10) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                }
                else if (curState == 9) {
                    curState = nextState == 0 ? 19 : nextState;
                    if (nextState >= 9 && nextState <= 10) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                }
                else if (curState == 10) {
                    curState = nextState == 0 ? 14 : nextState;
                    if (nextState == 11) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                    else if (nextState == 12) stringBuilder.Append('+');
                }
                else if (curState == 11) curState = nextState == 0 ? 14 : nextState;
                else if (curState == 12) {
                    curState = nextState == 0 ? 19 : nextState;
                    if (nextState == 12) {
                        rawLexeme.Append(peekedChar);
                        stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                    }
                }
            }

            string parsedNumberString = stringBuilder.ToString();
            newLexeme.value = parsedNumberString;
            newLexeme.raw = rawLexeme.ToString();

            if (curState == 13) {
                ExceptionHandler.Throw(Exceptions.IntegerIncorrectFormat, streamHandler.lineNumber, streamHandler.charNumber + 1);
            }
            else if (curState == 14) {
                ExceptionHandler.Throw(Exceptions.FloatIncorrectFormat, streamHandler.lineNumber, streamHandler.charNumber + 1);
            }
            else if (curState >= 15 && curState <= 18 || curState == 20) {
                newLexeme.type = Constants.LexemeType.INTEGER;
                int radix = 10;
                uint parsedInt;
                
                switch (curState) {
                    case 15: 
                        radix = 16;
                        break;
                    case 16: 
                        radix = 8;
                        break;
                    case 17: 
                        radix = 2;
                        break;
                }
                try {
                    StringToUInt(parsedNumberString, radix, out parsedInt);
                    newLexeme.value = parsedInt.ToString();
                }
                catch (OverflowException) {
                    if (lexer.curLexeme != null && lexer.curLexeme.value == "-" && (parsedNumberString == "2147483648" || parsedNumberString == "20000000000" || parsedNumberString == "10000000000000000000000000000000" || parsedNumberString == "80000000")) {
                        newLexeme.value = "2147483648";
                    }
                    else {
                        ExceptionHandler.Throw(Exceptions.IntegerOverflow, streamHandler.lineNumber, streamHandler.charNumber + 1);
                    }
                }
            }
            else if (curState == 19) {
                newLexeme.type = Constants.LexemeType.REAL;
                float parsedFloat;

                try {
                    StringToFloat(parsedNumberString, out parsedFloat);
                    newLexeme.value = parsedFloat.ToString();
                }
                catch (OverflowException) {
                    ExceptionHandler.Throw(Exceptions.FloatOverflow, streamHandler.lineNumber, streamHandler.charNumber + 1);
                }
            }

            return newLexeme;
        }
    }
}
