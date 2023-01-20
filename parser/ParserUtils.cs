namespace PascalCompiler.Parser {
    public static class ParserUtils {
        public static void RequireLexeme(Lexer.Lexer lexer, CommonConstants.ServiceWords expectedLexeme) {
            if (lexer.curLexeme!.subtype != expectedLexeme) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, CommonConstants.ServiceWordsDict[expectedLexeme]);
            }
            lexer.GetNextLexeme();
        }
    }
}
