using System.Text;

namespace PascalCompiler.Lexer {
    public class IdentifierReservedWordSM: StateMachine {
        public IdentifierReservedWordSM(StreamHandler sh): base(sh) {
            streamHandler = sh;
            rules = new Dictionary<int, Dictionary<char[], int>> {
                { 1, new Dictionary<char[], int> {
                        { Constants.Letters, 1 },
                        { Constants.Digits, 1 },
                        { new char[] {'_'}, 1 }
                    }
                }
            };
        }

        public Lexeme? GetNextLexeme() {
            var newLexeme = new Lexeme();
            var curState = 1;
            var foundString = new StringBuilder(Constants.MAX_IDENTIFIER_LENGTH);
            var rawLexeme = new StringBuilder();
            char peekedChar;

            while (true) {
                peekedChar = Char.ToLower(streamHandler.Peek());
                int nextState = getNextState(curState, peekedChar);

                if (nextState == 1) {
                    rawLexeme.Append(peekedChar);
                    if (foundString.Length < Constants.MAX_IDENTIFIER_LENGTH) foundString.Append(Char.ToLower(streamHandler.GetChar()));
                    else ExceptionHandler.Throw(Exceptions.LongIdentifierName, streamHandler.lineNumber, streamHandler.charNumber + 1);
                }
                else break;
            }

            newLexeme.raw = rawLexeme.ToString();
            newLexeme.value = newLexeme.raw;

            if (Constants.ReservedWords.Contains(foundString.ToString())) {
                if (Constants.Operators.Contains(foundString.ToString())) {
                    newLexeme.type = Constants.LexemeType.OPERATOR;
                }
                else {
                    newLexeme.type = Constants.LexemeType.RESERVED;
                }
            }
            else {
                newLexeme.type = Constants.LexemeType.IDENTIFIER;
            }

            return newLexeme;
        }
    }
}
