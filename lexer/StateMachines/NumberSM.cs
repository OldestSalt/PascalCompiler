using System.Text;
using System.Globalization;

namespace PascalCompiler.Lexer {
    public class NumberSM: StateMachine {
        private states[] endStates = new states[] {
            states.WRONG_DIGIT,
            states.WRONG_FRACTIONAL_PART,
            states.HEX,
            states.OCT,
            states.BIN,
            states.BIN,
            states.DEC,
            states.REAL,
            states.POINT_IS_ELLIPSIS
        };

        private enum states {
            OTHER,
            START,
            EXPECT_HEX_DIGIT,
            EXPECT_OCT_DIGIT,
            EXPECT_BIN_DIGIT,
            REPEAT_HEX_DIGIT,
            REPEAT_OCT_DIGIT,
            REPEAT_BIN_DIGIT,
            EXPECT_DIGIT_OR_POINT_OR_E,
            EXPECT_E_OR_DIGIT,
            EXPECT_SIGN_OR_DIGIT,
            EXPECT_DIGIT_AFTER_SIGN,
            REPEAT_DIGIT_IN_FRACTIONAL_PART,
            WRONG_DIGIT,
            WRONG_FRACTIONAL_PART,
            HEX,
            OCT,
            BIN,
            DEC,
            REAL,
            POINT_IS_ELLIPSIS
        }

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
            rules = new Dictionary<int, Dictionary<HashSet<char>, int>> {
                { (int)states.START, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '$' }, (int)states.EXPECT_HEX_DIGIT },
                        { new HashSet<char> { '&' }, (int)states.EXPECT_OCT_DIGIT },
                        { new HashSet<char> { '%' }, (int)states.EXPECT_BIN_DIGIT },
                        { Constants.Digits, (int)states.EXPECT_DIGIT_OR_POINT_OR_E }
                    }
                },
                { (int)states.EXPECT_HEX_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.HexaDigits, (int)states.REPEAT_HEX_DIGIT }
                    }
                },
                { (int)states.EXPECT_OCT_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.OctalDigits, (int)states.REPEAT_OCT_DIGIT }
                    }
                },
                { (int)states.EXPECT_BIN_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.BinDigits, (int)states.REPEAT_BIN_DIGIT }
                    }
                },
                { (int)states.REPEAT_HEX_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.HexaDigits, (int)states.REPEAT_HEX_DIGIT }
                    }
                },
                { (int)states.REPEAT_OCT_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.OctalDigits, (int)states.REPEAT_OCT_DIGIT },
                        { Constants.NonOctalDigits, (int)states.WRONG_DIGIT }
                    }
                },
                { (int)states.REPEAT_BIN_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.BinDigits, (int)states.REPEAT_BIN_DIGIT },
                        { Constants.NonBinDigits, (int)states.WRONG_DIGIT }
                    }
                },
                { (int)states.EXPECT_DIGIT_OR_POINT_OR_E, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, (int)states.EXPECT_DIGIT_OR_POINT_OR_E },
                        { new HashSet<char> { '.' }, (int)states.EXPECT_E_OR_DIGIT },
                        { new HashSet<char> { 'e' }, (int)states.EXPECT_SIGN_OR_DIGIT }
                    }
                },
                { (int)states.EXPECT_E_OR_DIGIT, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, (int)states.EXPECT_E_OR_DIGIT },
                        { new HashSet<char> { 'e' }, (int)states.EXPECT_SIGN_OR_DIGIT }
                    }
                },
                { (int)states.EXPECT_SIGN_OR_DIGIT, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '+', '-' }, (int)states.EXPECT_DIGIT_AFTER_SIGN },
                        { Constants.Digits, (int)states.REPEAT_DIGIT_IN_FRACTIONAL_PART }
                    }
                },
                { (int)states.EXPECT_DIGIT_AFTER_SIGN, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, (int)states.REPEAT_DIGIT_IN_FRACTIONAL_PART }
                    }
                },
                { (int)states.REPEAT_DIGIT_IN_FRACTIONAL_PART, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, (int)states.REPEAT_DIGIT_IN_FRACTIONAL_PART }
                    }
                }
            };
        }

        public Lexeme GetNextLexeme(Lexer lexer) {
            var newLexeme = new Lexeme();
            states curState = states.START;
            states nextState;
            char peekedChar;
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder rawLexeme = new StringBuilder();

            while (!endStates.Contains(curState)) {
                peekedChar = Char.ToLower(streamHandler.Peek());
                nextState = (states)getNextState((int)curState, peekedChar);

                switch (curState) {
                    case states.START:
                        curState = nextState;
                        if (nextState == states.EXPECT_HEX_DIGIT || nextState == states.EXPECT_OCT_DIGIT || nextState == states.EXPECT_BIN_DIGIT) {
                            rawLexeme.Append(peekedChar);
                            streamHandler.GetChar();
                        }
                        break;
                    case states.EXPECT_HEX_DIGIT:
                    case states.EXPECT_OCT_DIGIT:
                    case states.EXPECT_BIN_DIGIT:
                        curState = nextState == states.OTHER ? states.WRONG_DIGIT : nextState;
                        break;
                    case states.REPEAT_HEX_DIGIT:
                        curState = nextState == states.OTHER ? states.HEX : nextState;
                        if (nextState == states.REPEAT_HEX_DIGIT) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        break;
                    case states.REPEAT_OCT_DIGIT:
                        curState = nextState == states.OTHER ? states.OCT : nextState;
                        if (nextState == states.REPEAT_OCT_DIGIT) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        break;
                    case states.REPEAT_BIN_DIGIT:
                        curState = nextState == states.OTHER ? states.BIN : nextState;
                        if (nextState == states.REPEAT_BIN_DIGIT) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        break;
                    case states.EXPECT_DIGIT_OR_POINT_OR_E:
                        curState = nextState == states.OTHER ? states.DEC : nextState;
                        if (nextState == states.EXPECT_E_OR_DIGIT && streamHandler.Peek2() == '.')
                            curState = states.POINT_IS_ELLIPSIS;
                        else if (nextState == states.EXPECT_DIGIT_OR_POINT_OR_E || nextState == states.EXPECT_E_OR_DIGIT || nextState == states.EXPECT_SIGN_OR_DIGIT) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        break;
                    case states.EXPECT_E_OR_DIGIT:
                        curState = nextState == states.OTHER ? states.REAL : nextState;
                        if (nextState == states.EXPECT_E_OR_DIGIT || nextState == states.EXPECT_SIGN_OR_DIGIT) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        break;
                    case states.EXPECT_SIGN_OR_DIGIT:
                        curState = nextState == states.OTHER ? states.WRONG_FRACTIONAL_PART : nextState;
                        if (nextState == states.EXPECT_DIGIT_AFTER_SIGN) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        else if (nextState == states.REPEAT_DIGIT_IN_FRACTIONAL_PART) stringBuilder.Append('+');
                        break;
                    case states.EXPECT_DIGIT_AFTER_SIGN:
                        curState = nextState == states.OTHER ? states.WRONG_FRACTIONAL_PART : nextState;
                        break;
                    case states.REPEAT_DIGIT_IN_FRACTIONAL_PART:
                        curState = nextState == states.OTHER ? states.REAL : nextState;
                        if (nextState == states.REPEAT_DIGIT_IN_FRACTIONAL_PART) {
                            rawLexeme.Append(peekedChar);
                            stringBuilder.Append(Char.ToLower(streamHandler.GetChar()));
                        }
                        break;
                }
            }

            string parsedNumberString = stringBuilder.ToString();
            newLexeme.value = parsedNumberString;
            newLexeme.raw = rawLexeme.ToString();

            switch (curState) {
                case states.WRONG_DIGIT:
                    ExceptionHandler.Throw(Exceptions.IntegerIncorrectFormat, streamHandler.lineNumber, streamHandler.charNumber + 1);
                    break;
                case states.WRONG_FRACTIONAL_PART:
                    ExceptionHandler.Throw(Exceptions.FloatIncorrectFormat, streamHandler.lineNumber, streamHandler.charNumber + 1);
                    break;
                case states.HEX:
                case states.OCT:
                case states.BIN:
                case states.DEC:
                case states.POINT_IS_ELLIPSIS:
                    newLexeme.type = Constants.LexemeType.INTEGER;
                    int radix = 10;
                    uint parsedInt;

                    switch (curState) {
                        case states.HEX:
                            radix = 16;
                            break;
                        case states.OCT:
                            radix = 8;
                            break;
                        case states.BIN:
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
                    break;
                case states.REAL:
                    newLexeme.type = Constants.LexemeType.REAL;
                    float parsedFloat;

                    try {
                        StringToFloat(parsedNumberString, out parsedFloat);
                        newLexeme.value = parsedFloat.ToString();
                    }
                    catch (OverflowException) {
                        ExceptionHandler.Throw(Exceptions.FloatOverflow, streamHandler.lineNumber, streamHandler.charNumber + 1);
                    }
                    break;
            }

            return newLexeme;
        }
    }
}
