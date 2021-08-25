﻿namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Query Network
    /// </summary>
    public interface ITronNetQueryNetworkRest
    {
        /// <summary>
        /// Get Block by Number
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        TronNetBlockJson GetBlockByNum(ulong blockHeight);

        /// <summary>
        /// Get Transaction By Txid
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        TronNetTransactionRestJson GetTransactionByID(string txid);
    }
}
