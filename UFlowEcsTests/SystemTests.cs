using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class SystemTests {
        private World m_world;
        
        [SetUp]
        public void SetUp() {
            m_world = EcsUtils.Worlds.CreateWorldFromType<DefaultWorldType>();
        }

        [TearDown]
        public void TearDown() {
            m_world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }

        [Test]
        public void SystemsRegistered()
        {
            Assert.Multiple(() =>
            {
                Assert.That(m_world.GetOrCreateSystemGroup<SimulationSystemGroup>().Has<MySystem1>());
                Assert.That(m_world.GetOrCreateSystemGroup<SimulationSystemGroup>().Has<MySystem2>());
                Assert.That(m_world.GetOrCreateSystemGroup<SimulationSystemGroup>().Has<MySystem3>());
            });
        }

        [Test]
        public void CorrectOrder() {
            var count = 0;
            foreach (var system in m_world.GetOrCreateSystemGroup<SimulationSystemGroup>()) {
                var type = count switch {
                    0 => typeof(MySystem1),
                    1 => typeof(MySystem3),
                    2 => typeof(MySystem2),
                    _ => default
                };
                Assert.That(system.GetType(), Is.EqualTo(type));
                count++;
            }
        }

        [ExecuteInWorld(typeof(DefaultWorldType))]
        [ExecuteInGroup(typeof(SimulationSystemGroup))]
        [ExecuteBefore(typeof(MySystem2))]
        private sealed class MySystem1 : BaseRunSystem {
            public MySystem1(in World world) : base(in world) { }
        }
        
        [ExecuteInWorld(typeof(DefaultWorldType))]
        [ExecuteInGroup(typeof(SimulationSystemGroup))]
        [ExecuteAfter(typeof(MySystem3))]
        private sealed class MySystem2 : BaseRunSystem {
            public MySystem2(in World world) : base(in world) { }
        }
        
        [ExecuteInWorld(typeof(DefaultWorldType))]
        [ExecuteInGroup(typeof(SimulationSystemGroup))]
        private sealed class MySystem3 : BaseRunSystem {
            public MySystem3(in World world) : base(in world) { }
        }
    }
}