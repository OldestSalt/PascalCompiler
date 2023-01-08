using System;
namespace PascalCompiler {
	public class PascalCompiler {
		static void Main(string[] args) {
			if (args.Length == 2) {
				string fileName = args[0];
				string compilerStage = args[1];

				switch (compilerStage) {
					case "-l":
                        Lexer.Lexer lexer = new Lexer.Lexer(fileName);
                        Lexer.Lexeme? lexeme = lexer.GetNextLexeme();
						OutputHandler.WriteLexeme(lexeme);

						while (lexeme!.type != Lexer.Constants.LexemeType.EOF) {
							lexeme = lexer.GetNextLexeme();
							OutputHandler.WriteLexeme(lexeme);
                        }
						//OutputHandler.WriteLexemes(lexemeList);
						break;
					default:
						ExceptionHandler.Throw(Exceptions.UnknownKeys);
						break;
                }
			}
			else {
				ExceptionHandler.Throw(Exceptions.WithoutKeys);
			}
        }
	}
}

