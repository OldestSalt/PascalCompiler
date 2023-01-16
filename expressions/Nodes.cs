using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Expressions {
    public class Node {
        public Node? parent = null;
        public string? value = "";
    }
    
    public class BinaryOperation: Node {
        public Node left;
        public Node right;
        public BinaryOperation(Node left, Node right, string? value) {
            this.left = left;
            left.parent = this;
            this.right = right;
            right.parent = this;
            this.value = value;
        }
    }

    public class Number: Node {
        public Number(string? value) {
            this.value = value;
        }
    }

    public class Variable : Node {
        public Variable(string? value) {
            this.value = value;
        }
    }
}
