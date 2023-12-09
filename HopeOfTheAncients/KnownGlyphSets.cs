using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;

public static class KnownGlyphSets
{
    public static GlyphSet Milkyway { get; } =
        new GlyphSet("Milkyway", 39,
            new Glyph[] {
                new("Terra", 1,1,'A'),
                new("Crater", 2,2,'B'),
                new("Virgo", 3,3,'C'),
                new("Bootes", 4,4,'D'),
                new("Centaurus", 5,5,'E'),
                new("Libra", 6,6,'F'),
                new("Serpens", 7,7,'G'),
                new("Norma", 8,8,'H'),
                new("Scorpio", 9,9,'I'),
                new("Corona Australis", 10,10,'J'),
                new("Scutum", 11,11,'K'),
                new("Sagittarius", 12,12,'L'),
                new("Aquila", 13,13,'M'),
                new("Microscopium", 14,14,'N'),
                new("Capricornus", 15,15,'O'),
                new("Piscis Austrinus", 16,16,'P'),
                new("Equuleus", 17,17,'Q'),
                new("Aquarius", 18,18,'R'),
                new("Pegasus", 19,19,'S'),
                new("Sculptor", 20,20,'T'),
                new("Pisces", 21,21,'U'),
                new("Andromeda", 22,22,'V'),
                new("Triangulum", 23,23,'W'),
                new("Aries", 24,24,'X'),
                new("Perseus", 25,25,'Y'),
                new("Cetus", 26,26,'Z'),
                new("Taurus", 27,27,'a'),
                new("Auriga", 28,28,'b'),
                new("Eridanus", 29,29,'c'),
                new("Orion", 30,30,'d'),
                new("Canis Minor", 31,31,'e'),
                new("Monoceros", 32,32,'f'),
                new("Gemini", 33,33,'g'),
                new("Hydra", 34,34,'h'),
                new("Lynx", 35,35,'i'),
                new("Cancer", 36,36,'j'),
                new("Sextans", 37,37,'k'),
                new("Leo Minor", 38,38,'l'),
                new("Leo", 39,39,'m'),
            });

    public static GlyphSet Pegasus { get; } = new GlyphSet();

    public static GlyphSet Destiny { get; } = new GlyphSet();
}
