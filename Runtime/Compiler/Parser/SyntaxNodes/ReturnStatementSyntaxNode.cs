﻿// zlib/libpng License
//
// Copyright(c) 2020 Sinoa
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

using SnowRabbit.Compiler.Assembler.Symbols;
using SnowRabbit.RuntimeEngine;

namespace SnowRabbit.Compiler.Parser.SyntaxNodes
{
    public class ReturnStatementSyntaxNode : SyntaxNode
    {
        public override void Compile(SrCompileContext context)
        {
            var functionSymbol = context.AssemblyData.GetFunctionSymbol(context.CurrentCompileFunctionName);
            if (Children.Count > 0)
            {
                if (functionSymbol.ReturnType == SrRuntimeType.Void)
                {
                    // 関数は戻り地を返さない
                    throw new System.Exception();
                }


                Children[0].Compile(context);
            }
            else
            {
                if (functionSymbol.ReturnType != SrRuntimeType.Void)
                {
                    // 関数に戻り地を必要とする
                    throw new System.Exception();
                }
            }


            var instruction = new SrInstruction();
            instruction.Set(OpCode.Brl, 0, 0, 0, context.CurrentFunctionLeaveLabelSymbol.InitialAddress);
            context.AddBodyCode(instruction, true);
        }
    }
}
