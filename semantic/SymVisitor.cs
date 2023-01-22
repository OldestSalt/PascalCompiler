using PascalCompiler.Parser.Nodes;
using PascalCompiler.Semantic.Symbols;

namespace PascalCompiler.Semantic {
    public class SymVisitor {
        private SymStack symStack;
        public SymVisitor() {
            symStack = new SymStack();
        }
        public void VisitProgram(Program node) {
            symStack.pushSym(new SymTypeInteger("integer"));
            symStack.pushSym(new SymType("string"));
            symStack.pushSym(new SymTypeReal("real"));
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
        public void VisitProcedures(Procedures node) { }
        public void VisitNewProcedure(NewProcedure node) { }
        public void VisitFunctions(Functions node) { }
        public void VisitNewFunction(NewFunction node) { }
        public void VisitSubroutineBody(SubroutineBody node) { } //scope +1 here!
        public void VisitSubroutineArgs(SubroutineArgs node) { }
        public void VisitNewSubroutineArg(NewSubroutineArg node) { }
        public void VisitArgModifier(ArgModifier node) { }
        public void VisitArraySubroutineArg(ArraySubroutineArg node) { }
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
            VisitReference(node.leftPart);
            VisitExpression(node.rightPart);
            if (node.leftPart.symType?.name != node.rightPart.symType?.name) {
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
        public void VisitSubroutineCall(SubroutineCall node) { }
        public void VisitExpression(Expression node) {
            VisitSimpleExpression(node.leftComparingOperand);
            if (node.rightComparingOperand != null) {
                VisitSimpleExpression(node.rightComparingOperand);
                string leftType = node.leftComparingOperand.symType?.name!;
                string rightType = node.rightComparingOperand.symType?.name!;
                if (leftType != rightType || (leftType == "integer" || leftType == "real") && rightType == "string" || (rightType == "integer" || rightType == "real") && leftType == "string") {
                    ExceptionHandler.Throw(Exceptions.IncompatibleTypes, node.compareOperator!.lexeme!.lineNumber, node.compareOperator!.lexeme!.charNumber, leftType!, rightType!);
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
                if (symStack.getSym(subroutineName.value!) is SymProc) {
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
        public void VisitReference(Reference node) {
            if (node is ArrayAccess) VisitArrayAccess((ArrayAccess)node);
            else if (node is RecordAccess) VisitRecordAccess((RecordAccess)node);
            else if (node is Identifier) {
                if (!symStack.checkSymInScope(node.lexeme!.value!)) {
                    ExceptionHandler.Throw(Exceptions.UnknownIdentifier, node.lexeme.lineNumber, node.lexeme.charNumber, node.lexeme!.value!);
                }

                if (symStack.getSym(node.lexeme!.value!) is not SymVar) {
                    ExceptionHandler.Throw(Exceptions.NotAVar, node.lexeme.lineNumber, node.lexeme.charNumber);
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
            
            if (type is not SymTypeArray) {
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
        }
        public void VisitNewField(NewField node) { }
        public void VisitCompareOperator(CompareOperator node) { }
        public void VisitAddOperator(AddOperator node) { }
        public void VisitMultiplyOperator(MultiplyOperator node) { }
        public void VisitIdentifier(Identifier node) { }
        public void VisitConstant(Constant node) { }
    }
}
