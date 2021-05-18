using System;

namespace MMORPG.Help {
    [Serializable]
    public class GameRestrictionException: Exception {
        public GameRestrictionException()
        {
        }

        public GameRestrictionException(string message)
            : base(message)
        {
        }

        public GameRestrictionException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
        protected GameRestrictionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}