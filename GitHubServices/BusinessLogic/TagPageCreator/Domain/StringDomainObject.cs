using System;
using System.Collections;
using System.Collections.Generic;

namespace GitHubServices.BusinessLogic.TagPageCreator.Domain
{
    [Serializable]
    public class StringDomainObject<T> : IEquatable<StringDomainObject<T>>, IComparable<StringDomainObject<T>>, IEnumerable<char>
        where T : StringDomainObject<T>
    {
        protected readonly string value;

        protected StringDomainObject(string value)
        {
            this.value = value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StringDomainObject<T>);
        }

        public bool Equals(StringDomainObject<T> other)
        {
            return string.Equals(value, other.value);
        }

        public int CompareTo(StringDomainObject<T> other)
        {
            return value.CompareTo(other);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<char> GetEnumerator()
        {
            return value.GetEnumerator();
        }

        public override string ToString()
        {
            return value;
        }

        public static T operator +(StringDomainObject<T> a, StringDomainObject<T> b)
        {
            return a.New(a.value + b.value);
        }
        public static bool operator ==(StringDomainObject<T> a, StringDomainObject<T> b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(StringDomainObject<T> a, StringDomainObject<T> b)
        {
            return !(a == b);
        }

        protected virtual T New(string a)
        {
            throw new NotImplementedException("Must be implemented in subclasses");
        }
    }
}