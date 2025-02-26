﻿using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// Etherscan的统一返回单个实体对象结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EtherscanSingleResult<T> : EtherscanBaseResult
    {
        /// <summary>
        /// 数据结果
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
