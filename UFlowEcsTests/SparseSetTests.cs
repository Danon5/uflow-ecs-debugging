using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class SparseSetTests {
        private SparseSet<int> m_set;

        [SetUp]
        public void SetUp() {
            m_set = new SparseSet<int>();
        }

        [TearDown]
        public void TearDown() {
            m_set.Dispose();
        }
        
        
    }
}