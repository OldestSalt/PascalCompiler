
using PascalCompiler.Parser.Nodes;

namespace PascalCompiler {
    public static class TestSystem {
        public static StringWriter? output = null;
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

                List<string>? correctLexemeList = null;

                try {
                    correctLexemeList = File.ReadAllText($"{CommonConstants.ProjectPath}/tests/lexer/output/{file.Name}").Split('\n').ToList();
                }
                catch (FileNotFoundException) {
                    ExceptionHandler.Throw(Exceptions.TestError);
                }

                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                List<string> lexemeList = output.ToString().Split('\n').ToList();
                output = null;

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

                List<string>? correctASTlines = null;

                try {
                    correctASTlines = File.ReadAllText($"{CommonConstants.ProjectPath}/tests/expressions/output/{file.Name}").Split('\n').ToList();
                }
                catch (FileNotFoundException) {
                    ExceptionHandler.Throw(Exceptions.TestError);
                }

                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                List<string> ASTlines = output.ToString().Split('\n').ToList();
                output = null;

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

                List<string>? correctASTlines = null;
                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                try {
                    correctASTlines = File.ReadAllText($"{CommonConstants.ProjectPath}/tests/parser/output/{file.Name}").Split('\n').ToList();
                }
                catch (FileNotFoundException) {
                    ExceptionHandler.Throw(Exceptions.TestError);
                }

                List<string> ASTlines = output.ToString().Split('\n').ToList();
                output = null;


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

                List<string>? correctASTlines = null;
                StreamWriter origin = new StreamWriter(Console.OpenStandardOutput());
                origin.AutoFlush = true;
                Console.SetOut(origin);

                try {
                    correctASTlines = File.ReadAllText($"{CommonConstants.ProjectPath}/tests/semantic/output/{file.Name}").Split('\n').ToList();
                }
                catch (FileNotFoundException) {
                    ExceptionHandler.Throw(Exceptions.TestError);
                }

                List<string> ASTlines = output.ToString().Split('\n').ToList();
                output = null;

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
