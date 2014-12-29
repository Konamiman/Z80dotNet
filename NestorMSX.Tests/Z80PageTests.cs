using System;
using System.Linq;
using Konamiman.NestorMSX.Misc;
using NUnit.Framework;

namespace Konamiman.NestorMSX.Tests
{
    public class Z80PageTests
    {
        public static int[] ValidPageNumbers = { 0, 1, 2, 3 };

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Can_create_instances_with_values_0_to_3(int page)
        {
            var sut = new Z80Page(page);
            Assert.IsNotNull(sut);
        }

        
        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Can_create_instance_from_address(int page)
        {
            var address = (ushort)(page * 16384);
            var sut = Z80Page.FromAddress(address);
            Assert.AreEqual(page, sut.Value);
        }

        [Test]
        public void Cannot_create_instances_with_values_outside_valid_ones()
        {
            var tooSmallValue = ValidPageNumbers.Min() - 1;
            Assert.Throws<InvalidOperationException>(() => new Z80Page(tooSmallValue));

            var tooBigValue = ValidPageNumbers.Max() + 1;
            Assert.Throws<InvalidOperationException>(() => new Z80Page(tooBigValue));
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Page_number_can_be_retrieved_via_PageNumber_property(int page)
        {
            var sut = new Z80Page(page);
            Assert.AreEqual(page, sut.Value);
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Address_mask_can_be_retrieved_via_AddressMask_property(int page)
        {
            var masks = new[] {0x0000, 0x4000, 0x8000, 0xC000};
            var sut = new Z80Page(page);
            Assert.AreEqual(masks[page], sut.AddressMask);
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Two_instances_are_equal_if_page_number_is_the_same(int page)
        {
            var sut1 = new Z80Page(page);
            var sut2 = new Z80Page(page);
            Assert.True(sut1 == sut2);
            Assert.True(sut1.Equals(sut2));
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Two_instances_are_different_if_page_number_is_not_the_same(int page)
        {
            var sut1 = new Z80Page(page);
            var sut2 = new Z80Page((page+1) & 3);
            Assert.True(sut1 != sut2);
            Assert.False(sut1.Equals(sut2));
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Can_be_implicitly_cast_to_int(int page)
        {
            var sut = new Z80Page(page);
            var pageNumber = (int)sut;
            Assert.AreEqual(sut.Value, pageNumber);
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Can_be_implicitly_cast_from_int(int page)
        {
            var sut = (Z80Page)page;
            Assert.AreEqual(page, sut.Value);
        }

        [Test]
        [TestCaseSource("ValidPageNumbers")]
        public void Can_be_compared_to_int(int page)
        {
            var sut = new Z80Page(page);
            Assert.AreEqual(page, sut);

            var otherPage = (page + 1) & 3;
            Assert.AreNotEqual(otherPage, sut);
        }
    }
}
