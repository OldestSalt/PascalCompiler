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

    public class SymTypeArray : SymType {
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
        public CommonConstants.ServiceWords? modifier;
        public SymVarParam(string name, SymType type, CommonConstants.ServiceWords? modifier) : base(name, type) {
            this.modifier = modifier;
        }
    }

    public class SymVarConst : SymVar {
        public SymVarConst(string name, SymType type) : base(name, type) { }
    }

    public class SymProc : Symbol {
        public List<SymVarParam> args;
        public SymProc(string name, List<SymVarParam> args) : base(name) {
            this.args = args;
        }
    }

    public class SymFunc : SymProc {
        public SymType returnType;
        public SymFunc(string name, List<SymVarParam> args, SymType returnType) : base(name, args) {
            this.returnType = returnType;
        }
    }

    //public class SymOverloadingParam : Symbol {
    //    public List<SymType> types;
    //    public SymOverloadingParam(string name, List<SymType> types) : base(name) {
    //        this.types = types;
    //    }
    //}

    public class SymWrite : Symbol {
        public SymWrite(string name) : base(name) { }
    }

    public class SymRead : Symbol {
        public SymRead(string name) : base(name) { }
    }
}
