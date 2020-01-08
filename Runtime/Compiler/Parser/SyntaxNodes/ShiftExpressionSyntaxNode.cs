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

namespace SnowRabbit.Compiler.Parser.SyntaxNodes
{
    /// <summary>
    /// ビットシフト演算構文を表す構文ノードをクラスです
    /// </summary>
    public class ShiftExpressionSyntaxNode : SyntaxNode
    {
        // メンバ変数定義
        private Token operation;



        /// <summary>
        /// この構文ノードが対応する構文ノードを生成します
        /// </summary>
        /// <param name="context">コンパイルする対象となる翻訳単位コンテキスト</param>
        /// <returns>構文ノードを生成出来た場合は構文ノードのインスタンスを、生成出来ない場合は null を返します</returns>
        public static SyntaxNode Create(LocalCompileContext context)
        {
            // トークンの参照を取得する
            ref var token = ref context.Lexer.LastReadToken;


            // 次の優先順位の高い式を生成して自身の式に対応するトークンが続く間ループ
            var expression = AddSubExpressionSyntaxNode.Create(context);
            while (token.Kind == TokenKind.DoubleOpenAngle || token.Kind == TokenKind.DoubleCloseAngle)
            {
                // 実行するべきオペレーションを覚える
                var operation = token;


                // トークンを読み込んでもう一度次の優先順位の高い式を生成する
                context.Lexer.ReadNextToken();
                var leftExpression = AddSubExpressionSyntaxNode.Create(context);


                // 自身の構文を生成する
                var relationalExpression = new ShiftExpressionSyntaxNode();
                relationalExpression.Add(leftExpression);
                relationalExpression.Add(expression);
                relationalExpression.operation = operation;


                // 自身が右辺になる
                expression = relationalExpression;
            }


            // 最終的な式を返す
            return expression;
        }
    }
}
