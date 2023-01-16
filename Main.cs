﻿using System;
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
					case "-exp":
						lexer = new Lexer.Lexer(fileName);
						Expressions.ExpressionParser parser = new Expressions.ExpressionParser(lexer);
						Expressions.Node tree = parser.ParseExpression();
						OutputHandler.WriteAST(tree);
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
					case "-expt":
						TestSystem.ExpressionsParserTest();
						break;
				}
			}

			else {
				ExceptionHandler.Throw(Exceptions.WithoutKeys);
			}
        }
	}
}

