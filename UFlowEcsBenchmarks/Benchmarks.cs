using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsBenchmarks {
    [MemoryDiagnoser]
    [SimpleJob(iterationCount: 25, invocationCount: 5000)]
    public class EcsBenchmarks {
        [Params(128, 256, 512, 1024, 2048, 4096, 8192)]
        public int Iterations;
        private World m_world;
        private DynamicEntitySet m_query;

        [IterationSetup]
        public void IterationSetup() {
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
            m_world = new World();
            m_query = m_world.BuildQuery().With<Health>().With<Mana>().AsSet();
            for (var i = 1; i <= Iterations; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<Health>();
                if (i % 3 == 0)
                    entity.Set<Mana>();
            }
        }

        [IterationCleanup]
        public void IterationCleanup() {
            m_world.Destroy();
        }
        
        [Benchmark]
        public void IterateHealthManaQuery() {
            foreach (var entity in m_query) {
                ref var health = ref entity.Get<Health>();
                ref var mana = ref entity.Get<Mana>();
            }
        }

        //[Benchmark]
        public void CreateQuery() {
            var query = m_world.BuildQuery().With<Health>().AsSet();
        }

        //[Benchmark]
        public void CreateEntityWithHealthManaComponents() {
            for (var i = 0; i < Iterations; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<Health>();
                entity.Set<Mana>();
            }
        }

        //[Benchmark]
        public void AllocEntities() {
            for (var i = 0; i < Iterations; i++)
                m_world.CreateEntity();
        }

        //[Benchmark]
        public void BasicComponentOperations() {
            for (var i = 0; i < Iterations; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<ExampleComponent>();
                if (entity.Has<ExampleComponent>())
                    entity.Remove<ExampleComponent>();
            }
        }

        //[Benchmark]
        public void AllocAndDestroyEntities() {
            for (var i = 0; i < Iterations; i++) {
                var entity = m_world.CreateEntity();
                entity.Destroy();
            }
        }

        //[Benchmark]
        public void AllocAndSetDataAndDestroyEntities() {
            for (var i = 0; i < Iterations; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<ExampleComponent>();
                entity.Destroy();
            }
        }

        private struct ExampleComponent : IEcsComponent {
            public byte someData;
        }

        private struct Health : IEcsComponent {
            public byte someData;
        }

        private struct Mana : IEcsComponent {
            public byte someData;
        }
    }

    internal static class Benchmarks {
        public static void Main(string[] args) {
            //BenchmarkSwitcher.FromAssembly(typeof(Benchmarks).Assembly).Run(args, new DebugInProcessConfig());
            BenchmarkRunner.Run<EcsBenchmarks>();
        }
    }
}