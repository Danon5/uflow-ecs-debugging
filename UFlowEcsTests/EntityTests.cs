using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class EntityTests {
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
        public void Create_Single() {
            m_world.Entity();
            Assert.That(m_world.EntityCount, Is.EqualTo(1));
        }
        
        [Test]
        public void Create_Many() {
            for (var i = 0; i < 1000; i++)
                m_world.Entity();
            Assert.That(m_world.EntityCount, Is.EqualTo(1000));
        }
        
        [Test]
        public void CreateAndDestroy_Single()
        {
            var entity = m_world.Entity();
            Assert.That(m_world.EntityCount, Is.EqualTo(1));
            entity.Destroy();
            Assert.Multiple(() =>
            {
                Assert.That(entity.IsAlive, Is.EqualTo(false));
                Assert.That(m_world.EntityCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CreateAndDestroy_Many() {
            var entities = new Entity[1000];
            for (var i = 0; i < 1000; i++)
                entities[i] = m_world.Entity();
            Assert.That(m_world.EntityCount, Is.EqualTo(1000));
            for (var i = 0; i < 1000; i++) {
                entities[i].Destroy();
                Assert.That(entities[i].IsAlive, Is.EqualTo(false));
            }
            Assert.That(m_world.EntityCount, Is.EqualTo(0));
        }

        [Test]
        public void Set_SingleComponent() {
            var entity = m_world.Entity();
            
            entity.Set(new TestComp1());
            Assert.That(entity.Has<TestComp1>(), Is.EqualTo(true));
        }

        [Test]
        public void Set_MultipleComponents()
        {
            var entity = m_world.Entity();
            
            entity.Set(new TestComp1());
            entity.Set(new TestComp2());
            Assert.Multiple(() =>
            {
                Assert.That(entity.Has<TestComp1>(), Is.EqualTo(true));
                Assert.That(entity.Has<TestComp2>(), Is.EqualTo(true));
            });
        }
        
        [Test]
        public void Get_SingleComponent() {
            var entity = m_world.Entity();
            
            entity.Set(new TestComp1 {
                someData = 1
            });
            
            ref var comp = ref entity.Get<TestComp1>();
            Assert.That(comp.someData, Is.EqualTo(1));
            comp.someData = 2;
            Assert.That(entity.Get<TestComp1>().someData, Is.EqualTo(2));
        }
        
        [Test]
        public void Get_MultipleComponents() {
            var entity = m_world.Entity();
            
            entity.Set(new TestComp1 {
                someData = 1
            });
            entity.Set(new TestComp2 {
                someData = 2
            });
            
            ref var comp1 = ref entity.Get<TestComp1>();
            Assert.That(comp1.someData, Is.EqualTo(1));
            comp1.someData = 3;
            Assert.That(entity.Get<TestComp1>().someData, Is.EqualTo(3));
            
            ref var comp2 = ref entity.Get<TestComp2>();
            Assert.That(comp2.someData, Is.EqualTo(2));
            comp2.someData = 4;
            Assert.That(entity.Get<TestComp2>().someData, Is.EqualTo(4));
        }
        
        [Test]
        public void SetAndRemove_SingleComponent() {
            var entity = m_world.Entity();
            
            entity.Set(new TestComp1());
            Assert.That(entity.Has<TestComp1>(), Is.EqualTo(true));
            entity.Remove<TestComp1>();
            Assert.That(entity.Has<TestComp1>(), Is.EqualTo(false));
        }
        
        [Test]
        public void SetAndRemove_MultipleComponents() {
            var entity = m_world.Entity();
            
            entity.Set(new TestComp1());
            Assert.That(entity.Has<TestComp1>(), Is.EqualTo(true));
            entity.Remove<TestComp1>();
            Assert.That(entity.Has<TestComp1>(), Is.EqualTo(false));
            
            entity.Set(new TestComp2());
            Assert.That(entity.Has<TestComp2>(), Is.EqualTo(true));
            entity.Remove<TestComp2>();
            Assert.That(entity.Has<TestComp2>(), Is.EqualTo(false));
        }
    }
}