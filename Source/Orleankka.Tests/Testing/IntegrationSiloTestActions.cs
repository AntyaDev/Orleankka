﻿using System;
using System.Linq;

using NUnit.Framework;

using Orleankka.Testing;
[assembly: TeardownSilo]

namespace Orleankka.Testing
{
    using Cluster;
    using Playground;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequiresSiloAttribute : TestActionAttribute
    {
        public bool Fresh;
        public int DefaultKeepAliveTimeoutInMinutes = 1;

        public override void BeforeTest(TestDetails details)
        {
            if (!details.IsSuite)
                return;
            
            if (Fresh)
                TeardownExisting();

            StartNew();
        }

        static void TeardownExisting()
        {
            if (TestActorSystem.Instance == null)
                return;

            TestActorSystem.Instance.Dispose();
            TestActorSystem.Instance = null;
        }

        void StartNew()
        {
            if (TestActorSystem.Instance != null)
                return;

            TestActorSystem.Instance = ActorSystem.Configure()
                .Playground()
                .Tweak(cluster => cluster
                    .DefaultKeepAliveTimeout(TimeSpan.FromMinutes(DefaultKeepAliveTimeoutInMinutes)))
                .Register(GetType().Assembly)
                .Serializer<JsonSerializer>()
                .Done();
        }
    }

    public class TeardownSiloAttribute : TestActionAttribute
    {
        public override void AfterTest(TestDetails details)
        {
            if (!details.IsSuite)
                return;

            if (TestActorSystem.Instance == null)
                return;

            TestActorSystem.Instance.Dispose();
            TestActorSystem.Instance = null;
        }
    }
}
