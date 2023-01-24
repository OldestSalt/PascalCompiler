using PascalCompiler.Parser.Nodes;
using PascalCompiler.Semantic.Symbols;

namespace PascalCompiler.Semantic {
    public class SymVisitor {
        private SymStack symStack;
        public SymVisitor() {
            symStack = new SymStack();
        }

        public void Print() {
            symStack.Print();
        }
        public void VisitProgram(Program node) {
            symStack.pushSym(new SymTypeInteger("integer"));
            symStack.pushSym(new SymType("string"));
            symStack.pushSym(new SymTypeReal("real"));

            symStack.pushSym(new SymWrite("writeln"));
            symStack.pushSym(new SymWrite("write"));
            symStack.pushSym(new SymRead("readln"));
            symStack.pushSym(new SymRead("read"));

            
            //symStack.pushSym(new SymFunc("trunc", new List<SymVarParam>() { new SymVarParam("r", new SymTypeReal("real"), null) }, new SymTypeInteger("integer")));
            //symStack.pushSym(new SymFunc("round", new List<SymVarParam>() { new SymVarParam("r", new SymTypeReal("real"), null) }, new SymTypeInteger("integer")));
            symStack.pushScope();

            if (node.optionalBlock != null) VisitOptionalBlock(node.optionalBlock);
            VisitBlock(node.mainBlock);
        }
        public void VisitOptionalBlock(OptionalBlock node) {
            if (node.declarations != null) {
                foreach (var decl in node.declarations) {
                    VisitDeclarationSection(decl);
                }
            }
        }
        public void VisitBlock(Block node) {
            if (node.statements != null) {
                foreach (var statement in node.statements) {
                    VisitStatement(statement);
                }
            }
        }
        public void VisitProgramName(ProgramName node) { }
        public void VisitDeclarationSection(DeclarationSection node) {
            if (node is Constants) VisitConstants((Constants)node);
            else if (node is Variables) VisitVariables((Variables)node);
            else if (node is Types) VisitTypes((Types)node);
            else if (node is Procedures) VisitProcedures((Procedures)node);
            else if (node is Functions) VisitFunctions((Functions)node);
        }
        public void VisitConstants(Constants node) {
            foreach (var constant in node.constants) {
                VisitNewConstant(constant);
            }
        }
        public void VisitNewConstant(NewConstant node) {

            if (symStack.checkSymInScope(node.name.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, node.name.lexeme.lineNumber, node.name.lexeme.charNumber, node.name.lexeme!.value!);
            }

            VisitExpression(node.value);
            if (node.type != null) {
                VisitBaseDatatype(node.type);
                if (node.type.symType!.name != node.value.symType!.name && !(node.type.symType!.name == "real" || node.value.symType!.name == "integer")) {
                    ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.value.lineNumber, node.value.charNumber, node.type.symType!.name, node.value.symType!.name);
                }
            }

            symStack.pushSym(new SymVarConst(node.name.lexeme!.value!, node.value.symType!));
        }
        public void VisitVariables(Variables node) {
            foreach (var variable in node.variables) {
                VisitNewVariable(variable);
            }
        }
        public void VisitNewVariable(NewVariable node) {
            VisitDatatype(node.type);
            foreach (var name in node.names) {
                if (symStack.checkSymInScope(name.lexeme!.value!)) {
                    ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, name.lexeme.lineNumber, name.lexeme.charNumber, name.lexeme!.value!);
                }
                if (node.value != null) {
                    VisitExpression(node.value);
                    if (node.type.symType!.name != node.value.symType!.name && !(node.type.symType!.name == "real" || node.value.symType!.name == "integer")) {
                        ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.value.lineNumber, node.value.charNumber, node.type.symType!.name, node.value.symType!.name);
                    }
                }
                symStack.pushSym(new SymVar(name.lexeme!.value!, node.type.symType!));
            }

        }
        public void VisitTypes(Types node) {
            foreach (var type in node.types) {
                VisitNewType(type);
            }
        }
        public void VisitNewType(NewType node) {

            if (symStack.checkSymInScope(node.name.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, node.name.lexeme.lineNumber, node.name.lexeme.charNumber, node.name.lexeme!.value!);
            }

            VisitDatatype(node.type);
            symStack.pushSym(new SymTypeAlias(node.type.symType!, node.name.lexeme!.value!));
        }
        public void VisitProcedures(Procedures node) {
            foreach (var procedure in node.procedures) {
                VisitNewProcedure(procedure);
            }
        }
        public void VisitNewProcedure(NewProcedure node) {
            if (symStack.checkSymInScope(node.name.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, node.name.lexeme.lineNumber, node.name.lexeme.charNumber, node.name.lexeme!.value!);
            }

            symStack.pushScope();

            VisitSubroutineArgs(node.args);
            VisitSubroutineBody(node.body);

            symStack.popScope();
            symStack.pushSym(new SymProc(node.name.lexeme.value!, node.args.sym));
        }
        public void VisitFunctions(Functions node) {
            foreach (var function in node.functions) {
                VisitNewFunction(function);
            }
        }
        public void VisitNewFunction(NewFunction node) {
            if (symStack.checkSymInScope(node.name.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, node.name.lexeme.lineNumber, node.name.lexeme.charNumber, node.name.lexeme!.value!);
            }

            symStack.pushScope();

            VisitBaseDatatype(node.returnType);

            symStack.pushSym(new SymVar(node.name.lexeme.value!, node.returnType.symType!));

            VisitSubroutineArgs(node.args);
            
            VisitSubroutineBody(node.body);

            symStack.popScope();
            symStack.pushSym(new SymFunc(node.name.lexeme.value!, node.args.sym, node.returnType.symType!));
        }
        public void VisitSubroutineBody(SubroutineBody node) {
            if (node.decls != null) {
                foreach (var decl in node.decls) {
                    VisitDeclarationSection(decl);
                }
            }

            VisitBlock(node.body);
        }
        public void VisitSubroutineArgs(SubroutineArgs node) {
            if (node.args != null) {
                foreach (var arg in node.args) {
                    VisitNewSubroutineArg(arg);

                    node.sym.AddRange(arg.sym);
                }
            }
        }
        public void VisitNewSubroutineArg(NewSubroutineArg node) {
            CommonConstants.ServiceWords? modifier = null;
            if (node.modifier != null) {
                modifier = Lexer.LexerUtils.GetEnumValue(node.modifier.value);
            }

            if (node.type is BaseDatatype) VisitBaseDatatype((BaseDatatype)node.type);
            else if (node.type is ArraySubroutineArg) VisitArraySubroutineArg((ArraySubroutineArg)node.type);

            foreach (var name in node.names) {
                if (symStack.checkSymInScope(name.lexeme!.value!)) {
                    ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, name.lexeme.lineNumber, name.lexeme.charNumber, name.lexeme!.value!);
                }

                SymVarParam arg = null!;
                if (node.type is BaseDatatype) arg = new SymVarParam(name.lexeme!.value!, ((BaseDatatype)node.type).symType!, modifier);
                else if (node.type is ArraySubroutineArg) arg = new SymVarParam(name.lexeme!.value!, ((ArraySubroutineArg)node.type).symType!, modifier);

                symStack.pushSym(arg);
                node.sym.Add(arg);
            }
        }
        public void VisitArgModifier(ArgModifier node) { }
        public void VisitArraySubroutineArg(ArraySubroutineArg node) {
            VisitBaseDatatype(node.type);
            node.symType = new SymTypeArray($"array of {node.type.symType!.name}", node.type.symType);
        }
        public void VisitStatement(Statement node) {
            if (node is IfStatement) VisitIfStatement((IfStatement)node);
            else if (node is WhileStatement) VisitWhileStatement((WhileStatement)node);
            else if (node is RepeatStatement) VisitRepeatStatement((RepeatStatement)node);
            else if (node is ForStatement) VisitForStatement((ForStatement)node);
            else if (node is Block) VisitBlock((Block)node);
            else if (node is AssignmentStatement) VisitAssignmentStatement((AssignmentStatement)node);
            else if (node is SubroutineCall) VisitSubroutineCall((SubroutineCall)node);
        }
        public void VisitEmptyStatement(EmptyStatement node) { }
        public void VisitAssignmentStatement(AssignmentStatement node) {
            VisitReference(node.leftPart, true);
            VisitExpression(node.rightPart);
            if (node.leftPart.symType?.name != node.rightPart.symType?.name && !(node.leftPart.symType?.name == "real" && node.rightPart.symType?.name == "integer")) {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.rightPart.lineNumber, node.rightPart.charNumber, node.leftPart.symType?.name!, node.rightPart.symType?.name!);
            }
        }
        public void VisitIfStatement(IfStatement node) {
            VisitExpression(node.condition);
            if (node.condition.symType?.name != "integer") {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.condition.lineNumber, node.condition.charNumber, "boolean", node.condition.symType?.name!);
            }

            VisitStatement(node.trueStatement);
            if (node.falseStatement != null) {
                VisitStatement(node.falseStatement);
            }
        }
        public void VisitWhileStatement(WhileStatement node) {
            VisitExpression(node.condition);
            if (node.condition.symType?.name != "integer") {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.condition.lineNumber, node.condition.charNumber, "boolean", node.condition.symType?.name!);
            }

            VisitStatement(node.body);
        }
        public void VisitRepeatStatement(RepeatStatement node) {
            VisitExpression(node.condition);
            if (node.condition.symType?.name != "integer") {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.condition.lineNumber, node.condition.charNumber, "boolean", node.condition.symType?.name!);
            }

            if (node.body != null) {
                foreach (var statement in node.body) {
                    VisitStatement(statement);
                }
            }
        }
        public void VisitForStatement(ForStatement node) {
            if (!symStack.checkSymInScope(node.counter.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.UnknownIdentifier, node.counter.lexeme.lineNumber, node.counter.lexeme.charNumber, node.counter.lexeme!.value!);
            }

            Symbol? counter = symStack.getSym(node.counter.lexeme!.value!);
            if (counter is not SymVar) {
                ExceptionHandler.Throw(Exceptions.NotAVar, node.counter.lexeme.lineNumber, node.counter.lexeme.charNumber);
            }

            if (((SymVar)counter!).type.name != "integer") {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.counter.lexeme.lineNumber, node.counter.lexeme.charNumber, "integer", ((SymVar)counter).type.name!);
            }

            VisitExpression(node.start);

            if (node.start.symType is not SymTypeInteger) {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.start.lineNumber, node.start.charNumber, "integer", node.start.symType!.name);
            }

            VisitExpression(node.end);

            if (node.end.symType is not SymTypeInteger) {
                ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.end.lineNumber, node.end.charNumber, "integer", node.end.symType!.name);
            }

            VisitStatement(node.body);
        }
        public void VisitSubroutineCall(SubroutineCall node) {
            if (!symStack.checkSym(node.name.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.UnknownSubroutine, node.name.lexeme!.lineNumber, node.name.lexeme!.charNumber);
            }

            Symbol subroutine = symStack.getSym(node.name.lexeme!.value!)!;
            if (subroutine is SymProc) {
                SymProc procedure = (SymProc)symStack.getSym(node.name.lexeme!.value!)!;
                if (procedure.args.Count != node.args.Count) {
                    ExceptionHandler.Throw(Exceptions.IncorrectParameters, node.name.lexeme!.lineNumber, node.name.lexeme!.charNumber);
                }

                for (int i = 0; i < node.args.Count; i++) {
                    VisitExpression(node.args[i]);
                    if (node.args[i].symType!.name != procedure.args[i].type.name && !(node.args[i].symType!.name == "integer" && procedure.args[i].type.name == "real")) {
                        ExceptionHandler.Throw(Exceptions.IncorrectParameters, node.args[i].lineNumber, node.args[i].charNumber);
                    }

                    if ((procedure.args[i].modifier == CommonConstants.ServiceWords.VAR || procedure.args[i].modifier == CommonConstants.ServiceWords.OUT) && !node.args[i].isVariable) {
                        ExceptionHandler.Throw(Exceptions.ExpectedCharacters, node.args[i].lineNumber, node.args[i].charNumber, "variable");
                    }
                }

                if (procedure is SymFunc) node.symType = ((SymFunc)procedure).returnType;
            }
            else if (subroutine is SymWrite || subroutine is SymRead) {
                for (int i = 0; i < node.args.Count; i++) {
                    VisitExpression(node.args[i]);
                    if (!new string[] { "integer", "real", "string" }.Contains(node.args[i].symType!.name)) {
                        ExceptionHandler.Throw(Exceptions.IncorrectParameters, node.args[i].lineNumber, node.args[i].lexeme!.charNumber);
                    }
                }
            }
        }
        public void VisitExpression(Expression node) {
            VisitSimpleExpression(node.leftComparingOperand);
            if (node.rightComparingOperand != null) {
                VisitSimpleExpression(node.rightComparingOperand);
                SymType leftType = node.leftComparingOperand.symType!;
                SymType rightType = node.rightComparingOperand.symType!;
                if (node.compareOperator!.lexeme!.subtype == CommonConstants.ServiceWords.EQUAL || node.compareOperator.lexeme.subtype == CommonConstants.ServiceWords.NOT_EQUAL) {
                    if (leftType is not SymTypeScalar && rightType is not SymTypeScalar || leftType.name != rightType.name) {
                        ExceptionHandler.Throw(Exceptions.IncomparableTypes, node.compareOperator!.lexeme!.lineNumber, node.compareOperator!.lexeme!.charNumber);
                    }
                }
                else {
                    if (leftType is not SymTypeScalar && rightType is not SymTypeScalar) {
                        ExceptionHandler.Throw(Exceptions.IncomparableTypes, node.compareOperator!.lexeme!.lineNumber, node.compareOperator!.lexeme!.charNumber);
                    }
                }
                node.symType = new SymTypeInteger("integer");
            }
            else {
                node.symType = node.leftComparingOperand.symType;
            }

            node.lineNumber = node.leftComparingOperand.lineNumber;
            node.charNumber = node.leftComparingOperand.charNumber;
        }
        public void VisitSimpleExpression(SimpleExpression node) {
            if (node.left != null) {
                VisitSimpleExpression(node.left);
                VisitTerm(node.right);
                SymType leftType = node.left.symType!;
                SymType rightType = node.right.symType!;

                switch (node.addOperator!.lexeme!.subtype) {
                    case CommonConstants.ServiceWords.PLUS:
                        if ((leftType is not SymTypeScalar || rightType is not SymTypeScalar) && !(leftType.name == "string" && rightType.name == "string")) {
                            ExceptionHandler.Throw(Exceptions.OperationError, node.addOperator!.lexeme!.lineNumber, node.addOperator!.lexeme!.charNumber, "Only scalars and strings can be summed up");
                        }
                        break;
                    case CommonConstants.ServiceWords.MINUS:
                        if (leftType is not SymTypeScalar || rightType is not SymTypeScalar) {
                            ExceptionHandler.Throw(Exceptions.OperationError, node.addOperator!.lexeme!.lineNumber, node.addOperator!.lexeme!.charNumber, "Only scalars can be subtracted");
                        }
                        break;
                    case CommonConstants.ServiceWords.OR:
                    case CommonConstants.ServiceWords.XOR:
                        if (leftType is not SymTypeInteger || rightType is not SymTypeInteger) {
                            ExceptionHandler.Throw(Exceptions.OperationError, node.addOperator!.lexeme!.lineNumber, node.addOperator!.lexeme!.charNumber, "Logical operations can be used only with integers");
                        }
                        break;
                }
                
                if (leftType.name == rightType.name && leftType is SymTypeInteger) {
                    node.symType = new SymTypeInteger("integer");
                }
                else if (leftType.name == "string") {
                    node.symType = new SymType("string");
                }
                else {
                    node.symType = new SymTypeReal("real");
                }

                node.lineNumber = node.left.lineNumber;
                node.charNumber = node.left.charNumber;
            }
            else {
                VisitTerm(node.right);
                node.symType = node.right.symType;

                node.lineNumber = node.right.lineNumber;
                node.charNumber = node.right.charNumber;
            }
        }
        public void VisitTerm(Term node) {
            if (node.left != null) {
                VisitTerm(node.left);
                VisitSimpleTerm(node.right);
                SymType leftType = node.left.symType!;
                SymType rightType = node.right.symType!;

                if (node.multiplyOperator!.lexeme!.subtype == CommonConstants.ServiceWords.AND) {
                    if (leftType is not SymTypeInteger || rightType is not SymTypeInteger) {
                        ExceptionHandler.Throw(Exceptions.OperationError, node.multiplyOperator!.lexeme!.lineNumber, node.multiplyOperator!.lexeme!.charNumber, "Logical operations can be used only with integers");
                    }
                }
                else {
                    if (leftType is not SymTypeScalar || rightType is not SymTypeScalar) {
                        ExceptionHandler.Throw(Exceptions.OperationError, node.multiplyOperator!.lexeme!.lineNumber, node.multiplyOperator!.lexeme!.charNumber, "Only scalars can be multiplied or divided");
                    }
                }

                if (leftType.name == rightType.name && leftType.name == "integer") {
                    node.symType = new SymTypeInteger("integer");
                }
                else {
                    node.symType = new SymTypeReal("real");
                }

                node.lineNumber = node.left.lineNumber;
                node.charNumber = node.left.charNumber;
            }
            else {
                VisitSimpleTerm(node.right);
                node.symType = node.right.symType;

                node.lineNumber = node.right.lineNumber;
                node.charNumber = node.right.charNumber;
            }
        }
        public void VisitSimpleTerm(SimpleTerm node) {
            if (node.unaryOperators != null && node.unaryOperators.Count > 0) {
                VisitFactor(node.factor);
                foreach (var op in node.unaryOperators) {
                    if (op.lexeme!.subtype == CommonConstants.ServiceWords.NOT) {
                        if (node.factor.symType is not SymTypeInteger) {
                            ExceptionHandler.Throw(Exceptions.OperationError, op!.lexeme!.lineNumber, op!.lexeme!.charNumber, "Logical operations can be used only with integers");
                        }
                    }
                    else {
                        if (node.factor.symType is not SymTypeScalar) {
                            ExceptionHandler.Throw(Exceptions.OperationError, op!.lexeme!.lineNumber, op!.lexeme!.charNumber, "Unary '+' and '-' can be applied only to scalars");
                        }
                    }
                }

            }

            VisitFactor(node.factor);
            node.symType = node.factor.symType;

            node.lineNumber = node.factor.lineNumber;
            node.charNumber = node.factor.charNumber;
        }
        public void VisitFactor(Factor node) {
            if (node.value is Expression) {
                VisitExpression((Expression)node.value);
                node.lineNumber = ((Expression)node.value).lineNumber;
                node.charNumber = ((Expression)node.value).charNumber;
                node.symType = ((Expression)node.value).symType;
            }
            else if (node.value is Reference) {
                VisitReference((Reference)node.value);
                node.lineNumber = ((Reference)node.value).lineNumber;
                node.charNumber = ((Reference)node.value).charNumber;
                node.symType = ((Reference)node.value).symType;
            }
            else if (node.value is SubroutineCall) {
                VisitSubroutineCall((SubroutineCall)node.value);
                Lexer.Lexeme subroutineName = ((SubroutineCall)node.value).name.lexeme!;
                if (symStack.getSym(subroutineName.value!) is not SymFunc) {
                    ExceptionHandler.Throw(Exceptions.NoReturnValue, subroutineName.lineNumber, subroutineName.charNumber);
                }
                node.lineNumber = ((SubroutineCall)node.value).name.lexeme!.lineNumber;
                node.charNumber = ((SubroutineCall)node.value).name.lexeme!.charNumber;
                node.symType = ((SubroutineCall)node.value).symType;
            }
            else if (node.value is Constant) {
                node.lineNumber = ((Constant)node.value).lexeme!.lineNumber;
                node.charNumber = ((Constant)node.value).lexeme!.charNumber;
                if (((Constant)node.value).lexeme!.type == Lexer.Constants.LexemeType.INTEGER) node.symType = new SymTypeInteger("integer");
                else if (((Constant)node.value).lexeme!.type == Lexer.Constants.LexemeType.REAL) node.symType = new SymTypeReal("real");
                else if (((Constant)node.value).lexeme!.type == Lexer.Constants.LexemeType.STRING) node.symType = new SymType("string");
            }
        }
        public void VisitReference(Reference node, bool isAssignment = false) {
            if (node is ArrayAccess) VisitArrayAccess((ArrayAccess)node);
            else if (node is RecordAccess) VisitRecordAccess((RecordAccess)node);
            else if (node is Identifier) {
                if (!symStack.checkSymInScope(node.lexeme!.value!)) {
                    ExceptionHandler.Throw(Exceptions.UnknownIdentifier, node.lexeme.lineNumber, node.lexeme.charNumber, node.lexeme!.value!);
                }

                Symbol sym = symStack.getSym(node.lexeme!.value!)!;
                if (sym is not SymVar) {
                    ExceptionHandler.Throw(Exceptions.NotAVar, node.lexeme.lineNumber, node.lexeme.charNumber, node.lexeme!.value!);
                }


                if (isAssignment && (sym is SymVarParam && ((SymVarParam)sym).modifier == CommonConstants.ServiceWords.CONST || sym is SymVarConst)) {
                    ExceptionHandler.Throw(Exceptions.ImmutableSymbol, node.lexeme.lineNumber, node.lexeme.charNumber, node.lexeme!.value!);
                }

                node.symType = ((SymVar)symStack.getSym(node.lexeme!.value!)!).type;
                node.lineNumber = ((Identifier)node).lexeme!.lineNumber;
                node.charNumber = ((Identifier)node).lexeme!.charNumber;
            }
        }
        public void VisitArrayAccess(ArrayAccess node) {
            SymType? type;
            if (node.name is Reference) {
                VisitReference((Reference)node.name);
                type = ((Reference)node.name).symType;
                if (node.name is Identifier) {
                    node.lineNumber = ((Identifier)node.name).lexeme!.lineNumber;
                    node.charNumber = ((Identifier)node.name).lexeme!.charNumber;
                }
                else {
                    node.lineNumber = ((Reference)node.name).lineNumber;
                    node.charNumber = ((Reference)node.name).charNumber;
                }
            }
            else {
                VisitSubroutineCall((SubroutineCall)node.name);
                type = ((SubroutineCall)node.name).symType;
                node.lineNumber = ((SubroutineCall)node.name).lineNumber;
                node.charNumber = ((SubroutineCall)node.name).charNumber;
            }
            
            if (type is not SymTypeArray && type!.name != "string") {
                ExceptionHandler.Throw(Exceptions.NotAnArray, node.lineNumber, node.charNumber);
            }

            foreach (var index in node.indexes) {
                VisitExpression(index);
                if (index.symType is not SymTypeInteger) {
                    ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.lineNumber, node.charNumber, "integer", node.symType!.name);
                }
            }

            node.symType = ((SymTypeArray)type!).elemType;
        }
        public void VisitRecordAccess(RecordAccess node) {
            SymType? type;
            if (node.name is Reference) {
                VisitReference((Reference)node.name);
                type = ((Reference)node.name).symType;
                if (node.name is Identifier) {
                    node.lineNumber = ((Identifier)node.name).lexeme!.lineNumber;
                    node.charNumber = ((Identifier)node.name).lexeme!.charNumber;
                }
                else {
                    node.lineNumber = ((Reference)node.name).lineNumber;
                    node.charNumber = ((Reference)node.name).charNumber;
                }
            }
            else {
                VisitSubroutineCall((SubroutineCall)node.name);
                type = ((SubroutineCall)node.name).symType;
                node.lineNumber = ((SubroutineCall)node.name).lineNumber;
                node.charNumber = ((SubroutineCall)node.name).charNumber;
            }

            if (type is not SymTypeRecord) {
                ExceptionHandler.Throw(Exceptions.NotARecord, node.lineNumber, node.charNumber);
            }

            if (!((SymTypeRecord)type!).fields.checkSym(node.field.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.UnknownField, node.field.lexeme.lineNumber, node.field.lexeme.charNumber);
            }

            node.symType = (((SymTypeRecord)type!).fields.getSym(node.field.lexeme!.value!) as SymVar)!.type;
        }
        public void VisitDatatype(Datatype node) {
            if (node is BaseDatatype) VisitBaseDatatype((BaseDatatype)node);
            else if (node is ArrayDatatype) VisitArrayDatatype((ArrayDatatype)node);
            else if (node is RecordDatatype) VisitRecordDatatype((RecordDatatype)node); 
        }
        public void VisitBaseDatatype(BaseDatatype node) {
            if (!symStack.checkSym(node.name.lexeme!.value!)) {
                ExceptionHandler.Throw(Exceptions.UnknownType, node.name.lexeme.lineNumber, node.name.lexeme.charNumber, node.name.lexeme.value!);
            }

            if (symStack.getSym(node.name.lexeme!.value!) is not SymType) {
                ExceptionHandler.Throw(Exceptions.UnknownType, node.name.lexeme.lineNumber, node.name.lexeme.charNumber, node.name.lexeme.value!);
            }

            string typeName = node.name.lexeme!.value!;
            while (symStack.getSym(typeName) is SymTypeAlias) {
                typeName = ((SymTypeAlias)symStack.getSym(typeName)!).refType.name;
            }
            
            if (typeName == "integer") {
                node.symType = new SymTypeInteger("integer");
            }
            else if (typeName == "real") {
                node.symType = new SymTypeReal("real");
            }
            else if (typeName == "string") {
                node.symType = new SymType("string");
            }
        }
        public void VisitArrayDatatype(ArrayDatatype node) {
            VisitDatatype(node.type);

            List<string> sizes = new List<string>();
            foreach (var size in node.sizes) {
                sizes.Add($"{size.start.lexeme!.value}..{size.end.lexeme!.value}");
            }

            node.symType = new SymTypeArray($"array [{string.Join(", ", sizes)}] of {node.type.symType!.name}", node.type.symType!);
        }
        public void VisitIndex(Parser.Nodes.Index node) { }
        public void VisitRecordDatatype(RecordDatatype node) {
            SymTable fields = new SymTable();
            foreach (var field in node.fields) {
                VisitDatatype(field.type);
                foreach (var name in field.names) {
                    if (fields.checkSym(name.lexeme!.value!)) {
                        ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, name.lexeme.lineNumber, name.lexeme.charNumber, name.lexeme.value!);
                    }
                    fields.pushSym(new SymVar(name.lexeme.value!, field.type.symType!));
                }
            }
            node.symType = new SymTypeRecord("record", fields);
        }
        public void VisitNewField(NewField node) { }
        public void VisitCompareOperator(CompareOperator node) { }
        public void VisitAddOperator(AddOperator node) { }
        public void VisitMultiplyOperator(MultiplyOperator node) { }
        public void VisitIdentifier(Identifier node) { }
        public void VisitConstant(Constant node) { }
    }
}
