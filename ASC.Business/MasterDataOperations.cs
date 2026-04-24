using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.Model.BaseTypes;
using ASC.Model.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business
{
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        public MasterDataOperations(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
        public async Task<List<MasterDataKey>> GetAllMasterKeysAsync()
        {
            var masterkeys = await _unitOfWork.Repository<MasterDataKey>().FindAllAsync();
            return masterkeys.ToList();
        }
        public async Task<List<MasterDataKey>> GetMasterKeyByNameAsync(string name)
        {
            var masterkeys = await _unitOfWork.Repository<MasterDataKey>().FindAllByPartitionKeyAsync(name);
            return masterkeys.ToList();
        }
        public async Task<bool> InsertMasterKeyAsync(MasterDataKey masterkey)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Repository<MasterDataKey>().AddAsync(masterkey);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key)
        {
            try
            {
                var masterkeys = await _unitOfWork.Repository<MasterDataValue>().FindAllByPartitionKeyAsync(key);
                return masterkeys.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
        public async Task<MasterDataValue> GetMasterValueByNameAsync(string key, string name)
        {
            var masterValues = await _unitOfWork.Repository<MasterDataValue>().
            FindAsync(key, name);
            return masterValues;
        }
        public async Task<bool> InsertMasterValueAsync(MasterDataValue value)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Repository<MasterDataValue>().AddAsync(value);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<bool> UpdateMasterKeyAsync(string orginalPartitionKey, MasterDataKey key)
        {
            using (_unitOfWork)
            {
                var masterkey = await _unitOfWork.Repository<MasterDataKey>().
                FindAsync(orginalPartitionKey, key.RowKey);
                masterkey.IsActive = key.IsActive;
                masterkey.IsDeleted = key.IsDeleted;
                masterkey.Name = key.Name;
                _unitOfWork.Repository<MasterDataKey>().Update(masterkey);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<bool> UpdateMasterValueAsync(string originalPartitionkey, string originalRowkey, MasterDataValue value)
        { 
            using (_unitOfWork)
            {
                var masterValue = await _unitOfWork.Repository<MasterDataValue>().
                FindAsync(originalPartitionkey, originalRowkey);
                masterValue.IsActive = value.IsActive;
                masterValue.IsDeleted = value.IsDeleted;
                masterValue.Name = value.Name;
                _unitOfWork.Repository<MasterDataValue>().Update(masterValue);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<List<MasterDataValue>> GetAllMasterValuesAsync()
        { 
            var masterValues = await _unitOfWork.Repository<MasterDataValue>().
            FindAllAsync();
            return masterValues.ToList();
        }
        public async Task<bool> UploadBulkMasterData(List<MasterDataValue> values)
        { 
            using (_unitOfWork)
            {
                foreach (var value in values)
                {
                    // Find, if null insert Masterkey
                    var masterkey = await GetMasterKeyByNameAsync(value.PartitionKey);
                    if (!masterkey.Any())
                    {
                        await _unitOfWork.Repository<MasterDataKey>().AddAsync(new MasterDataKey()
                        {
                            Name = value.PartitionKey,
                            RowKey = Guid.NewGuid().ToString(),
                            PartitionKey = value.PartitionKey
                        });
                    }
                    // Find, if null Insert MasterValue
                    var masterValuesByKey = await GetAllMasterValuesByKeyAsync(value.PartitionKey);
                    var masterValue = masterValuesByKey.FirstOrDefault(p => p.Name == value.Name);
                    if (masterValue == null)
                    { 
                        await _unitOfWork.Repository<MasterDataValue>().AddAsync(value);
                    }
                    else
                    { 
                        masterValue.IsActive = value.IsActive;
                        masterValue.IsDeleted = value.IsDeleted;
                        masterValue.Name = value.Name;
                        _unitOfWork.Repository<MasterDataValue>().Update(masterValue);
                    }
                }
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
    }
}

