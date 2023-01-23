using PascalCompiler.Semantic.Symbols;
using PascalCompiler.Semantic;

namespace PascalCompiler.Parser.Nodes {
    public abstract class Node {
        public Lexer.Lexeme? lexeme;
        protected Node(Lexer.Lexeme? lexeme = null) {
            this.lexeme = lexeme;
        }

        public abstract void Print(PrintVisitor visitor);
        public abstract void Sym(SymVisitor visitor);
    }

    public class Program : Node {
        public Program(OptionalBlock? optBlock, Block mainBlock) {
            this.mainBlock = mainBlock;
            optionalBlock = optBlock;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitProgram(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitProgram(this);
        }
        public OptionalBlock? optionalBlock;
        public Block mainBlock;
    }

    public class OptionalBlock : Node {
        public ProgramName? programName;
        public List<DeclarationSection>? declarations;
        public OptionalBlock(ProgramName? progName, List<DeclarationSection>? decls) {
            programName = progName;
            declarations = decls;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitOptionalBlock(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitOptionalBlock(this);
        }
    }

    public class Block : Statement {
        public List<Statement>? statements;
        public Block(List<Statement>? statements) {
            this.statements = statements;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitBlock(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitBlock(this);
        }
    }

    public class ProgramName: Node {
        public Identifier name;
        public ProgramName(Identifier name) {
            this.name = name;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitProgramName(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitProgramName(this);
        }
    }

    public abstract class DeclarationSection : Node { }

    public abstract class NewDeclaration : Node { }

    public class Constants: DeclarationSection {
        public List<NewConstant> constants;
        public Constants(List<NewConstant> constants) {
            this.constants = constants;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitConstants(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitConstants(this);
        }
    }

    public class NewConstant : NewDeclaration {
        public Identifier name;
        public BaseDatatype? type;
        public Expression value;
        public NewConstant(Identifier name, BaseDatatype? type, Expression value) {
            this.name = name;
            this.type = type;
            this.value = value;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewConstant(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewConstant(this);
        }
    }

    public class Variables : DeclarationSection {
        public List<NewVariable> variables;
        public Variables(List<NewVariable> variables) {
            this.variables = variables;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitVariables(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitVariables(this);
        }
    }

    public class NewVariable : NewDeclaration {
        public List<Identifier> names;
        public Datatype type;
        public Expression? value;
        public NewVariable(List<Identifier> names, Datatype type, Expression? value) {
            this.names = names;
            this.type = type;
            this.value = value;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewVariable(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewVariable(this);
        }
    }

    public class Types : DeclarationSection {
        public List<NewType> types;
        public Types(List<NewType> types) {
            this.types = types;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitTypes(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitTypes(this);
        }
    }

    public class NewType : NewDeclaration {
        public Identifier name;
        public Datatype type;
        public NewType(Identifier name, Datatype type) {
            this.name = name;
            this.type = type;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewType(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewType(this);
        }
    }

    public class Procedures: DeclarationSection {
        public List<NewProcedure> procedures;
        public Procedures(List<NewProcedure> procedures) {
            this.procedures = procedures;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitProcedures(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitProcedures(this);
        }
    }

    public class NewProcedure : NewDeclaration {
        public Identifier name;
        public SubroutineArgs args;
        public SubroutineBody body;
        public NewProcedure(Identifier name, SubroutineArgs args, SubroutineBody body) {
            this.name = name;
            this.args = args;
            this.body = body;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewProcedure(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewProcedure(this);
        }
    }

    public class Functions: DeclarationSection {
        public List<NewFunction> functions;
        public Functions(List<NewFunction> functions) {
            this.functions = functions;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitFunctions(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitFunctions(this);
        }
    }

    public class NewFunction : NewDeclaration {
        public Identifier name;
        public SubroutineArgs args;
        public Datatype returnType;
        public SubroutineBody body;
        public NewFunction(Identifier name, SubroutineArgs args, Datatype returnType, SubroutineBody body) {
            this.name = name;
            this.args = args;
            this.returnType = returnType;
            this.body = body;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewFunction(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewFunction(this);
        }
    }

    public class SubroutineBody : Node {
        public List<DeclarationSection>? decls;
        public Block body;
        public SubroutineBody(List<DeclarationSection>? decls, Block body) {
            this.decls = decls;
            this.body = body;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitSubroutineBody(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitSubroutineBody(this);
        }
    }

    public class SubroutineArgs : Node {
        public List<NewSubroutineArg>? args;
        public List<SymVarParam> sym = new List<SymVarParam>();
        public SubroutineArgs(List<NewSubroutineArg>? args) {
            this.args = args;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitSubroutineArgs(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitSubroutineArgs(this);
        }
    }

    public class NewSubroutineArg : Node {
        public ArgModifier? modifier;
        public List<Identifier> names;
        public Node type;
        public List<SymVarParam> sym = new List<SymVarParam>();
        public NewSubroutineArg(ArgModifier? modifier, List<Identifier> names, Node type) {
            this.modifier = modifier;
            this.names = names;
            this.type = type;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewSubroutineArg(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewSubroutineArg(this);
        }
    }

    public class ArgModifier : Node {
        public string value;
        public ArgModifier(string value) {
            this.value = value;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitArgModifier(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitArgModifier(this);
        }
    }

    public class ArraySubroutineArg : Node {
        public BaseDatatype type;
        public SymType? symType;
        public ArraySubroutineArg(BaseDatatype type) {
            this.type = type;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitArraySubroutineArg(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitArraySubroutineArg(this);
        }
    }

    public abstract class Statement : Node { }

    public class EmptyStatement : Statement {
        public EmptyStatement() { }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitEmptyStatement(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitEmptyStatement(this);
        }
    }

    public class AssignmentStatement : Statement {
        public Reference leftPart;
        public Expression rightPart;
        public AssignmentStatement(Reference leftPart, Expression rightPart) {
            this.leftPart = leftPart;
            this.rightPart = rightPart;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitAssignmentStatement(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitAssignmentStatement(this);
        }
    }

    public class IfStatement : Statement {
        public Expression condition;
        public Statement trueStatement;
        public Statement? falseStatement;
        public IfStatement(Expression condition, Statement trueStatement, Statement? falseStatement) {
            this.condition = condition;
            this.trueStatement = trueStatement;
            this.falseStatement = falseStatement;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitIfStatement(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitIfStatement(this);
        }
    }

    public class WhileStatement : Statement {
        public Expression condition;
        public Statement body;
        public WhileStatement(Expression condition, Statement body) {
            this.condition = condition;
            this.body = body;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitWhileStatement(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitWhileStatement(this);
        }
    }

    public class RepeatStatement : Statement {
        public Expression condition;
        public List<Statement>? body;
        public RepeatStatement(Expression condition, List<Statement>? body) {
            this.condition = condition;
            this.body = body;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitRepeatStatement(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitRepeatStatement(this);
        }
    }

    public class ForStatement : Statement {
        public Identifier counter;
        public Expression start;
        public Lexer.Lexeme direction;
        public Expression end;
        public Statement body;
        public ForStatement(Identifier counter, Expression start, Lexer.Lexeme direction, Expression end, Statement body) {
            this.counter = counter;
            this.start = start;
            this.direction = direction;
            this.end = end;
            this.body = body;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitForStatement(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitForStatement(this);
        }
    }

    public class SubroutineCall : Statement {
        public Identifier name;
        public List<Expression> args;
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
        public SubroutineCall(Identifier name, List<Expression>? args) {
            this.name = name;
            this.args = args;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitSubroutineCall(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitSubroutineCall(this);
        }
    }

    public class Expression : Node {
        public SimpleExpression leftComparingOperand;
        public SimpleExpression? rightComparingOperand;
        public CompareOperator? compareOperator;
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
        public bool isVariable = false;
        public Expression(SimpleExpression leftComparingOperand, SimpleExpression? rightComparingOperand, CompareOperator? compareOperator) {
            this.leftComparingOperand = leftComparingOperand;
            this.rightComparingOperand = rightComparingOperand;
            this.compareOperator = compareOperator;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitExpression(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitExpression(this);
        }
    }

    public class SimpleExpression : Node {
        public SimpleExpression? left;
        public Term right;
        public AddOperator? addOperator;
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
        public bool isVariable = false;
        public SimpleExpression(SimpleExpression? left, AddOperator? addOperator, Term right) {
            this.left = left;
            this.addOperator = addOperator;
            this.right = right;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitSimpleExpression(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitSimpleExpression(this);
        }
    }

    public class Term : Node {
        public Term? left;
        public MultiplyOperator? multiplyOperator;
        public SimpleTerm right;
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
        public bool isVariable = false;
        public Term(Term? left, MultiplyOperator? multiplyOperator, SimpleTerm right) {
            this.left = left;
            this.multiplyOperator = multiplyOperator;
            this.right = right;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitTerm(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitTerm(this);
        }
    }

    public class SimpleTerm : Node {
        public List<UnaryOperator>? unaryOperators;
        public Factor factor;
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
        public bool isVariable = false;
        public SimpleTerm(List<UnaryOperator>? unaryOperators, Factor factor) {
            this.unaryOperators = unaryOperators;
            this.factor = factor;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitSimpleTerm(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitSimpleTerm(this);
        }
    }

    public class Factor : Node {
        public Node value;
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
        public bool isVariable = false;
        public Factor(Node value) {
            this.value = value;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitFactor(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitFactor(this);
        }
    }

    public abstract class Reference: Node {
        public SymType? symType;
        public uint lineNumber = 0, charNumber = 0;
    }

    public class ArrayAccess: Reference {
        public Node name;
        public List<Expression> indexes;
        public ArrayAccess(Node name, List<Expression> indexes) {
            this.name = name;
            this.indexes = indexes;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitArrayAccess(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitArrayAccess(this);
        }
    }

    public class RecordAccess : Reference {
        public Node name;
        public Identifier field;
        public RecordAccess(Node name, Identifier field) {
            this.name = name;
            this.field = field;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitRecordAccess(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitRecordAccess(this);
        }
    }

    public abstract class Datatype : Node {
        public SymType? symType;
    }

    public class BaseDatatype : Datatype {
        public Identifier name;
        public BaseDatatype(Identifier name) {
            this.name = name;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitBaseDatatype(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitBaseDatatype(this);
        }
    }

    public class ArrayDatatype : Datatype {
        public Datatype type;
        public List<Index> sizes;
        public ArrayDatatype(Datatype type, List<Index> sizes) {
            this.type = type;
            this.sizes = sizes;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitArrayDatatype(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitArrayDatatype(this);
        }
    }

    public class Index : Node {
        public Constant start;
        public Constant end;
        public Index(Constant start, Constant end) {
            this.start = start;
            this.end = end;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitIndex(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitIndex(this);
        }
    }

    public class RecordDatatype : Datatype {
        public List<NewField> fields;
        public RecordDatatype(List<NewField> fields) {
            this.fields = fields;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitRecordDatatype(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitRecordDatatype(this);
        }
    }

    public class NewField : Node {
        public List<Identifier> names;
        public Datatype type;
        public NewField(List<Identifier> names, Datatype type) {
            this.names = names;
            this.type = type;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitNewField(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitNewField(this);
        }
    }

    public abstract class Operator : Node {
        public Operator(Lexer.Lexeme value) : base(value) { }
    }

    public class CompareOperator : Operator {
        public CompareOperator(Lexer.Lexeme value) : base(value) { }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitCompareOperator(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitCompareOperator(this);
        }
    }

    public class AddOperator : Operator {
        public AddOperator(Lexer.Lexeme value) : base(value) { }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitAddOperator(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitAddOperator(this);
        }
    }

    public class MultiplyOperator : Operator {
        public MultiplyOperator(Lexer.Lexeme value) : base(value) { }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitMultiplyOperator(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitMultiplyOperator(this);
        }
    }

    public class UnaryOperator : Operator {
        public UnaryOperator(Lexer.Lexeme value) : base(value) { }
        public override void Print(PrintVisitor visitor) { }
        public override void Sym(SymVisitor visitor) { }
    }

    public class Identifier : Reference {
        public Identifier(Lexer.Lexeme lexeme) {
            this.lexeme = lexeme;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitIdentifier(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitIdentifier(this);
        }
    }

    public class Constant : Node {
        public Constant(Lexer.Lexeme lexeme) {
            this.lexeme = lexeme;
        }
        public override void Print(PrintVisitor visitor) {
            visitor.VisitConstant(this);
        }
        public override void Sym(SymVisitor visitor) {
            visitor.VisitConstant(this);
        }
    }
}
