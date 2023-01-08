﻿namespace PascalCompiler.Lexer {
    public class Lexer {
        private readonly StreamHandler streamHandler;
        private readonly IdentifierReservedWordSM identifierReservedWordSM;
        private readonly StringSM stringSM;
        private readonly OperatorSeparatorSM operatorSeparatorSM;
        private readonly NumberSM numberSM;
        public static Lexeme? prevLexeme = null;

        public Lexer(string fileName) {
            streamHandler = new StreamHandler(fileName);
            identifierReservedWordSM = new IdentifierReservedWordSM(streamHandler);
            stringSM = new StringSM(streamHandler);
            operatorSeparatorSM = new OperatorSeparatorSM(streamHandler);
            numberSM = new NumberSM(streamHandler);
        }

        private bool IsEOF() {
            streamHandler.SkipWhiteSpacesAndComments();
            if (streamHandler.Peek() == '\0') {
                return true;
            }
            return false;
        }

        public Lexeme? GetNextLexeme() {
            Lexeme? nextLexeme = null;
            streamHandler.SkipWhiteSpacesAndComments();
            char nextChar = streamHandler.Peek();
            uint lineNumber = streamHandler.lineNumber;
            uint charNumber = streamHandler.charNumber;
            
            if (!IsEOF()) {
                if (Char.IsLetter(nextChar)) {
                    nextLexeme = identifierReservedWordSM.GetNextLexeme();
                }
                else if (Char.IsDigit(nextChar) || Constants.ModifierChars.Contains(nextChar)) {
                    nextLexeme = numberSM.GetNextLexeme();
                }
                else if (nextChar == '\'' || nextChar == '#') {
                    nextLexeme = stringSM.GetNextLexeme();
                }
                else {
                    nextLexeme = operatorSeparatorSM.GetNextLexeme();
                }
            }
            else {
                nextLexeme = new Lexeme();
                nextLexeme.type = Constants.LexemeType.EOF;
            }

            nextLexeme!.lineNumber = lineNumber;
            nextLexeme!.charNumber = charNumber + 1;

            prevLexeme = nextLexeme;
            
            return nextLexeme;
        }
    }
}