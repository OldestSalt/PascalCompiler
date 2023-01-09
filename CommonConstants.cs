using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler {
    public static class CommonConstants {
        public static readonly string ProjectPath =
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
    }
}
