namespace PascalCompiler.Lexer {
    public class Buffer {
        private int[] bufferArray = new int[2];
        private StreamReader sr;
        public Buffer(StreamReader streamReader) {
            sr = streamReader;
            bufferArray[0] = streamReader.Read();
            bufferArray[1] = streamReader.Read();
        }

        public char Read() {
            int c = 0;
            if (bufferArray[0] == -1) return (char)c;
            c = bufferArray[0];

            bufferArray[0] = bufferArray[1];
            if (bufferArray[1] != -1) bufferArray[1] = sr.Read();

            return (char)c;
        }

        public char Peek() {
            if (bufferArray[0] == -1) return (char)0;
            return (char)bufferArray[0];
        }

        public char Peek2() {
            if (bufferArray[1] == -1) return (char)0;
            return (char)bufferArray[1];
        }
    }
}
