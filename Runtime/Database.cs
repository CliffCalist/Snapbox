using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteArrow.DataSaving
{
    public class Database
    {
        private readonly Dictionary<string, ISavingMetadata> _metadatas = new();
        private readonly Dictionary<string, object> _cache = new();

        private readonly IDatabaseLogger _logger;
        private readonly IGameLoader _loader;
        private readonly IGameSaver _saver;



        public Database(IGameLoader loader, IGameSaver saver, IDatabaseLogger logger)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _saver = saver ?? throw new ArgumentNullException(nameof(saver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void UpdateMetadata(params ISavingMetadata[] metadataParams)
        {
            var newMetadataDict = metadataParams.ToDictionary(m => m.DataName);

            var keysToRemove = _metadatas.Keys.Where(key => !newMetadataDict.ContainsKey(key)).ToList();
            foreach (var key in keysToRemove)
            {
                _metadatas.Remove(key);
                _cache.Remove(key);
            }

            foreach (var metadata in metadataParams)
            {
                if (!_metadatas.ContainsKey(metadata.DataName))
                {
                    _metadatas.Add(metadata.DataName, metadata);
                    _cache[metadata.DataName] = null;
                }
            }
        }



        public async Task LoadNewDataAsync()
        {
            foreach (var kvp in _metadatas)
            {
                if (!kvp.Value.IsDeleted && (!_cache.ContainsKey(kvp.Key) || _cache[kvp.Key] == null))
                {
                    try
                    {
                        var data = await _loader.LoadAsync(kvp.Value);

                        if (!_cache.ContainsKey(kvp.Key))
                            _cache.Add(kvp.Key, data);
                        else _cache[kvp.Key] = data;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error loading data for key '{kvp.Value.DataName}': {ex.Message}");
                        _cache[kvp.Key] = null;
                    }
                }
            }
        }

        public async Task SaveAllAsync()
        {
            foreach (var kvp in _metadatas)
            {
                if (kvp.Value.IsChanged)
                {
                    try
                    {
                        var data = _cache[kvp.Key];
                        if (data == null && kvp.Value.IsDeleted)
                        {
                            await _saver.DeleteAsync(kvp.Value);
                            kvp.Value.IsChanged = false;
                            _logger.Log($"Data for key '{kvp.Value.DataName}' deleted successfully.");
                        }
                        else if (data != null)
                        {
                            await _saver.SaveAsync(kvp.Value, data);
                            kvp.Value.IsChanged = false;
                            _logger.Log($"Data for key '{kvp.Value.DataName}' saved successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error saving data for key '{kvp.Value.DataName}': {ex.Message}");
                    }
                }
            }
        }



        public T GetData<T>(string key)
        {
            if (!_cache.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key}' not found in cache.");

            return (T)_cache[key];
        }

        public void SetData(string key, object value)
        {
            if (!_cache.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key}' not found in cache.");

            _metadatas[key].IsChanged = true;
            if (value == null)
            {
                _metadatas[key].IsDeleted = true;
                _cache[key] = null;
            }
            else
            {
                _cache[key] = value;
                _metadatas[key].IsDeleted = false;
            }
        }
    }
}