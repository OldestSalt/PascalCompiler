namespace PascalCompiler {
	public class PascalCompiler {
		public static StreamWriter? outputStream = null;
		static void Main(string[] args) {
			if (args.Length == 2) {
				string fileName = args[0];
				string compilerStage = args[1];
				Lexer.Lexer lexer = null;


				switch (compilerStage) {
					case "-l":
						lexer = new Lexer.Lexer(fileName);
						Lexer.Lexeme? lexeme = lexer.GetNextLexeme();
						OutputHandler.WriteLexeme(lexeme);

						while (lexeme!.type != Lexer.Constants.LexemeType.EOF) {
							lexeme = lexer.GetNextLexeme();
							OutputHandler.WriteLexeme(lexeme);
                        }
						break;
					case "-e":
						lexer = new Lexer.Lexer(fileName);
						Expressions.ExpressionParser expressionsParser = new Expressions.ExpressionParser(lexer);
						Expressions.Node tree = expressionsParser.ParseExpression();
						OutputHandler.WriteExpressionAST(tree);
						break;
					case "-s":
						lexer = new Lexer.Lexer(fileName);
						Parser.Parser parser = new Parser.Parser(lexer);
						Parser.Nodes.Node ast = parser.ParseProgram();
						ast.Accept(new Parser.Nodes.PrintVisitor());
						break;
					default:
						ExceptionHandler.Throw(Exceptions.UnknownKeys);
						break;
                }
			}
			else if (args.Length == 1) {
				switch (args[0]) {
					case "-lt":
						TestSystem.LexerTests();
						break;
					case "-et":
						TestSystem.ExpressionsParserTest();
						break;
					case "-st":
						TestSystem.ParserTest();
						break;
				}
			}

			else {
				ExceptionHandler.Throw(Exceptions.WithoutKeys);
			}
        }
	}
}

