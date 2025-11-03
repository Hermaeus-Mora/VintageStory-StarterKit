using StarterKit.Commands;
using StarterKit.Core;
using StarterKit.Localization;
using StarterKit.Tweaks.ItemStack.Attributes;
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
        private readonly ICoreServerAPI? API;

        /// <summary>
        /// Указывает, нужно ли сбрасывать счётчик полученных наборов после смерти.
        /// </summary>
        public bool ResetOnDeath;
        /// <summary>
        /// Позволенное количество получений для игрока.
        /// </summary>
        public int PermittedReceives;
        /// <summary>
        /// Предметы стартового набора.
        /// </summary>
        public StackInfo?[] Items;

        /// <summary>
        /// Конструктор конфигурации мода.
        /// </summary>
        public StarterKitConfig()
        {
            ResetOnDeath = false;
            PermittedReceives = 1;
            Items = Array.Empty<StackInfo?>();
        }
        /// <summary>
        /// Конструктор конфигурации мода.
        /// </summary>
        /// <param name="api">API сервера.</param>
        internal StarterKitConfig(ICoreServerAPI api) : this()
        {
            API = api;
            API.Event.PlayerDeath += OnPlayerDeath;
        }

        /// <summary>
        /// Загружает конфигурацию из файла.
        /// </summary>
        /// <param name="createIfError">Записать новый файл конфигурации при ошибке.</param>
        public void Load(bool createIfError = false)
        {
            // Проверка корректности
            if (API == null)
                return;

            // Чтение файла конфигурации
            StarterKitConfig? config = null;
            try
            {
                config = API.LoadModConfig<StarterKitConfig>($"{StarterKitModSystem.ModName}.json");
            }
            catch (Exception ex)
            {
                API.Logger.Error(Localizer.Get("IO.ConfigReadException"));
                API.Logger.Error(ex);

                if (!createIfError)
                    return;
            }

            // Создание нового экземпляра, если необходимо
            bool needSave = config == null;
            config ??= new StarterKitConfig(API);

            // Копирование полей и сохранение
            CopyFields(config);
            if (needSave)
                Save();
        }
        /// <summary>
        /// Сохраняет конфигурацию в файл.
        /// </summary>
        public void Save()
        {
            // Проверка корректности
            if (API == null)
                return;

            try
            {
                API.StoreModConfig<StarterKitConfig>(this, $"{StarterKitModSystem.ModName}.json");
            }
            catch (Exception ex)
            {
                API.Logger.Error(Localizer.Get("IO.ConfigWriteException"));
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

            ResetOnDeath = config.ResetOnDeath;
            PermittedReceives = config.PermittedReceives;
            Items = config.Items ?? Array.Empty<StackInfo?>();
        }
        /// <summary>
        /// Создаёт стаки предметов стартового набора.
        /// </summary>
        /// <returns>Список стаков или пустой, если возникла ошибка.</returns>
        public List<ItemStack> CreateItems()
        {
            // Проверка корректности
            if (API == null)
                return [];

            // Копирование ссылки на информацию о стаках
            StackInfo?[] items = Items;

            // Объявление игровых стаков
            List<ItemStack> stacks = new List<ItemStack>();

            // Формирование стаков
            foreach (StackInfo? info in Items)
            {
                // Игнорирование пустых стаков
                if (info == null)
                    continue;
                if (info.Amount <= 0)
                    continue;

                // Получение предмета по коду
                Item? item;
                try
                {
                    item = API.World.GetItem(new AssetLocation(info.Code));
                }
                catch (Exception ex)
                {
                    API.Logger.Error(Localizer.Get("Creator.CreateItemException", info.Code ?? string.Empty));
                    API.Logger.Error(ex);
                    return [];
                }
                if (item == null)
                {
                    API.Logger.Error(Localizer.Get("Creator.ItemCodeNotFound", info.Code ?? string.Empty));
                    return [];
                }

                // Формирование стака
                ItemStack? stack;
                try
                {
                    stack = new ItemStack(item, info.Amount);
                }
                catch
                {
                    API.Logger.Error(Localizer.Get("Creator.CreateStackException", info.Code!, info.Amount));
                    return [];
                }

                // Запись атрибутов
                if (info.Attributes != null && info.Attributes.Length > 0)
                {
                    foreach (StackAttribute attribute in info.Attributes)
                    {
                        // Ошибка при пустом ключе
                        if (string.IsNullOrWhiteSpace(attribute.Key))
                        {
                            API.Logger.Error(Localizer.Get("Attributes.EmptyKey"));
                            return [];
                        }    

                        // Ошибка при пустом или некорректном значении
                        object? value = attribute.GetConverted();
                        if (value == null)
                        {
                            API.Logger.Error(Localizer.Get("Attributes.InvalidValueOrType", attribute.Key));
                            return [];
                        }

                        // Установка значения
                        if (!AttributeSetter.Set(stack, attribute.Key, value, API))
                            return [];
                    }
                }

                // Добавление стака
                stacks.Add(stack);
            }

            // Возврат
            return stacks;
        }

        /// <summary>
        /// Обработчик события смерти игрока.
        /// </summary>
        /// <param name="byPlayer">Погибший игрок.</param>
        /// <param name="damageSource">Источник урона.</param>
        private void OnPlayerDeath(IServerPlayer byPlayer, DamageSource damageSource)
        {
            // Проверка корректности
            if (API == null || byPlayer == null)
                return;

            // Сброс счётчика полученных наборов
            if (ResetOnDeath)
                byPlayer.ServerData.CustomPlayerData.Remove(StarterKitCommand.ReceivesKey);
        }
    }
}