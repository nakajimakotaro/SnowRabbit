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

/*

# 雪兎のスクリプト構文

## Simple syntax

### literal
    : <integer>
    | <number>
    | <string>
    | 'true'
    | 'false'
    | 'null'

### type
    : 'void'
    | 'int'
    | 'number'
    | 'string'
    | 'object'
    | 'bool'

### parameter
    : type <identifier>

### argument
    : expression

### type_list
    : type { ',' type }

### parameter_list
    : parameter { ',' parameter }

### argument_list
    : argument { ',' argument }



## Compile unit syntax

### compile_unit
    : { directives }
    | { peripheral_declare }
    | { global_variable_declare }
    | { function_declare }



## Pre-Processor directive syntax

### directives
    : '#' script_compile_directive
    | '#' link_object_directive
    | '#' constant_define_directive

### script_compile_directive
    : 'compile' <string>

### link_object_directive
    : 'link' <string>

### constant_define_directive
    : 'const' <identifier> literal



## Define and Declare syntax

### peripheral_declare
    : 'using' <identifier> '=' type <identifier> '.' <identifier> '(' [type_list] ')' ';' 

### global_variable_declare
    : 'global' type <identifier> [ '=' literal ] ';'

### local_variable_declare
    : 'local' type <identifier> [ '=' expression ] ';'

### function_declare
    : 'function' type <identifier> '(' [parameter_list] ')' { block } 'end'



## Block syntax

### block
    : statement

### statement
    : empty_statement
    | local_variable_declare
    | for_statement
    | while_statement
    | if_statement
    | break_statement
    | return_statement
    | expression ';'



## Statement syntax

### empty_statement
    : ';'

### for_statement
    : 'for' '(' [ expression ] ';' [ expression ] ';' [ expression ] ')' { block } 'end'

### while_statement
    : 'while' '(' expression ')' { block } 'end'

### if_statement
    : 'if' '(' expression ')' { block } 'end'
    | 'if' '(' expression ')' { block } 'else' { block } 'end'

### break_statement
    : 'break' ';'

### return_statement
    : 'return' [ expression ] ';'



## Expression syntax

### expression
    : assignment_expression

### assignment_expression
    : condition_or_expression
    | assignment_expression { '=' condition_or_expression }
    | assignment_expression { '+=' condition_or_expression }
    | assignment_expression { '-=' condition_or_expression }
    | assignment_expression { '*=' condition_or_expression }
    | assignment_expression { '/=' condition_or_expression }
    | assignment_expression { '&=' condition_or_expression }
    | assignment_expression { '|=' condition_or_expression }
    | assignment_expression { '^=' condition_or_expression }

### condition_or_expression
    : condition_and_expression
    | condition_or_expression { '||' condition_and_expression }

### condition_and_expression
    : logical_or_expression
    | condition_and_expression { '&&' logical_or_expression }

### logical_or_expression
    : logical_exclusive_or_expression
    | logical_or_expression { '|' logical_exclusive_or_expression }

### logical_exclusive_or_expression
    : logical_and_expression
    | logical_exclusive_or_expression { '^' logical_and_expression }

### logical_and_expression
    : equality_expression
    | logical_and_expression { '&' equality_expression }

### equality_expression
    : relational_expression
    | equality_expression { '==' relational_expression }
    | equality_expression { '!=' relational_expression }

### relational_expression
    : shift_expression
    | relational_expression { '<' shift_expression }
    | relational_expression { '>' shift_expression }
    | relational_expression { '<=' shift_expression }
    | relational_expression { '>=' shift_expression }

### shift_expression
    : addsub_expression
    | shift_expression { '<<' addsub_expression }
    | shift_expression { '>>' addsub_expression }

### addsub_expression
    : muldiv_expression
    | addsub_expression { '+' muldiv_expression }
    | addsub_expression { '-' muldiv_expression }

### muldiv_expression
    : unary_expression
    | muldiv_expression { '*' unary_expression }
    | muldiv_expression { '/' unary_expression }

### unary_expression
    : function_call
    | '+' unary_expression
    | '-' unary_expression
    | '!' unary_expression

### function_call
    : primary_expression
    | primary_expression '(' [ argument_list ] ')'

### primary_expression
    : literal
    | <identifier>
    | paren_expression

### paren_expression
    : '(' expression ')'

*/

using System;
using System.Collections.Generic;
using SnowRabbit.Compiler.IO;
using SnowRabbit.Compiler.Lexer;
using SnowRabbit.Compiler.Parser.SyntaxNodes;

namespace SnowRabbit.Compiler.Parser
{
    /// <summary>
    /// SnowRabbit の構文解析を行う構文解析クラスです
    /// </summary>
    public class SrParser
    {
        // メンバ変数定義
        private readonly ISrScriptStorage scriptStorage;
        private readonly Stack<SrLexer> lexerStack;
        private SrLexer currentLexer;



        #region コンストラクタとAPIインターフェイス
        /// <summary>
        /// SrParser クラスのインスタンスを初期化します
        /// </summary>
        /// <param name="storage">構文解析するスクリプトを読み込むストレージ</param>
        /// <exception cref="ArgumentNullException">storage が null です</exception>
        public SrParser(ISrScriptStorage storage)
        {
            // 諸々初期化する
            scriptStorage = storage ?? throw new ArgumentNullException(nameof(storage));
            lexerStack = new Stack<SrLexer>();
        }


        /// <summary>
        /// 指定されたパスにあるスクリプトの構文解析をします。
        /// </summary>
        /// <param name="path">構文解析する対象となるスクリプトのパス</param>
        /// <returns>構文解析された抽象構文木のルートノードを返します</returns>
        /// <exception cref="ScriptNotFoundException">スクリプト '{path}' が見つけられませんでした。</exception>
        public SyntaxNode Parse(string path)
        {
            // ストレージからスクリプトを開いてレキサを生成する
            var textReader = scriptStorage.OpenRead(path);
            using (var lexer = new SrLexer(path, textReader ?? throw new ScriptNotFoundException(path)))
            {
                // 現在処理中のレキサがいるのならスタックに積んでおいて、処理するべきレキサを切り替える
                if (currentLexer != null) lexerStack.Push(currentLexer);
                currentLexer = lexer;


                // レキサに一番最初のトークンを読み込ませて、コンパイル単位の翻訳を始める
                lexer.ReadNextToken();
                var rootNode = ParseCompileUnit();


                // スタックにレキサが積まれていればポップして処理するべきレキサを戻して、ルートノードを返す
                currentLexer = lexerStack.Count > 0 ? lexerStack.Pop() : null;
                return rootNode;
            }
        }
        #endregion


        #region Utilities
        /// <summary>
        /// トークンが最後まで読み込まれたかどうかを調べます
        /// </summary>
        /// <returns>最後までトークンが読み込まれた場合は true を、まだ読み込まれていない場合は false を返します</returns>
        private bool CheckEndOfToken()
        {
            // そのままのプロパティ値を返す
            return currentLexer.EndOfToken;
        }


        /// <summary>
        /// 次のトークンを読み込みます
        /// </summary>
        private void ReadNextToken()
        {
            // 次のトークンを読み込むだけ
            currentLexer.ReadNextToken();
        }


        /// <summary>
        /// 指定されたトークンかどうかを調べます
        /// </summary>
        /// <param name="tokenKind">調べるトークン</param>
        /// <returns>指定されたトークンの場合は true を、異なる場合は false を返します</returns>
        private bool CheckToken(int tokenKind)
        {
            // そのまま比較結果を返す
            return currentLexer.LastReadToken.Kind == tokenKind;
        }


        /// <summary>
        /// 指定されたトークンかどうかを調べて、指定されたトークンの場合は次のトークンを読み込みます
        /// </summary>
        /// <param name="tokenKind">調べるトークン</param>
        /// <returns>指定されたトークンの場合は true を、異なる場合は false を返します</returns>
        private bool CheckTokenAndReadNext(int tokenKind)
        {
            // もしトークン比較結果をもらって異なる場合はそのまま失敗を返す
            if (!CheckToken(tokenKind)) return false;
            currentLexer.ReadNextToken();
            return true;
        }


        /// <summary>
        /// 現在の位置にいるトークンの参照を取得します
        /// </summary>
        /// <returns>現在の位置にいるトークンの参照を返します</returns>
        private ref Token GetCurrentToken()
        {
            // 最後に読み取ったトークンの参照を返す
            return ref currentLexer.LastReadToken;
        }


        /// <summary>
        /// 現在の位置にいるトークンを取り出して、次のトークンを読み込みます
        /// </summary>
        /// <param name="token">取り出したトークンを設定する参照パラメータ</param>
        private void GetCurrentTokenAndReadNext(out Token token)
        {
            // 最後に読み取ったトークンを設定して次のトークンを読み込む
            token = currentLexer.LastReadToken;
            currentLexer.ReadNextToken();
        }
        #endregion


        #region 全パース関数
        #region Simple syntax
        private SyntaxNode ParseIdentifier()
        {
            if (!CheckToken(TokenKind.Identifier)) return null;
            GetCurrentTokenAndReadNext(out var token);
            return new IdentifierSyntaxNode(token);
        }


        private SyntaxNode ParseLiteral()
        {
            if (CheckToken(TokenKind.Integer) ||
                CheckToken(TokenKind.Number) ||
                CheckToken(TokenKind.String) ||
                CheckToken(SrTokenKind.True) ||
                CheckToken(SrTokenKind.False) ||
                CheckToken(SrTokenKind.Null))
            {
                GetCurrentTokenAndReadNext(out var token);
                return new LiteralSyntaxNode(token);
            }


            return null;
        }


        private SyntaxNode ParseType()
        {
            if (CheckToken(SrTokenKind.TypeVoid) ||
                CheckToken(SrTokenKind.TypeInt) ||
                CheckToken(SrTokenKind.TypeNumber) ||
                CheckToken(SrTokenKind.TypeString) ||
                CheckToken(SrTokenKind.TypeObject) ||
                CheckToken(SrTokenKind.TypeBool))
            {
                GetCurrentTokenAndReadNext(out var token);
                return new TypeSyntaxNode(token);
            }


            return null;
        }


        private SyntaxNode ParseParameter()
        {
            var type = ParseType();
            if (type == null) return null;
            var name = ParseIdentifier();
            if (name == null) return null;


            var node = new ParameterSyntaxNode();
            node.Add(type);
            node.Add(name);
            return node;
        }


        private SyntaxNode ParseArgument()
        {
            var expression = ParseExpression();
            if (expression == null) return null;


            var argument = new ArgumentSyntaxNode();
            argument.Add(expression);
            return argument;
        }


        private SyntaxNode ParseTypeList()
        {
            var type = ParseType();
            if (type == null) return null;


            var typeList = new TypeListSyntaxNode();
            typeList.Add(type);
            while ((type = ParseType()) != null)
            {
                typeList.Add(type);
            }


            return typeList;
        }


        private SyntaxNode ParseParameterList()
        {
            var parameter = ParseParameter();
            if (parameter == null) return null;


            var parameterList = new ParameterListSyntaxNode();
            parameterList.Add(parameter);
            while ((parameter = ParseParameter()) != null)
            {
                parameterList.Add(parameter);
            }


            return parameterList;
        }


        private SyntaxNode ParseArgumentList()
        {
            var argument = ParseArgument();
            if (argument == null) return null;


            var argumentList = new ArgumentListSyntaxNode();
            argumentList.Add(argument);
            while ((argument = ParseArgument()) != null)
            {
                argumentList.Add(argument);
            }


            return argumentList;
        }
        #endregion


        #region Compile unit syntax
        private SyntaxNode ParseCompileUnit()
        {
            var compileUnit = new CompileUnitSyntaxNode();
            while (!CheckEndOfToken())
            {
                var node =
                    ParseDirectives() ??
                    ParsePeripheralDeclare() ??
                    ParseGlobalVariableDeclare() ??
                    ParseFunctionDeclare() ??
                    null;


                if (node == null) return null;
                compileUnit.Add(node);
            }


            return compileUnit.Children.Count > 0 ? compileUnit : null;
        }
        #endregion


        #region Pre-Processor directive syntax
        private SyntaxNode ParseDirectives()
        {
            if (!CheckTokenAndReadNext(TokenKind.Sharp)) return null;


            return
                ParseScriptCompileDirective() ??
                ParseLinkObjectDirective() ??
                ParseConstantDefineDirective() ??
                null;
        }


        private SyntaxNode ParseScriptCompileDirective()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Compile)) return null;
            if (!CheckToken(TokenKind.String)) return null;
            GetCurrentTokenAndReadNext(out var token);
            return Parse(token.Text);
        }


        private SyntaxNode ParseLinkObjectDirective()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Link)) return null;
            if (!CheckToken(TokenKind.String)) return null;
            GetCurrentTokenAndReadNext(out var token);
            return new LinkObjectDirectiveSyntaxNode(token);
        }


        private SyntaxNode ParseConstantDefineDirective()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Const)) return null;
            var name = ParseIdentifier();
            if (name == null) return null;
            var literal = ParseLiteral();
            if (literal == null) return null;


            var constant = new ConstantDefineDirectiveSyntaxNode();
            constant.Add(name);
            constant.Add(literal);
            return constant;
        }
        #endregion


        #region Define and Declare syntax
        private SyntaxNode ParsePeripheralDeclare()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Using)) return null;


            var name = ParseIdentifier();
            if (name == null) return null;
            if (!CheckTokenAndReadNext(TokenKind.Equal)) return null;


            var type = ParseType();
            if (type == null) return null;


            var peripheralName = ParseIdentifier();
            if (peripheralName == null && !CheckTokenAndReadNext(TokenKind.Period)) return null;


            var functionName = ParseIdentifier();
            if (functionName == null && !CheckTokenAndReadNext(TokenKind.OpenParen)) return null;


            var typeList = ParseTypeList();
            if (!CheckTokenAndReadNext(TokenKind.CloseParen)) return null;
            if (!CheckTokenAndReadNext(TokenKind.Semicolon)) return null;


            var peripheralDeclare = new PeripheralDeclareSyntaxNode();
            peripheralDeclare.Add(name);
            peripheralDeclare.Add(type);
            peripheralDeclare.Add(peripheralName);
            peripheralDeclare.Add(functionName);
            peripheralDeclare.Add(typeList);
            return peripheralDeclare;
        }


        private SyntaxNode ParseGlobalVariableDeclare()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Global)) return null;


            var type = ParseType();
            if (type == null) return null;


            var name = ParseIdentifier();
            if (name == null) return null;


            var globalVariableDeclare = new GlobalVariableDeclareSyntaxNode();
            globalVariableDeclare.Add(type);
            globalVariableDeclare.Add(name);


            if (CheckTokenAndReadNext(TokenKind.Equal))
            {
                var literal = ParseLiteral();
                if (literal == null) return null;
                globalVariableDeclare.Add(literal);
            }


            if (!CheckTokenAndReadNext(TokenKind.Semicolon)) return null;
            return globalVariableDeclare;
        }


        private SyntaxNode ParseLocalVariableDeclare()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Local)) return null;


            var type = ParseType();
            if (type == null) return null;


            var name = ParseIdentifier();
            if (name == null) return null;


            var localVariableDeclare = new LocalVariableDeclareSyntaxNode();
            localVariableDeclare.Add(type);
            localVariableDeclare.Add(name);


            if (CheckTokenAndReadNext(TokenKind.Equal))
            {
                var literal = ParseExpression();
                if (literal == null) return null;
                localVariableDeclare.Add(literal);
            }


            if (!CheckTokenAndReadNext(TokenKind.Semicolon)) return null;
            return localVariableDeclare;
        }


        private SyntaxNode ParseFunctionDeclare()
        {
            if (!CheckTokenAndReadNext(SrTokenKind.Function)) return null;


            var type = ParseType();
            if (type == null) return null;
            var name = ParseIdentifier();
            if (name == null) return null;
            if (!CheckTokenAndReadNext(TokenKind.OpenParen)) return null;
            var parameterList = ParseParameterList();
            if (!CheckTokenAndReadNext(TokenKind.CloseParen)) return null;
            var block = ParseBlock();
            if (!CheckTokenAndReadNext(SrTokenKind.End)) return null;


            var functionDeclare = new FunctionDeclareSyntaxNode();
            functionDeclare.Add(type);
            functionDeclare.Add(name);
            functionDeclare.Add(parameterList);
            functionDeclare.Add(block);
            return functionDeclare;
        }
        #endregion


        #region Block syntax
        private SyntaxNode ParseBlock()
        {
            return ParseStatement();
        }


        private SyntaxNode ParseStatement()
        {
            var result =
                ParseEmptyStatement() ??
                ParseForStatement() ??
                ParseWhileStatement() ??
                ParseIfStatement() ??
                ParseBreakStatement() ??
                ParseReturnStatement() ??
                null;
            if (result != null) return result;


            result = ParseExpression();
            if (result == null || !CheckTokenAndReadNext(TokenKind.Semicolon)) return null;
            return result;
        }
        #endregion


        #region Statement syntax
        private SyntaxNode ParseEmptyStatement()
        {
            if (!CheckToken(TokenKind.Semicolon)) return null;
            GetCurrentTokenAndReadNext(out var token);
            return new EmptyStatementSyntaxNode(token);
        }


        private SyntaxNode ParseForStatement()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseWhileStatement()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseIfStatement()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseBreakStatement()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseReturnStatement()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Expression syntax
        private SyntaxNode ParseExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseAssignmentExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseConditionOrExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseConditionAndExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseLogicalOrExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseLogicalExclusiveOrExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseLogicalAndExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseEqualityExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseRelationalExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseShiftExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseAddSubExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseMulDivExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseUnaryExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseFunctionCall()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParsePrimaryExpression()
        {
            throw new NotImplementedException();
        }


        private SyntaxNode ParseParenExpression()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}