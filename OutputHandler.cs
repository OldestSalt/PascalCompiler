using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler {
    public static class OutputHandler {
        public static void WriteLexemes(List<Lexer.Lexeme?> lexemes) {
            foreach (var lexeme in lexemes) {
                Console.WriteLine($"{lexeme!.lineNumber, 3}, {lexeme!.charNumber, 2}:\t{lexeme!.type, 10}\t{lexeme!.value, 20}\t{lexeme!.raw, 20}");
            }
        }

        public static void WriteLexeme(Lexer.Lexeme? lexeme) {
            Console.WriteLine($"{lexeme!.lineNumber, 3}, {lexeme!.charNumber, 2}:\t{lexeme!.type, 10}\t{lexeme!.value, 20}\t{lexeme!.raw, 20}");
        }
    }
}
