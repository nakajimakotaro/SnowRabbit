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
using System.Runtime.Serialization;
using CarrotCompilerCollection.Compiler;

namespace CarrotCompilerCollection.IO
{
    /// <summary>
    /// スクリプト名に対してリソースが見つからなかった場合の例外です
    /// </summary>
    [Serializable]
    public class ScriptNotFoundException : CccCompileErrorException
    {
        /// <summary>
        /// ScriptNotFoundException クラスのインスタンスを初期化します
        /// </summary>
        /// <param name="scriptName">見つけられなかったスクリプト名</param>
        public ScriptNotFoundException(string scriptName) : base($"スクリプト名 '{scriptName}' のスクリプトが見つかりませんでした。")
        {
        }


        /// <summary>
        /// ScriptNotFoundException のインスタンスを初期化します
        /// </summary>
        /// <param name="scriptName">見つけられなかったスクリプト名</param>
        /// <param name="innerException">この例外を発生させた原因の例外</param>
        public ScriptNotFoundException(string scriptName, Exception innerException) : base($"スクリプト名 '{scriptName}' のスクリプトが見つかりませんでした。", innerException)
        {
        }


        /// <summary>
        /// シリアル化したデータから ScriptNotFoundException のインスタンスを初期化します
        /// </summary>
        /// <param name="info">シリアル化されたオブジェクト情報</param>
        /// <param name="context">シリアルデータの転送コンテキスト</param>
        protected ScriptNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}