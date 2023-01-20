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
            OptionalBlock? optBlock = ParseOptionalBlock();
            lexer.GetNextLexeme();
            Program node = new Program(optBlock, ParseBlock());
            ParserUtils.RequireLexeme(lexer, ".");
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
            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                name = new ProgramName(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
                ParserUtils.RequireLexeme(lexer, ";");
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

            ParserUtils.RequireLexeme(lexer, "=");
            value = ParseExpression();
            ParserUtils.RequireLexeme(lexer, ";");

            return new NewConstant(constName, constType, value);
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

            return new Variables(varList);
        }

        public NewVariable ParseNewVariable() {
            Identifier varName = new Identifier(lexer.curLexeme!);
            Datatype varType = null;
            Expression? value = null;
            lexer.GetNextLexeme();
            ParserUtils.RequireLexeme(lexer, ":");

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.value != "array" && lexer.curLexeme!.value != "record") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
            }

            varType = ParseDatatype();

            if (lexer.curLexeme!.value == "=") {
                lexer.GetNextLexeme();
                value = ParseExpression();
            }

            ParserUtils.RequireLexeme(lexer, ";");

            return new NewVariable(varName, varType, value);
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

            return new Types(typeList);
        }

        public NewType ParseNewType() {
            Identifier newTypeName = new Identifier(lexer.curLexeme!);
            Datatype oldTypeName = null;
            lexer.GetNextLexeme();
            ParserUtils.RequireLexeme(lexer, "=");

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.value != "array" && lexer.curLexeme!.value != "record") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
            }

            oldTypeName = ParseDatatype();
            ParserUtils.RequireLexeme(lexer, ";");

            return new NewType(newTypeName, oldTypeName);
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

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure name");
            }
            procName = new Identifier(lexer.curLexeme!);
            lexer.GetNextLexeme();
            ParserUtils.RequireLexeme(lexer, "(");
            args = ParseSubroutineArgs(false);

            if (!(new string[] { "const", "var", "type", "begin" }).Contains(lexer.curLexeme!.value)) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure body");
            }

            body = ParseSubroutineBody();
            ParserUtils.RequireLexeme(lexer, ";");

            return new NewProcedure(procName, args, body);
        }

        public SubroutineArgs ParseSubroutineArgs(bool isFunction) {
            List<NewSubroutineArg> argsList= new List<NewSubroutineArg>();

            if ((new string[] { "const", "var", "out" }).Contains(lexer.curLexeme!.value) || lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                argsList.Add(ParseNewSubroutineArg());
                while (lexer.curLexeme!.value == ";") {
                    lexer.GetNextLexeme();
                    if (!(new string[] { "const", "var", "out" }).Contains(lexer.curLexeme!.value) && lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg");
                    }

                    argsList.Add(ParseNewSubroutineArg());
                }
            }

            ParserUtils.RequireLexeme(lexer, ")");

            if (!(lexer.curLexeme!.value == ";" && !isFunction || lexer.curLexeme!.value == ":" && isFunction)) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, isFunction ? ":" : ";");
            }
            lexer.GetNextLexeme();
            
            return new SubroutineArgs(argsList);
        }

        public NewSubroutineArg ParseNewSubroutineArg() {
            ArgModifier? mod = null;
            List<Identifier> argNames = new List<Identifier>();
            Node type = null;

            if ((new string[] { "const", "var", "out" }).Contains(lexer.curLexeme!.value)) {
                mod = new ArgModifier(lexer.curLexeme!.value);
                lexer.GetNextLexeme();
            }

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg name");
            }
            argNames.Add(new Identifier(lexer.curLexeme!));
            lexer.GetNextLexeme();

            while (lexer.curLexeme!.value == ",") {
                if (lexer.GetNextLexeme()!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg name");
                }

                argNames.Add(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            ParserUtils.RequireLexeme(lexer, ":");


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

            ParserUtils.RequireLexeme(lexer, "of");

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
            }

            type = new BaseDatatype(new Identifier(lexer.curLexeme!));

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

            ParserUtils.RequireLexeme(lexer, "begin");

            body = ParseBlock();

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

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "function name");
            }

            funcName = new Identifier(lexer.curLexeme!);

            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, "(");

            args = ParseSubroutineArgs(true);

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.value != "array" && lexer.curLexeme!.value != "record") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "return datatype");
            }
            returnType = ParseDatatype();

            ParserUtils.RequireLexeme(lexer, ";");

            if (!(new string[] { "const", "var", "type", "begin" }).Contains(lexer.curLexeme!.value)) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "function body");
            }

            body = ParseSubroutineBody();

            ParserUtils.RequireLexeme(lexer, ";");

            return new NewFunction(funcName, args, returnType, body);
        }

        public Block ParseBlock() {
            List<Statement> statements = new List<Statement>();
            
            while (lexer.curLexeme!.value != "end" && lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                if (lexer.curLexeme!.value == ";") {
                    statements.Add(new EmptyStatement());
                }
                else {
                    statements.Add(ParseStatement());
                }

                if (lexer.curLexeme!.value == "end") break;
                else ParserUtils.RequireLexeme(lexer, ";");
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
                lexer.GetNextLexeme();
                return ParseBlock();
            }
            else if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                Node node = ParseStatementStartingWithIdentifier();

                if (node is SubroutineCall) {
                    return node as SubroutineCall;
                }
                else if (node is Identifier && (new string[] { ";", "end", "until", "else" }).Contains(lexer.curLexeme!.value)) {
                    return new SubroutineCall(node as Identifier, null);
                }
                else {
                    return ParseAssignmentStatement(node as Reference);
                }
            }

            ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "statement");

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
                if (lexer.GetNextLexeme()!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }

                Identifier field = new Identifier(lexer.curLexeme!);
                lexer.GetNextLexeme();

                if (isSubroutine) {
                    return new RecordAccess(new SubroutineCall(name, args), field);
                }
                else {
                    return new RecordAccess(name, field);
                }
            }
            else if (isSubroutine) {
                return new SubroutineCall(name, args);
            }
            else {
                return name;
            }
        }

        public Node ParseReference(Node reference) {
            while (lexer.curLexeme!.value == "." || lexer.curLexeme!.value == "[") {
                if (lexer.curLexeme!.value == ".") {
                    lexer.GetNextLexeme();
                    if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                    }

                    Identifier field = new Identifier(lexer.curLexeme!);
                    lexer.GetNextLexeme();
                    reference =  new RecordAccess(reference, field);
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

            ParserUtils.RequireLexeme(lexer, "then");

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

            ParserUtils.RequireLexeme(lexer, "do");

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
                else ParserUtils.RequireLexeme(lexer, ";");
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
            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, ":=");
            start = ParseExpression();

            if (lexer.curLexeme!.value != "to" && lexer.curLexeme!.value != "downto") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "to / downto");
            }
            lexer.GetNextLexeme();

            end = ParseExpression();
            ParserUtils.RequireLexeme(lexer, "do");

            body = ParseStatement();

            return new ForStatement(counter, start, end, body);
        }

        public AssignmentStatement ParseAssignmentStatement(Node reference) {
            Node leftPart = ParseReference(reference);
            if (leftPart is SubroutineCall) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "variable, array access or record access");
            }
            ParserUtils.RequireLexeme(lexer, ":=");

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
            if (lexer.GetNextLexeme()!.value != "[") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "[");
            }

            List<Nodes.Index> indexes = new List<Nodes.Index>();
            Datatype type = null;

            lexer.GetNextLexeme();
            do {
                Constant start = null;
                Constant end = null;

                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.INTEGER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "start index");
                }
                start = new Constant(lexer.curLexeme!);
                lexer.GetNextLexeme();

                ParserUtils.RequireLexeme(lexer, "..");

                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.INTEGER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "start index");
                }
                end = new Constant(lexer.curLexeme!);
                lexer.GetNextLexeme();

                indexes.Add(new Nodes.Index(start, end));

                if (lexer.curLexeme!.value == ",") {
                    lexer.GetNextLexeme();
                }
            } while (lexer.curLexeme!.value != "]");

            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, "of");

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.value != "array" && lexer.curLexeme!.value != "record") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "array datatype");
            }

            type = ParseDatatype();

            return new ArrayDatatype(type, indexes);
        }

        public RecordDatatype ParseRecordDatatype() {
            List<NewField> fields = new List<NewField>();

            lexer.GetNextLexeme();
            while (lexer.curLexeme!.value != "end") {
                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }
                fields.Add(ParseNewField());

                if (lexer.curLexeme!.value != "end" && lexer.curLexeme!.value != ";") {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }

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
                ParserUtils.RequireLexeme(lexer, ",");

                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }

                names.Add(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            lexer.GetNextLexeme();
            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.value != "array" && lexer.curLexeme!.value != "record") {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field datatype");
            }

            type = ParseDatatype();

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
                ParserUtils.RequireLexeme(lexer, ")");
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "factor");
            }

            return new Factor(value);
        }
    }
}
