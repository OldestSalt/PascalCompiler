namespace PascalCompiler.Parser {
    public static class ParserUtils {
        public static void RequireLexeme(Lexer.Lexer lexer, string expectedLexeme) {
            if (lexer.curLexeme!.value != expectedLexeme) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, expectedLexeme);
            }
            lexer.GetNextLexeme();
        }
    }
}
