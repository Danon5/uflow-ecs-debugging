using DanonUnityFramework.Core.Runtime.Ecs;
using NUnit.Framework;

namespace DanonEcsTests {
    [TestFixture]
    public class Tests {
        private World m_world;

        [SetUp]
        public void SetUp() {
            m_world = World.Create();
        }

        [TearDown]
        public void TearDown() {
            m_world.Destroy();
        }

        [Test]
        public void AddComponentTest() {
            var entity = m_world.CreateEntity();
            entity.Set<ExampleComponent>();
            Assert.IsTrue(entity.Has<ExampleComponent>());
        }

        [Test]
        public void SetComponentDataTest() {
            var entity = m_world.CreateEntity();
            entity.Set(new ExampleComponent {
                someData = 5
            });
            Assert.AreEqual(entity.Get<ExampleComponent>().someData, 5);
        }

        [Test]
        public void RemoveComponentTest() {
            var entity = m_world.CreateEntity();
            entity.Set<ExampleComponent>();
            entity.Remove<ExampleComponent>();
            Assert.IsFalse(entity.Has<ExampleComponent>());
        }

        [Test]
        public void BasicMultiWorldTest() {
            var world1 = World.Create();
            var world2 = World.Create();

            var entity1 = world1.CreateEntity();
            Assert.IsFalse(entity1.Has<ExampleComponent>());
            entity1.Set<ExampleComponent>();
            Assert.IsTrue(entity1.Has<ExampleComponent>());

            var entity2 = world2.CreateEntity();
            Assert.IsFalse(entity2.Has<ExampleComponent>());
            entity2.Set<ExampleComponent>();
            Assert.IsTrue(entity2.Has<ExampleComponent>());

            world1.Destroy();
            world2.Destroy();
        }

        [Test]
        public void ManyEntitiesTest() {
            for (var i = 0; i < 10000; i++)
                m_world.CreateEntity();
        }

        [Test]
        public void DestroyEntitiesTest() {
            for (var i = 0; i < 5; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<ExampleComponent>();
                entity.Destroy();
            }

            for (var i = 0; i < 5; i++) {
                var entity = m_world.CreateEntity();
                Assert.IsFalse(entity.Has<ExampleComponent>());
            }

            for (var i = 0; i < 5; i++) {
                var entity = m_world.CreateEntity();
                entity.Set<ExampleComponent>();
            }
        }

        [Test]
        public void DestroyWorldTest() {
            var world = World.Create();

            for (var i = 0; i < 5; i++) {
                var entity = world.CreateEntity();
                entity.Set<ExampleComponent>();
            }

            world.Destroy();
        }

        [Test]
        public void EntityIsValidTest() {
            var entity = m_world.CreateEntity();
            Assert.IsTrue(entity.IsAlive());
            entity.Destroy();
            Assert.IsFalse(entity.IsAlive());
            var entity2 = m_world.CreateEntity();
            Assert.IsFalse(entity.IsAlive());
            Assert.IsTrue(entity2.IsAlive());
            entity2.Destroy();
            Assert.IsFalse(entity2.IsAlive());
        }

        [Test]
        public void WorldIsValidTest() {
            var world = World.Create();
            Assert.IsTrue(world.IsAlive());
            world.Destroy();
            Assert.IsFalse(world.IsAlive());
        }

        private struct ExampleComponent {
            public int someData;
        }
    }
}