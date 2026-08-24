using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    /// <summary>
    /// کلاس پایه برای تمام Value Object ها در دامنه.
    /// این کلاس منطق مقایسه و تولید HashCode را پیاده‌سازی می‌کند.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// این متد باید توسط کلاس‌های فرزند پیاده‌سازی شود
        /// و اجزای اصلی که برای مقایسه برابری استفاده می‌شوند را برگرداند.
        /// </summary>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        /// <summary>
        /// مقایسه دو شیء بر اساس اجزای مقایسه‌ای.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// تولید HashCode برای شیء فعلی بر اساس اجزای مقایسه‌ای.
        /// </summary>
        public override int GetHashCode()
        {
            var components = GetEqualityComponents().ToArray();
            return HashCode.Combine(components);
        }

        /// <summary>
        /// Operator == برای مقایسه دو Value Object
        /// </summary>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Operator != برای مقایسه دو Value Object
        /// </summary>
        public static bool operator !=(ValueObject? left, ValueObject? right)
            => !(left == right);
    }
}
