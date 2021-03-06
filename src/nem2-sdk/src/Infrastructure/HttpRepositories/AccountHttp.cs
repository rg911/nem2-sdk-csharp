﻿// ***********************************************************************
// Assembly         : nem2-sdk
// Author           : kailin
// Created          : 01-15-2018
//
// Last Modified By : kailin
// Last Modified On : 02-01-2018
// ***********************************************************************
// <copyright file="AccountHttp.cs" company="Nem.io">
// Copyright 2018 NEM
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using io.nem2.sdk.Core.Crypto.Chaso.NaCl;
using io.nem2.sdk.Infrastructure.Buffers.Model;
using io.nem2.sdk.Infrastructure.Imported.Api;
using io.nem2.sdk.Model.Accounts;
using io.nem2.sdk.Model.Blockchain;
using io.nem2.sdk.Model.Mosaics;
using io.nem2.sdk.Model.Transactions;

namespace io.nem2.sdk.Infrastructure.HttpRepositories
{
    /// <summary>
    /// Account Http Repository.
    /// </summary>
    /// <seealso cref="io.nem2.sdk.Infrastructure.HttpRepositories.HttpRouter" />
    /// <seealso cref="io.nem2.sdk.Infrastructure.HttpRepositories.IAccountRepository" />
    /// <seealso cref="HttpRouter" />
    /// <seealso cref="IAccountRepository" />
    public class AccountHttp : HttpRouter, IAccountRepository
    {
        /// <summary>
        /// Gets the account routes API.
        /// </summary>
        /// <value>The account routes API.</value>
        private AccountRoutesApi AccountRoutesApi { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountHttp" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <exception cref="ArgumentException">Value cannot be null or empty. - host</exception>
        public AccountHttp(string host) 
            : this(host, new NetworkHttp(host)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountHttp" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="networkHttp">The network HTTP.</param>
        /// <exception cref="ArgumentException">Value cannot be null or empty. - host</exception>
        public AccountHttp(string host, NetworkHttp networkHttp) : base(host, networkHttp)
        {     
            AccountRoutesApi = new AccountRoutesApi(host);
        }

        /// <summary>
        /// Gets the account information.
        /// </summary>
        /// <param name="address">The account.</param>
        /// <returns>IObservable&lt;AccountInfoDTO&gt;.</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<AccountInfo> GetAccountInfo(Address address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountInfoAsync(address.Plain))
                .Select(accountInfo  => new AccountInfo(
                    Address.CreateFromEncoded(accountInfo.Account.Address),
                    accountInfo.Account.AddressHeight,
                    accountInfo.Account.PublicKey,
                    accountInfo.Account.PublicKeyHeight,
                    accountInfo.Account.Importance,
                    accountInfo.Account.ImportanceHeight, 
                    accountInfo.Account.Mosaics.Select(mosaic => new Mosaic(new MosaicId(BitConverter.ToUInt64( mosaic.MosaicId.FromHex().Reverse().ToArray(), 0)), mosaic.Amount)).ToList()));
        }

        /// <summary>
        /// Gets the account information.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>IObservable&lt;AccountInfoDTO&gt;.</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<AccountInfo> GetAccountInfo(PublicAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountInfoAsync(account.PublicKey))
                .Select(accountInfo => new AccountInfo(
                    Address.CreateFromEncoded(accountInfo.Account.Address),
                    accountInfo.Account.AddressHeight,
                    accountInfo.Account.PublicKey,
                    accountInfo.Account.PublicKeyHeight,
                    accountInfo.Account.Importance,
                    accountInfo.Account.ImportanceHeight,
                    accountInfo.Account.Mosaics.Select(
                        mosaic => new Mosaic(new MosaicId(
                                BitConverter.ToUInt64(mosaic.MosaicId.FromHex().Reverse().ToArray(), 0)),
                            mosaic.Amount)
                    ).ToList()));
        }

        /// <summary>
        /// Get account information.
        /// </summary>
        /// <param name="accountIds">The account ids for which account information should be returned.</param>
        /// <returns>An IObservable of a List of AccountInfoDTO</returns>
        /// <exception cref="ArgumentNullException">accountIds</exception>
        public IObservable<List<AccountInfo>> GetAccountsInfo(AddressesDTO accountIds)
        {
            if (accountIds == null) throw new ArgumentNullException(nameof(accountIds));

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountsInfoAsync(accountIds)).Select(e =>                      
            e.Select(accountInfo =>
            {
                return new AccountInfo(
                    Address.CreateFromEncoded(accountInfo.Account.Address),
                    accountInfo.Account.AddressHeight,
                    accountInfo.Account.PublicKey,
                    accountInfo.Account.PublicKeyHeight,
                    accountInfo.Account.Importance,
                    accountInfo.Account.ImportanceHeight,
                    accountInfo.Account.Mosaics.Select(
                        mosaic => new Mosaic(new MosaicId(
                                BitConverter.ToUInt64(mosaic.MosaicId.FromHex().Reverse().ToArray(), 0)),
                            mosaic.Amount)
                    ).ToList());
            }).ToList());
        }

        /// <summary>
        /// Get account information.
        /// </summary>
        /// <param name="accountIds">The account ids for which account information should be returned.</param>
        /// <returns>An IObservable of a List of AccountInfoDTO</returns>
        /// <exception cref="ArgumentNullException">accountIds</exception>
        public IObservable<List<AccountInfo>> GetAccountsInfo(PublicKeysDTO accountIds)
        {
            if (accountIds == null) throw new ArgumentNullException(nameof(accountIds));

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountsInfoAsync(accountIds)).Select(e =>
                e.Select(accountInfo => new AccountInfo(
                    Address.CreateFromEncoded(accountInfo.Account.Address),
                    accountInfo.Account.AddressHeight,
                    accountInfo.Account.PublicKey,
                    accountInfo.Account.PublicKeyHeight,
                    accountInfo.Account.Importance,
                    accountInfo.Account.ImportanceHeight,
                    accountInfo.Account.Mosaics.Select(
                        mosaic => new Mosaic(new MosaicId(
                                BitConverter.ToUInt64(mosaic.MosaicId.FromHex().Reverse().ToArray(), 0)),
                            mosaic.Amount)
                    ).ToList())
                ).ToList());
        }

        /// <summary>
        /// Get multisig account information.
        /// </summary>
        /// <param name="account">The account for which multisig info should be returned.</param>
        /// <returns>An IObservable of type MultisigEntryDTO.</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<MultisigAccountInfo> GetMultisigAccountInfo(PublicAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            IObservable<NetworkType.Types> networkTypeResolve = GetNetworkTypeObservable().Take(1);

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountMultisigAsync(account.PublicKey))
                .Select(entry => new MultisigAccountInfo(
                    new PublicAccount(
                        entry.Multisig.Account, 
                        networkTypeResolve.Wait()),
                        entry.Multisig.MinApproval, 
                        entry.Multisig.MinRemoval,
                        entry.Multisig.Cosignatories.Select(
                              cosig => new PublicAccount(
                                cosig,
                                networkTypeResolve.Wait())
                                ).ToList(),
                        entry.Multisig.MultisigAccounts.Select(
                              cosig => new PublicAccount(
                                cosig,
                                networkTypeResolve.Wait())
                                ).ToList())).Take(1);
        }

        /// <summary>
        /// Get multisig account information.
        /// </summary>
        /// <param name="account">The account for which multisig info should be returned.</param>
        /// <returns>An IObservable of type MultisigEntryDTO.</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<MultisigAccountInfo> GetMultisigAccountInfo(Address account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            IObservable<NetworkType.Types> networkTypeResolve = GetNetworkTypeObservable().Take(1);

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountMultisigAsync(account.Plain)).Select(entry => new MultisigAccountInfo(
                    new PublicAccount(
                        entry.Multisig.Account,
                        networkTypeResolve.Wait()),
                        entry.Multisig.MinApproval,
                        entry.Multisig.MinRemoval,
                        entry.Multisig.Cosignatories.Select(
                            cosig => new PublicAccount(
                                cosig,
                                networkTypeResolve.Wait())
                            ).ToList(),
                        entry.Multisig.MultisigAccounts.Select(
                            cosig => new PublicAccount(
                                cosig,
                                networkTypeResolve.Wait())
                            ).ToList()));
        }

        /// <summary>
        /// Get multisig graph information
        /// </summary>
        /// <param name="account">The account for which multisig graph information should be returned.</param>
        /// <returns>An IObservable list of MultisigAccountGraphInfoDTO</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<MultisigAccountGraphInfo> GetMultisigAccountGraphInfo(PublicAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountMultisigGraphAsync(account.PublicKey))
                .Select(entry =>
                {
                    Dictionary<int, List<MultisigAccountInfo>> graphInfoMap = new Dictionary<int, List<MultisigAccountInfo>>();

                    entry.ForEach(item => graphInfoMap.Add(
                        item.Level,
                        item.MultisigEntries.Select(i => 
                        new MultisigAccountInfo(
                            PublicAccount.CreateFromPublicKey(
                                i.Multisig.Account,
                                GetNetworkTypeObservable().Wait()
                                ),
                            i.Multisig.MinApproval, 
                            i.Multisig.MinRemoval,
                            i.Multisig.Cosignatories.Select(
                                e => PublicAccount.CreateFromPublicKey(
                                    e, GetNetworkTypeObservable().Wait()
                                    )).ToList(), 
                            i.Multisig.MultisigAccounts.Select(
                                o => PublicAccount.CreateFromPublicKey(
                                    o, GetNetworkTypeObservable().Wait()
                                    )).ToList() ) ).ToList()));

                    return new MultisigAccountGraphInfo(graphInfoMap);
                });
        }

        /// <summary>
        /// Get multisig graph information
        /// </summary>
        /// <param name="account">The account for which multisig graph information should be returned.</param>
        /// <returns>An IObservable list of MultisigAccountGraphInfoDTO</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<MultisigAccountGraphInfo> GetMultisigAccountGraphInfo(Address account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            return Observable.FromAsync(async ar => await AccountRoutesApi.GetAccountMultisigGraphAsync(account.Plain))
                .Select(entry =>
                {
                    Dictionary<int, List<MultisigAccountInfo>> graphInfoMap = new Dictionary<int, List<MultisigAccountInfo>>();

                    entry.ForEach(item => graphInfoMap.Add(
                        item.Level,
                        item.MultisigEntries.Select(i =>
                            new MultisigAccountInfo(
                                PublicAccount.CreateFromPublicKey(
                                    i.Multisig.Account,
                                    GetNetworkTypeObservable().Wait()
                                ),
                                i.Multisig.MinApproval,
                                i.Multisig.MinRemoval,
                                i.Multisig.Cosignatories.Select(
                                    e => PublicAccount.CreateFromPublicKey(
                                        e, GetNetworkTypeObservable().Wait()
                                    )).ToList(),
                                i.Multisig.MultisigAccounts.Select(
                                    o => PublicAccount.CreateFromPublicKey(
                                        o, GetNetworkTypeObservable().Wait()
                                    )).ToList())).ToList()));

                    return new MultisigAccountGraphInfo(graphInfoMap);
                });
        }

        /// <summary>
        /// Get incoming transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <returns>IObservable list of type Transaction.</returns>
        /// <exception cref="ArgumentNullException">account</exception>
        public IObservable<List<Transaction>> IncomingTransactions(PublicAccount account)
        {
            return IncomingTransactions(account, new QueryParams(10));
        }

        /// <summary>
        /// Gets the incoming transactions.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="query">The query.</param>
        /// <returns>IObservable&lt;List&lt;Transaction&gt;&gt;.</returns>
        /// <exception cref="ArgumentNullException">
        /// account
        /// or
        /// query
        /// </exception>
        public IObservable<List<Transaction>> IncomingTransactions(PublicAccount account, QueryParams query)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return Observable.FromAsync(async ar => await AccountRoutesApi.IncomingTransactionsAsync(account.PublicKey, query.GetPageSize(), query.GetId()));
        }

        /// <summary>
        /// Gets the outgoing transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <returns>IObservable&lt;List&lt;Transaction&gt;&gt;.</returns>
        public IObservable<List<Transaction>> OutgoingTransactions(PublicAccount account)
        {
            return OutgoingTransactions(account, new QueryParams(10));
        }

        /// <summary>
        /// Get incoming transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <param name="query">The query parameters.</param>
        /// <returns>IObservable list of type Transaction.</returns>
        /// <exception cref="ArgumentNullException">account
        /// or
        /// query</exception>
        public IObservable<List<Transaction>> OutgoingTransactions(PublicAccount account, QueryParams query)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return Observable.FromAsync(async ar => await AccountRoutesApi.OutgoingTransactionsAsync(account.PublicKey, query.GetPageSize(), query.GetId()));
        }

        /// <summary>
        /// Get unconfirmed transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <returns>IObservable list of type Transaction.</returns>
        public IObservable<List<Transaction>> UnconfirmedTransactions(PublicAccount account) 
        {
            return UnconfirmedTransactions(account, new QueryParams(10));
        }

        /// <summary>
        /// Gets the unconfirmed transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <param name="query">The query parameters.</param>
        /// <returns>IObservable&lt;List&lt;Transaction&gt;&gt;.</returns>
        /// <exception cref="ArgumentNullException">account
        /// or
        /// query</exception>
        public IObservable<List<Transaction>> UnconfirmedTransactions(PublicAccount account, QueryParams query)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return Observable.FromAsync(async ar => await AccountRoutesApi.UnconfirmedTransactionsAsync(account.PublicKey, query.GetPageSize(), query.GetId()));
        }

        /// <summary>
        /// Gets the partial transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <returns>IObservable&lt;List&lt;Transaction&gt;&gt;.</returns>
        public IObservable<List<AggregateTransaction>> AggregateBondedTransactions(PublicAccount account) 
        {
            return AggregateBondedTransactions(account, new QueryParams(10));
        }

        /// <summary>
        /// Get partial transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <param name="query">The query parameters.</param>
        /// <returns>IObservable list of type Transaction.</returns>
        /// <exception cref="ArgumentNullException">account
        /// or
        /// query</exception>
        public IObservable<List<AggregateTransaction>> AggregateBondedTransactions(PublicAccount account, QueryParams query)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return Observable.FromAsync(async ar => await AccountRoutesApi.PartialTransactionsAsync(account.PublicKey, query.GetPageSize(), query.GetId()));

        }

        /// <summary>
        /// Get all transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <returns>IObservable list of type Transaction.</returns>
        public IObservable<List<Transaction>> Transactions(PublicAccount account)
        {
            
            return Transactions(account, new QueryParams(10));

        }

        /// <summary>
        /// Get all transactions.
        /// </summary>
        /// <param name="account">The account for which transactions should be returned.</param>
        /// <param name="query">The query parameters.</param>
        /// <returns>IObservable list of type Transaction.</returns>
        /// <exception cref="ArgumentNullException">account
        /// or
        /// query</exception>
        public IObservable<List<Transaction>> Transactions(PublicAccount account, QueryParams query)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (query == null) throw new ArgumentNullException(nameof(query));

            return Observable.FromAsync(async ar => await AccountRoutesApi.TransactionsAsync(account.PublicKey, query.GetPageSize(), query.GetId()));
        }
    }
}
