using PascalCompiler.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                value = ParseExpression();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "=");
            }
            if (lexer.curLexeme!.value == ";") {
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
                value = ParseExpression();
            }

            if (lexer.curLexeme!.value == ";") {
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

            if (lexer.curLexeme!.value == ";") {
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
                args = ParseSubroutineArgs(false);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "(");
            }

            if ((new string[] { "const", "var", "type", "begin" }).Contains(lexer.curLexeme!.value)) {
                
                body = ParseSubroutineBody();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure body");
            }

            if (lexer.curLexeme!.value != ";") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
            }

            lexer.GetNextLexeme();
            return new NewProcedure(procName, args, body);
        }

        public SubroutineArgs ParseSubroutineArgs(bool isFunction) {
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
            if (lexer.curLexeme!.value == ")") {
                lexer.GetNextLexeme();
                if (lexer.curLexeme!.value == ";" && !isFunction || lexer.curLexeme!.value == ":" && isFunction) {
                    lexer.GetNextLexeme();
                    return new SubroutineArgs(argsList);
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, isFunction ? ":" : ";");
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
            while ((new string[] { "const", "var", "type" }).Contains(lexer.curLexeme!.value)) {
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
            Datatype returnType = null;
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
                args = ParseSubroutineArgs(true);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "(");
            }

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER || lexer.curLexeme!.value == "array" || lexer.curLexeme!.value == "record") {
                returnType = ParseDatatype();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "return datatype");
            }

            //lexer.GetNextLexeme();

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

            if (lexer.curLexeme!.value != ";") {
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
            lexer.GetNextLexeme();

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
                Node node = ParseStatementStartingWithIdentifier();
                if (node is SubroutineCall) {
                    return node as SubroutineCall;
                }
                else if (node is Identifier && (lexer.curLexeme!.value == ";" || lexer.curLexeme!.value == "end" || lexer.curLexeme!.value == "until" || lexer.curLexeme!.value == "else")) {
                    return new SubroutineCall(node as Identifier, null);
                }
                else {
                    return ParseAssignmentStatement(node as Reference);
                }
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "statement");
            }

            return null;
        }

        public Node ParseStatementStartingWithIdentifier() {
            Identifier name = new Identifier(lexer.curLexeme!);
            List<Expression> args = new List<Expression>();
            bool isSubroutine = false;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.value == "(") {
                isSubroutine = true;
                lexer.GetNextLexeme();
                while (lexer.curLexeme!.value != ")") {
                    args.Add(ParseExpression(true));
                    if (lexer.curLexeme!.value == ",") lexer.GetNextLexeme();
                    else if (lexer.curLexeme!.value != ")") {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ")");
                    }
                }

                lexer.GetNextLexeme();
            }

            if (lexer.curLexeme!.value == "[") {
                lexer.GetNextLexeme();
                List<Expression> indexes = new List<Expression>() { ParseExpression() };

                while (lexer.curLexeme!.value != "]") {
                    if (lexer.curLexeme!.value == ",") lexer.GetNextLexeme();
                    else if (lexer.curLexeme!.value != "]") {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "]");
                    }
                    indexes.Add(ParseExpression());
                }
                lexer.GetNextLexeme();

                if (isSubroutine) {
                    return new ArrayAccess(new SubroutineCall(name, args), indexes);
                }
                else {
                    return new ArrayAccess(name, indexes);
                }
            }
            else if (lexer.curLexeme!.value == ".") {
                lexer.GetNextLexeme();
                if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                    Identifier field = new Identifier(lexer.curLexeme!);
                    lexer.GetNextLexeme();

                    if (isSubroutine) {
                        return new RecordAccess(new SubroutineCall(name, args), field);
                    }
                    else {
                        return new RecordAccess(name, field);
                    }
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }
            }
            else if (isSubroutine) {
                return new SubroutineCall(name, args);
            }
            else {
                return name;
            }

            return null;
        }

        public Node ParseReference(Node reference) {
            while (lexer.curLexeme!.value == "." || lexer.curLexeme!.value == "[") {
                if (lexer.curLexeme!.value == ".") {
                    lexer.GetNextLexeme();
                    if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                        Identifier field = new Identifier(lexer.curLexeme!);
                        lexer.GetNextLexeme();
                        reference =  new RecordAccess(reference, field);
                    }
                    else {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                    }
                }
                else {
                    lexer.GetNextLexeme();
                    List<Expression> indexes = new List<Expression>() { ParseExpression() };

                    while (lexer.curLexeme!.value != "]") {
                        if (lexer.curLexeme!.value == ",") lexer.GetNextLexeme();
                        else if (lexer.curLexeme!.value != "]") {
                            ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "]");
                        }
                        indexes.Add(ParseExpression());
                    }

                    lexer.GetNextLexeme();

                    reference = new ArrayAccess(reference, indexes);
                }
            }

            return reference;

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

            while (lexer.curLexeme!.value != "until") {
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
            lexer.GetNextLexeme();

            condition = ParseExpression();

            return new RepeatStatement(condition, statements);
        }

        public ForStatement ParseForStatement() {
            Identifier counter = null;
            Expression start = null;
            Expression end = null;
            Statement body = null;

            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "counter name");
            }
            counter = new Identifier(lexer.curLexeme!);

            if (lexer.GetNextLexeme()!.value != ":=") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ":=");
            }
            lexer.GetNextLexeme();
            start = ParseExpression();

            if (lexer.curLexeme!.value != "to" && lexer.curLexeme!.value != "downto") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "to / downto");
            }

            lexer.GetNextLexeme();

            end = ParseExpression();

            if (lexer.curLexeme!.value != "do") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "do");
            }
            lexer.GetNextLexeme();

            body = ParseStatement();

            return new ForStatement(counter, start, end, body);
        }

        public AssignmentStatement ParseAssignmentStatement(Node reference) {
            Node leftPart = ParseReference(reference);
            if (leftPart is SubroutineCall) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "variable, array access or record access");
            }
            if (lexer.curLexeme!.value != ":=") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ":=");
            }
            lexer.GetNextLexeme();

            return new AssignmentStatement(leftPart, ParseExpression());
        }

        public Datatype ParseDatatype() {
            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                Identifier type = new Identifier(lexer.curLexeme!);
                lexer.GetNextLexeme();
                return new BaseDatatype(type);
            }
            else if (lexer.curLexeme!.value == "array") {
                return ParseArrayDatatype();
            }
            else if (lexer.curLexeme!.value == "record") {
                return ParseRecordDatatype();
            }
            return null;
        }

        public ArrayDatatype ParseArrayDatatype() {
            if (lexer.GetNextLexeme()!.value == "[") {
                List<Nodes.Index> indexes = new List<Nodes.Index>();
                Datatype type = null;

                lexer.GetNextLexeme();
                do {
                    Constant start = null;
                    Constant end = null;

                    if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.INTEGER) {
                        start = new Constant(lexer.curLexeme!);
                    }
                    else {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "start index");
                    }
                    lexer.GetNextLexeme();

                    if (lexer.curLexeme!.value != "..") {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "..");
                    }
                    lexer.GetNextLexeme();

                    if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.INTEGER) {
                        end = new Constant(lexer.curLexeme!);
                    }
                    else {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "start index");
                    }
                    lexer.GetNextLexeme();

                    indexes.Add(new Nodes.Index(start, end));

                    if (lexer.curLexeme!.value == ",") {
                        lexer.GetNextLexeme();
                    }
                } while (lexer.curLexeme!.value != "]");

                lexer.GetNextLexeme();

                if (lexer.curLexeme!.value != "of") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "of");
                }
                lexer.GetNextLexeme();

                if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER || lexer.curLexeme!.value == "array" || lexer.curLexeme!.value == "record") {
                    type = ParseDatatype();
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "array datatype");
                }

                return new ArrayDatatype(type, indexes);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "[");
            }
            return null;
        }

        public RecordDatatype ParseRecordDatatype() {
            List<NewField> fields = new List<NewField>();

            lexer.GetNextLexeme();
            while (lexer.curLexeme!.value != "end") {
                if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                    fields.Add(ParseNewField());
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }

                if (lexer.curLexeme!.value != "end" && lexer.curLexeme!.value != ";") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }

                //lexer.GetNextLexeme();

                if (lexer.curLexeme!.value == ";") lexer.GetNextLexeme();
            }
            lexer.GetNextLexeme();
            return new RecordDatatype(fields);
        }

        public NewField ParseNewField() {
            List<Identifier> names = new List<Identifier>() { new Identifier(lexer.curLexeme!) };
            Datatype type = null;

            lexer.GetNextLexeme();

            while (lexer.curLexeme!.value != ":") {
                if (lexer.curLexeme!.value != ",") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ",");
                }
                lexer.GetNextLexeme();

                if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                    names.Add(new Identifier(lexer.curLexeme!));
                }
                else {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }
                lexer.GetNextLexeme();
            }

            lexer.GetNextLexeme();
            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER || lexer.curLexeme!.value == "array" || lexer.curLexeme!.value == "record") {
                type = ParseDatatype();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field datatype");
            }

            return new NewField(names, type);
        }

        public Expression ParseExpression(bool isInBrackets = false) {
            SimpleExpression leftComparingOperand = ParseSimpleExpression();
            SimpleExpression? rightComparingOperand = null;
            CompareOperator? compareOperator = null;

            if ((new string[] { ">", "<", "=", "<>", ">=", "<=" }).Contains(lexer.curLexeme!.value)) {
                compareOperator = new CompareOperator(lexer.curLexeme!);
                lexer.GetNextLexeme();
                rightComparingOperand = ParseSimpleExpression();
            }

            if (lexer.curLexeme!.value == ")" && !isInBrackets) {
                ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ")");
            }

            return new Expression(leftComparingOperand, rightComparingOperand, compareOperator);
        }

        public SimpleExpression ParseSimpleExpression() {
            SimpleExpression? se = new SimpleExpression(null, null, ParseTerm());
            AddOperator? op = null;
            while ((new string[] { "+", "-", "or", "xor" }).Contains(lexer.curLexeme!.value)) {
                op = new AddOperator(lexer.curLexeme!);
                lexer.GetNextLexeme();
                se = new SimpleExpression(se, op, ParseTerm());
            }

            return se;
        }

        public Term ParseTerm() {
            MultiplyOperator? op = null;
            Term? t = new Term(null, null, ParseSimpleTerm());

            if ((new string[] { "*", "/", "div", "mod", "and" }).Contains(lexer.curLexeme!.value)) {
                op = new MultiplyOperator(lexer.curLexeme!);
                lexer.GetNextLexeme();
                t = new Term(t, op, ParseSimpleTerm());
            }

            return t;
        }

        public SimpleTerm ParseSimpleTerm() {
            List<UnaryOperator>? unaryOperators = new List<UnaryOperator>();

            while ((new string[] { "+", "-", "not" }).Contains(lexer.curLexeme!.value)) {
                unaryOperators.Add(new UnaryOperator(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }
            return new SimpleTerm(unaryOperators, ParseFactor());
        }

        public Factor ParseFactor() {
            Node value = null;

            if ((new Lexer.Constants.LexemeType[] { Lexer.Constants.LexemeType.REAL, Lexer.Constants.LexemeType.STRING, Lexer.Constants.LexemeType.INTEGER }).Contains(lexer.curLexeme!.type)) {
                value = new Constant(lexer.curLexeme!);

                lexer.GetNextLexeme();
            }
            else if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                value = ParseReference(ParseStatementStartingWithIdentifier());
            }
            else if (lexer.curLexeme!.value == "(") {
                lexer.GetNextLexeme();
                value = ParseExpression(true);
                if (lexer.curLexeme!.value != ")") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ")");
                }

                lexer.GetNextLexeme();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "factor");
            }


            return new Factor(value);
        }
    }
}
