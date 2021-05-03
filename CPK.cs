using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altera
{
    public class CPK
    {
        private readonly Tools tools;
        public Dictionary<string, object> cpkdata;
        private UTF files;
        public List<FileEntry> FileTable;

        public ulong TocOffset, EtocOffset, ItocOffset, GtocOffset, ContentOffset;
        public UTF utf;

        public CPK(Tools tool)
        {
            tools = tool;
            isUtfEncrypted = false;
            FileTable = new List<FileEntry>();
        }

        public bool isUtfEncrypted { get; set; }
        public int unk1 { get; set; }
        public long utf_size { get; set; }
        public byte[] utf_packet { get; set; }

        public byte[] CPK_packet { get; set; }
        public byte[] TOC_packet { get; set; }
        public byte[] ITOC_packet { get; set; }
        public byte[] ETOC_packet { get; set; }
        public byte[] GTOC_packet { get; set; }

        public bool ReadCPK(string sPath)
        {
            if (!File.Exists(sPath)) return false;
            uint Files;
            ushort Align;

            var br = new EndianReader(File.OpenRead(sPath), true);
            MemoryStream ms;
            EndianReader utfr;

            if (tools.ReadCString(br, 4) != "CPK ")
            {
                br.Close();
                return false;
            }

            ReadUTFData(br);

            CPK_packet = utf_packet;

            var CPAK_entry = new FileEntry
            {
                FileName = "CPK_HDR",
                FileOffsetPos = br.BaseStream.Position + 0x10,
                FileSize = CPK_packet.Length,
                Encrypted = isUtfEncrypted,
                FileType = "CPK"
            };

            FileTable.Add(CPAK_entry);

            ms = new MemoryStream(utf_packet);
            utfr = new EndianReader(ms, false);

            utf = new UTF(tools);
            if (!utf.ReadUTF(utfr))
            {
                br.Close();
                return false;
            }

            utfr.Close();
            ms.Close();

            cpkdata = new Dictionary<string, object>();

            try
            {
                for (var i = 0; i < utf.columns.Count; i++)
                    cpkdata.Add(utf.columns[i].name, utf.rows[0].rows[i].GetValue());
            }
            catch (Exception)
            {
                //ignore
            }

            TocOffset = (ulong) GetColumsData2(utf, 0, "TocOffset", 3);
            var TocOffsetPos = GetColumnPostion(utf, 0, "TocOffset");

            EtocOffset = (ulong) GetColumsData2(utf, 0, "EtocOffset", 3);
            var ETocOffsetPos = GetColumnPostion(utf, 0, "EtocOffset");

            ItocOffset = (ulong) GetColumsData2(utf, 0, "ItocOffset", 3);
            var ITocOffsetPos = GetColumnPostion(utf, 0, "ItocOffset");

            GtocOffset = (ulong) GetColumsData2(utf, 0, "GtocOffset", 3);
            var GTocOffsetPos = GetColumnPostion(utf, 0, "GtocOffset");

            ContentOffset = (ulong) GetColumsData2(utf, 0, "ContentOffset", 3);
            var ContentOffsetPos = GetColumnPostion(utf, 0, "ContentOffset");
            FileTable.Add(CreateFileEntry("CONTENT_OFFSET", ContentOffset, typeof(ulong), ContentOffsetPos, "CPK",
                "CONTENT", false));

            Files = (uint) GetColumsData2(utf, 0, "Files", 2);
            Align = (ushort) GetColumsData2(utf, 0, "Align", 1);

            if (TocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                var entry = CreateFileEntry("TOC_HDR", TocOffset, typeof(ulong), TocOffsetPos, "CPK", "HDR", false);
                FileTable.Add(entry);

                if (!ReadTOC(br, TocOffset, ContentOffset))
                    return false;
            }

            if (EtocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                var entry = CreateFileEntry("ETOC_HDR", EtocOffset, typeof(ulong), ETocOffsetPos, "CPK", "HDR",
                    false);
                FileTable.Add(entry);

                if (!ReadETOC(br, EtocOffset))
                    return false;
            }

            if (ItocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                var entry = CreateFileEntry("ITOC_HDR", ItocOffset, typeof(ulong), ITocOffsetPos, "CPK", "HDR",
                    false);
                FileTable.Add(entry);

                if (!ReadITOC(br, ItocOffset, ContentOffset, Align))
                    return false;
            }

            if (GtocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                var entry = CreateFileEntry("GTOC_HDR", GtocOffset, typeof(ulong), GTocOffsetPos, "CPK", "HDR",
                    false);
                FileTable.Add(entry);

                if (!ReadGTOC(br, GtocOffset))
                    return false;
            }

            br.Close();

            files = null;
            return true;
        }

        private FileEntry CreateFileEntry(string FileName, ulong FileOffset, Type FileOffsetType, long FileOffsetPos,
            string TOCName, string FileType, bool encrypted)
        {
            var entry = new FileEntry
            {
                FileName = FileName,
                FileOffset = FileOffset,
                FileOffsetType = FileOffsetType,
                FileOffsetPos = FileOffsetPos,
                TOCName = TOCName,
                FileType = FileType,
                Encrypted = encrypted
            };

            return entry;
        }

        public bool ReadTOC(EndianReader br, ulong TocOffset, ulong ContentOffset)
        {
            ulong add_offset = 0;

            if (ContentOffset < 0)
            {
                add_offset = TocOffset;
            }
            else
            {
                if (TocOffset < 0)
                    add_offset = ContentOffset;
                else
                    add_offset = ContentOffset < TocOffset ? ContentOffset : TocOffset;
            }

            br.BaseStream.Seek((long) TocOffset, SeekOrigin.Begin);

            if (tools.ReadCString(br, 4) != "TOC ")
            {
                br.Close();
                return false;
            }

            ReadUTFData(br);

            TOC_packet = utf_packet;

            var toc_entry = FileTable.Where(x => x.FileName.ToString() == "TOC_HDR").Single();
            toc_entry.Encrypted = isUtfEncrypted;
            toc_entry.FileSize = TOC_packet.Length;

            var ms = new MemoryStream(utf_packet);
            var utfr = new EndianReader(ms, false);

            files = new UTF(tools);
            if (!files.ReadUTF(utfr))
            {
                br.Close();
                return false;
            }

            utfr.Close();
            ms.Close();

            FileEntry temp;
            for (var i = 0; i < files.num_rows; i++)
            {
                temp = new FileEntry
                {
                    TOCName = "TOC",
                    DirName = GetColumnData(files, i, "DirName"),
                    FileName = GetColumnData(files, i, "FileName"),
                    FileSize = GetColumnData(files, i, "FileSize"),
                    FileSizePos = GetColumnPostion(files, i, "FileSize"),
                    FileSizeType = GetColumnType(files, i, "FileSize"),
                    ExtractSize = GetColumnData(files, i, "ExtractSize"),
                    ExtractSizePos = GetColumnPostion(files, i, "ExtractSize"),
                    ExtractSizeType = GetColumnType(files, i, "ExtractSize"),
                    FileOffset = (ulong) GetColumnData(files, i, "FileOffset") + add_offset,
                    FileOffsetPos = GetColumnPostion(files, i, "FileOffset"),
                    FileOffsetType = GetColumnType(files, i, "FileOffset"),
                    FileType = "FILE",
                    Offset = add_offset,
                    ID = GetColumnData(files, i, "ID"),
                    UserString = GetColumnData(files, i, "UserString")
                };

                FileTable.Add(temp);
            }

            files = null;

            return true;
        }

        public void WritePacket(BinaryWriter cpk, string ID, ulong position, byte[] packet)
        {
            if (position == 0xffffffffffffffff) return;
            cpk.BaseStream.Seek((long) position, SeekOrigin.Begin);
            var encrypted = DecryptUTF(packet); // Yes it says decrypt...
            cpk.Write(Encoding.ASCII.GetBytes(ID));
            cpk.Write(0);
            cpk.Write((ulong) encrypted.Length);
            cpk.Write(encrypted);
        }

        public bool ReadITOC(EndianReader br, ulong startoffset, ulong ContentOffset, ushort Align)
        {
            br.BaseStream.Seek((long) startoffset, SeekOrigin.Begin);

            if (tools.ReadCString(br, 4) != "ITOC")
            {
                br.Close();
                return false;
            }

            ReadUTFData(br);

            ITOC_packet = utf_packet;

            var itoc_entry = FileTable.Where(x => x.FileName.ToString() == "ITOC_HDR").Single();
            itoc_entry.Encrypted = isUtfEncrypted;
            itoc_entry.FileSize = ITOC_packet.Length;

            var ms = new MemoryStream(utf_packet);
            var utfr = new EndianReader(ms, false);

            files = new UTF(tools);
            if (!files.ReadUTF(utfr))
            {
                br.Close();
                return false;
            }

            utfr.Close();
            ms.Close();

            var DataL = (byte[]) GetColumnData(files, 0, "DataL");
            var DataLPos = GetColumnPostion(files, 0, "DataL");

            var DataH = (byte[]) GetColumnData(files, 0, "DataH");
            var DataHPos = GetColumnPostion(files, 0, "DataH");

            UTF utfDataL, utfDataH;
            Dictionary<int, uint> SizeTable, CSizeTable;
            Dictionary<int, long> SizePosTable, CSizePosTable;
            Dictionary<int, Type> SizeTypeTable, CSizeTypeTable;

            var IDs = new List<int>();

            SizeTable = new Dictionary<int, uint>();
            SizePosTable = new Dictionary<int, long>();
            SizeTypeTable = new Dictionary<int, Type>();

            CSizeTable = new Dictionary<int, uint>();
            CSizePosTable = new Dictionary<int, long>();
            CSizeTypeTable = new Dictionary<int, Type>();

            ushort ID, size1;
            uint size2;
            long pos;
            Type type;

            if (DataL != null)
            {
                ms = new MemoryStream(DataL);
                utfr = new EndianReader(ms, false);
                utfDataL = new UTF(tools);
                utfDataL.ReadUTF(utfr);

                for (var i = 0; i < utfDataL.num_rows; i++)
                {
                    ID = (ushort) GetColumnData(utfDataL, i, "ID");
                    size1 = (ushort) GetColumnData(utfDataL, i, "FileSize");
                    SizeTable.Add(ID, size1);

                    pos = GetColumnPostion(utfDataL, i, "FileSize");
                    SizePosTable.Add(ID, pos + DataLPos);

                    type = GetColumnType(utfDataL, i, "FileSize");
                    SizeTypeTable.Add(ID, type);

                    if (GetColumnData(utfDataL, i, "ExtractSize") != null)
                    {
                        size1 = (ushort) GetColumnData(utfDataL, i, "ExtractSize");
                        CSizeTable.Add(ID, size1);

                        pos = GetColumnPostion(utfDataL, i, "ExtractSize");
                        CSizePosTable.Add(ID, pos + DataLPos);

                        type = GetColumnType(utfDataL, i, "ExtractSize");
                        CSizeTypeTable.Add(ID, type);
                    }

                    IDs.Add(ID);
                }
            }

            if (DataH != null)
            {
                ms = new MemoryStream(DataH);
                utfr = new EndianReader(ms, false);
                utfDataH = new UTF(tools);
                utfDataH.ReadUTF(utfr);

                for (var i = 0; i < utfDataH.num_rows; i++)
                {
                    ID = (ushort) GetColumnData(utfDataH, i, "ID");
                    size2 = (uint) GetColumnData(utfDataH, i, "FileSize");
                    SizeTable.Add(ID, size2);

                    pos = GetColumnPostion(utfDataH, i, "FileSize");
                    SizePosTable.Add(ID, pos + DataHPos);

                    type = GetColumnType(utfDataH, i, "FileSize");
                    SizeTypeTable.Add(ID, type);

                    if (GetColumnData(utfDataH, i, "ExtractSize") != null)
                    {
                        size2 = (uint) GetColumnData(utfDataH, i, "ExtractSize");
                        CSizeTable.Add(ID, size2);

                        pos = GetColumnPostion(utfDataH, i, "ExtractSize");
                        CSizePosTable.Add(ID, pos + DataHPos);

                        type = GetColumnType(utfDataH, i, "ExtractSize");
                        CSizeTypeTable.Add(ID, type);
                    }

                    IDs.Add(ID);
                }
            }

            FileEntry temp;
            uint value = 0, value2 = 0;
            var baseoffset = ContentOffset;

            IDs = IDs.OrderBy(x => x).ToList();


            foreach (var id in IDs)
            {
                temp = new FileEntry();
                SizeTable.TryGetValue(id, out value);
                CSizeTable.TryGetValue(id, out value2);

                temp.TOCName = "ITOC";

                temp.DirName = null;
                temp.FileName = id.ToString("D4");

                temp.FileSize = value;
                temp.FileSizePos = SizePosTable[id];
                temp.FileSizeType = SizeTypeTable[id];

                if (CSizeTable.Count > 0 && CSizeTable.ContainsKey(id))
                {
                    temp.ExtractSize = value2;
                    temp.ExtractSizePos = CSizePosTable[id];
                    temp.ExtractSizeType = CSizeTypeTable[id];
                }

                temp.FileType = "FILE";


                temp.FileOffset = baseoffset;
                temp.ID = id;
                temp.UserString = null;

                FileTable.Add(temp);

                if (value % Align > 0)
                    baseoffset += value + (Align - value % Align);
                else
                    baseoffset += value;
            }

            files = null;
            utfDataL = null;
            utfDataH = null;

            ms.Close();
            utfr.Close();


            return true;
        }

        private void ReadUTFData(EndianReader br)
        {
            isUtfEncrypted = false;
            br.IsLittleEndian = true;

            unk1 = br.ReadInt32();
            utf_size = br.ReadInt64();
            utf_packet = br.ReadBytes((int) utf_size);

            if (utf_packet[0] != 0x40 && utf_packet[1] != 0x55 && utf_packet[2] != 0x54 && utf_packet[3] != 0x46) //@UTF
            {
                utf_packet = DecryptUTF(utf_packet);
                isUtfEncrypted = true;
            }

            br.IsLittleEndian = false;
        }

        public bool ReadGTOC(EndianReader br, ulong startoffset)
        {
            br.BaseStream.Seek((long) startoffset, SeekOrigin.Begin);

            if (tools.ReadCString(br, 4) != "GTOC")
            {
                br.Close();
                return false;
            }

            br.BaseStream.Seek(0xC, SeekOrigin.Current); //skip header data

            return true;
        }

        public bool ReadETOC(EndianReader br, ulong startoffset)
        {
            br.BaseStream.Seek((long) startoffset, SeekOrigin.Begin);

            if (tools.ReadCString(br, 4) != "ETOC")
            {
                br.Close();
                return false;
            }


            ReadUTFData(br);

            ETOC_packet = utf_packet;

            var etoc_entry = FileTable.Where(x => x.FileName.ToString() == "ETOC_HDR").Single();
            etoc_entry.Encrypted = isUtfEncrypted;
            etoc_entry.FileSize = ETOC_packet.Length;

            var ms = new MemoryStream(utf_packet);
            var utfr = new EndianReader(ms, false);

            files = new UTF(tools);
            if (!files.ReadUTF(utfr))
            {
                br.Close();
                return false;
            }

            utfr.Close();
            ms.Close();

            var fileEntries = FileTable.Where(x => x.FileType == "FILE").ToList();

            for (var i = 0; i < fileEntries.Count; i++) FileTable[i].LocalDir = GetColumnData(files, i, "LocalDir");

            return true;
        }

        public byte[] DecryptUTF(byte[] input)
        {
            var result = new byte[input.Length];

            int m, t;
            byte d;

            m = 0x0000655f;
            t = 0x00004115;

            for (var i = 0; i < input.Length; i++)
            {
                d = input[i];
                d = (byte) (d ^ (byte) (m & 0xff));
                result[i] = d;
                m *= t;
            }

            return result;
        }

        public byte[] DecompressCRILAYLA(byte[] input, int USize)
        {
            byte[] result;

            var ms = new MemoryStream(input);
            var br = new EndianReader(ms, true);

            br.BaseStream.Seek(8, SeekOrigin.Begin); // Skip CRILAYLA
            var uncompressed_size = br.ReadInt32();
            var uncompressed_header_offset = br.ReadInt32();
            result = new byte[uncompressed_size + 0x100];

            Array.Copy(input, uncompressed_header_offset + 0x10, result, 0, 0x100);

            var input_end = input.Length - 0x100 - 1;
            var input_offset = input_end;
            var output_end = 0x100 + uncompressed_size - 1;
            byte bit_pool = 0;
            int bits_left = 0, bytes_output = 0;
            var vle_lens = new int[4] {2, 3, 5, 8};

            while (bytes_output < uncompressed_size)
                if (get_next_bits(input, ref input_offset, ref bit_pool, ref bits_left, 1) > 0)
                {
                    var backreference_offset = output_end - bytes_output +
                                               get_next_bits(input, ref input_offset, ref bit_pool, ref bits_left, 13) +
                                               3;
                    var backreference_length = 3;
                    int vle_level;

                    for (vle_level = 0; vle_level < vle_lens.Length; vle_level++)
                    {
                        int this_level = get_next_bits(input, ref input_offset, ref bit_pool, ref bits_left,
                            vle_lens[vle_level]);
                        backreference_length += this_level;
                        if (this_level != (1 << vle_lens[vle_level]) - 1) break;
                    }

                    if (vle_level == vle_lens.Length)
                    {
                        int this_level;
                        do
                        {
                            this_level = get_next_bits(input, ref input_offset, ref bit_pool, ref bits_left, 8);
                            backreference_length += this_level;
                        } while (this_level == 255);
                    }

                    for (var i = 0; i < backreference_length; i++)
                    {
                        result[output_end - bytes_output] = result[backreference_offset--];
                        bytes_output++;
                    }
                }
                else
                {
                    result[output_end - bytes_output] =
                        (byte) get_next_bits(input, ref input_offset, ref bit_pool, ref bits_left, 8);
                    bytes_output++;
                }

            br.Close();
            ms.Close();

            return result;
        }

        private ushort get_next_bits(byte[] input, ref int offset_p, ref byte bit_pool_p, ref int bits_left_p,
            int bit_count)
        {
            ushort out_bits = 0;
            var num_bits_produced = 0;
            int bits_this_round;

            while (num_bits_produced < bit_count)
            {
                if (bits_left_p == 0)
                {
                    bit_pool_p = input[offset_p];
                    bits_left_p = 8;
                    offset_p--;
                }

                if (bits_left_p > bit_count - num_bits_produced)
                    bits_this_round = bit_count - num_bits_produced;
                else
                    bits_this_round = bits_left_p;

                out_bits <<= bits_this_round;

                out_bits |= (ushort) ((ushort) (bit_pool_p >> (bits_left_p - bits_this_round)) &
                                      ((1 << bits_this_round) - 1));

                bits_left_p -= bits_this_round;
                num_bits_produced += bits_this_round;
            }

            return out_bits;
        }

        public object GetColumsData2(UTF utf, int row, string Name, int type)
        {
            var Temp = GetColumnData(utf, row, Name);

            switch (Temp)
            {
                case null:
                    switch (type)
                    {
                        case 0: // byte
                            return (byte) 0xFF;
                        case 1: // short
                            return (ushort) 0xFFFF;
                        case 2: // int
                            return 0xFFFFFFFF;
                        case 3: // long
                            return 0xFFFFFFFFFFFFFFFF;
                    }

                    break;
                case ulong temp:
                    return Temp == null ? 0xFFFFFFFFFFFFFFFF : temp;
                case uint temp:
                    return Temp == null ? 0xFFFFFFFF : temp;
                case ushort temp:
                    return Temp == null ? (ushort) 0xFFFF : temp;
            }

            return 0;
        }

        public object GetColumnData(UTF utf, int row, string Name)
        {
            object result = null;

            try
            {
                for (var i = 0; i < utf.num_columns; i++)
                    if (utf.columns[i].name == Name)
                    {
                        result = utf.rows[row].rows[i].GetValue();
                        break;
                    }
            }
            catch (Exception)
            {
                return null;
            }


            return result;
        }

        public long GetColumnPostion(UTF utf, int row, string Name)
        {
            long result = -1;

            try
            {
                for (var i = 0; i < utf.num_columns; i++)
                    if (utf.columns[i].name == Name)
                    {
                        result = utf.rows[row].rows[i].position;
                        break;
                    }
            }
            catch (Exception)
            {
                return -1;
            }

            return result;
        }

        public Type GetColumnType(UTF utf, int row, string Name)
        {
            Type result = null;

            try
            {
                for (var i = 0; i < utf.num_columns; i++)
                    if (utf.columns[i].name == Name)
                    {
                        result = utf.rows[row].rows[i].GetType();
                        break;
                    }
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }

        public void UpdateFileEntry(FileEntry fileEntry)
        {
            if (fileEntry.FileType != "FILE" && fileEntry.FileType != "HDR") return;
            byte[] updateMe = null;
            switch (fileEntry.TOCName)
            {
                case "CPK":
                    updateMe = CPK_packet;
                    break;
                case "TOC":
                    updateMe = TOC_packet;
                    break;
                case "ITOC":
                    updateMe = ITOC_packet;
                    break;
                case "ETOC":
                    updateMe = ETOC_packet;
                    break;
                default:
                    throw new Exception("I need to implement this TOC!");
                    break;
            }


            if (fileEntry.ExtractSizePos > 0)
                UpdateValue(ref updateMe, fileEntry.ExtractSize, fileEntry.ExtractSizePos,
                    fileEntry.ExtractSizeType);

            if (fileEntry.FileSizePos > 0)
                UpdateValue(ref updateMe, fileEntry.FileSize, fileEntry.FileSizePos, fileEntry.FileSizeType);

            if (fileEntry.FileOffsetPos > 0)
                UpdateValue(ref updateMe, fileEntry.FileOffset - (ulong) (fileEntry.TOCName == "TOC" ? 0x800 : 0),
                    fileEntry.FileOffsetPos, fileEntry.FileOffsetType);

            switch (fileEntry.TOCName)
            {
                case "CPK":
                    updateMe = CPK_packet;
                    break;
                case "TOC":
                    TOC_packet = updateMe;
                    break;
                case "ITOC":
                    ITOC_packet = updateMe;
                    break;
                case "ETOC":
                    updateMe = ETOC_packet;
                    break;
                default:
                    throw new Exception("I need to implement this TOC!");
                    break;
            }
        }

        public void UpdateValue(ref byte[] packet, object value, long pos, Type type)
        {
            var temp = new MemoryStream();
            temp.Write(packet, 0, packet.Length);

            var toc = new EndianWriter(temp, false);
            toc.Seek((int) pos, SeekOrigin.Begin);

            value = Convert.ChangeType(value, type);

            if (type == typeof(byte))
                toc.Write((byte) value);
            else if (type == typeof(ushort))
                toc.Write((ushort) value);
            else if (type == typeof(uint))
                toc.Write((uint) value);
            else if (type == typeof(ulong))
                toc.Write((ulong) value);
            else if (type == typeof(float))
                toc.Write((float) value);
            else
                throw new Exception("Not supported type!");

            toc.Close();

            var myStream = (MemoryStream) toc.BaseStream;
            packet = myStream.ToArray();
        }
    }

    public class UTF
    {
        public enum COLUMN_FLAGS
        {
            STORAGE_MASK = 0xf0,
            STORAGE_NONE = 0x00,
            STORAGE_ZERO = 0x10,
            STORAGE_CONSTANT = 0x30,
            STORAGE_PERROW = 0x50,


            TYPE_MASK = 0x0f,
            TYPE_DATA = 0x0b,
            TYPE_STRING = 0x0a,
            TYPE_FLOAT = 0x08,
            TYPE_8BYTE2 = 0x07,
            TYPE_8BYTE = 0x06,
            TYPE_4BYTE2 = 0x05,
            TYPE_4BYTE = 0x04,
            TYPE_2BYTE2 = 0x03,
            TYPE_2BYTE = 0x02,
            TYPE_1BYTE2 = 0x01,
            TYPE_1BYTE = 0x00
        }

        private readonly Tools tools;

        public List<COLUMN> columns;
        public List<ROWS> rows;

        public UTF(Tools tool)
        {
            tools = tool;
        }

        public int table_size { get; set; }

        public long rows_offset { get; set; }
        public long strings_offset { get; set; }
        public long data_offset { get; set; }
        public int table_name { get; set; }
        public short num_columns { get; set; }
        public short row_length { get; set; }
        public int num_rows { get; set; }

        public bool ReadUTF(EndianReader br)
        {
            var offset = br.BaseStream.Position;

            if (tools.ReadCString(br, 4) != "@UTF") return false;

            table_size = br.ReadInt32();
            rows_offset = br.ReadInt32();
            strings_offset = br.ReadInt32();
            data_offset = br.ReadInt32();

            rows_offset += offset + 8;
            strings_offset += offset + 8;
            data_offset += offset + 8;

            table_name = br.ReadInt32();
            num_columns = br.ReadInt16();
            row_length = br.ReadInt16();
            num_rows = br.ReadInt32();

            columns = new List<COLUMN>();
            COLUMN column;

            for (var i = 0; i < num_columns; i++)
            {
                column = new COLUMN {flags = br.ReadByte()};
                if (column.flags == 0)
                {
                    br.BaseStream.Seek(3, SeekOrigin.Current);
                    column.flags = br.ReadByte();
                }

                column.name = tools.ReadCString(br, -1, br.ReadInt32() + strings_offset);
                columns.Add(column);
            }

            rows = new List<ROWS>();
            ROWS current_entry;
            ROW current_row;
            int storage_flag;

            for (var j = 0; j < num_rows; j++)
            {
                br.BaseStream.Seek(rows_offset + j * row_length, SeekOrigin.Begin);

                current_entry = new ROWS();

                for (var i = 0; i < num_columns; i++)
                {
                    current_row = new ROW();

                    storage_flag = columns[i].flags & (int) COLUMN_FLAGS.STORAGE_MASK;

                    switch (storage_flag)
                    {
                        case (int) COLUMN_FLAGS.STORAGE_NONE:
                            current_entry.rows.Add(current_row);
                            continue;

                        case (int) COLUMN_FLAGS.STORAGE_ZERO:
                            current_entry.rows.Add(current_row);
                            continue;

                        case (int) COLUMN_FLAGS.STORAGE_CONSTANT:
                            current_entry.rows.Add(current_row);
                            continue;
                    }


                    current_row.type = columns[i].flags & (int) COLUMN_FLAGS.TYPE_MASK;

                    current_row.position = br.BaseStream.Position;

                    switch (current_row.type)
                    {
                        case 0:
                        case 1:
                            current_row.uint8 = br.ReadByte();
                            break;

                        case 2:
                        case 3:
                            current_row.uint16 = br.ReadUInt16();
                            break;

                        case 4:
                        case 5:
                            current_row.uint32 = br.ReadUInt32();
                            break;

                        case 6:
                        case 7:
                            current_row.uint64 = br.ReadUInt64();
                            break;

                        case 8:
                            current_row.ufloat = br.ReadSingle();
                            break;

                        case 0xA:
                            current_row.str = tools.ReadCString(br, -1, br.ReadInt32() + strings_offset);
                            break;

                        case 0xB:
                            var position = br.ReadInt32() + data_offset;
                            current_row.position = position;
                            current_row.data = tools.GetData(br, position, br.ReadInt32());
                            break;

                        default: throw new NotImplementedException();
                    }


                    current_entry.rows.Add(current_row);
                }

                rows.Add(current_entry);
            }

            return true;
        }
    }

    public class COLUMN
    {
        public byte flags { get; set; }
        public string name { get; set; }
    }

    public class ROWS
    {
        public List<ROW> rows;

        public ROWS()
        {
            rows = new List<ROW>();
        }
    }

    public class ROW
    {
        public ROW()
        {
            type = -1;
        }

        public int type { get; set; }

        public byte uint8 { get; set; }
        public ushort uint16 { get; set; }
        public uint uint32 { get; set; }
        public ulong uint64 { get; set; }
        public float ufloat { get; set; }
        public string str { get; set; }
        public byte[] data { get; set; }
        public long position { get; set; }

        public object GetValue()
        {
            object result = -1;

            switch (type)
            {
                case 0:
                case 1: return uint8;

                case 2:
                case 3: return uint16;

                case 4:
                case 5: return uint32;

                case 6:
                case 7: return uint64;

                case 8: return ufloat;

                case 0xA: return str;

                case 0xB: return data;

                default: return null;
            }
        }

        public Type GetType()
        {
            object result = -1;

            switch (type)
            {
                case 0:
                case 1: return uint8.GetType();

                case 2:
                case 3: return uint16.GetType();

                case 4:
                case 5: return uint32.GetType();

                case 6:
                case 7: return uint64.GetType();

                case 8: return ufloat.GetType();

                case 0xA: return str.GetType();

                case 0xB: return data.GetType();

                default: return null;
            }
        }
    }

    public class FileEntry
    {
        public FileEntry()
        {
            DirName = null;
            FileName = null;
            FileSize = null;
            ExtractSize = null;
            ID = null;
            UserString = null;
            LocalDir = null;

            FileOffset = 0;
            UpdateDateTime = 0;
        }

        public object DirName { get; set; } // string
        public object FileName { get; set; } // string

        public object FileSize { get; set; }
        public long FileSizePos { get; set; }
        public Type FileSizeType { get; set; }

        public object ExtractSize { get; set; } // int
        public long ExtractSizePos { get; set; }
        public Type ExtractSizeType { get; set; }

        public ulong FileOffset { get; set; }
        public long FileOffsetPos { get; set; }
        public Type FileOffsetType { get; set; }


        public ulong Offset { get; set; }
        public object ID { get; set; } // int
        public object UserString { get; set; } // string
        public ulong UpdateDateTime { get; set; }
        public object LocalDir { get; set; } // string
        public string TOCName { get; set; }

        public bool Encrypted { get; set; }

        public string FileType { get; set; }
    }

    public class Tools
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, long count);

        public string ReadCString(BinaryReader br, int MaxLength = -1, long lOffset = -1, Encoding enc = null)
        {
            int Max;
            Max = MaxLength == -1 ? 255 : MaxLength;

            var fTemp = br.BaseStream.Position;
            byte bTemp = 0;
            var i = 0;
            var result = "";

            if (lOffset > -1) br.BaseStream.Seek(lOffset, SeekOrigin.Begin);

            do
            {
                bTemp = br.ReadByte();
                if (bTemp == 0)
                    break;
                i += 1;
            } while (i < Max);

            if (MaxLength == -1)
                Max = i + 1;
            else
                Max = MaxLength;

            if (lOffset > -1)
            {
                br.BaseStream.Seek(lOffset, SeekOrigin.Begin);

                result = enc == null
                    ? Encoding.GetEncoding("SJIS").GetString(br.ReadBytes(i))
                    : enc.GetString(br.ReadBytes(i));

                br.BaseStream.Seek(fTemp, SeekOrigin.Begin);
            }
            else
            {
                br.BaseStream.Seek(fTemp, SeekOrigin.Begin);
                result = enc == null
                    ? Encoding.GetEncoding("SJIS").GetString(br.ReadBytes(i))
                    : enc.GetString(br.ReadBytes(i));

                br.BaseStream.Seek(fTemp + Max, SeekOrigin.Begin);
            }

            return result;
        }

        public void DeleteFileIfExists(string sPath)
        {
            if (File.Exists(sPath))
                File.Delete(sPath);
        }

        public string GetPath(string input)
        {
            return Path.GetDirectoryName(input) + "\\" + Path.GetFileNameWithoutExtension(input);
        }

        public byte[] GetData(BinaryReader br, long offset, int size)
        {
            byte[] result = null;
            var backup = br.BaseStream.Position;
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            result = br.ReadBytes(size);
            br.BaseStream.Seek(backup, SeekOrigin.Begin);
            return result;
        }
    }

    public class EndianReader : BinaryReader
    {
        private readonly byte[] buffer = new byte[8];

        public EndianReader(Stream input, Encoding encoding, bool isLittleEndian)
            : base(input, encoding)
        {
            IsLittleEndian = isLittleEndian;
        }

        public EndianReader(Stream input, bool isLittleEndian)
            : this(input, Encoding.UTF8, isLittleEndian)
        {
        }

        public bool IsLittleEndian { get; set; }


        public override double ReadDouble()
        {
            if (IsLittleEndian)
                return base.ReadDouble();
            FillMyBuffer(8);
            return BitConverter.ToDouble(buffer.Take(8).Reverse().ToArray(), 0);
        }

        public override short ReadInt16()
        {
            if (IsLittleEndian)
                return base.ReadInt16();
            FillMyBuffer(2);
            return BitConverter.ToInt16(buffer.Take(2).Reverse().ToArray(), 0);
        }

        public override int ReadInt32()
        {
            if (IsLittleEndian)
                return base.ReadInt32();
            FillMyBuffer(4);
            return BitConverter.ToInt32(buffer.Take(4).Reverse().ToArray(), 0);
        }

        public override long ReadInt64()
        {
            if (IsLittleEndian)
                return base.ReadInt64();
            FillMyBuffer(8);
            return BitConverter.ToInt64(buffer.Take(8).Reverse().ToArray(), 0);
        }

        public override float ReadSingle()
        {
            if (IsLittleEndian)
                return base.ReadSingle();
            FillMyBuffer(4);
            return BitConverter.ToSingle(buffer.Take(4).Reverse().ToArray(), 0);
        }

        public override ushort ReadUInt16()
        {
            if (IsLittleEndian)
                return base.ReadUInt16();
            FillMyBuffer(2);
            return BitConverter.ToUInt16(buffer.Take(2).Reverse().ToArray(), 0);
        }


        public override uint ReadUInt32()
        {
            if (IsLittleEndian)
                return base.ReadUInt32();
            FillMyBuffer(4);
            return BitConverter.ToUInt32(buffer.Take(4).Reverse().ToArray(), 0);
        }

        public override ulong ReadUInt64()
        {
            if (IsLittleEndian)
                return base.ReadUInt64();
            FillMyBuffer(8);
            return BitConverter.ToUInt64(buffer.Take(8).Reverse().ToArray(), 0);
        }

        private void FillMyBuffer(int numBytes)
        {
            var offset = 0;
            var num2 = 0;
            if (numBytes == 1)
            {
                num2 = BaseStream.ReadByte();
                if (num2 == -1) throw new EndOfStreamException("Attempted to read past the end of the stream.");
                buffer[0] = (byte) num2;
            }
            else
            {
                do
                {
                    num2 = BaseStream.Read(buffer, offset, numBytes - offset);
                    if (num2 == 0) throw new EndOfStreamException("Attempted to read past the end of the stream.");
                    offset += num2;
                } while (offset < numBytes);
            }
        }
    }

    public class EndianWriter : BinaryWriter
    {
        public EndianWriter(Stream input, Encoding encoding, bool isLittleEndian)
            : base(input, encoding)
        {
            IsLittleEndian = isLittleEndian;
        }

        public EndianWriter(Stream input, bool isLittleEndian)
            : this(input, Encoding.UTF8, isLittleEndian)
        {
        }

        public bool IsLittleEndian { get; set; }

        public void Write<T>(T value)
        {
            dynamic input = value;
            byte[] someBytes = BitConverter.GetBytes(input);
            if (!IsLittleEndian)
                someBytes = someBytes.Reverse().ToArray();

            base.Write(someBytes);
        }

        public void Write(FileEntry entry)
        {
            if (entry.ExtractSizeType == typeof(byte))
                Write((byte) entry.ExtractSize);
            else if (entry.ExtractSizeType == typeof(ushort))
                Write((ushort) entry.ExtractSize);
            else if (entry.ExtractSizeType == typeof(uint))
                Write((uint) entry.ExtractSize);
            else if (entry.ExtractSizeType == typeof(ulong))
                Write((ulong) entry.ExtractSize);
            else if (entry.ExtractSizeType == typeof(float))
                Write((float) entry.ExtractSize);
            else
                throw new Exception("Not supported type!");
        }
    }
}