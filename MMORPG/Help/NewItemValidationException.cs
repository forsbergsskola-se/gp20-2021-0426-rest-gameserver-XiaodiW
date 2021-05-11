using System;

namespace MMORPG.Help {
    [Serializable]
    public class NewItemValidationException : Exception {
        public NewItemValidationException()
        {
        }

        public NewItemValidationException(string message)
            : base(message)
        {
        }

        public NewItemValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
        protected NewItemValidationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}