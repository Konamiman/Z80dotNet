using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Host
{
    /// <summary>
    /// This class executes MSX-DOS function calls.
    /// Only the absolutely minimum necessary for the stripped down
    /// Disk BASIC ROM (dskbasic.mac) is implemented.
    /// </summary>
    public class DosFunctionCallExecutor
    {
        private const int minFileHandle = 5;
        private const int maxFileHandle = 127;

        private const byte UnauthorizedAccess = 0xB0;   //Not a real MSX-DOS error
        private const byte FileHandleNotOpen = 0xC2;
        private const byte NoSpareFileHandles = 0xC4;
        private const byte EndOfFile = 0xC7;
        private const byte FileInUse = 0xCA;
        private const byte DirectoryNotFound = 0xD6;
        private const byte FileNotFound = 0xD7;
        private const byte InvalidPathname = 0xD9;
        private const byte DiskError = 0xFD;

        private readonly IZ80Registers regs;
        private readonly IMemory memory;
        private readonly Dictionary<byte, FileStream> files = new Dictionary<byte, FileStream>();

        private Dictionary<Type, byte> exceptionsMapping = new Dictionary<Type, byte>
        {
            {typeof(UnauthorizedAccessException), UnauthorizedAccess},
            {typeof(FileHandleNotOpenException), FileHandleNotOpen},
            {typeof(FileNotFoundException), FileNotFound},
            {typeof(DirectoryNotFoundException), DirectoryNotFound},
            {typeof(IOException), FileInUse},
            {typeof(ArgumentException), InvalidPathname},
            {typeof(NotSupportedException), InvalidPathname}
        };

        public DosFunctionCallExecutor(IZ80Registers regs, IMemory memory)
        {
            this.regs = regs;
            this.memory = memory;
        }

        public void ExecuteFunctionCall()
        {
            regs.A = 0;

            try
            {
                switch (regs.C)
                {
                    case 0x43:
                        Open();
                        break;
                    case 0x44:
                        Create();
                        break;
                    case 0x45:
                        Close();
                        break;
                    case 0x48:
                        Read();
                        break;
                    case 0x49:
                        Write();
                        break;
                    case 0x4A:
                        Seek();
                        break;
                }
            }
            catch(Exception ex)
            {
                var exType = ex.GetType();
                if(exceptionsMapping.ContainsKey(exType))
                    regs.A = exceptionsMapping[exType];
                else
                    regs.A = DiskError;
            }

            regs.ZF = (regs.A == 0);
        }

        private void Open()
        {
            var fileName = GetFilenamePointedByDE();

            var fileHandle = GetFreeFileHandle();
            if(fileHandle == 0)
            {
                regs.A = NoSpareFileHandles;
                return;
            }

            var stream = File.Open(fileName, FileMode.Open);

            files.Add(fileHandle, stream);
            regs.B = fileHandle;
        }

        private byte GetFreeFileHandle()
        {
            for(byte h = minFileHandle; h<=maxFileHandle; h++)
                if (!files.Keys.Contains(h))
                    return h;

            return 0;
        }

        private string GetFilenamePointedByDE()
        {
            var pointer = (ushort)regs.DE;
            var stringBytes = new List<byte>();
            while(memory[pointer] != 0)
            {
                stringBytes.Add(memory[pointer]);
                pointer++;
            }

            var filename = Encoding.ASCII.GetString(stringBytes.ToArray());
            if(!Path.IsPathRooted(filename))
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "NestorMSX\\FileSystem");
                if(!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                filename = Path.Combine(folder, filename);
            }

            return filename;
        }

        private void Create()
        {
            var fileName = GetFilenamePointedByDE();

            var fileHandle = GetFreeFileHandle();
            if(fileHandle == 0)
            {
                regs.A = NoSpareFileHandles;
                return;
            }

            var stream = File.Create(fileName);

            files.Add(fileHandle, stream);
            regs.B = fileHandle;
        }

        private void Close()
        {
            var fileHandle = regs.B;
            if(!files.Keys.Contains(fileHandle))
            {
                return;
            }

            var file = files[fileHandle];
            file.Close();
            files.Remove(fileHandle);
        }

        private void Read()
        {
            var file = GetOpenFile();
            var count = regs.HL.ToUShort();
            var dest = regs.DE.ToUShort();
            ushort actualCount = 0;

            while(count>0)
            {
                var datum = file.ReadByte();
                if(datum == -1)
                    break;

                memory[dest] = (byte)datum;
                dest++;
                count--;
                actualCount++;
            }

            regs.HL = actualCount.ToShort();
            if(count > 0 && actualCount == 0)
                regs.A = EndOfFile;
        }

        private void Write()
        {
            var file = GetOpenFile();
            var count = regs.HL.ToUShort();
            var src = regs.DE.ToUShort();

            regs.HL = 0;
            while(count>0)
            {
                var datum = memory[src];
                file.WriteByte(datum);
                src++;
                count--;
            }

            regs.HL = count.ToShort();
        }

        private void Seek()
        {
            var file = GetOpenFile();
            long offset = (regs.DE.ToUShort() << 16) | regs.HL.ToUShort();
            var origin = (SeekOrigin)regs.A;

            file.Seek(offset, origin);
        }

        private FileStream GetOpenFile()
        {
            var fileHandle = regs.B;
            if(!files.Keys.Contains(fileHandle))
            {
                throw new FileHandleNotOpenException();
            }

            return files[fileHandle];
        }

        class FileHandleNotOpenException : Exception
        {
        }
    }
}
