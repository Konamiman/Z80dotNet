using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class BitTests
    {
        [Test]
        public void Default_constructor_creates_instance_with_value_zero()
        {
            Assert.That(new Bit().Value, Is.EqualTo(0));
        }

        [Test]
        public void Can_implicitly_convert_to_int()
        {
            int theInt = new Bit(0);
            Assert.That(theInt, Is.EqualTo(0));

            theInt = new Bit(1);
            Assert.That(theInt, Is.EqualTo(1));
        }

        [Test]
        public void Can_implicitly_convert_from_zero_int_to_zero_bit()
        {
            Bit theBit = 0;
            Assert.That(theBit.Value, Is.EqualTo(0));
        }

        [Test]
        public void Can_implicitly_convert_from_any_nonzero_int_to_one_bit()
        {
            Bit theBit = 34;
            Assert.That(theBit.Value, Is.EqualTo(1));
        }
        
        [Test]
        public void Can_implicity_convert_to_and_from_bool()
        {
            bool b1 = new Bit(1);
            bool b2 = new Bit(0);
            Assert.That(b1, Is.True);
            Assert.That(b2, Is.False);

            Bit theTrue = true;
            Bit theFalse = false;
            Assert.Multiple(() =>
            {
                Assert.That(theTrue.Value, Is.EqualTo(1));
                Assert.That(theFalse.Value, Is.EqualTo(0));
            });
        }

        [Test]
        public void Can_create_one_instance_from_another()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.Multiple(() =>
            {
                Assert.That(new Bit(zero), Is.EqualTo(zero));
                Assert.That(new Bit(one), Is.EqualTo(one));
            });
        }

        [Test]
        public void Can_compare_to_other_bit_with_equals_sign()
        {
            Assert.That(new Bit(0) == new Bit(0));
            Assert.That(new Bit(1) == new Bit(1));
            Assert.That(new Bit(0) == new Bit(1), Is.False);
        }

        [Test]
        public void Can_compare_to_other_bit_with_equals_method()
        {
            Assert.That(new Bit(0).Equals(new Bit(0)));
            Assert.That(new Bit(1).Equals(new Bit(1)));
            Assert.That(new Bit(0).Equals(new Bit(1)), Is.False);
        }

        [Test]
        public void Can_compare_for_equality_to_zero_int()
        {
            int zero = 0;
            int nonZero = 34;

            Assert.That(new Bit(0) == zero);
            Assert.That(new Bit(0).Equals(zero));

            Assert.That(zero == new Bit(0));
            Assert.That(zero.Equals(new Bit(0)));

            Assert.That(new Bit(0) == nonZero, Is.False);
            Assert.That(new Bit(0).Equals(nonZero), Is.False);

            Assert.That(nonZero == new Bit(0), Is.False);
            Assert.That(nonZero.Equals(new Bit(0)), Is.False);
        }

        [Test]
        public void Can_compare_for_equality_to_non_zero_int()
        {
            int zero = 0;
            int nonZero = 34;

            Assert.That(new Bit(1) == nonZero);
            Assert.That(new Bit(1).Equals(nonZero));

            Assert.That(nonZero == new Bit(1));
            //Assert.That(nonZero.Equals(new Bit(1)));

            Assert.That(new Bit(1) == zero, Is.False);
            Assert.That(new Bit(1).Equals(zero), Is.False);

            Assert.That(zero == new Bit(1), Is.False);
            //Assert.That(zero.Equals(new Bit(1)), Is.False);
        }

        [Test]
        public void Can_compare_for_inequality_to_zero_int()
        {
            int zero = 0;
            int nonZero = 34;

            Assert.That(new Bit(1) != zero);
            Assert.That(zero != new Bit(1));

            Assert.That(new Bit(1) != nonZero, Is.False);
            Assert.That(nonZero != new Bit(1), Is.False);
        }

        [Test]
        public void Can_convert_to_bool()
        {
            Assert.That((bool)new Bit(0), Is.False);
            Assert.That(new Bit(1));
        }

        [Test]
        public void Can_do_arithmetic_OR()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((bool)(zero | zero), Is.False);
            Assert.That(zero | one);
            Assert.That(one | zero);
            Assert.That(one | one);
        }

        [Test]
        public void Can_do_logical_OR()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((bool)(zero || zero), Is.False);
            Assert.That(zero || one);
            Assert.That(one || zero);
            Assert.That(one || one);
        }

        [Test]
        public void Can_do_arithmetic_AND()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((bool)(zero & zero), Is.False);
            Assert.That((bool)(zero & one), Is.False);
            Assert.That((bool)(one & zero), Is.False);
            Assert.That((bool)(one & one), Is.True);
        }

        [Test]
        public void Can_do_logical_AND()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((bool)(zero && zero), Is.False);
            Assert.That((bool)(zero && one), Is.False);
            Assert.That((bool)(one && zero), Is.False);
            Assert.That(one && one);
        }

        [Test]
        public void Can_do_arithmetic_XOR()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.That((bool)(zero ^ zero), Is.False);
            Assert.That(zero ^ one);
            Assert.That(one ^ zero);
            Assert.That((bool)(one ^ one), Is.False);
        }

        [Test]
        public void Can_do_arithmetic_NOT()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.Multiple(() =>
            {
                Assert.That(~one, Is.EqualTo(zero));
                Assert.That(~zero, Is.EqualTo(one));
            });
        }

        [Test]
        public void Can_do_logical_NOT()
        {
            Bit zero = 0;
            Bit one = 1;

            Assert.Multiple(() =>
            {
                Assert.That(!one, Is.EqualTo(zero));
                Assert.That(!zero, Is.EqualTo(one));
            });
        }
    }
}
