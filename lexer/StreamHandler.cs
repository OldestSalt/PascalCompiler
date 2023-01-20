namespace PascalCompiler.Lexer {
    public class StreamHandler {
        private StreamReader streamReader;
        private Buffer buffer;
        public uint lineNumber { get; private set; } = 1;
        public uint charNumber { get; private set; } = 0;
        
        public StreamHandler(string fileName) {
            try {
                streamReader = new StreamReader(Path.Combine(CommonConstants.ProjectPath, fileName));
            }
            catch (FileNotFoundException) {
                ExceptionHandler.Throw(Exceptions.FileNotFound);
            }
            catch (DirectoryNotFoundException) {
                ExceptionHandler.Throw(Exceptions.DirectoryNotFound);
            }
            buffer = new Buffer(streamReader);

        }
        ~StreamHandler() {
            streamReader.Dispose();
        }
        public char GetChar() {
            char c = buffer.Read();
            if (c == '\n') {
                lineNumber++;
                charNumber = 0;
            }
            else 
                charNumber++;
            return c;
        }
        public char Peek() {
            return buffer.Peek();
        }
        public char Peek2() {
            return buffer.Peek2();
        }
        public void SkipWhiteSpacesAndComments() {
            int c, cn;
            bool isCurlyComment = false;
            bool isBigramComment = false;
            bool isLineComment = false;

            while (true) {
                c = Peek();
                cn = Peek2();

                if (c == '\0') break;

                if (c == '{') {
                    GetChar();
                    isCurlyComment = true;
                }
                else if (c == '(' && cn == '*') {
                    GetChar();
                    GetChar();
                    isBigramComment = true;
                }
                else if (c == '/' && cn == '/') {
                    GetChar();
                    GetChar();
                    isLineComment = true;
                }

                c = Peek();
                cn = Peek2();

                while (isCurlyComment || isBigramComment || isLineComment) {
                    if (c == '}' && (isCurlyComment || isBigramComment)) {
                        isCurlyComment = false;
                        isBigramComment = false;
                    }
                    else if (c == '*' && cn == ')' && (isBigramComment || isCurlyComment)) {
                        isBigramComment = false;
                        isCurlyComment = false;
                        GetChar();
                    }
                    else if ((c == '\n' || c == '\0') && isLineComment) {
                        isLineComment = false;
                    }

                    GetChar();
                    c = Peek();
                    cn = Peek2();
                }

                if (c == '{' || c == '(' && cn == '*' || c == '/' && cn == '/') continue;

                if (Char.IsWhiteSpace((char)c)) GetChar();
                else break;
            }
        }
    }
}