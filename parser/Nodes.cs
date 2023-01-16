using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Parser.Nodes {
    public abstract class Node {
        public Lexer.Lexeme? lexeme;
        public Node? parent = null;
        protected Node(Lexer.Lexeme? lexeme = null) {
            this.lexeme = lexeme;
        }
    }

    public class Program : Node {
        public Program(OptionalBlock? optBlock, Block mainBlock) {
            this.mainBlock = mainBlock;
            optionalBlock = optBlock;
        }
        OptionalBlock? optionalBlock;
        Block mainBlock;
    }

    public class OptionalBlock : Node {
        public ProgramName? programName;
        public List<DeclarationSection>? declarations;
        public OptionalBlock(ProgramName? progName, List<DeclarationSection>? decls) {
            programName = progName;
            declarations = decls;
        }
    }

    public class Block : Statement {
        public List<Statement>? statements;
        public Block(List<Statement>? statements) {
            this.statements = statements;
        }
    }

    public class ProgramName: Node {
        public Node name;
        public ProgramName(Node name) {
            this.name = name;
        }
    }

    public abstract class DeclarationSection : Node { }

    public abstract class NewDeclaration : Node { }

    public class Constants: DeclarationSection {
        public List<NewConstant> constants;
        public Constants(List<NewConstant> constants) {
            this.constants = constants;
        }
    }

    public class NewConstant : NewDeclaration {
        public Identifier name;
        public Datatype? type;
        public Expression value;
        public NewConstant(Identifier name, Datatype? type, Expression value) {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    public class Variables : DeclarationSection {
        public List<NewVariable> variables;
        public Variables(List<NewVariable> variables) {
            this.variables = variables;
        }
    }

    public class NewVariable : NewDeclaration {
        public Identifier name;
        public Datatype type;
        public Expression? value;
        public NewVariable(Identifier name, Datatype type, Expression? value) {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    public class Types : DeclarationSection {
        public List<NewType> types;
        public Types(List<NewType> types) {
            this.types = types;
        }
    }

    public class NewType : NewDeclaration {
        public Identifier name;
        public Datatype type;
        public NewType(Identifier name, Datatype type) {
            this.name = name;
            this.type = type;
        }
    }

    public class Procedures: DeclarationSection {
        public List<NewProcedure> procedures;
        public Procedures(List<NewProcedure> procedures) {
            this.procedures = procedures;
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
    }

    public class Functions: DeclarationSection {
        public List<NewFunction> functions;
        public Functions(List<NewFunction> functions) {
            this.functions = functions;
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
    }

    public class SubroutineBody : Node {
        public List<DeclarationSection>? decls;
        public Block body;
        public SubroutineBody(List<DeclarationSection>? decls, Block body) {
            this.decls = decls;
            this.body = body;
        }
    }

    public class SubroutineArgs : Node {
        public List<NewSubroutineArg>? args;
        public SubroutineArgs(List<NewSubroutineArg>? args) {
            this.args = args;
        }
    }

    public class NewSubroutineArg : Node {
        public ArgModifier? modifier;
        public List<Identifier> names;
        public Node type;
        public NewSubroutineArg(ArgModifier modifier, List<Identifier> names, Node type) {
            this.modifier = modifier;
            this.names = names;
            this.type = type;
        }
    }

    public class ArgModifier : Node {
        public string value;
        public ArgModifier(string value) {
            this.value = value;
        }
    }

    public class ArraySubroutineArg : Node {
        public BaseDatatype type;
        public ArraySubroutineArg(BaseDatatype type) {
            this.type = type;
        }
    }

    public abstract class Statement : Node { }

    public class EmptyStatement : Statement {
        public EmptyStatement() { }
    }

    public class AssignmentStatement : Statement {
        public Expression leftPart;
        public Expression rightPart;
        public AssignmentStatement(Expression leftPart, Expression rightPart) {
            this.leftPart = leftPart;
            this.rightPart = rightPart;
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
    }

    public class WhileStatement : Statement {
        public Expression condition;
        public Statement body;
        public WhileStatement(Expression condition, Statement body) {
            this.condition = condition;
            this.body = body;
        }
    }

    public class RepeatStatement : Statement {
        public Expression condition;
        public List<Statement>? body;
        public RepeatStatement(Expression condition, List<Statement>? body) {
            this.condition = condition;
            this.body = body;
        }
    }

    public class ForStatement : Statement {
        public Identifier counter;
        public Expression start;
        public Expression end;
        public Statement body;
        public ForStatement(Identifier counter, Expression start, Expression end, Statement body) {
            this.counter = counter;
            this.start = start;
            this.end = end;
            this.body = body;
        }
    }

    public class SubroutineCall : Statement {
        public Identifier name;
        public List<Expression>? args;
        public SubroutineCall(Identifier name, List<Expression>? args) {
            this.name = name;
            this.args = args;
        }
    }

    public class Expression : Node {
        public SimpleExpression leftComparingOperand;
        public SimpleExpression? rightComparingOperand;
        public CompareOperator compareOperator;
        public Expression(SimpleExpression leftComparingOperand, SimpleExpression? rightComparingOperand, CompareOperator compareOperator) {
            this.leftComparingOperand = leftComparingOperand;
            this.rightComparingOperand = rightComparingOperand;
            this.compareOperator = compareOperator;
        }
    }

    public class SimpleExpression : Node {
        public Term left;
        public SimpleExpression? right;
        public AddOperator? addOperator;
        public SimpleExpression(Term left, AddOperator addOperator, SimpleExpression? right) {
            this.left = left;
            this.addOperator = addOperator;
            this.right = right;
        }
    }

    public class Term : Node {
        public Factor left;
        public MultiplyOperator? multiplyOperator;
        public Term? right;
        public Term(Factor left, MultiplyOperator multiplyOperator, Term right) {
            this.left = left;
            this.multiplyOperator = multiplyOperator;
            this.right = right;
        }
    }

    public class Factor : Node {
        public List<UnaryOperator>? unaryOperators;
        public Node value;
        public Factor(List<UnaryOperator>? unaryOperators, Node value) {
            this.unaryOperators = unaryOperators;
            this.value = value;
        }
    }

    public abstract class Reference: Node {
        public Reference(Lexer.Lexeme lexeme) : base(lexeme) { }
    }

    public class ArrayAccess: Reference {
        public Reference name;
        public List<Expression> indexes;
        public ArrayAccess(Reference name, List<Expression> indexes, Lexer.Lexeme lexeme) : base(lexeme) {
            this.name = name;
            this.indexes = indexes;
        }
    }

    public class RecordAccess : Reference {
        public Reference name;
        public Identifier field;
        public RecordAccess(Reference name, Identifier field, Lexer.Lexeme lexeme) : base(lexeme) {
            this.name = name;
            this.field = field;
        }
    }

    public class FunctionCall : Reference {
        public Identifier name;
        public List<Expression> args;
        public FunctionCall(Identifier name, List<Expression> args, Lexer.Lexeme lexeme) : base(lexeme) {
            this.name = name;
            this.args = args;
        }
    }

    public abstract class Datatype : Node { }

    public class BaseDatatype : Datatype {
        public Identifier name;
        public BaseDatatype(Identifier name) {
            this.name = name;
        }
    }

    public class ArrayDatatype : Datatype {
        public Datatype type;
        public List<Index> sizes;
        public ArrayDatatype(Datatype type, List<Index> sizes) {
            this.type = type;
            this.sizes = sizes;
        }
    }

    public class Index : Node {
        public Constant start;
        public Constant end;
        public Index(Constant start, Constant end) {
            this.start = start;
            this.end = end;
        }
    }

    public class RecordDatatype : Datatype {
        public List<NewField> fields;
        public RecordDatatype(List<NewField> fields) {
            this.fields = fields;
        }
    }

    public class NewField : Node {
        public List<Identifier> names;
        public Datatype type;
        public NewField(List<Identifier> names, Datatype type) {
            this.names = names;
            this.type = type;
        }
    }

    public abstract class Operator : Node {
        public Operator(Lexer.Lexeme value) : base(value) { }
    }

    public class CompareOperator : Operator {
        public CompareOperator(Lexer.Lexeme value) : base(value) { }
    }

    public class AddOperator : Operator {
        public AddOperator(Lexer.Lexeme value) : base(value) { }
    }

    public class MultiplyOperator : Operator {
        public MultiplyOperator(Lexer.Lexeme value) : base(value) { }
    }

    public class UnaryOperator : Operator {
        public UnaryOperator(Lexer.Lexeme value) : base(value) { }
    }

    public class Identifier : Reference {
        public Identifier(Lexer.Lexeme lexeme) : base(lexeme) { }
    }

    public class Constant : Reference {
        public Constant(Lexer.Lexeme lexeme) : base(lexeme) { }
    }
}
