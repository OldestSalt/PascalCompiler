namespace PascalCompiler.Lexer {
    public static class LexerUtils {
        public static CommonConstants.ServiceWords GetEnumValue(string? value) {
            switch (value) {
                case "array": return CommonConstants.ServiceWords.ARRAY;
                case "begin": return CommonConstants.ServiceWords.BEGIN;
                case "case": return CommonConstants.ServiceWords.CASE;
                case "const": return CommonConstants.ServiceWords.CONST;
                case "do": return CommonConstants.ServiceWords.DO;
                case "downto": return CommonConstants.ServiceWords.DOWNTO;
                case "else": return CommonConstants.ServiceWords.ELSE;
                case "end": return CommonConstants.ServiceWords.END;
                case "for": return CommonConstants.ServiceWords.FOR;
                case "function": return CommonConstants.ServiceWords.FUNCTION;
                case "if": return CommonConstants.ServiceWords.IF;
                case "of": return CommonConstants.ServiceWords.OF;
                case "procedure": return CommonConstants.ServiceWords.PROCEDURE;
                case "program": return CommonConstants.ServiceWords.PROGRAM;
                case "record": return CommonConstants.ServiceWords.RECORD;
                case "repeat": return CommonConstants.ServiceWords.REPEAT;
                case "then": return CommonConstants.ServiceWords.THEN;
                case "to": return CommonConstants.ServiceWords.TO;
                case "type": return CommonConstants.ServiceWords.TYPE;
                case "until": return CommonConstants.ServiceWords.UNTIL;
                case "var": return CommonConstants.ServiceWords.VAR;
                case "while": return CommonConstants.ServiceWords.WHILE;
                case "and": return CommonConstants.ServiceWords.AND;
                case "or": return CommonConstants.ServiceWords.OR;
                case "xor": return CommonConstants.ServiceWords.XOR;
                case "not": return CommonConstants.ServiceWords.NOT;
                case "div": return CommonConstants.ServiceWords.DIV;
                case "mod": return CommonConstants.ServiceWords.MOD;
                case "+": return CommonConstants.ServiceWords.PLUS;
                case "-": return CommonConstants.ServiceWords.MINUS;
                case "*": return CommonConstants.ServiceWords.MULTIPLY;
                case "/": return CommonConstants.ServiceWords.DIVIDE;
                case ":=": return CommonConstants.ServiceWords.ASSIGN;
                case "=": return CommonConstants.ServiceWords.EQUAL;
                case "<>": return CommonConstants.ServiceWords.NOT_EQUAL;
                case "<": return CommonConstants.ServiceWords.LESSER;
                case "<=": return CommonConstants.ServiceWords.LESSER_OR_EQUAL;
                case ">=": return CommonConstants.ServiceWords.GREATER_OR_EQUAL;
                case ">": return CommonConstants.ServiceWords.GREATER;
                case "..": return CommonConstants.ServiceWords.ELLIPSIS;
                case ":": return CommonConstants.ServiceWords.COLON;
                case ".": return CommonConstants.ServiceWords.POINT;
                case ",": return CommonConstants.ServiceWords.COMMA;
                case ";": return CommonConstants.ServiceWords.SEMICOLON;
                case "(": return CommonConstants.ServiceWords.LEFT_ROUND_BRACKET;
                case ")": return CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET;
                case "[": return CommonConstants.ServiceWords.LEFT_SQUARE_BRACKET;
                case "]": return CommonConstants.ServiceWords.RIGHT_SQUARE_BRACKET;
                case "out": return CommonConstants.ServiceWords.OUT;
                default: return CommonConstants.ServiceWords.NONE;
            }
        }
    }
}
