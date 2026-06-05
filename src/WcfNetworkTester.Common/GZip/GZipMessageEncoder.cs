using System;
using System.IO;
using System.IO.Compression;
using System.ServiceModel.Channels;
using System.Xml;

namespace WcfNetworkTester.Common.GZip
{
    /// <summary>
    /// WCF message encoder that wraps an inner text encoder with GZip compression.
    /// On write: inner encoder serialises → GZip-compressed bytes sent on wire.
    /// On read:  GZip-decompressed bytes → inner encoder deserialises.
    /// </summary>
    internal sealed class GZipMessageEncoder : MessageEncoder
    {
        private readonly MessageEncoder _innerEncoder;

        public GZipMessageEncoder(MessageEncoder innerEncoder)
        {
            _innerEncoder = innerEncoder ?? throw new ArgumentNullException(nameof(innerEncoder));
        }

        public override string ContentType => _innerEncoder.ContentType;
        public override string MediaType   => _innerEncoder.MediaType;
        public override MessageVersion MessageVersion => _innerEncoder.MessageVersion;

        // ── Buffer-based read (HTTP) ────────────────────────────────────────────

        public override Message ReadMessage(
            ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] decompressed = Decompress(buffer.Array, buffer.Offset, buffer.Count);
            var decompressedSegment = new ArraySegment<byte>(decompressed);
            return _innerEncoder.ReadMessage(decompressedSegment, bufferManager, contentType);
        }

        // ── Stream-based read ───────────────────────────────────────────────────

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            // Leave outer stream open; GZipStream disposes only its own internal state.
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true))
            using (var ms = new MemoryStream())
            {
                gzipStream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return _innerEncoder.ReadMessage(ms, maxSizeOfHeaders, contentType);
            }
        }

        // ── Buffer-based write (HTTP) ───────────────────────────────────────────

        public override ArraySegment<byte> WriteMessage(
            Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            // Write uncompressed into a temporary buffer.
            ArraySegment<byte> uncompressed = _innerEncoder.WriteMessage(
                message, maxMessageSize, bufferManager, 0);

            byte[] compressed = Compress(uncompressed.Array, uncompressed.Offset, uncompressed.Count);
            bufferManager.ReturnBuffer(uncompressed.Array);

            int totalLength = messageOffset + compressed.Length;
            byte[] result = bufferManager.TakeBuffer(totalLength);
            Array.Copy(compressed, 0, result, messageOffset, compressed.Length);

            return new ArraySegment<byte>(result, messageOffset, compressed.Length);
        }

        // ── Stream-based write ──────────────────────────────────────────────────

        public override void WriteMessage(Message message, Stream stream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
            {
                _innerEncoder.WriteMessage(message, gzipStream);
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private static byte[] Compress(byte[] data, int offset, int count)
        {
            using (var output = new MemoryStream())
            {
                using (var gz = new GZipStream(output, CompressionMode.Compress))
                {
                    gz.Write(data, offset, count);
                }
                return output.ToArray();
            }
        }

        private static byte[] Decompress(byte[] data, int offset, int count)
        {
            using (var input  = new MemoryStream(data, offset, count))
            using (var gz     = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gz.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}
