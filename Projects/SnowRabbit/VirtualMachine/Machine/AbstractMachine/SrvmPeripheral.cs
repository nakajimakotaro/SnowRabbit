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
using System.Collections.Generic;
using SnowRabbit.VirtualMachine.Runtime;

namespace SnowRabbit.VirtualMachine.Machine
{
    /// <summary>
    /// 仮想マシンが実装する仮想マシン周辺装置の抽象クラスです
    /// </summary>
    public abstract class SrvmPeripheral : SrvmMachineParts
    {
        // メンバ変数定義
        private int nextFunctionID;
        private Dictionary<string, int> functionIDTable;
        private Dictionary<int, Action<SrStackFrame>> functionTable;



        /// <summary>
        /// 周辺機器名を取得します
        /// </summary>
        public abstract string PeripheralName { get; }



        /// <summary>
        /// SrvmPeripheral クラスのインスタンスを初期化します
        /// </summary>
        public SrvmPeripheral()
        {
            // 必要なフィールドの初期化をする
            nextFunctionID = 0;
            functionIDTable = new Dictionary<string, int>();
            functionTable = new Dictionary<int, Action<SrStackFrame>>();
        }


        /// <summary>
        /// 関数テーブルを初期化します
        /// </summary>
        internal void InitializeFunctionTable()
        {
            // 関数テーブルに登録するための関数を関数セットアップ関数に渡す
            SetupFunction(AddFunction);
        }


        /// <summary>
        /// 関数テーブルに登録されているすべての関数を解除します
        /// </summary>
        internal void UnregisterFunctionAll()
        {
            functionTable.Clear();
            functionIDTable.Clear();
        }


        internal int GetFunctionID(string name)
        {
            return functionIDTable.TryGetValue(name, out var id) ? id : -1;
        }


        internal Action<SrStackFrame> GetFunction(int id)
        {
            return functionTable.TryGetValue(id, out var function) ? function : null;
        }


        private void AddFunction(string name, Action<SrStackFrame> hostFunction)
        {
            var functionID = nextFunctionID++;
            functionIDTable[name] = functionID;
            functionTable[functionID] = hostFunction;
        }


        protected abstract void SetupFunction(Action<string, Action<SrStackFrame>> registryHandler);
    }
}