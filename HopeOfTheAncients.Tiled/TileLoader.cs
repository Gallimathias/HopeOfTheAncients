using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using HopeOfTheAncients.Tiled.Schemas;
using Encoding = System.Text.Encoding;

namespace HopeOfTheAncients.Tiled
{
    public class TileLoader
    {
        private static IntReader LoadLayerData(TileLayer layer)
        {
            return LoadData(layer.data);
        }

        private static IntReader LoadData(Data data)
        {
            switch (data.encoding)
            {
                case Schemas.Encoding.base64:
                    var m = new MemoryStream(Encoding.UTF8.GetBytes(data.Value));
                    var c = new CryptoStream(m, new FromBase64Transform(), CryptoStreamMode.Read, false);
                    Stream stream = c;
                    if (data.compressionSpecified)
                    {
                        switch (data.compression)
                        {
                            case Compression.gzip:
                                stream = new GZipStream(c, CompressionMode.Decompress, false);
                                break;
                            case Compression.zlib:
                                var cmf = c.ReadByte();
                                var flags = c.ReadByte();
                                if (cmf == -1 || flags == -1)
                                    throw new EndOfStreamException("No ZLib header");
                                if ((cmf >> 4) != 0x7)
                                    throw new FormatException("Invalid window size");
                                if ((cmf & 0xF) != 0x8)
                                    throw new FormatException("Only deflate supported");
                                if (flags == 0x9C || flags == 0xDA)
                                    stream = new DeflateStream(c, CompressionMode.Decompress, false);
                                else if (flags != 0x01)
                                    throw new FormatException("Unsupported compression info");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    return new IntReader(stream);
                case Schemas.Encoding.csv:
                    if (data.compressionSpecified)
                        throw new NotSupportedException("CSV encoding in combination with compression not supported!");
                    return new IntReader(data.Value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.encoding));
            }
        }
        public static TileSet LoadTileset(string file)
        {
            var r = new XmlRootAttribute("tileset");
            var ser = new XmlSerializer(typeof(Schemas.TileSet), r);
            using var fs = File.OpenRead(file);
            var ts = (Schemas.TileSet)(ser.Deserialize(fs) ?? throw new InvalidDataException());
            var res = new TileSet(ts.tile.Length);

            for (int i=0;i<ts.tile.Length;i++)
            {
                res.Tiles[i] = ts.tile[i].image.source;
            }

            return res;
        }
        public static Map Load(FileInfo file)
        {
            var r = new XmlRootAttribute("map");
            var ser = new XmlSerializer(typeof(Schemas.Map), r);
            using var fs = File.OpenRead(file.FullName);
            var mp = (Schemas.Map)(ser.Deserialize(fs) ?? throw new InvalidDataException());

            var tileSets = new TileSet[mp.tileset.Length];

            for (int i = 0; i < mp.tileset.Length; i++)
            {
                var path = Path.Combine(file.Directory?.FullName ?? ".", mp.tileset[i].source);
                if (File.Exists(path))
                {
                    var ts = LoadTileset(path);
                    tileSets[i] = ts;
                }
            }
            var res = new Map(tileSets, mp.Layers.Length);

            switch (mp.orientation)
            {
                case Orientation.orthogonal:

                    break;
            }

            for (int i = 0; i < mp.Layers.Length; i++)
            {
                if (mp.Layers[i] is TileLayer tileLayer)
                {
                    var l = LoadLayerData(tileLayer);


                    var resLayer = new Map.TileLayer(tileLayer.width, tileLayer.height);
                    for (int j = 0; j < resLayer.Data.Length; j++)
                        resLayer.Data[j] = -1;

                    int read = l.Read(resLayer.Data, 0, resLayer.Data.Length);

                    res.Layers[i] = resLayer;
                }
            }



            return res;
        }
    }
}