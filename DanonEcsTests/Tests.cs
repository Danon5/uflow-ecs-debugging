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
            Assert.That(entity.Has<ExampleComponent>());
        }

        [Test]
        public void SetComponentDataTest() {
            var entity = m_world.CreateEntity();
            entity.Set(new ExampleComponent {
                someData = 5
            });
            Assert.That(entity.Get<ExampleComponent>().someData, Is.EqualTo(5));
        }

        [Test]
        public void RemoveComponentTest() {
            var entity = m_world.CreateEntity();
            entity.Set<ExampleComponent>();
            entity.Remove<ExampleComponent>();
            Assert.That(entity.Has<ExampleComponent>(), Is.False);
        }

        [Test]
        public void BasicMultiWorldTest() {
            var world1 = World.Create();
            var world2 = World.Create();

            var entity1 = world1.CreateEntity();
            Assert.That(entity1.Has<ExampleComponent>(), Is.False);
            entity1.Set<ExampleComponent>();
            Assert.That(entity1.Has<ExampleComponent>(), Is.True);

            var entity2 = world2.CreateEntity();
            Assert.That(entity2.Has<ExampleComponent>(), Is.False);
            entity2.Set<ExampleComponent>();
            Assert.That(entity2.Has<ExampleComponent>(), Is.True);

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
                Assert.That(entity.Has<ExampleComponent>(), Is.False);
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
        public void EntityIsValidTest()
        {
            var entity = m_world.CreateEntity();
            Assert.That(entity.IsAlive(), Is.True);
            entity.Destroy();
            Assert.That(entity.IsAlive(), Is.False);
            var entity2 = m_world.CreateEntity();
            Assert.Multiple(() =>
            {
                Assert.That(entity.IsAlive(), Is.False);
                Assert.That(entity2.IsAlive(), Is.True);
            });
            entity2.Destroy();
            Assert.That(entity2.IsAlive(), Is.False);
        }

        [Test]
        public void WorldIsValidTest() {
            var world = World.Create();
            Assert.That(world.IsAlive(), Is.True);
            world.Destroy();
            Assert.That(world.IsAlive(), Is.False);
        }

        [Test]
        public void WithQueryTest() {
            var entity1 = m_world.CreateEntity();
            entity1.Set(new ExampleComponent());
            var query1 = m_world.CreateQuery().With<ExampleComponent>();
            Assert.That(query1.GetEntityCount(), Is.EqualTo(1));
            
            var entity2 = m_world.CreateEntity();
            entity2.Set(new ExampleComponent());
            Assert.That(query1.GetEntityCount(), Is.EqualTo(2));

            var query2 = m_world.CreateQuery().With<OtherComponent>().Without<ExampleComponent>();
            Assert.That(query2.GetEntityCount(), Is.EqualTo(0));

            var entity3 = m_world.CreateEntity();
            entity3.Set(new OtherComponent());
            entity3.Set(new ExampleComponent());
            Assert.That(query2.GetEntityCount(), Is.EqualTo(0));
            
            var entity4 = m_world.CreateEntity();
            entity4.Set(new OtherComponent());
            Assert.That(query2.GetEntityCount(), Is.EqualTo(1));
        }

        private struct ExampleComponent {
            public int someData;
        }

        private struct OtherComponent {
            public int someData;
        }
    }
}