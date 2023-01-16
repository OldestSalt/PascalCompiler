using PascalCompiler.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Parser {
    public class Parser {
        Lexer.Lexer lexer;
        public Parser(Lexer.Lexer lexer) {
            this.lexer = lexer;
            lexer.GetNextLexeme();
        }

        public Program ParseProgram() {
            Program node = new Program(ParseOptionalBlock(), ParseBlock());
            if (lexer.curLexeme!.value != ".") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme.lineNumber, lexer.curLexeme.charNumber, ".");
            }
            lexer.GetNextLexeme();
            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme.lineNumber, lexer.curLexeme.charNumber, "end of file");
            }
            return node;
        }

        public OptionalBlock? ParseOptionalBlock() {
            ProgramName? name = null;
            List<DeclarationSection> decls = new List<DeclarationSection>();
            if (lexer.curLexeme!.value == "program") {
                name = ParseProgramName();
            }

            while ((new string[] { "const", "var", "type", "procedure", "function" }).Contains(lexer.curLexeme!.value)) {
                decls.Add(ParseDeclarationSection());
            }

            if (lexer.curLexeme!.value == "begin") {
                if (name == null && decls.Count == 0) {
                    return null;
                }
                return new OptionalBlock(name, decls.Count == 0 ? null : decls);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme.lineNumber, lexer.curLexeme.charNumber, "begin");
            }
            return null;
        }

        public ProgramName? ParseProgramName() {
            lexer.GetNextLexeme();
            ProgramName? name = null;
            if (lexer.curLexeme != null && lexer.curLexeme.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                name = new ProgramName(new Identifier(lexer.curLexeme));
                if (lexer.GetNextLexeme()!.value != ";") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }
                lexer.GetNextLexeme();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "program name");
            }
            return name;
        }

        public DeclarationSection ParseDeclarationSection() {
            switch (lexer.curLexeme!.value) {
                case "const":
                    return ParseConstants();
                case "var":
                    return ParseVariables();
                case "type":
                    return ParseTypes();
                case "procedure":
                    return ParseProcedures();
                case "function":
                    return ParseFunctions();
                default:
                    ExceptionHandler.Throw(Exceptions.UnknownError, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber);
                    break;
            }
            return null;
        }

        public Constants ParseConstants() {
            List<NewConstant> constList = new List<NewConstant>();
            lexer.GetNextLexeme();
            while (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                constList.Add(ParseNewConstant());
            }
            if (constList.Count == 0) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "const declaration");
            }
            else {
                return new Constants(constList);
            }
            return null;
        }

        public NewConstant ParseNewConstant() {
            Identifier constName = new Identifier(lexer.curLexeme!);
            Datatype? constType = null;
            Expression value = null;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value == ":") {
                if (lexer.GetNextLexeme()!.type == Lexer.Constants.LexemeType.IDENTIFIER || lexer.curLexeme!.value == "array" || lexer.curLexeme!.value == "record") {
                    constType = ParseDatatype();
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
                }
            }

            if (lexer.curLexeme!.value == "=") {
                lexer.GetNextLexeme();
                value = ParseExpression(); //expression check isn't here
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "=");
            }
            if (lexer.GetNextLexeme()!.value == ";") {
                lexer.GetNextLexeme();
                return new NewConstant(constName, constType, value);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }
            return null;
        }

        public Variables ParseVariables() {
            List<NewVariable> varList = new List<NewVariable>();
            lexer.GetNextLexeme();
            while (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                varList.Add(ParseNewVariable());
            }
            if (varList.Count == 0) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "variable declaration");
            }
            else {
                return new Variables(varList);
            }
            return null;
        }

        public NewVariable ParseNewVariable() {
            Identifier varName = new Identifier(lexer.curLexeme!);
            Datatype varType = null;
            Expression? value = null;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value == ":") {
                if (lexer.GetNextLexeme()!.type == Lexer.Constants.LexemeType.IDENTIFIER || lexer.curLexeme!.value == "array" || lexer.curLexeme!.value == "record") {
                    varType = ParseDatatype();
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
                }
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ":");
            }

            if (lexer.curLexeme!.value == "=") {
                lexer.GetNextLexeme();
                value = ParseExpression(); //expression check isn't here
            }

            if (lexer.GetNextLexeme()!.value == ";") {
                lexer.GetNextLexeme();
                return new NewVariable(varName, varType, value);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }
            return null;
        }

        public Types ParseTypes() {
            List<NewType> typeList = new List<NewType>();
            lexer.GetNextLexeme();
            while (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                typeList.Add(ParseNewType());
            }
            if (typeList.Count == 0) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "type declaration");
            }
            else {
                return new Types(typeList);
            }
            return null;
        }

        public NewType ParseNewType() {
            Identifier newTypeName = new Identifier(lexer.curLexeme!);
            Datatype oldTypeName = null;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value == "=") {
                if (lexer.GetNextLexeme()!.type == Lexer.Constants.LexemeType.IDENTIFIER || lexer.curLexeme!.value == "array" || lexer.curLexeme!.value == "record") {
                    oldTypeName = ParseDatatype();
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
                }
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "=");
            }

            if (lexer.GetNextLexeme()!.value == ";") {
                lexer.GetNextLexeme();
                return new NewType(newTypeName, oldTypeName);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }
            return null;
        }

        public Procedures ParseProcedures() {
            List<NewProcedure> procList = new List<NewProcedure>();
            while (lexer.curLexeme!.value == "procedure") {
                procList.Add(ParseNewProcedure());
            }
            return new Procedures(procList);
        }

        public NewProcedure ParseNewProcedure() {
            Identifier procName = null;
            SubroutineArgs args = null;
            SubroutineBody body = null;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                procName = new Identifier(lexer.curLexeme!);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure name");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value == "(") {
                args = ParseSubroutineArgs();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "(");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value != ";") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }

            lexer.GetNextLexeme();

            if ((new string[] { "const", "var", "type", "begin" }).Contains(lexer.curLexeme!.value)) {
                
                body = ParseSubroutineBody();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure body");
            }

            if (lexer.GetNextLexeme()!.value != ";") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }

            lexer.GetNextLexeme();
            return new NewProcedure(procName, args, body);
        }

        public SubroutineArgs ParseSubroutineArgs() {
            List<NewSubroutineArg> argsList= new List<NewSubroutineArg>();
            lexer.GetNextLexeme();

            if ((new string[] { "const", "var", "out" }).Contains(lexer.curLexeme!.value) || lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                argsList.Add(ParseNewSubroutineArg());
                while (lexer.curLexeme!.value == ";") {
                    lexer.GetNextLexeme();
                    if ((new string[] { "const", "var", "out" }).Contains(lexer.curLexeme!.value) || lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                        argsList.Add(ParseNewSubroutineArg());
                    }
                    else {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg");
                    }
                }
            }
            else if (lexer.curLexeme!.value == ")") {
                if (lexer.GetNextLexeme()!.value == ";") {
                    lexer.GetNextLexeme();
                    return new SubroutineArgs(argsList);
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ")");
            }
            return null;
        }

        public NewSubroutineArg ParseNewSubroutineArg() {
            ArgModifier? mod = null;
            List<Identifier> argNames= new List<Identifier>();
            Node type = null;

            if ((new string[] { "const", "var", "out" }).Contains(lexer.curLexeme!.value)) {
                mod = new ArgModifier(lexer.curLexeme.value);
                lexer.GetNextLexeme();
            }

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                argNames.Add(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
                while (lexer.curLexeme!.value == ",") {
                    if (lexer.GetNextLexeme()!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                        argNames.Add(new Identifier(lexer.curLexeme!));
                        lexer.GetNextLexeme();
                    }
                    else {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg name");
                    }
                }
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg name");
            }

            if (lexer.curLexeme!.value != ":") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ":");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                type = new BaseDatatype(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }
            else if (lexer.curLexeme!.value == "array") {
                type = ParseArraySubroutineArg();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg datatype");
            }

            return new NewSubroutineArg(mod, argNames, type);

        }

        public ArraySubroutineArg ParseArraySubroutineArg() {
            BaseDatatype type = null;
            lexer.GetNextLexeme();
            if (lexer.curLexeme!.value == "of") {
                if (lexer.GetNextLexeme()!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                    type = new BaseDatatype(new Identifier(lexer.curLexeme!));
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
                }
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "of");
            }
            return new ArraySubroutineArg(type);
        }

        public SubroutineBody ParseSubroutineBody() {
            List<DeclarationSection>? decls = new List<DeclarationSection>();
            Block body = null;
            while ((new string[] { "const", "var", "type" }).Contains(lexer.curLexeme!.value) {
                if (lexer.curLexeme!.value == "const") {
                    decls.Add(ParseConstants());
                }
                else if (lexer.curLexeme!.value == "var") {
                    decls.Add(ParseVariables());
                }
                else {
                    decls.Add(ParseTypes());
                }
            }

            if (lexer.curLexeme!.value == "begin") {
                body = ParseBlock();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "subroutine body");
            }

            return new SubroutineBody(decls, body);
        }

        public Functions ParseFunctions() {
            List<NewFunction> funcList = new List<NewFunction>();
            while (lexer.curLexeme!.value == "function") {
                funcList.Add(ParseNewFunction());
            }
            return new Functions(funcList);
        }

        public NewFunction ParseNewFunction() {
            Identifier funcName = null;
            SubroutineArgs args = null;
            BaseDatatype returnType = null;
            SubroutineBody body = null;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                funcName = new Identifier(lexer.curLexeme!);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "function name");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value == "(") {
                args = ParseSubroutineArgs();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "(");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value != ":") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ":");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                returnType = new BaseDatatype(new Identifier(lexer.curLexeme!));
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "return datatype");
            }

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value != ";") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }

            lexer.GetNextLexeme();

            if ((new string[] { "const", "var", "type", "begin" }).Contains(lexer.curLexeme!.value)) {

                body = ParseSubroutineBody();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "function body");
            }

            if (lexer.GetNextLexeme()!.value != ";") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }

            lexer.GetNextLexeme();
            return new NewFunction(funcName, args, returnType, body);
        }

        public Block ParseBlock() {
            lexer.GetNextLexeme();
            List<Statement> statements = new List<Statement>();
            
            while (lexer.curLexeme!.value != "end" && lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                if (lexer.curLexeme!.value == ";") {
                    statements.Add(new EmptyStatement());
                }
                else {
                    statements.Add(ParseStatement());
                }
                if (lexer.curLexeme!.value == "end") break;
                else if (lexer.curLexeme!.value != ";") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }
                lexer.GetNextLexeme();
            }

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.EOF) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "end");
            }

            return new Block(statements);
        }

        public Statement ParseStatement() {
            if (lexer.curLexeme!.value == "if") {
                return ParseIfStatement();
            }
            else if (lexer.curLexeme!.value == "while") {
                return ParseWhileStatement();
            }
            else if (lexer.curLexeme!.value == "repeat") {
                return ParseRepeatStatement();
            }
            else if (lexer.curLexeme!.value == "for") {
                return ParseForStatement();
            }
            else if (lexer.curLexeme!.value == "begin") {
                return ParseBlock();
            }
            else if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                lexer.GetNextLexeme();
                if (lexer.curLexeme!.value == "(") {
                    return Parse
                }
            }
        }

        public IfStatement ParseIfStatement() {
            Expression condition = null;
            Statement trueStatement = null;
            Statement? falseStatement = null;

            lexer.GetNextLexeme();

            condition = ParseExpression();

            if (lexer.curLexeme!.value != "then") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "then");
            }
            lexer.GetNextLexeme();

            trueStatement = ParseStatement();

            if (lexer.curLexeme!.value == "else") {
                lexer.GetNextLexeme();
                falseStatement = ParseStatement();
            }

            return new IfStatement(condition, trueStatement, falseStatement);
        }

        public WhileStatement ParseWhileStatement() {
            Expression condition = null;
            Statement body = null;

            lexer.GetNextLexeme();

            condition = ParseExpression();

            if (lexer.curLexeme!.value != "do") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "do");
            }
            lexer.GetNextLexeme();

            body = ParseStatement();

            return new WhileStatement(condition, body);
        }

        public RepeatStatement ParseRepeatStatement() {
            List<Statement> statements = new List<Statement>();
            Expression condition = null;

            lexer.GetNextLexeme();

            while (lexer.curLexeme!.value != "until" && lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                if (lexer.curLexeme!.value == ";") {
                    statements.Add(new EmptyStatement());
                }
                else {
                    statements.Add(ParseStatement());
                }
                if (lexer.curLexeme!.value == "until") break;
                else if (lexer.curLexeme!.value != ";") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }
                lexer.GetNextLexeme();
            }

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.EOF) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "end");
            }

            condition = ParseExpression();

            return new RepeatStatement(condition, statements);
        }

        public ForStatement ParseForStatement() {
        }

        public AssignmentStatement ParseAssignmentStatement(List<Lexer.Lexeme>? lexemes) {

        }

        public SubroutineCall ParseSubroutineCall() {

        }

        public Datatype ParseDatatype() {

        }

        public Expression ParseExpression() {

        }
    }
}
