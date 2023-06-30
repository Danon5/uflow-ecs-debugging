using System;
using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class QueryTests {
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
        public void Set_With_Single_PreInitialize() {
            var entity1 = m_world.Entity();
            entity1.Set(new TestComp1());
            m_world.Entity().Set(new TestComp2());
            using var query = m_world.Query().With<TestComp1>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Single_PostInitialize() {
            using var query = m_world.Query().With<TestComp1>().AsSet();
            var entity1 = m_world.Entity();
            entity1.Set(new TestComp1());
            m_world.Entity().Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Multiple_PreInitialize() {
            var entity1 = m_world.Entity();
            entity1.Set(new TestComp1());
            entity1.Set(new TestComp2());
            m_world.Entity().Set(new TestComp2());
            using var query = m_world.Query().With<TestComp1>().With<TestComp2>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Multiple_PostInitialize() {
            using var query = m_world.Query().With<TestComp1>().With<TestComp2>().AsSet();
            var entity1 = m_world.Entity();
            entity1.Set(new TestComp1());
            m_world.Entity().Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void Set_With_Without_Single_PreInitialize() {
            var entity1 = m_world.Entity();
            entity1.Set(new TestComp1());
            var entity2 = m_world.Entity();
            entity2.Set(new TestComp1());
            entity2.Set(new TestComp2());
            using var query = m_world.Query().With<TestComp1>().Without<TestComp2>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Remove<TestComp2>();
            Assert.That(query.EntityCount, Is.EqualTo(2));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
    }
}