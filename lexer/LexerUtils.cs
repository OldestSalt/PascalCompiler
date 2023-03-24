namespace PascalCompiler.Lexer {
    public static class LexerUtils {
        private static Dictionary<string, CommonConstants.ServiceWords> ServiceWords = new Dictionary<string, CommonConstants.ServiceWords>() {
            { "array", CommonConstants.ServiceWords.ARRAY },
            { "begin", CommonConstants.ServiceWords.BEGIN },
            { "case", CommonConstants.ServiceWords.CASE },
            { "const", CommonConstants.ServiceWords.CONST },
            { "do", CommonConstants.ServiceWords.DO },
            { "downto", CommonConstants.ServiceWords.DOWNTO },
            { "else", CommonConstants.ServiceWords.ELSE },
            { "end", CommonConstants.ServiceWords.END },
            { "for", CommonConstants.ServiceWords.FOR },
            { "function", CommonConstants.ServiceWords.FUNCTION },
            { "if", CommonConstants.ServiceWords.IF },
            { "of", CommonConstants.ServiceWords.OF },
            { "procedure", CommonConstants.ServiceWords.PROCEDURE },
            { "program", CommonConstants.ServiceWords.PROGRAM },
            { "record", CommonConstants.ServiceWords.RECORD },
            { "repeat", CommonConstants.ServiceWords.REPEAT },
            { "then", CommonConstants.ServiceWords.THEN },
            { "to", CommonConstants.ServiceWords.TO },
            { "type", CommonConstants.ServiceWords.TYPE },
            { "until", CommonConstants.ServiceWords.UNTIL },
            { "var", CommonConstants.ServiceWords.VAR },
            { "while", CommonConstants.ServiceWords.WHILE },
            { "and", CommonConstants.ServiceWords.AND },
            { "or", CommonConstants.ServiceWords.OR },
            { "xor", CommonConstants.ServiceWords.XOR },
            { "not", CommonConstants.ServiceWords.NOT },
            { "div", CommonConstants.ServiceWords.DIV },
            { "mod", CommonConstants.ServiceWords.MOD },
            { "+", CommonConstants.ServiceWords.PLUS },
            { "-", CommonConstants.ServiceWords.MINUS },
            { "*", CommonConstants.ServiceWords.MULTIPLY },
            { "/", CommonConstants.ServiceWords.DIVIDE },
            { ":=", CommonConstants.ServiceWords.ASSIGN },
            { "=", CommonConstants.ServiceWords.EQUAL },
            { "<>", CommonConstants.ServiceWords.NOT_EQUAL },
            { "<", CommonConstants.ServiceWords.LESSER },
            { "<=", CommonConstants.ServiceWords.LESSER_OR_EQUAL },
            { ">=", CommonConstants.ServiceWords.GREATER_OR_EQUAL },
            { ">", CommonConstants.ServiceWords.GREATER },
            { "..", CommonConstants.ServiceWords.ELLIPSIS },
            { ":", CommonConstants.ServiceWords.COLON },
            { ".", CommonConstants.ServiceWords.POINT },
            { ",", CommonConstants.ServiceWords.COMMA },
            { ";", CommonConstants.ServiceWords.SEMICOLON },
            { "(", CommonConstants.ServiceWords.LEFT_ROUND_BRACKET },
            { ")", CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET },
            { "[", CommonConstants.ServiceWords.LEFT_SQUARE_BRACKET },
            { "]", CommonConstants.ServiceWords.RIGHT_SQUARE_BRACKET },
            { "out", CommonConstants.ServiceWords.OUT }
        };

        public static CommonConstants.ServiceWords GetEnumValue(string? value) {
            CommonConstants.ServiceWords word;
            if (value != null && ServiceWords.TryGetValue(value, out word)) {
                return word;
            }
            else {
                return CommonConstants.ServiceWords.NONE;
            }
        }
    }
}
