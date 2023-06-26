using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsBenchmarks {
    [MemoryDiagnoser]
    [Config(typeof(Config))]
    [ShortRunJob]
    public class EcsBenchmarks {
        private const int c_iteration_count = 1;
        private World m_world;
        private DynamicEntitySet m_query;

        private sealed class Config : ManualConfig {
            public Config() {
                AddColumn(new TagColumn("N", _ => c_iteration_count.ToString()));
            }
        }

        [IterationSetup]
        public void IterationSetup() {
            m_world = World.Create();
            /*
            m_query = m_world.CreateQuery().With<Health>().With<Mana>().AsSet();
            for (var i = 1; i <= c_iteration_count; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<Health>();
                if (entity % 3 == 0)
                    entity.Set<Mana>();
            }
            */
        }

        [IterationCleanup]
        public void IterationCleanup() {
            m_world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent.Invoke();
        }

        [Benchmark]
        public void CreateQuery() {
            var query = m_world.CreateQuery().With<Health>().AsSet();
        }
        
        //[Benchmark]
        public void IterateHealthManaQuery() {
            foreach (var entity in m_query) {
                ref var health = ref entity.Get<Health>();
                ref var mana = ref entity.Get<Mana>();
            }
        }

        //[Benchmark]
        public void CreateEntityWithHealthManaComponents() {
            for (var i = 0; i < c_iteration_count; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<Health>();
                entity.Set<Mana>();
            }
        }

        //[Benchmark]
        public void AllocEntities() {
            for (var i = 0; i < c_iteration_count; i++)
                m_world.CreateEntity();
        }

        //[Benchmark]
        public void BasicComponentOperations() {
            for (var i = 0; i < c_iteration_count; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<ExampleComponent>();
                if (entity.Has<ExampleComponent>())
                    entity.Remove<ExampleComponent>();
            }
        }

        //[Benchmark]
        public void AllocAndDestroyEntities() {
            for (var i = 0; i < c_iteration_count; i++) {
                var entity = m_world.CreateEntity();
                entity.Destroy();
            }
        }

        //[Benchmark]
        public void AllocAndSetDataAndDestroyEntities() {
            for (var i = 0; i < c_iteration_count; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<ExampleComponent>();
                entity.Destroy();
            }
        }

        private struct ExampleComponent {
            public byte someData;
        }

        private struct Health {
            public byte someData;
        }

        private struct Mana {
            public byte someData;
        }
    }

    internal static class Benchmarks {
        public static void Main(string[] args) {
            BenchmarkRunner.Run<EcsBenchmarks>();
        }
    }
}