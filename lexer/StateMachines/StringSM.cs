using System.Text;

namespace PascalCompiler.Lexer {
    public class StringSM: StateMachine {
        private enum states {
            OTHER,
            START,
            EXPECT_CHAR,
            EXPECT_SHARP_OR_QUOTE,
            EXPECT_INT,
            INT,
            EXCEPTION,
            END
        }
        private readonly states[] endStates = new states[2] { states.EXCEPTION, states.END };
        public StringSM(StreamHandler sh) : base(sh) {
            rules = new Dictionary<int, Dictionary<HashSet<char>, int>> {
                { (int)states.START, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '\'' }, (int)states.EXPECT_CHAR },
                        { new HashSet<char> { '#' }, (int)states.EXPECT_INT}
                    }
                },
                { (int)states.EXPECT_CHAR, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '\n', '\r', '\0' }, (int)states.EXCEPTION },
                        { new HashSet<char> { '\'' }, (int)states.EXPECT_SHARP_OR_QUOTE }
                    }
                },
                { (int)states.EXPECT_SHARP_OR_QUOTE, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '\'' }, (int)states.EXPECT_CHAR },
                        { new HashSet<char> { '#' }, (int)states.EXPECT_INT }
                    }
                },
                { (int)states.EXPECT_INT, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, (int)states.INT },
                        { Constants.ModifierChars, (int)states.INT }
                    }
                },
                { (int)states.INT, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, (int)states.INT },
                        { new HashSet<char> { '#' }, (int)states.EXPECT_INT },
                        { new HashSet<char> { '\'' }, (int)states.EXPECT_CHAR }
                    }
                }
            };
        }

        public Lexeme GetNextLexeme(Lexer lexer) {
            var newLexeme = new Lexeme();
            states curState = states.START;
            var foundString = new StringBuilder();
            var rawLexeme = new StringBuilder();
            char peekedChar;

            while (!endStates.Contains(curState)) {
                peekedChar = streamHandler.Peek();
                states nextState = (states)getNextState((int)curState, peekedChar);
                switch (curState) {
                    case states.START:
                        curState = nextState;
                        rawLexeme.Append(peekedChar);
                        streamHandler.GetChar();
                        break;
                    case states.EXPECT_CHAR:
                        curState = nextState == states.OTHER ? curState : nextState;

                        switch (nextState) {
                            case states.OTHER:
                                rawLexeme.Append(peekedChar);
                                foundString.Append(streamHandler.GetChar());
                                break;
                            case states.EXCEPTION:
                            case states.EXPECT_SHARP_OR_QUOTE:
                                rawLexeme.Append(peekedChar);
                                streamHandler.GetChar();
                                break;
                        }
                        break;
                    case states.EXPECT_SHARP_OR_QUOTE:
                        curState = nextState == states.OTHER ? states.END : nextState;

                        switch (nextState) {
                            case states.EXPECT_CHAR:
                                rawLexeme.Append(peekedChar);
                                foundString.Append(streamHandler.GetChar());
                                break;
                            case states.EXPECT_INT:
                                rawLexeme.Append(peekedChar);
                                streamHandler.GetChar();
                                break;
                        }
                        break;
                    case states.EXPECT_INT:
                        curState = nextState == states.OTHER ? states.EXCEPTION : nextState;
                        break;
                    case states.INT:
                        NumberSM numberSM = new NumberSM(streamHandler);
                        Lexeme? charCodeLexeme = numberSM.GetNextLexeme(lexer);

                        if (charCodeLexeme == null ||
                            charCodeLexeme.value == null ||
                            charCodeLexeme.type != Constants.LexemeType.INTEGER ||
                            uint.Parse(charCodeLexeme.value) > Constants.MAX_CHARACTER_NUMBER) {
                            curState = states.EXCEPTION;
                            break;
                        }

                        foundString.Append((char)uint.Parse(charCodeLexeme.value));
                        rawLexeme.Append(charCodeLexeme.raw);

                        peekedChar = streamHandler.Peek();
                        nextState = (states)getNextState((int)curState, peekedChar);
                        curState = nextState == states.OTHER ? states.END : nextState;
                        if (nextState == states.EXPECT_INT || nextState == states.EXPECT_CHAR) {
                            rawLexeme.Append(peekedChar);
                            streamHandler.GetChar();
                        }
                        break;
                }
            }

            if (curState == states.EXCEPTION) {
                ExceptionHandler.Throw(Exceptions.IncorrectStringFormat, streamHandler.lineNumber, streamHandler.charNumber + 1);
            }
            if (curState == states.END) {
                newLexeme.type = Constants.LexemeType.STRING;
                newLexeme.value = foundString.ToString();
                newLexeme.raw = rawLexeme.ToString();
            }
            return newLexeme;
        }
    }
}
