namespace PascalCompiler.Lexer {
    public static class Constants {
        public static readonly int MAX_IDENTIFIER_LENGTH = 127; 

        public static readonly uint MAX_UNSIGNED_INT = 2147483647;

        public static readonly uint MAX_CHARACTER_NUMBER = 1114111;
        
        public enum LexemeType {
            UNKNOWN, // Useful for debug
            RESERVED,
            IDENTIFIER,
            STRING,
            OPERATOR,
            SEPARATOR,
            INTEGER,
            REAL,
            EOF
        };

        public static readonly string[] ReservedWords = {
            "array",
            "begin",
            "case",
            "const",
            "do",
            "downto",
            "else",
            "end",
            "file",
            "for",
            "function",
            "goto",
            "if",
            "label",
            "nil",
            "of",
            "packed",
            "procedure",
            "program",
            "record",
            "repeat",
            "set",
            "then",
            "to",
            "type",
            "until",
            "var",
            "while",
            "with",
            "and",
            "or",
            "not",
            "div",
            "mod",
            "in"
        };

        public static readonly char[] Letters = {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z'
        };

        public static readonly string[] Operators = {
            "+",
            "-",
            "*",
            "/",
            ":=",
            "=",
            "<>",
            "<",
            "<=",
            ">=",
            ">",
            "^",
            ".",
            "and",
            "or",
            "not",
            "div",
            "mod",
            "in"
        };

        public static readonly string[] Separators = {
            ",",
            ";",
            ":",
            "(",
            ")",
            "[",
            "]",
            ".."
        };

        public static readonly char[] InsignificantChars = {
            ' ',
            '\n',
            '\t',
            '\r',
            '\0'
        };

        public static readonly char[] ModifierChars = {
            '$',
            '&',
            '%'
        };

        public static readonly char[] Digits = {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9'
        };

        public static readonly char[] NonDecDigits = {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f'
        };

        public static readonly char[] BinDigits = {
            '0',
            '1'
        };

        public static readonly char[] NonBinDigits = {
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9'
        };

        public static readonly char[] OctalDigits = {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7'
        };

        public static readonly char[] NonOctalDigits = {
            '8',
            '9'
        };

        public static readonly char[] HexaDigits = {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f'
        };

        public static readonly char[] SpecialChars = {
            '+',
            '-',
            '*',
            '/',
            ':',
            '=',
            '<',
            '>',
            '^',
            '.',
            ',',
            ';',
            '(',
            ')',
            '[',
            ']'
        };

        public static readonly string ProjectPath =
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
    }
}
