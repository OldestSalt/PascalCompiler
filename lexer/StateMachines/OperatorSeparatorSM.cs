using System.Text;

namespace PascalCompiler.Lexer {
    public class OperatorSeparatorSM: StateMachine {
        private readonly states[] endStates = new states[2] {states.OPERATOR, states.SEPARATOR};
        private enum states {
            OTHER,
            START,
            EXPECT_EQUAL_OR_MORE,
            EXPECT_EQUAL,
            EXPECT_ASSIGN,
            EXPECT_POINT,
            OPERATOR,
            SEPARATOR
        }

        public OperatorSeparatorSM(StreamHandler sh): base(sh) {
            rules = new Dictionary<int, Dictionary<HashSet<char>, int>>{
                { (int)states.START, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char>{ '<' }, (int)states.EXPECT_EQUAL_OR_MORE },
                        { new HashSet<char> { '>' }, (int)states.EXPECT_EQUAL },
                        { new HashSet<char> { ':' }, (int)states.EXPECT_ASSIGN },
                        { new HashSet<char> { '.' }, (int)states.EXPECT_POINT },
                        { new HashSet<char> { '+', '-', '*', '/', '=' }, (int)states.OPERATOR },
                        { new HashSet<char> { ',', ';', '(', ')', '[', ']' } , (int)states.SEPARATOR }
                    }
                },
                { (int)states.EXPECT_EQUAL_OR_MORE, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '>', '=' }, 6 }
                    }
                },
                { (int)states.EXPECT_EQUAL, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '=' }, 6 }
                    }
                },
                { (int)states.EXPECT_ASSIGN, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '=' }, 6 }
                    }
                },
                { (int)states.EXPECT_POINT, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '.' }, 7 }
                    }
                }
            };
        }

        public Lexeme GetNextLexeme() {
            Lexeme newLexeme = new Lexeme();
            states curState = states.START;
            StringBuilder rawLexeme = new StringBuilder();
            char peekedChar;

            while (!endStates.Contains(curState)) {
                peekedChar = streamHandler.Peek();
                states nextState = (states)getNextState((int)curState, peekedChar);
                if (nextState != states.OTHER) {
                    curState = nextState;
                    rawLexeme.Append(peekedChar);
                    streamHandler.GetChar();
                }
                else {
                    curState = curState == states.EXPECT_ASSIGN || curState == states.EXPECT_POINT ? states.SEPARATOR : states.OPERATOR;
                }
            }

            newLexeme.raw = rawLexeme.ToString();
            newLexeme.value = newLexeme.raw;
            newLexeme.type = curState == states.OPERATOR ? Constants.LexemeType.OPERATOR : Constants.LexemeType.SEPARATOR;
            return newLexeme;
        }
    }
}