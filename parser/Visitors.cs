using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PascalCompiler.Parser.Nodes {
    public abstract class Visitor {
        public abstract void VisitProgram(Program node, string indents = "");
        public abstract void VisitOptionalBlock(OptionalBlock node, string indents = "");
        public abstract void VisitBlock(Block node, string indents = "");
        public abstract void VisitProgramName(ProgramName node, string indents = "");
        public abstract void VisitDeclarationSection(DeclarationSection node, string indents = "");
        public abstract void VisitConstants(Constants node, string indents = "");
        public abstract void VisitNewConstant(NewConstant node, string indents = "");
        public abstract void VisitVariables(Variables node, string indents = "");
        public abstract void VisitNewVariable(NewVariable node, string indents = "");
        public abstract void VisitTypes(Types node, string indents = "");
        public abstract void VisitNewType(NewType node, string indents = "");
        public abstract void VisitProcedures(Procedures node, string indents = "");
        public abstract void VisitNewProcedure(NewProcedure node, string indents = "");
        public abstract void VisitFunctions(Functions node, string indents = "");
        public abstract void VisitNewFunction(NewFunction node, string indents = "");
        public abstract void VisitSubroutineBody(SubroutineBody node, string indents = "");
        public abstract void VisitSubroutineArgs(SubroutineArgs node, string indents = "");
        public abstract void VisitNewSubroutineArg(NewSubroutineArg node, string indents = "");
        public abstract void VisitArgModifier(ArgModifier node, string indents = "");
        public abstract void VisitArraySubroutineArg(ArraySubroutineArg node, string indents = "");
        public abstract void VisitStatement(Statement node, string indents = "");
        public abstract void VisitEmptyStatement(EmptyStatement node, string indents = "");
        public abstract void VisitAssignmentStatement(AssignmentStatement node, string indents = "");
        public abstract void VisitIfStatement(IfStatement node, string indents = "");
        public abstract void VisitWhileStatement(WhileStatement node, string indents = "");
        public abstract void VisitRepeatStatement(RepeatStatement node, string indents = "");
        public abstract void VisitForStatement(ForStatement node, string indents = "");
        public abstract void VisitSubroutineCall(SubroutineCall node, string indents = "");
        public abstract void VisitExpression(Expression node, string indents = "");
        public abstract void VisitSimpleExpression(SimpleExpression node, string indents = "");
        public abstract void VisitTerm(Term node, string indents = "");
        public abstract void VisitSimpleTerm(SimpleTerm node, string indents = "");
        public abstract void VisitFactor(Factor node, string indents = "");
        public abstract void VisitReference(Reference node, string indents = "");
        public abstract void VisitArrayAccess(ArrayAccess node, string indents = "");
        public abstract void VisitRecordAccess(RecordAccess node, string indents = "");
        public abstract void VisitDatatype(Datatype node, string indents = "");
        public abstract void VisitBaseDatatype(BaseDatatype node, string indents = "");
        public abstract void VisitArrayDatatype(ArrayDatatype node, string indents = "");
        public abstract void VisitIndex(Index node, string indents = "");
        public abstract void VisitRecordDatatype(RecordDatatype node, string indents = "");
        public abstract void VisitNewField(NewField node, string indents = "");
        public abstract void VisitCompareOperator(CompareOperator node, string indents = "");
        public abstract void VisitAddOperator(AddOperator node, string indents = "");
        public abstract void VisitMultiplyOperator(MultiplyOperator node, string indents = "");
        public abstract void VisitIdentifier(Identifier node, string indents = "");
        public abstract void VisitConstant(Constant node, string indents = "");
    }

    public class PrintVisitor : Visitor {
        public override void VisitProgram(Program node, string indents = "") {
            OutputHandler.WriteLine("program");
            if (node.optionalBlock != null) {
                OutputHandler.Write(indents + "├─── ");
                VisitOptionalBlock(node.optionalBlock, indents + "│    ");
            }
            OutputHandler.Write(indents + "└─── ");
            VisitBlock(node.mainBlock, indents + "     ");
        }
        public override void VisitOptionalBlock(OptionalBlock node, string indents = "") {
            OutputHandler.WriteLine("optional block");
            if (node.declarations != null && node.declarations.Count > 0) {
                if (node.programName != null) {
                    OutputHandler.Write(indents + "├─── program name: ");
                    VisitProgramName(node.programName, indents + "│    ");
                }

                for (int i = 0; i < node.declarations.Count - 1; i++) {
                    OutputHandler.Write(indents + "├─── ");
                    VisitDeclarationSection(node.declarations[i], indents + "│    ");
                }
                OutputHandler.Write(indents + "└─── ");
                VisitDeclarationSection(node.declarations.Last(), indents + "     ");
            }
            else {
                if (node.programName != null) {
                    OutputHandler.Write(indents + "└─── program name: ");
                    VisitProgramName(node.programName, indents + "     ");
                }
            }
        }
        public override void VisitBlock(Block node, string indents = "") {
            OutputHandler.WriteLine("block");
            if (node.statements!.Count > 0) {
                for (int i = 0; i < node.statements.Count - 1; i++) {
                    OutputHandler.Write(indents + "├─── ");
                    VisitStatement(node.statements[i], indents + "│    ");
                }
                OutputHandler.Write(indents + "└─── ");
                VisitStatement(node.statements.Last(), indents + "     ");
            }
        }
        public override void VisitProgramName(ProgramName node, string indents = "") {
            OutputHandler.WriteLine(node.name.lexeme.value);
        }
        public override void VisitDeclarationSection(DeclarationSection node, string indents = "") {
            if (node is Constants) VisitConstants(node as Constants, indents);
            else if (node is Variables) VisitVariables(node as Variables, indents);
            else if (node is Types) VisitTypes(node as Types, indents);
            else if (node is Procedures) VisitProcedures(node as Procedures, indents);
            else VisitFunctions(node as Functions, indents);
        }
        public override void VisitConstants(Constants node, string indents = "") {
            OutputHandler.WriteLine("const");

            for (int i = 0; i < node.constants.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewConstant(node.constants[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewConstant(node.constants.Last(), indents + "     ");
        }
        public override void VisitNewConstant(NewConstant node, string indents = "") {
            OutputHandler.WriteLine("const declaration");

            OutputHandler.Write(indents + "├─── ");
            VisitIdentifier(node.name, indents + "│    ");

            if (node.type != null) {
                OutputHandler.Write(indents + "├─── ");
                VisitDatatype(node.type, indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitExpression(node.value, indents + "     ");
        }
        public override void VisitVariables(Variables node, string indents = "") {
            OutputHandler.WriteLine("var");

            for (int i = 0; i < node.variables.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewVariable(node.variables[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewVariable(node.variables.Last(), indents + "     ");
        }
        public override void VisitNewVariable(NewVariable node, string indents = "") {
            OutputHandler.WriteLine("variable declaration");

            OutputHandler.Write(indents + "├─── ");
            VisitIdentifier(node.name, indents + "│    ");


            if (node.value!= null) {
                OutputHandler.Write(indents + "├─── ");
                VisitDatatype(node.type, indents + "│    ");

                OutputHandler.Write(indents + "└─── ");
                VisitExpression(node.value, indents + "     ");
            }
            else {
                OutputHandler.Write(indents + "└─── ");
                VisitDatatype(node.type, indents + "     ");
            }
        }
        public override void VisitTypes(Types node, string indents = "") {
            OutputHandler.WriteLine("type");

            for (int i = 0; i < node.types.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewType(node.types[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewType(node.types.Last(), indents + "     ");
        }
        public override void VisitNewType(NewType node, string indents = "") {
            OutputHandler.WriteLine("type declaration");

            OutputHandler.Write(indents + "├─── ");
            VisitIdentifier(node.name, indents + "│    ");

            OutputHandler.Write(indents + "└─── ");
            VisitDatatype(node.type, indents + "     ");
        }
        public override void VisitProcedures(Procedures node, string indents = "") {
            OutputHandler.WriteLine("procedures");

            for (int i = 0; i < node.procedures.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewProcedure(node.procedures[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewProcedure(node.procedures.Last(), indents + "     ");
        }
        public override void VisitNewProcedure(NewProcedure node, string indents = "") {
            OutputHandler.WriteLine("procedure declaration");

            OutputHandler.Write(indents + "├─── ");
            VisitIdentifier(node.name, indents + "│    ");

            if (node.args != null && node.args.args!.Count > 0) {
                OutputHandler.Write(indents + "├─── ");
                VisitSubroutineArgs(node.args, indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitSubroutineBody(node.body, indents + "     ");
        }
        public override void VisitFunctions(Functions node, string indents = "") {
            OutputHandler.WriteLine("functions");

            for (int i = 0; i < node.functions.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewFunction(node.functions[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewFunction(node.functions.Last(), indents + "     ");
        }
        public override void VisitNewFunction(NewFunction node, string indents = "") {
            OutputHandler.WriteLine("procedure declaration");

            OutputHandler.Write(indents + "├─── ");
            VisitIdentifier(node.name, indents + "│    ");

            if (node.args != null && node.args.args!.Count > 0) {
                OutputHandler.Write(indents + "├─── ");
                VisitSubroutineArgs(node.args, indents + "│    ");
            }

            OutputHandler.Write(indents + "├─── ");
            VisitDatatype(node.returnType, indents + "│    ");

            OutputHandler.Write(indents + "└─── ");
            VisitSubroutineBody(node.body, indents + "     ");
        }
        public override void VisitSubroutineBody(SubroutineBody node, string indents = "") {
            OutputHandler.WriteLine("subroutine body");

            if (node.decls != null && node.decls!.Count > 0) {
                foreach (var decl in node.decls) {
                    OutputHandler.Write(indents + "├─── ");
                    VisitDeclarationSection(decl, indents + "│    ");
                }
            }

            OutputHandler.Write(indents + "└─── ");
            VisitBlock(node.body, indents + "     ");
        }
        public override void VisitSubroutineArgs(SubroutineArgs node, string indents = "") {
            OutputHandler.WriteLine("subroutine args");

            for (int i = 0; i < node.args.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewSubroutineArg(node.args[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewSubroutineArg(node.args.Last(), indents + "     ");
        }
        public override void VisitNewSubroutineArg(NewSubroutineArg node, string indents = "") {
            OutputHandler.WriteLine("arg");

            if (node.modifier != null) {
                OutputHandler.Write(indents + "├─── ");
                VisitArgModifier(node.modifier, indents + "│    ");
            }

            foreach (var name in node.names) {
                OutputHandler.Write(indents + "├─── ");
                VisitIdentifier(name, indents + "│    ");
            }

            if (node.type is BaseDatatype) {
                OutputHandler.Write(indents + "└─── ");
                VisitBaseDatatype(node.type as BaseDatatype, indents + "     ");
            }
            else {
                OutputHandler.Write(indents + "└─── ");
                VisitArraySubroutineArg(node.type as ArraySubroutineArg, indents + "     ");
            }
        }
        public override void VisitArgModifier(ArgModifier node, string indents = "") {
            OutputHandler.WriteLine("arg modifier");
            OutputHandler.Write(indents + "└─── " + node.value);
        }
        public override void VisitArraySubroutineArg(ArraySubroutineArg node, string indents = "") {
            OutputHandler.WriteLine("array arg");
            OutputHandler.Write(indents + "└─── ");
            VisitBaseDatatype(node.type, indents + "     ");
        }
        public override void VisitStatement(Statement node, string indents = "") {
            if (node is EmptyStatement) VisitEmptyStatement(node as EmptyStatement, indents);
            else if (node is AssignmentStatement) VisitAssignmentStatement(node as AssignmentStatement, indents);
            else if (node is IfStatement) VisitIfStatement(node as IfStatement, indents);
            else if (node is WhileStatement) VisitWhileStatement(node as WhileStatement, indents);
            else if (node is RepeatStatement) VisitRepeatStatement(node as RepeatStatement, indents);
            else if (node is ForStatement) VisitForStatement(node as ForStatement, indents);
            else if (node is Block) VisitBlock(node as Block, indents);
            else VisitSubroutineCall(node as SubroutineCall, indents);
        }
        public override void VisitAssignmentStatement(AssignmentStatement node, string indents = "") {
            OutputHandler.WriteLine(":=");

            if (node.leftPart is ArrayAccess) {
                OutputHandler.Write(indents + "├─── ");
                VisitArrayAccess(node.leftPart as ArrayAccess, indents + "│    ");
            }
            else if (node.leftPart is RecordAccess) {
                OutputHandler.Write(indents + "├─── ");
                VisitRecordAccess(node.leftPart as RecordAccess, indents + "│    ");
            }
            else {
                OutputHandler.Write(indents + "├─── ");
                VisitIdentifier(node.leftPart as Identifier, indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitExpression(node.rightPart, indents + "     ");
        }
        public override void VisitEmptyStatement(EmptyStatement node, string indents = "") {
            OutputHandler.WriteLine("empty statement");
        }
        public override void VisitIfStatement(IfStatement node, string indents = "") {
            OutputHandler.WriteLine("if");

            OutputHandler.Write(indents + "├─── ");
            VisitExpression(node.condition, indents + "│    ");

            if (node.falseStatement != null) {
                OutputHandler.Write(indents + "├─── ");
                VisitStatement(node.trueStatement, indents + "│    ");

                OutputHandler.Write(indents + "└─── ");
                VisitStatement(node.falseStatement, indents + "     ");
            }
            else {
                OutputHandler.Write(indents + "└─── ");
                VisitStatement(node.trueStatement, indents + "     ");
            }
        }
        public override void VisitWhileStatement(WhileStatement node, string indents = "") {
            OutputHandler.WriteLine("while");

            OutputHandler.Write(indents + "├─── ");
            VisitExpression(node.condition, indents + "│    ");

            OutputHandler.Write(indents + "└─── ");
            VisitStatement(node.body, indents + "     ");
        }
        public override void VisitRepeatStatement(RepeatStatement node, string indents = "") {
            OutputHandler.WriteLine("repeat");

            if (node.body!.Count > 0) {
                OutputHandler.Write(indents + "├─── ");
                VisitExpression(node.condition, indents + "│    ");

                for (int i = 0; i < node.body.Count - 1; i++) {
                    OutputHandler.Write(indents + "├─── ");
                    VisitStatement(node.body[i], indents + "│    ");
                }

                OutputHandler.Write(indents + "└─── ");
                VisitStatement(node.body.Last(), indents + "     ");
            }
            else {
                OutputHandler.Write(indents + "└─── ");
                VisitExpression(node.condition, indents + "     ");
            }

        }
        public override void VisitForStatement(ForStatement node, string indents = "") {
            OutputHandler.WriteLine("for");

            OutputHandler.Write(indents + "├─── ");
            VisitIdentifier(node.counter, indents + "│    ");

            OutputHandler.Write(indents + "├─── ");
            VisitExpression(node.start, indents + "│    ");

            OutputHandler.Write(indents + "├─── ");
            VisitExpression(node.end, indents + "│    ");

            OutputHandler.Write(indents + "└─── ");
            VisitStatement(node.body, indents + "     ");
        }
        public override void VisitSubroutineCall(SubroutineCall node, string indents = "") {
            OutputHandler.WriteLine("subroutine call");

            if (node.args != null && node.args!.Count > 0) {
                OutputHandler.Write(indents + "├─── ");
                VisitIdentifier(node.name, indents + "│    ");

                for (int i = 0; i < node.args.Count - 1; i++) {
                    OutputHandler.Write(indents + "├─── ");
                    VisitExpression(node.args[i], indents + "│    ");
                }

                OutputHandler.Write(indents + "└─── ");
                VisitExpression(node.args.Last(), indents + "     ");
            }
            else {
                OutputHandler.Write(indents + "└─── ");
                VisitIdentifier(node.name, indents + "     ");
            }
        }
        public override void VisitExpression(Expression node, string indents = "") {
            if (node.compareOperator != null) {
                OutputHandler.WriteLine(node.compareOperator.lexeme.value);

                OutputHandler.Write(indents + "├─── ");
                VisitSimpleExpression(node.leftComparingOperand, indents + "│    ");

                OutputHandler.Write(indents + "└─── ");
                VisitSimpleExpression(node.rightComparingOperand!, indents + "     ");
            }
            else {
                VisitSimpleExpression(node.leftComparingOperand, indents);
            }
        }
        public override void VisitSimpleExpression(SimpleExpression node, string indents = "") {
            if (node.addOperator != null) {
                OutputHandler.WriteLine(node.addOperator.lexeme.value);

                OutputHandler.Write(indents + "├─── ");
                VisitSimpleExpression(node.left!, indents + "│    ");

                OutputHandler.Write(indents + "└─── ");
                VisitTerm(node.right, indents + "     ");
            }
            else {
                VisitTerm(node.right, indents);
            }
        }
        public override void VisitTerm(Term node, string indents = "") {
            if (node.multiplyOperator != null) {
                OutputHandler.WriteLine(node.multiplyOperator.lexeme.value);

                OutputHandler.Write(indents + "├─── ");
                VisitTerm(node.left!, indents + "│    ");

                OutputHandler.Write(indents + "└─── ");
                VisitSimpleTerm(node.right, indents + "     ");
            }
            else {
                VisitSimpleTerm(node.right, indents);
            }
        }
        public override void VisitSimpleTerm(SimpleTerm node, string indents = "") {
            if (node.unaryOperators!.Count > 0) {
                foreach (var op in node.unaryOperators!) {
                    OutputHandler.Write(op.lexeme!.value! + " ");
                }
                OutputHandler.WriteLine("");

                OutputHandler.Write(indents + "└─── ");
                VisitFactor(node.factor, indents + "     ");
            }
            else {
                VisitFactor(node.factor, indents);
            }
        }
        public override void VisitFactor(Factor node, string indents = "") {
            if (node.value is Expression) VisitExpression(node.value as Expression, indents);
            else if (node.value is Reference) VisitReference(node.value as Reference, indents);
            else if (node.value is Constant) VisitConstant(node.value as Constant, indents);
            else VisitSubroutineCall(node.value as SubroutineCall, indents);
        }
        public override void VisitReference(Reference node, string indents = "") {
            if (node is ArrayAccess) VisitArrayAccess(node as ArrayAccess, indents);
            else if (node is RecordAccess) VisitRecordAccess(node as RecordAccess, indents);
            else VisitIdentifier(node as Identifier, indents);
        }
        public override void VisitArrayAccess(ArrayAccess node, string indents = "") {
            OutputHandler.WriteLine("array access");

            if (node.name is Reference) {
                OutputHandler.Write(indents + "├─── ");
                VisitReference(node.name as Reference, indents + "│    ");
            }
            else {
                OutputHandler.Write(indents + "├─── ");
                VisitSubroutineCall(node.name as SubroutineCall, indents + "│    ");
            }

            for (int i = 0; i < node.indexes.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitExpression(node.indexes[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitExpression(node.indexes.Last(), indents + "     ");
        }
        public override void VisitRecordAccess(RecordAccess node, string indents = "") {
            OutputHandler.WriteLine("record access");

            if (node.name is Reference) {
                OutputHandler.Write(indents + "├─── ");
                VisitReference(node.name as Reference, indents + "│    ");
            }
            else {
                OutputHandler.Write(indents + "├─── ");
                VisitSubroutineCall(node.name as SubroutineCall, indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitIdentifier(node.field, indents + "     ");
        }
        public override void VisitDatatype(Datatype node, string indents = "") {
            if (node is BaseDatatype) VisitBaseDatatype(node as BaseDatatype, indents);
            else if (node is ArrayDatatype) VisitArrayDatatype(node as ArrayDatatype, indents);
            else VisitRecordDatatype(node as RecordDatatype, indents);
        }
        public override void VisitBaseDatatype(BaseDatatype node, string indents = "") {
            OutputHandler.Write("datatype: ");
            VisitIdentifier(node.name, indents);
        }
        public override void VisitArrayDatatype(ArrayDatatype node, string indents = "") {
            OutputHandler.WriteLine("array");

            OutputHandler.Write(indents + "├─── ");
            VisitDatatype(node.type, indents + "│    ");

            for (int i = 0; i < node.sizes.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitIndex(node.sizes[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitIndex(node.sizes.Last(), indents + "     ");
        }
        public override void VisitIndex(Index node, string indents = "") {
            OutputHandler.WriteLine("..");

            OutputHandler.Write(indents + "├─── ");
            VisitConstant(node.start, indents + "│    ");

            OutputHandler.Write(indents + "└─── ");
            VisitConstant(node.end, indents + "     ");
        }
        public override void VisitRecordDatatype(RecordDatatype node, string indents = "") {
            OutputHandler.WriteLine("record");

            for (int i = 0; i < node.fields.Count - 1; i++) {
                OutputHandler.Write(indents + "├─── ");
                VisitNewField(node.fields[i], indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitNewField(node.fields.Last(), indents + "     ");
        }
        public override void VisitNewField(NewField node, string indents = "") {
            OutputHandler.WriteLine("field");

            foreach (var name in node.names) {
                OutputHandler.Write(indents + "├─── ");
                VisitIdentifier(name, indents + "│    ");
            }

            OutputHandler.Write(indents + "└─── ");
            VisitDatatype(node.type, indents + "     ");
        }
        public override void VisitCompareOperator(CompareOperator node, string indents = "") {
            OutputHandler.WriteLine(node.lexeme.value);
        }
        public override void VisitAddOperator(AddOperator node, string indents = "") {
            OutputHandler.WriteLine(node.lexeme.value);
        }
        public override void VisitMultiplyOperator(MultiplyOperator node, string indents = "") {
            OutputHandler.WriteLine(node.lexeme.value);
        }
        public override void VisitIdentifier(Identifier node, string indents = "") {
            OutputHandler.WriteLine(node.lexeme.value);
        }
        public override void VisitConstant(Constant node, string indents = "") {
            OutputHandler.WriteLine(node.lexeme.value);
        }
    }
}
