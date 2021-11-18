﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// Omni scan client interface
    /// </summary>
    public interface IOmniScanClient
    {
        /// <summary>
        /// Returns the balance information for a given address. 
        /// For multiple addresses in a single query use the v2 endpoint
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        OmniAssetCollectionJson GetAddressV1(string address);

        /// <summary>
        /// Returns the balance information for multiple addresses
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Dictionary<string, OmniAssetCollectionJson> GetAddressV2(params string[] address);

        /// <summary>
        /// Returns the balance information and transaction history list for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        OmniAddressDetailsResponse GetAddressDetails(string address);
    }
}
