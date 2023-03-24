namespace PascalCompiler.Lexer {
    public static class Constants {
        public static readonly int MAX_IDENTIFIER_LENGTH = 127;

        public static readonly uint MAX_UNSIGNED_INT = 2147483647;

        public static readonly uint MAX_CHARACTER_NUMBER = 1114111;
        
        public enum LexemeType {
            RESERVED,
            IDENTIFIER,
            STRING,
            OPERATOR,
            SEPARATOR,
            INTEGER,
            REAL,
            EOF
        };

        public static readonly HashSet<string> ReservedWords = new HashSet<string> {
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
            "shl",
            "shr",
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
            "xor",
            "not",
            "div",
            "mod",
            "out",
            "in",
            "uses",
            "is"
        };

        public static readonly HashSet<char> Letters = new HashSet<char> {
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

        public static readonly HashSet<string> Operators = new HashSet<string>{
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
            "..",
            "and",
            "or",
            "xor",
            "not",
            "div",
            "mod"
        };

        public static readonly HashSet<char> ModifierChars = new HashSet<char> {
            '$',
            '&',
            '%'
        };

        public static readonly HashSet<char> Digits = new HashSet<char> {
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

        public static readonly HashSet<char> NonDecDigits = new HashSet<char> {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f'
        };

        public static readonly HashSet<char> BinDigits = new HashSet<char> {
            '0',
            '1'
        };

        public static readonly HashSet<char> NonBinDigits = new HashSet<char>{
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9'
        };

        public static readonly HashSet<char> OctalDigits = new HashSet<char> {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7'
        };

        public static readonly HashSet<char> NonOctalDigits = new HashSet<char>{
            '8',
            '9'
        };

        public static readonly HashSet<char> HexaDigits = new HashSet<char> {
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

        public static readonly HashSet<char> SpecialChars = new HashSet<char>{
            '+',
            '-',
            '*',
            '/',
            ':',
            '=',
            '<',
            '>',
            '.',
            ',',
            ';',
            '(',
            ')',
            '[',
            ']'
        };
    }
}
