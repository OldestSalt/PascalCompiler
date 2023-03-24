using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace PascalCompiler.Lexer {
    public class StringSM: StateMachine {
        private readonly int[] endStates = new int[2] { 6, 7 };
        public StringSM(StreamHandler sh) : base(sh) {
            rules = new Dictionary<int, Dictionary<HashSet<char>, int>> {
                { 1, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '\'' }, 2 },
                        { new HashSet<char> { '#' }, 4 }
                    }
                },
                { 2, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '\n', '\r', '\0' }, 6 },
                        { new HashSet<char> { '\'' }, 3 }
                    }
                },
                { 3, new Dictionary<HashSet<char>, int> {
                        { new HashSet<char> { '\'' }, 2 },
                        { new HashSet<char> { '#' }, 4 }
                    }
                },
                { 4, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, 5 },
                        { Constants.ModifierChars, 5 }
                    }
                },
                { 5, new Dictionary<HashSet<char>, int> {
                        { Constants.Digits, 5 },
                        { new HashSet<char> { '#' }, 4 },
                        { new HashSet<char> { '\'' }, 2 }
                    }
                }
            };
        }

        public Lexeme GetNextLexeme(Lexer lexer) {
            var newLexeme = new Lexeme();
            var curState = 1;
            var foundString = new StringBuilder();
            var rawLexeme = new StringBuilder();
            char peekedChar;

            while (!endStates.Contains(curState)) {
                peekedChar = streamHandler.Peek();
                int nextState = getNextState(curState, peekedChar);

                if (curState == 1) {
                    curState = nextState;
                    rawLexeme.Append(peekedChar);
                    streamHandler.GetChar();
                }
                else if (curState == 2) {
                    curState = nextState == 0 ? curState : nextState;

                    if (nextState == 0) {
                        rawLexeme.Append(peekedChar);
                        foundString.Append(streamHandler.GetChar());
                    }
                    else if (nextState == 6 || nextState == 3) {
                        rawLexeme.Append(peekedChar);
                        streamHandler.GetChar();
                    }
                }
                else if (curState == 3) {
                    curState = nextState == 0 ? 7 : nextState;

                    if (nextState == 2)  {
                        rawLexeme.Append(peekedChar);
                        foundString.Append(streamHandler.GetChar());
                    }
                    else if (nextState == 4) {
                        rawLexeme.Append(peekedChar);
                        streamHandler.GetChar();
                    }
                }
                else if (curState == 4) {
                    curState = nextState == 0 ? 6 : nextState;
                }
                else if (curState == 5) {
                    NumberSM numberSM = new NumberSM(streamHandler);
                    Lexeme? charCodeLexeme = numberSM.GetNextLexeme(lexer);

                    if (charCodeLexeme == null || charCodeLexeme.value == null || charCodeLexeme.type != Constants.LexemeType.INTEGER || uint.Parse(charCodeLexeme.value) > Constants.MAX_CHARACTER_NUMBER) {
                        curState = 6;
                        break;
                    }

                    foundString.Append((char)uint.Parse(charCodeLexeme.value));
                    rawLexeme.Append(charCodeLexeme.raw);

                    peekedChar = streamHandler.Peek();
                    nextState = getNextState(curState, peekedChar);
                    curState = nextState == 0 ? 7 : nextState;
                    if (nextState == 4 || nextState == 2) {
                        rawLexeme.Append(peekedChar);
                        streamHandler.GetChar();
                    }
                }
            }

            if (curState == 6) {
                ExceptionHandler.Throw(Exceptions.IncorrectStringFormat, streamHandler.lineNumber, streamHandler.charNumber + 1);
            }
            if (curState == 7) {
                newLexeme.type = Constants.LexemeType.STRING;
                newLexeme.value = foundString.ToString();
                newLexeme.raw = rawLexeme.ToString();
            }
            return newLexeme;
        }
    }
}
