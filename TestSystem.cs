
namespace PascalCompiler {
    public static class TestSystem {
        public static void LexerTests() {
            foreach (var file in new DirectoryInfo($"{CommonConstants.ProjectPath}/tests/lexer/input").GetFiles()) {
                PascalCompiler.outputStream = new StreamWriter($"{CommonConstants.ProjectPath}/tests/lexer/output.txt");
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

                List<string>? correctLexemeList = null;
                PascalCompiler.outputStream.Close();
                PascalCompiler.outputStream = null;

                try {
                    correctLexemeList = File.ReadAllLines($"{CommonConstants.ProjectPath}/tests/lexer/output/{file.Name}").ToList();
                }
                catch (FileNotFoundException) {
                    ExceptionHandler.Throw(Exceptions.TestError);
                }

                List<string> lexemeList = File.ReadAllLines($"{CommonConstants.ProjectPath}/tests/lexer/output.txt").ToList();

                bool isCorrect = true;

                if (correctLexemeList!.Count != lexemeList.Count) {
                    Console.WriteLine($"Test file '{file.Name}':\tWrong answer!");
                    continue;
                }

                for (int i = 0; i < correctLexemeList!.Count; i++) {
                    if (correctLexemeList[i] != lexemeList[i]) {
                        Console.WriteLine($"Test file '{file.Name}':\tWrong answer! Expected:\n\t{correctLexemeList[i]}\nGot:\n\t{lexemeList[i]}");
                        isCorrect = false;
                        break;
                    }
                }
                if (isCorrect) Console.WriteLine($"Test file '{file.Name}':\tOK");
            }
        }

        public static void ExpressionsParserTest() {
            foreach (var file in new DirectoryInfo($"{CommonConstants.ProjectPath}/tests/expressions/input").GetFiles()) {
                PascalCompiler.outputStream = new StreamWriter($"{CommonConstants.ProjectPath}/tests/expressions/output.txt");
                Parser.ExpressionParser parser = new Parser.ExpressionParser(new Lexer.Lexer(file.FullName));
                try {
                    OutputHandler.WriteAST(parser.ParseExpression());
                }
                catch {

                }

                List<string>? correctASTlines = null;
                PascalCompiler.outputStream.Close();
                PascalCompiler.outputStream = null;

                try {
                    correctASTlines = File.ReadAllLines($"{CommonConstants.ProjectPath}/tests/expressions/output/{file.Name}").ToList();
                }
                catch (FileNotFoundException) {
                    ExceptionHandler.Throw(Exceptions.TestError);
                }

                List<string> ASTlines = File.ReadAllLines($"{CommonConstants.ProjectPath}/tests/expressions/output.txt").ToList();

                bool isCorrect = true;

                if (correctASTlines!.Count != ASTlines.Count) {
                    Console.WriteLine($"Test file '{file.Name}':\tWrong answer!");
                    continue;
                }

                for (int i = 0; i < correctASTlines!.Count; i++) {
                    if (correctASTlines[i] != ASTlines[i]) {
                        Console.WriteLine($"Test file '{file.Name}':\tWrong answer in line {i}!");
                        isCorrect = false;
                        break;
                    }
                }
                if (isCorrect) Console.WriteLine($"Test file '{file.Name}':\tOK");
            }
        }
    }
}
