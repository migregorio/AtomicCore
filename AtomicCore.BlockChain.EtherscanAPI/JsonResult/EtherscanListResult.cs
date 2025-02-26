﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// Etherscan列表列表结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EtherscanListResult<T> : EtherscanBaseResult
        where T : class, new()
    {
        /// <summary>
        /// 列表结果
        /// </summary>
        [JsonProperty("result")]
        public List<T> Result { get; set; }
    }
}
