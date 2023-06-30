using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class SparseSetTests {
        private SparseSet<int> m_sparseSet;

        [SetUp]
        public void SetUp() {
            m_sparseSet = new SparseSet<int>();
        }

        [TearDown]
        public void TearDown() {
            m_sparseSet.Dispose();
        }
        
        [Test]
        public void Set_Empty_Single() {
            m_sparseSet.Set(1, 1);
        }

        [Test]
        public void Set_Empty_Multiple() {
            m_sparseSet.Set(1, 1);
            m_sparseSet.Set(2, 2);
            m_sparseSet.Set(3, 3);
        }
        
        [Test]
        public void Set_NonEmpty_Single() {
            m_sparseSet.Set(1, 1);
            m_sparseSet.Set(1, 2);
        }

        [Test]
        public void Set_NonEmpty_Multiple() {
            m_sparseSet.Set(1, 1);
            m_sparseSet.Set(1, 2);
            m_sparseSet.Set(2, 3);
            m_sparseSet.Set(2, 4);
        }
        
        [Test]
        public void Set_AndHas_Empty_Single()
        {
            m_sparseSet.Set(1, 1);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseSet.Has(1), Is.EqualTo(true));
                Assert.That(m_sparseSet.Has(2), Is.EqualTo(false));
            });
        }

        [Test]
        public void Set_AndHas_Empty_Multiple()
        {
            m_sparseSet.Set(1, 1);
            Assert.That(m_sparseSet.Has(1), Is.EqualTo(true));
            m_sparseSet.Set(2, 2);
            Assert.That(m_sparseSet.Has(2), Is.EqualTo(true));
            m_sparseSet.Set(3, 3);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseSet.Has(3), Is.EqualTo(true));
                Assert.That(m_sparseSet.Has(4), Is.EqualTo(false));
            });
        }
        
        [Test]
        public void Set_Get_NonEmpty_Single() {
            m_sparseSet.Set(1, 1);
            Assert.That(m_sparseSet.Get(1), Is.EqualTo(1));
            m_sparseSet.Set(1, 2);
            Assert.That(m_sparseSet.Get(1), Is.EqualTo(2));
        }

        [Test]
        public void Get_Single() {
            m_sparseSet.Set(1, 1);
            Assert.That(m_sparseSet.Get(1), Is.EqualTo(1));
        }
        
        [Test]
        public void Get_Multiple() {
            m_sparseSet.Set(1, 1);
            Assert.That(m_sparseSet.Get(1), Is.EqualTo(1));
            m_sparseSet.Set(2, 2);
            Assert.That(m_sparseSet.Get(2), Is.EqualTo(2));
            m_sparseSet.Set(3, 3);
            Assert.That(m_sparseSet.Get(3), Is.EqualTo(3));
        }

        [Test]
        public void Set_Remove_Single() {
            m_sparseSet.Set(1, 1);
            Assert.That(m_sparseSet.Has(1), Is.EqualTo(true));
            m_sparseSet.Remove(1);
            Assert.That(m_sparseSet.Has(1), Is.EqualTo(false));
        }
        
        [Test]
        public void Set_Remove_Multiple()
        {
            m_sparseSet.Set(1, 1);
            m_sparseSet.Set(2, 2);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseSet.Has(1), Is.EqualTo(true));
                Assert.That(m_sparseSet.Has(2), Is.EqualTo(true));
            });
            m_sparseSet.Remove(2);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseSet.Has(1), Is.EqualTo(true));
                Assert.That(m_sparseSet.Has(2), Is.EqualTo(false));
            });
            m_sparseSet.Set(1, 1);
            Assert.That(m_sparseSet.Has(1), Is.EqualTo(true));
        }
    }
}