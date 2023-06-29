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
            entity.Set<TestComp1>();
            Assert.That(entity.Has<TestComp1>());
        }

        [Test]
        public void SetComponentDataTest() {
            var entity = m_world.Entity();
            entity.Set(new TestComp1 {
                someData = 5
            });
            
            Assert.That(entity.Get<TestComp1>().someData, Is.EqualTo(5));
        }

        [Test]
        public void RemoveComponentTest() {
            var entity = m_world.Entity();
            entity.Set<TestComp1>();
            entity.Remove<TestComp1>();
            Assert.That(entity.Has<TestComp1>(), Is.False);
        }

        [Test]
        public void BasicMultiWorldTest() {
            var world1 = new World();
            var world2 = new World();
            var entity1 = world1.Entity();
            Assert.That(entity1.Has<TestComp1>(), Is.False);
            
            entity1.Set<TestComp1>();
            Assert.That(entity1.Has<TestComp1>(), Is.True);

            var entity2 = world2.Entity();
            Assert.That(entity2.Has<TestComp1>(), Is.False);
            
            entity2.Set<TestComp1>();
            Assert.That(entity2.Has<TestComp1>(), Is.True);

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
                entity.Set<TestComp1>();
                entity.Destroy();
            }

            for (var i = 0; i < 5; i++) {
                var entity = m_world.Entity();
                Assert.That(entity.Has<TestComp1>(), Is.False);
            }

            for (var i = 0; i < 5; i++) {
                var entity = m_world.Entity();
                entity.Set<TestComp1>();
            }
        }

        [Test]
        public void DestroyWorldTest() {
            var world = new World();

            for (var i = 0; i < 5; i++) {
                var entity = world.Entity();
                entity.Set<TestComp1>();
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
            entity1.Set(new TestComp1());
            var query1 = m_world.Query().With<TestComp1>().AsSet();
            Assert.That(query1.EntityCount, Is.EqualTo(1));
            
            var entity2 = m_world.Entity();
            entity2.Set(new TestComp2());
            entity2.Set(new TestComp1());
            Assert.That(query1.EntityCount, Is.EqualTo(2));
            
            query1.Dispose();
            m_query2 = m_world.Query().With<TestComp2>().Without<TestComp1>().AsSet();
            Assert.That(m_query2.EntityCount, Is.EqualTo(0));

            var entity3 = m_world.Entity();
            entity3.Set(new TestComp2());
            entity3.Set(new TestComp1());
            Assert.That(m_query2.EntityCount, Is.EqualTo(0));
            
            var entity4 = m_world.Entity();
            entity4.Set(new TestComp2());
            Assert.That(m_query2.EntityCount, Is.EqualTo(1));
        }
        
        [Test]
        public void DestroyQueryTest() {
            var entity1 = m_world.Entity();
            entity1.Set(new TestComp1());
            var query1 = m_world.Query().With<TestComp1>().AsSet();
            Assert.That(query1.EntityCount, Is.EqualTo(1));
            
            entity1.Destroy();
            Assert.That(query1.EntityCount, Is.EqualTo(0));
            
            var entity2 = m_world.Entity();
            entity2.Set(new TestComp2());
            var query2 = m_world.Query().WithEither<TestComp1>().Or<TestComp2>().AsSet();
            Assert.That(query2.EntityCount, Is.EqualTo(1));
            
            entity2.Destroy();
            Assert.That(query2.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void StandardUseCaseTest() {
            for (var i = 0; i < 1; i++) {
                var world1 = new World();
                var query = world1.Query().With<TestComp1>().With<TestComp2>().AsSet();
            
                for (var j = 1; j <= 10; j++) {
                    var entity = world1.Entity();
                    entity.Set<TestComp1>();
                    if (j % 3 == 0)
                        entity.Set<TestComp2>();
                }

                foreach (var entity in query) {
                    ref var example = ref entity.Get<TestComp1>();
                    ref var other = ref entity.Get<TestComp2>();
                }
            
                world1.Destroy();
            }
        }

        [Test]
        public void EntityEnumerationCountTest() {
            var firstEntity = m_world.Entity();
            firstEntity.Set(new TestComp1());
            for (var i = 2; i <= 9; i++) {
                m_world.Entity().Set(new TestComp1());
            }

            var lastEntity = m_world.Entity();
            lastEntity.Set(new TestComp1());
            var query = m_world.Query().With<TestComp1>().AsSet();
            var count = 0;
            foreach (var entity in query) {
                count++;
                switch (count) {
                    case 1:
                        Assert.That(entity, Is.EqualTo(firstEntity));
                        break;
                    case 10:
                        Assert.That(entity, Is.EqualTo(lastEntity));
                        break;
                }
            }
            
            Assert.That(count, Is.EqualTo(10));
            
        }

        [Test]
        public void WhenQueryTest() {
            var query = m_world.Query().WhenAdded<TestComp1>().Without<TestComp2>().AsSet();
            var entity = m_world.Entity();
            entity.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            
            entity.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(0));

            var entity2 = m_world.Entity();
            entity2.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            
            var entity3 = m_world.Entity();
            entity3.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(2));
            
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void WhenEitherQueryTest() {
            var query = m_world.Query().WhenEitherAdded<TestComp1>().Or<TestComp2>().AsSet();
            m_world.Entity().Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            
            m_world.Entity().Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(2));
            
            m_world.Entity().Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(2));
            
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void WhenEitherAndWithQueryTest() {
            var query = m_world.Query().With<TestComp3>().WhenEitherAdded<TestComp1>().Or<TestComp2>().AsSet();
            m_world.Entity().Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            
            m_world.Entity().Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            
            var entity = m_world.Entity();
            entity.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(0));

            entity.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void WhenRemovedQueryTest() {
            var query = m_world.Query().WhenRemoved<TestComp3>().AsSet();
            var entity = m_world.Entity();
            entity.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(0));

            entity.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            
            entity.Remove<TestComp3>();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void WhenEitherRemovedQueryTest() {
            var query = m_world.Query().With<TestComp1>().WhenEitherRemoved<TestComp2>().Or<TestComp3>().AsSet();
            var entity = m_world.Entity();
            entity.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(0));

            entity.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            
            entity.Remove<TestComp2>();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            
            query.ResetCache();
            
            entity.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(0));

            entity.Remove<TestComp3>();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void WithoutInitialEntitiesTest() {
            m_world.Entity().Set(new TestComp1());
            m_world.Entity().Set(new TestComp1());
            var query = m_world.Query().With<TestComp1>().WithoutInitialEntities().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(0));
            
            m_world.Entity().Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
        }

        [Test]
        public void RemovingEventTest() {
            var entity = m_world.Entity();
            entity.Set(new TestComp1());
            var called = false;
            var sub = m_world.WhenEntityComponentRemoving((in Entity e, ref TestComp1 c) => called = true);
            entity.Remove<TestComp1>();
            Assert.That(called, Is.EqualTo(true));
            sub.Dispose();
        }

        private struct TestComp1 {
            public int someData;
        }

        private struct TestComp2 {
            public int someData;
        }
        
        private struct TestComp3 {
            public int someData;
        }
    }
}