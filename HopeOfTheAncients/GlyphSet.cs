using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;

public readonly struct GlyphSet : IEquatable<GlyphSet>
{
    public readonly string Name { get;  }
    public readonly int Count { get; }

    public readonly ReadOnlySpan<Glyph> Glyphs => glyphs;

    private readonly Glyph[] glyphs;

    public GlyphSet(string name, int count, Glyph[] glyphs) : this()
    {
        Name = name;
        Count = count;
        this.glyphs = glyphs;
    }

    public override bool Equals(object? obj) 
        => obj is GlyphSet set && Equals(set);

    public bool Equals(GlyphSet other) 
        => Name == other.Name 
        && Count == other.Count 
        && Glyphs.SequenceEqual(other.Glyphs);

    public override int GetHashCode() 
        => HashCode.Combine(Name, Count, glyphs);

    public static bool operator ==(GlyphSet left, GlyphSet right)
        => left.Equals(right);

    public static bool operator !=(GlyphSet left, GlyphSet right) 
        => !(left == right);
}
