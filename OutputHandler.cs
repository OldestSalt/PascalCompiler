using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler {
    public static class OutputHandler {

        public static void WriteLexeme(Lexer.Lexeme? lexeme) {
            var outputStream = PascalCompiler.outputStream != null ? PascalCompiler.outputStream : Console.Out;
            outputStream.WriteLine($"{lexeme!.lineNumber}, {lexeme!.charNumber}:\t{lexeme!.type}\t{lexeme!.value}\t{lexeme!.raw}");
        }
        public static void WriteExpressionAST(Expressions.Node ast, string indents = "") {
            var outputStream = PascalCompiler.outputStream != null ? PascalCompiler.outputStream : Console.Out;
            outputStream.WriteLine(ast.value);
            if (ast is Expressions.BinaryOperation) {
                Expressions.BinaryOperation node = (Expressions.BinaryOperation)ast;
                outputStream.Write(indents + "├─── ");
                WriteExpressionAST(node.left, indents + "│    ");

                outputStream.Write(indents + "└─── ");
                WriteExpressionAST(node.right, indents + "     ");
            }
        }

        public static void Write(string text) {
            var outputStream = PascalCompiler.outputStream != null ? PascalCompiler.outputStream : Console.Out;
            outputStream.Write(text);
        }

        public static void WriteLine(string text) {
            var outputStream = PascalCompiler.outputStream != null ? PascalCompiler.outputStream : Console.Out;
            outputStream.WriteLine(text);
        }
    }
}
