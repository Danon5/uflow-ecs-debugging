using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace DanonEcsTests {
    [TestFixture]
    public class SparseArrayTests {
        private SparseArray<int> m_sparseArray;

        [SetUp]
        public void SetUp() {
            m_sparseArray = new SparseArray<int>();
        }

        [TearDown]
        public void TearDown() {
            m_sparseArray.Dispose();
        }
        
        [Test]
        public void Set_Empty_Single() {
            m_sparseArray.Set(1, 1);
        }

        [Test]
        public void Set_Empty_Multiple() {
            m_sparseArray.Set(1, 1);
            m_sparseArray.Set(2, 2);
            m_sparseArray.Set(3, 3);
        }
        
        [Test]
        public void Set_NonEmpty_Single() {
            m_sparseArray.Set(1, 1);
            m_sparseArray.Set(1, 2);
        }

        [Test]
        public void Set_NonEmpty_Multiple() {
            m_sparseArray.Set(1, 1);
            m_sparseArray.Set(1, 2);
            m_sparseArray.Set(2, 3);
            m_sparseArray.Set(2, 4);
        }
        
        [Test]
        public void Set_Has_Empty_Single()
        {
            m_sparseArray.Set(1, 1);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseArray.Has(1), Is.EqualTo(true));
                Assert.That(m_sparseArray.Has(2), Is.EqualTo(false));
            });
        }

        [Test]
        public void Set_Has_Empty_Multiple()
        {
            m_sparseArray.Set(1, 1);
            Assert.That(m_sparseArray.Has(1), Is.EqualTo(true));
            m_sparseArray.Set(2, 2);
            Assert.That(m_sparseArray.Has(2), Is.EqualTo(true));
            m_sparseArray.Set(3, 3);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseArray.Has(3), Is.EqualTo(true));
                Assert.That(m_sparseArray.Has(4), Is.EqualTo(false));
            });
        }
        
        [Test]
        public void Set_Get_NonEmpty_Single() {
            m_sparseArray.Set(1, 1);
            Assert.That(m_sparseArray.Get(1), Is.EqualTo(1));
            m_sparseArray.Set(1, 2);
            Assert.That(m_sparseArray.Get(1), Is.EqualTo(2));
        }

        [Test]
        public void Get_Single() {
            m_sparseArray.Set(1, 1);
            Assert.That(m_sparseArray.Get(1), Is.EqualTo(1));
        }
        
        [Test]
        public void Get_Multiple() {
            m_sparseArray.Set(1, 1);
            Assert.That(m_sparseArray.Get(1), Is.EqualTo(1));
            m_sparseArray.Set(2, 2);
            Assert.That(m_sparseArray.Get(2), Is.EqualTo(2));
            m_sparseArray.Set(3, 3);
            Assert.That(m_sparseArray.Get(3), Is.EqualTo(3));
        }

        [Test]
        public void Set_Remove_Single() {
            m_sparseArray.Set(1, 1);
            Assert.That(m_sparseArray.Has(1), Is.EqualTo(true));
            m_sparseArray.Remove(1);
            Assert.That(m_sparseArray.Has(1), Is.EqualTo(false));
        }
        
        [Test]
        public void Set_Remove_Multiple()
        {
            m_sparseArray.Set(1, 1);
            m_sparseArray.Set(2, 2);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseArray.Has(1), Is.EqualTo(true));
                Assert.That(m_sparseArray.Has(2), Is.EqualTo(true));
            });
            m_sparseArray.Remove(2);
            Assert.Multiple(() =>
            {
                Assert.That(m_sparseArray.Has(1), Is.EqualTo(true));
                Assert.That(m_sparseArray.Has(2), Is.EqualTo(false));
            });
            m_sparseArray.Set(1, 1);
            Assert.That(m_sparseArray.Has(1), Is.EqualTo(true));
        }
    }
}