namespace PascalCompiler {
    public static class OutputHandler {

        public static void WriteLexeme(Lexer.Lexeme? lexeme) {
            Console.WriteLine($"{lexeme!.lineNumber}, {lexeme!.charNumber}:\t{lexeme!.type}\t{lexeme!.value}\t{lexeme!.raw}");
        }
        public static void WriteExpressionAST(Expressions.Node ast, string indents = "") {
            Console.WriteLine(ast.value);
            if (ast is Expressions.BinaryOperation) {
                Expressions.BinaryOperation node = (Expressions.BinaryOperation)ast;
                Console.Write(indents + "├─── ");
                WriteExpressionAST(node.left, indents + "│    ");

                Console.Write(indents + "└─── ");
                WriteExpressionAST(node.right, indents + "     ");
            }
        }

        public static void Write(string text) {
            Console.Write(text);
        }

        public static void WriteLine(string text) {
            Console.WriteLine(text);
        }
    }
}
