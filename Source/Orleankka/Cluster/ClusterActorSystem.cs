﻿using System;
using System.Linq;
using System.Net;

using Orleans.Runtime.Host;
using Orleans.Runtime.Configuration;

namespace Orleankka.Cluster
{
    public class ClusterActorSystem : MarshalByRefObject, IActorSystem
    {
        static IActorSystem current;

        public static IActorSystem Current 
        {
            get
            {
                if (current == null)
                    throw new InvalidOperationException("Cluster actor system hasn't been initialized");

                return current;
            }

            internal set
            {
                if (current != null)
                    throw new InvalidOperationException("Cluster actor system has been already initialized");

                current = value;
            }
        }

        readonly IDisposable configurator;
        SiloHost host;

        internal ClusterActorSystem(IDisposable configurator, ClusterConfiguration configuration)
        {
            Current = this;
            this.configurator = configurator;
            host = new SiloHost(Dns.GetHostName(), configuration);
        }

        ActorRef IActorSystem.ActorOf(ActorPath path)
        {
            return ActorRef.Resolve(path);
        }

        internal void Start()
        {
            host.LoadOrleansConfig();
            host.InitializeOrleansSilo();
            host.StartOrleansSilo();
        }

        void IDisposable.Dispose()
        {
            if (host == null)
                return;

            host.StopOrleansSilo();
            host.UnInitializeOrleansSilo();
            host.Dispose();
            host = null;

            configurator.Dispose();
        }
    }
}