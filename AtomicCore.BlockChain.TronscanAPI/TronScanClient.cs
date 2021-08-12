﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// TRON SCAN CLIENT
    /// </summary>
    public class TronScanClient : ITronScanClient
    {
        #region Variables

        /// <summary>
        /// tron mainnet
        /// </summary>
        public const string c_tron_main = "https://apilist.tronscan.org";

        /// <summary>
        /// tron shstanet
        /// </summary>
        public const string c_tron_shasta = "https://shastapi.tronscan.org";

        /// <summary>
        /// base url
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// agent url tmp
        /// </summary>
        private readonly string _agentGetTmp;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseUrl">基础URL</param>
        /// <param name="agentGetTmp">代理模版</param>
        public TronScanClient(string baseUrl = c_tron_main, string agentGetTmp = null)
        {
            this._baseUrl = baseUrl;
            this._agentGetTmp = agentGetTmp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 创建Rest Url
        /// </summary>
        /// <param name="actionUrl">接口URL</param>
        /// <returns></returns>
        private string CreateRestUrl(string actionUrl)
        {
            return string.Format(
                "{0}/api/{1}",
                this._baseUrl,
                actionUrl
            );
        }

        /// <summary>
        /// Rest Get Request
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <returns></returns>
        private string RestGet(string url)
        {
            string resp;
            try
            {
                if (string.IsNullOrEmpty(this._agentGetTmp))
                    resp = HttpProtocol.HttpGet(url);
                else
                {
                    string encodeUrl = UrlEncoder.UrlEncode(url);
                    string remoteUrl = string.Format(this._agentGetTmp, encodeUrl);

                    resp = HttpProtocol.HttpGet(remoteUrl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// JSON解析OBJECT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private T ObjectParse<T>(string resp)
            where T : class, new()
        {
            T jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        /// <summary>
        /// JSON解析单模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private TronscanSingleResult<T> SingleParse<T>(string resp)
        {
            TronscanSingleResult<T> jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TronscanSingleResult<T>>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        /// <summary>
        /// JSON解析列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private TronscanListResult<T> ListParse<T>(string resp)
            where T : class, new()
        {
            TronscanListResult<T> jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TronscanListResult<T>>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 1.Block Overview
        /// </summary>
        /// <returns></returns>
        public TronChainOverviewJson BlockOverview()
        {
            string url = this.CreateRestUrl("system/status");
            string resp = this.RestGet(url);
            TronChainOverviewJson jsonResult = ObjectParse<TronChainOverviewJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 2.Get Last Block
        /// </summary>
        /// <returns></returns>
        public TronBlockBasicJson GetLastBlock()
        {
            string url = this.CreateRestUrl("block/latest");
            string resp = this.RestGet(url);
            TronBlockBasicJson jsonResult = ObjectParse<TronBlockBasicJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 3.List all the accounts in the blockchain
        /// only 10,000 accounts are displayed, sorted by TRX balance from high to low
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        public TronChainTopAddressListJson GetChainTopAddress(int start = 0, int limit = 20, string sort = "-balance")
        {
            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");
            if (!string.IsNullOrEmpty(sort))
                paramBuilder.AppendFormat("sort={0}&", sort);

            //create url
            string url = this.CreateRestUrl(string.Format("account/list?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronChainTopAddressListJson jsonResult = ObjectParse<TronChainTopAddressListJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 4.Get Account Assets
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public TronAccountAssetJson GetAccountAssets(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            string url = this.CreateRestUrl(string.Format("account?address={0}", address));
            string resp = this.RestGet(url);
            TronAccountAssetJson jsonResult = ObjectParse<TronAccountAssetJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 5.List the blocks in the blockchain
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        public TronBlockInfoListJson GetLastBlocks(int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-number")
        {
            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");
            if (count)
                paramBuilder.AppendFormat("count=true&");
            if (!string.IsNullOrEmpty(sort))
                paramBuilder.AppendFormat("sort={0}&", sort);

            //create url
            string url = this.CreateRestUrl(string.Format("block?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronBlockInfoListJson jsonResult = ObjectParse<TronBlockInfoListJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 6.List all the blocks produced by the specified SR in the blockchain
        /// </summary>
        /// <param name="srAddress">SR address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        public TronBlockInfoListJson GetSRBlocks(string srAddress, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-number")
        {
            if (string.IsNullOrEmpty(srAddress))
                throw new ArgumentNullException(nameof(srAddress));

            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            paramBuilder.AppendFormat("producer={0}&", srAddress);
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");
            if (count)
                paramBuilder.AppendFormat("count=true&");
            if (!string.IsNullOrEmpty(sort))
                paramBuilder.AppendFormat("sort={0}&", sort);

            //create url
            string url = this.CreateRestUrl(string.Format("block?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronBlockInfoListJson jsonResult = ObjectParse<TronBlockInfoListJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 7.Get a single block's detail
        /// </summary>
        /// <param name="number">block number</param>
        /// <returns></returns>
        public TronBlockDetailsJson GetBlockByNumber(ulong number)
        {
            string url = this.CreateRestUrl(string.Format("block?number={0}", number));
            string resp = this.RestGet(url);

            TronBlockInfoByNumberJson jsonResult = ObjectParse<TronBlockInfoByNumberJson>(resp);
            if (null == jsonResult.Data)
                return null;

            return jsonResult.Data.FirstOrDefault();
        }

        /// <summary>
        /// 8.Get Last Transaction List
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        public TronNormalTransactionListJson GetLastTransactions(int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-timestamp")
        {
            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");
            if (count)
                paramBuilder.AppendFormat("count=true&");
            if (!string.IsNullOrEmpty(sort))
                paramBuilder.AppendFormat("sort={0}&", sort);

            //create url
            string url = this.CreateRestUrl(string.Format("transaction?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronNormalTransactionListJson jsonResult = ObjectParse<TronNormalTransactionListJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 9.List the transactions related to a specified account
        /// </summary>
        /// <param name="address">address: an account(No address specified means all)</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        public TronNormalTransactionListJson GetNormalTransactions(string address = null, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-timestamp")
        {
            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(address))
                paramBuilder.AppendFormat("address={0}&", address);

            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");

            if (null != start_timestamp && start_timestamp > 0UL)
                paramBuilder.AppendFormat("start_timestamp={0}&", start_timestamp);
            if (null != end_timestamp && end_timestamp > 0UL)
                paramBuilder.AppendFormat("end_timestamp={0}&", end_timestamp);

            if (count)
                paramBuilder.AppendFormat("count=true&");
            if (!string.IsNullOrEmpty(sort))
                paramBuilder.AppendFormat("sort={0}&", sort);

            //create url
            string url = this.CreateRestUrl(string.Format("transaction?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronNormalTransactionListJson jsonResult = ObjectParse<TronNormalTransactionListJson>(resp);

            return jsonResult;
        }


        /// <summary>
        /// 39.List the TRC-20 transfers related to a specified account
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="address">an account</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        public TronTRC20TransferEventListJson GetTRC20TransferEvents(string address, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            throw new NotImplementedException();
        }

        /// <summary>
        /// 40.List the internal transactions related to a specified account
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="address">an account</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns></returns>
        public TronInternalTransactionListJson GetInternalTransactions(string address, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(address))
                paramBuilder.AppendFormat("address={0}&", address);
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");

            if (null != start_timestamp && start_timestamp > 0UL)
                paramBuilder.AppendFormat("start_timestamp={0}&", start_timestamp);
            if (null != end_timestamp && end_timestamp > 0UL)
                paramBuilder.AppendFormat("end_timestamp={0}&", end_timestamp);

            //create url
            string url = this.CreateRestUrl(string.Format("internal-transaction?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronInternalTransactionListJson jsonResult = ObjectParse<TronInternalTransactionListJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 41.List the transfers related to a specified TRC10 token(Order by Desc)
        /// ps:only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="issueAddress">token creation address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="name">token name</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns>TRC10 token transfers list</returns>
        public TronTRC10TransactionListJson GetTRC10Transactions(string issueAddress, int start = 0, int limit = 20, string name = null, ulong? start_timestamp = null, ulong? end_timestamp = null)
        {
            if (string.IsNullOrEmpty(issueAddress))
                throw new ArgumentNullException(nameof(issueAddress));

            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(issueAddress))
                paramBuilder.AppendFormat("issueAddress={0}&", issueAddress);
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");
            if (!string.IsNullOrEmpty(name))
                paramBuilder.AppendFormat("name={0}&", name);
            if (null != start_timestamp && start_timestamp > 0UL)
                paramBuilder.AppendFormat("start_timestamp={0}&", start_timestamp);
            if (null != end_timestamp && end_timestamp > 0UL)
                paramBuilder.AppendFormat("end_timestamp={0}&", end_timestamp);

            //create url
            string url = this.CreateRestUrl(string.Format("asset/transfer?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronTRC10TransactionListJson jsonResult = ObjectParse<TronTRC10TransactionListJson>(resp);

            return jsonResult;
        }

        /// <summary>
        /// 42.List the transfers related to a specified TRC20 token
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="contractAddress">contract address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns>TRC20 token transfers list</returns>
        public TronTRC20TransactionListJson GetTRC20Transactions(string contractAddress, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null)
        {
            //Params Builder
            StringBuilder paramBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(contractAddress))
                paramBuilder.AppendFormat("contract_address={0}&", contractAddress);
            if (start > -1)
                paramBuilder.AppendFormat("start={0}&", start);
            else
                paramBuilder.Append("start=0&");
            if (limit > 0)
                paramBuilder.AppendFormat("limit={0}&", limit);
            else
                paramBuilder.Append("limit=20&");
            if (null != start_timestamp && start_timestamp > 0UL)
                paramBuilder.AppendFormat("start_timestamp={0}&", start_timestamp);
            if (null != end_timestamp && end_timestamp > 0UL)
                paramBuilder.AppendFormat("end_timestamp={0}&", end_timestamp);

            //create url
            string url = this.CreateRestUrl(string.Format("token_trc20/transfers?{0}", paramBuilder.Remove(paramBuilder.Length - 1, 1).ToString()));

            //http get
            string resp = this.RestGet(url);

            //json parse
            TronTRC20TransactionListJson jsonResult = ObjectParse<TronTRC20TransactionListJson>(resp);

            return jsonResult;
        }

        #endregion
    }
}
