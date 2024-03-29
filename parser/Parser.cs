﻿using PascalCompiler.Parser.Nodes;
using System.ComponentModel.DataAnnotations;

namespace PascalCompiler.Parser {
    public class Parser {
        Lexer.Lexer lexer;
        public Parser(Lexer.Lexer lexer) {
            this.lexer = lexer;
            //lexer.GetNextLexeme();
        }

        public Program ParseProgram() {
            lexer.GetNextLexeme();
            OptionalBlock? optBlock = ParseOptionalBlock();
            lexer.GetNextLexeme();
            Program node = new Program(optBlock, ParseBlock());
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.POINT);
            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme.lineNumber, lexer.curLexeme.charNumber, "end of file");
            }
            return node;
        }

        public OptionalBlock? ParseOptionalBlock() {
            ProgramName? name = null;
            List<DeclarationSection> decls = new List<DeclarationSection>();
            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.PROGRAM) {
                name = ParseProgramName();
            }

            while ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.TYPE, CommonConstants.ServiceWords.PROCEDURE, CommonConstants.ServiceWords.FUNCTION }).Contains(lexer.curLexeme!.subtype)) {
                decls.Add(ParseDeclarationSection());
            }

            if (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.BEGIN) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme.lineNumber, lexer.curLexeme.charNumber, "begin");
            }

            if (name == null && decls.Count == 0) {
                return null;
            }
            return new OptionalBlock(name, decls.Count == 0 ? null : decls);
        }

        public ProgramName? ParseProgramName() {
            lexer.GetNextLexeme();
            ProgramName? name = null;
            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                name = new ProgramName(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
                ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "program name");
            }
            return name;
        }

        public DeclarationSection ParseDeclarationSection() {
            switch (lexer.curLexeme!.subtype) {
                case CommonConstants.ServiceWords.CONST:
                    return ParseConstants();
                case CommonConstants.ServiceWords.VAR:
                    return ParseVariables();
                case CommonConstants.ServiceWords.TYPE:
                    return ParseTypes();
                case CommonConstants.ServiceWords.PROCEDURE:
                    return ParseProcedures();
                case CommonConstants.ServiceWords.FUNCTION:
                    return ParseFunctions();
                default:
                    ExceptionHandler.Throw(Exceptions.UnknownError, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber);
                    break;
            }
            return null!;
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
            return null!;
        }

        public NewConstant ParseNewConstant() {
            Identifier constName = new Identifier(lexer.curLexeme!);
            BaseDatatype? constType = null;
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COLON) {
                if (lexer.GetNextLexeme()!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
                }

                constType = new BaseDatatype(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.EQUAL);
            Expression value = ParseExpression();
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);

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
            List<Identifier> varNames = new List<Identifier>() { new Identifier(lexer.curLexeme!) };
            Expression? value = null;
            lexer.GetNextLexeme();

            while (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COMMA) {
                lexer.GetNextLexeme();
                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "variable name");
                }
                varNames.Add(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.COLON);

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.ARRAY && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RECORD) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
            }

            Datatype varType = ParseDatatype();

            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.EQUAL) {
                if (varNames.Count > 1) {
                    ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "=");
                }
                lexer.GetNextLexeme();
                value = ParseExpression();
            }

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);

            return new NewVariable(varNames, varType, value);
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
            lexer.GetNextLexeme();
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.EQUAL);

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.ARRAY && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RECORD) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
            }

            Datatype oldTypeName = ParseDatatype();
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);

            return new NewType(newTypeName, oldTypeName);
        }

        public Procedures ParseProcedures() {
            List<NewProcedure> procList = new List<NewProcedure>();

            while (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.PROCEDURE) {
                procList.Add(ParseNewProcedure());
            }

            return new Procedures(procList);
        }

        public NewProcedure ParseNewProcedure() {
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure name");
            }
            Identifier procName = new Identifier(lexer.curLexeme!);
            lexer.GetNextLexeme();
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.LEFT_ROUND_BRACKET);
            SubroutineArgs args = ParseSubroutineArgs(false);

            if (!(new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.TYPE, CommonConstants.ServiceWords.BEGIN }).Contains(lexer.curLexeme!.subtype)) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "procedure body");
            }

            SubroutineBody body = ParseSubroutineBody();
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);

            return new NewProcedure(procName, args, body);
        }

        public SubroutineArgs ParseSubroutineArgs(bool isFunction) {
            List<NewSubroutineArg> argsList= new List<NewSubroutineArg>();

            if ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.OUT }).Contains(lexer.curLexeme!.subtype) || lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                argsList.Add(ParseNewSubroutineArg());
                while (lexer.curLexeme!.value == ";") {
                    lexer.GetNextLexeme();
                    if (!(new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.OUT }).Contains(lexer.curLexeme!.subtype) && lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg");
                    }

                    argsList.Add(ParseNewSubroutineArg());
                }
            }

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET);

            if (!(lexer.curLexeme!.subtype == CommonConstants.ServiceWords.SEMICOLON && !isFunction || lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COLON && isFunction)) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, isFunction ? ":" : ";");
            }
            lexer.GetNextLexeme();
            
            return new SubroutineArgs(argsList);
        }

        public NewSubroutineArg ParseNewSubroutineArg() {
            ArgModifier? mod = null;
            List<Identifier> argNames = new List<Identifier>();
            Node type = null!;

            if ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.OUT }).Contains(lexer.curLexeme!.subtype)) {
                mod = new ArgModifier(lexer.curLexeme!.value!);
                lexer.GetNextLexeme();
            }

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg name");
            }
            argNames.Add(new Identifier(lexer.curLexeme!));
            lexer.GetNextLexeme();

            while (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COMMA) {
                if (lexer.GetNextLexeme()!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg name");
                }

                argNames.Add(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.COLON);


            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                type = new BaseDatatype(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.ARRAY) {
                type = ParseArraySubroutineArg();
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "arg datatype");
            }

            return new NewSubroutineArg(mod, argNames, type);

        }

        public ArraySubroutineArg ParseArraySubroutineArg() {
            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.OF);

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "datatype");
            }

            BaseDatatype type = new BaseDatatype(new Identifier(lexer.curLexeme!));

            return new ArraySubroutineArg(type);
        }

        public SubroutineBody ParseSubroutineBody() {
            List<DeclarationSection>? decls = new List<DeclarationSection>();

            while ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.TYPE }).Contains(lexer.curLexeme!.subtype)) {
                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.CONST) {
                    decls.Add(ParseConstants());
                }
                else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.VAR) {
                    decls.Add(ParseVariables());
                }
                else {
                    decls.Add(ParseTypes());
                }
            }

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.BEGIN);

            Block body = ParseBlock();

            return new SubroutineBody(decls, body);
        }

        public Functions ParseFunctions() {
            List<NewFunction> funcList = new List<NewFunction>();
            while (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.FUNCTION) {
                funcList.Add(ParseNewFunction());
            }
            return new Functions(funcList);
        }

        public NewFunction ParseNewFunction() {
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "function name");
            }

            Identifier funcName = new Identifier(lexer.curLexeme!);

            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.LEFT_ROUND_BRACKET);

            SubroutineArgs args = ParseSubroutineArgs(true);

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "return datatype");
            }
            BaseDatatype returnType = new BaseDatatype(new Identifier(lexer.curLexeme!));
            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);

            if (!(new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.CONST, CommonConstants.ServiceWords.VAR, CommonConstants.ServiceWords.TYPE, CommonConstants.ServiceWords.BEGIN }).Contains(lexer.curLexeme!.subtype)) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "function body");
            }

            SubroutineBody body = ParseSubroutineBody();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);

            return new NewFunction(funcName, args, returnType, body);
        }

        public Block ParseBlock() {
            List<Statement> statements = new List<Statement>();
            
            while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.END && lexer.curLexeme!.type != Lexer.Constants.LexemeType.EOF) {
                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.SEMICOLON) {
                    statements.Add(new EmptyStatement());
                }
                else {
                    statements.Add(ParseStatement());
                }

                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.END) break;
                else ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);
            }
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.EOF) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "end");
            }

            return new Block(statements);
        }

        public Statement ParseStatement() {
            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.IF) {
                return ParseIfStatement();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.WHILE) {
                return ParseWhileStatement();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.REPEAT) {
                return ParseRepeatStatement();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.FOR) {
                return ParseForStatement();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.BEGIN) {
                lexer.GetNextLexeme();
                return ParseBlock();
            }
            else if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                Node node = ParseReferenceOrSubroutineCall();

                if (node is SubroutineCall) {
                    return (SubroutineCall)node;
                }
                else if (node is Identifier && (new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.SEMICOLON, CommonConstants.ServiceWords.END, CommonConstants.ServiceWords.UNTIL, CommonConstants.ServiceWords.ELSE }).Contains(lexer.curLexeme!.subtype)) {
                    return new SubroutineCall((Identifier)node, null);
                }
                else {
                    return ParseAssignmentStatement((Reference)node);
                }
            }

            ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "statement");

            return null!;
        }

        public Node ParseReferenceOrSubroutineCall() {
            Identifier name = new Identifier(lexer.curLexeme!);
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.LEFT_ROUND_BRACKET) {
                lexer.GetNextLexeme();
                List<Expression> args = new List<Expression>();
                while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET) {
                    args.Add(ParseExpression(true));
                    if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COMMA) lexer.GetNextLexeme();
                    else if (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET) {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ")");
                    }
                }
                lexer.GetNextLexeme();
                return new SubroutineCall(name, args);
            }

            Reference reference = name;
            while (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.POINT || lexer.curLexeme!.subtype == CommonConstants.ServiceWords.LEFT_SQUARE_BRACKET) {
                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.POINT) {
                    lexer.GetNextLexeme();
                    if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                    }

                    Identifier field = new Identifier(lexer.curLexeme!);
                    lexer.GetNextLexeme();
                    reference =  new RecordAccess(name, field);
                }
                else {
                    lexer.GetNextLexeme();
                    List<Expression> indexes = new List<Expression>() { ParseExpression() };

                    while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RIGHT_SQUARE_BRACKET) {
                        if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COMMA) lexer.GetNextLexeme();
                        else if (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RIGHT_SQUARE_BRACKET) {
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
            Statement? falseStatement = null;

            lexer.GetNextLexeme();

            Expression condition = ParseExpression();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.THEN);

            Statement trueStatement = ParseStatement();

            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.ELSE) {
                lexer.GetNextLexeme();
                falseStatement = ParseStatement();
            }

            return new IfStatement(condition, trueStatement, falseStatement);
        }

        public WhileStatement ParseWhileStatement() {

            lexer.GetNextLexeme();

            Expression condition = ParseExpression();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.DO);

            Statement body = ParseStatement();

            return new WhileStatement(condition, body);
        }

        public RepeatStatement ParseRepeatStatement() {
            List<Statement> statements = new List<Statement>();

            lexer.GetNextLexeme();

            while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.UNTIL) {
                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.SEMICOLON) {
                    statements.Add(new EmptyStatement());
                }
                else {
                    statements.Add(ParseStatement());
                }
                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.UNTIL) break;
                else ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.SEMICOLON);
            }
            lexer.GetNextLexeme();

            Expression condition = ParseExpression();

            return new RepeatStatement(condition, statements);
        }

        public ForStatement ParseForStatement() {
            lexer.GetNextLexeme();

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "counter name");
            }
            Identifier counter = new Identifier(lexer.curLexeme!);
            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.ASSIGN);
            Expression start = ParseExpression();

            if (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.TO && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.DOWNTO) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "to / downto");
            }
            Lexer.Lexeme direction = lexer.curLexeme!;
            lexer.GetNextLexeme();

            Expression end = ParseExpression();
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.DO);

            Statement body = ParseStatement();

            return new ForStatement(counter, start, direction, end, body);
        }

        public AssignmentStatement ParseAssignmentStatement(Reference reference) {
            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.ASSIGN);

            return new AssignmentStatement(reference, ParseExpression());
        }

        public Datatype ParseDatatype() {
            if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                Identifier type = new Identifier(lexer.curLexeme!);
                lexer.GetNextLexeme();
                return new BaseDatatype(type);
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.ARRAY) {
                return ParseArrayDatatype();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.RECORD) {
                return ParseRecordDatatype();
            }
            return null!;
        }

        public ArrayDatatype ParseArrayDatatype() {
            if (lexer.GetNextLexeme()!.subtype != CommonConstants.ServiceWords.LEFT_SQUARE_BRACKET) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "[");
            }

            List<Nodes.Index> indexes = new List<Nodes.Index>();

            lexer.GetNextLexeme();
            do {
                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.INTEGER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "start index");
                }
                Constant start = new Constant(lexer.curLexeme!);
                lexer.GetNextLexeme();

                ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.ELLIPSIS);

                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.INTEGER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "start index");
                }
                Constant end = new Constant(lexer.curLexeme!);
                lexer.GetNextLexeme();

                indexes.Add(new Nodes.Index(start, end));

                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.COMMA) {
                    lexer.GetNextLexeme();
                }
            } while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RIGHT_SQUARE_BRACKET);

            lexer.GetNextLexeme();

            ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.OF);

            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.ARRAY && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RECORD) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "array datatype");
            }

            Datatype type = ParseDatatype();

            return new ArrayDatatype(type, indexes);
        }

        public RecordDatatype ParseRecordDatatype() {
            List<NewField> fields = new List<NewField>();

            lexer.GetNextLexeme();
            while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.END) {
                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }
                fields.Add(ParseNewField());

                if (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.END && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.SEMICOLON) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ";");
                }

                if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.SEMICOLON) lexer.GetNextLexeme();
            }
            lexer.GetNextLexeme();
            return new RecordDatatype(fields);
        }

        public NewField ParseNewField() {
            List<Identifier> names = new List<Identifier>() { new Identifier(lexer.curLexeme!) };

            lexer.GetNextLexeme();

            while (lexer.curLexeme!.subtype != CommonConstants.ServiceWords.COLON) {
                ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.COMMA);

                if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER) {
                    ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field name");
                }

                names.Add(new Identifier(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            lexer.GetNextLexeme();
            if (lexer.curLexeme!.type != Lexer.Constants.LexemeType.IDENTIFIER && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.ARRAY && lexer.curLexeme!.subtype != CommonConstants.ServiceWords.RECORD) {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "field datatype");
            }

            Datatype type = ParseDatatype();

            return new NewField(names, type);
        }

        public Expression ParseExpression(bool isInBrackets = false) {
            SimpleExpression leftComparingOperand = ParseSimpleExpression();
            SimpleExpression? rightComparingOperand = null;
            CompareOperator? compareOperator = null;

            if ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.GREATER, CommonConstants.ServiceWords.LESSER, CommonConstants.ServiceWords.EQUAL, CommonConstants.ServiceWords.NOT_EQUAL, CommonConstants.ServiceWords.GREATER_OR_EQUAL, CommonConstants.ServiceWords.LESSER_OR_EQUAL }).Contains(lexer.curLexeme!.subtype)) {
                compareOperator = new CompareOperator(lexer.curLexeme!);
                lexer.GetNextLexeme();
                rightComparingOperand = ParseSimpleExpression();
            }

            if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET && !isInBrackets) {
                ExceptionHandler.Throw(Exceptions.UnexpectedCharacter, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, ")");
            }

            Expression ex = new Expression(leftComparingOperand, rightComparingOperand, compareOperator);
            ex.isVariable = rightComparingOperand == null ? leftComparingOperand.isVariable : false;
            return ex;
        }

        public SimpleExpression ParseSimpleExpression() {
            SimpleExpression? se = new SimpleExpression(null, null, ParseTerm());
            AddOperator? op = null;
            while ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.PLUS, CommonConstants.ServiceWords.MINUS, CommonConstants.ServiceWords.OR, CommonConstants.ServiceWords.XOR }).Contains(lexer.curLexeme!.subtype)) {
                op = new AddOperator(lexer.curLexeme!);
                lexer.GetNextLexeme();
                se = new SimpleExpression(se, op, ParseTerm());
            }

            se.isVariable = se.left == null ? se.right.isVariable : false;
            return se;
        }

        public Term ParseTerm() {
            MultiplyOperator? op = null;
            Term? t = new Term(null, null, ParseSimpleTerm());

            if ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.MULTIPLY, CommonConstants.ServiceWords.DIVIDE, CommonConstants.ServiceWords.DIV, CommonConstants.ServiceWords.MOD, CommonConstants.ServiceWords.AND }).Contains(lexer.curLexeme!.subtype)) {
                op = new MultiplyOperator(lexer.curLexeme!);
                lexer.GetNextLexeme();
                t = new Term(t, op, ParseSimpleTerm());
            }

            t.isVariable = t.left == null ? t.right.isVariable : false;
            return t;
        }

        public SimpleTerm ParseSimpleTerm() {
            List<UnaryOperator>? unaryOperators = new List<UnaryOperator>();

            while ((new CommonConstants.ServiceWords[] { CommonConstants.ServiceWords.PLUS, CommonConstants.ServiceWords.MINUS, CommonConstants.ServiceWords.NOT }).Contains(lexer.curLexeme!.subtype)) {
                unaryOperators.Add(new UnaryOperator(lexer.curLexeme!));
                lexer.GetNextLexeme();
            }

            SimpleTerm st = new SimpleTerm(unaryOperators, ParseFactor());
            st.isVariable = unaryOperators.Count == 0 ? st.factor.isVariable : false;
            return st;
        }

        public Factor ParseFactor() {
            Node value = null!;

            if ((new Lexer.Constants.LexemeType[] { Lexer.Constants.LexemeType.REAL, Lexer.Constants.LexemeType.STRING, Lexer.Constants.LexemeType.INTEGER }).Contains(lexer.curLexeme!.type)) {
                value = new Constant(lexer.curLexeme!);

                lexer.GetNextLexeme();
            }
            else if (lexer.curLexeme!.type == Lexer.Constants.LexemeType.IDENTIFIER) {
                value = ParseReferenceOrSubroutineCall();
            }
            else if (lexer.curLexeme!.subtype == CommonConstants.ServiceWords.LEFT_ROUND_BRACKET) {
                lexer.GetNextLexeme();
                value = ParseExpression(true);
                ParserUtils.RequireLexeme(lexer, CommonConstants.ServiceWords.RIGHT_ROUND_BRACKET);
            }
            else {
                ExceptionHandler.Throw(Exceptions.ExpectedCharacters, lexer.curLexeme!.lineNumber, lexer.curLexeme!.charNumber, "factor");
            }

            Factor f = new Factor(value!);
            f.isVariable = value is Identifier;
            return f;
        }
    }
}
