using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class WorldTests {
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
                world.Entity();

            world.Destroy();
            Assert.Pass();
        }
        
        [Test]
        public void CreateAndDestroy_WithEntitiesThatHaveComponents() {
            var world = new World();

            for (var i = 0; i < 5; i++)
                world.Entity().Set(new TestComp1());

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
            var query1 = world.Query().With<TestComp1>().AsSet();
            var query2 = world.Query().With<TestComp2>().AsSet();

            for (var i = 0; i < 5; i++)
                world.Entity().Set(new TestComp1());
            
            for (var i = 0; i < 5; i++)
                world.Entity().Set(new TestComp2());
            
            query1.Dispose();
            query2.Dispose();
            world.Destroy();
            Assert.Pass();
        }
    }
}