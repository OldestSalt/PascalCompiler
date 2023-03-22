
using PascalCompiler.Parser.Nodes;

namespace PascalCompiler {
    public static class TestSystem {
        public static StringWriter? output = null;
        public static List<string> getCorrect(string path) {
            List<string>? correctOutput = null;

            try {
                correctOutput = File.ReadAllText(path).Split('\n').ToList();
            }
            catch (FileNotFoundException) {
                ExceptionHandler.Throw(Exceptions.TestError);
            }
            return correctOutput!;
        }
        public static void Compare(List<string> output, List<string> correctOutput, string testFilename) {
            bool isCorrect = true;

            if (output!.Count != correctOutput.Count) {
                Console.WriteLine($"Test file '{testFilename}':\tWrong answer!");
                return;
            }

            for (int i = 0; i < correctOutput!.Count; i++) {
                if (correctOutput[i] != output[i]) {
                    Console.WriteLine($"Test file '{testFilename}':\tWrong answer! Expected:\n\t{correctOutput[i]}\nGot:\n\t{output[i]}");
                    isCorrect = false;
                    break;
                }
            }
            if (isCorrect) Console.WriteLine($"Test file '{testFilename}':\tOK");
        }
        public static void LexerTests() {
            foreach (var file in new DirectoryInfo($"{CommonConstants.ProjectPath}/tests/lexer/input").GetFiles()) {
                output = new StringWriter();
                Console.SetOut(output);
                Lexer.Lexer lexer = new Lexer.Lexer(file.FullName);
                try {
                    Lexer.Lexeme? lexeme = lexer.GetNextLexeme();
                    OutputHandler.WriteLexeme(lexeme);
                    while (lexeme!.type != Lexer.Constants.LexemeType.EOF) {
                        lexeme = lexer.GetNextLexeme();
                        OutputHandler.WriteLexeme(lexeme);
                    }
                }
                catch {

                }

                List<string> correctLexemeList = getCorrect($"{CommonConstants.ProjectPath}/tests/lexer/output/{file.Name}");

                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                List<string> lexemeList = output.ToString().Split('\n').ToList();
                output = null;

                Compare(lexemeList, correctLexemeList!, file.Name);
            }
        }

        public static void ExpressionsParserTest() {
            foreach (var file in new DirectoryInfo($"{CommonConstants.ProjectPath}/tests/expressions/input").GetFiles()) {
                output = new StringWriter();
                Console.SetOut(output);
                Lexer.Lexer lexer = new Lexer.Lexer(file.FullName);
                Expressions.ExpressionParser parser = new Expressions.ExpressionParser(lexer);

                try {
                    OutputHandler.WriteExpressionAST(parser.ParseExpression());
                    if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                        ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber);
                    }
                }
                catch {

                }
                
                List<string> correctASTlines = getCorrect($"{CommonConstants.ProjectPath}/tests/expressions/output/{file.Name}");
                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                List<string> ASTlines = output.ToString().Split('\n').ToList();
                output = null;

                Compare(ASTlines, correctASTlines!, file.Name);
            }
        }

        public static void ParserTest() {
            foreach (var file in new DirectoryInfo($"{CommonConstants.ProjectPath}/tests/parser/input").GetFiles()) {
                output = new StringWriter();
                Console.SetOut(output);
                Lexer.Lexer lexer = new Lexer.Lexer(file.FullName);
                Parser.Parser parser = new Parser.Parser(lexer);

                try {
                    parser.ParseProgram().Print(new PrintVisitor());
                    if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                        ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber);
                    }
                }
                catch {

                }

                List<string>? correctASTlines = getCorrect($"{CommonConstants.ProjectPath}/tests/parser/output/{file.Name}");
                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);


                List<string> ASTlines = output.ToString().Split('\n').ToList();
                output = null;


                Compare(ASTlines, correctASTlines!, file.Name);
            }
        }

        public static void SemanticTest() {
            foreach (var file in new DirectoryInfo($"{CommonConstants.ProjectPath}/tests/semantic/input").GetFiles()) {
                output = new StringWriter();
                Console.SetOut(output);
                Lexer.Lexer lexer = new Lexer.Lexer(file.FullName);
                Parser.Parser parser = new Parser.Parser(lexer);

                try {
                    Program ast = parser.ParseProgram();
                    Semantic.SymVisitor sym = new Semantic.SymVisitor();
                    ast.Sym(sym);
                    ast.Print(new PrintVisitor());
                    sym.Print();
                    if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                        ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber);
                    }
                }
                catch {

                }

                List<string>? correctASTlines = getCorrect($"{CommonConstants.ProjectPath}/tests/semantic/output/{file.Name}");
                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                List<string> ASTlines = output.ToString().Split('\n').ToList();
                output = null;

                Compare(ASTlines, correctASTlines!, file.Name);
            }
        }
    }
}
