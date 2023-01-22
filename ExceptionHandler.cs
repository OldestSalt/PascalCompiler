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
        IncorrectStringFormat,
        UnexpectedCharacter,
        LongIdentifierName,
        TestError,
        ExpectedCharacters,
        UnknownError,
        DeclaredIdentifier,
        UnknownType,
        IncompatibleTypes,
        UnknownIdentifier,
        OperationError,
        NotAVar,
        NotAnArray,
        NotARecord,
        UnknownField,
        NoReturnValue
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
            { Exceptions.UnexpectedCharacter, "Unexpected character" },
            { Exceptions.LongIdentifierName, "Identifier's name is too long" },
            { Exceptions.TestError, "Test error detected, check output files" },
            { Exceptions.ExpectedCharacters, "Expected" },
            { Exceptions.UnknownError, "Unknown error" },
            { Exceptions.DeclaredIdentifier, "Duplicate indentidier" },
            { Exceptions.UnknownType, "Unknown datatype" },
            { Exceptions.IncompatibleTypes, "Incompatible types" },
            { Exceptions.UnknownIdentifier, "Unknown identifier" },
            { Exceptions.OperationError, "Operation error:" },
            { Exceptions.NotAVar, "It is not a variable or constant" },
            { Exceptions.NotAnArray, "It is not an array" },
            { Exceptions.NotARecord, "It is not a record" },
            { Exceptions.UnknownField, "Unknown record field" },
            { Exceptions.NoReturnValue, "No return value" }

        };

        public static void Throw(Exceptions ex, uint line = 0, uint ch = 0, params string[] extraData) {
            var outputStream = PascalCompiler.outputStream != null ? PascalCompiler.outputStream : Console.Error;

            if (extraData.Length == 1) {
                outputStream.WriteLine($"Error detected at position {line}, {ch}: {ExceptionMessages[ex]} {extraData[0]}");
            }
            else if (ex == Exceptions.IncompatibleTypes) {
                outputStream.WriteLine($"Error detected at position {line}, {ch}: {ExceptionMessages[ex]}: expected '{extraData[0]}'; got '{extraData[1]}'");
            }
            else if (line != 0 && ch != 0) {
                outputStream.WriteLine($"Error detected at position {line}, {ch}: {ExceptionMessages[ex]}");
            }
            else {
                outputStream.WriteLine($"Error detected: {ExceptionMessages[ex]}");
            }

            if (PascalCompiler.outputStream != null) {
                throw new Exception();
            }
            else {
                Environment.Exit(1);
            }
        }
    }
}
