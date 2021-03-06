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

using SnowRabbit.Compiler.Lexer;

namespace SnowRabbit.Compiler.Assembler.Symbols
{
    /// <summary>
    /// グローバル変数シンボルを表すシンボルクラスです
    /// </summary>
    public class SrGlobalVariableSymbol : SrVariableSymbol
    {
        /// <summary>
        /// 初期化式によってリテラル値がある場合の値
        /// </summary>
        public Token InitializeLiteral { get; set; }



        /// <summary>
        /// SrGlobalVariableSymbol クラスのインスタンスを初期化します
        /// </summary>
        /// <param name="name">シンボル名</param>
        /// <param name="initialAddress">初期アドレス</param>
        public SrGlobalVariableSymbol(string name, int initialAddress) : base(name, initialAddress, SrScopeType.Global, SrSymbolKind.GlobalVariable)
        {
        }
    }
}
