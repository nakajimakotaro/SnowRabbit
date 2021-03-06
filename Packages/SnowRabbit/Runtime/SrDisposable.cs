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

using System;

namespace SnowRabbit
{
    /// <summary>
    /// SnowRabbit で実装する解放可能なクラスを実装する場合の
    /// </summary>
    public class SrDisposable : IDisposable
    {
        /// <summary>
        /// 確保したリソースを解放します
        /// </summary>
        ~SrDisposable()
        {
            // アンマネージドのみの解放をする
            Dispose(false);
        }


        /// <summary>
        /// 確保したリソースを解放します
        /// </summary>
        public void Dispose()
        {
            // マネージドを含む解放をしてGCのファイナライザに追加されないようにする
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 確保したリソースを解放します
        /// </summary>
        /// <param name="disposing">マネージドを含む解放の場合は true を、アンマネージドのみ解放の場合は false を指定</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
