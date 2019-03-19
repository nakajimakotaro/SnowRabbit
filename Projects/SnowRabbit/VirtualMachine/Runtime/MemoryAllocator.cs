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
using static SnowRabbit.VirtualMachine.Runtime.MemoryAllocatorUtility;

namespace SnowRabbit.VirtualMachine.Runtime
{
    #region Utility
    /// <summary>
    /// メモリアロケータの実装で使用する機能をユーティリティ化したクラスです
    /// </summary>
    public static class MemoryAllocatorUtility
    {
        /// <summary>
        /// バイトサイズから配列の要素数を求めます
        /// </summary>
        /// <param name="size">計算するバイト数</param>
        /// <returns>バイト数から要素数に変換した値を返します</returns>
        public static int ByteSizeToElementCount(int size)
        {
            // パディング込みの計算式例（ビット演算ではなく汎用計算式の場合）
            // paddingSize = (blockSize - (dataSize % blockSize)) % blockSize
            // finalSize = dataSize + paddingSize
            // blockCount = finalSize / blockSize
            // サイズから確保するべき配列の要素数を求める（1ブロック8byteなので8byte倍数に収まるようにしてから求める）
            return (size + ((8 - (size & 7)) & 7)) >> 3;
        }


        /// <summary>
        /// 配列の要素数からバイトサイズを求めます
        /// </summary>
        /// <param name="count">計算する要素数</param>
        /// <returns>要素数からバイト数に変換した値を返します</returns>
        public static int ElementCountToByteSize(int count)
        {
            // ブロックサイズは8byteなのでそのまま8倍にして返す
            return count << 3;
        }
    }
    #endregion



    #region interface
    /// <summary>
    /// 仮想マシンが使用するメモリを確保するメモリアロケータインターフェイスです
    /// </summary>
    public interface IMemoryAllocator
    {
        /// <summary>
        /// メモリの確保をします。確保されるメモリのサイズは size が収まる様に確保されますが、実際の確保サイズは必ず一致しているかどうかを保証していません。
        /// </summary>
        /// <param name="size">確保するメモリのサイズ</param>
        /// <param name="type">確保するメモリの利用タイプ</param>
        /// <returns>確保したメモリのブロックを返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">確保するサイズに 0 以下の指定は出来ません</exception>
        /// <exception cref="SrOutOfMemoryException">指定されたサイズのメモリを確保できませんでした</exception>
        MemoryBlock Allocate(int size, AllocationType type);


        /// <summary>
        /// 指定されたメモリブロックを解放します。
        /// </summary>
        /// <param name="memoryBlock">解放するメモリブロック</param>
        void Deallocate(MemoryBlock memoryBlock);
    }
    #endregion



    #region abstract MemoryAllocator
    /// <summary>
    /// 仮想マシンが使用するメモリを確保するメモリアロケータの抽象クラスです
    /// </summary>
    public abstract class MemoryAllocator : IMemoryAllocator
    {
        /// <summary>
        /// メモリの確保をします。確保されるメモリのサイズは size が収まる様に確保されますが、実際の確保サイズは必ず一致しているかどうかを保証していません。
        /// </summary>
        /// <param name="size">確保するメモリのサイズ</param>
        /// <param name="type">確保するメモリの利用タイプ</param>
        /// <returns>確保したメモリのブロックを返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">確保するサイズに 0 以下の指定は出来ません</exception>
        /// <exception cref="SrOutOfMemoryException">指定されたサイズのメモリを確保できませんでした</exception>
        /// <exception cref="ArgumentException">メモリ確保の種類に 'Free' の指定は出来ません</exception>
        public abstract MemoryBlock Allocate(int size, AllocationType type);


        /// <summary>
        /// 指定されたメモリブロックを解放します。
        /// </summary>
        /// <param name="memoryBlock">解放するメモリブロック</param>
        public abstract void Deallocate(MemoryBlock memoryBlock);


        /// <summary>
        /// 無効な確保サイズ または 空き領域タイプとして 要求された時に例外をスローします
        /// </summary>
        /// <param name="size">判断するための要求メモリサイズ</param>
        /// <exception cref="ArgumentOutOfRangeException">確保するサイズに 0 以下の指定は出来ません</exception>
        /// <exception cref="ArgumentException">メモリ確保の種類に 'Free' の指定は出来ません</exception>
        protected void ThrowExceptionIfRequestInvalidSizeOrType(int size, AllocationType type)
        {
            // 0 以下のサイズ要求が有った場合は
            if (size <= 0)
            {
                // 例外を吐く
                throw new ArgumentOutOfRangeException(nameof(size), "確保するサイズに 0 以下の指定は出来ません");
            }


            // 空き領域タイプの要求は無効
            if (type == AllocationType.Free)
            {
                // 例外を吐く
                throw new ArgumentException($"メモリ確保の種類に '{nameof(AllocationType.Free)}' の指定は出来ません", nameof(type));
            }
        }


        /// <summary>
        /// 要求メモリサイズの確保が出来ない例外をスローします
        /// </summary>
        protected void ThrowExceptionSrOutOfMemory()
        {
            // 無条件で例外をスローする
            throw new SrOutOfMemoryException("指定されたサイズのメモリを確保できませんでした");
        }
    }
    #endregion



    #region Standard MemoryAllocator
    /// <summary>
    /// SnowRabbit 仮想マシンが使用する標準メモリアロケータクラスです
    /// </summary>
    public class StandardMemoryAllocator : MemoryAllocator
    {
        // 定数定義
        public const int HeadMemoryInfoCount = 1;
        public const int TailMemoryInfoCount = 1;
        public const int RequireMemoryInfoCount = HeadMemoryInfoCount + TailMemoryInfoCount;
        public const int RequireMinimumPoolCount = RequireMemoryInfoCount + 1;
        public const int RequireMinimumPoolSize = RequireMinimumPoolCount * 8;
        private const int IndexNotFound = -1;

        // メンバ変数定義
        private SrValue[] memoryPool;
        private int freePoolHeadIndex;



        #region Constructor and initializer
        /// <summary>
        /// メモリプールの容量を指定して StandardMemoryAllocator のインスタンスを初期化します
        /// </summary>
        /// <param name="memoryPoolSize">確保しておくメモリプールのバイトサイズ。ただし、内部で SrValue 型のサイズが確保出来るように倍数調整が行われます。</param>
        /// <exception cref="ArgumentOutOfRangeException">poolSize が RequireMinimumPoolSize 未満です</exception>
        public StandardMemoryAllocator(int memoryPoolSize)
        {
            // 事前の例外ハンドリングをして、要求サイズから確保するサイズを求めて配列を生成後初期化する
            ThrowExceptionIfInvalidPoolSize(memoryPoolSize);
            Initialize(new SrValue[ByteSizeToElementCount(memoryPoolSize)]);
        }


        /// <summary>
        /// メモリプールを直接渡して StandardMemoryAllocator のインスタンスを初期化します
        /// </summary>
        /// <param name="memoryPool">このアロケータに使ってもらうメモリプール</param>
        /// <exception cref="ArgumentNullException">memoryPool が null です</exception>
        /// <exception cref="ArgumentOutOfRangeException">memoryPool の長さが RequireMinimumPoolCount 未満です</exception>
        public StandardMemoryAllocator(SrValue[] memoryPool)
        {
            // 渡された引数を使って、そのまま初期化処理を呼ぶ
            Initialize(memoryPool);
        }


        /// <summary>
        /// 指定されたメモリプールのバッファでインスタンスの初期化を行います
        /// </summary>
        /// <param name="memoryPool">このアロケータが使用するメモリプール</param>
        /// <exception cref="ArgumentNullException">memoryPool が null です</exception>
        /// <exception cref="ArgumentOutOfRangeException">memoryPool の長さが RequireMinimumPoolCount 未満です</exception>
        private unsafe void Initialize(SrValue[] memoryPool)
        {
            // 事前例外ハンドリングをして、メンバ変数を初期化する
            ThrowExceptionIfInvalidMemoryPool(memoryPool);
            this.memoryPool = memoryPool;
            freePoolHeadIndex = 0;


            // 最初のフリー領域の大きさを管理する
            SetAllocationInfo(freePoolHeadIndex, memoryPool.Length - (RequireMinimumPoolCount - 1), AllocationType.Free, IndexNotFound);
        }
        #endregion


        #region AllocationInfo control function
        /// <summary>
        /// 指定したメモリプールのインデックスに対して、メモリ管理情報を設定します
        /// </summary>
        /// <param name="poolIndex">管理情報を設定するメモリプールのインデックス位置</param>
        /// <param name="allocatedCount">管理情報を含まない要求メモリの確保要素数</param>
        /// <param name="type">該当メモリブロックの確保タイプ</param>
        /// <param name="nextIndex">次のメモリブロック管理インデックス</param>
        private unsafe void SetAllocationInfo(int poolIndex, int allocatedCount, AllocationType type, int nextIndex)
        {
            // 指定されたインデックスの位置には [0]確保した要素の数(管理情報を含まない数) [1]メモリ管理情報(管理タイプ) が設定される
            // さらに、メモリ確保分サイズの末尾には [0]確保した要素の数(管理情報を含む全体の数) [1]次のフリーリストインデックス が設定される
            var headInfoIndex = poolIndex;
            var tailInfoIndex = poolIndex + HeadMemoryInfoCount + allocatedCount;
            memoryPool[headInfoIndex].Value.Int[0] = allocatedCount;
            memoryPool[headInfoIndex].Value.Int[1] = (int)type;
            memoryPool[tailInfoIndex].Value.Int[0] = allocatedCount + RequireMemoryInfoCount;
            memoryPool[tailInfoIndex].Value.Int[1] = nextIndex;
        }


        /// <summary>
        /// 指定したメモリプールのインデックスから、メモリ管理情報を取得します
        /// </summary>
        /// <param name="poolIndex">管理情報を取得するメモリプールのインデックス位置</param>
        /// <param name="allocatedCount">管理情報を含まない要求メモリの確保要素数</param>
        /// <param name="type">該当メモリブロックの確保タイプ</param>
        /// <param name="nextIndex">次のメモリブロック管理インデックス</param>
        private unsafe void GetAllocationInfo(int poolIndex, out int allocatedCount, out AllocationType type, out int nextIndex)
        {
            // 指定されたインデックスの位置には [0]確保した要素の数(管理情報を含まない数) [1]メモリ管理情報(管理タイプ) が入っている
            allocatedCount = memoryPool[poolIndex].Value.Int[0];
            type = (AllocationType)memoryPool[poolIndex].Value.Int[1];


            // メモリ確保分サイズの末尾には [0]確保した要素の数(管理情報を含む全体の数) [1]次のフリーリストインデックス が入っている
            nextIndex = memoryPool[poolIndex + HeadMemoryInfoCount + allocatedCount].Value.Int[1];
        }


        /// <summary>
        /// 指定したメモリプールのインデックスから、1つ前のメモリ管理情報メモリプールインデックスを取得します。
        /// </summary>
        /// <param name="poolIndex">取得したい1つ前のメモリ情報を知るためのメモリプールインデックス</param>
        /// <returns>指定されたプールインデックスの1つ前のメモリ管理情報メモリプールインデックスを取得出来た場合はそのインデックスを返しますが、先頭もしくは見つけられない場合はIndexNotFoundを返します</returns>
        private unsafe int GetPreviousAllocationInfoIndex(int poolIndex)
        {
            // 指定されたインデックス位置が0以下なら
            if (poolIndex <= 0)
            {
                // インデックスを見つけられなかったことを返す
                return IndexNotFound;
            }


            // 1つ前の要素から確保情報全体サイズを取得後、1つ前のインデックスを求めて負の値なら
            var totalAllocatedCount = memoryPool[poolIndex - 1].Value.Int[0];
            var previousMemoryInfoIndex = poolIndex - totalAllocatedCount;
            if (previousMemoryInfoIndex < 0)
            {
                // 取得した管理情報は正しい管理情報ではないので見つけられなかったことを返す
                return IndexNotFound;
            }


            // 計算されたインデックスを返す
            return previousMemoryInfoIndex;
        }


        /// <summary>
        /// 最後にいる空きメモリ管理情報のプールインデックスを取得します
        /// </summary>
        /// <returns>最後の空きメモリ管理情報のプールインデックスを返しますが、空きメモリがない場合は IndexNotFound を返します</returns>
        private unsafe int GetLastFreeMemoryInfoIndex()
        {
            // 最初の空きインデックスが負の値なら
            if (freePoolHeadIndex < 0)
            {
                // 空き領域がないことを返す
                return IndexNotFound;
            }


            // 次のインデックスがIndexNotFoundになるまで空きメモリ領域を回る
            var hitIndex = freePoolHeadIndex;
            GetAllocationInfo(hitIndex, out _, out _, out var nextFreePoolIndex);
            while (nextFreePoolIndex != IndexNotFound)
            {
                // 次の情報を取得する
                hitIndex = nextFreePoolIndex;
                GetAllocationInfo(hitIndex, out _, out _, out nextFreePoolIndex);
            }


            // 最後に到達したインデックスを返す
            return hitIndex;
        }
        #endregion


        #region Allocation function
        /// <summary>
        /// メモリの確保をします。確保されるメモリのサイズは size が収まる様に確保されますが、実際の確保サイズは必ず一致しているかどうかを保証していません。
        /// </summary>
        /// <param name="size">確保するメモリのサイズ</param>
        /// <param name="type">確保するメモリの利用タイプ</param>
        /// <returns>確保したメモリのブロックを返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">確保するサイズに 0 以下の指定は出来ません</exception>
        /// <exception cref="SrOutOfMemoryException">指定されたサイズのメモリを確保できませんでした</exception>
        /// <exception cref="ArgumentException">メモリ確保の種類に 'Free' の指定は出来ません</exception>
        public override MemoryBlock Allocate(int size, AllocationType type)
        {
            // 事前例外処理をして必要な確保要素数を求める
            ThrowExceptionIfRequestInvalidSizeOrType(size, type);
            var requestCount = ByteSizeToElementCount(size);


            // 空き領域を検索して見つけられなかったら
            var freeIndex = FindFirstFreeBlock(requestCount);
            if (freeIndex == IndexNotFound)
            {
                // メモリの空き容量が足りない例外を吐く
                ThrowExceptionSrOutOfMemory();
            }


            // 見つけた空き領域を適切なサイズに切り取ってもらい、次の空きインデックスを覚えて確保タイプを設定する
            freePoolHeadIndex = DivideFreeMemoryBlock(freeIndex, requestCount, type);


            // 見つけたインデックス+先頭管理領域分の位置とサイズでメモリブロックを生成して返す
            return new MemoryBlock(memoryPool, freeIndex + HeadMemoryInfoCount, requestCount);
        }


        /// <summary>
        /// 指定されたメモリブロックを解放します。
        /// </summary>
        /// <param name="memoryBlock">解放するメモリブロック</param>
        public override void Deallocate(MemoryBlock memoryBlock)
        {
            // 指定されたメモリブロックの1つ前のインデックスを取得して有効なインデックスなら
            var returnedPoolIndex = memoryBlock.Offset - HeadMemoryInfoCount;
            var previousPoolIndex = GetPreviousAllocationInfoIndex(returnedPoolIndex);
            if (previousPoolIndex != IndexNotFound)
            {
                // メモリ管理領域を取得して、もし空き領域なら
                GetAllocationInfo(previousPoolIndex, out _, out var type, out var nextFreeIndex);
                if (type == AllocationType.Free)
                {
                    // 領域をマージして終了
                    MergeMemoryBlock(previousPoolIndex, returnedPoolIndex);
                    return;
                }
            }


            // 今回の返却領域の更新をする
            SetAllocationInfo(returnedPoolIndex, memoryBlock.Length, AllocationType.Free, IndexNotFound);


            // 最後の空き領域インデックスを取得するが見つけられなかったら
            var lastFreePoolIndex = GetLastFreeMemoryInfoIndex();
            if (lastFreePoolIndex == IndexNotFound)
            {
                // 今回の返却領域が最後の空き領域となるので空きプール先頭インデックスの更新をして終了
                freePoolHeadIndex = returnedPoolIndex;
                return;
            }


            // 最後の空き領域インデックスの情報を取得して今回の領域にリンクを繋げる
            GetAllocationInfo(lastFreePoolIndex, out var lastAllocCount, out _, out _);
            SetAllocationInfo(lastFreePoolIndex, lastAllocCount, AllocationType.Free, returnedPoolIndex);
        }
        #endregion


        #region Allocation sub function
        /// <summary>
        /// 指定された要素数を収めることの出来る最初のフリーブロックを探します
        /// </summary>
        /// <param name="count">確保したいメモリの要素数</param>
        /// <returns>指定された要素数を収めることの出来る最初のブロックを見つけた場合はそのプールインデックスを返しますが、見つけられなかった場合は IndexNotFound を返します</returns>
        private int FindFirstFreeBlock(int count)
        {
            // 空き要素インデックスが負の値なら
            if (freePoolHeadIndex < 0)
            {
                // 見つけられなかったことを返す
                return IndexNotFound;
            }


            // 最初のメモリ管理情報を取得して収めることの出来るインデックスを見つけるまでループ
            var hitIndex = freePoolHeadIndex;
            GetAllocationInfo(hitIndex, out var poolCount, out var type, out var nextFreeIndex);
            while (poolCount < count)
            {
                // もし次の管理情報が無いなら
                if (nextFreeIndex == IndexNotFound)
                {
                    // 見つけることが出来なかったことを返す
                    return IndexNotFound;
                }


                // 次の管理情報を取得する
                hitIndex = nextFreeIndex;
                GetAllocationInfo(hitIndex, out poolCount, out type, out nextFreeIndex);
            }


            // 見つけたインデックスを返す
            return hitIndex;
        }


        /// <summary>
        /// 指定されたメモリプールインデックスから、指定要素数分の空き領域メモリブロック分割を行います
        /// </summary>
        /// <param name="poolIndex">分割を行う空き領域の管理要素インデックス</param>
        /// <param name="count">分割する容量</param>
        /// <param name="type">分割したメモリブロックに割り当てるタイプ</param>
        /// <returns>分割した次の空き領域を指すインデックスが生成された場合は次の空き領域へのインデックスを返します。空きインデックスがない場合は IndexNotFound を返します。</returns>
        private int DivideFreeMemoryBlock(int poolIndex, int count, AllocationType type)
        {
            // 指定領域のメモリ管理情報を取得する
            GetAllocationInfo(poolIndex, out var freeCount, out var _, out var nextFreeIndex);


            // もし分割後の後方空き領域が必要空き領域未満 または 空きカウントと必要サイズが一致 したのなら
            if ((freeCount - RequireMinimumPoolCount) < count || freeCount == count)
            {
                // そのまま次の空き領域インデックスを返す
                return nextFreeIndex;
            }


            // 分割する要求されたサイズを元に管理情報を更新する
            SetAllocationInfo(poolIndex, count, type, IndexNotFound);


            // 次の新しい空き領域のインデックスを求めて管理情報を更新する
            var newNextFreeIndex = poolIndex + RequireMemoryInfoCount + count;
            var newFreeCount = freeCount - (RequireMemoryInfoCount + count);
            SetAllocationInfo(newNextFreeIndex, newFreeCount, AllocationType.Free, nextFreeIndex);


            // 新しい空き領域インデックスを返す
            return newNextFreeIndex;
        }


        /// <summary>
        /// 指定された2つのプールインデックスのメモリ管理情報を論理的にマージします。
        /// メモリ管理情報は previousPoolIndex 側の管理情報が採用されます。
        /// </summary>
        /// <param name="previousPoolIndex">マージする1つ前のメモリプールインデックス</param>
        /// <param name="currentPoolIndex">マージするメモリプールインデックス</param>
        private void MergeMemoryBlock(int previousPoolIndex, int currentPoolIndex)
        {
            // 2つのメモリ管理情報を取得する
            GetAllocationInfo(previousPoolIndex, out var prevAllocCount, out var prevAllocType, out var prevNextIndex);
            GetAllocationInfo(currentPoolIndex, out var currAllocCount, out var currAllocType, out var currNextIndex);


            // 2つのメモリ容量の合計値（結合後のサイズの管理情報も含む）を求めてから prev 側のメモリ管理情報元に更新する
            var totalAllocCount = prevAllocCount + currAllocCount + RequireMemoryInfoCount * 2;
            SetAllocationInfo(previousPoolIndex, totalAllocCount - RequireMemoryInfoCount, prevAllocType, prevNextIndex);
        }
        #endregion


        #region Exception builder function
        /// <summary>
        /// プールサイズが最低限確保するべきサイズを指定されなかった場合例外をスローします
        /// </summary>
        /// <param name="poolSize">確保するプールサイズ</param>
        /// <exception cref="ArgumentOutOfRangeException">poolSize が RequireMinimumPoolSize 未満です</exception>
        protected void ThrowExceptionIfInvalidPoolSize(int poolSize)
        {
            // RequireMinimumPoolSize 未満なら
            if (poolSize < RequireMinimumPoolSize)
            {
                // 例外を吐く
                throw new ArgumentOutOfRangeException(nameof(poolSize), $"{nameof(poolSize)} が {nameof(RequireMinimumPoolSize)} 未満です");
            }
        }


        /// <summary>
        /// メモリプールの配列参照が null だった場合に例外をスローします
        /// </summary>
        /// <param name="memoryPool">確認する配列</param>
        /// <exception cref="ArgumentNullException">memoryPool が null です</exception>
        /// <exception cref="ArgumentOutOfRangeException">memoryPool の長さが RequireMinimumPoolCount 未満です</exception>
        protected void ThrowExceptionIfInvalidMemoryPool(SrValue[] memoryPool)
        {
            // null の場合は
            if (memoryPool == null)
            {
                // 例外を吐く
                throw new ArgumentNullException(nameof(memoryPool));
            }


            // 長さが RequireMinimumPoolCount 未満なら
            if (memoryPool.Length < RequireMinimumPoolCount)
            {
                // 例外を吐く
                throw new ArgumentOutOfRangeException($"{nameof(memoryPool)} の長さが {nameof(RequireMinimumPoolCount)} 未満です", nameof(memoryPool));
            }
        }
        #endregion
    }
    #endregion



    #region Dynamic MemoryAllocator
    /// <summary>
    /// 直接.NETランタイムからメモリを割り当てるメモリアロケータクラスです
    /// </summary>
    public class DynamicManagedMemoryAllocator : MemoryAllocator
    {
        /// <summary>
        /// メモリの確保をします。確保されるメモリのサイズは size が収まる様に最大8の倍数に調整された確保が行われます。
        /// </summary>
        /// <param name="size">確保するメモリのサイズ</param>
        /// <param name="type">確保するメモリの利用タイプ</param>
        /// <returns>確保したメモリのブロックを返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">確保するサイズに 0 以下の指定は出来ません</exception>
        /// <exception cref="OutOfMemoryException">指定されたサイズのメモリを確保できませんでした</exception>
        /// <exception cref="ArgumentException">メモリ確保の種類に 'Free' の指定は出来ません</exception>
        public override MemoryBlock Allocate(int size, AllocationType type)
        {
            // 事前の例外ハンドリングをしてから、サイズを計算後メモリブロックを生成して返す
            ThrowExceptionIfRequestInvalidSizeOrType(size, type);
            var allocateBlock = ByteSizeToElementCount(size);
            return new MemoryBlock(new SrValue[allocateBlock], 0, allocateBlock);
        }


        /// <summary>
        /// 指定されたメモリブロックを解放します。
        /// </summary>
        /// <param name="memoryBlock">解放するメモリブロック</param>
        public override void Deallocate(MemoryBlock memoryBlock)
        {
            // .NETランタイムへ返却するだけなので、このメモリアロケータは何もしません
        }
    }
    #endregion
}