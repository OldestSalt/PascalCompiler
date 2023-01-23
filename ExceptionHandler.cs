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
        NoReturnValue,
        ImmutableSymbol,
        UnknownSubroutine,
        IncorrectParameters,
        IncomparableTypes
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
            { Exceptions.NotAVar, "It is not a variable" },
            { Exceptions.NotAnArray, "It is not an array" },
            { Exceptions.NotARecord, "It is not a record" },
            { Exceptions.UnknownField, "Unknown record field" },
            { Exceptions.NoReturnValue, "No return value" },
            { Exceptions.ImmutableSymbol, "It is immutable symbol" },
            { Exceptions.UnknownSubroutine, "Call to unknown function or procedure" },
            { Exceptions.IncorrectParameters, "Incorrect parameters" },
            { Exceptions.IncomparableTypes, "Incomparable types" }

        };

        public static void Throw(Exceptions ex, uint line = 0, uint ch = 0, params string[] extraData) {

            if (extraData.Length == 1) {
                Console.WriteLine($"Error detected at position {line}, {ch}: {ExceptionMessages[ex]} {extraData[0]}");
            }
            else if (ex == Exceptions.IncompatibleTypes) {
                Console.WriteLine($"Error detected at position {line}, {ch}: {ExceptionMessages[ex]}: expected '{extraData[0]}'; got '{extraData[1]}'");
            }
            else if (line != 0 && ch != 0) {
                Console.WriteLine($"Error detected at position {line}, {ch}: {ExceptionMessages[ex]}");
            }
            else {
                Console.WriteLine($"Error detected: {ExceptionMessages[ex]}");
            }

            if (TestSystem.output != null) {
                throw new Exception();
            }
            else {
                Environment.Exit(1);
            }
        }
    }
}
