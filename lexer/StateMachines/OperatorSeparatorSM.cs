using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Lexer {
    public class OperatorSeparatorSM: StateMachine {
        private readonly int[] endStates = new int[2] {6, 7};

        public OperatorSeparatorSM(StreamHandler sh): base(sh) {
            rules = new Dictionary<int, Dictionary<HashSet<char>, int>>{
                { 1, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char>{ '<' }, 2 },
                        { new HashSet<char> { '>' }, 3 },
                        { new HashSet<char> { ':' }, 4 },
                        { new HashSet<char> { '.' }, 5 },
                        { new HashSet<char> { '+', '-', '*', '/', '=' }, 6 },
                        { new HashSet<char> { ',', ';', '(', ')', '[', ']' } , 7 }
                    }
                },
                { 2, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '>', '=' }, 6 }
                    }
                },
                { 3, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '=' }, 6 }
                    }
                },
                { 4, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '=' }, 6 }
                    }
                },
                { 5, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '.' }, 7 }
                    }
                }
            };
        }

        public Lexeme GetNextLexeme() {
            Lexeme newLexeme = new Lexeme();
            int curState = 1;
            StringBuilder rawLexeme = new StringBuilder();
            char peekedChar;

            while (!endStates.Contains(curState)) {
                peekedChar = streamHandler.Peek();
                int nextState = getNextState(curState, peekedChar);
                if (nextState != 0) {
                    curState = nextState;
                    rawLexeme.Append(peekedChar);
                    streamHandler.GetChar();
                }
                else {
                    curState = curState == 4 || curState == 5 ? 7 : 6;
                }
            }

            newLexeme.raw = rawLexeme.ToString();
            newLexeme.value = newLexeme.raw;
            newLexeme.type = curState == 6 ? Constants.LexemeType.OPERATOR : Constants.LexemeType.SEPARATOR;
            return newLexeme;
        }
    }
}