namespace Pckgs
{
    using NUnit.Framework;
    using System.Collections.Generic;

    public class PaginatedDataTests
    {
        private PaginatedData<string> CreateSampleData(int from, int perPage, int total)
        {
            return new PaginatedData<string>
            {
                From = from,
                PerPage = perPage,
                Total = total,
                Data = new List<string>(),
            };
        }

        [Test]
        public void GetCurrentPage_ShouldCalculateCorrectly()
        {
            var data = CreateSampleData(20, 10, 100);
            Assert.AreEqual(3, data.GetCurrentPage());
        }

        [Test]
        public void GetTotalPages_ShouldCalculateCorrectly()
        {
            var data = CreateSampleData(0, 10, 95);
            Assert.AreEqual(10, data.GetTotalPages());
        }

        [Test]
        public void GetPaginationPages_TypicalMiddlePage()
        {
            var data = CreateSampleData(40, 10, 100); // Page 5 of 10
            var expected = new List<string> { "1", "...", "4", "5", "6", "...", "10" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_FirstPage()
        {
            var data = CreateSampleData(0, 10, 50); // Page 1 of 5
            var expected = new List<string> { "1", "2", "...", "5" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_LastPage()
        {
            var data = CreateSampleData(90, 10, 100); // Page 10 of 10
            var expected = new List<string> { "1", "...", "9", "10" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_SecondPage()
        {
            var data = CreateSampleData(10, 10, 100); // Page 2
            var expected = new List<string> { "1", "2", "3", "...", "10" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_SecondLastPage()
        {
            var data = CreateSampleData(80, 10, 100); // Page 9 of 10
            var expected = new List<string> { "1", "...", "8", "9", "10" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_PageCountIsThree()
        {
            var data = CreateSampleData(10, 10, 30); // Page 2 of 3
            var expected = new List<string> { "1", "2", "3" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_PageCountIsOne()
        {
            var data = CreateSampleData(0, 10, 1); // 1 page only
            var expected = new List<string> { "1" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_PageCountIsTwo()
        {
            var data = CreateSampleData(10, 10, 15); // Page 2 of 2
            var expected = new List<string> { "1", "2" };
            CollectionAssert.AreEqual(expected, data.GetPaginationPages());
        }

        [Test]
        public void GetPaginationPages_ZeroItems()
        {
            var data = new PaginatedData<object>
            {
                Total = 0,
                PerPage = 10,
                From = 0,
                Size = 0
            };

            var pages = data.GetPaginationPages();
            var expected = new List<string> { "1" };
            Assert.AreEqual(expected, pages);
        }

    }

}