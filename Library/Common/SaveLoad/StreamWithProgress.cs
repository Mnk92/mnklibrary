using System;
using System.IO;

namespace Mnk.Library.Common.SaveLoad
{
    public class StreamWithProgress : Stream
    {
        private readonly Stream file;
        private readonly long length;
        private long bytesRead;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        public StreamWithProgress(Stream file)
        {
            this.file = file;
            length = file.Length;
            bytesRead = 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush() { }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => bytesRead;
            set => throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = file.Read(buffer, offset, count);
            bytesRead += result;
            if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(bytesRead, length));
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
