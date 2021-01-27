using System;

namespace Torify
{
    public class SalesItem : IEquatable<SalesItem>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string LinkUri { get; set; } = "";

        public string Area => LinkUri.Split('/').Length > 3 ? LinkUri.Split('/')[3] : "";
        public string Date { get; set; }

        public bool Equals(SalesItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Description == other.Description 
                   && Price == other.Price 
                   && LinkUri == other.LinkUri;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SalesItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Price;
                hashCode = (hashCode * 397) ^ (LinkUri != null ? LinkUri.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}