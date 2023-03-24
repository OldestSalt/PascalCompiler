using PascalCompiler.Expressions;
using PascalCompiler.Parser.Nodes;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace PascalCompiler.Semantic.Symbols {
    public abstract class Symbol {
        public string name;
        public Symbol(string name) {
            this.name = name;
        }

        public abstract void Print(string indents = "");
    }

    public class SymTable {
        private OrderedDictionary data;
        public SymTable() {
            data = new OrderedDictionary();
        }

        public void pushSym(Symbol sym) {
            if (!data.Contains(sym.name)) {
                data.Add(sym.name, sym);
            }
        }

        public Symbol? getSym(string name) {
            if (data.Contains(name)) {
                return data[name] as Symbol;
            }
            return null;
        }

        public bool checkSym(string name) {
            return data.Contains(name);
        }

        public void Print(string indents = "") {
            foreach (DictionaryEntry sym in data) {
                ((Symbol)sym.Value!).Print(indents);
            }
        }

    }
    public class SymStack {
        private Stack<SymTable> stack;
        public SymStack() {
            stack = new Stack<SymTable>();
            pushScope();
        }

        public bool checkSymInScope(string name) {
            return (stack.Peek().checkSym(name));
        }

        public bool checkSym(string name) {
            foreach (var scope in stack) {
                if (scope.checkSym(name)) return true;
            }
            return false;
        }

        public void pushScope() {
            stack.Push(new SymTable());
        }

        public void popScope() {
            stack.Pop();
        }

        public Symbol? getSym(string name) {
            foreach (var scope in stack) {
                if (scope.checkSym(name)) return scope.getSym(name);
            }
            return null;
        }

        public Symbol? getSymInScope(string name) {
            return stack.Peek().getSym(name);
        }

        public void pushSym(Symbol sym) {
            if (!checkSym(sym.name)) {
                stack.Peek().pushSym(sym);
            }
        }

        public void Print() {
            string indents = "";
            foreach (var symTable in stack) {
                symTable.Print(indents);
                indents += "\t";
                Console.WriteLine("--------------------------");
            }
        }

        public void checkIdentifierDuplication(Lexer.Lexeme lexeme) {
            if (checkSymInScope(lexeme.value!)) {
                ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, lexeme.lineNumber, lexeme.charNumber, lexeme.value!);
            }
        }
    }

    public class SymType : Symbol {
        public SymType(string name) : base(name) { }

        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}type {name}");
        }
    }

    public class SymTypeScalar : SymType {
        public SymTypeScalar(string name) : base(name) { }

        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}type {name}");
        }
    }

    public class SymTypeInteger : SymTypeScalar {
        public SymTypeInteger(string name) : base(name) { }
        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}type {name}");
        }
    }

    public class SymTypeReal : SymTypeScalar {
        public SymTypeReal(string name) : base(name) { }
        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}type {name}");
        }
    }

    public class SymTypeAlias : SymType {
        public SymType refType;
        public SymTypeAlias(SymType refType, string name) : base(name) { 
            this.refType = refType;
        }

        public override void Print(string indents = "") {
            Console.Write($"{indents}alias {name} = ");
            refType.Print();
        }
    }

    public class SymTypeArray : SymType {
        public SymType elemType;
        public SymTypeArray(string name, SymType elemType) : base(name) {
            this.elemType = elemType;
        }

        public override void Print(string indents = "") {
            Console.Write($"{indents}{name}\t");
            elemType.Print();
        }
    }

    public class SymTypeRecord : SymType {
        public SymTable fields;
        public SymTypeRecord(string name, SymTable fields) : base(name) {
            this.fields = fields;
        }

        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}{name}\t");
            fields.Print("\t" + indents);
        }
    }

    public class SymVar : Symbol {
        public SymType type;
        public SymVar(string name, SymType type) : base(name) {
            this.type = type;
        }

        public override void Print(string indents = "") {
            Console.Write($"{indents}var {name}\t");
            type.Print();
        }
    }

    public class SymVarParam : SymVar {
        public CommonConstants.ServiceWords? modifier;
        public SymVarParam(string name, SymType type, CommonConstants.ServiceWords? modifier) : base(name, type) {
            this.modifier = modifier;
        }

        public override void Print(string indents = "") {
            Console.Write($"{indents}param ");
            if (modifier != null) {
                Console.Write($"{modifier}\t");
            }
            Console.Write($"{name}\t");
            type.Print();
        }
    }

    public class SymVarConst : SymVar {
        public SymVarConst(string name, SymType type) : base(name, type) { }
        public override void Print(string indents = "") {
            Console.Write($"{indents}const {name}\t");
            type.Print();
        }
    }

    public class SymProc : Symbol {
        public List<SymVarParam> args;
        public SymProc(string name, List<SymVarParam> args) : base(name) {
            this.args = args;
        }
        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}procedure {name}\t");
            foreach (var arg in args) {
                arg.Print(indents + "\t");
            }
        }
    }

    public class SymFunc : SymProc {
        public SymType returnType;
        public SymFunc(string name, List<SymVarParam> args, SymType returnType) : base(name, args) {
            this.returnType = returnType;
        }
        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}function {name}\t");
            foreach (var arg in args) {
                arg.Print(indents + "\t");
            }
            returnType.Print(indents + "\t");
        }
    }

    public class SymWrite : Symbol {
        public SymWrite(string name) : base(name) { }
        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}embedded write: {name}\t");
        }
    }

    public class SymRead : Symbol {
        public SymRead(string name) : base(name) { }
        public override void Print(string indents = "") {
            Console.WriteLine($"{indents}embedded read: {name}\t");
        }
    }
}
