﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.BlockChain.TronNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class TronNetRestTests
    {
        #region Variables

        private readonly TronTestRecord _record;
        private readonly ITronNetRest _restAPI;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public TronNetRestTests()
        {
            _record = TronTestServiceExtension.GetMainRecord();
            _restAPI = _record.TronClient.GetRestAPI();
        }

        #endregion

        #region ITronAddressUtilitiesRestAPI

        [TestMethod()]
        public void ValidateAddressTest()
        {
            var result = _restAPI.ValidateAddress("TEfiVcH2MF43NDXLpxmy6wRpaMxnZuc4iX");

            Assert.IsTrue(result.Result);
        }

        #endregion

        #region ITronQueryNetworkRestAPI

        [TestMethod()]
        public void GetTransactionByIDTest()
        {
            string txid = "ca8d10f2b141a3a8d8e31453ff50716258d873c89fd189f6abce92effaa1960d";

            TronNetTransactionRestJson rest_txInfo = _restAPI.GetTransactionByID(txid);

            TronNetContractJson contractJson = rest_txInfo.RawData.Contract.FirstOrDefault();
            Assert.IsNotNull(contractJson);

            TronNetTriggerSmartContractJson valueJson = contractJson.Parameter.Value.ToContractValue<TronNetTriggerSmartContractJson>();
            Assert.IsNotNull(valueJson);

            string toEthAddress = valueJson.GetToEthAddress();
            Assert.IsTrue("0x10b6bb9e59f3e7b139a3e23c340eabc841817976".Equals(toEthAddress, StringComparison.OrdinalIgnoreCase));

            string toTronAddress = valueJson.GetToTronAddress();
            Assert.IsTrue("TBVaidbMvnXovzHJV7TTxeZ5Tkehxrx5UW".Equals(toTronAddress, StringComparison.OrdinalIgnoreCase));

            ulong amount = valueJson.GetOriginalAmount();
            Assert.IsTrue(amount > 0);

            Assert.IsTrue(!string.IsNullOrEmpty(rest_txInfo.TxID));
        }

        #endregion
    }
}