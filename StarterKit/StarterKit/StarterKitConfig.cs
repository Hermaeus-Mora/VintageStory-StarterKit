using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace StarterKit
{
    /// <summary>
    /// Конфигурация мода.
    /// </summary>
    public sealed class StarterKitConfig
    {
        /// <summary>
        /// API сервера.
        /// </summary>
        private readonly ICoreServerAPI API;

        /// <summary>
        /// Позволенное количество получений для игрока.
        /// </summary>
        public int PermittedReceives;
        /// <summary>
        /// Предметы стартового набора.
        /// </summary>
        public ItemInfo[] Items;

        /// <summary>
        /// Конструктор конфигурации мода.
        /// </summary>
        /// <param name="api">API сервера.</param>
        public StarterKitConfig(ICoreServerAPI api)
        {
            API = api;

            PermittedReceives = 1;
            Items = Array.Empty<ItemInfo>();
        }

        /// <summary>
        /// Загружает конфигурацию из файла.
        /// </summary>
        /// <param name="createIfError">Записать новый файл конфигурации при ошибке.</param>
        public void Load(bool createIfError = false)
        {
            StarterKitConfig? config = null;
            try
            {
                config = API.LoadModConfig<StarterKitConfig>($"{StarterKitModSystem.ModName}.json");
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Failed to load {StarterKitModSystem.ModName} configuration.");
                API.Logger.Error(ex);

                if (!createIfError)
                    return;
            }

            bool needSave = config == null;
            config ??= new StarterKitConfig(API);

            CopyFields(config);
            if (needSave)
                Save();
        }
        /// <summary>
        /// Сохраняет конфигурацию в файл.
        /// </summary>
        public void Save()
        {
            try
            {
                API.StoreModConfig<StarterKitConfig>(this, $"{StarterKitModSystem.ModName}.json");
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Failed to save {StarterKitModSystem.ModName} configuration.");
                API.Logger.Error(ex);
            }
        }

        /// <summary>
        /// Копирует поля экземпляра <paramref name="config"/> в текущий экземпляр.
        /// </summary>
        /// <param name="config">Конфигурация.</param>
        private void CopyFields(StarterKitConfig config)
        {
            if (config == null)
                return;

            PermittedReceives = config.PermittedReceives;
            Items = config.Items;
        }
        /// <summary>
        /// Создаёт стаки предметов стартового набора.
        /// </summary>
        /// <returns>Список стаков или пустой, если возникла ошибка.</returns>
        public List<ItemStack> CreateItems()
        {
            ItemInfo[] items = Items;
            List<ItemStack> stacks = new List<ItemStack>();
            foreach (ItemInfo info in Items)
            {
                if (info.Amount <= 0)
                    continue;

                Item? item;
                try { item = API.World.GetItem(new AssetLocation(info.Code)); }
                catch { return []; }
                if (item == null)
                    return [];

                ItemStack? stack;
                try { stack = new ItemStack(item, info.Amount); }
                catch { return []; }

                stacks.Add(stack);
            }

            return stacks;
        }
    }
}