using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidFactory.Production.Data
{
    struct DATA_Tag
    {
        private readonly string Str;

        public DATA_Tag(string str)
        {
            Str = str;
        }

        public static bool operator ==(DATA_Tag tag1, DATA_Tag tag2)
        {
            return (tag1.Str == tag2.Str);
        }
        public static bool operator !=(DATA_Tag tag1, DATA_Tag tag2)
        {
            return !(tag1 == tag2);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    struct DATA_Tags
    {
        private readonly DATA_Tag[] Tags;

        public DATA_Tags(string[] strs)
        {
            Tags = new DATA_Tag[strs.Length];
            for (int i = 0; i < Tags.Length; i++)
                Tags[i] = new DATA_Tag(strs[i]);
        }

        public static bool operator ==(DATA_Tags tags, DATA_Tag tag)
        {
            for (int i = 0; i < tags.Tags.Length; i++)
            {
                if (tags.Tags[i] == tag)
                    return true;
            }
            return false;
        }
        public static bool operator !=(DATA_Tags tags, DATA_Tag tag)
        {
            return !(tags == tag);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
