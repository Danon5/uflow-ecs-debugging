using System;
using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class Tests {
        private World m_world;

        [SetUp]
        public void SetUp() {
            m_world = new World();
        }

        [TearDown]
        public void TearDown() {
            m_world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }

        [Test]
        public void AddComponentTest() {
            var entity = m_world.Entity();
            entity.Set<ExampleComponent>();
            Assert.That(entity.Has<ExampleComponent>());
        }

        [Test]
        public void SetComponentDataTest() {
            var entity = m_world.Entity();
            entity.Set(new ExampleComponent {
                someData = 5
            });
            Assert.That(entity.Get<ExampleComponent>().someData, Is.EqualTo(5));
        }

        [Test]
        public void RemoveComponentTest() {
            var entity = m_world.Entity();
            entity.Set<ExampleComponent>();
            entity.Remove<ExampleComponent>();
            Assert.That(entity.Has<ExampleComponent>(), Is.False);
        }

        [Test]
        public void BasicMultiWorldTest() {
            var world1 = new World();
            var world2 = new World();

            var entity1 = world1.Entity();
            Assert.That(entity1.Has<ExampleComponent>(), Is.False);
            entity1.Set<ExampleComponent>();
            Assert.That(entity1.Has<ExampleComponent>(), Is.True);

            var entity2 = world2.Entity();
            Assert.That(entity2.Has<ExampleComponent>(), Is.False);
            entity2.Set<ExampleComponent>();
            Assert.That(entity2.Has<ExampleComponent>(), Is.True);

            world1.Destroy();
            world2.Destroy();
        }

        [Test]
        public void ManyEntitiesTest() {
            for (var i = 0; i < 10000; i++)
                m_world.Entity();
        }

        [Test]
        public void DestroyEntitiesTest() {
            for (var i = 0; i < 5; i++) {
                var entity = m_world.Entity();
                entity.Set<ExampleComponent>();
                entity.Destroy();
            }

            for (var i = 0; i < 5; i++) {
                var entity = m_world.Entity();
                Assert.That(entity.Has<ExampleComponent>(), Is.False);
            }

            for (var i = 0; i < 5; i++) {
                var entity = m_world.Entity();
                entity.Set<ExampleComponent>();
            }
        }

        [Test]
        public void DestroyWorldTest() {
            var world = new World();

            for (var i = 0; i < 5; i++) {
                var entity = world.Entity();
                entity.Set<ExampleComponent>();
            }

            world.Destroy();
        }

        [Test]
        public void EntityIsValidTest()
        {
            var entity = m_world.Entity();
            Assert.That(entity.IsAlive(), Is.True);
            entity.Destroy();
            Assert.That(entity.IsAlive(), Is.False);
            
            var entity2 = m_world.Entity();
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
            var world = new World();
            Assert.That(world.IsAlive(), Is.True);
            world.Destroy();
            Assert.That(world.IsAlive(), Is.False);
        }

        private DynamicEntitySet m_query2;
        
        [Test]
        public void MultipleQueryTest() {
            var entity1 = m_world.Entity();
            entity1.Set(new ExampleComponent());
            var query1 = m_world.Query().With<ExampleComponent>().AsSet();
            Assert.That(query1.EntityCount, Is.EqualTo(1));
            
            var entity2 = m_world.Entity();
            entity2.Set(new OtherComponent());
            entity2.Set(new ExampleComponent());
            Assert.That(query1.EntityCount, Is.EqualTo(2));
            
            query1.Dispose();

            m_query2 = m_world.Query().With<OtherComponent>().Without<ExampleComponent>().AsSet();
            Assert.That(m_query2.EntityCount, Is.EqualTo(0));

            var entity3 = m_world.Entity();
            entity3.Set(new OtherComponent());
            entity3.Set(new ExampleComponent());
            Assert.That(m_query2.EntityCount, Is.EqualTo(0));
            
            var entity4 = m_world.Entity();
            entity4.Set(new OtherComponent());
            Assert.That(m_query2.EntityCount, Is.EqualTo(1));
        }
        
        [Test]
        public void DestroyQueryTest() {
            var entity1 = m_world.Entity();
            entity1.Set(new ExampleComponent());
            var query1 = m_world.Query().With<ExampleComponent>().AsSet();
            Assert.That(query1.EntityCount, Is.EqualTo(1));
            
            entity1.Destroy();
            Assert.That(query1.EntityCount, Is.EqualTo(0));
            
            var entity2 = m_world.Entity();
            entity2.Set(new OtherComponent());
            var query2 = m_world.Query().WithEither<ExampleComponent>().Or<OtherComponent>().AsSet();
            Assert.That(query2.EntityCount, Is.EqualTo(1));
            
            entity2.Destroy();
            Assert.That(query2.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void StandardUseCaseTest() {
            for (var i = 0; i < 1; i++) {
                var world1 = new World();
                var query = world1.Query().With<ExampleComponent>().With<OtherComponent>().AsSet();
            
                for (var j = 1; j <= 10; j++) {
                    var entity = world1.Entity();
                    entity.Set<ExampleComponent>();
                    if (j % 3 == 0)
                        entity.Set<OtherComponent>();
                }

                foreach (var entity in query) {
                    ref var example = ref entity.Get<ExampleComponent>();
                    ref var other = ref entity.Get<OtherComponent>();
                }
            
                world1.Destroy();
            }
        }

        private struct ExampleComponent {
            public int someData;
        }

        private struct OtherComponent {
            public int someData;
        }
    }
}