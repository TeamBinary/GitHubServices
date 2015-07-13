using System;
using System.Collections;
using System.Collections.Generic;

namespace GitHubServices.BusinessLogic.TagPageCreator.Domain
{
    public abstract class StringDomainObject<T> : IEquatable<StringDomainObject<T>>, IComparable<StringDomainObject<T>>, IEnumerable<char>
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
    }
}