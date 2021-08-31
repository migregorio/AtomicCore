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

        #region ITronTransactionsRest

        [TestMethod()]
        public void CreateTransactionTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetCreateTransactionRestJson result = testRestAPI.CreateTransaction(
                TronTestAccountCollection.TestMain.Address,
                TronTestAccountCollection.TestA.Address,
                1
            );

            Assert.IsTrue(!string.IsNullOrEmpty(result.TxID));
        }

        [TestMethod()]
        public void GetTransactionSignTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetCreateTransactionRestJson createTransaction = testRestAPI.CreateTransaction(
                TronTestAccountCollection.TestMain.Address,
                TronTestAccountCollection.TestA.Address,
                1
            );
            Assert.IsTrue(createTransaction.IsAvailable());

            var result = testRestAPI.GetTransactionSign(TronTestAccountCollection.TestMain.PirvateKey, createTransaction);

            Assert.IsTrue(result.IsAvailable());
        }

        /// <summary>
        /// API ERROR
        /// </summary>
        [TestMethod()]
        public void BroadcastTransactionTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            string from = TronTestAccountCollection.TestMain.Address;
            string to = TronTestAccountCollection.TestA.Address;
            string from_priv = TronTestAccountCollection.TestMain.PirvateKey;

            TronNetCreateTransactionRestJson createTransaction = testRestAPI.CreateTransaction(
                from,
                to,
                1
            );
            Assert.IsTrue(!string.IsNullOrEmpty(createTransaction.TxID));

            TronNetSignedTransactionRestJson signTransaction = testRestAPI.GetTransactionSign(from_priv, createTransaction);

            TronNetResultJson result = testRestAPI.BroadcastTransaction(signTransaction);

            Assert.IsTrue(result.Result);
        }

        #endregion

        #region ITronQueryNetworkRestAPI

        [TestMethod()]
        public void GetBlockByNumTest()
        {
            TronNetBlockJson result = _restAPI.GetBlockByNum(200);

            Assert.IsTrue(!string.IsNullOrEmpty(result.BlockID));
        }

        [TestMethod()]
        public void GetBlockByIdTest()
        {
            TronNetBlockJson result = _restAPI.GetBlockById("0000000001f9f486548b36b7a54732ffc070b4311247cb88999cf7cef5c1bfa2");

            Assert.IsTrue(!string.IsNullOrEmpty(result.BlockID));
        }

        [TestMethod()]
        public void GetBlockByLatestNumTest()
        {
            TronNetBlockListJson result = _restAPI.GetBlockByLatestNum(10);

            Assert.IsTrue(result.Blocks != null);
        }

        [TestMethod()]
        public void GetBlockByLimitNextTest()
        {
            TronNetBlockListJson result = _restAPI.GetBlockByLimitNext(10, 11);

            Assert.IsTrue(result.Blocks != null);
        }

        [TestMethod()]
        public void GetNowBlockTest()
        {
            TronNetBlockDetailsJson result = _restAPI.GetNowBlock();

            Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public void GetTransactionByIDTest()
        {
            string txid = "ca8d10f2b141a3a8d8e31453ff50716258d873c89fd189f6abce92effaa1960d";

            TronNetTransactionRestJson rest_txInfo = _restAPI.GetTransactionByID(txid);

            TronNetContractJson contractJson = rest_txInfo.RawData.Contract.FirstOrDefault();
            Assert.IsNotNull(contractJson);

            string ownerAddress = contractJson.Parameter.Value.GetOwnerTronAddress();
            Assert.IsTrue(!string.IsNullOrEmpty(ownerAddress));

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

        [TestMethod()]
        public void GetTransactionInfoByIdTest()
        {
            //TRX => f337385642d56a981fe8938049e3765e6abcca53ac9412a327b1906df272bdc1
            //TRC10 => c43d19f4517ce1a4c31c66eb4d9b41409caddc3be1cb733e28e941b3220e1b2d
            //TRC20 => f2eb864b3058b708d082b4aecf6573bc5606c741b51cf8870da30dd56e4aae40

            TronNetTransactionInfoJson result = _restAPI.GetTransactionInfoById("f2eb864b3058b708d082b4aecf6573bc5606c741b51cf8870da30dd56e4aae40");

            Assert.IsTrue(!string.IsNullOrEmpty(result.TxID));
        }

        [TestMethod()]
        public void ListNodesTest()
        {
            var result = _restAPI.ListNodes();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetNodeInfoTest()
        {
            TronNetNodeOverviewJson result = _restAPI.GetNodeInfo();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetChainParametersTest()
        {
            TronNetChainParameterOverviewJson result = _restAPI.GetChainParameters();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetBlockBalanceTest()
        {
            TronNetBlockBalanceJson result = _restAPI.GetBlockBalance("0000000001fa75a71a08b6b79ad1043601e4d7a283e51cc3ce36d46cbb1b6b5e", 33191335, true);
            if (!result.IsAvailable())
                Assert.IsTrue(!string.IsNullOrEmpty(result.Error));

            result.Error = null;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            Assert.IsTrue(null != result);
        }

        #endregion

        #region ITronNetTRC10TokenRest

        [TestMethod()]
        public void GetAssetIssueByAccountTest()
        {
            //TestNet
            ////TronNetAssetCollectionJson result = _restAPI.GetAssetIssueByAccount("TXLL4wzNZicjNZDcE9KM987dSaxpffWjkq");
            ////Assert.IsTrue(null != result.AssetIssue && result.AssetIssue.Any());

            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();
            TronNetAssetCollectionJson test_result = testRestAPI.GetAssetIssueByAccount("TEhn1qUkP28puJjeVeo9TK27zu2gJEACin");
            Assert.IsTrue(null != test_result.AssetIssue && test_result.AssetIssue.Any());
        }

        [TestMethod()]
        public void GetAssetIssueByIdTest()
        {
            TronNetAssetInfoJson result = _restAPI.GetAssetIssueById(1000001);

            Assert.IsTrue("1000001".Equals(result.ID));
        }

        [TestMethod()]
        public void GetAssetIssueListTest()
        {
            TronNetAssetCollectionJson result = _restAPI.GetAssetIssueList();

            Assert.IsTrue(null != result.AssetIssue && result.AssetIssue.Any());
        }

        [TestMethod()]
        public void GetPaginatedAssetIssueListTest()
        {
            TronNetAssetCollectionJson result = _restAPI.GetPaginatedAssetIssueList(1, 1);

            Assert.IsTrue(null != result.AssetIssue && result.AssetIssue.Any());
        }

        [TestMethod()]
        public void CreateAssetIssueTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            //create transactin
            TronNetCreateTransactionRestJson createTransactionResult = testRestAPI.CreateAssetIssue(TronTestAccountCollection.TestMain.Address, "HuZiToken", 2, "HZT", 2100000000, 1, 1, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "hu hu hu", "http://www.google.com", 10000, 10000, new TronNetFrozenSupplyJson()
            {
                FrozenAmount = 1,
                FrozenDays = 2
            });
            Assert.IsTrue(createTransactionResult.IsAvailable());

            //sign transaction
            TronNetSignedTransactionRestJson signTransactionResult = testRestAPI.GetTransactionSign(TronTestAccountCollection.TestMain.PirvateKey, createTransactionResult);
            Assert.IsTrue(signTransactionResult.IsAvailable());

            //broadcast transaction
            var result = testRestAPI.BroadcastTransaction(signTransactionResult);
            Assert.IsTrue(result.IsAvailable());
        }

        #endregion
    }
}