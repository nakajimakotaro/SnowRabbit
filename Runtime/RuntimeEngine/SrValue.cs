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

namespace SnowRabbit.RuntimeEngine
{
    /// <summary>
    /// 仮想マシンが使用する値の最小単位を表現した構造体です
    /// </summary>
    public struct SrValue
    {
        /// <summary>
        /// プリミティブ型としての値
        /// </summary>
        public SrPrimitive Primitive;


        /// <summary>
        /// 参照型としての値
        /// </summary>
        public object Object;



        /// <summary>
        /// sbyte から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">sbyte の値</param>
        public static implicit operator SrValue(sbyte value)
        {
            // sbyte を受け取る
            SrValue srValue = default;
            srValue.Primitive.Sbyte = value;
            return srValue;
        }


        /// <summary>
        /// byte から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">byte の値</param>
        public static implicit operator SrValue(byte value)
        {
            // byte を受け取る
            SrValue srValue = default;
            srValue.Primitive.Byte = value;
            return srValue;
        }


        /// <summary>
        /// short から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">short の値</param>
        public static implicit operator SrValue(short value)
        {
            // short を受け取る
            SrValue srValue = default;
            srValue.Primitive.Short = value;
            return srValue;
        }


        /// <summary>
        /// ushort から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">ushort の値</param>
        public static implicit operator SrValue(ushort value)
        {
            // ushort を受け取る
            SrValue srValue = default;
            srValue.Primitive.Ushort = value;
            return srValue;
        }


        /// <summary>
        /// char から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">char の値</param>
        public static implicit operator SrValue(char value)
        {
            // char を受け取る
            SrValue srValue = default;
            srValue.Primitive.Char = value;
            return srValue;
        }


        /// <summary>
        /// int から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">int の値</param>
        public static implicit operator SrValue(int value)
        {
            // int を受け取る
            SrValue srValue = default;
            srValue.Primitive.Int = value;
            return srValue;
        }


        /// <summary>
        /// uint から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">uint の値</param>
        public static implicit operator SrValue(uint value)
        {
            // uint を受け取る
            SrValue srValue = default;
            srValue.Primitive.Uint = value;
            return srValue;
        }


        /// <summary>
        /// long から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">long の値</param>
        public static implicit operator SrValue(long value)
        {
            // long を受け取る
            SrValue srValue = default;
            srValue.Primitive.Long = value;
            return srValue;
        }


        /// <summary>
        /// ulong から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">ulong の値</param>
        public static implicit operator SrValue(ulong value)
        {
            // ulong を受け取る
            SrValue srValue = default;
            srValue.Primitive.Ulong = value;
            return srValue;
        }


        /// <summary>
        /// float から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">float の値</param>
        public static implicit operator SrValue(float value)
        {
            // float を受け取る
            SrValue srValue = default;
            srValue.Primitive.Float = value;
            return srValue;
        }


        /// <summary>
        /// double から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">double の値</param>
        public static implicit operator SrValue(double value)
        {
            // double を受け取る
            SrValue srValue = default;
            srValue.Primitive.Double = value;
            return srValue;
        }


        /// <summary>
        /// string から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">string の値</param>
        public static implicit operator SrValue(string value)
        {
            // string を受け取る
            SrValue srValue = default;
            srValue.Object = value;
            return srValue;
        }


        /// <summary>
        /// SrInstruction から SrValue へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrInstruction の値</param>
        public static implicit operator SrValue(SrInstruction value)
        {
            // SrInstruction を受け取る
            SrValue srValue = default;
            srValue.Primitive.Instruction = value;
            return srValue;
        }


        /// <summary>
        /// SrValue から sbyte へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator sbyte(SrValue value)
        {
            // sbyte の返却
            return value.Primitive.Sbyte;
        }


        /// <summary>
        /// SrValue から byte へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator byte(SrValue value)
        {
            // byte の返却
            return value.Primitive.Byte;
        }


        /// <summary>
        /// SrValue から short へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator short(SrValue value)
        {
            // short の返却
            return value.Primitive.Short;
        }


        /// <summary>
        /// SrValue から ushort へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator ushort(SrValue value)
        {
            // ushort の返却
            return value.Primitive.Ushort;
        }


        /// <summary>
        /// SrValue から char へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator char(SrValue value)
        {
            // char の返却
            return value.Primitive.Char;
        }


        /// <summary>
        /// SrValue から int へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator int(SrValue value)
        {
            // int の返却
            return value.Primitive.Int;
        }


        /// <summary>
        /// SrValue から uint へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator uint(SrValue value)
        {
            // uint の返却
            return value.Primitive.Uint;
        }


        /// <summary>
        /// SrValue から long へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator long(SrValue value)
        {
            // long の返却
            return value.Primitive.Long;
        }


        /// <summary>
        /// SrValue から ulong へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator ulong(SrValue value)
        {
            // ulong の返却
            return value.Primitive.Ulong;
        }


        /// <summary>
        /// SrValue から float へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator float(SrValue value)
        {
            // float の返却
            return value.Primitive.Float;
        }


        /// <summary>
        /// SrValue から double へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator double(SrValue value)
        {
            // double の返却
            return value.Primitive.Double;
        }


        /// <summary>
        /// SrValue から string へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator string(SrValue value)
        {
            // string へ as キャストして返却
            return value.Object as string;
        }


        /// <summary>
        /// SrValue から SrInstruction へのキャストオーバーライドです
        /// </summary>
        /// <param name="value">SrValue インスタンス</param>
        public static implicit operator SrInstruction(SrValue value)
        {
            // SrInstruction の返却
            return value.Primitive.Instruction;
        }
    }
}