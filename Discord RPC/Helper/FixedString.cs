using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Helper
{
	/// <summary>
	/// This class prevents strings from being larger than the specified max. Will implicitly cast into a string.
	/// </summary>
	public abstract class SizedString
	{
		/// <summary>
		/// The encoding used to calculate the string byte size.
		/// </summary>
		public Encoding Encoding
		{
			get { return _encoding; }
			set
			{
				if (!SetEncoding(value))
					throw new Exception("Existing string is too big for the new encoding.");
			}
		}
		private Encoding _encoding;

		/// <summary>
		/// The maxium size allowed by the string.
		/// </summary>
		public uint MaxSize { get { return _maxsize; } }

		/// <summary>
		/// The string being stored. 
		/// </summary>
		public string Value
		{
			get { return _value; }
			set
			{
				if (!SetString(value))
					throw new Exception("String is too big for the sized string! Was expecting a length of " + MaxSize + " bytes.");
			}
		}

		private string _value;
		private uint _maxsize;

		/// <summary>
		/// Creates a new SizedString with UTF8 encoding by default
		/// </summary>
		/// <param name="maxSize">Maxium size (in bytes) that the strings can be.</param>
		public SizedString(uint maxSize) : this(maxSize, Encoding.UTF8) { }

		/// <summary>
		/// Creates a new SizedString 
		/// </summary>
		/// <param name="maxSize">Maxium size (in bytes) that the strings can be.</param>
		/// <param name="encoder">The encoding used to calculate length</param>
		public SizedString(uint maxSize, Encoding encoder)
		{
			this._maxsize = maxSize;
			this._encoding = encoder;
		}

		/// <summary>
		/// Attempts to set the encoding. Will fail if the currently stored string is bigger than the <see cref="MaxSize"/> with the new encoding. Will return true if succesful.
		/// </summary>
		/// <param name="encoding">The encoding to set</param>
		/// <returns>Returns true if the string is within the size limit.</returns>
		public bool SetEncoding(Encoding encoding)
		{
			//We have no content, so might as well succed
			if (IsNullOrEmpty())
			{
				_encoding = encoding;
				return true;
			}

			//Make sure its the correct length for the new encoding
			if (encoding.GetByteCount(_value) > MaxSize)
				return false;

			//Set the encoding
			_encoding = encoding;
			return true;
		}

		/// <summary>
		/// Attemps to set the string. Will fail if the string is bigger than the <see cref="MaxSize"/>. Will return true if succesful. 
		/// </summary>
		/// <param name="value">The string to be stored</param>
		/// <returns>Returns true if the string is within the size limit.</returns>
		public bool SetString(string value)
		{
			//Its null so write null
			if (value == null)
			{
				_value = null;
				return true;
			}

			//Make sure its the correct length
			if (Encoding.GetByteCount(value) > MaxSize)
				return false;

			//Set the value
			_value = value;
			return true;
		}

		public abstract SizedString Clone();

		/// <summary>
		/// Returns true if the stored string is null
		/// </summary>
		/// <returns></returns>
		public bool IsNullOrEmpty()
		{
			return string.IsNullOrEmpty(_value);
		}

		/// <summary>
		/// Retursn the value of the string
		/// </summary>
		/// <param name="FS"></param>
		public static implicit operator string(SizedString FS)
		{
			if (FS == null) return null;
			return FS._value;
		}
	}

	/// <summary>
	/// A string with the maxium size of 32 bytes.
	/// </summary>
	public class String32 : SizedString
	{
		public String32() : this(null) { }
		public String32(string value) : base(32) { SetString(value); }

		public override SizedString Clone() { return new String32(Value); }
		public static implicit operator String32(string value)
		{
			return new String32(value);
		}
	}

	/// <summary>
	/// A string with the maxium size of 128 bytes.
	/// </summary>
	public class String128 : SizedString
	{

		public String128() : this(null) { }
		public String128(string value) : base(128) { SetString(value); }

		public override SizedString Clone() { return new String128(Value); }
		public static implicit operator String128(string value)
		{
			return new String128(value);
		}
	}

	public class Test
	{
		void T()
		{
			String32 fs = new String32("Test");
			string text = fs;
			String32 fs2 = (String32)text;
		}
	}
}
