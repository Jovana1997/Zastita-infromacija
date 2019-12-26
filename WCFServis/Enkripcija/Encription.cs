using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enkripcija
{
	public class Encription
	{
        //<--Simple Substitution-->
		private static bool SimpleSubstitution(string input, string oldAlphabet, string newAlphabet, out string output)
		{
			output = string.Empty;

			if (oldAlphabet.Length != newAlphabet.Length)
				return false;

			for (int i = 0; i < input.Length; ++i)
			{
				int oldCharIndex = oldAlphabet.IndexOf(char.ToLower(input[i]));

				if (oldCharIndex >= 0)
					output += char.IsUpper(input[i]) ? char.ToUpper(newAlphabet[oldCharIndex]) : newAlphabet[oldCharIndex];
				else
					output += input[i];
			}

			return true;
		}
		public static bool EncipherSS(string input, out string output)
		{
			string plainAlphabet = "abcdefghijklmnopqrstuvwxyz";
			string cipherAlphabet = "yhkqgvxfoluapwmtzecjdbsnri";
			return SimpleSubstitution(input, plainAlphabet, cipherAlphabet, out output);
		}
		public static bool DecipherSS(string input, out string output)
		{
			string plainAlphabet = "abcdefghijklmnopqrstuvwxyz";
			string cipherAlphabet = "yhkqgvxfoluapwmtzecjdbsnri";
			return SimpleSubstitution(input, cipherAlphabet, plainAlphabet, out output);
		}
        //<--XXTEA-->
        public static String EncryptXXTEA(String text, String password)
        {

            if (text.Length == 0)
                return "";  

            if (password.Length < 8)
            {
                throw new ArgumentException("The password for encryption is too short");
            }

            while (password.Length < 16) { password += password; }

            var v = ToLongs((new UTF8Encoding()).GetBytes(text));

            if (v.Length == 1) { v[0] = 0; }

            var k = ToLongs((new UTF8Encoding()).GetBytes(password.Substring(0, 16)));

            UInt32 n = (UInt32)v.Length,
                   z = v[n - 1],
                   y = v[0],
                   delta = 0x9e3779b9,
                   e,
                   q = (UInt32)(6 + (52 / n)),
                   sum = 0,
                   p = 0;

            while (q-- > 0)
            {
                sum += delta;
                e = sum >> 2 & 3;

                for (p = 0; p < (n - 1); p++)
                {
                    y = v[(p + 1)];
                    z = v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                }

                y = v[0];
                z = v[n - 1] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
            }

            return Convert.ToBase64String(ToBytes(v));
        }
        public static String DecryptXXTEA(String ciphertext, String password)
        {

            if (ciphertext.Length == 0) { return ""; }

			while (password.Length < 16) { password += password; }

			var v = ToLongs(Convert.FromBase64String(ciphertext));
            var k = ToLongs((new UTF8Encoding()).GetBytes(password.Substring(0, 16)));

            UInt32 n = (UInt32)v.Length,
                   z = v[n - 1],
                   y = v[0],
                   delta = 0x9e3779b9,
                   e,
                   q = (UInt32)(6 + (52 / n)),
                   sum = q * delta,
                   p = 0;

            while (sum != 0)
            {
                e = sum >> 2 & 3;

                for (p = (n - 1); p > 0; p--)
                {
                    z = v[p - 1];
                    y = v[p] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                }

                z = v[n - 1];
                y = v[0] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);

                sum -= delta;
            }

            var plaintext = (new UTF8Encoding()).GetString(ToBytes(v));

            return plaintext;
        }
        private static UInt32[] ToLongs(byte[] s)
        {

            var l = new UInt32[(int)Math.Ceiling(((decimal)s.Length / 4))];

            for (int i = 0; i < l.Length; i++)
            {
                l[i] = ((s[i * 4])) +
                       ((i * 4 + 1) >= s.Length ? (UInt32)0 << 8 : ((UInt32)s[i * 4 + 1] << 8)) +
                       ((i * 4 + 2) >= s.Length ? (UInt32)0 << 16 : ((UInt32)s[i * 4 + 2] << 16)) +
                       ((i * 4 + 3) >= s.Length ? (UInt32)0 << 24 : ((UInt32)s[i * 4 + 3] << 24));
            }

            return l;
        }
        private static byte[] ToBytes(UInt32[] l)
        {
            byte[] b = new byte[l.Length * 4];

            for (Int32 i = 0; i < l.Length; i++)
            {
                b[(i * 4)] = (byte)(l[i] & 0xFF);
                b[(i * 4) + 1] = (byte)(l[i] >> (8 & 0xFF));
                b[(i * 4) + 2] = (byte)(l[i] >> (16 & 0xFF));
                b[(i * 4) + 3] = (byte)(l[i] >> (24 & 0xFF));
            }
            return b;
        }
        //<--PCBC-->
        public byte[] EncryptPCBC(byte[] file)
        {
            return file;
        }
        public byte[] DecryptPCBC(byte[] file)
        {
            return file;
        }
        //<--Knapsack-->
        public static string EncryptKnapsack(string publickey, string data)
		{
            string data_result = "";
            string[] vals = publickey.Split(',');
            int[] weights = new int[vals.Length];
            for (int i = 0; i < vals.Length; i++) weights[i] = Convert.ToInt32(vals[i]);
            int ptr = 0;
            int bit = 0;
            int total = 0;
            do
            {
                total = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[ptr] == '1') bit = 1;
                    else bit = 0;
                    total += weights[i] * bit;
                    ptr++;
                }
                if (data_result == "") data_result += total.ToString();
                else data_result += "," + total.ToString();

            } while (ptr < data.Length);


            return (data_result);

        }
		public static string DecryptKnapsack(string privatekey, string dataa, string n, string m)
		{
			string data_result = "";
			string[] vals = privatekey.Split(',');
			int[] weights = new int[vals.Length];
			int i;
			for (i = 0; i < vals.Length; i++) weights[i] = Convert.ToInt32(vals[i]);
			int bit;
			int im = Solve(Convert.ToInt32(n),Convert.ToInt32(m));
			int c = Convert.ToInt32(dataa);
			int tc = (c * im) % Convert.ToInt32(n);
			i = 6;
			do
			{
				if (tc < weights[i])
					bit = 0;
				else
				{
					bit = 1;
					tc -= weights[i];
				}
				i--;
				data_result = bit.ToString() + data_result;
			} while (i >= 0);
			return data_result;
		}
		public static int Solve(int n, int m)
        {
            int res = 0;
            for (int i = 0; i < 5000; i++)
            {
                if (((m * i) % n) == 1) return (i);
            }
            return (res);
        }
        public static string GetPublicKey(string key, string n, string m)
        {
            string[] vals = key.Split(',');
            string k = "";
            foreach (string v in vals)
            {
                int i = (Convert.ToInt32(v) * Convert.ToInt32(m)) % Convert.ToInt32(n);
                if (k == "") k += i.ToString();
                else k += "," + i.ToString();

            }
            return (k);

        }
		//<--SHA2-->
		static void DBL_INT_ADD(ref uint a, ref uint b, uint c)
		{
			if (a > 0xffffffff - c) ++b; a += c;
		}

		static uint ROTLEFT(uint a, byte b)
		{
			return ((a << b) | (a >> (32 - b)));
		}

		static uint ROTRIGHT(uint a, byte b)
		{
			return (((a) >> (b)) | ((a) << (32 - (b))));
		}

		static uint CH(uint x, uint y, uint z)
		{
			return (((x) & (y)) ^ (~(x) & (z)));
		}

		static uint MAJ(uint x, uint y, uint z)
		{
			return (((x) & (y)) ^ ((x) & (z)) ^ ((y) & (z)));
		}

		static uint EP0(uint x)
		{
			return (ROTRIGHT(x, 2) ^ ROTRIGHT(x, 13) ^ ROTRIGHT(x, 22));
		}

		static uint EP1(uint x)
		{
			return (ROTRIGHT(x, 6) ^ ROTRIGHT(x, 11) ^ ROTRIGHT(x, 25));
		}

		static uint SIG0(uint x)
		{
			return (ROTRIGHT(x, 7) ^ ROTRIGHT(x, 18) ^ ((x) >> 3));
		}

		static uint SIG1(uint x)
		{
			return (ROTRIGHT(x, 17) ^ ROTRIGHT(x, 19) ^ ((x) >> 10));
		}

		struct SHA256_CTX
		{
			public byte[] data;
			public uint datalen;
			public uint[] bitlen;
			public uint[] state;
		}

		static uint[] k = 
		{
			0x428a2f98,0x71374491,0xb5c0fbcf,0xe9b5dba5,0x3956c25b,0x59f111f1,0x923f82a4,0xab1c5ed5,
			0xd807aa98,0x12835b01,0x243185be,0x550c7dc3,0x72be5d74,0x80deb1fe,0x9bdc06a7,0xc19bf174,
			0xe49b69c1,0xefbe4786,0x0fc19dc6,0x240ca1cc,0x2de92c6f,0x4a7484aa,0x5cb0a9dc,0x76f988da,
			0x983e5152,0xa831c66d,0xb00327c8,0xbf597fc7,0xc6e00bf3,0xd5a79147,0x06ca6351,0x14292967,
			0x27b70a85,0x2e1b2138,0x4d2c6dfc,0x53380d13,0x650a7354,0x766a0abb,0x81c2c92e,0x92722c85,
			0xa2bfe8a1,0xa81a664b,0xc24b8b70,0xc76c51a3,0xd192e819,0xd6990624,0xf40e3585,0x106aa070,
			0x19a4c116,0x1e376c08,0x2748774c,0x34b0bcb5,0x391c0cb3,0x4ed8aa4a,0x5b9cca4f,0x682e6ff3,
			0x748f82ee,0x78a5636f,0x84c87814,0x8cc70208,0x90befffa,0xa4506ceb,0xbef9a3f7,0xc67178f2
		};

		static void SHA256Transform(ref SHA256_CTX ctx, byte[] data)
		{
			uint a, b, c, d, e, f, g, h, i, j, t1, t2;
			uint[] m = new uint[64];

			for (i = 0, j = 0; i < 16; ++i, j += 4)
				m[i] = (uint)((data[j] << 24) | (data[j + 1] << 16) | (data[j + 2] << 8) | (data[j + 3]));

			for (; i < 64; ++i)
				m[i] = SIG1(m[i - 2]) + m[i - 7] + SIG0(m[i - 15]) + m[i - 16];

			a = ctx.state[0];
			b = ctx.state[1];
			c = ctx.state[2];
			d = ctx.state[3];
			e = ctx.state[4];
			f = ctx.state[5];
			g = ctx.state[6];
			h = ctx.state[7];

			for (i = 0; i < 64; ++i)
			{
				t1 = h + EP1(e) + CH(e, f, g) + k[i] + m[i];
				t2 = EP0(a) + MAJ(a, b, c);
				h = g;
				g = f;
				f = e;
				e = d + t1;
				d = c;
				c = b;
				b = a;
				a = t1 + t2;
			}

			ctx.state[0] += a;
			ctx.state[1] += b;
			ctx.state[2] += c;
			ctx.state[3] += d;
			ctx.state[4] += e;
			ctx.state[5] += f;
			ctx.state[6] += g;
			ctx.state[7] += h;
		}

		static void SHA256Init(ref SHA256_CTX ctx)
		{
			ctx.datalen = 0;
			ctx.bitlen[0] = 0;
			ctx.bitlen[1] = 0;
			ctx.state[0] = 0x6a09e667;
			ctx.state[1] = 0xbb67ae85;
			ctx.state[2] = 0x3c6ef372;
			ctx.state[3] = 0xa54ff53a;
			ctx.state[4] = 0x510e527f;
			ctx.state[5] = 0x9b05688c;
			ctx.state[6] = 0x1f83d9ab;
			ctx.state[7] = 0x5be0cd19;
		}

		static void SHA256Update(ref SHA256_CTX ctx, byte[] data, uint len)
		{
			for (uint i = 0; i < len; ++i)
			{
				ctx.data[ctx.datalen] = data[i];
				ctx.datalen++;

				if (ctx.datalen == 64)
				{
					SHA256Transform(ref ctx, ctx.data);
					DBL_INT_ADD(ref ctx.bitlen[0], ref ctx.bitlen[1], 512);
					ctx.datalen = 0;
				}
			}
		}

		static void SHA256Final(ref SHA256_CTX ctx, byte[] hash)
		{
			uint i = ctx.datalen;

			if (ctx.datalen < 56)
			{
				ctx.data[i++] = 0x80;

				while (i < 56)
					ctx.data[i++] = 0x00;
			}
			else
			{
				ctx.data[i++] = 0x80;

				while (i < 64)
					ctx.data[i++] = 0x00;

				SHA256Transform(ref ctx, ctx.data);
			}

			DBL_INT_ADD(ref ctx.bitlen[0], ref ctx.bitlen[1], ctx.datalen * 8);
			ctx.data[63] = (byte)(ctx.bitlen[0]);
			ctx.data[62] = (byte)(ctx.bitlen[0] >> 8);
			ctx.data[61] = (byte)(ctx.bitlen[0] >> 16);
			ctx.data[60] = (byte)(ctx.bitlen[0] >> 24);
			ctx.data[59] = (byte)(ctx.bitlen[1]);
			ctx.data[58] = (byte)(ctx.bitlen[1] >> 8);
			ctx.data[57] = (byte)(ctx.bitlen[1] >> 16);
			ctx.data[56] = (byte)(ctx.bitlen[1] >> 24);
			SHA256Transform(ref ctx, ctx.data);

			for (i = 0; i < 4; ++i)
			{
				hash[i] = (byte)(((ctx.state[0]) >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 4] = (byte)(((ctx.state[1]) >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 8] = (byte)(((ctx.state[2]) >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 12] = (byte)((ctx.state[3] >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 16] = (byte)((ctx.state[4] >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 20] = (byte)((ctx.state[5] >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 24] = (byte)((ctx.state[6] >> (int)(24 - i * 8)) & 0x000000ff);
				hash[i + 28] = (byte)((ctx.state[7] >> (int)(24 - i * 8)) & 0x000000ff);
			}
		}

		public static string SHA256(string data)
		{
			SHA256_CTX ctx = new SHA256_CTX();
			ctx.data = new byte[64];
			ctx.bitlen = new uint[2];
			ctx.state = new uint[8];

			byte[] hash = new byte[32];
			string hashStr = string.Empty;

			SHA256Init(ref ctx);
			SHA256Update(ref ctx, Encoding.Default.GetBytes(data), (uint)data.Length);
			SHA256Final(ref ctx, hash);

			for (int i = 0; i < 32; i++)
			{
				hashStr += string.Format("{0:X2}", hash[i]);
			}

			return hashStr;
		}
	}
}
