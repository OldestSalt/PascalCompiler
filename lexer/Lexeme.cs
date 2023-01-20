using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Lexer {
    public class Lexeme {
        public Constants.LexemeType type { get; set; }

        public uint lineNumber { get; set; } = 0;
        public uint charNumber { get; set; } = 0;
        public string? raw { get; set; } = null;
        public string? value { get; set; } = null;
    }
}
