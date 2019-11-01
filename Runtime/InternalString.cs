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

namespace SnowRabbit
{
    /// <summary>
    /// 内部用の文字列を取り扱うクラスです
    /// </summary>
    internal static class InternalString
    {
        /// <summary>
        /// Conditional 属性に設定する文字列を保持したクラスです
        /// </summary>
        internal static class Conditional
        {
            /// <summary>
            /// RELEASE コンパイル定数
            /// </summary>
            internal const string RELEASE = "RELEASE";


            /// <summary>
            /// DEBUG コンパイル定数
            /// </summary>
            internal const string DEBUG = "DEBUG";


            /// <summary>
            /// TRACE コンパイル定数
            /// </summary>
            internal const string TRACE = "TRACE";
        }



        /// <summary>
        /// ロガーに渡すタグ文字列を保持したクラスです
        /// </summary>
        internal static class LogTag
        {
            /// <summary>
            /// 仮想マシンタグ
            /// </summary>
            internal const string VIRTUAL_MACHINE = "VirtualMachine";


            /// <summary>
            /// 周辺機器タグ
            /// </summary>
            internal const string PERIPHERAL = "Peripheral";
        }



        /// <summary>
        /// ロガーに渡すメッセージ文字列を保持したクラスです
        /// </summary>
        internal static class LogMessage
        {
            /// <summary>
            /// 仮想マシンのログメッセージを保持したクラスです
            /// </summary>
            internal static class VirtualMachine
            {
                /// <summary>
                /// SnowRabbit.RuntimeEngine.VirtualMachine.SrvmMachineParts.Dispose(bool) が呼び出されたときのログメッセージ
                /// </summary>
                internal const string EMPTY_DISPOSE_CALL = "Called SnowRabbit.RuntimeEngine.VirtualMachine.SrvmMachineParts.Dispose(bool)";
            }



            /// <summary>
            /// 周辺機器のログメッセージを保持したクラスです
            /// </summary>
            internal static class Peripheral
            {
                /// <summary>
                /// 周辺機器関数の生成開始トレースメッセージ
                /// </summary>
                internal const string BEGIN_CREATE_PERIPHERAL_FUNCTION = "Begin create peripheral function.";


                /// <summary>
                /// 戻り値のセットアップをするトレースメッセージ
                /// </summary>
                internal const string SETUP_RETURN_INFO = "Setup return info.";


                /// <summary>
                /// 引数のセットアップをするトレースメッセージ
                /// </summary>
                internal const string SETUP_ARGUMENT_INFO = "Setup argument info.";


                /// <summary>
                /// タスクが戻り値だった場合のトレースメッセージ
                /// </summary>
                internal const string RETURN_TYPE_IS_TASK = "Return type is task.";


                /// <summary>
                /// 戻り値の型がTaskでなかった場合のトレースメッセージ
                /// </summary>
                internal const string RETURN_TYPE_IS_NOT_TASK = "Return type is not task.";


                /// <summary>
                /// 非ジェネリックタスクだった場合のトレースメッセージ
                /// </summary>
                internal const string NON_GENERIC_TASK = "Task is not generic.";


                /// <summary>
                /// 引数が無い場合のトレースメッセージ
                /// </summary>
                internal const string ARGUMENT_EMPTY = "Argument empty.";


                /// <summary>
                /// ジェネリックタスクだった場合のジェネリック型のトレースメッセージ
                /// </summary>
                /// <param name="type">ジェネリックの型</param>
                /// <returns>トレースメッセージ</returns>
                internal static string TASK_GENERIC_TYPE_IS(System.Type type) => $"Task generic type is '{type.FullName}'.";


                /// <summary>
                /// 引数の数のトレースメッセージ
                /// </summary>
                /// <param name="count">引数の数</param>
                /// <returns>トレースメッセージ</returns>
                internal static string ARGUMENT_COUNT_IS(int count) => $"Argument count = {count}.";


                /// <summary>
                /// 対応する変換関数が見つからなかった時のメッセージ
                /// </summary>
                /// <param name="type">見つからなかった型</param>
                /// <returns>メッセージ</returns>
                internal static string CONVERT_FUNCTION_NOT_FOUND(System.Type type) => $"'{type.FullName}' convert function not found.";
            }
        }



        /// <summary>
        /// 例外メッセージ文字列を保持したクラスです
        /// </summary>
        internal static class ExceptionMessage
        {
            /// <summary>
            /// SrPeripheralAttribute クラスの例外メッセージ
            /// </summary>
            internal static class SrPeripheralAttribute
            {
                /// <summary>
                /// 無効な周辺機器名を使用した例外メッセージ
                /// </summary>
                internal const string INVALID_PERIPHERAL_NAME = "周辺機器名に null または 空文字列 または 空白 を設定することは出来ません。";
            }



            /// <summary>
            /// SrHostFunctionAttribute クラスの例外メッセージ
            /// </summary>
            internal static class SrHostFunctionAttribute
            {
                /// <summary>
                /// 無効な関数名を使用した例外メッセージ
                /// </summary>
                internal const string INVALID_FUNCTION_NAME = "関数名に null または 空文字列 または 空白 を設定することは出来ません。";
            }
        }
    }
}