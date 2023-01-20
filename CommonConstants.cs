using PascalCompiler.Lexer;
using PascalCompiler.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler {
    public static class CommonConstants {
        public enum ServiceWords {
            NONE,
            ARRAY,
            BEGIN,
            CASE,
            CONST,
            DO,
            DOWNTO,
            ELSE,
            END,
            FOR,
            FUNCTION,
            IF,
            OF,
            PROCEDURE,
            PROGRAM,
            RECORD,
            REPEAT,
            THEN,
            TO,
            TYPE,
            UNTIL,
            VAR,
            WHILE,
            AND,
            OR,
            XOR,
            NOT,
            DIV,
            MOD,
            PLUS,
            MINUS,
            MULTIPLY,
            DIVIDE,
            ASSIGN,
            EQUAL,
            GREATER,
            LESSER,
            GREATEROREQUAL,
            LESSEROREQUAL,
            NOTEQUAL,
            ELLIPSIS,
            COLON,
            POINT,
            COMMA,
            SEMICOLON,
            LEFTROUNDBRACKET,
            RIGHTROUNDBRACKET,
            LEFTSQUAREBRACKET,
            RIGHTSQUAREBRACKET,
            OUT
        }

        public static Dictionary<ServiceWords, string> ServiceWordsDict = new Dictionary<ServiceWords, string>() {
            { ServiceWords.ARRAY, "array" },
            { ServiceWords.BEGIN, "begin" },
            { ServiceWords.CASE, "case" },
            { ServiceWords.CONST, "const" },
            { ServiceWords.DO, "do" },
            { ServiceWords.DOWNTO, "downto" },
            { ServiceWords.ELSE, "else" },
            { ServiceWords.END, "end" },
            { ServiceWords.FOR, "for" },
            { ServiceWords.FUNCTION, "function" },
            { ServiceWords.IF, "if" },
            { ServiceWords.OF, "of" },
            { ServiceWords.PROCEDURE, "procedure" },
            { ServiceWords.PROGRAM, "program" },
            { ServiceWords.RECORD, "record" },
            { ServiceWords.REPEAT, "repeat" },
            { ServiceWords.THEN, "then" },
            { ServiceWords.TO, "to" },
            { ServiceWords.TYPE, "type" },
            { ServiceWords.UNTIL, "until" },
            { ServiceWords.VAR, "var" },
            { ServiceWords.WHILE, "while" },
            { ServiceWords.AND, "and" },
            { ServiceWords.OR, "or" },
            { ServiceWords.XOR, "xor" },
            { ServiceWords.NOT, "not" },
            { ServiceWords.DIV, "div" },
            { ServiceWords.MOD, "mod" },
            { ServiceWords.PLUS, "+" },
            { ServiceWords.MINUS, "-" },
            { ServiceWords.MULTIPLY, "*" },
            { ServiceWords.DIVIDE, "/" },
            { ServiceWords.ASSIGN, ":=" },
            { ServiceWords.EQUAL, "=" },
            { ServiceWords.GREATER, ">" },
            { ServiceWords.LESSER, "<" },
            { ServiceWords.GREATEROREQUAL, ">=" },
            { ServiceWords.LESSEROREQUAL, "<=" },
            { ServiceWords.NOTEQUAL, "<>" },
            { ServiceWords.ELLIPSIS, ".." },
            { ServiceWords.COLON, ":" },
            { ServiceWords.POINT, "." },
            { ServiceWords.COMMA, "," },
            { ServiceWords.SEMICOLON, ";" },
            { ServiceWords.LEFTROUNDBRACKET, "(" },
            { ServiceWords.RIGHTROUNDBRACKET, ")" },
            { ServiceWords.LEFTSQUAREBRACKET, "[" },
            { ServiceWords.RIGHTSQUAREBRACKET, "]" },
            { ServiceWords.OUT, "out"}
        };

        public static readonly string ProjectPath =
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
    }
}
