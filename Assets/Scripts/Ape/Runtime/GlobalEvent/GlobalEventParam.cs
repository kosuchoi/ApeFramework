using System;

namespace Ape.Runtime.GlobalEvent
{
    public class GlobalEventParam<T> : GlobalEventParamBase
    {
        public override Enum EventType { get; }
        public T Value { get; }

        public GlobalEventParam(Enum eventType, T value)
        {
            EventType = eventType;
            Value = value;
        }
    }
}