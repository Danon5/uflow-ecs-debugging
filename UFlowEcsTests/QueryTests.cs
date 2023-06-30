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
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            m_world.CreateEntity().Set(new TestComp2());
            using var query = m_world.BuildQuery().With<TestComp1>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Single_PostInitialize() {
            using var query = m_world.BuildQuery().With<TestComp1>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            m_world.CreateEntity().Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Multiple_PreInitialize() {
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            entity1.Set(new TestComp2());
            m_world.CreateEntity().Set(new TestComp2());
            using var query = m_world.BuildQuery().With<TestComp1>().With<TestComp2>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Multiple_PostInitialize() {
            using var query = m_world.BuildQuery().With<TestComp1>().With<TestComp2>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            m_world.CreateEntity().Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void Set_With_Without_PreInitialize() {
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp1());
            entity2.Set(new TestComp2());
            using var query = m_world.BuildQuery().With<TestComp1>().Without<TestComp2>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Remove<TestComp2>();
            Assert.That(query.EntityCount, Is.EqualTo(2));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_With_Without_PostInitialize() {
            using var query = m_world.BuildQuery().With<TestComp1>().Without<TestComp2>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp1());
            entity2.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Remove<TestComp2>();
            Assert.That(query.EntityCount, Is.EqualTo(2));
            entity1.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void Set_WhenAdded_PostInitialize() {
            using var query = m_world.BuildQuery().WhenAdded<TestComp1>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(2));
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_WhenEnabled_PostInitialize() {
            using var query = m_world.BuildQuery().WhenEnabled<TestComp1>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(2));
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        
        [Test]
        public void Set_WhenDisabled_PostInitialize() {
            using var query = m_world.BuildQuery().WhenDisabled<TestComp1>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Disable<TestComp1>();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Disable<TestComp2>();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            entity2.Disable<TestComp1>();
            Assert.That(query.EntityCount, Is.EqualTo(2));
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_WhenRemoved_PostInitialize() {
            using var query = m_world.BuildQuery().WhenRemoved<TestComp1>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Disable<TestComp1>();
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Remove<TestComp2>();
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Remove<TestComp1>();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }
        
        [Test]
        public void Set_When_With_PostInitialize() {
            using var query = m_world.BuildQuery().WhenAdded<TestComp1>().With<TestComp2>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            query.ResetCache();
            Assert.That(query.EntityCount, Is.EqualTo(0));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity2.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void Set_When_Without_PostInitialize() {
            using var query = m_world.BuildQuery().WhenAdded<TestComp1>().Without<TestComp2>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity1.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            entity2.Set(new TestComp1());
            Assert.That(query.EntityCount, Is.EqualTo(1));
        }

        [Test]
        public void Set_WithEither_PreInitialize() {
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            entity1.Set(new TestComp3());
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp2());
            var entity3 = m_world.CreateEntity();
            entity3.Set(new TestComp3());
            using var query = m_world.BuildQuery().WithEither<TestComp1>().Or<TestComp2>().AsSet();
            Assert.That(query.EntityCount, Is.EqualTo(2));
        }
        
        [Test]
        public void Set_WithEither_PostInitialize() {
            using var query = m_world.BuildQuery().WithEither<TestComp1>().Or<TestComp2>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            entity1.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp2());
            var entity3 = m_world.CreateEntity();
            entity3.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(2));
        }
        
        [Test]
        public void Set_WithEither_WhenAddedEither_PostInitialize() {
            using var query = m_world.BuildQuery().WithEither<TestComp1>().Or<TestComp2>().EndEither().WhenAdded<TestComp2>().AsSet();
            var entity1 = m_world.CreateEntity();
            entity1.Set(new TestComp1());
            entity1.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(0));
            var entity2 = m_world.CreateEntity();
            entity2.Set(new TestComp2());
            Assert.That(query.EntityCount, Is.EqualTo(1));
            var entity3 = m_world.CreateEntity();
            entity3.Set(new TestComp3());
            Assert.That(query.EntityCount, Is.EqualTo(1));
        }
    }
}