using System;
using System.IO;

namespace HopeOfTheAncients.Tiled
{
    public class IntReader : IDisposable
    {
        private readonly int[] _dataBuffer;
        private int _dataBufferSize;
        private readonly int _bufferSize;
        private readonly Action<int[]> _prebufferFunc;

        private int _csvIndex = 0;

        private readonly Stream? _byteStream;
        
        public IntReader(Stream byteStream, int bufferSize = 256)
        {
            _bufferSize = bufferSize;
            _dataBuffer = new int[bufferSize];
            _byteStream = byteStream;
            var b = new BinaryReader(_byteStream);
            _prebufferFunc = buffer =>
            {
                _dataBufferSize = 0;
                try
                {
                    for (int i = 0; i < _bufferSize; i++)
                    {
                        buffer[i] = b.ReadInt32();
                        _dataBufferSize++;
                    }
                }
                catch(EndOfStreamException)
                {
                    
                }

            };
        }
        public IntReader(string csv, string csvSeparator = ";", int bufferSize = 256)
        {
            _bufferSize = bufferSize;
            _dataBuffer = new int[bufferSize];
            _prebufferFunc = buffer =>
            {
                int index = 0;
                var csvBuffer = csv;
                var bufferSize = _bufferSize;
                var startIndex = _csvIndex;
                var csvSep = csvSeparator;

                if (csvBuffer.Length - startIndex == 0)
                    throw new EndOfStreamException();
                do
                {
                    var f = csvBuffer.IndexOf(csvSep, startIndex, StringComparison.Ordinal);
                    if (f == -1)
                    {
                        f = csvBuffer.Length - 1;
                    }

                    var len = f - startIndex;
                    if (len == 0)
                        throw new EndOfStreamException();

                    var data = csvBuffer.Substring(startIndex, len);
                    if (!int.TryParse(data, out var res))
                        throw new FormatException();

                    buffer[index++] = res;
                    startIndex = f + 1;

                } while (index < bufferSize && startIndex < csvBuffer.Length);

                _csvIndex = startIndex;
            };
        }
        
        public int Position { get; private set; }

        private int PreBuffer()
        {
            var subIndex = Position % _bufferSize;
            if (subIndex == 0)
            {
                // TOOD: prebufer
                _prebufferFunc(_dataBuffer);
            }

            return subIndex;
        }
        
        
        
        public int Read()
        {

            var subIndex = PreBuffer();
            if (subIndex >= _dataBufferSize)
                throw new EndOfStreamException();
            

            ++Position;
            return _dataBuffer[subIndex];
        }

        public int Read(int[] buffer, int startIndex, int count)
        {
            if (startIndex < 0 || startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0 || startIndex + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            //
            //
            // if (Position >= Length)
            //     return -1;
            
            int read = 0;
            while (count > 0)
            {
                var subIndex = PreBuffer();
                if (subIndex >= _dataBufferSize)
                    return read == 0 ? -1 : read;
                var maxCopy = Math.Min(Math.Min(count, _dataBufferSize - subIndex), _bufferSize - subIndex);
                
                Array.Copy(_dataBuffer, subIndex, buffer, startIndex, maxCopy);

                Position += maxCopy;
                read += maxCopy;
                startIndex += maxCopy;
                count -= maxCopy;
            }

            return read;
        }

        public void Dispose()
        {
            _byteStream?.Dispose();
        }
    }
}