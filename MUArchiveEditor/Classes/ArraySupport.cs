using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class ArraySupport
{

	public bool UseLittleEndian = true;

	public ArraySupport()
	{

	}

	public ArraySupport(bool LittleEndian)
	{
		UseLittleEndian = LittleEndian;
	}

	internal short GetInt16(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 2);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt16(array, 0);
	}
	internal bool GetBool(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 1);

        switch (array[0])
        {
			case 0x00:
				return false;
				break;
			case 0x01:
				return true;
				break;
			default:
				return false;
				break;
        }
	}

	internal float Getfloat(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 4);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToSingle(array, 0);
	}

	internal ushort GetUInt16(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 2);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt16(array, 0);
	}

	internal int GetInt32(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 4);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt32(array, 0);
	}

	internal uint GetUInt32(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 4);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt32(array, 0);
	}

	internal byte[] ReadStreamBytes(Stream InputStream, int ReadNumber)
	{
		byte[] array = new byte[ReadNumber];
		InputStream.Read(array, 0, ReadNumber);
		return array;
	}
	internal byte[] ReadStreamBytesFromPos(Stream InputStream, int ReadNumber, int position)
	{
		byte[] array = new byte[ReadNumber];
		long startPos = InputStream.Position;
		InputStream.Position = (long)position;
		InputStream.Read(array, 0, ReadNumber);
		InputStream.Position = startPos;
		return array;
	}

	internal byte[] endianReverseUnicode(byte[] str)
	{
		byte[] newStr = new byte[str.Length];
		for (int i = 0; i < str.Length; i += 2)
		{
			newStr[i] = str[i + 1];
			newStr[i + 1] = str[i];
		}
		return newStr;
	}

	internal string GetString(Stream InputStream)
	{
		ushort num = GetUInt16(InputStream);
		byte[] array = new byte[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		Encoding utf = Encoding.UTF8;
		return utf.GetString(array);
	}

	internal string GetStringInt32(Stream InputStream)
	{
		int num = GetInt32(InputStream);
		byte[] array = new byte[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		Encoding utf = Encoding.UTF8;
		return utf.GetString(array);
	}

	internal string GetStringUTF16(Stream InputStream)
	{
		int num = GetInt32(InputStream) * 2;
		byte[] array = new byte[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		array = endianReverseUnicode(array);
		Encoding utf = Encoding.Unicode;
		return utf.GetString(array);
	}

	internal char[] GetCharArray(Stream InputStream, int Size)
	{
		byte[] array = new byte[Size];
		for (int i = 0; i < Size; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		Encoding utf = Encoding.UTF8;
		return utf.GetString(array).ToCharArray();
	}

	internal void WriteShortToStream(short short_0, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(short_0);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}
	internal void WriteBoolToStream(bool value, Stream InputStream)
	{
		switch (value)
		{
			case true:
				InputStream.Write(new byte[] {0x01}, 0, 1);
				break;
			case false:
				InputStream.Write(new byte[] { 0x00 }, 0, 1);
				break;
		}
	}

	internal void WriteIntToStream(int Number, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(Number);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}
	internal void WriteByteToStream(byte Byte, Stream InputStream)
	{
		InputStream.Write(new[] { Byte }, 0, 1);
	}
	internal void WriteByteArrayToStream(byte[] Byte, Stream InputStream)
	{
		InputStream.Write(Byte, 0, Byte.Length);
	}
	internal void WriteFloatToStream(float Number, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(Number);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}

	internal void WriteInt16ToStream(int Number, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes((Int16)Number);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}

	internal void WriteUIntToStream(uint uint_0, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(uint_0);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}

	internal void WriteStringToStream(string string_0, Stream memoryInputStream)
	{
		Encoding utf = Encoding.UTF8;
		byte[] bytes = utf.GetBytes(string_0);
		WriteShortToStream((short)bytes.Length, memoryInputStream);
		memoryInputStream.Write(bytes, 0, bytes.Length);
	}
	internal MemoryStream RemoveBytesFromStreamAtPosition(MemoryStream memoryInputStream, int range, int position)
	{
		List<byte> array = memoryInputStream.ToArray().ToList();
		array.RemoveRange(position, range);
		return new MemoryStream(array.ToArray());
	}

}
