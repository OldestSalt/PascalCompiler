using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler {
    public enum Exceptions {
        FileNotFound,
        DirectoryNotFound,
        WithoutKeys,
        UnknownKeys,
        IntegerIncorrectFormat,
        FloatIncorrectFormat,
        IntegerOverflow,
        FloatOverflow,
        IncorrectStringFormat
    }

    public static class ExceptionHandler {
        private static Dictionary<Exceptions, string> ExceptionMessages = new Dictionary<Exceptions, string> {
            { Exceptions.FileNotFound, "The file doesn't exist or it can't be opened" },
            { Exceptions.DirectoryNotFound, "This directory doesn't exist" },
            { Exceptions.WithoutKeys, "Use keys" },
            { Exceptions.UnknownKeys, "Unknown key(s)" },
            { Exceptions.IntegerIncorrectFormat, "Integer has an incorrect format" },
            { Exceptions.FloatIncorrectFormat, "Float has an incorrect format" },
            { Exceptions.IntegerOverflow, "Integer overflow" },
            { Exceptions.FloatOverflow, "Float overflow" },
            { Exceptions.IncorrectStringFormat, "String has an incorrect format" },

        };

        public static void Throw(Exceptions ex, uint line = 0, uint ch = 0) {
            if (line != 0 && ch != 0) {
                Console.Error.WriteLine($"Error detected on position {line}, {ch}: {ExceptionMessages[ex]}.");
            }
            else {
                Console.Error.WriteLine($"Error detected: {ExceptionMessages[ex]}");
            }
            Environment.Exit(1);
        }
    }
}
