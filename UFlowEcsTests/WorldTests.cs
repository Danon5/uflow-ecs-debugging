using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class WorldTests {
        [TearDown]
        public void TearDown() {
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }
        
        [Test]
        public void CreateAndDestroy_Single() {
            var world = new World();
            world.Destroy();
            Assert.Pass();
        }

        [Test]
        public void CreateAndDestroy_Multiple() {
            var world1 = new World();
            var world2 = new World();
            var world3 = new World();
            world1.Destroy();
            world2.Destroy();
            world3.Destroy();
            Assert.Pass();
        }

        [Test]
        public void CreateAndDestroy_WithEntities() {
            var world = new World();

            for (var i = 0; i < 5; i++)
                world.CreateEntity();

            world.Destroy();
            Assert.Pass();
        }
        
        [Test]
        public void CreateAndDestroy_WithEntitiesThatHaveComponents() {
            var world = new World();

            for (var i = 0; i < 5; i++)
                world.CreateEntity().Set(new TestComp1());

            world.Destroy();
            Assert.Pass();
        }
        
        [Test]
        public void CreateAndDestroy_WithWorldComponents() {
            var world = new World();

            for (var i = 0; i < 5; i++)
                world.Set(new TestComp1());

            world.Destroy();
            Assert.Pass();
        }
        
        [Test]
        public void CreateAndDestroy_WithQueries() {
            var world = new World();
            var query1 = world.BuildQuery().With<TestComp1>().AsSet();
            var query2 = world.BuildQuery().With<TestComp2>().AsSet();

            for (var i = 0; i < 5; i++)
                world.CreateEntity().Set(new TestComp1());
            
            for (var i = 0; i < 5; i++)
                world.CreateEntity().Set(new TestComp2());
            
            query1.Dispose();
            query2.Dispose();
            world.Destroy();
            Assert.Pass();
        }

        [Test]
        public void Set() {
            var world = new World();
            world.Set(new TestComp1());
            world.Destroy();
        }

        [Test]
        public void Set_Has() {
            var world = new World();
            Assert.That(world.Has<TestComp1>(), Is.EqualTo(false));
            world.Set(new TestComp1());
            Assert.That(world.Has<TestComp1>(), Is.EqualTo(true));
            world.Destroy();
        }
        
        [Test]
        public void Set_Get() {
            var world = new World();
            world.Set(new TestComp1 {
                someData = 3
            });
            Assert.That(world.Get<TestComp1>().someData, Is.EqualTo(3));
            world.Destroy();
        }
        
        [Test]
        public void Set_Remove() {
            var world = new World();
            world.Set(new TestComp1());
            world.Remove<TestComp1>();
            world.Destroy();
        }
        
        [Test]
        public void Set_Remove_Has() {
            var world = new World();
            Assert.That(world.Has<TestComp1>(), Is.EqualTo(false));
            world.Set(new TestComp1());
            Assert.That(world.Has<TestComp1>(), Is.EqualTo(true));
            world.Remove<TestComp1>();
            Assert.That(world.Has<TestComp1>(), Is.EqualTo(false));
            world.Destroy();
        }
    }
}