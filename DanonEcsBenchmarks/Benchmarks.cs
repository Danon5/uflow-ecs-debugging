using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DanonEcs;

namespace DanonEcsBenchmarks {
    [MemoryDiagnoser]
    public class EcsBenchmarks {
        private const int c_iteration_count = 16000;
        private World m_world;

        [GlobalSetup]
        public void GlobalSetup() {
            m_world = World.Create();
        }

        [GlobalCleanup]
        public void GlobalCleanup() {
            m_world.Destroy();
        }

        [IterationCleanup]
        public void IterationCleanup() {
            m_world.DestroyAllEntities();
        }

        [Benchmark]
        public void BasicTest() {
            for (var i = 0; i < c_iteration_count; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<HealthComponent>();
                entity.Set<ManaComponent>();
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
            public int someData;
        }

        private struct HealthComponent {
            public byte someData;
        }

        private struct ManaComponent {
            public byte someData;
        }
    }

    internal static class Benchmarks {
        public static void Main(string[] args) {
            BenchmarkRunner.Run<EcsBenchmarks>();
        }
    }
}