using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.Lexer {
    public class StateMachine {
        protected StreamHandler streamHandler;
        protected Dictionary<int, Dictionary<HashSet<char>, int>> rules;

        public StateMachine(StreamHandler sh) {
            streamHandler = sh;
        }

        protected int getNextState(int curState, char nextChar) {
            Dictionary<char[], int>? matchingRules;
            int nextState = 0;

            if (!rules.TryGetValue(curState, out matchingRules)) return nextState;
            return matchingRules.FirstOrDefault(item => item.Key.Contains(nextChar)).Value;
        }
    }
}
