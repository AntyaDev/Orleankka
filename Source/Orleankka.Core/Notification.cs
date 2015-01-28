﻿using System;
using System.Linq;

using Orleans.Serialization;
using Orleans.CodeGeneration;

namespace Orleankka
{
    /// <summary>
    /// Represent actor notification data
    /// </summary>
    public sealed class Notification
    {
        /// <summary>
        /// The source actor that provides notification information.
        /// </summary>
        public readonly ActorPath Source;

        /// <summary>
        /// The object that provides additional information about the notification.
        /// </summary>
        public readonly object Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="source">The source actor that provides notification information.</param>
        /// <param name="message">The object that provides additional information about the notification.</param>
        public Notification(ActorPath source, object message)
        {
            Requires.NotNull(source, "source");
            Requires.NotNull(message, "message");

            Source = source;
            Message = message;
        }

        [SerializerMethod]
        internal static void Serialize(object obj, BinaryTokenStreamWriter stream, Type t)
        {
            var msg = (Notification)obj;

            SerializationManager.SerializeInner(msg.Source, stream, typeof(ActorPath));
            SerializationManager.SerializeInner(Internal.Message.Serializer(msg.Message), stream, typeof(byte[]));
        }

        [DeserializerMethod]
        internal static object Deserialize(Type t, BinaryTokenStreamReader stream)
        {
            var source = (ActorPath)SerializationManager.DeserializeInner(typeof(ActorPath), stream);
            var message = Internal.Message.Deserializer((byte[])SerializationManager.DeserializeInner(typeof(byte[]), stream));

            return new Notification(source, message);
        }

        [CopierMethod]
        internal static object DeepCopy(object original)
        {
            return original;
        }
    }
}
