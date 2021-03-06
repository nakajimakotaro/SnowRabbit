﻿// zlib/libpng License
//
// Copyright(c) 2019 Sinoa
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software.
//    If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using NUnit.Framework;
using SnowRabbit.RuntimeEngine;

namespace SnowRabbitTest
{
    /// <summary>
    /// SrVirtualMemory クラスに対するテストクラスです
    /// </summary>
    [TestFixture]
    public class SrVirtualMemoryTest
    {
        /// <summary>
        /// 仮想アドレスに対してアクセスを行い想定した値の設定取得が出来ているかのテストをします
        /// </summary>
        [Test]
        public void VirtualAddressAccessTest()
        {
            // まずは実体の配列を用意して仮想メモリのインスタンスを用意する
            var rawMemory = new SrValue[40];
            var programMemory = new MemoryBlock<SrValue>(rawMemory, 0, 10);
            var globalMemory = new MemoryBlock<SrValue>(rawMemory, 10, 10);
            var heapMemory = new MemoryBlock<SrValue>(rawMemory, 20, 10);
            var stackMemory = new MemoryBlock<SrValue>(rawMemory, 30, 10);
            var virtualMemory = new SrVirtualMemory(programMemory, globalMemory, heapMemory, stackMemory);


            // プログラムメモリ領域に対して書き込んだら実体にも書き込まれているか確認
            virtualMemory[0x00000004] = 123456;
            Assert.AreEqual(123456, programMemory[0x00000004].Primitive.Int);
            Assert.AreEqual(123456, rawMemory[0x00000004].Primitive.Int);


            // グローバル領域に対して書き込んだら実体にも書き込まれているか確認
            virtualMemory[0x00100005] = 654321;
            Assert.AreEqual(654321, globalMemory[0x00000005].Primitive.Int);
            Assert.AreEqual(654321, rawMemory[0x0000000F].Primitive.Int);


            // ヒープ領域に対して書き込んだら実体にも書き込まれているか確認
            virtualMemory[0x00200009] = 112233;
            Assert.AreEqual(112233, heapMemory[0x00000009].Primitive.Int);
            Assert.AreEqual(112233, rawMemory[0x0000001D].Primitive.Int);


            // スタック領域に対して書き込んだら実体にも書き込まれているか確認
            virtualMemory[0x00300004] = 445566;
            Assert.AreEqual(445566, stackMemory[0x00000004].Primitive.Int);
            Assert.AreEqual(445566, rawMemory[0x00000022].Primitive.Int);
        }
    }
}
