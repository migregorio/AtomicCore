﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// ICoin Parameters
    /// </summary>
    public interface ICoinParameters
    {
        /// <summary>
        /// Parameters
        /// </summary>
        CoinService.CoinParameters Parameters { get; }
    }
}