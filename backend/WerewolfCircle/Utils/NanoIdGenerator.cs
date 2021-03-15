// MIT License
// 
// Copyright (c) 2021 Joel Liechti
// Copyright (c) 2017 zhu yu
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Numerics;
using System.Security.Cryptography;

namespace WerewolfCircle.Utils
{
    /// <summary>
    /// Adapted from <a href="https://github.com/codeyu/nanoid-net/blob/master/src/Nanoid/Nanoid.cs">Nanoid's .NET port</a>.
    /// </summary>
    internal static class NanoIdGenerator
    {
        private const string DefaultAlphabet = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Generates a random, cryptographically sound, url-safe id.
        /// </summary>
        /// <param name="alphabet">Symbols to use for generating the id.</param>
        /// <param name="length">The length of the id.</param>
        public static string Generate(string alphabet = DefaultAlphabet, int length = 21)
        {
            if (string.IsNullOrEmpty(alphabet) || alphabet.Length >= 256)
                throw new ArgumentException("alphabet must contain between 1 and 255 symbols.", nameof(alphabet));

            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "size has to be positive.");

            // See https://github.com/ai/nanoid/blob/c1c333db2b/index.js for explanation
            // why masking is used (`random % alphabet` is a common mistake security-wise).
            int mask = (2 << 31 - BitOperations.LeadingZeroCount((uint)((alphabet.Length - 1) | 1))) - 1;
            int step = (int)Math.Ceiling(1.6 * mask * length / alphabet.Length);

            Span<char> idBuilder = stackalloc char[length];
            Span<byte> randomBytes = stackalloc byte[step];

            int count = 0;

            while (true)
            {
                RandomNumberGenerator.Fill(randomBytes);

                for (int i = 0; i < step; i++)
                {
                    int alphabetIndex = randomBytes[i] & mask;

                    if (alphabetIndex >= alphabet.Length) continue;
                    idBuilder[count] = alphabet[alphabetIndex];
                    if (++count == length)
                    {
                        return new string(idBuilder);
                    }
                }
            }
        }
    }
}
