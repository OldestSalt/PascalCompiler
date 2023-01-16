using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Expressions {
    public class ExpressionParser {
        private Lexer.Lexer lexer;
        public ExpressionParser(Lexer.Lexer lexer) {
            this.lexer = lexer;
            lexer.GetNextLexeme();
        }

        public Node ParseExpression(bool isBracket = false) {
            Node n = ParseTerm();
            Lexer.Lexeme? lexeme = lexer.curLexeme;

            while (lexeme!.value == "+" || lexeme!.value == "-") {
                lexer.GetNextLexeme();
                n = new BinaryOperation(n, ParseTerm(), lexeme.value);
                lexeme = lexer.curLexeme;
            }

            if (lexeme!.value == ")" && !isBracket) {
                ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, lexeme.value);
            }

            return n;
        }

        public Node ParseTerm() {
            Node n = ParseFactor();
            Lexer.Lexeme? lexeme = lexer.curLexeme;

            while (lexeme!.value == "*" || lexeme!.value == "/") {
                lexer.GetNextLexeme();
                n = new BinaryOperation(n, ParseFactor(), lexeme.value);
                lexeme = lexer.curLexeme;
            }
            return n;
        }

        public Node ParseFactor() {
        Lexer.Lexeme? lexeme = lexer.curLexeme;
            if (lexeme!.type == Lexer.Constants.LexemeType.REAL || lexeme!.type == Lexer.Constants.LexemeType.INTEGER) {
                lexer.GetNextLexeme();
                return new Number(lexeme.value);
            }
            else if (lexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                lexer.GetNextLexeme();
                return new Variable(lexeme.value);
            }
            else if (lexeme.value == "(") {
                lexer.GetNextLexeme();
                Node n = ParseExpression(true);

                if (lexer.curLexeme!.value != ")") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme.lineNumber, lexer.curLexeme.charNumber, ")");
                }

                lexer.GetNextLexeme();
                return n;
            }
            //else {
            //    ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, lexeme.value);
            //}

            ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "operand");
            return null;
        }
    }
}
