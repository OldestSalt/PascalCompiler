namespace PascalCompiler.Lexer {
    public class Lexer {
        private readonly StreamHandler streamHandler;
        private readonly IdentifierReservedWordSM identifierReservedWordSM;
        private readonly StringSM stringSM;
        private readonly OperatorSeparatorSM operatorSeparatorSM;
        private readonly NumberSM numberSM;
        public Lexeme curLexeme;

        public Lexer(string fileName) {
            streamHandler = new StreamHandler(fileName);
            identifierReservedWordSM = new IdentifierReservedWordSM(streamHandler);
            stringSM = new StringSM(streamHandler);
            operatorSeparatorSM = new OperatorSeparatorSM(streamHandler);
            numberSM = new NumberSM(streamHandler);
            curLexeme = new Lexeme();
        }

        private bool IsEOF() {
            streamHandler.SkipWhiteSpacesAndComments();
            if (streamHandler.Peek() == '\0') {
                return true;
            }
            return false;
        }

        public Lexeme GetNextLexeme() {
            Lexeme nextLexeme = new Lexeme();
            streamHandler.SkipWhiteSpacesAndComments();
            char nextChar = streamHandler.Peek();
            uint lineNumber = streamHandler.lineNumber;
            uint charNumber = streamHandler.charNumber;
            
            if (!IsEOF()) {
                if (nextChar >= 'A' && nextChar <= 'Z' || nextChar >= 'a' && nextChar <= 'z') {
                    nextLexeme = identifierReservedWordSM.GetNextLexeme();
                }
                else if (Char.IsDigit(nextChar) || Constants.ModifierChars.Contains(nextChar)) {
                    nextLexeme = numberSM.GetNextLexeme(this);
                }
                else if (nextChar == '\'' || nextChar == '#') {
                    nextLexeme = stringSM.GetNextLexeme(this);
                }
                else if (Constants.SpecialChars.Contains(nextChar)) {
                    nextLexeme = operatorSeparatorSM.GetNextLexeme();
                }
                else {
                    ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lineNumber, charNumber + 1, nextChar.ToString());
                }
            }
            else {
                nextLexeme.type = Constants.LexemeType.EOF;
            }

            nextLexeme!.subtype = LexerUtils.GetEnumValue(nextLexeme.value);
            nextLexeme!.lineNumber = lineNumber;
            nextLexeme!.charNumber = charNumber + 1;

            curLexeme = nextLexeme;
            
            return nextLexeme;
        }
    }
}
