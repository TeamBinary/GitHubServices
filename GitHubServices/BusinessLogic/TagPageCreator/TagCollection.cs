using System;
using System.Collections;
using System.Collections.Generic;

using GitHubServices.BusinessLogic.TagPageCreator.Domain;

namespace GitHubServices.BusinessLogic.TagPageCreator
{
    public class TagCollection : IEnumerable<KeyValuePair<Tag, List<Page>>>
    {
        readonly Dictionary<Tag, List<Page>> Tags = new Dictionary<Tag, List<Page>>();
        readonly Dictionary<string, Tag> lowerCaseDistinct = new Dictionary<string, Tag>();
        private IEnumerable<Tuple<Tag, Page[]>> tags;

        public TagCollection(IEnumerable<Tuple<Tag, Page[]>> tags)
        {
            foreach (var tagInfo in tags)
            {
                Add(tagInfo.Item1, tagInfo.Item2);
            }
        }

        public TagCollection(IEnumerable<TagCollection> tagCollections)
        {
            foreach (var collection in tagCollections)
            {
                Add(collection);
            }
        }

        public void Add(Tag tag, params Page[] url)
        {
            Tag distinctTag;
            if (!lowerCaseDistinct.TryGetValue(tag.Value.ToLower(), out distinctTag))
            {
                distinctTag = tag;
                lowerCaseDistinct.Add(tag.Value.ToLower(), tag);
            }

            List<Page> urls;
            if (!Tags.TryGetValue(distinctTag, out urls))
                Tags.Add(tag, urls = new List<Page>());
            urls.AddRange(url);
        }

        public void Add(TagCollection tagsForPage)
        {
            foreach (var kv in tagsForPage.Tags)
                Add(kv.Key, kv.Value.ToArray());
        }

        public IEnumerator<KeyValuePair<Tag, List<Page>>> GetEnumerator()
        {
            return Tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}