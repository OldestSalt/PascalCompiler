using PascalCompiler.Parser.Nodes;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace PascalCompiler.Semantic.Symbols {
    public abstract class Symbol {
        public string name;
        public Symbol(string name) {
            this.name = name;
        }
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

        //public void pushVar(NewVariable node) {
        //    SymType symType = null;

        //    if (node.type is BaseDatatype) {
        //        var type = (node.type as BaseDatatype)!.name.lexeme!;
        //        if (!checkSym(type.value!) || getSym(type.value!) is not SymType) {
        //            ExceptionHandler.Throw(Exceptions.UnknownType, type.lineNumber, type.charNumber);
        //        }
        //        symType = new SymType(node.type, type.value!);
        //    }
        //    else if (node.type is ArrayDatatype) {
        //        var elemType = (node.type as ArrayDatatype).
        //    }

        //    foreach (var name in node.names) {
        //        if (checkSymInScope(name.lexeme!.value!)) {
        //            ExceptionHandler.Throw(Exceptions.DeclaredIdentifier, name.lexeme!.lineNumber, name.lexeme!.charNumber);
        //        }
        //    }
        //}
    }

    public class SymType : Symbol {
        public SymType(string name) : base(name) { }
    }

    public class SymTypeScalar : SymType {
        public SymTypeScalar(string name) : base(name) { }
    }

    public class SymTypeInteger : SymTypeScalar {
        public SymTypeInteger(string name) : base(name) { }
    }

    public class SymTypeReal : SymTypeScalar {
        public SymTypeReal(string name) : base(name) { }
    }

    public class SymTypeAlias : SymType {
        public SymType refType;
        public SymTypeAlias(SymType refType, string name) : base(name) { 
            this.refType = refType;
        }
    }

    public class SymTypeArray : SymType { //name example: array [1..20] of Integer
        public SymType elemType;
        public SymTypeArray(string name, SymType elemType) : base(name) {
            this.elemType = elemType;
        }
    }

    public class SymTypeRecord : SymType {
        public SymTable fields;
        public SymTypeRecord(string name, SymTable fields) : base(name) {
            this.fields = fields;
        }
    }

    public class SymVar : Symbol {
        public SymType type;
        public SymVar(string name, SymType type) : base(name) {
            this.type = type;
        }
    }

    public class SymVarParam : SymVar {
        public SymVarParam(string name, SymType type) : base(name, type) { }
    }

    public class SymVarConst : SymVar {
        public SymVarConst(string name, SymType type) : base(name, type) { }
    }

    public class SymProc : Symbol {
        public List<SymVarParam> args;
        public List<SymVar> locals;
        public SymProc(string name, List<SymVarParam> args, List<SymVar> locals) : base(name) {
            this.args = args;
            this.locals = locals;
        }
    }

    public class SymFunc : SymProc {
        public SymType returnType;
        public SymFunc(string name, List<SymVarParam> args, List<SymVar> locals, SymType returnType) : base(name, args, locals) {
            this.returnType = returnType;
        }
    }
}
